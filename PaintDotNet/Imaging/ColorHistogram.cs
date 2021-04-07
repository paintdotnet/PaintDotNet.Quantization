/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using PaintDotNet.Functional;
using PaintDotNet.Rendering;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace PaintDotNet.Imaging
{
    public static class ColorHistogram
    {
        public static unsafe ColorHistogram<TPixel> Create<TPixel>(IBitmapSource<TPixel> source)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>, IEquatable<TPixel>
        {
            return Create<TPixel>(source, CancellationToken.None);
        }

        public static unsafe ColorHistogram<TPixel> Create<TPixel>(IBitmapSource<TPixel> source, CancellationToken cancelToken)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>, IEquatable<TPixel>
        {
            return Create<TPixel, TPixel, TruePredicate<TPixel>, IdentitySelector<TPixel>>(source, default, default, cancelToken);
        }

        private struct TruePredicate<T>
            : IFunc<T, bool>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Invoke(T value)
            {
                return true;
            }
        }

        private struct IdentitySelector<T>
            : IFunc<T, T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Invoke(T value)
            {
                return value;
            }
        }

        public static unsafe ColorHistogram<ColorBgr24> CreateOpaque(IBitmapSource<ColorBgra32> source)
        {
            return CreateOpaque(source, CancellationToken.None);
        }

        public static unsafe ColorHistogram<ColorBgr24> CreateOpaque(IBitmapSource<ColorBgra32> source, CancellationToken cancelToken)
        {
            return Create<ColorBgra32, ColorBgr24, OnlyOpaqueColorsPredicate, Bgr24FromBgra32Selector>(source, default, default, cancelToken);
        }

        private struct OnlyOpaqueColorsPredicate
            : IFunc<ColorBgra32, bool>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Invoke(ColorBgra32 value)
            {
                return value.A == 255;
            }
        }

        private struct Bgr24FromBgra32Selector
            : IFunc<ColorBgra32, ColorBgr24>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public unsafe ColorBgr24 Invoke(ColorBgra32 value)
            {
                return *(ColorBgr24*)(&value);
            }
        }

        private static unsafe ColorHistogram<TPixelResult> Create<TPixel, TPixelResult, TPredicate, TSelector>(
            IBitmapSource<TPixel> source,
            TPredicate predicate,
            TSelector selector,
            CancellationToken cancelToken)
            where TPixel : unmanaged, INaturalPixelInfo<TPixel>
            where TPixelResult : unmanaged, INaturalPixelInfo, IEquatable<TPixelResult>
            where TPredicate : IFunc<TPixel, bool>
            where TSelector : IFunc<TPixel, TPixelResult>
        {
            cancelToken.ThrowIfCancellationRequested();

            SizeInt32 sourceSize = source.Size;

            using ThreadLocal<IBitmapLock<TPixel>> perThreadRowBuffer = new ThreadLocal<IBitmapLock<TPixel>>(
                delegate ()
                {
                    using IBitmap<TPixel> rowBuffer = ImagingFactory.Instance.CreateBitmap<TPixel>(sourceSize.Width, 1);
                    return rowBuffer.Lock(BitmapLockOptions.ReadWrite);
                },
                true);

            // build partials
            ConcurrentQueue<ColorHistogramBuilder<TPixelResult>> histogramBuilderPool = new ConcurrentQueue<ColorHistogramBuilder<TPixelResult>>();
            Parallel.For(
                0,
                sourceSize.height,
                delegate (int y)
                {
                    cancelToken.ThrowIfCancellationRequested();

                    if (!histogramBuilderPool.TryDequeue(out ColorHistogramBuilder<TPixelResult> histogramBuilder))
                    {
                        histogramBuilder = new ColorHistogramBuilder<TPixelResult>();
                    }

                    IBitmapLock<TPixel> rowBufferLock = perThreadRowBuffer.Value;
                    cancelToken.ThrowIfCancellationRequested();

                    source.CopyPixels(new RectInt32(0, y, sourceSize.Width, 1), rowBufferLock);
                    cancelToken.ThrowIfCancellationRequested();

                    TPixel* pRowBuffer = rowBufferLock.Buffer;
                    for (int x = 0; x < sourceSize.width; ++x)
                    {
                        TPixel color = pRowBuffer[x];
                        if (predicate.Invoke(color))
                        {
                            TPixelResult colorResult = selector.Invoke(color);
                            histogramBuilder.AddColor(colorResult);
                        }
                    }

                    histogramBuilderPool.Enqueue(histogramBuilder);
                });

            cancelToken.ThrowIfCancellationRequested();

            // merge (parallel)
            ColorHistogramBuilder<TPixelResult> finalHistogramBuilder;
            Queue<Task<ColorHistogramBuilder<TPixelResult>>> mergeTasks = new Queue<Task<ColorHistogramBuilder<TPixelResult>>>();
            while (histogramBuilderPool.Count > 0)
            {
                cancelToken.ThrowIfCancellationRequested();

                histogramBuilderPool.TryDequeue(out ColorHistogramBuilder<TPixelResult> partialHistogram1);
                histogramBuilderPool.TryDequeue(out ColorHistogramBuilder<TPixelResult> partialHistogram2);

                if (partialHistogram1 != null && partialHistogram2 != null)
                {
                    Task<ColorHistogramBuilder<TPixelResult>> mergeTask = Task.Run(
                        delegate()
                        {
                            cancelToken.ThrowIfCancellationRequested();
                            return UnionIntoLarger(partialHistogram1, partialHistogram2);
                        });

                    mergeTasks.Enqueue(mergeTask);

                }
                else if (partialHistogram1 != null)
                {
                    mergeTasks.Enqueue(Task.FromResult(partialHistogram1));
                }
            }

            while (mergeTasks.Count > 1)
            {
                mergeTasks.TryDequeue(out Task<ColorHistogramBuilder<TPixelResult>> mergeTask1);
                mergeTasks.TryDequeue(out Task<ColorHistogramBuilder<TPixelResult>> mergeTask2);

                Task<ColorHistogramBuilder<TPixelResult>> mergeTask = Task.WhenAll(mergeTask1, mergeTask2)
                    .ContinueWith((Task<ColorHistogramBuilder<TPixelResult>[]> mergedTask) =>
                    {
                        cancelToken.ThrowIfCancellationRequested();
                        return UnionIntoLarger(mergedTask.Result[0], mergedTask.Result[1]);
                    });

                mergeTasks.Enqueue(mergeTask);
            }

            Task<ColorHistogramBuilder<TPixelResult>> finalHistogramTask = mergeTasks.Single();
            finalHistogramBuilder = finalHistogramTask.Result;

            cancelToken.ThrowIfCancellationRequested();

            ColorHistogram<TPixelResult> histogram = finalHistogramBuilder.Build();
            cancelToken.ThrowIfCancellationRequested();

            return histogram;
        }

        private static ColorHistogramBuilder<TPixel> UnionIntoLarger<TPixel>(ColorHistogramBuilder<TPixel> builder1, ColorHistogramBuilder<TPixel> builder2)
            where TPixel : unmanaged, INaturalPixelInfo, IEquatable<TPixel>
        {
            if (builder1.Count < builder2.Count)
            {
                return UnionIntoLarger(builder2, builder1);
            }

            builder1.UnionWith(builder2);
            return builder1;
        }
    }
}

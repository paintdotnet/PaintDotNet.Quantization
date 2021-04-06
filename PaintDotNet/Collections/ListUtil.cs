/////////////////////////////////////////////////////////////////////////////////
// paint.net                                                                   //
// Copyright (C) dotPDN LLC, Rick Brewster, and contributors.                  //
// All Rights Reserved.                                                        //
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PaintDotNet.Collections
{
    public static class ListUtil
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SwapElements<T, TList>(TList a, int i, int j)
            where TList : IList<T>
        {
            if (i != j)
            {
                T local = a[i];
                a[i] = a[j];
                a[j] = local;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(IList<T> list)
        {
            Sort<T, IList<T>>(list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T, TList>(TList list)
            where TList : IList<T>
        {
            Sort<T, TList, Comparer<T>>(list, 0, list.Count, Comparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(IList<T> list, IComparer<T> comparer)
        {
            Sort<T, IList<T>, IComparer<T>>(list, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(IList<T> list, Comparison<T> comparer)
        {
            Sort<T, IList<T>, ComparisonWrapper<T>>(list, new ComparisonWrapper<T>(comparer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T, TList, TComparer>(TList list, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            Sort<T, TList, TComparer>(list, 0, list.Count, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(IList<T> list, int startIndex, int length, IComparer<T> comparer)
        {
            Sort<T, IList<T>, IComparer<T>>(list, startIndex, length, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Sort<T>(IList<T> list, int startIndex, int length, Comparison<T> comparison)
        {
            Sort<T, IList<T>, ComparisonWrapper<T>>(list, startIndex, length, new ComparisonWrapper<T>(comparison));
        }

        public static void Sort<T, TList, TComparer>(TList list, int startIndex, int length, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            /*Validate.Begin()
                    .IsNotNullIfRefType(ref list, nameof(list))
                    .IsNotNullIfRefType(ref comparer, nameof(comparer))
                    .Check()
                    .IsNotNegative(startIndex, nameof(startIndex))
                    .IsNotNegative(length, nameof(length))
                    .IsRangeValid(list.Count, startIndex, length, nameof(list))
                    .Check();*/

            if (length == 0 || length == 1)
            {
                return;
            }

            SortImpl<T, TList, TComparer>(list, startIndex, length, comparer);
        }

        private static void SortImpl<T, TList, TComparer>(TList list, int startIndex, int length, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            AlgorithmsWithComparer<T, TList, TComparer>.IntrospectiveSort(list, startIndex, length, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortParallel<T>(IList<T> list)
        {
            SortParallel<T, IList<T>, Comparer<T>>(list, 0, list.Count, Comparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortParallel<T, TList>(TList list)
            where TList : IList<T>
        {
            SortParallel<T, TList, Comparer<T>>(list, 0, list.Count, Comparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortParallel<T>(IList<T> list, IComparer<T> comparer)
        {
            SortParallel<T, IList<T>, IComparer<T>>(list, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortParallel<T>(IList<T> list, Comparison<T> comparer)
        {
            SortParallel<T, IList<T>, ComparisonWrapper<T>>(list, new ComparisonWrapper<T>(comparer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortParallel<T, TList, TComparer>(TList list, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            SortParallel<T, TList, TComparer>(list, 0, list.Count, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortParallel<T>(IList<T> list, int startIndex, int length, IComparer<T> comparer)
        {
            SortParallel<T, IList<T>, IComparer<T>>(list, startIndex, length, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SortParallel<T>(IList<T> list, int startIndex, int length, Comparison<T> comparison)
        {
            SortParallel<T, IList<T>, ComparisonWrapper<T>>(list, startIndex, length, new ComparisonWrapper<T>(comparison));
        }

        public static void SortParallel<T, TList, TComparer>(TList list, int startIndex, int length, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            /*Validate.Begin()
                    .IsNotNullIfRefType(ref list, nameof(list))
                    .IsNotNullIfRefType(ref comparer, nameof(comparer))
                    .Check()
                    .IsNotNegative(startIndex, nameof(startIndex))
                    .IsNotNegative(length, nameof(length))
                    .IsRangeValid(list.Count, startIndex, length, nameof(list))
                    .Check();*/

            if (length == 0 || length == 1)
            {
                return;
            }

            try
            {
                SortParallelImpl<T, TList, TComparer>(list, startIndex, length, comparer);
            }
            catch (AggregateException aggEx)
            {
                if (aggEx.IsOnly<OperationCanceledException>())
                {
                    throw new OperationCanceledException(null, aggEx);
                }
                else if (aggEx.IsOnly<OutOfMemoryException>())
                {
                    throw new OutOfMemoryException(null, aggEx);
                }
                else
                {
                    throw;
                }
            }
        }

        private static void SortParallelImpl<T, TList, TComparer>(TList list, int startIndex, int length, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            // Derived from: https://stackoverflow.com/a/1897484/1191082
            // via https://web.archive.org/web/20120201062954/http://www.darkside.co.za/archive/2008/03/14/microsoft-parallel-extensions-.net-framework.aspx
            const int sequentialThreshold = 2048;
            int endIndex = startIndex + length - 1;
            if (endIndex > startIndex)
            {
                if (length < sequentialThreshold)
                {
                    SortImpl<T, TList, TComparer>(list, startIndex, length, comparer);
                }
                else
                {
                    int pivotIndex = AlgorithmsWithComparer<T, TList, TComparer>.PickPivotAndPartition(list, startIndex, endIndex, comparer);
                    Parallel.Invoke(
                        () => SortParallelImpl<T, TList, TComparer>(list, startIndex, (pivotIndex - 1) - startIndex + 1, comparer),
                        () => SortParallelImpl<T, TList, TComparer>(list, pivotIndex + 1, endIndex - (pivotIndex + 1) + 1, comparer));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort<T>(IList<T> list)
        {
            QuickSort<T, IList<T>>(list);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort<T, TList>(TList list)
            where TList : IList<T>
        {
            QuickSort<T, TList, Comparer<T>>(list, 0, list.Count, Comparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort<T>(IList<T> list, IComparer<T> comparer)
        {
            QuickSort<T, IList<T>, IComparer<T>>(list, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort<T>(IList<T> list, Comparison<T> comparer)
        {
            QuickSort<T, IList<T>, ComparisonWrapper<T>>(list, new ComparisonWrapper<T>(comparer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort<T, TList, TComparer>(TList list, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            QuickSort<T, TList, TComparer>(list, 0, list.Count, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort<T>(IList<T> list, int startIndex, int length, IComparer<T> comparer)
        {
            QuickSort<T, IList<T>, IComparer<T>>(list, startIndex, length, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void QuickSort<T>(IList<T> list, int startIndex, int length, Comparison<T> comparer)
        {
            QuickSort<T, IList<T>, ComparisonWrapper<T>>(list, startIndex, length, new ComparisonWrapper<T>(comparer));
        }

        public static void QuickSort<T, TList, TComparer>(TList list, int startIndex, int length, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            /*Validate.Begin()
                    .IsNotNullIfRefType(ref list, nameof(list))
                    .IsNotNullIfRefType(ref comparer, nameof(comparer))
                    .Check()
                    .IsNotNegative(startIndex, nameof(startIndex))
                    .IsNotNegative(length, nameof(length))
                    .IsRangeValid(list.Count, startIndex, length, nameof(list))
                    .Check();*/

            if (length == 0 || length == 1)
            {
                return;
            }

            AlgorithmsWithComparer<T, TList, TComparer>.QuickSortImpl(list, startIndex, startIndex + length - 1, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(IList<T> array, T item)
        {
            return AlgorithmsWithComparer<T, IList<T>, Comparer<T>>.InternalBinarySearch(array, 0, array.Count, item, Comparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T, TList>(TList array, T item)
            where TList : IList<T>
        {
            return AlgorithmsWithComparer<T, TList, Comparer<T>>.InternalBinarySearch(array, 0, array.Count, item, Comparer<T>.Default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(IList<T> array, T item, IComparer<T> comparer)
        {
            return AlgorithmsWithComparer<T, IList<T>, IComparer<T>>.InternalBinarySearch(array, 0, array.Count, item, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(IList<T> array, T item, Comparison<T> comparer)
        {
            return AlgorithmsWithComparer<T, IList<T>, ComparisonWrapper<T>>.InternalBinarySearch(array, 0, array.Count, item, new ComparisonWrapper<T>(comparer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T, TList, TComparer>(TList array, T item, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            return AlgorithmsWithComparer<T, TList, TComparer>.InternalBinarySearch(array, 0, array.Count, item, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(IList<T> array, int index, int count, T item, IComparer<T> comparer)
        {
            return AlgorithmsWithComparer<T, IList<T>, IComparer<T>>.InternalBinarySearch(array, index, count, item, comparer);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T>(IList<T> array, int index, int count, T item, Comparison<T> comparer)
        {
            return AlgorithmsWithComparer<T, IList<T>, ComparisonWrapper<T>>.InternalBinarySearch(array, index, count, item, new ComparisonWrapper<T>(comparer));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int BinarySearch<T, TList, TComparer>(TList array, int index, int count, T item, TComparer comparer)
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            return AlgorithmsWithComparer<T, TList, TComparer>.InternalBinarySearch(array, index, count, item, comparer);
        }

        private static class AlgorithmsWithComparer<T, TList, TComparer>
            where TList : IList<T>
            where TComparer : IComparer<T>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SwapIfGreaterWithItems(TList keys, TComparer comparer, int a, int b)
            {
                if ((a != b) && (comparer.Compare(keys[a], keys[b]) > 0))
                {
                    T local = keys[a];
                    keys[a] = keys[b];
                    keys[b] = local;
                }
            }

            public static void QuickSortImpl(TList keys, int left, int right, TComparer comparer)
            {
                do
                {
                    int lo = left;
                    int hi = right;
                    int mid = lo + ((hi - lo) >> 1);

                    SwapIfGreaterWithItems(keys, comparer, lo, mid);
                    SwapIfGreaterWithItems(keys, comparer, lo, hi);
                    SwapIfGreaterWithItems(keys, comparer, mid, hi);

                    T y = keys[mid];
                    do
                    {
                        while (comparer.Compare(keys[lo], y) < 0)
                        {
                            lo++;
                        }

                        while (comparer.Compare(y, keys[hi]) < 0)
                        {
                            hi--;
                        }

                        if (lo > hi)
                        {
                            break;
                        }

                        if (lo < hi)
                        {
                            T local2 = keys[lo];
                            keys[lo] = keys[hi];
                            keys[hi] = local2;
                        }

                        lo++;
                        hi--;
                    } while (lo <= hi);

                    if ((hi - left) <= (right - lo))
                    {
                        if (left < hi)
                        {
                            QuickSortImpl(keys, left, hi, comparer);
                        }

                        left = lo;
                    }
                    else
                    {
                        if (lo < right)
                        {
                            QuickSortImpl(keys, lo, right, comparer);
                        }

                        right = hi;
                    }
                } while (left < right);
            }

            public static void IntrospectiveSort(TList keys, int startIndex, int length, TComparer comparer)
            {
                int depthLimit = 2 * Int32Util.Log2Floor(keys.Count);
                IntrospectiveSort(keys, startIndex, (startIndex + length) - 1, depthLimit, comparer);
            }

            public static void IntrospectiveSort(TList keys, int lo, int hi, int depthLimit, TComparer comparer)
            {
                while (hi > lo)
                {
                    int num = (hi - lo) + 1;
                    if (num <= 16)
                    {
                        switch (num)
                        {
                            case 1:
                                return;

                            case 2:
                                SwapIfGreater(keys, comparer, lo, hi);
                                return;

                            case 3:
                                SwapIfGreater(keys, comparer, lo, hi - 1);
                                SwapIfGreater(keys, comparer, lo, hi);
                                SwapIfGreater(keys, comparer, hi - 1, hi);
                                return;

                            default:
                                InsertionSort(keys, lo, hi, comparer);
                                return;
                        }
                    }

                    if (depthLimit == 0)
                    {
                        Heapsort(keys, lo, hi, comparer);
                        return;
                    }

                    depthLimit--;
                    int num2 = PickPivotAndPartition(keys, lo, hi, comparer);
                    IntrospectiveSort(keys, num2 + 1, hi, depthLimit, comparer);
                    hi = num2 - 1;
                }
            }

            public static void Heapsort(TList keys, int lo, int hi, TComparer comparer)
            {
                int n = (hi - lo) + 1;

                for (int i = n >> 1; i >= 1; i--)
                {
                    DownHeap(keys, i, n, lo, comparer);
                }

                for (int j = n; j > 1; j--)
                {
                    SwapElements<T, TList>(keys, lo, (lo + j) - 1);
                    DownHeap(keys, 1, j - 1, lo, comparer);
                }
            }

            public static void DownHeap(TList keys, int i, int n, int lo, TComparer comparer)
            {
                T x = keys[(lo + i) - 1];

                while (i <= (n >> 1))
                {
                    int num = 2 * i;

                    if ((num < n) && (comparer.Compare(keys[(lo + num) - 1], keys[lo + num]) < 0))
                    {
                        num++;
                    }

                    if (comparer.Compare(x, keys[(lo + num) - 1]) >= 0)
                    {
                        break;
                    }

                    keys[(lo + i) - 1] = keys[(lo + num) - 1];
                    i = num;
                }

                keys[(lo + i) - 1] = x;
            }

            public static int PickPivotAndPartition(TList keys, int lo, int hi, TComparer comparer)
            {
                int b = lo + ((hi - lo) >> 1);

                SwapIfGreater(keys, comparer, lo, b);
                SwapIfGreater(keys, comparer, lo, hi);
                SwapIfGreater(keys, comparer, b, hi);

                T y = keys[b];
                SwapElements<T, TList>(keys, b, hi - 1);

                int i = lo;
                int j = hi - 1;

                while (i < j)
                {
                    while (comparer.Compare(keys[++i], y) < 0)
                    {
                    }

                    while (comparer.Compare(y, keys[--j]) < 0)
                    {
                    }

                    if (i >= j)
                    {
                        break;
                    }

                    SwapElements<T, TList>(keys, i, j);
                }

                SwapElements<T, TList>(keys, i, hi - 1);
                return i;
            }

            public static void InsertionSort(TList keys, int lo, int hi, TComparer comparer)
            {
                for (int i = lo; i < hi; i++)
                {
                    int index = i;
                    T x = keys[i + 1];

                    while ((index >= lo) && (comparer.Compare(x, keys[index]) < 0))
                    {
                        keys[index + 1] = keys[index];
                        index--;
                    }

                    keys[index + 1] = x;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static void SwapIfGreater(TList keys, TComparer comparer, int i, int j)
            {
                if ((i != j) && (comparer.Compare(keys[i], keys[j]) > 0))
                {
                    T local = keys[i];
                    keys[i] = keys[j];
                    keys[j] = local;
                }
            }

            public static int InternalBinarySearch(TList array, int lowIndex, int length, T value, TComparer comparer)
            {
                int lo = lowIndex;
                int hi = (lowIndex + length) - 1;

                while (lo <= hi)
                {
                    int mid = lo + ((hi - lo) >> 1);
                    int comp = comparer.Compare(array[mid], value);

                    if (comp == 0)
                    {
                        return mid;
                    }

                    if (comp < 0)
                    {
                        lo = mid + 1;
                    }
                    else
                    {
                        hi = mid - 1;
                    }
                }

                return ~lo;
            }
        }
    }
}

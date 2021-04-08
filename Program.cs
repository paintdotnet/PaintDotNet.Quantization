using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using PaintDotNet.Imaging;
using PaintDotNet.Imaging.Quantization;
using System;
using System.IO;

namespace PaintDotNet.Quantization
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            // TODO: supposed to have benchmarks here

            QuantizeSomething();
        }

        private static void QuantizeSomething()
        {
            IImagingFactory factory = ImagingFactory.Instance;

            const string imagePath = "IMG_0386.jpg";
            IBitmap bitmap;
            using (Stream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                bitmap = factory.LoadBitmapFromStream(stream);
            }

            IBitmapSource<ColorBgra32> sourceBgra32 = factory.CreateFormatConvertedBitmap<ColorBgra32>(bitmap);

            ColorHistogram<ColorBgr24> histogram = ColorHistogram.CreateOpaque(sourceBgra32);

            OctreeQuantizer quantizer = new OctreeQuantizer();
            ColorBgra32[] colors = quantizer.GeneratePalette(histogram, 256, false);
            PaletteMap paletteMap = new ProximityPaletteMap(colors);

            QuantizedBitmapSource quantizedSource = new QuantizedBitmapSource(sourceBgra32, paletteMap, 0);
            IBitmapSource<ColorBgra32> quantizedSourceBgra32 = factory.CreateFormatConvertedBitmap<ColorBgra32>(quantizedSource);
            IBitmap<ColorBgra32> result = factory.CreateBitmap<ColorBgra32>(bitmap.Size);

            using (IBitmapLock<ColorBgra32> resultLock = result.Lock(BitmapLockOptions.ReadWrite))
            {
                quantizedSourceBgra32.CopyPixels(null, resultLock);
            }

            using (Stream output = new FileStream("quantized.png", FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                factory.SaveBitmapToStream(output, result, ImageFormat.Png);
            }
        }
    }
}

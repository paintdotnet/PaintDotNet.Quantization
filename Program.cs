using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;

namespace PaintDotNet.Quantization
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmarks>();
        }
    }

    public class Benchmarks
    {
        [Benchmark]
        public void CalculateSomething()
        {
            double[] array = new double[5000];
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = Math.Sqrt(i * i);
            }
        }

        [Benchmark]
        public void CalculateSomething2()
        {
            double[] array = new double[5000];
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = Math.Sqrt(Math.Sqrt(i * i * i * i));
            }
        }

    }
}

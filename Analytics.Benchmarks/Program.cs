using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Linq;

BenchmarkRunner.Run<ProcessingBenchmarks>();

public class ProcessingBenchmarks
{
    private int[] numbers;

    [GlobalSetup]
    public void Setup()
    {
        numbers = Enumerable.Range(1, 10000).ToArray();
    }

    [Benchmark]
    public int SumUsingForLoop()
    {
        int total = 0;
        for (int i = 0; i < numbers.Length; i++)
        {
            total += numbers[i];
        }
        return total;
    }

    [Benchmark]
    public int SumUsingLinq()
    {
        return numbers.Sum();
    }
}

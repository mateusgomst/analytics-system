```

BenchmarkDotNet v0.15.2, Windows 11 (10.0.26100.3194/24H2/2024Update/HudsonValley)
Unknown processor
.NET SDK 8.0.412
  [Host]     : .NET 8.0.18 (8.0.1825.31117), X64 RyuJIT AVX2
  DefaultJob : .NET 8.0.18 (8.0.1825.31117), X64 RyuJIT AVX2


```
| Method          | Mean       | Error    | StdDev   |
|---------------- |-----------:|---------:|---------:|
| SumUsingForLoop | 3,391.1 ns | 58.07 ns | 48.50 ns |
| SumUsingLinq    |   730.6 ns | 10.65 ns |  8.90 ns |

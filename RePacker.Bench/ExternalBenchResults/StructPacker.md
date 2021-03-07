Benchmarks from https://github.com/RudolfKurka/StructPacker

RePacker is built in netcoreapp3.1 mode

Ran on an I5-4670K @ 4.3GHz


## SmallMessageBenchmarks
```
|               Method |        Mean |    Error |   StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |------------:|---------:|---------:|------:|--------:|-------:|------:|------:|----------:|
|         StructPacker |    331.2 ns |  1.87 ns |  1.66 ns |  1.00 |    0.00 | 0.2728 |     - |     - |     856 B |
|           BinaryPack |    372.3 ns |  1.52 ns |  1.35 ns |  1.12 |    0.01 | 0.3519 |     - |     - |    1104 B |
| 'MessagePack for C#' |  1,166.6 ns |  3.21 ns |  3.00 ns |  3.52 |    0.02 | 0.3510 |     - |     - |    1104 B |
|      BinaryFormatter | 16,344.2 ns | 57.89 ns | 54.15 ns | 49.36 |    0.30 | 4.5166 |     - |     - |   14231 B |
|      Newtonsoft.Json | 14,242.8 ns | 62.44 ns | 58.40 ns | 43.02 |    0.32 | 3.8910 |     - |     - |   12232 B |
|             RePacker |    341.0 ns |  2.31 ns |  2.04 ns |  1.03 |    0.01 | 0.2165 |     - |     - |     680 B |
|        RePacker-Auto |    429.3 ns |  1.11 ns |  0.99 ns |  1.30 |    0.01 | 0.3643 |     - |     - |    1144 B |
```

## LargeMessageBenchmarks
```
|               Method |       Mean |     Error |    StdDev |  Ratio | RatioSD |      Gen 0 |     Gen 1 |   Gen 2 | Allocated |
|--------------------- |-----------:|----------:|----------:|-------:|--------:|-----------:|----------:|--------:|----------:|
|         StructPacker |   1.218 ms | 0.0172 ms | 0.0161 ms |   1.00 |    0.00 |   199.2188 |  105.4688 |  9.7656 |   2.59 MB |
|           BinaryPack |   3.615 ms | 0.0126 ms | 0.0112 ms |   2.97 |    0.04 |   257.8125 |  175.7813 | 62.5000 |  10.66 MB |
| 'MessagePack for C#' |   9.212 ms | 0.0185 ms | 0.0173 ms |   7.56 |    0.10 |   187.5000 |   93.7500 |       - |   4.63 MB |
|      BinaryFormatter |   4.552 ms | 0.0289 ms | 0.0271 ms |   3.74 |    0.04 |   750.0000 |  398.4375 | 46.8750 |   9.01 MB |
|      Newtonsoft.Json | 208.683 ms | 1.1096 ms | 1.0379 ms | 171.34 |    2.57 | 64000.0000 | 1000.0000 |       - | 234.34 MB |
|             RePacker |   1.167 ms | 0.0163 ms | 0.0144 ms |   0.96 |    0.02 |   197.2656 |  105.4688 |  9.7656 |   2.59 MB |
|        RePacker-Auto |   1.921 ms | 0.0288 ms | 0.0269 ms |   1.58 |    0.03 |   232.4219 |  134.7656 | 27.3438 |   4.67 MB |
```
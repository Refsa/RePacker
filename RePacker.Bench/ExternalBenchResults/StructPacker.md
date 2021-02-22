Benchmarks from https://github.com/RudolfKurka/StructPacker

RePacker is built in netcoreapp3.1 mode

Ran on an I5-4670K @ 4.3GHz


## SmallMessageBenchmarks
```
|               Method |        Mean |     Error |    StdDev | Ratio | RatioSD |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|--------------------- |------------:|----------:|----------:|------:|--------:|-------:|------:|------:|----------:|
|         StructPacker |    325.6 ns |   1.59 ns |   1.41 ns |  1.00 |    0.00 | 0.2728 |     - |     - |     856 B |
|           BinaryPack |    368.9 ns |   4.68 ns |   4.38 ns |  1.13 |    0.01 | 0.3519 |     - |     - |    1104 B |
| 'MessagePack for C#' |  1,179.3 ns |   8.74 ns |   8.18 ns |  3.62 |    0.02 | 0.3510 |     - |     - |    1104 B |
|      BinaryFormatter | 16,405.4 ns | 131.85 ns | 116.88 ns | 50.39 |    0.45 | 4.5166 |     - |     - |   14231 B |
|      Newtonsoft.Json | 14,410.9 ns |  92.30 ns |  86.34 ns | 44.28 |    0.36 | 3.8910 |     - |     - |   12232 B |
|             RePacker |    305.5 ns |   1.36 ns |   1.27 ns |  0.94 |    0.00 | 0.2165 |     - |     - |     680 B |
```

## LargeMessageBenchmarks
```
|               Method |       Mean |     Error |    StdDev |  Ratio | RatioSD |      Gen 0 |     Gen 1 |   Gen 2 | Allocated |
|--------------------- |-----------:|----------:|----------:|-------:|--------:|-----------:|----------:|--------:|----------:|
|         StructPacker |   1.152 ms | 0.0225 ms | 0.0259 ms |   1.00 |    0.00 |   197.2656 |  105.4688 |  9.7656 |   2.59 MB |
|           BinaryPack |   3.608 ms | 0.0610 ms | 0.0570 ms |   3.12 |    0.09 |   257.8125 |  175.7813 | 62.5000 |  10.66 MB |
| 'MessagePack for C#' |   9.135 ms | 0.0361 ms | 0.0320 ms |   7.90 |    0.19 |   187.5000 |   93.7500 |       - |   4.63 MB |
|      BinaryFormatter |   4.544 ms | 0.0386 ms | 0.0361 ms |   3.93 |    0.09 |   757.8125 |  398.4375 | 46.8750 |   9.01 MB |
|      Newtonsoft.Json | 204.402 ms | 1.0804 ms | 0.9022 ms | 176.74 |    4.14 | 64000.0000 | 1000.0000 |       - | 234.34 MB |
|             RePacker |   1.108 ms | 0.0221 ms | 0.0295 ms |   0.97 |    0.03 |   195.3125 |  105.4688 |  9.7656 |   2.59 MB |
```
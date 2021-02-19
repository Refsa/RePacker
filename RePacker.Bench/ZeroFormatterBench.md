Small Object(int,string,string,enum) 10000 Iteration

MessagePack C#
      Serialize   1.5552 ms
    Deserialize   1.5261 ms
    Binary Size   22.00 B

ZeroFormatter
      Serialize   2.2588 ms
    Deserialize   1.6268 ms
    Binary Size   50.00 B

Wire
      Serialize   4.3507 ms
    Deserialize   6.7752 ms
    Binary Size   119.00 B

NetSerializer
      Serialize   2.7995 ms
    Deserialize   3.452 ms
    Binary Size   21.00 B

RePacker
      Serialize   1.4082 ms
    Deserialize   1.8097 ms
    Binary Size   34.00 B

Bois
      Serialize   3.6361 ms
    Deserialize   5.3119 ms
    Binary Size   20.00 B

Large Array(SmallObject[1000]) 10000 Iteration

MessagePack C#
      Serialize   1198.9308 ms
    Deserialize   1479.6694 ms
    Binary Size   19.53 KB

ZeroFormatter
      Serialize   1263.1224 ms
    Deserialize   1731.3268 ms
    Binary Size   48.83 KB

Wire
      Serialize   3248.7518 ms
    Deserialize   3893.7544 ms
    Binary Size   25.54 KB

NetSerializer
      Serialize   2035.5846 ms
    Deserialize   2936.6888 ms
    Binary Size   20.51 KB

RePacker
      Serialize   1403.4431 ms
    Deserialize   1935.8 ms
    Binary Size   33.21 KB


Additional Benchmarks

Int32(1) 10000 Iteration

MessagePack C#
      Serialize   0.4084 ms
    Deserialize   0.1913 ms
    Binary Size   5.00 B

ZeroFormatter
      Serialize   0.2925 ms
    Deserialize   0.1525 ms
    Binary Size   4.00 B

Wire
      Serialize   1.5368 ms
    Deserialize   0.4424 ms
    Binary Size   5.00 B

NetSerializer
      Serialize   1.1886 ms
    Deserialize   0.287 ms
    Binary Size   5.00 B

RePacker
      Serialize   0.2363 ms
    Deserialize   0.2421 ms
    Binary Size   4.00 B

Bois
      Serialize   2.0231 ms
    Deserialize   1.0308 ms
    Binary Size   5.00 B

Vector3(float, float, float) 10000 Iteration

MessagePack C#
      Serialize   0.5404 ms
    Deserialize   0.3833 ms
    Binary Size   16.00 B

ZeroFormatter
      Serialize   0.5114 ms
    Deserialize   0.2916 ms
    Binary Size   12.00 B

Wire
      Serialize   2.655 ms
    Deserialize   2.8247 ms
    Binary Size   64.00 B

NetSerializer
      Serialize   1.7795 ms
    Deserialize   0.6986 ms
    Binary Size   15.00 B

RePacker
      Serialize   0.4922 ms
    Deserialize   0.468 ms
    Binary Size   12.00 B

Bois
      Serialize   3.6061 ms
    Deserialize   1.9631 ms
    Binary Size   15.00 B

HtmlString(309081bytes) 10000 Iteration

MessagePack C#
      Serialize   5421.9541 ms
    Deserialize   3544.7263 ms
    Binary Size   301.84 KB

ZeroFormatter
      Serialize   4122.0115 ms
    Deserialize   3403.7562 ms
    Binary Size   301.84 KB

Wire
      Serialize   5445.2144 ms
    Deserialize   4698.9201 ms
    Binary Size   301.84 KB

NetSerializer
      Serialize   7365.664 ms
    Deserialize   6360.3134 ms
    Binary Size   301.84 KB

RePacker
      Serialize   1635.1404 ms
    Deserialize   3814.0186 ms
    Binary Size   301.84 KB

Vector3[100] 10000 Iteration

MessagePack C#
      Serialize   24.7503 ms
    Deserialize   34.8203 ms
    Binary Size   1.57 KB

ZeroFormatter
      Serialize   21.65 ms
    Deserialize   18.144 ms
    Binary Size   1.18 KB

Wire
      Serialize   140.4411 ms
    Deserialize   138.6006 ms
    Binary Size   1.57 KB

NetSerializer
      Serialize   80.1401 ms
    Deserialize   53.8876 ms
    Binary Size   1.47 KB

RePacker
      Serialize   1.1159 ms
    Deserialize   3.7357 ms
    Binary Size   1.18 KB

Press key to exit.
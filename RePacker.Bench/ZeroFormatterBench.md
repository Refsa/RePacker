Bench suite in ZeroFormatter ran on a 4670K @ 4.3GHz

Small Object(int,string,string,enum) 10000 Iteration

MessagePack C#
      Serialize   1.6642 ms
    Deserialize   1.3752 ms
    Binary Size   22.00 B

ZeroFormatter
      Serialize   2.2676 ms
    Deserialize   1.6494 ms
    Binary Size   50.00 B

Wire
      Serialize   5.1432 ms
    Deserialize   6.8146 ms
    Binary Size   119.00 B

NetSerializer
      Serialize   3.1481 ms
    Deserialize   2.7244 ms
    Binary Size   21.00 B

RePacker
      Serialize   1.3502 ms
    Deserialize   1.5581 ms
    Binary Size   34.00 B

Bois
      Serialize   4.5452 ms
    Deserialize   5.3307 ms
    Binary Size   20.00 B

Large Array(SmallObject[1000]) 10000 Iteration

MessagePack C#
      Serialize   1153.8415 ms
    Deserialize   1448.6883 ms
    Binary Size   19.53 KB

ZeroFormatter
      Serialize   1257.8474 ms
    Deserialize   1755.3549 ms
    Binary Size   48.83 KB

Wire
      Serialize   3036.6525 ms
    Deserialize   3741.9953 ms
    Binary Size   25.54 KB

NetSerializer
      Serialize   2053.8752 ms
    Deserialize   2867.0336 ms
    Binary Size   20.51 KB

RePacker
      Serialize   1374.0483 ms
    Deserialize   1646.3019 ms
    Binary Size   33.21 KB


Additional Benchmarks

Int32(1) 10000 Iteration

MessagePack C#
      Serialize   0.4124 ms
    Deserialize   0.1867 ms
    Binary Size   5.00 B

ZeroFormatter
      Serialize   0.2814 ms
    Deserialize   0.1516 ms
    Binary Size   4.00 B

Wire
      Serialize   1.5382 ms
    Deserialize   0.4517 ms
    Binary Size   5.00 B

NetSerializer
      Serialize   1.2107 ms
    Deserialize   0.2877 ms
    Binary Size   5.00 B

RePacker
      Serialize   0.1077 ms
    Deserialize   0.1025 ms
    Binary Size   4.00 B

Bois
      Serialize   1.1714 ms
    Deserialize   1.7085 ms
    Binary Size   5.00 B

Vector3(float, float, float) 10000 Iteration

MessagePack C#
      Serialize   0.5339 ms
    Deserialize   0.3858 ms
    Binary Size   16.00 B

ZeroFormatter
      Serialize   0.4436 ms
    Deserialize   0.2837 ms
    Binary Size   12.00 B

Wire
      Serialize   4.3974 ms
    Deserialize   2.8309 ms
    Binary Size   64.00 B

NetSerializer
      Serialize   1.7758 ms
    Deserialize   0.7389 ms
    Binary Size   15.00 B

RePacker
      Serialize   0.2297 ms
    Deserialize   0.2194 ms
    Binary Size   12.00 B

Bois
      Serialize   2.145 ms
    Deserialize   2.1207 ms
    Binary Size   15.00 B

HtmlString(309081bytes) 10000 Iteration

MessagePack C#
      Serialize   5310.4681 ms
    Deserialize   3516.29 ms
    Binary Size   301.84 KB

ZeroFormatter
      Serialize   4071.2564 ms
    Deserialize   3426.6567 ms
    Binary Size   301.84 KB

Wire
      Serialize   5345.9001 ms
    Deserialize   4693.4702 ms
    Binary Size   301.84 KB

NetSerializer
      Serialize   7181.1968 ms
    Deserialize   6316.6237 ms
    Binary Size   301.84 KB

RePacker
      Serialize   1660.2608 ms
    Deserialize   3739.5829 ms
    Binary Size   301.84 KB

Vector3[100] 10000 Iteration

MessagePack C#
      Serialize   22.3564 ms
    Deserialize   32.7238 ms
    Binary Size   1.57 KB

ZeroFormatter
      Serialize   21.8432 ms
    Deserialize   19.945 ms
    Binary Size   1.18 KB

Wire
      Serialize   127.6926 ms
    Deserialize   133.3241 ms
    Binary Size   1.57 KB

NetSerializer
      Serialize   74.8765 ms
    Deserialize   50.9654 ms
    Binary Size   1.47 KB

RePacker
      Serialize   0.9915 ms
    Deserialize   2.2206 ms
    Binary Size   1.18 KB
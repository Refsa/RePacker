Benchmarks ran on a 4670K @ 4.3GHz with 32GB ram

```
Small Object(int,string,string,enum) 10000 Iteration

MessagePack C#
      Serialize   1.5147 ms
    Deserialize   1.614 ms
    Binary Size   22.00 B

MessagePack C# LZ4
      Serialize   1.7738 ms
    Deserialize   1.5223 ms
    Binary Size   22.00 B

ZeroFormatter
      Serialize   2.8857 ms
    Deserialize   1.641 ms
    Binary Size   50.00 B

protobuf-net
      Serialize   4.1284 ms
    Deserialize   5.0154 ms
    Binary Size   23.00 B

MsgPack-CLI
      Serialize   5.7861 ms
    Deserialize   9.7606 ms
    Binary Size   26.00 B

JSON.NET
      Serialize   19.9357 ms
    Deserialize   24.4137 ms
    Binary Size   66.00 B

Jil
      Serialize   11.1291 ms
    Deserialize   9.409 ms
    Binary Size   71.00 B

FsPickler
      Serialize   12.3676 ms
    Deserialize   9.4075 ms
    Binary Size   44.00 B

BinaryFormatter
      Serialize   43.3643 ms
    Deserialize   62.5431 ms
    Binary Size   285.00 B

DataContractSerializer
      Serialize   14.0876 ms
    Deserialize   49.7786 ms
    Binary Size   206.00 B

FlatBuffers
      Serialize   7.7623 ms
    Deserialize   0.314 ms
    Binary Size   60.00 B

Google.Protobuf
      Serialize   4.5366 ms
    Deserialize   1.4605 ms
    Binary Size   23.00 B

Wire
      Serialize   5.1148 ms
    Deserialize   7.4366 ms
    Binary Size   119.00 B

NetSerializer
      Serialize   3.1129 ms
    Deserialize   2.7465 ms
    Binary Size   21.00 B

RePacker
      Serialize   2.6919 ms
    Deserialize   2.7199 ms
    Binary Size   34.00 B

Large Array(SmallObject[1000]) 10000 Iteration

MessagePack C#
      Serialize   949.2056 ms
    Deserialize   1366.4259 ms
    Binary Size   19.53 KB

MessagePack C# LZ4
      Serialize   1100.0675 ms
    Deserialize   1464.686 ms
    Binary Size   4.92 KB

ZeroFormatter
      Serialize   1245.1177 ms
    Deserialize   1.0184 ms
    Binary Size   52.74 KB

protobuf-net
      Serialize   3184.4768 ms
    Deserialize   4670.9791 ms
    Binary Size   23.44 KB

MsgPack-CLI
      Serialize   3069.269 ms
    Deserialize   9865.4772 ms
    Binary Size   25.39 KB

JSON.NET
      Serialize   8599.7852 ms
    Deserialize   18512.3559 ms
    Binary Size   61.53 KB

Jil
      Serialize   2821.6074 ms
    Deserialize   5003.3574 ms
    Binary Size   68.36 KB

FsPickler
      Serialize   3385.5981 ms
    Deserialize   3333.7984 ms
    Binary Size   25.53 KB

BinaryFormatter
      Serialize   19468.6493 ms
    Deserialize   21291.5938 ms
    Binary Size   37.38 KB

DataContractSerializer
      Serialize   5850.6481 ms
    Deserialize   19135.1986 ms
    Binary Size   103.64 KB

FlatBuffers
      Serialize   2775.5741 ms
    Deserialize   0.3231 ms
    Binary Size   46.91 KB

Google.Protobuf
      Serialize   1029.2765 ms
    Deserialize   1635.4997 ms
    Binary Size   23.44 KB

Wire
      Serialize   2963.9796 ms
    Deserialize   3673.2062 ms
    Binary Size   25.54 KB

NetSerializer
      Serialize   1976.1563 ms
    Deserialize   2770.6321 ms
    Binary Size   20.51 KB
RePacker
      Serialize   2541.0121 ms
    Deserialize   3113.9046 ms
    Binary Size   33.21 KB

Additional Benchmarks

Int32(1) 10000 Iteration

MessagePack C#
      Serialize   0.4567 ms
    Deserialize   0.2268 ms
    Binary Size   5.00 B

MessagePack C# LZ4
      Serialize   0.6992 ms
    Deserialize   0.2628 ms
    Binary Size   5.00 B

ZeroFormatter
      Serialize   0.2928 ms
    Deserialize   0.1615 ms
    Binary Size   4.00 B

MsgPack-CLI
      Serialize   3.1176 ms
    Deserialize   3.7811 ms
    Binary Size   5.00 B

protobuf-net
      Serialize   11.6449 ms
    Deserialize   8.1642 ms
    Binary Size   6.00 B

Wire
      Serialize   2.7263 ms
    Deserialize   0.4656 ms
    Binary Size   5.00 B

NetSerializer
      Serialize   1.5517 ms
    Deserialize   0.2971 ms
    Binary Size   5.00 B

RePacker
      Serialize   0.3222 ms
    Deserialize   0.3294 ms
    Binary Size   4.00 B

Vector3(float, float, float) 10000 Iteration

MessagePack C#
      Serialize   0.5622 ms
    Deserialize   0.4273 ms
    Binary Size   16.00 B

MessagePack C# LZ4
      Serialize   0.8358 ms
    Deserialize   0.8149 ms
    Binary Size   16.00 B

ZeroFormatter
      Serialize   0.4609 ms
    Deserialize   0.2921 ms
    Binary Size   12.00 B

MsgPack-CLI
      Serialize   4.381 ms
    Deserialize   5.3772 ms
    Binary Size   17.00 B

protobuf-net
      Serialize   3.3567 ms
    Deserialize   3.5912 ms
    Binary Size   15.00 B

Wire
      Serialize   2.6697 ms
    Deserialize   3.0453 ms
    Binary Size   64.00 B

NetSerializer
      Serialize   1.8925 ms
    Deserialize   0.7322 ms
    Binary Size   15.00 B

RePacker
      Serialize   0.6204 ms
    Deserialize   0.6253 ms
    Binary Size   12.00 B

HtmlString(309081bytes) 10000 Iteration

MessagePack C#
      Serialize   3970.9675 ms
    Deserialize   2334.0618 ms
    Binary Size   301.84 KB

MessagePack C# LZ4
      Serialize   9099.122 ms
    Deserialize   3818.3476 ms
    Binary Size   85.93 KB

ZeroFormatter
      Serialize   3406.8465 ms
    Deserialize   2326.1449 ms
    Binary Size   301.84 KB

MsgPack-CLI
      Serialize   4393.3865 ms
    Deserialize   2771.362 ms
    Binary Size   301.84 KB

protobuf-net
      Serialize   3937.9462 ms
    Deserialize   5936.4167 ms
    Binary Size   301.84 KB

Wire
      Serialize   3883.1663 ms
    Deserialize   2703.3934 ms
    Binary Size   301.84 KB

NetSerializer
      Serialize   5088.5714 ms
    Deserialize   3608.078 ms
    Binary Size   301.84 KB

RePacker
      Serialize   4315.2534 ms
    Deserialize   2147.9541 ms
    Binary Size   301.84 KB

Vector3[100] 10000 Iteration

MessagePack C#
      Serialize   23.0147 ms
    Deserialize   36.023 ms
    Binary Size   1.57 KB

MessagePack C# LZ4
      Serialize   53.9214 ms
    Deserialize   38.5386 ms
    Binary Size   45.00 B

ZeroFormatter
      Serialize   21.389 ms
    Deserialize   0.9238 ms
    Binary Size   1.18 KB

MsgPack-CLI
      Serialize   159.4162 ms
    Deserialize   509.1876 ms
    Binary Size   1.66 KB

protobuf-net
      Serialize   398.0115 ms
    Deserialize   675.5429 ms
    Binary Size   1.66 KB

Wire
      Serialize   124.3347 ms
    Deserialize   129.5983 ms
    Binary Size   1.57 KB

NetSerializer
      Serialize   75.8055 ms
    Deserialize   49.7211 ms
    Binary Size   1.47 KB

RePacker
      Serialize   52.8675 ms
    Deserialize   78.0656 ms
    Binary Size   1.18 KB
```
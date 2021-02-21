Ran on a Intel I5-4670K @ 4.3GHz

```
Small Object(int,string,string,enum) 10000 Iteration

MessagePack C#
      Serialize   1.5245 ms
    Deserialize   1.4752 ms
    Binary Size   22.00 B

MessagePack C# LZ4
      Serialize   1.7748 ms
    Deserialize   1.5805 ms
    Binary Size   22.00 B

ZeroFormatter
      Serialize   2.5516 ms
    Deserialize   2.2874 ms
    Binary Size   50.00 B

protobuf-net
      Serialize   4.4625 ms
    Deserialize   5.1629 ms
    Binary Size   23.00 B

MsgPack-CLI
      Serialize   6.3356 ms
    Deserialize   11.1965 ms
    Binary Size   26.00 B

JSON.NET
      Serialize   22.405 ms
    Deserialize   27.6444 ms
    Binary Size   66.00 B

Jil
      Serialize   9.8309 ms
    Deserialize   9.5392 ms
    Binary Size   71.00 B

FsPickler
      Serialize   10.2191 ms
    Deserialize   9.4137 ms
    Binary Size   44.00 B

BinaryFormatter
      Serialize   45.6252 ms
    Deserialize   54.0651 ms
    Binary Size   285.00 B

DataContractSerializer
      Serialize   14.0902 ms
    Deserialize   46.618 ms
    Binary Size   206.00 B

FlatBuffers
      Serialize   7.5003 ms
    Deserialize   0.3556 ms
    Binary Size   60.00 B

Google.Protobuf
      Serialize   5.1775 ms
    Deserialize   1.4125 ms
    Binary Size   23.00 B

Wire
      Serialize   4.3746 ms
    Deserialize   6.6662 ms
    Binary Size   119.00 B

NetSerializer
      Serialize   2.9856 ms
    Deserialize   2.6734 ms
    Binary Size   21.00 B

RePacker
      Serialize   1.1307 ms
    Deserialize   1.6199 ms
    Binary Size   34.00 B

Bois
      Serialize   7.3789 ms
    Deserialize   8.7223 ms
    Binary Size   20.00 B

Large Array(SmallObject[1000]) 10000 Iteration

MessagePack C#
      Serialize   946.9976 ms
    Deserialize   1422.7729 ms
    Binary Size   19.53 KB

MessagePack C# LZ4
      Serialize   1133.5478 ms
    Deserialize   1464.5387 ms
    Binary Size   4.92 KB

ZeroFormatter
      Serialize   1193.7488 ms
    Deserialize   1662.6651 ms
    Binary Size   48.83 KB

protobuf-net
      Serialize   3190.0189 ms
    Deserialize   4515.6173 ms
    Binary Size   23.44 KB

MsgPack-CLI
      Serialize   3024.8521 ms
    Deserialize   10056.5763 ms
    Binary Size   25.39 KB

JSON.NET
      Serialize   8891.5849 ms
    Deserialize   16227.4228 ms
    Binary Size   61.53 KB

Jil
      Serialize   2926.2008 ms
    Deserialize   4984.1163 ms
    Binary Size   68.36 KB

FsPickler
      Serialize   3336.2355 ms
    Deserialize   3546.4489 ms
    Binary Size   25.42 KB

BinaryFormatter
      Serialize   19764.2385 ms
    Deserialize   21245.8755 ms
    Binary Size   37.38 KB

DataContractSerializer
      Serialize   6556.9513 ms
    Deserialize   18945.8405 ms
    Binary Size   103.64 KB

FlatBuffers
      Serialize   2714.637 ms
    Deserialize   0.3726 ms
    Binary Size   46.91 KB

Google.Protobuf
      Serialize   1012.9659 ms
    Deserialize   1773.3345 ms
    Binary Size   23.44 KB

Wire
      Serialize   3068.66 ms
    Deserialize   3612.9407 ms
    Binary Size   25.54 KB

NetSerializer
      Serialize   1950.4378 ms
    Deserialize   2790.9395 ms
    Binary Size   20.51 KB

RePacker
      Serialize   1027.519 ms
    Deserialize   1538.5211 ms
    Binary Size   33.21 KB


Additional Benchmarks

Int32(1) 10000 Iteration

MessagePack C#
      Serialize   0.4878 ms
    Deserialize   0.1902 ms
    Binary Size   5.00 B

MessagePack C# LZ4
      Serialize   0.6776 ms
    Deserialize   0.2568 ms
    Binary Size   5.00 B

ZeroFormatter
      Serialize   0.2949 ms
    Deserialize   0.156 ms
    Binary Size   4.00 B

MsgPack-CLI
      Serialize   2.8029 ms
    Deserialize   2.3937 ms
    Binary Size   5.00 B

protobuf-net
      Serialize   7.0452 ms
    Deserialize   6.9136 ms
    Binary Size   6.00 B

Wire
      Serialize   1.8617 ms
    Deserialize   0.4921 ms
    Binary Size   5.00 B

NetSerializer
      Serialize   1.5277 ms
    Deserialize   0.307 ms
    Binary Size   5.00 B

RePacker
      Serialize   0.1053 ms
    Deserialize   0.1136 ms
    Binary Size   4.00 B

Bois
      Serialize   4.3169 ms
    Deserialize   4.2721 ms
    Binary Size   5.00 B

Vector3(float, float, float) 10000 Iteration

MessagePack C#
      Serialize   0.5542 ms
    Deserialize   0.4397 ms
    Binary Size   16.00 B

MessagePack C# LZ4
      Serialize   0.8772 ms
    Deserialize   0.5893 ms
    Binary Size   16.00 B

ZeroFormatter
      Serialize   0.445 ms
    Deserialize   0.2867 ms
    Binary Size   12.00 B

MsgPack-CLI
      Serialize   4.7671 ms
    Deserialize   5.0642 ms
    Binary Size   17.00 B

protobuf-net
      Serialize   3.1822 ms
    Deserialize   4.9257 ms
    Binary Size   15.00 B

Wire
      Serialize   2.9538 ms
    Deserialize   3.3874 ms
    Binary Size   64.00 B

NetSerializer
      Serialize   2.1446 ms
    Deserialize   0.7417 ms
    Binary Size   15.00 B

RePacker
      Serialize   0.2148 ms
    Deserialize   0.2164 ms
    Binary Size   12.00 B

Bois
      Serialize   3.8174 ms
    Deserialize   2.8925 ms
    Binary Size   15.00 B

HtmlString(309081bytes) 10000 Iteration

MessagePack C#
      Serialize   5219.3907 ms
    Deserialize   3290.6709 ms
    Binary Size   301.84 KB

MessagePack C# LZ4
      Serialize   10325.3331 ms
    Deserialize   5555.3901 ms
    Binary Size   85.93 KB

ZeroFormatter
      Serialize   3974.6218 ms
    Deserialize   3312.6659 ms
    Binary Size   301.84 KB

MsgPack-CLI
      Serialize   6296.1499 ms
    Deserialize   4629.4635 ms
    Binary Size   301.84 KB

protobuf-net
      Serialize   5566.4671 ms
    Deserialize   7782.1616 ms
    Binary Size   301.84 KB

Wire
      Serialize   5602.404 ms
    Deserialize   4564.5271 ms
    Binary Size   301.84 KB

NetSerializer
      Serialize   6972.8421 ms
    Deserialize   6489.5602 ms
    Binary Size   301.84 KB

RePacker
      Serialize   1558.8781 ms
    Deserialize   3608.0331 ms
    Binary Size   301.84 KB

Vector3[100] 10000 Iteration

MessagePack C#
      Serialize   22.12 ms
    Deserialize   31.9848 ms
    Binary Size   1.57 KB

MessagePack C# LZ4
      Serialize   51.9288 ms
    Deserialize   36.3355 ms
    Binary Size   45.00 B

ZeroFormatter
      Serialize   20.9181 ms
    Deserialize   17.4602 ms
    Binary Size   1.18 KB

MsgPack-CLI
      Serialize   153.8792 ms
    Deserialize   513.4285 ms
    Binary Size   1.66 KB

protobuf-net
      Serialize   393.1885 ms
    Deserialize   427.7192 ms
    Binary Size   1.66 KB

Wire
      Serialize   122.7543 ms
    Deserialize   133.1002 ms
    Binary Size   1.57 KB

NetSerializer
      Serialize   71.6016 ms
    Deserialize   52.7038 ms
    Binary Size   1.47 KB

RePacker
      Serialize   0.9072 ms
    Deserialize   1.7335 ms
    Binary Size   1.18 KB
```
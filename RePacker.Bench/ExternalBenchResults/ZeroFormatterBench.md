Benchmarks from https://github.com/neuecc/ZeroFormatter

RePacker is built in net4.6.1 mode

Ran on an I5-4670K @ 4.3GHz

```
Small Object(int,string,string,enum) 10000 Iteration

MessagePack C#
      Serialize   1.5543 ms
    Deserialize   1.5426 ms
    Binary Size   22.00 B

MessagePack C# LZ4
      Serialize   1.7279 ms
    Deserialize   1.7142 ms
    Binary Size   22.00 B

ZeroFormatter
      Serialize   2.7639 ms
    Deserialize   3.3335 ms
    Binary Size   50.00 B

protobuf-net
      Serialize   5.7789 ms
    Deserialize   5.7004 ms
    Binary Size   23.00 B

MsgPack-CLI
      Serialize   8.1921 ms
    Deserialize   11.8135 ms
    Binary Size   26.00 B

JSON.NET
      Serialize   20.0928 ms
    Deserialize   27.5261 ms
    Binary Size   66.00 B

Jil
      Serialize   9.1675 ms
    Deserialize   9.8863 ms
    Binary Size   71.00 B

FsPickler
      Serialize   10.0456 ms
    Deserialize   9.6722 ms
    Binary Size   44.00 B

BinaryFormatter
      Serialize   51.8232 ms
    Deserialize   59.9197 ms
    Binary Size   285.00 B

DataContractSerializer
      Serialize   16.5106 ms
    Deserialize   53.2821 ms
    Binary Size   206.00 B

FlatBuffers
      Serialize   8.7233 ms
    Deserialize   0.3543 ms
    Binary Size   60.00 B

Google.Protobuf
      Serialize   11.147 ms
    Deserialize   1.6414 ms
    Binary Size   23.00 B

Wire
      Serialize   5.6071 ms
    Deserialize   9.5124 ms
    Binary Size   119.00 B

NetSerializer
      Serialize   3.0544 ms
    Deserialize   2.7097 ms
    Binary Size   21.00 B

RePacker
      Serialize   0.6502 ms
 Serialize-Auto   1.5272 ms
    Deserialize   1.0732 ms
    Binary Size   34.00 B

Large Array(SmallObject[1000]) 10000 Iteration

MessagePack C#
      Serialize   1116.0047 ms
    Deserialize   1545.7527 ms
    Binary Size   19.53 KB

MessagePack C# LZ4
      Serialize   1195.1753 ms
    Deserialize   1583.4883 ms
    Binary Size   4.92 KB

ZeroFormatter
      Serialize   1352.9125 ms
    Deserialize   1812.3203 ms
    Binary Size   48.83 KB

protobuf-net
      Serialize   3423.9146 ms
    Deserialize   4899.3281 ms
    Binary Size   23.44 KB

MsgPack-CLI
      Serialize   3279.2549 ms
    Deserialize   10536.4166 ms
    Binary Size   25.39 KB

JSON.NET
      Serialize   9633.3952 ms
    Deserialize   17120.9545 ms
    Binary Size   61.53 KB

Jil
      Serialize   2823.454 ms
    Deserialize   5478.5762 ms
    Binary Size   68.36 KB

FsPickler
      Serialize   3793.0897 ms
    Deserialize   3748.5347 ms
    Binary Size   25.42 KB

BinaryFormatter
      Serialize   21323.0215 ms
    Deserialize   22704.7738 ms
    Binary Size   37.38 KB

DataContractSerializer
      Serialize   6246.1163 ms
    Deserialize   20147.5255 ms
    Binary Size   103.64 KB

FlatBuffers
      Serialize   2877.8423 ms
    Deserialize   0.414 ms
    Binary Size   46.91 KB

Google.Protobuf
      Serialize   1158.691 ms
    Deserialize   1757.6393 ms
    Binary Size   23.44 KB

Wire
      Serialize   3161.1446 ms
    Deserialize   3889.2075 ms
    Binary Size   25.54 KB

NetSerializer
      Serialize   2115.4307 ms
    Deserialize   2968.4544 ms
    Binary Size   20.51 KB

RePacker
      Serialize   616.9024 ms
 Serialize-Auto   1219.7565 ms
    Deserialize   1217.0159 ms
    Binary Size   33.21 KB


Additional Benchmarks

Int32(1) 10000 Iteration

MessagePack C#
      Serialize   0.4006 ms
    Deserialize   0.1951 ms
    Binary Size   5.00 B

MessagePack C# LZ4
      Serialize   0.6965 ms
    Deserialize   0.2593 ms
    Binary Size   5.00 B

ZeroFormatter
      Serialize   0.2967 ms
    Deserialize   0.1842 ms
    Binary Size   4.00 B

MsgPack-CLI
      Serialize   3.1773 ms
    Deserialize   2.4179 ms
    Binary Size   5.00 B

protobuf-net
      Serialize   7.6466 ms
    Deserialize   6.8099 ms
    Binary Size   6.00 B

Wire
      Serialize   2.7654 ms
    Deserialize   0.5529 ms
    Binary Size   5.00 B

NetSerializer
      Serialize   1.9052 ms
    Deserialize   0.2851 ms
    Binary Size   5.00 B

RePacker
      Serialize   0.0685 ms
 Serialize-Auto   0.1831 ms
    Deserialize   0.0611 ms
    Binary Size   4.00 B

Vector3(float, float, float) 10000 Iteration

MessagePack C#
      Serialize   0.6729 ms
    Deserialize   0.3959 ms
    Binary Size   16.00 B

MessagePack C# LZ4
      Serialize   0.9389 ms
    Deserialize   0.5578 ms
    Binary Size   16.00 B

ZeroFormatter
      Serialize   0.4954 ms
    Deserialize   0.2868 ms
    Binary Size   12.00 B

MsgPack-CLI
      Serialize   4.9352 ms
    Deserialize   5.8993 ms
    Binary Size   17.00 B

protobuf-net
      Serialize   5.8201 ms
    Deserialize   3.5913 ms
    Binary Size   15.00 B

Wire
      Serialize   4.8677 ms
    Deserialize   2.897 ms
    Binary Size   64.00 B

NetSerializer
      Serialize   2.3143 ms
    Deserialize   0.7617 ms
    Binary Size   15.00 B

RePacker
      Serialize   0.1531 ms
 Serialize-Auto   0.2904 ms
    Deserialize   0.2189 ms
    Binary Size   12.00 B

HtmlString(309081bytes) 10000 Iteration

MessagePack C#
      Serialize   4286.5655 ms
    Deserialize   2494.7379 ms
    Binary Size   301.84 KB

MessagePack C# LZ4
      Serialize   10210.3636 ms
    Deserialize   4157.8598 ms
    Binary Size   85.93 KB

ZeroFormatter
      Serialize   3588.7474 ms
    Deserialize   2548.7291 ms
    Binary Size   301.84 KB

MsgPack-CLI
      Serialize   4928.5177 ms
    Deserialize   3065.3257 ms
    Binary Size   301.84 KB

protobuf-net
      Serialize   4364.5253 ms
    Deserialize   6983.2975 ms
    Binary Size   301.84 KB

Wire
      Serialize   4498.3491 ms
    Deserialize   3174.176 ms
    Binary Size   301.84 KB

NetSerializer
      Serialize   5555.3857 ms
    Deserialize   4050.7059 ms
    Binary Size   301.84 KB

RePacker
      Serialize   1676.7775 ms
 Serialize-Auto   4042.0152 ms
    Deserialize   2428.1084 ms
    Binary Size   301.84 KB

Vector3[100] 10000 Iteration

MessagePack C#
      Serialize   25.2017 ms
    Deserialize   38.6729 ms
    Binary Size   1.57 KB

MessagePack C# LZ4
      Serialize   59.6591 ms
    Deserialize   37.6376 ms
    Binary Size   45.00 B

ZeroFormatter
      Serialize   22.4488 ms
    Deserialize   19.7394 ms
    Binary Size   1.18 KB

MsgPack-CLI
      Serialize   217.0555 ms
    Deserialize   576.4621 ms
    Binary Size   1.66 KB

protobuf-net
      Serialize   458.7125 ms
    Deserialize   473.4946 ms
    Binary Size   1.66 KB

Wire
      Serialize   136.7039 ms
    Deserialize   147.3404 ms
    Binary Size   1.57 KB

NetSerializer
      Serialize   83.0969 ms
    Deserialize   63.6148 ms
    Binary Size   1.47 KB

RePacker
      Serialize   0.4082 ms
 Serialize-Auto   3.1202 ms
    Deserialize   1.7196 ms
    Binary Size   1.18 KB
```
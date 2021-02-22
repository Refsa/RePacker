Benchmarks from https://github.com/neuecc/ZeroFormatter

RePacker is built in net4.6.1 mode

Ran on an I5-4670K @ 4.3GHz

```
Small Object(int,string,string,enum) 10000 Iteration

MessagePack C#
      Serialize   1.4161 ms
    Deserialize   1.3619 ms
    Binary Size   22.00 B

MessagePack C# LZ4
      Serialize   1.775 ms
    Deserialize   1.6123 ms
    Binary Size   22.00 B

ZeroFormatter
      Serialize   2.4384 ms
    Deserialize   2.044 ms
    Binary Size   50.00 B

protobuf-net
      Serialize   4.2191 ms
    Deserialize   5.1431 ms
    Binary Size   23.00 B

MsgPack-CLI
      Serialize   6.0679 ms
    Deserialize   10.0801 ms
    Binary Size   26.00 B

JSON.NET
      Serialize   21.3305 ms
    Deserialize   25.244 ms
    Binary Size   66.00 B

Jil
      Serialize   10.1206 ms
    Deserialize   10.3804 ms
    Binary Size   71.00 B

FsPickler
      Serialize   10.188 ms
    Deserialize   8.0458 ms
    Binary Size   44.00 B

BinaryFormatter
      Serialize   42.7625 ms
    Deserialize   49.4241 ms
    Binary Size   285.00 B

DataContractSerializer
      Serialize   13.1998 ms
    Deserialize   47.4062 ms
    Binary Size   206.00 B

FlatBuffers
      Serialize   7.5222 ms
    Deserialize   0.3583 ms
    Binary Size   60.00 B

Google.Protobuf
      Serialize   4.6144 ms
    Deserialize   1.5204 ms
    Binary Size   23.00 B

Wire
      Serialize   4.6084 ms
    Deserialize   7.0546 ms
    Binary Size   119.00 B

NetSerializer
      Serialize   3.6722 ms
    Deserialize   2.7285 ms
    Binary Size   21.00 B

RePacker
      Serialize   0.6807 ms
    Deserialize   1.1537 ms
    Binary Size   34.00 B

Large Array(SmallObject[1000]) 10000 Iteration

MessagePack C#
      Serialize   922.6622 ms
    Deserialize   1364.6661 ms
    Binary Size   19.53 KB

MessagePack C# LZ4
      Serialize   1052.8201 ms
    Deserialize   1427.228 ms
    Binary Size   4.92 KB

ZeroFormatter
      Serialize   1181.9315 ms
    Deserialize   1571.3185 ms
    Binary Size   48.83 KB

protobuf-net
      Serialize   3105.8202 ms
    Deserialize   4432.4646 ms
    Binary Size   23.44 KB

MsgPack-CLI
      Serialize   2967.8371 ms
    Deserialize   9455.4117 ms
    Binary Size   25.39 KB

JSON.NET
      Serialize   8675.4526 ms
    Deserialize   15884.4734 ms
    Binary Size   61.53 KB

Jil
      Serialize   2673.4804 ms
    Deserialize   5024.777 ms
    Binary Size   68.36 KB

FsPickler
      Serialize   3209.1945 ms
    Deserialize   3346.5425 ms
    Binary Size   25.42 KB

BinaryFormatter
      Serialize   19028.9329 ms
    Deserialize   20239.759 ms
    Binary Size   37.38 KB

DataContractSerializer
      Serialize   5740.4942 ms
    Deserialize   18444.361 ms
    Binary Size   103.64 KB

FlatBuffers
      Serialize   2595.5348 ms
    Deserialize   0.3582 ms
    Binary Size   46.91 KB

Google.Protobuf
      Serialize   971.1356 ms
    Deserialize   1571.7644 ms
    Binary Size   23.44 KB

Wire
      Serialize   2872.0506 ms
    Deserialize   3527.8949 ms
    Binary Size   25.54 KB

NetSerializer
      Serialize   1902.7132 ms
    Deserialize   2753.3041 ms
    Binary Size   20.51 KB

RePacker
      Serialize   559.9482 ms
    Deserialize   1053.6444 ms
    Binary Size   33.21 KB


Additional Benchmarks

Int32(1) 10000 Iteration

MessagePack C#
      Serialize   0.4748 ms
    Deserialize   0.2196 ms
    Binary Size   5.00 B

MessagePack C# LZ4
      Serialize   0.71 ms
    Deserialize   0.2691 ms
    Binary Size   5.00 B

ZeroFormatter
      Serialize   0.3034 ms
    Deserialize   0.16 ms
    Binary Size   4.00 B

MsgPack-CLI
      Serialize   2.9334 ms
    Deserialize   2.5216 ms
    Binary Size   5.00 B

protobuf-net
      Serialize   7.5828 ms
    Deserialize   6.5364 ms
    Binary Size   6.00 B

Wire
      Serialize   1.8877 ms
    Deserialize   0.4625 ms
    Binary Size   5.00 B

NetSerializer
      Serialize   1.4918 ms
    Deserialize   0.3571 ms
    Binary Size   5.00 B

RePacker
      Serialize   0.1078 ms
    Deserialize   0.1027 ms
    Binary Size   4.00 B

Bois
      Serialize   4.627 ms
    Deserialize   3.5584 ms
    Binary Size   5.00 B

Vector3(float, float, float) 10000 Iteration

MessagePack C#
      Serialize   0.5641 ms
    Deserialize   0.3813 ms
    Binary Size   16.00 B

MessagePack C# LZ4
      Serialize   0.853 ms
    Deserialize   0.519 ms
    Binary Size   16.00 B

ZeroFormatter
      Serialize   0.4516 ms
    Deserialize   0.3233 ms
    Binary Size   12.00 B

MsgPack-CLI
      Serialize   4.6473 ms
    Deserialize   5.3198 ms
    Binary Size   17.00 B

protobuf-net
      Serialize   3.6408 ms
    Deserialize   3.3201 ms
    Binary Size   15.00 B

Wire
      Serialize   2.6402 ms
    Deserialize   2.8844 ms
    Binary Size   64.00 B

NetSerializer
      Serialize   1.8287 ms
    Deserialize   0.9415 ms
    Binary Size   15.00 B

RePacker
      Serialize   0.2242 ms
    Deserialize   0.2121 ms
    Binary Size   12.00 B

HtmlString(309081bytes) 10000 Iteration

MessagePack C#
      Serialize   3841.9993 ms
    Deserialize   2243.1229 ms
    Binary Size   301.84 KB

MessagePack C# LZ4
      Serialize   8985.3819 ms
    Deserialize   3654.6184 ms
    Binary Size   85.93 KB

ZeroFormatter
      Serialize   3297.5321 ms
    Deserialize   2267.5609 ms
    Binary Size   301.84 KB

MsgPack-CLI
      Serialize   4359.5222 ms
    Deserialize   2692.8862 ms
    Binary Size   301.84 KB

protobuf-net
      Serialize   3899.4917 ms
    Deserialize   5968.0224 ms
    Binary Size   301.84 KB

Wire
      Serialize   3846.2853 ms
    Deserialize   2692.9901 ms
    Binary Size   301.84 KB

NetSerializer
      Serialize   5129.5196 ms
    Deserialize   3512.0454 ms
    Binary Size   301.84 KB

RePacker
      Serialize   1519.1072 ms
    Deserialize   2115.3295 ms
    Binary Size   301.84 KB

Vector3[100] 10000 Iteration

MessagePack C#
      Serialize   21.1086 ms
    Deserialize   31.2557 ms
    Binary Size   1.57 KB

MessagePack C# LZ4
      Serialize   51.4363 ms
    Deserialize   33.5014 ms
    Binary Size   45.00 B

ZeroFormatter
      Serialize   20.1812 ms
    Deserialize   16.523 ms
    Binary Size   1.18 KB

MsgPack-CLI
      Serialize   146.9428 ms
    Deserialize   492.2714 ms
    Binary Size   1.66 KB

protobuf-net
      Serialize   381.8467 ms
    Deserialize   411.2946 ms
    Binary Size   1.66 KB

Wire
      Serialize   116.8124 ms
    Deserialize   125.6865 ms
    Binary Size   1.57 KB

NetSerializer
      Serialize   70.193 ms
    Deserialize   53.7789 ms
    Binary Size   1.47 KB

RePacker
      Serialize   0.9229 ms
    Deserialize   1.5679 ms
    Binary Size   1.18 KB
```
using BenchmarkDotNet.Attributes;
using RePacker.Buffers;
using RePacker.Builder;

namespace RePacker.Benchmarks
{
    [MemoryDiagnoser]
    public class SetupBench
    {
        Buffer intBuffer;
        public SetupBench()
        {
            int val = 123456789;
            intBuffer = new Buffer(1024);
            RePacking.Pack<int>(intBuffer, ref val);
        }

        /* [Benchmark]
        public void BenchSetupTime()
        {
            RePacker.Init();
        } */

        // netcoreapp3.0

        // Raw buffer speed
        // | IntSerialize10K   |  55.86 us
        // | IntDeserialize10K |  45.87 us

        // After buffer fixes
        // |   IntSerialize10K | 43.94 us
        // | IntDeserialize10K | 45.83 us

        // Dictionary Lookup
        // |    ILGen_IntSerialize10K | 367.0 us
        // |  ILGen_IntDeserialize10K | 381.1 us
        // | ILGen_VectorSerialize10K | 521.9 us

        // Expression Tree Switch
        // |    ILGen_IntSerialize10K | 242.6 us
        // |  ILGen_IntDeserialize10K | 227.2 us
        // | ILGen_VectorSerialize10K | 387.6 us

        // Expression Tree Switch without dictionary branch
        // |    ILGen_IntSerialize10K | 180.7 us
        // |  ILGen_IntDeserialize10K | 173.7 us
        // | ILGen_VectorSerialize10K | 319.3 us
        
        // after buffer fixes
        // |    ILGen_IntSerialize10K | 164.3 us
        // |  ILGen_IntDeserialize10K | 164.3 us
        // | ILGen_VectorSerialize10K | 338.2 us

        // IL Generated If/Else
        // |    ILGen_IntSerialize10K | 344.4 us
        // |  ILGen_IntDeserialize10K | 331.5 us
        // | ILGen_VectorSerialize10K | 704.0 us

        /* [Benchmark]
        public void ILGen_IntSerialize10K()
        {
            var buffer = new Buffer(1 << 16);
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                buffer.Pack<int>(ref val);
            }
        } */

        [Benchmark]
        public void ILGen_IntDeserialize10K()
        {
            for (int i = 0; i < 10_000; i++)
            {
                var val = RePacking.Unpack<int>(intBuffer);
                intBuffer.Reset();
            }
        }

        /* [RePacker]
        public struct Vector
        {
            public float X;
            public float Y;
            public float Z;
        }

        [Benchmark]
        public void ILGen_VectorSerialize10K()
        {
            Vector vector = new Vector { X = 1f, Y = 2f, Z = 3f };
            var Buffer = new Buffer(1 << 20);
            for (int i = 0; i < 10_000; i++)
            {
                Buffer.Pack(ref vector);
            }
        } */

        /* [Benchmark]
        public void IntSerialize10K()
        {
            var buffer = new Buffer(new byte[1 << 16]);
            int val = 123456789;

            for (int i = 0; i < 10_000; i++)
            {
                buffer.PackInt(ref val);
            }
        }

        [Benchmark]
        public void IntDeserialize10K()
        {
            var buffer = new Buffer(new byte[1 << 16]);
            int val = 123456789;
            buffer.Pack(ref val);

            for (int i = 0; i < 10_000; i++)
            {
                buffer.UnpackInt(out int _);
                buffer.Reset();
            }
        } */

        /* [Benchmark]
        public void WrappedIntSerialize10K()
        {
            var buffer = new Buffer(1 << 16);
            var val = new JustFiller673 { Int = 123456789 };

            for (int i = 0; i < 10_000; i++)
            {
                buffer.Pack(ref val);
            }
        }

        [Benchmark]
        public void WrappedIntDeserialize10K()
        {
            var buffer = new Buffer(1 << 16);
            var val = new JustFiller673 { Int = 123456789 };
            buffer.Pack(ref val);

            for (int i = 0; i < 10_000; i++)
            {
                var _ = buffer.Unpack<JustFiller673>();
            }
        } */

        /* [Benchmark]
        public void TonsOfTypes()
        {
            var js = new JustFiller673 { Int = 1234567890 };

            var buffer = new Buffer(1024);

            for (int i = 0; i < 10_000; i++)
            {
                buffer.Pack(ref js);
                buffer.Reset();
            }
        } */

        /* [RePacker] public struct JustFiller0 { public int Int; }
        [RePacker] public struct JustFiller1 { public int Int; }
        [RePacker] public struct JustFiller2 { public int Int; }
        [RePacker] public struct JustFiller3 { public int Int; }
        [RePacker] public struct JustFiller4 { public int Int; }
        [RePacker] public struct JustFiller5 { public int Int; }
        [RePacker] public struct JustFiller6 { public int Int; }
        [RePacker] public struct JustFiller7 { public int Int; }
        [RePacker] public struct JustFiller8 { public int Int; }
        [RePacker] public struct JustFiller9 { public int Int; }
        [RePacker] public struct JustFiller10 { public int Int; }
        [RePacker] public struct JustFiller11 { public int Int; }
        [RePacker] public struct JustFiller12 { public int Int; }
        [RePacker] public struct JustFiller13 { public int Int; }
        [RePacker] public struct JustFiller14 { public int Int; }
        [RePacker] public struct JustFiller15 { public int Int; }
        [RePacker] public struct JustFiller16 { public int Int; }
        [RePacker] public struct JustFiller17 { public int Int; }
        [RePacker] public struct JustFiller18 { public int Int; }
        [RePacker] public struct JustFiller19 { public int Int; }
        [RePacker] public struct JustFiller20 { public int Int; }
        [RePacker] public struct JustFiller21 { public int Int; }
        [RePacker] public struct JustFiller22 { public int Int; }
        [RePacker] public struct JustFiller23 { public int Int; }
        [RePacker] public struct JustFiller24 { public int Int; }
        [RePacker] public struct JustFiller25 { public int Int; }
        [RePacker] public struct JustFiller26 { public int Int; }
        [RePacker] public struct JustFiller27 { public int Int; }
        [RePacker] public struct JustFiller28 { public int Int; }
        [RePacker] public struct JustFiller29 { public int Int; }
        [RePacker] public struct JustFiller30 { public int Int; }
        [RePacker] public struct JustFiller31 { public int Int; }
        [RePacker] public struct JustFiller32 { public int Int; }
        [RePacker] public struct JustFiller33 { public int Int; }
        [RePacker] public struct JustFiller34 { public int Int; }
        [RePacker] public struct JustFiller35 { public int Int; }
        [RePacker] public struct JustFiller36 { public int Int; }
        [RePacker] public struct JustFiller37 { public int Int; }
        [RePacker] public struct JustFiller38 { public int Int; }
        [RePacker] public struct JustFiller39 { public int Int; }
        [RePacker] public struct JustFiller40 { public int Int; }
        [RePacker] public struct JustFiller41 { public int Int; }
        [RePacker] public struct JustFiller42 { public int Int; }
        [RePacker] public struct JustFiller43 { public int Int; }
        [RePacker] public struct JustFiller44 { public int Int; }
        [RePacker] public struct JustFiller45 { public int Int; }
        [RePacker] public struct JustFiller46 { public int Int; }
        [RePacker] public struct JustFiller47 { public int Int; }
        [RePacker] public struct JustFiller48 { public int Int; }
        [RePacker] public struct JustFiller49 { public int Int; }
        [RePacker] public struct JustFiller50 { public int Int; }
        [RePacker] public struct JustFiller51 { public int Int; }
        [RePacker] public struct JustFiller52 { public int Int; }
        [RePacker] public struct JustFiller53 { public int Int; }
        [RePacker] public struct JustFiller54 { public int Int; }
        [RePacker] public struct JustFiller55 { public int Int; }
        [RePacker] public struct JustFiller56 { public int Int; }
        [RePacker] public struct JustFiller57 { public int Int; }
        [RePacker] public struct JustFiller58 { public int Int; }
        [RePacker] public struct JustFiller59 { public int Int; }
        [RePacker] public struct JustFiller60 { public int Int; }
        [RePacker] public struct JustFiller61 { public int Int; }
        [RePacker] public struct JustFiller62 { public int Int; }
        [RePacker] public struct JustFiller63 { public int Int; }
        [RePacker] public struct JustFiller64 { public int Int; }
        [RePacker] public struct JustFiller65 { public int Int; }
        [RePacker] public struct JustFiller66 { public int Int; }
        [RePacker] public struct JustFiller67 { public int Int; }
        [RePacker] public struct JustFiller68 { public int Int; }
        [RePacker] public struct JustFiller69 { public int Int; }
        [RePacker] public struct JustFiller70 { public int Int; }
        [RePacker] public struct JustFiller71 { public int Int; }
        [RePacker] public struct JustFiller72 { public int Int; }
        [RePacker] public struct JustFiller73 { public int Int; }
        [RePacker] public struct JustFiller74 { public int Int; }
        [RePacker] public struct JustFiller75 { public int Int; }
        [RePacker] public struct JustFiller76 { public int Int; }
        [RePacker] public struct JustFiller77 { public int Int; }
        [RePacker] public struct JustFiller78 { public int Int; }
        [RePacker] public struct JustFiller79 { public int Int; }
        [RePacker] public struct JustFiller80 { public int Int; }
        [RePacker] public struct JustFiller81 { public int Int; }
        [RePacker] public struct JustFiller82 { public int Int; }
        [RePacker] public struct JustFiller83 { public int Int; }
        [RePacker] public struct JustFiller84 { public int Int; }
        [RePacker] public struct JustFiller85 { public int Int; }
        [RePacker] public struct JustFiller86 { public int Int; }
        [RePacker] public struct JustFiller87 { public int Int; }
        [RePacker] public struct JustFiller88 { public int Int; }
        [RePacker] public struct JustFiller89 { public int Int; }
        [RePacker] public struct JustFiller90 { public int Int; }
        [RePacker] public struct JustFiller91 { public int Int; }
        [RePacker] public struct JustFiller92 { public int Int; }
        [RePacker] public struct JustFiller93 { public int Int; }
        [RePacker] public struct JustFiller94 { public int Int; }
        [RePacker] public struct JustFiller95 { public int Int; }
        [RePacker] public struct JustFiller96 { public int Int; }
        [RePacker] public struct JustFiller97 { public int Int; }
        [RePacker] public struct JustFiller98 { public int Int; }
        [RePacker] public struct JustFiller99 { public int Int; }
        [RePacker] public struct JustFiller100 { public int Int; }
        [RePacker] public struct JustFiller101 { public int Int; }
        [RePacker] public struct JustFiller102 { public int Int; }
        [RePacker] public struct JustFiller103 { public int Int; }
        [RePacker] public struct JustFiller104 { public int Int; }
        [RePacker] public struct JustFiller105 { public int Int; }
        [RePacker] public struct JustFiller106 { public int Int; }
        [RePacker] public struct JustFiller107 { public int Int; }
        [RePacker] public struct JustFiller108 { public int Int; }
        [RePacker] public struct JustFiller109 { public int Int; }
        [RePacker] public struct JustFiller110 { public int Int; }
        [RePacker] public struct JustFiller111 { public int Int; }
        [RePacker] public struct JustFiller112 { public int Int; }
        [RePacker] public struct JustFiller113 { public int Int; }
        [RePacker] public struct JustFiller114 { public int Int; }
        [RePacker] public struct JustFiller115 { public int Int; }
        [RePacker] public struct JustFiller116 { public int Int; }
        [RePacker] public struct JustFiller117 { public int Int; }
        [RePacker] public struct JustFiller118 { public int Int; }
        [RePacker] public struct JustFiller119 { public int Int; }
        [RePacker] public struct JustFiller120 { public int Int; }
        [RePacker] public struct JustFiller121 { public int Int; }
        [RePacker] public struct JustFiller122 { public int Int; }
        [RePacker] public struct JustFiller123 { public int Int; }
        [RePacker] public struct JustFiller124 { public int Int; }
        [RePacker] public struct JustFiller125 { public int Int; }
        [RePacker] public struct JustFiller126 { public int Int; }
        [RePacker] public struct JustFiller127 { public int Int; }
        [RePacker] public struct JustFiller128 { public int Int; }
        [RePacker] public struct JustFiller129 { public int Int; }
        [RePacker] public struct JustFiller130 { public int Int; }
        [RePacker] public struct JustFiller131 { public int Int; }
        [RePacker] public struct JustFiller132 { public int Int; }
        [RePacker] public struct JustFiller133 { public int Int; }
        [RePacker] public struct JustFiller134 { public int Int; }
        [RePacker] public struct JustFiller135 { public int Int; }
        [RePacker] public struct JustFiller136 { public int Int; }
        [RePacker] public struct JustFiller137 { public int Int; }
        [RePacker] public struct JustFiller138 { public int Int; }
        [RePacker] public struct JustFiller139 { public int Int; }
        [RePacker] public struct JustFiller140 { public int Int; }
        [RePacker] public struct JustFiller141 { public int Int; }
        [RePacker] public struct JustFiller142 { public int Int; }
        [RePacker] public struct JustFiller143 { public int Int; }
        [RePacker] public struct JustFiller144 { public int Int; }
        [RePacker] public struct JustFiller145 { public int Int; }
        [RePacker] public struct JustFiller146 { public int Int; }
        [RePacker] public struct JustFiller147 { public int Int; }
        [RePacker] public struct JustFiller148 { public int Int; }
        [RePacker] public struct JustFiller149 { public int Int; }
        [RePacker] public struct JustFiller150 { public int Int; }
        [RePacker] public struct JustFiller151 { public int Int; }
        [RePacker] public struct JustFiller152 { public int Int; }
        [RePacker] public struct JustFiller153 { public int Int; }
        [RePacker] public struct JustFiller154 { public int Int; }
        [RePacker] public struct JustFiller155 { public int Int; }
        [RePacker] public struct JustFiller156 { public int Int; }
        [RePacker] public struct JustFiller157 { public int Int; }
        [RePacker] public struct JustFiller158 { public int Int; }
        [RePacker] public struct JustFiller159 { public int Int; }
        [RePacker] public struct JustFiller160 { public int Int; }
        [RePacker] public struct JustFiller161 { public int Int; }
        [RePacker] public struct JustFiller162 { public int Int; }
        [RePacker] public struct JustFiller163 { public int Int; }
        [RePacker] public struct JustFiller164 { public int Int; }
        [RePacker] public struct JustFiller165 { public int Int; }
        [RePacker] public struct JustFiller166 { public int Int; }
        [RePacker] public struct JustFiller167 { public int Int; }
        [RePacker] public struct JustFiller168 { public int Int; }
        [RePacker] public struct JustFiller169 { public int Int; }
        [RePacker] public struct JustFiller170 { public int Int; }
        [RePacker] public struct JustFiller171 { public int Int; }
        [RePacker] public struct JustFiller172 { public int Int; }
        [RePacker] public struct JustFiller173 { public int Int; }
        [RePacker] public struct JustFiller174 { public int Int; }
        [RePacker] public struct JustFiller175 { public int Int; }
        [RePacker] public struct JustFiller176 { public int Int; }
        [RePacker] public struct JustFiller177 { public int Int; }
        [RePacker] public struct JustFiller178 { public int Int; }
        [RePacker] public struct JustFiller179 { public int Int; }
        [RePacker] public struct JustFiller180 { public int Int; }
        [RePacker] public struct JustFiller181 { public int Int; }
        [RePacker] public struct JustFiller182 { public int Int; }
        [RePacker] public struct JustFiller183 { public int Int; }
        [RePacker] public struct JustFiller184 { public int Int; }
        [RePacker] public struct JustFiller185 { public int Int; }
        [RePacker] public struct JustFiller186 { public int Int; }
        [RePacker] public struct JustFiller187 { public int Int; }
        [RePacker] public struct JustFiller188 { public int Int; }
        [RePacker] public struct JustFiller189 { public int Int; }
        [RePacker] public struct JustFiller190 { public int Int; }
        [RePacker] public struct JustFiller191 { public int Int; }
        [RePacker] public struct JustFiller192 { public int Int; }
        [RePacker] public struct JustFiller193 { public int Int; }
        [RePacker] public struct JustFiller194 { public int Int; }
        [RePacker] public struct JustFiller195 { public int Int; }
        [RePacker] public struct JustFiller196 { public int Int; }
        [RePacker] public struct JustFiller197 { public int Int; }
        [RePacker] public struct JustFiller198 { public int Int; }
        [RePacker] public struct JustFiller199 { public int Int; }
        [RePacker] public struct JustFiller200 { public int Int; }
        [RePacker] public struct JustFiller201 { public int Int; }
        [RePacker] public struct JustFiller202 { public int Int; }
        [RePacker] public struct JustFiller203 { public int Int; }
        [RePacker] public struct JustFiller204 { public int Int; }
        [RePacker] public struct JustFiller205 { public int Int; }
        [RePacker] public struct JustFiller206 { public int Int; }
        [RePacker] public struct JustFiller207 { public int Int; }
        [RePacker] public struct JustFiller208 { public int Int; }
        [RePacker] public struct JustFiller209 { public int Int; }
        [RePacker] public struct JustFiller210 { public int Int; }
        [RePacker] public struct JustFiller211 { public int Int; }
        [RePacker] public struct JustFiller212 { public int Int; }
        [RePacker] public struct JustFiller213 { public int Int; }
        [RePacker] public struct JustFiller214 { public int Int; }
        [RePacker] public struct JustFiller215 { public int Int; }
        [RePacker] public struct JustFiller216 { public int Int; }
        [RePacker] public struct JustFiller217 { public int Int; }
        [RePacker] public struct JustFiller218 { public int Int; }
        [RePacker] public struct JustFiller219 { public int Int; }
        [RePacker] public struct JustFiller220 { public int Int; }
        [RePacker] public struct JustFiller221 { public int Int; }
        [RePacker] public struct JustFiller222 { public int Int; }
        [RePacker] public struct JustFiller223 { public int Int; }
        [RePacker] public struct JustFiller224 { public int Int; }
        [RePacker] public struct JustFiller225 { public int Int; }
        [RePacker] public struct JustFiller226 { public int Int; }
        [RePacker] public struct JustFiller227 { public int Int; }
        [RePacker] public struct JustFiller228 { public int Int; }
        [RePacker] public struct JustFiller229 { public int Int; }
        [RePacker] public struct JustFiller230 { public int Int; }
        [RePacker] public struct JustFiller231 { public int Int; }
        [RePacker] public struct JustFiller232 { public int Int; }
        [RePacker] public struct JustFiller233 { public int Int; }
        [RePacker] public struct JustFiller234 { public int Int; }
        [RePacker] public struct JustFiller235 { public int Int; }
        [RePacker] public struct JustFiller236 { public int Int; }
        [RePacker] public struct JustFiller237 { public int Int; }
        [RePacker] public struct JustFiller238 { public int Int; }
        [RePacker] public struct JustFiller239 { public int Int; }
        [RePacker] public struct JustFiller240 { public int Int; }
        [RePacker] public struct JustFiller241 { public int Int; }
        [RePacker] public struct JustFiller242 { public int Int; }
        [RePacker] public struct JustFiller243 { public int Int; }
        [RePacker] public struct JustFiller244 { public int Int; }
        [RePacker] public struct JustFiller245 { public int Int; }
        [RePacker] public struct JustFiller246 { public int Int; }
        [RePacker] public struct JustFiller247 { public int Int; }
        [RePacker] public struct JustFiller248 { public int Int; }
        [RePacker] public struct JustFiller249 { public int Int; }
        [RePacker] public struct JustFiller250 { public int Int; }
        [RePacker] public struct JustFiller251 { public int Int; }
        [RePacker] public struct JustFiller252 { public int Int; }
        [RePacker] public struct JustFiller253 { public int Int; }
        [RePacker] public struct JustFiller254 { public int Int; }
        [RePacker] public struct JustFiller255 { public int Int; }
        [RePacker] public struct JustFiller256 { public int Int; }
        [RePacker] public struct JustFiller257 { public int Int; }
        [RePacker] public struct JustFiller258 { public int Int; }
        [RePacker] public struct JustFiller259 { public int Int; }
        [RePacker] public struct JustFiller260 { public int Int; }
        [RePacker] public struct JustFiller261 { public int Int; }
        [RePacker] public struct JustFiller262 { public int Int; }
        [RePacker] public struct JustFiller263 { public int Int; }
        [RePacker] public struct JustFiller264 { public int Int; }
        [RePacker] public struct JustFiller265 { public int Int; }
        [RePacker] public struct JustFiller266 { public int Int; }
        [RePacker] public struct JustFiller267 { public int Int; }
        [RePacker] public struct JustFiller268 { public int Int; }
        [RePacker] public struct JustFiller269 { public int Int; }
        [RePacker] public struct JustFiller270 { public int Int; }
        [RePacker] public struct JustFiller271 { public int Int; }
        [RePacker] public struct JustFiller272 { public int Int; }
        [RePacker] public struct JustFiller273 { public int Int; }
        [RePacker] public struct JustFiller274 { public int Int; }
        [RePacker] public struct JustFiller275 { public int Int; }
        [RePacker] public struct JustFiller276 { public int Int; }
        [RePacker] public struct JustFiller277 { public int Int; }
        [RePacker] public struct JustFiller278 { public int Int; }
        [RePacker] public struct JustFiller279 { public int Int; }
        [RePacker] public struct JustFiller280 { public int Int; }
        [RePacker] public struct JustFiller281 { public int Int; }
        [RePacker] public struct JustFiller282 { public int Int; }
        [RePacker] public struct JustFiller283 { public int Int; }
        [RePacker] public struct JustFiller284 { public int Int; }
        [RePacker] public struct JustFiller285 { public int Int; }
        [RePacker] public struct JustFiller286 { public int Int; }
        [RePacker] public struct JustFiller287 { public int Int; }
        [RePacker] public struct JustFiller288 { public int Int; }
        [RePacker] public struct JustFiller289 { public int Int; }
        [RePacker] public struct JustFiller290 { public int Int; }
        [RePacker] public struct JustFiller291 { public int Int; }
        [RePacker] public struct JustFiller292 { public int Int; }
        [RePacker] public struct JustFiller293 { public int Int; }
        [RePacker] public struct JustFiller294 { public int Int; }
        [RePacker] public struct JustFiller295 { public int Int; }
        [RePacker] public struct JustFiller296 { public int Int; }
        [RePacker] public struct JustFiller297 { public int Int; }
        [RePacker] public struct JustFiller298 { public int Int; }
        [RePacker] public struct JustFiller299 { public int Int; }
        [RePacker] public struct JustFiller300 { public int Int; }
        [RePacker] public struct JustFiller301 { public int Int; }
        [RePacker] public struct JustFiller302 { public int Int; }
        [RePacker] public struct JustFiller303 { public int Int; }
        [RePacker] public struct JustFiller304 { public int Int; }
        [RePacker] public struct JustFiller305 { public int Int; }
        [RePacker] public struct JustFiller306 { public int Int; }
        [RePacker] public struct JustFiller307 { public int Int; }
        [RePacker] public struct JustFiller308 { public int Int; }
        [RePacker] public struct JustFiller309 { public int Int; }
        [RePacker] public struct JustFiller310 { public int Int; }
        [RePacker] public struct JustFiller311 { public int Int; }
        [RePacker] public struct JustFiller312 { public int Int; }
        [RePacker] public struct JustFiller313 { public int Int; }
        [RePacker] public struct JustFiller314 { public int Int; }
        [RePacker] public struct JustFiller315 { public int Int; }
        [RePacker] public struct JustFiller316 { public int Int; }
        [RePacker] public struct JustFiller317 { public int Int; }
        [RePacker] public struct JustFiller318 { public int Int; }
        [RePacker] public struct JustFiller319 { public int Int; }
        [RePacker] public struct JustFiller320 { public int Int; }
        [RePacker] public struct JustFiller321 { public int Int; }
        [RePacker] public struct JustFiller322 { public int Int; }
        [RePacker] public struct JustFiller323 { public int Int; }
        [RePacker] public struct JustFiller324 { public int Int; }
        [RePacker] public struct JustFiller325 { public int Int; }
        [RePacker] public struct JustFiller326 { public int Int; }
        [RePacker] public struct JustFiller327 { public int Int; }
        [RePacker] public struct JustFiller328 { public int Int; }
        [RePacker] public struct JustFiller329 { public int Int; }
        [RePacker] public struct JustFiller330 { public int Int; }
        [RePacker] public struct JustFiller331 { public int Int; }
        [RePacker] public struct JustFiller332 { public int Int; }
        [RePacker] public struct JustFiller333 { public int Int; }
        [RePacker] public struct JustFiller334 { public int Int; }
        [RePacker] public struct JustFiller335 { public int Int; }
        [RePacker] public struct JustFiller336 { public int Int; }
        [RePacker] public struct JustFiller337 { public int Int; }
        [RePacker] public struct JustFiller338 { public int Int; }
        [RePacker] public struct JustFiller339 { public int Int; }
        [RePacker] public struct JustFiller340 { public int Int; }
        [RePacker] public struct JustFiller341 { public int Int; }
        [RePacker] public struct JustFiller342 { public int Int; }
        [RePacker] public struct JustFiller343 { public int Int; }
        [RePacker] public struct JustFiller344 { public int Int; }
        [RePacker] public struct JustFiller345 { public int Int; }
        [RePacker] public struct JustFiller346 { public int Int; }
        [RePacker] public struct JustFiller347 { public int Int; }
        [RePacker] public struct JustFiller348 { public int Int; }
        [RePacker] public struct JustFiller349 { public int Int; }
        [RePacker] public struct JustFiller350 { public int Int; }
        [RePacker] public struct JustFiller351 { public int Int; }
        [RePacker] public struct JustFiller352 { public int Int; }
        [RePacker] public struct JustFiller353 { public int Int; }
        [RePacker] public struct JustFiller354 { public int Int; }
        [RePacker] public struct JustFiller355 { public int Int; }
        [RePacker] public struct JustFiller356 { public int Int; }
        [RePacker] public struct JustFiller357 { public int Int; }
        [RePacker] public struct JustFiller358 { public int Int; }
        [RePacker] public struct JustFiller359 { public int Int; }
        [RePacker] public struct JustFiller360 { public int Int; }
        [RePacker] public struct JustFiller361 { public int Int; }
        [RePacker] public struct JustFiller362 { public int Int; }
        [RePacker] public struct JustFiller363 { public int Int; }
        [RePacker] public struct JustFiller364 { public int Int; }
        [RePacker] public struct JustFiller365 { public int Int; }
        [RePacker] public struct JustFiller366 { public int Int; }
        [RePacker] public struct JustFiller367 { public int Int; }
        [RePacker] public struct JustFiller368 { public int Int; }
        [RePacker] public struct JustFiller369 { public int Int; }
        [RePacker] public struct JustFiller370 { public int Int; }
        [RePacker] public struct JustFiller371 { public int Int; }
        [RePacker] public struct JustFiller372 { public int Int; }
        [RePacker] public struct JustFiller373 { public int Int; }
        [RePacker] public struct JustFiller374 { public int Int; }
        [RePacker] public struct JustFiller375 { public int Int; }
        [RePacker] public struct JustFiller376 { public int Int; }
        [RePacker] public struct JustFiller377 { public int Int; }
        [RePacker] public struct JustFiller378 { public int Int; }
        [RePacker] public struct JustFiller379 { public int Int; }
        [RePacker] public struct JustFiller380 { public int Int; }
        [RePacker] public struct JustFiller381 { public int Int; }
        [RePacker] public struct JustFiller382 { public int Int; }
        [RePacker] public struct JustFiller383 { public int Int; }
        [RePacker] public struct JustFiller384 { public int Int; }
        [RePacker] public struct JustFiller385 { public int Int; }
        [RePacker] public struct JustFiller386 { public int Int; }
        [RePacker] public struct JustFiller387 { public int Int; }
        [RePacker] public struct JustFiller388 { public int Int; }
        [RePacker] public struct JustFiller389 { public int Int; }
        [RePacker] public struct JustFiller390 { public int Int; }
        [RePacker] public struct JustFiller391 { public int Int; }
        [RePacker] public struct JustFiller392 { public int Int; }
        [RePacker] public struct JustFiller393 { public int Int; }
        [RePacker] public struct JustFiller394 { public int Int; }
        [RePacker] public struct JustFiller395 { public int Int; }
        [RePacker] public struct JustFiller396 { public int Int; }
        [RePacker] public struct JustFiller397 { public int Int; }
        [RePacker] public struct JustFiller398 { public int Int; }
        [RePacker] public struct JustFiller399 { public int Int; }
        [RePacker] public struct JustFiller400 { public int Int; }
        [RePacker] public struct JustFiller401 { public int Int; }
        [RePacker] public struct JustFiller402 { public int Int; }
        [RePacker] public struct JustFiller403 { public int Int; }
        [RePacker] public struct JustFiller404 { public int Int; }
        [RePacker] public struct JustFiller405 { public int Int; }
        [RePacker] public struct JustFiller406 { public int Int; }
        [RePacker] public struct JustFiller407 { public int Int; }
        [RePacker] public struct JustFiller408 { public int Int; }
        [RePacker] public struct JustFiller409 { public int Int; }
        [RePacker] public struct JustFiller410 { public int Int; }
        [RePacker] public struct JustFiller411 { public int Int; }
        [RePacker] public struct JustFiller412 { public int Int; }
        [RePacker] public struct JustFiller413 { public int Int; }
        [RePacker] public struct JustFiller414 { public int Int; }
        [RePacker] public struct JustFiller415 { public int Int; }
        [RePacker] public struct JustFiller416 { public int Int; }
        [RePacker] public struct JustFiller417 { public int Int; }
        [RePacker] public struct JustFiller418 { public int Int; }
        [RePacker] public struct JustFiller419 { public int Int; }
        [RePacker] public struct JustFiller420 { public int Int; }
        [RePacker] public struct JustFiller421 { public int Int; }
        [RePacker] public struct JustFiller422 { public int Int; }
        [RePacker] public struct JustFiller423 { public int Int; }
        [RePacker] public struct JustFiller424 { public int Int; }
        [RePacker] public struct JustFiller425 { public int Int; }
        [RePacker] public struct JustFiller426 { public int Int; }
        [RePacker] public struct JustFiller427 { public int Int; }
        [RePacker] public struct JustFiller428 { public int Int; }
        [RePacker] public struct JustFiller429 { public int Int; }
        [RePacker] public struct JustFiller430 { public int Int; }
        [RePacker] public struct JustFiller431 { public int Int; }
        [RePacker] public struct JustFiller432 { public int Int; }
        [RePacker] public struct JustFiller433 { public int Int; }
        [RePacker] public struct JustFiller434 { public int Int; }
        [RePacker] public struct JustFiller435 { public int Int; }
        [RePacker] public struct JustFiller436 { public int Int; }
        [RePacker] public struct JustFiller437 { public int Int; }
        [RePacker] public struct JustFiller438 { public int Int; }
        [RePacker] public struct JustFiller439 { public int Int; }
        [RePacker] public struct JustFiller440 { public int Int; }
        [RePacker] public struct JustFiller441 { public int Int; }
        [RePacker] public struct JustFiller442 { public int Int; }
        [RePacker] public struct JustFiller443 { public int Int; }
        [RePacker] public struct JustFiller444 { public int Int; }
        [RePacker] public struct JustFiller445 { public int Int; }
        [RePacker] public struct JustFiller446 { public int Int; }
        [RePacker] public struct JustFiller447 { public int Int; }
        [RePacker] public struct JustFiller448 { public int Int; }
        [RePacker] public struct JustFiller449 { public int Int; }
        [RePacker] public struct JustFiller450 { public int Int; }
        [RePacker] public struct JustFiller451 { public int Int; }
        [RePacker] public struct JustFiller452 { public int Int; }
        [RePacker] public struct JustFiller453 { public int Int; }
        [RePacker] public struct JustFiller454 { public int Int; }
        [RePacker] public struct JustFiller455 { public int Int; }
        [RePacker] public struct JustFiller456 { public int Int; }
        [RePacker] public struct JustFiller457 { public int Int; }
        [RePacker] public struct JustFiller458 { public int Int; }
        [RePacker] public struct JustFiller459 { public int Int; }
        [RePacker] public struct JustFiller460 { public int Int; }
        [RePacker] public struct JustFiller461 { public int Int; }
        [RePacker] public struct JustFiller462 { public int Int; }
        [RePacker] public struct JustFiller463 { public int Int; }
        [RePacker] public struct JustFiller464 { public int Int; }
        [RePacker] public struct JustFiller465 { public int Int; }
        [RePacker] public struct JustFiller466 { public int Int; }
        [RePacker] public struct JustFiller467 { public int Int; }
        [RePacker] public struct JustFiller468 { public int Int; }
        [RePacker] public struct JustFiller469 { public int Int; }
        [RePacker] public struct JustFiller470 { public int Int; }
        [RePacker] public struct JustFiller471 { public int Int; }
        [RePacker] public struct JustFiller472 { public int Int; }
        [RePacker] public struct JustFiller473 { public int Int; }
        [RePacker] public struct JustFiller474 { public int Int; }
        [RePacker] public struct JustFiller475 { public int Int; }
        [RePacker] public struct JustFiller476 { public int Int; }
        [RePacker] public struct JustFiller477 { public int Int; }
        [RePacker] public struct JustFiller478 { public int Int; }
        [RePacker] public struct JustFiller479 { public int Int; }
        [RePacker] public struct JustFiller480 { public int Int; }
        [RePacker] public struct JustFiller481 { public int Int; }
        [RePacker] public struct JustFiller482 { public int Int; }
        [RePacker] public struct JustFiller483 { public int Int; }
        [RePacker] public struct JustFiller484 { public int Int; }
        [RePacker] public struct JustFiller485 { public int Int; }
        [RePacker] public struct JustFiller486 { public int Int; }
        [RePacker] public struct JustFiller487 { public int Int; }
        [RePacker] public struct JustFiller488 { public int Int; }
        [RePacker] public struct JustFiller489 { public int Int; }
        [RePacker] public struct JustFiller490 { public int Int; }
        [RePacker] public struct JustFiller491 { public int Int; }
        [RePacker] public struct JustFiller492 { public int Int; }
        [RePacker] public struct JustFiller493 { public int Int; }
        [RePacker] public struct JustFiller494 { public int Int; }
        [RePacker] public struct JustFiller495 { public int Int; }
        [RePacker] public struct JustFiller496 { public int Int; }
        [RePacker] public struct JustFiller497 { public int Int; }
        [RePacker] public struct JustFiller498 { public int Int; }
        [RePacker] public struct JustFiller499 { public int Int; }
        [RePacker] public struct JustFiller500 { public int Int; }
        [RePacker] public struct JustFiller501 { public int Int; }
        [RePacker] public struct JustFiller502 { public int Int; }
        [RePacker] public struct JustFiller503 { public int Int; }
        [RePacker] public struct JustFiller504 { public int Int; }
        [RePacker] public struct JustFiller505 { public int Int; }
        [RePacker] public struct JustFiller506 { public int Int; }
        [RePacker] public struct JustFiller507 { public int Int; }
        [RePacker] public struct JustFiller508 { public int Int; }
        [RePacker] public struct JustFiller509 { public int Int; }
        [RePacker] public struct JustFiller510 { public int Int; }
        [RePacker] public struct JustFiller511 { public int Int; }
        [RePacker] public struct JustFiller512 { public int Int; }
        [RePacker] public struct JustFiller513 { public int Int; }
        [RePacker] public struct JustFiller514 { public int Int; }
        [RePacker] public struct JustFiller515 { public int Int; }
        [RePacker] public struct JustFiller516 { public int Int; }
        [RePacker] public struct JustFiller517 { public int Int; }
        [RePacker] public struct JustFiller518 { public int Int; }
        [RePacker] public struct JustFiller519 { public int Int; }
        [RePacker] public struct JustFiller520 { public int Int; }
        [RePacker] public struct JustFiller521 { public int Int; }
        [RePacker] public struct JustFiller522 { public int Int; }
        [RePacker] public struct JustFiller523 { public int Int; }
        [RePacker] public struct JustFiller524 { public int Int; }
        [RePacker] public struct JustFiller525 { public int Int; }
        [RePacker] public struct JustFiller526 { public int Int; }
        [RePacker] public struct JustFiller527 { public int Int; }
        [RePacker] public struct JustFiller528 { public int Int; }
        [RePacker] public struct JustFiller529 { public int Int; }
        [RePacker] public struct JustFiller530 { public int Int; }
        [RePacker] public struct JustFiller531 { public int Int; }
        [RePacker] public struct JustFiller532 { public int Int; }
        [RePacker] public struct JustFiller533 { public int Int; }
        [RePacker] public struct JustFiller534 { public int Int; }
        [RePacker] public struct JustFiller535 { public int Int; }
        [RePacker] public struct JustFiller536 { public int Int; }
        [RePacker] public struct JustFiller537 { public int Int; }
        [RePacker] public struct JustFiller538 { public int Int; }
        [RePacker] public struct JustFiller539 { public int Int; }
        [RePacker] public struct JustFiller540 { public int Int; }
        [RePacker] public struct JustFiller541 { public int Int; }
        [RePacker] public struct JustFiller542 { public int Int; }
        [RePacker] public struct JustFiller543 { public int Int; }
        [RePacker] public struct JustFiller544 { public int Int; }
        [RePacker] public struct JustFiller545 { public int Int; }
        [RePacker] public struct JustFiller546 { public int Int; }
        [RePacker] public struct JustFiller547 { public int Int; }
        [RePacker] public struct JustFiller548 { public int Int; }
        [RePacker] public struct JustFiller549 { public int Int; }
        [RePacker] public struct JustFiller550 { public int Int; }
        [RePacker] public struct JustFiller551 { public int Int; }
        [RePacker] public struct JustFiller552 { public int Int; }
        [RePacker] public struct JustFiller553 { public int Int; }
        [RePacker] public struct JustFiller554 { public int Int; }
        [RePacker] public struct JustFiller555 { public int Int; }
        [RePacker] public struct JustFiller556 { public int Int; }
        [RePacker] public struct JustFiller557 { public int Int; }
        [RePacker] public struct JustFiller558 { public int Int; }
        [RePacker] public struct JustFiller559 { public int Int; }
        [RePacker] public struct JustFiller560 { public int Int; }
        [RePacker] public struct JustFiller561 { public int Int; }
        [RePacker] public struct JustFiller562 { public int Int; }
        [RePacker] public struct JustFiller563 { public int Int; }
        [RePacker] public struct JustFiller564 { public int Int; }
        [RePacker] public struct JustFiller565 { public int Int; }
        [RePacker] public struct JustFiller566 { public int Int; }
        [RePacker] public struct JustFiller567 { public int Int; }
        [RePacker] public struct JustFiller568 { public int Int; }
        [RePacker] public struct JustFiller569 { public int Int; }
        [RePacker] public struct JustFiller570 { public int Int; }
        [RePacker] public struct JustFiller571 { public int Int; }
        [RePacker] public struct JustFiller572 { public int Int; }
        [RePacker] public struct JustFiller573 { public int Int; }
        [RePacker] public struct JustFiller574 { public int Int; }
        [RePacker] public struct JustFiller575 { public int Int; }
        [RePacker] public struct JustFiller576 { public int Int; }
        [RePacker] public struct JustFiller577 { public int Int; }
        [RePacker] public struct JustFiller578 { public int Int; }
        [RePacker] public struct JustFiller579 { public int Int; }
        [RePacker] public struct JustFiller580 { public int Int; }
        [RePacker] public struct JustFiller581 { public int Int; }
        [RePacker] public struct JustFiller582 { public int Int; }
        [RePacker] public struct JustFiller583 { public int Int; }
        [RePacker] public struct JustFiller584 { public int Int; }
        [RePacker] public struct JustFiller585 { public int Int; }
        [RePacker] public struct JustFiller586 { public int Int; }
        [RePacker] public struct JustFiller587 { public int Int; }
        [RePacker] public struct JustFiller588 { public int Int; }
        [RePacker] public struct JustFiller589 { public int Int; }
        [RePacker] public struct JustFiller590 { public int Int; }
        [RePacker] public struct JustFiller591 { public int Int; }
        [RePacker] public struct JustFiller592 { public int Int; }
        [RePacker] public struct JustFiller593 { public int Int; }
        [RePacker] public struct JustFiller594 { public int Int; }
        [RePacker] public struct JustFiller595 { public int Int; }
        [RePacker] public struct JustFiller596 { public int Int; }
        [RePacker] public struct JustFiller597 { public int Int; }
        [RePacker] public struct JustFiller598 { public int Int; }
        [RePacker] public struct JustFiller599 { public int Int; }
        [RePacker] public struct JustFiller600 { public int Int; }
        [RePacker] public struct JustFiller601 { public int Int; }
        [RePacker] public struct JustFiller602 { public int Int; }
        [RePacker] public struct JustFiller603 { public int Int; }
        [RePacker] public struct JustFiller604 { public int Int; }
        [RePacker] public struct JustFiller605 { public int Int; }
        [RePacker] public struct JustFiller606 { public int Int; }
        [RePacker] public struct JustFiller607 { public int Int; }
        [RePacker] public struct JustFiller608 { public int Int; }
        [RePacker] public struct JustFiller609 { public int Int; }
        [RePacker] public struct JustFiller610 { public int Int; }
        [RePacker] public struct JustFiller611 { public int Int; }
        [RePacker] public struct JustFiller612 { public int Int; }
        [RePacker] public struct JustFiller613 { public int Int; }
        [RePacker] public struct JustFiller614 { public int Int; }
        [RePacker] public struct JustFiller615 { public int Int; }
        [RePacker] public struct JustFiller616 { public int Int; }
        [RePacker] public struct JustFiller617 { public int Int; }
        [RePacker] public struct JustFiller618 { public int Int; }
        [RePacker] public struct JustFiller619 { public int Int; }
        [RePacker] public struct JustFiller620 { public int Int; }
        [RePacker] public struct JustFiller621 { public int Int; }
        [RePacker] public struct JustFiller622 { public int Int; }
        [RePacker] public struct JustFiller623 { public int Int; }
        [RePacker] public struct JustFiller624 { public int Int; }
        [RePacker] public struct JustFiller625 { public int Int; }
        [RePacker] public struct JustFiller626 { public int Int; }
        [RePacker] public struct JustFiller627 { public int Int; }
        [RePacker] public struct JustFiller628 { public int Int; }
        [RePacker] public struct JustFiller629 { public int Int; }
        [RePacker] public struct JustFiller630 { public int Int; }
        [RePacker] public struct JustFiller631 { public int Int; }
        [RePacker] public struct JustFiller632 { public int Int; }
        [RePacker] public struct JustFiller633 { public int Int; }
        [RePacker] public struct JustFiller634 { public int Int; }
        [RePacker] public struct JustFiller635 { public int Int; }
        [RePacker] public struct JustFiller636 { public int Int; }
        [RePacker] public struct JustFiller637 { public int Int; }
        [RePacker] public struct JustFiller638 { public int Int; }
        [RePacker] public struct JustFiller639 { public int Int; }
        [RePacker] public struct JustFiller640 { public int Int; }
        [RePacker] public struct JustFiller641 { public int Int; }
        [RePacker] public struct JustFiller642 { public int Int; }
        [RePacker] public struct JustFiller643 { public int Int; }
        [RePacker] public struct JustFiller644 { public int Int; }
        [RePacker] public struct JustFiller645 { public int Int; }
        [RePacker] public struct JustFiller646 { public int Int; }
        [RePacker] public struct JustFiller647 { public int Int; }
        [RePacker] public struct JustFiller648 { public int Int; }
        [RePacker] public struct JustFiller649 { public int Int; }
        [RePacker] public struct JustFiller650 { public int Int; }
        [RePacker] public struct JustFiller651 { public int Int; }
        [RePacker] public struct JustFiller652 { public int Int; }
        [RePacker] public struct JustFiller653 { public int Int; }
        [RePacker] public struct JustFiller654 { public int Int; }
        [RePacker] public struct JustFiller655 { public int Int; }
        [RePacker] public struct JustFiller656 { public int Int; }
        [RePacker] public struct JustFiller657 { public int Int; }
        [RePacker] public struct JustFiller658 { public int Int; }
        [RePacker] public struct JustFiller659 { public int Int; }
        [RePacker] public struct JustFiller660 { public int Int; }
        [RePacker] public struct JustFiller661 { public int Int; }
        [RePacker] public struct JustFiller662 { public int Int; }
        [RePacker] public struct JustFiller663 { public int Int; }
        [RePacker] public struct JustFiller664 { public int Int; }
        [RePacker] public struct JustFiller665 { public int Int; }
        [RePacker] public struct JustFiller666 { public int Int; }
        [RePacker] public struct JustFiller667 { public int Int; }
        [RePacker] public struct JustFiller668 { public int Int; }
        [RePacker] public struct JustFiller669 { public int Int; }
        [RePacker] public struct JustFiller670 { public int Int; }
        [RePacker] public struct JustFiller671 { public int Int; }
        [RePacker] public struct JustFiller672 { public int Int; }
        [RePacker] public struct JustFiller673 { public int Int; }
        [RePacker] public struct JustFiller674 { public int Int; }
        [RePacker] public struct JustFiller675 { public int Int; }
        [RePacker] public struct JustFiller676 { public int Int; }
        [RePacker] public struct JustFiller677 { public int Int; }
        [RePacker] public struct JustFiller678 { public int Int; }
        [RePacker] public struct JustFiller679 { public int Int; } */
    }
}
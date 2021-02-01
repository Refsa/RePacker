
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker
{
    public static class TypeCache
    {
        public struct Info
        {
            public Type Type;
            public bool HasCustomSerializer;
            public bool IsUnmanaged;
            public FieldInfo[] SerializedFields;
        }

        static Dictionary<Type, Info> typeCache;
        static Dictionary<Type, Delegate> testLookup;
        static Dictionary<Type, TypePacker> packerLookup;

        public static void Init() { }

        static TypeCache()
        {
            Console.WriteLine("Setting up TypeCache");
            var sw = new System.Diagnostics.Stopwatch(); sw.Restart();
            BuildTypeCache();
            BuildSerializers();
            sw.Stop();
            Console.WriteLine($"TypeCache Setup Took {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / packerLookup.Count()}ms per type)");
        }

        static void BuildSerializers()
        {
            var serializerLookup = new Dictionary<Type, MethodInfo>();
            var deserializerLookup = new Dictionary<Type, MethodInfo>();
            var loggerLookup = new Dictionary<Type, MethodInfo>();

            packerLookup = new Dictionary<Type, TypePacker>();
            testLookup = new Dictionary<Type, Delegate>();

            var testMethodCreators = new List<(Type, Func<Delegate>)>();
            var loggerMethodCreators = new List<(Type, Func<MethodInfo>)>();

            var serMethodCreators = new List<(Type, Func<MethodInfo>)>();
            var deserMethodCreators = new List<(Type, Func<MethodInfo>)>();

            SerializerBuilder.Setup();

            foreach ((Type type, Info info) in typeCache)
            {
                if (SerializerBuilder.CreateSerializer(info) is Func<MethodInfo> deserDelegate)
                {
                    serMethodCreators.Add((type, deserDelegate));
                }

                if (SerializerBuilder.CreateDeserializer(info) is Func<MethodInfo> serDelegate)
                {
                    deserMethodCreators.Add((type, serDelegate));
                }

                if (SerializerBuilder.CreateTestMethod(info) is Func<Delegate> testDelegate)
                {
                    testMethodCreators.Add((type, testDelegate));
                }

                if (SerializerBuilder.CreateDataLogger(info) is Func<MethodInfo> loggerDelegate)
                {
                    loggerMethodCreators.Add((type, loggerDelegate));
                }
            }

            SerializerBuilder.Complete();

            foreach (var tmc in testMethodCreators)
            {
                testLookup.Add(tmc.Item1, tmc.Item2.Invoke());
            }

            foreach (var tmc in serMethodCreators)
            {
                serializerLookup.Add(tmc.Item1, tmc.Item2.Invoke());
            }

            foreach (var tmc in deserMethodCreators)
            {
                deserializerLookup.Add(tmc.Item1, tmc.Item2.Invoke());
            }

            foreach (var lmc in loggerMethodCreators)
            {
                loggerLookup.Add(lmc.Item1, lmc.Item2.Invoke());
            }

            foreach ((Type type, Info info) in typeCache)
            {
                var deser = deserializerLookup[type];
                var ser = serializerLookup[type];
                var logger = loggerLookup[type];

                var packer = new TypePacker(info);

                var mi = typeof(TypePacker).GetMethod(nameof(TypePacker.Setup)).MakeGenericMethod(type);
                mi.Invoke(packer, new object[] { ser, deser, logger });

                // var setLogger = typeof(TypePacker).GetMethod(nameof(TypePacker.SetLogger)).MakeGenericMethod(type);
                // mi.Invoke(packer, new object[] { logger });

                packerLookup.Add(
                    type,
                    packer
                );
            }
        }

        static void BuildTypeCache()
        {
            typeCache = new Dictionary<Type, Info>();
            foreach ((string name, Type type) in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(e => e.GetTypes())
                .Where(t => Attribute.GetCustomAttribute(t, typeof(RePackerAttribute)) != null)
                .Select(e => (e.Name, e)))
            {
                if (typeCache.TryGetValue(type, out var info)) continue;

                var tci = new Info
                {
                    Type = type,
                    IsUnmanaged = type.IsUnmanaged(),
                    HasCustomSerializer = type.GetInterface(nameof(ISerializer)) != null,
                };

                if (!tci.HasCustomSerializer)
                {
                    var fields =
                        type
                            .GetFields(BindingFlags.Public | BindingFlags.Instance)
                            // .Where(fi => fi.GetCustomAttribute(typeof(IdentifierAttribute)) != null)
                            // .OrderBy(fi =>
                            // {
                            // var ia = fi.GetCustomAttribute(typeof(IdentifierAttribute)) as IdentifierAttribute;
                            // return ia.Order;
                            // })
                            .ToArray();

                    tci.SerializedFields = fields;
                }

                typeCache.Add(type, tci);
            }
        }

        public static bool TryGetTypeInfo<T>(out Info typeCacheInfo)
        {
            return TryGetTypeInfo(typeof(T), out typeCacheInfo);
        }

        public static bool TryGetTypeInfo(Type type, out Info typeCacheInfo)
        {
            if (typeCache.TryGetValue(type, out typeCacheInfo))
            {
                return true;
            }

            return false;
        }

        public static void Serialize<T>(BoxedBuffer buffer, ref T value)
        {
            if (packerLookup.TryGetValue(typeof(T), out var typePacker))
            {
                typePacker.Pack<T>(buffer, ref value);
            }
        }

        public static T Deserialize<T>(BoxedBuffer buffer)
        {
            if (packerLookup.TryGetValue(typeof(T), out var typePacker))
            {
                return typePacker.Unpack<T>(buffer);
            }

            return default(T);
        }

        public static void LogData<T>(ref T value)
        {
            if (packerLookup.TryGetValue(typeof(T), out var typePacker))
            {
                typePacker.RunLogger<T>(ref value);
            }
        }

        public static void RunTestMethod<T>()
        {
            if (testLookup.TryGetValue(typeof(T), out var testMethod))
            {
                testMethod.DynamicInvoke(null);
            }
        }
    }
}
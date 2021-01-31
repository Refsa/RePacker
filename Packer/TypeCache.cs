
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Builder;

using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker
{
    public static class TypeCache
    {
        public struct Info
        {
            public Type Type;
            public bool HasCustomSerializer;
            public FieldInfo[] SerializedFields;
        }

        static Dictionary<Type, Info> typeCache;
        static Dictionary<Type, MethodInfo> serializerLookup;
        static Dictionary<Type, MethodInfo> deserializerLookup;

        static Dictionary<Type, Delegate> testLookup;

        public static void Init() { }

        static TypeCache()
        {
            Console.WriteLine("Setting up TypeCache");
            BuildTypeCache();
            BuildSerializers();
        }

        static void BuildSerializers()
        {
            serializerLookup = new Dictionary<Type, MethodInfo>();
            deserializerLookup = new Dictionary<Type, MethodInfo>();

            testLookup = new Dictionary<Type, Delegate>();

            var testMethodCreators = new List<(Type, Func<Delegate>)>();

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

        static object[] serializeParameters = new object[2];
        public static void Serialize<T>(BoxedBuffer buffer, ref T value)
        {
            if (serializerLookup.TryGetValue(typeof(T), out var serializer))
            {
                serializeParameters[0] = buffer;
                serializeParameters[1] = value;

                serializer.Invoke(null, serializeParameters);
            }
        }

        static object[] deserializeParameters = new object[1];
        public static T Deserialize<T>(BoxedBuffer buffer)
        {
            if (deserializerLookup.TryGetValue(typeof(T), out var deserializer))
            {
                deserializeParameters[0] = buffer;
                T val = (T)deserializer.Invoke(null, deserializeParameters);
                return val;
            }

            return default(T);
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
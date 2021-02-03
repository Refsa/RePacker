
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
        static Dictionary<Type, TypePacker> packerLookup;

        public static void Init() { }

        static TypeCache()
        {
            RePacker.Settings.Log.Log("Setting up TypeCache");
            var sw = new System.Diagnostics.Stopwatch(); sw.Restart();
            BuildTypeCache();
            BuildSerializers();
            BuildCustomSerializers();
            sw.Stop();
            RePacker.Settings.Log.Log($"TypeCache Setup Took {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / packerLookup.Count()}ms per type)");
        }

        static void VerifySerializers()
        {
            
        }

        static void BuildCustomSerializers()
        {
            foreach ((string name, Type type) in AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(e => e.GetTypes())
                .Where(t =>
                    Attribute.GetCustomAttribute(t, typeof(RePackerWrapperAttribute)) != null
                )
                .Select(e => (e.Name, e)))
            {
                var attr = (RePackerWrapperAttribute)Attribute.GetCustomAttribute(type, typeof(RePackerWrapperAttribute));

                var genWrapper = typeof(RePackerWrapper<>).MakeGenericType(attr.WrapperFor);
                if (!type.IsSubclassOf(genWrapper))
                {
                    RePacker.Settings.Log.Warn($"{type} does not extend from RePackerWrapper<{attr.WrapperFor}>");
                    continue;
                }

                if (typeCache.TryGetValue(type, out var _))
                {
                    RePacker.Settings.Log.Warn($"Packer already exists for type {attr.WrapperFor}");
                    continue;
                }

                var typeInfo = new Info
                {
                    Type = attr.WrapperFor,
                    HasCustomSerializer = true,
                    IsUnmanaged = false,
                    SerializedFields = null,
                };

                typeCache.Add(type, typeInfo);

                var typePacker = new TypePacker(typeInfo);
                var serializer = Activator.CreateInstance(type);
                typePacker.Setup((ITypeSerializer)serializer);

                packerLookup.Add(typeInfo.Type, typePacker);
            }
        }

        static void BuildSerializers()
        {
            packerLookup = new Dictionary<Type, TypePacker>();

            if (!RePacker.Settings.GenerateIL)
            {
                RePacker.Settings.Log.Warn("IL Generation is turned off");
                return;
            }

            var serializerLookup = new Dictionary<Type, MethodInfo>();
            var deserializerLookup = new Dictionary<Type, MethodInfo>();
            var loggerLookup = new Dictionary<Type, MethodInfo>();

            var loggerMethodCreators = new List<(Type, Func<MethodInfo>)>();

            var serMethodCreators = new List<(Type, Func<MethodInfo>)>();
            var deserMethodCreators = new List<(Type, Func<MethodInfo>)>();

            SerializerBuilder.Setup();

            foreach ((Type type, Info info) in typeCache)
            {
                try
                {
                    if (SerializerBuilder.CreateSerializer(info) is Func<MethodInfo> deserDelegate)
                    {
                        serMethodCreators.Add((type, deserDelegate));
                    }

                    if (SerializerBuilder.CreateDeserializer(info) is Func<MethodInfo> serDelegate)
                    {
                        deserMethodCreators.Add((type, serDelegate));
                    }

                    if (SerializerBuilder.CreateDataLogger(info) is Func<MethodInfo> loggerDelegate)
                    {
                        loggerMethodCreators.Add((type, loggerDelegate));
                    }
                }
                catch (Exception e)
                {
                    RePacker.Settings.Log.Error($"Error when generating serializer for {type}");
                    RePacker.Settings.Log.Exception(e);
                }
            }

            SerializerBuilder.Complete();

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

                var mi = typeof(TypePacker).GetMethod(nameof(TypePacker.Setup), new Type[] { typeof(MethodInfo), typeof(MethodInfo), typeof(MethodInfo) }).MakeGenericMethod(type);
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
                .Where(t =>
                    Attribute.GetCustomAttribute(t, typeof(RePackerAttribute)) != null
                )
                .Select(e => (e.Name, e)))
            {
                if (typeCache.TryGetValue(type, out var _))
                {
                    RePacker.Settings.Log.Warn($"Packer already exists for type {type}");
                    continue;
                }

                var tci = new Info
                {
                    Type = type,
                    IsUnmanaged = type.IsUnmanaged(),
                    HasCustomSerializer = type.GetInterface(nameof(ISerializer)) != null,
                };

                // if (!tci.HasCustomSerializer)
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
            else
            {
                RePacker.Settings.Log.Warn($"TypeInfo for {type} not found");
            }

            return false;
        }

        public static bool TryGetTypePacker(Type type, out TypePacker packer)
        {
            if (packerLookup.TryGetValue(type, out packer))
            {
                return true;
            }
            else
            {
                RePacker.Settings.Log.Warn($"Packer for {type} not found");
            }

            return false;
        }

        public static void Serialize<T>(BoxedBuffer buffer, ref T value)
        {
            if (packerLookup.TryGetValue(typeof(T), out var typePacker))
            {
                typePacker.Pack<T>(buffer, ref value);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Packer for {typeof(T)} not found");
            }
        }

        public static T Deserialize<T>(BoxedBuffer buffer)
        {
            if (packerLookup.TryGetValue(typeof(T), out var typePacker))
            {
                return typePacker.Unpack<T>(buffer);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Packer for {typeof(T)} not found");
            }

            return default(T);
        }

        public static void DeserializeInto<T>(BoxedBuffer buffer, ref T target)
        {
            if (packerLookup.TryGetValue(typeof(T), out var typePacker) && typePacker.Info.HasCustomSerializer)
            {
                typePacker.Unpack<T>(buffer, target);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Packer for {typeof(T)} not found");
            }
        }

        public static bool DeserializeOut<T>(BoxedBuffer buffer, out T value)
        {
            if (packerLookup.TryGetValue(typeof(T), out var typePacker))
            {
                value = typePacker.Unpack<T>(buffer);
                return true;
            }
            else
            {
                RePacker.Settings.Log.Warn($"Unpacker for {typeof(T)} not found");
            }

            value = default(T);
            return false;
        }

        public static void LogData<T>(ref T value)
        {
            if (packerLookup.TryGetValue(typeof(T), out var typePacker))
            {
                typePacker.RunLogger<T>(ref value);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Logger for {typeof(T)} not found");
            }
        }
    }
}
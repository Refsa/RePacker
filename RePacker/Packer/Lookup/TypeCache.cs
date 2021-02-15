using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;

namespace Refsa.RePacker.Builder
{
    internal static class TypeCache
    {
        public struct Info
        {
            public Type Type;
            public bool HasCustomSerializer;
            public bool IsUnmanaged;
            public FieldInfo[] SerializedFields;
            public bool IsPrivate;

            public Info(Type type, bool isUnmanaged)
            {
                Type = type;
                IsUnmanaged = isUnmanaged;

                IsPrivate = false;

                HasCustomSerializer = false;
                SerializedFields = new FieldInfo[0];
            }
        }

        static IEnumerable<Type> allTypes;

        static Dictionary<Type, Info> typeCache;
        static Dictionary<Type, TypePackerHandler> packerLookup;
        static Dictionary<Type, GenericProducer> runtimePackerProducers;

        // static TypeResolver typeResolver;
        // static TypePackerHandler[] packerLookupFast;

        static bool isSetup = false;
        public static bool IsSetup => isSetup;

        internal static void Reload()
        {
            isSetup = false;
            typeCache = null;
            packerLookup = null;
            Setup();
        }

        internal static void Setup()
        {
            if (isSetup) return;

            RePacker.Settings.Log.Log("Setting up TypeCache");
            var sw = new System.Diagnostics.Stopwatch(); sw.Restart();

            packerLookup = new Dictionary<Type, TypePackerHandler>();
            allTypes = ReflectionUtils.GetAllTypes();

            BuildTypeCache();

            // TypeCode.Object
            BuildCustomPackers();
            BuildPackers();

            BuildRuntimePackerProviders();

            VerifyPackers();

            // packerLookupFast = new TypePackerHandler[packerLookup.Count];
            // int index = 0;
            // foreach ((Type type, TypePackerHandler handler) in packerLookup)
            // {
            //     packerLookupFast[index++] = handler;
            // }

            // TypeResolverBuilder.Begin();
            // typeResolver = TypeResolverBuilder.BuildHandler(packerLookup);

            // int idx = typeResolver.Resolver.Invoke(typeof(int));
            // Console.WriteLine(packerLookupFast[idx].Info.Type);

            sw.Stop();
            RePacker.Settings.Log.Log($"TypeCache Setup Took {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / packerLookup.Count()}ms per type)");

            isSetup = true;

            allTypes = null;
        }

        internal static void AddTypePackerProvider(Type targetType, GenericProducer producer)
        {
            runtimePackerProducers.Add(targetType, producer);
        }

        static void VerifyPackers()
        {
            List<Type> invalid = new List<Type>();

            // foreach ((Type type, Info info) in typeCache)
            foreach (var kv in typeCache)
            {
                bool valid = packerLookup.TryGetValue(kv.Key, out var packer) && packer != null;

                if (!valid)
                {
                    RePacker.Logger.Warn($"type of {kv.Key} does not have a valid serializer");
                    invalid.Add(kv.Key);
                }
            }

            foreach (Type type in invalid)
            {
                typeCache.Remove(type);
                packerLookup.Remove(type);
            }
        }

        static void BuildTypeCache()
        {
            typeCache = new Dictionary<Type, Info>();
            foreach (Type type in allTypes
                .WithAttribute(typeof(RePackerAttribute)))
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
                    IsPrivate = type.IsNotPublic,
                    HasCustomSerializer = type.GetInterface(typeof(IPacker<>).MakeGenericType(type).Name) != null,
                };

                List<FieldInfo> serializedFields = new List<FieldInfo>();

                // fields
                {
                    var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                    var rpattr = (RePackerAttribute)Attribute.GetCustomAttribute(type, typeof(RePackerAttribute));
                    if (!rpattr.UseOnAllPublicFields)
                    {
                        fields = fields.Where(fi => fi.GetCustomAttribute<RePackAttribute>() != null).ToArray();
                    }

                    serializedFields.AddRange(fields);
                }

                // Properties
                {
                    var props =
                        type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(pi => pi.GetCustomAttribute<RePackAttribute>() != null)
                        .Select(pi => ReflectionUtils.GetPropertyBackingFieldInfo(type, pi.Name))
                        .Where(fi => fi != null);

                    serializedFields.AddRange(props);
                }

                tci.SerializedFields = serializedFields.ToArray();

                typeCache.Add(type, tci);
            }
        }

        static void BuildCustomPackers()
        {
            Dictionary<Type, Info> customTypeInfo = new Dictionary<Type, Info>();

            foreach (Type type in allTypes
                .NotGeneric()
                .WithGenericBaseType(typeof(RePackerWrapper<>)))
            {
                Type wrapperFor = type.BaseType.GetGenericArguments()[0];
                Console.WriteLine(wrapperFor);

                if (wrapperFor == null)
                {
                    RePacker.Settings.Log.Warn($"Could not get {type} from RePackerWrapper<>");
                    continue;
                }

                if (typeCache.TryGetValue(type, out var _))
                {
                    RePacker.Settings.Log.Warn($"Packer already exists for type {wrapperFor}");
                    continue;
                }

                var typeInfo = new Info
                {
                    Type = wrapperFor,
                    HasCustomSerializer = true,
                    IsUnmanaged = false,
                    SerializedFields = null,
                };

                typeCache.Add(wrapperFor, typeInfo);
                customTypeInfo.Add(type, typeInfo);
            }

            foreach (var kv in customTypeInfo)
            {
                var typePacker = new TypePackerHandler(kv.Value);
                var serializer = Activator.CreateInstance(kv.Key);
                typePacker.Setup((ITypePacker)serializer);

                packerLookup.Add(kv.Value.Type, typePacker);
            }
        }

        private static void BuildRuntimePackerProviders()
        {
            runtimePackerProducers = new Dictionary<Type, GenericProducer>();

            foreach (Type type in allTypes
                .Where(t => t.IsSubclassOf(typeof(GenericProducer))))
            {
                var producer = (GenericProducer)Activator.CreateInstance(type);
                var forType = producer.ProducerFor;

                if (runtimePackerProducers.TryGetValue(forType, out var currProducer))
                {
                    RePacker.Logger.Warn($"Generic producer for {forType} already exists under {currProducer.GetType().Name}");
                    continue;
                }
                runtimePackerProducers.Add(
                    forType,
                    producer
                );
            }
        }

        static void BuildPackers()
        {
            if (!RePacker.Settings.GenerateIL)
            {
                RePacker.Settings.Log.Warn("IL Generation is turned off");
                return;
            }

            var serializerLookup = new Dictionary<Type, MethodInfo>();
            var serMethodCreators = new List<(Type, Func<MethodInfo>)>();

            var deserializerLookup = new Dictionary<Type, MethodInfo>();
            var deserMethodCreators = new List<(Type, Func<MethodInfo>)>();

            // var loggerLookup = new Dictionary<Type, MethodInfo>();
            // var loggerMethodCreators = new List<(Type, Func<MethodInfo>)>();

            PackerBuilder.Setup();

            // foreach ((Type type, Info info) in typeCache)
            foreach (var kv in typeCache)
            {
                (Type type, Info info) = (kv.Key, kv.Value);

                if (packerLookup.ContainsKey(type))
                {
                    continue;
                }

                try
                {
                    if (PackerBuilder.CreatePacker(info) is Func<MethodInfo> serDelegate)
                    {
                        serMethodCreators.Add((type, serDelegate));
                    }
                }
                catch (Exception e)
                {
                    RePacker.Settings.Log.Error($"Error when generating packer for {type}");
                    RePacker.Settings.Log.Exception(e);
                }

                try
                {
                    if (PackerBuilder.CreateUnpacker(info) is Func<MethodInfo> deserDelegate)
                    {
                        deserMethodCreators.Add((type, deserDelegate));
                    }
                }
                catch (Exception e)
                {
                    RePacker.Settings.Log.Error($"Error when generating unpacker for {type}");
                    RePacker.Settings.Log.Exception(e);
                }

                // if (PackerBuilder.CreateDataLogger(info) is Func<MethodInfo> loggerDelegate)
                // {
                //     loggerMethodCreators.Add((type, loggerDelegate));
                // }
            }

            PackerBuilder.Complete();

            foreach (var tmc in serMethodCreators)
            {
                serializerLookup.Add(tmc.Item1, tmc.Item2.Invoke());
            }

            foreach (var tmc in deserMethodCreators)
            {
                deserializerLookup.Add(tmc.Item1, tmc.Item2.Invoke());
            }

            // foreach (var lmc in loggerMethodCreators)
            // {
            //     loggerLookup.Add(lmc.Item1, lmc.Item2.Invoke());
            // }

            foreach (var kv in typeCache)
            {
                (Type type, Info info) = (kv.Key, kv.Value);

                if (packerLookup.ContainsKey(type))
                {
                    continue;
                }

                try
                {
                    var deser = deserializerLookup[type];
                    var ser = serializerLookup[type];
                    // var logger = loggerLookup[type];

                    var packer = new TypePackerHandler(info);

                    var mi = typeof(TypePackerHandler).GetMethod(nameof(TypePackerHandler.Setup), new Type[] { typeof(MethodInfo), typeof(MethodInfo) }).MakeGenericMethod(type);
                    mi.Invoke(packer, new object[] { ser, deser });

                    // var setLogger = typeof(TypePacker).GetMethod(nameof(TypePacker.SetLogger)).MakeGenericMethod(type);
                    // mi.Invoke(packer, new object[] { logger });

                    packerLookup.Add(
                        type,
                        packer
                    );
                }
                catch (Exception e)
                {
                    // RePacker.Settings.Log.Error($"Error when setting up TypePackerHandler for {type}");
                    RePacker.Settings.Log.Error(e.Message + "\n" + e.StackTrace);
                }
            }
        }

        static void AddTypeHandler(Type type, ITypePacker packer)
        {
            var info = new Info
            {
                Type = type,
            };

            var handler = new TypePackerHandler(info);
            handler.Setup(packer);

            typeCache.Add(type, info);
            packerLookup.Add(type, handler);
        }

        static bool AttemptToCreatePacker(Type type)
        {
            Type targetType = null;

            foreach (var iface in type.GetInterfaces())
            {
                if (runtimePackerProducers.TryGetValue(iface, out var producer))
                {
                    AddTypeHandler(type, producer.GetProducer(type));
                    return true;
                }
            }

            if (type.IsArray)
            {
                targetType = typeof(Array);
            }
            else if (type.IsGenericType)
            {
                targetType = type.GetGenericTypeDefinition();
            }

            if (targetType != null)
            {
                if (runtimePackerProducers.TryGetValue(targetType, out var producer))
                {
                    AddTypeHandler(type, producer.GetProducer(type));
                    return true;
                }
            }

            return false;
        }

        #region API
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
                // RePacker.Settings.Log.Warn($"TypeInfo for {type} not found: \n{System.Environment.StackTrace}");
            }

            return false;
        }

        public static bool TryGetTypePacker<T>(out TypePackerHandler packer)
        {
            return TryGetTypePacker(typeof(T), out packer);
        }

        public static bool TryGetTypePacker(Type type, out TypePackerHandler packer)
        {
            if (packerLookup.TryGetValue(type, out packer))
            {
                return true;
            }
            else
            {
                RePacker.Settings.Log.Warn($"Packer for {type} not found");
            }

            packer = null;
            return false;
        }

        public static void Pack<T>(BoxedBuffer buffer, ref T value)
        {
            if (packerLookup.TryGetValue(typeof(T), out var packer))
            {
                packer.Pack<T>(buffer, ref value);
            }
            else if (AttemptToCreatePacker(typeof(T)))
            {
                Pack<T>(buffer, ref value);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Packer for {typeof(T)} not found");
            }
        }

        static T UnpackInternal<T>(BoxedBuffer buffer)
        {
            if (packerLookup.TryGetValue(typeof(T), out var packer))
            {
                return packer.Unpack<T>(buffer);
            }
            else if (AttemptToCreatePacker(typeof(T)))
            {
                return UnpackInternal<T>(buffer);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Unpacker for {typeof(T)} not found");
            }

            return default(T);
        }

        public static T Unpack<T>(BoxedBuffer buffer)
        {
            return UnpackInternal<T>(buffer);
        }

        public static bool UnpackOut<T>(BoxedBuffer buffer, out T value)
        {
            value = UnpackInternal<T>(buffer);
            return value != null;
        }

        public static void UnpackInto<T>(BoxedBuffer buffer, ref T target)
        {
            if (packerLookup.TryGetValue(typeof(T), out var packer))
            {
                packer.UnpackInto<T>(buffer, ref target);
            }
            else if (AttemptToCreatePacker(typeof(T)))
            {
                UnpackInto<T>(buffer, ref target);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Unpacker for {typeof(T)} not found");
            }
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
        #endregion
    }
}
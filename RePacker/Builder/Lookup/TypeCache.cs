using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
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

            public Info(Type type, bool isUnmanaged)
            {
                Type = type;
                IsUnmanaged = isUnmanaged;

                HasCustomSerializer = false;
                SerializedFields = new FieldInfo[0];
            }
        }

        static IEnumerable<Type> allTypes;

        static Dictionary<Type, Info> typeCache;
        static Dictionary<Type, TypePackerHandler> packerLookup;
        static ConcurrentDictionary<Type, GenericProducer> runtimePackerProducers;

        static bool isSetup = false;
        public static bool IsSetup => isSetup;

        internal static void Reload()
        {
            isSetup = false;
            typeCache = null;
            packerLookup = null;
            runtimePackerProducers = null;
            Setup();
        }

        internal static void Setup()
        {
            if (isSetup) return;

            packerLookup = new Dictionary<Type, TypePackerHandler>();
            allTypes = ReflectionUtils.GetAllTypes();

            BuildTypeCache();

            BuildCustomPackers();
            BuildPackers();

            BuildRuntimePackerProviders();

            VerifyPackers();

            // Build efficient lookup
            {
                BuildLookup();
            }

            isSetup = true;
            // allTypes = null;
        }

        static void VerifyPackers()
        {
            List<Type> invalid = new List<Type>();

            foreach (var kv in typeCache)
            {
                bool valid = packerLookup.TryGetValue(kv.Key, out var packer) && packer != null;
                if (!valid) invalid.Add(kv.Key);
            }

            foreach (Type type in invalid)
            {
                RePacker.Logger.Warn($"type of {type} does not have a valid packer");

                typeCache.Remove(type);
                packerLookup.Remove(type);
            }
        }

        static void BuildLookup()
        {
            var packerGetter = typeof(TypePackerHandler).GetMethod(nameof(TypePackerHandler.GetTypePacker));
            foreach (var handler in packerLookup)
            {
                SetupTypeResolver(handler.Key, handler.Value.Packer);
            }
        }

        static void SetupTypeResolver(Type type, ITypePacker packer)
        {
            var packerType = typeof(IPacker<>).MakeGenericType(type);

            var resolver = typeof(TypeResolver<,>)
                .MakeGenericType(packerType, type);
            var setter = resolver.GetProperty("Packer");

            setter.SetMethod.Invoke(null, new object[] { packer });
        }

        static void BuildTypeCache()
        {
            typeCache = new Dictionary<Type, Info>();
            foreach (Type type in allTypes
                .WithAttribute(typeof(RePackerAttribute)))
            {
                if (typeCache.ContainsKey(type))
                {
                    RePacker.Settings.Log.Warn($"Packer already exists for type {type}");
                    continue;
                }

                var attr = type.GetCustomAttribute<RePackerAttribute>();

                var tci = new Info
                {
                    Type = type,
                    IsUnmanaged = type.IsUnmanaged(),
                    // HasCustomSerializer = type.GetInterface(typeof(IPacker<>).MakeGenericType(type).Name) != null,
                    // HasCustomSerializer =
                    // type.IsSubclassOf(typeof(RePackerWrapper<>).MakeGenericType(type))
                    // || !attr.UseOnAllPublicFields,
                    HasCustomSerializer = !type.IsValueType,
                };

                List<FieldInfo> serializedFields = new List<FieldInfo>();

                // fields
                {
                    IEnumerable<FieldInfo> fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

                    var rpattr = (RePackerAttribute)Attribute.GetCustomAttribute(type, typeof(RePackerAttribute));
                    if (!rpattr.UseOnAllPublicFields)
                    {
                        fields = fields.Where(fi => fi.GetCustomAttribute<RePackAttribute>() != null);
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

                if (tci.IsUnmanaged || type.IsUnmanagedStruct())
                {
                    if (ReflectionUtils.GetAllFields(type).Count() == serializedFields.Count)
                    {
                        tci.HasCustomSerializer = false;
                    }
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
                if (typeCache.ContainsKey(wrapperFor) || customTypeInfo.ContainsKey(type))
                {
                    continue;
                }

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
                    HasCustomSerializer = !wrapperFor.IsValueType,
                    IsUnmanaged = wrapperFor.IsUnmanaged(),
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

        internal static void AddTypePackerProvider(Type targetType, GenericProducer producer)
        {
            runtimePackerProducers.TryAdd(targetType, producer);
        }

        static void BuildRuntimePackerProviders()
        {
            runtimePackerProducers = new ConcurrentDictionary<Type, GenericProducer>();

            foreach (Type type in allTypes
                .Where(t => t.IsSubclassOf(typeof(GenericProducer))))
            {
                var producer = (GenericProducer)Activator.CreateInstance(type);

                if (producer.ProducerForAll != null)
                {
                    foreach (var p in producer.ProducerForAll)
                    {
                        AddProducerFor(p, producer);
                    }
                }
                else
                {
                    var forType = producer.ProducerFor;

                    AddProducerFor(forType, producer);
                }
            }
        }

        static void AddProducerFor(Type type, GenericProducer producer)
        {
            var forType = type;
            if (runtimePackerProducers.TryGetValue(forType, out var currProducer))
            {
                RePacker.Logger.Warn($"Generic producer for {forType} already exists under {currProducer.GetType().Name}");
                return;
            }

            runtimePackerProducers.TryAdd(
                forType,
                producer
            );
        }

        static void BuildPackers()
        {
            if (!RePacker.Settings.GenerateIL)
            {
                RePacker.Settings.Log.Warn("IL Generation is turned off");
                return;
            }

            Dictionary<Type, (MethodInfo packer, MethodInfo unpacker)> packers = new Dictionary<Type, (MethodInfo packer, MethodInfo unpacker)>();

            foreach (var kv in typeCache)
            {
                if (packerLookup.ContainsKey(kv.Key)) continue;
                (Type type, Info info) = (kv.Key, kv.Value);

                if (info.SerializedFields == null)
                {
                    RePacker.Logger.Error($"No serialized fields found on {type}");
                    continue;
                }

                MethodInfo packer = null;
                MethodInfo unpacker = null;

                // Generate Packer Method
                try
                {
                    packer = PackerBuilder.CreatePacker(info);
                }
                catch (Exception e)
                {
                    RePacker.Settings.Log.Error($"Error when generating packer for {type}");
                    RePacker.Settings.Log.Exception(e);
                    continue;
                }

                // Generate Unpacker Method
                try
                {
                    unpacker = PackerBuilder.CreateUnpacker(info);
                }
                catch (Exception e)
                {
                    RePacker.Settings.Log.Error($"Error when generating unpacker for {type}");
                    RePacker.Settings.Log.Exception(e);
                    continue;
                }

                packers.Add(type, (packer, unpacker));
            }

            foreach (var kv in packers)
            {
                if (packerLookup.ContainsKey(kv.Key)) continue;

                try
                {
                    var packerHandler = new TypePackerHandler(typeCache[kv.Key]);

                    var mi = typeof(TypePackerHandler)
                        .GetMethod(
                            nameof(TypePackerHandler.Setup),
                            new Type[] { typeof(MethodInfo), typeof(MethodInfo) }
                        ).MakeGenericMethod(kv.Key);

                    mi.Invoke(packerHandler, new object[] { kv.Value.packer, kv.Value.unpacker });

                    packerLookup.Add(
                        kv.Key,
                        packerHandler
                    );
                }
                catch (Exception e)
                {
                    RePacker.Logger.Exception(e);
                }
            }
        }

        static void AddTypeHandler(Type type)
        {
            var info = new Info
            {
                Type = type,
            };
            typeCache.Add(type, info);
        }

        internal static bool AttemptToCreatePacker(Type type)
        {
            Type targetType = null;

            foreach (var iface in type.GetInterfaces())
            {
                if (runtimePackerProducers.TryGetValue(iface, out var producer))
                {
                    AddTypeHandler(type);
                    SetupTypeResolver(type, producer.GetProducer(type));
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
                    AddTypeHandler(type);
                    SetupTypeResolver(type, producer.GetProducer(type));
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
            if (TypeResolver<IPacker<T>, T>.Packer is IPacker<T> packer)
            {
                packer.Pack(buffer, ref value);
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
            if (TypeResolver<IPacker<T>, T>.Packer is IPacker<T> packer)
            {
                packer.Unpack(buffer, out T value);
                return value;
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
            if (TypeResolver<IPacker<T>, T>.Packer is IPacker<T> packer)
            {
                packer.UnpackInto(buffer, ref target);
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
        #endregion
    }
}
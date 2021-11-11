using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using RePacker.Buffers;
using RePacker.Generator;
using RePacker.Utils;

using ReBuffer = RePacker.Buffers.ReBuffer;

namespace RePacker.Builder
{
    internal static class TypeCache
    {
        internal struct Info
        {
            public Type Type;
            public bool HasCustomSerializer;
            public bool IsUnmanaged;
            public bool IsDirectlyCopyable;
            public FieldInfo[] SerializedFields;

            public Info(Type type, bool isUnmanaged)
            {
                Type = type;
                IsUnmanaged = isUnmanaged;

                IsDirectlyCopyable = false;
                HasCustomSerializer = false;
                SerializedFields = new FieldInfo[0];
            }
        }

        static IEnumerable<Type> allTypes;

        static ConcurrentDictionary<Type, Info> typeCache;
        static Dictionary<Type, Type> wrapperTypeLookup;

        static ConcurrentDictionary<Type, ITypePacker> packerLookup;
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

            packerLookup = new ConcurrentDictionary<Type, ITypePacker>();
            wrapperTypeLookup = new Dictionary<Type, Type>();
            allTypes = ReflectionUtils.GetAllTypes();

            BuildTypeCache();
            BuildRuntimePackerProviders();

            BuildWrapperPackers();
            BuildPackers();

            VerifyPackers();

            BuildLookup();

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
                // RePacking.Logger.Warn($"type of {type} does not have a valid packer");

                typeCache.TryRemove(type, out var _);
                packerLookup.TryRemove(type, out var _);
            }
        }

        static void BuildLookup()
        {
            foreach (var handler in packerLookup)
            {
                SetupTypeResolver(handler.Key, handler.Value);
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
            typeCache = new ConcurrentDictionary<Type, Info>();
            List<FieldInfo> serializedFields = new List<FieldInfo>();

            foreach (Type type in allTypes
                .WithAttribute(typeof(RePackerAttribute)))
            {
                if (typeCache.ContainsKey(type))
                {
                    // RePacking.Settings.Log.Warn($"Packer already exists for type {type}");
                    continue;
                }

                var attr = type.GetCustomAttribute<RePackerAttribute>();

                var tci = new Info
                {
                    Type = type,
                    IsUnmanaged = type.IsUnmanaged(),
                    HasCustomSerializer = false,
                    IsDirectlyCopyable = false,
                };

                serializedFields.Clear();

                // fields
                {
                    IEnumerable<FieldInfo> fields = type
                        .GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Concat(
                            type
                                .GetFields(BindingFlags.NonPublic | BindingFlags.Instance)
                                .Where(fi => fi.GetCustomAttribute<RePackAttribute>() != null)
                        );

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
                        tci.IsDirectlyCopyable = true;
                    }
                }

                tci.SerializedFields = serializedFields.ToArray();

                typeCache.TryAdd(type, tci);
            }

            foreach (Type type in allTypes
                .NotGeneric()
                .WithGenericBaseType(typeof(RePackerWrapper<>)))
            {
                Type wrapperFor = type.BaseType.GetGenericArguments()[0];
                if (typeCache.ContainsKey(wrapperFor))
                {
                    continue;
                }

                if (wrapperFor == null)
                {
                    RePacking.Settings.Log.Warn($"Could not get {type} from RePackerWrapper<>");
                    continue;
                }

                if (typeCache.TryGetValue(type, out var _))
                {
                    RePacking.Settings.Log.Warn($"Packer already exists for type {wrapperFor}");
                    continue;
                }

                var isCopyableField = type.GetField("IsCopyable", BindingFlags.Static | BindingFlags.Public);
                bool isDirectlyCopyable = false;
                if ((wrapperFor.IsUnmanaged() || wrapperFor.IsUnmanagedStruct()) &&
                    isCopyableField is FieldInfo fieldInfo)
                {
                    isDirectlyCopyable = (bool)fieldInfo.GetValue(null);
                }

                var typeInfo = new Info
                {
                    Type = wrapperFor,
                    HasCustomSerializer = true,
                    IsDirectlyCopyable = isDirectlyCopyable,
                    IsUnmanaged = wrapperFor.IsUnmanaged(),
                    SerializedFields = null,
                };

                typeCache.TryAdd(wrapperFor, typeInfo);
                wrapperTypeLookup.Add(wrapperFor, type);
            }
        }

        static void BuildWrapperPackers()
        {
            foreach (var kv in typeCache)
            {
                if (!kv.Value.HasCustomSerializer) continue;

                if (wrapperTypeLookup.TryGetValue(kv.Key, out Type wrapper))
                {
                    var serializer = Activator.CreateInstance(wrapper);
                    packerLookup.TryAdd(kv.Value.Type, (ITypePacker)serializer);
                }
                else
                {
                    RePacking.Logger.Warn($"Could not create wrapper for type {kv.Key}, Type instance was null");
                }
            }
        }

        static void BuildPackers()
        {
            if (!RePacking.Settings.GenerateIL)
            {
                RePacking.Settings.Log.Warn("IL Generation is turned off");
                return;
            }

            // FIXME: Not supported on NET4.6.1
            Dictionary<Type, (MethodInfo packer, MethodInfo unpacker, MethodInfo getSizer)> packers =
                new Dictionary<Type, (MethodInfo packer, MethodInfo unpacker, MethodInfo getSizer)>();

            foreach (var kv in typeCache)
            {
                if (kv.Value.HasCustomSerializer) continue;

                if (packerLookup.ContainsKey(kv.Key)) continue;
                (Type type, Info info) = (kv.Key, kv.Value);

                if (info.SerializedFields == null)
                {
                    RePacking.Logger.Error($"No serialized fields found on {type}");
                    continue;
                }

                MethodInfo packer = null;
                MethodInfo unpacker = null;
                MethodInfo getSizer = null;

                // Generate Packer Method
                try
                {
                    packer = PackerBuilder.CreatePacker(info);
                }
                catch (Exception e)
                {
                    RePacking.Settings.Log.Error($"Error when generating packer for {type}");
                    RePacking.Settings.Log.Exception(e);
                    continue;
                }

                // Generate Unpacker Method
                try
                {
                    unpacker = PackerBuilder.CreateUnpacker(info);
                }
                catch (Exception e)
                {
                    RePacking.Settings.Log.Error($"Error when generating unpacker for {type}");
                    RePacking.Settings.Log.Exception(e);
                    continue;
                }

                try
                {
                    getSizer = (MethodInfo)typeof(GetSizeGenerator<>)
                        .MakeGenericType(info.Type)
                        .GetMethod("Create")
                        .Invoke(null, new object[] { info });
                }
                catch (Exception e)
                {
                    RePacking.Settings.Log.Error($"Error when generating getSizer for {type}");
                    RePacking.Settings.Log.Exception(e);
                }

                // FIXME: Not supported in NET4.6
                packers.Add(type, (packer, unpacker, getSizer));
            }

            foreach (var kv in packers)
            {
                if (packerLookup.ContainsKey(kv.Key)) continue;

                try
                {
                    var typePacker = (ITypePacker)Activator
                        .CreateInstance(
                            typeof(TypePacker<>).MakeGenericType(kv.Key),
                            kv.Value.packer, kv.Value.unpacker, kv.Value.getSizer);

                    packerLookup.TryAdd(
                        kv.Key,
                        typePacker
                    );
                }
                catch (Exception e)
                {
                    RePacking.Logger.Exception(e);
                }
            }
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
                RePacking.Logger.Warn($"Generic producer for {forType} already exists under {currProducer.GetType().Name}");
                return;
            }

            runtimePackerProducers.TryAdd(
                forType,
                producer
            );
        }

        #region Runtime
        static void AddRuntimeTypeInfo(Type type)
        {
            var info = new Info
            {
                Type = type,
            };
            typeCache.TryAdd(type, info);
        }

        internal static bool AttemptToCreatePacker(Type type)
        {
            if (type.IsInterface) return false;

            Type targetType = null;
            foreach (var iface in type.GetInterfaces())
            {
                if (runtimePackerProducers.TryGetValue(iface, out var producer))
                {
                    var prod = producer.GetProducer(type);
                    if (prod != null)
                    {
                        AddRuntimeTypeInfo(type);
                        SetupTypeResolver(type, prod);
                        return true;
                    }
                    return false;
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
                    var prod = producer.GetProducer(type);
                    if (prod != null)
                    {
                        AddRuntimeTypeInfo(type);
                        SetupTypeResolver(type, prod);
                        return true;
                    }
                }
            }
            else if (type.IsUnmanaged() || type.IsUnmanagedStruct())
            {
                try
                {
                    var packer = (ITypePacker)Activator.CreateInstance(typeof(UnmanagedPacker<>).MakeGenericType(type));
                    AddRuntimeTypeInfo(type);
                    SetupTypeResolver(type, packer);
                    return true;
                }
                catch { }
            }

            return false;
        }
        #endregion

        #region API
        internal static void AddTypePackerProvider(Type targetType, GenericProducer producer)
        {
            runtimePackerProducers.TryAdd(targetType, producer);
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
            else if (AttemptToCreatePacker(type))
            {
                return TryGetTypeInfo(type, out typeCacheInfo);
            }
            else
            {
                // RePacker.Settings.Log.Warn($"TypeInfo for {type} not found: \n{System.Environment.StackTrace}");
            }

            return false;
        }

        public static bool TryGetTypePacker<T>(out IPacker<T> packer)
        {
            if (TypeResolver<IPacker<T>, T>.Packer is IPacker<T> p)
            {
                packer = p;
                return true;
            }
            else if (AttemptToCreatePacker(typeof(T)))
            {
                return TryGetTypePacker<T>(out packer);
            }

            packer = null;
            return false;
        }

        public static void Pack<T>(ReBuffer buffer, ref T value)
        {
            if (TypeResolver<IPacker<T>, T>.Packer is IPacker<T> packer)
            {
                int writeCursor = buffer.WriteCursor();
                try
                {
                    packer.Pack(buffer, ref value);
                }
                catch (Exception e)
                {
                    buffer.SetWriteCursor(writeCursor);
                    throw e;
                }
            }
            else if (AttemptToCreatePacker(typeof(T)))
            {
                Pack<T>(buffer, ref value);
            }
            else if (typeof(T).IsInterface)
            {
                if (AttemptToCreatePacker(value.GetType()))
                {
                    var m = typeof(TypeCache).GetMethod(nameof(Pack), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(value.GetType());
                    m.Invoke(null, new object[] { buffer, value });
                }
            }
            else
            {
                throw new NotSupportedException($"Packer for {typeof(T)} not found");
            }
        }

        static T UnpackInternal<T>(ReBuffer buffer)
        {
            if (TypeResolver<IPacker<T>, T>.Packer is IPacker<T> packer)
            {
                int readCursor = buffer.ReadCursor();
                try
                {
                    packer.Unpack(buffer, out T value);
                    return value;
                }
                catch (Exception e)
                {
                    buffer.SetReadCursor(readCursor);
                    throw e;
                }
            }
            else if (AttemptToCreatePacker(typeof(T)))
            {
                return UnpackInternal<T>(buffer);
            }
            else if (typeof(T).IsInterface)
            {
                throw new NotSupportedException($"Can't unpack into an interface");
            }
            else
            {
                throw new NotSupportedException($"Unpacker for {typeof(T)} not found");
            }

            return default(T);
        }

        public static T Unpack<T>(ReBuffer buffer)
        {
            return UnpackInternal<T>(buffer);
        }

        public static bool UnpackOut<T>(ReBuffer buffer, out T value)
        {
            value = UnpackInternal<T>(buffer);
            return value != null;
        }

        public static void UnpackInto<T>(ReBuffer buffer, ref T target)
        {
            if (TypeResolver<IPacker<T>, T>.Packer is IPacker<T> packer)
            {
                int readCursor = buffer.ReadCursor();
                try
                {
                    packer.UnpackInto(buffer, ref target);
                }
                catch (Exception e)
                {
                    buffer.SetReadCursor(readCursor);
                    throw e;
                }
            }
            else if (AttemptToCreatePacker(typeof(T)))
            {
                UnpackInto<T>(buffer, ref target);
            }
            else if (target.GetType().IsInterface)
            {
                throw new NotSupportedException($"Can't unpack into an interface");
            }
            else
            {
                throw new NotSupportedException($"Unpacker for {typeof(T)} not found");
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int GetSize<T>(ref T value)
        {
            if (TypeResolver<IPacker<T>, T>.Packer is IPacker<T> packer)
            {
                return packer.SizeOf(ref value);
            }
            else if (AttemptToCreatePacker(typeof(T)))
            {
                return GetSize(ref value);
            }
            else if (typeof(T).IsInterface)
            {
                if (AttemptToCreatePacker(value.GetType()))
                {
                    var m = typeof(TypeCache).GetMethod(nameof(GetSize), BindingFlags.Static | BindingFlags.Public).MakeGenericMethod(value.GetType());
                    return (int)m.Invoke(null, new object[] { value });
                }
                else
                {
                    throw new NotSupportedException($"Can't get size of an anonymous interface");
                }
            }
            else
            {
                throw new NotSupportedException($"GetSizer for {typeof(T)} not found");
            }
        }
        #endregion
    }
}
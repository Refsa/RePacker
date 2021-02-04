
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

            public Info(Type type, bool isUnmanaged)
            {
                Type = type;
                IsUnmanaged = isUnmanaged;

                HasCustomSerializer = false;
                SerializedFields = new FieldInfo[0];
            }
        }

        struct PackerEntry
        {
            public Dictionary<Type, TypePackerHandler> Lookup;
            public TypePackerHandler Single;

            int type;

            public PackerEntry(TypePackerHandler single)
            {
                Single = single;
                Lookup = null;
                type = 1;
            }

            public PackerEntry(Dictionary<Type, TypePackerHandler> lookup)
            {
                Single = null;
                Lookup = lookup;
                type = 2;
            }

            public bool Get(Type type, out TypePackerHandler handler)
            {
                if (this.type == 1)
                {
                    handler = Single;
                    return true;
                }
                else if (this.type == 2)
                {
                    if (Lookup.TryGetValue(type, out var packer))
                    {
                        handler = packer;
                        return true;
                    }
                }

                handler = null;
                return false;
            }
        }

        static Dictionary<Type, Info> typeCache;
        static Dictionary<Type, TypePackerHandler> packerLookup;

        static TypeResolver typeResolver;
        static TypePackerHandler[] packerLookupFast;

        static bool isSetup = false;
        public static bool IsSetup => isSetup;

        public static void Reload()
        {
            typeCache = null;
            packerLookup = null;
            Setup();
        }

        public static void Setup()
        {
            if (isSetup) return;

            RePacker.Settings.Log.Log("Setting up TypeCache");
            var sw = new System.Diagnostics.Stopwatch(); sw.Restart();

            packerLookup = new Dictionary<Type, TypePackerHandler>();

            BuildTypeCache();

            BuildPrimitivePackers();

            // TypeCode.Object
            BuildPackers();
            BuildCustomPackers();

            VerifyPackers();

            packerLookupFast = new TypePackerHandler[packerLookup.Count];
            int index = 0;
            foreach ((Type type, TypePackerHandler handler) in packerLookup)
            {
                packerLookupFast[index++] = handler;
            }

            TypeResolverBuilder.Begin();
            typeResolver = TypeResolverBuilder.BuildHandler(packerLookup);

            // int idx = typeResolver.Resolver.Invoke(typeof(int));
            // Console.WriteLine(packerLookupFast[idx].Info.Type);

            sw.Stop();
            RePacker.Settings.Log.Log($"TypeCache Setup Took {sw.ElapsedMilliseconds}ms ({sw.ElapsedMilliseconds / packerLookup.Count()}ms per type)");

            isSetup = true;
        }

        static void VerifyPackers()
        {
            List<Type> invalid = new List<Type>();

            foreach ((Type type, Info info) in typeCache)
            {
                bool valid = packerLookup.TryGetValue(type, out var packer) && packer != null;

                if (!valid)
                {
                    RePacker.Logger.Warn($"type of {type} does not have a valid serializer");
                    invalid.Add(type);
                }
            }

            foreach (Type type in invalid)
            {
                typeCache.Remove(type);
                packerLookup.Remove(type);
            }
        }

        static void BuildCustomPackers()
        {
            Dictionary<Type, Info> customTypeInfo = new Dictionary<Type, Info>();

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

                typeCache.Add(attr.WrapperFor, typeInfo);
                customTypeInfo.Add(type, typeInfo);
            }

            foreach ((Type type, Info info) in customTypeInfo)
            {
                var typePacker = new TypePackerHandler(info);
                var serializer = Activator.CreateInstance(type);
                typePacker.Setup((ITypePacker)serializer);

                packerLookup.Add(info.Type, typePacker);
            }
        }

        static void BuildPrimitivePackers()
        {
            List<(Info, ITypePacker)> packerTypes = new List<(Info, ITypePacker)>
            {
                (new Info(typeof(byte), true), new ByteWrapper()),
                (new Info(typeof(sbyte), true), new SByteWrapper()),

                (new Info(typeof(short), true), new ShortWrapper()),
                (new Info(typeof(ushort), true), new UShortWrapper()),

                (new Info(typeof(int), true), new IntWrapper()),
                (new Info(typeof(uint), true), new UIntWrapper()),

                (new Info(typeof(long), true), new LongWrapper()),
                (new Info(typeof(ulong), true), new ULongWrapper()),

                (new Info(typeof(float), true), new FloatWrapper()),
                (new Info(typeof(double), true), new DoubleWrapper()),
                (new Info(typeof(decimal), true), new DecimalWrapper()),

                (new Info(typeof(DateTime), true), new DateTimeWrapper()),
                (new Info(typeof(string), true), new StringWrapper()),
            };

            foreach ((Info info, ITypePacker packer) in packerTypes)
            {
                if (!typeCache.ContainsKey(info.Type))
                {
                    typeCache.Add(info.Type, info);
                }

                // var handler = new TypePackerHandler(info);
                // handler.Setup(packer);
                // packerLookupFast[(int)Type.GetTypeCode(info.Type)] = new PackerEntry(handler);

                if (!packerLookup.ContainsKey(info.Type))
                {
                    var handler = new TypePackerHandler(info);
                    handler.Setup(packer);
                    packerLookup.Add(info.Type, handler);
                }
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

            foreach ((Type type, Info info) in typeCache)
            {
                if (packerLookup.ContainsKey(type))
                {
                    continue;
                }

                try
                {
                    if (PackerBuilder.CreateUnpacker(info) is Func<MethodInfo> deserDelegate)
                    {
                        serMethodCreators.Add((type, deserDelegate));
                    }
                }
                catch (Exception e)
                {
                    RePacker.Settings.Log.Error($"Error when generating unpacker for {type}");
                    RePacker.Settings.Log.Exception(e);
                    RePacker.Settings.Log.Warn(e.StackTrace);
                }

                try
                {
                    if (PackerBuilder.CreatePacker(info) is Func<MethodInfo> serDelegate)
                    {
                        deserMethodCreators.Add((type, serDelegate));
                    }
                }
                catch (Exception e)
                {
                    RePacker.Settings.Log.Error($"Error when generating packer for {type}");
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

            foreach ((Type type, Info info) in typeCache)
            {
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
                    HasCustomSerializer = type.GetInterface(nameof(IPacker)) != null,
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
            int idx = typeResolver.Resolver.Invoke(type);
            if (idx != -1)
            {
                packer = packerLookupFast[idx];
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
            int idx = typeResolver.Resolver.Invoke(typeof(T));
            if (idx != -1)
            {
                packerLookupFast[idx].Pack<T>(buffer, ref value);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Packer for {typeof(T)} not found");
            }
        }

        public static T Unpack<T>(BoxedBuffer buffer)
        {
            int idx = typeResolver.Resolver.Invoke(typeof(T));
            if (idx != -1)
            {
                return packerLookupFast[idx].Unpack<T>(buffer);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Unpacker for {typeof(T)} not found");
            }

            return default(T);
        }

        public static void UnpackInto<T>(BoxedBuffer buffer, ref T target)
        {
            int idx = typeResolver.Resolver.Invoke(typeof(T));
            if (idx != -1)
            {
                packerLookupFast[idx].Unpack<T>(buffer, target);
            }
            else
            {
                RePacker.Settings.Log.Warn($"Unpacker for {typeof(T)} not found");
            }
        }

        public static bool UnpackOut<T>(BoxedBuffer buffer, out T value)
        {
            int idx = typeResolver.Resolver.Invoke(typeof(T));
            if (idx != -1)
            {
                value = packerLookupFast[idx].Unpack<T>(buffer);
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
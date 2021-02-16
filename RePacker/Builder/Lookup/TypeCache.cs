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

        static TypeResolver typeResolver;
        static TypePackerHandler[] packerLookupFast;
        static Type lookupEnumType;
        static Func<Type, int> lookupFunction;

        static bool isSetup = false;
        public static bool IsSetup => isSetup;

        internal static void Reload()
        {
            isSetup = false;
            typeCache = null;
            packerLookup = null;
            packerLookupFast = null;
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

                // typeResolver = TypeResolverBuilder.BuildHandler(packerLookup);
            }

            isSetup = true;
            // allTypes = null;
        }

        internal static void AddTypePackerProvider(Type targetType, GenericProducer producer)
        {
            runtimePackerProducers.Add(targetType, producer);
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
            packerLookupFast = new TypePackerHandler[packerLookup.Count];

            /* var builder = new ABuilder()
                .NewModule("TypeCacheLookup")
                    .NewEnum(typeof(int), "TypesAsEnum")
                        .Run(eb =>
                        {
                            int index = 0;
                            foreach (var kv in packerLookup)
                            {
                                eb.AddEntry(kv.Key.Name, index);
                                packerLookupFast[index] = kv.Value;
                                index++;
                            }
                        })
                        .GetEnum(out lookupEnumType)
                    .Build()
                .Build(); */

            // foreach (object thing in Enum.GetValues(lookupEnumType))
            // {
            //     Console.WriteLine($"{thing}: {(int)thing}");
            // }
            // Enum.Parse(lookupEnumType, typeof(Int32).Name);

            /* var builder = new ABuilder()
                .NewModule("TypeCacheLookup")
                .Run(mb =>
                {
                    int index = 0;
                    Type[] parameters = new Type[1];
                    foreach (var kv in packerLookup)
                    {
                        parameters[0] = kv.Key;
                        mb.NewMethod(typeof(int), parameters, kv.Key.Name)
                        .GetILGenerator()
                        .Run(il =>
                        {
                            il
                                .Emit(OpCodes.Ldc_I4, index++)
                                .Emit(OpCodes.Ret);
                        })
                        .Build()
                        .Build(out var methodInfo);
                    }
                })
                .GetMethodBuilders(out var builders)
                .Build();

            foreach (var mb in builders)
            {
                Console.WriteLine(mb.MethodBuilder.Name);
            } */

            var returnLabel = Expression.Label(typeof(int));

            var cases = new SwitchCase[packerLookup.Count];
            int index = 0;
            foreach (var kv in packerLookup)
            {
                cases[index] = Expression.SwitchCase(
                    Expression.Return(returnLabel, Expression.Constant(index), typeof(int)),
                    Expression.Constant(kv.Key.GetHashCode())
                );

                packerLookupFast[index] = kv.Value;

                index++;
            }

            var hashCode = Expression.Variable(typeof(int), "HashCode");
            var parameter = Expression.Parameter(typeof(Type), "TypeParameter");

            var switchExpr = Expression.Switch(
                hashCode,
                Expression.Return(returnLabel, Expression.Constant(-1), typeof(int)),
                cases);

            MethodInfo getHashCode = typeof(Type).GetMethod(nameof(Type.GetHashCode));

            var methodExp = Expression.Block(
                new[] { hashCode },
                Expression.Assign(hashCode, Expression.Call(parameter, getHashCode)),
                switchExpr,
                Expression.Label(returnLabel, Expression.Constant(-1))
            );

            lookupFunction = Expression
                .Lambda<Func<Type, int>>(methodExp, parameter)
                .Compile();
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

        static void BuildRuntimePackerProviders()
        {
            runtimePackerProducers = new Dictionary<Type, GenericProducer>();

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

            runtimePackerProducers.Add(
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
            BuildLookup();
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
            /* int resolverIndex = typeResolver.Resolver.Invoke(typeof(T));
            if (resolverIndex != -1)
            {
                packerLookupFast[resolverIndex].Pack<T>(buffer, ref value);
                return;
            } */

            int fastIndex = lookupFunction.Invoke(typeof(T));
            if (fastIndex != -1)
            {
                packerLookupFast[fastIndex].Pack<T>(buffer, ref value);
            }
            /* else if (packerLookup.TryGetValue(typeof(T), out var packer))
            {
                packer.Pack<T>(buffer, ref value);
            } */
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
            /* int resolverIndex = typeResolver.Resolver.Invoke(typeof(T));
            if (resolverIndex != -1)
            {
                return packerLookupFast[resolverIndex].Unpack<T>(buffer);
            } */

            int fastIndex = lookupFunction.Invoke(typeof(T));
            if (fastIndex != -1)
            {
                return packerLookupFast[fastIndex].Unpack<T>(buffer);
            }
            /* else if (packerLookup.TryGetValue(typeof(T), out var packer))
            {
                return packer.Unpack<T>(buffer);
            } */
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
        #endregion
    }
}
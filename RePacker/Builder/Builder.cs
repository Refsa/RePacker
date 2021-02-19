/* using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Refsa.RePacker.Buffers;
using Refsa.RePacker.Utils;
using Buffer = Refsa.RePacker.Buffers.Buffer;

namespace Refsa.RePacker.Builder
{
    internal class Builder { }

    internal class ABuilderChild : Builder
    {
        protected ABuilder owner;

        public ABuilderChild(ABuilder owner)
        {
            this.owner = owner;
        }
    }

    internal class MBuilderChild : Builder
    {
        protected MBuilder owner;

        public MBuilderChild(MBuilder owner)
        {
            this.owner = owner;
        }
    }

    internal class ABuilder : Builder
    {
        public AssemblyBuilder AssemblyBuilder;
        public List<MBuilder> ModuleBuilders;

        public ABuilder(string name = null)
        {
            if (string.IsNullOrEmpty(name)) name = Guid.NewGuid().ToString();

            AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(
                new AssemblyName(name),
                AssemblyBuilderAccess.Run);

            ModuleBuilders = new List<MBuilder>();
        }

        public MBuilder NewModule(string name = null)
        {
            var moduleBuilder = new MBuilder(this, name);

            ModuleBuilders.Add(moduleBuilder);
            return moduleBuilder;
        }

        public ABuilder Run(Action<ABuilder> predicate)
        {
            predicate.Invoke(this);
            return this;
        }
    }

    internal class MBuilder : ABuilderChild
    {
        public ModuleBuilder ModuleBuilder;
        public List<EBuilder> EnumBuilders;
        public List<FBuilder> MethodBuilders;

        public MBuilder(ABuilder owner, string name = null) : base(owner)
        {
            if (string.IsNullOrEmpty(name)) name = Guid.NewGuid().ToString();
            ModuleBuilder = owner.AssemblyBuilder.DefineDynamicModule(name);

            EnumBuilders = new List<EBuilder>();
            MethodBuilders = new List<FBuilder>();
        }

        public ABuilder Build()
        {
            ModuleBuilder.CreateGlobalFunctions();
            return owner;
        }

        public EBuilder NewEnum(Type underlyingType, string name = null)
        {
            var enumBuilder = new EBuilder(this, underlyingType, name);
            EnumBuilders.Add(enumBuilder);
            return enumBuilder;
        }

        public FBuilder NewMethod(Type returnType, Type[] parameters, string name = null)
        {
            var builder = new FBuilder(this, returnType, parameters, name);
            MethodBuilders.Add(builder);
            return builder;
        }

        public MBuilder Run(Action<MBuilder> predicate)
        {
            predicate.Invoke(this);
            return this;
        }

        public MBuilder GetMethodBuilders(out List<FBuilder> builders)
        {
            builders = MethodBuilders;
            return this;
        }
    }

    internal class EBuilder : MBuilderChild
    {
        public EnumBuilder EnumBuilder;

        public EBuilder(MBuilder owner, Type underlyingType, string name = null) : base(owner)
        {
            if (string.IsNullOrEmpty(name)) name = Guid.NewGuid().ToString();
            EnumBuilder = owner.ModuleBuilder.DefineEnum(name, (TypeAttributes)0, underlyingType);
        }

        public MBuilder Build()
        {
            return owner;
        }

        public EBuilder AddEntry(string name, object value)
        {
            EnumBuilder.DefineLiteral(name, value);
            return this;
        }

        public EBuilder Run(Action<EBuilder> predicate)
        {
            predicate.Invoke(this);
            return this;
        }

        public EBuilder GetEnum(out Type enumType)
        {
            enumType = EnumBuilder.CreateType();
            return this;
        }
    }

    internal class FBuilder : MBuilderChild
    {
        public MethodBuilder MethodBuilder;

        public FBuilder(MBuilder owner, Type returnType, Type[] parameters, string name = null) : base(owner)
        {
            if (string.IsNullOrEmpty(name)) name = Guid.NewGuid().ToString();
            MethodBuilder = owner.ModuleBuilder
                .DefineGlobalMethod(
                    name,
                    MethodAttributes.Public | MethodAttributes.Static,
                    returnType,
                    parameters
                );
        }

        public ILBuilder<FBuilder> GetILGenerator()
        {
            var generator = MethodBuilder.GetILGenerator();
            var builder = new ILBuilder<FBuilder>(this, generator);

            return builder;
        }

        public MBuilder Build(out MethodInfo info)
        {
            info = MethodBuilder;
            return owner;
        }
    }

    internal class ILBuilder<T> where T : Builder
    {
        T owner;

        public ILGenerator ILGenerator;

        public ILBuilder(T owner, ILGenerator generator)
        {
            this.owner = owner;
            this.ILGenerator = generator;
        }

        public T Build()
        {
            return owner;
        }

        public ILBuilder<T> Run(Action<ILBuilder<T>> predicate)
        {
            predicate.Invoke(this);
            return this;
        }

        public ILBuilder<T> DeclareLocal(Func<Type> predicate)
        {
            ILGenerator.DeclareLocal(predicate.Invoke());
            return this;
        }

        public ILBuilder<T> Emit(OpCode opCode)
        {
            ILGenerator.Emit(opCode);
            return this;
        }

        public ILBuilder<T> Emit(OpCode opCode, int arg)
        {
            ILGenerator.Emit(opCode, arg);
            return this;
        }
    }
} */
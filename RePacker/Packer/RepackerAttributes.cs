using System;
using System.Reflection;

namespace Refsa.RePacker
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class RePackerAttribute : System.Attribute
    {
        public bool UseOnAllPublicFields;

        public RePackerAttribute(bool useOnAllPublicFields = true)
        {
            this.UseOnAllPublicFields = useOnAllPublicFields;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class RePackerWrapperAttribute : System.Attribute
    {
        public Type WrapperFor;

        public RePackerWrapperAttribute(Type wrapperFor)
        {
            
            WrapperFor = wrapperFor;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class IdentifierAttribute : System.Attribute
    {
        public int Order;

        public IdentifierAttribute(int order = 0)
        {
            Order = order;
        }
    }
}
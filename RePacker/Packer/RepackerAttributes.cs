using System;

namespace RePacker
{
    /// <summary>
    /// Marks a struct/class to generate a packer for it
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public sealed class RePackerAttribute : System.Attribute
    {
        public bool UseOnAllPublicFields;

        public RePackerAttribute(bool useOnAllPublicFields = true)
        {
            this.UseOnAllPublicFields = useOnAllPublicFields;
        }
    }
    
    /// <summary>
    /// Marks a field or property to be packed
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class RePackAttribute : System.Attribute
    {
        public int Order;

        public RePackAttribute(int order = 0)
        {
            Order = order;
        }
    }
}
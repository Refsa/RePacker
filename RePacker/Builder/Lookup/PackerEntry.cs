using System;
using System.Collections.Generic;

namespace RePacker.Builder
{
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
}
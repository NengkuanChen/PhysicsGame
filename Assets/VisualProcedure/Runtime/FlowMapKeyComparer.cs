using System;
using System.Collections.Generic;

namespace VisualProcedure.Runtime
{
    public class FlowMapKeyComparer : IEqualityComparer<(Type,int)>
    {
        public bool Equals((Type, int) x, (Type, int) y)
        {
            return x.Item1 == y.Item1 && x.Item2 == y.Item2;
        }

        public int GetHashCode((Type, int) obj)
        {
            unchecked
            {
                return (obj.Item1.GetHashCode() * 397) ^ obj.Item2;
            }
        }
    }
}
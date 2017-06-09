using DPRec_Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Helpers
{
    public class TypeComparer : IEqualityComparer<NamedTypeBase>
    {
        public bool Equals(NamedTypeBase x, NamedTypeBase y)
        {
            return x == y;
        }

        public int GetHashCode(NamedTypeBase obj)
        {
            return obj.GetHashCode();
        }
    }
}

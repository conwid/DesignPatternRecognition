using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Model
{
    public interface IMethodParameter
    {
        string Name { get; set; }

        bool IsTypeParameter { get; set; }

        INamedType Type { get;set; }

        int Ordinal { get; set; }

    }
}

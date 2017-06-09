using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Model
{
    public interface IGenericTypeParameter
    {
        int Position { get; set; }

        bool IsSubstituted { get; set; }

        INamedType Type { get; set; }
    }
}

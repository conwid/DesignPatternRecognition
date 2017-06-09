using DPRec_Lib.Model;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.CSharpImplementation
{
    [Serializable]
    public class CSharpNamedType : NamedTypeBase
    {
        [NonSerialized]
        private ITypeSymbol s;
        public ITypeSymbol SourceSymbol { get { return s; } set { s = value; } }
    }
}

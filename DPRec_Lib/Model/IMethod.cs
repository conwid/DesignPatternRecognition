using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Model
{
    public interface IMethod : ICodeElement
    {
        INamedType DeclaredReturnType { get; set; }

        INamedType Parent { get; set; }
        
        List<INamedType> ActualReturnTypes { get; set; }

        List<IMethodParameter> Parameters { get; set; }

        string Name { get; set; }

        
        bool IsStatic { get; set; }

        bool IsAbstract { get; set; }

        IVisibilityModifier Visibility { get;set; }

        bool IsVirtual { get; set; }

        bool IsCtor { get; set; }

        bool IsOverride { get; set; }

        bool IsGeneric { get; set; }

        List<IGenericTypeParameter> GenericParameters { get; set; }
    }
}

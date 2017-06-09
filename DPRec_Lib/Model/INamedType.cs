using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Model
{
    public interface INamedType :ICodeElement
    {
        string Namespace { get; set; }
        string Name { get; set; }        
        IVisibilityModifier Visibility { get; set; }
        List<INamedType> Bases { get; set; }
        bool IsAbstract { get; set; }
        bool IsStatic { get; set; }

        List<IField> Fields { get; set; }

        List<IMethod> Methods { get; set; }

        List<IGenericTypeParameter> GenericParameters { get; set; }

        bool IsGeneric { get; set; }        

        bool IsArray { get; set; }
    }
}

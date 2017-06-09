using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Model
{
    public interface IField : ICodeElement
    {
        string Name { get; set; }

        INamedType Parent { get; set; }

        IVisibilityModifier Visibility { get; set; }

        bool IsInitializedOnce { get; set; }

        bool IsInitializedOnCreation { get; set; }

        List<IMethod> DirectlyReturningMethods { get; set; }

        bool AutoImplemented { get; set; }

        bool IsStatic { get; set; }
        INamedType Type { get; set; }
    }
}

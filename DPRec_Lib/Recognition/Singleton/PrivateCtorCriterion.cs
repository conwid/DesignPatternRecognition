using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPRec_Lib.Model;

namespace DPRec_Lib.Recognition.Singleton
{
    public class PrivateCtorCriterion : ICriterion
    {
        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            var type = (NamedTypeBase)elementToAnalyze;
            return
                type.Methods.Where(m => m.IsCtor).All(m => (VisibilityModifierBase)m.Visibility == VisibilityModifierBase.Private)
                &&
                type.Methods.Any(m => m.IsCtor);
           
        }
    }
}

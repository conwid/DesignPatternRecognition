using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPRec_Lib.Model;

namespace DPRec_Lib.Recognition.Singleton
{
    public class FieldCriterion : ICriterion
    {
        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            var type = (NamedTypeBase)elementToAnalyze;
            var candidateFields = type.Fields.Where(f => f.IsStatic && (NamedTypeBase)f.Type == type && (f.IsInitializedOnce || f.IsInitializedOnCreation)).ToList();
            if (candidateFields.Count!=1)
            {
                return false;
            }
            var candidateField = candidateFields.Single();
            if ((VisibilityModifierBase)candidateField.Visibility==VisibilityModifierBase.Public)
            {
                return true;
            }
            if (candidateField.DirectlyReturningMethods.Count == 1 && (VisibilityModifierBase)candidateField.DirectlyReturningMethods[0].Visibility == VisibilityModifierBase.Public)
            {
                return true;
            }
            return false;
        }
    }
}

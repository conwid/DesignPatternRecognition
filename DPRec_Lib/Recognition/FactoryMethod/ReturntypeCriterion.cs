using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPRec_Lib.Model;

namespace DPRec_Lib.Recognition.FactoryMethod
{
    public class ReturntypeCriterion : ICriterion
    {
        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            MethodBase m = elementToAnalyze as MethodBase;
            if (m == null)
            {
                throw new ArgumentException();
            }
            if (m.ActualReturnTypes==null || m.DeclaredReturnType==null || !m.ActualReturnTypes.Any())
            {
                return false;
            }
            return m.ActualReturnTypes.All(at =>
                at.Bases.Any(atb => (NamedTypeBase)atb == (NamedTypeBase)m.DeclaredReturnType));

        }
    }
}

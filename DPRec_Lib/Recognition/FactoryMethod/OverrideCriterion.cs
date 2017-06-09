using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPRec_Lib.Model;

namespace DPRec_Lib.Recognition.FactoryMethod
{
    public class OverrideCriterion : ICriterion
    {
        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            MethodBase m = elementToAnalyze as MethodBase;
            if (m==null)
            {
                throw new ArgumentException();
            }
            return m.IsOverride;
        }
    }
}

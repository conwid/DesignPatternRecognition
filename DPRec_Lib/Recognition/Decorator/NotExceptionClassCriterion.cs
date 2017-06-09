using DPRec_Lib.Model;
using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Decorator
{
    public class NotExceptionClassCriterion : ICriterion
    {
        private bool DerivesFrom(NamedTypeBase type, string typename)
        {
            if (type.Bases.Any(b=>b.Name==typename))
            {
                return true;
            }
            foreach (var item in type.Bases)
            {
                if (DerivesFrom((NamedTypeBase)item,typename))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context)
        {
            var type = (NamedTypeBase)elementToAnalyze;
            return !DerivesFrom(type, "Exception");
        }
    }
}
using DPRec_Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Model
{
    public abstract class PatternRecognizerBase
    {
        protected List<ICriterion> Criteria { get; private set; } = new List<ICriterion>();

        public RecognitionContextBase Context { get; set; }

        public virtual bool IsInstance(ICodeElement elementToAnalyze)
        {            
            foreach (var criterion in this.Criteria)
            {
                if (!criterion.Check(elementToAnalyze, Context))
                {
                    return false;
                }
            }
            return true;
        }
    }
}

using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Decorator
{
    public class DecoratorRecognizer : PatternRecognizerBase
    {
        public DecoratorRecognizer() : base()
        {
            this.Criteria.Add(new DecoratorComplexCriterion());
            this.Criteria.Add(new NotExceptionClassCriterion());
        }
    }
}

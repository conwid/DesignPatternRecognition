using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Mediator
{
    public class MediatorRecognizer : PatternRecognizerBase
    {
        public MediatorRecognizer() : base()
        {
            this.Criteria.Add(new MediatorComplexCriterion());
        }
    }
}

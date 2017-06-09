using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.FactoryMethod
{
    public class FactoryMethodRecognizer : PatternRecognizerBase
    {
        public FactoryMethodRecognizer() : base()
        {
            this.Criteria.Add(new OverrideCriterion());
            this.Criteria.Add(new ReturntypeCriterion());
        }
    }
}

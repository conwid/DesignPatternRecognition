using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Singleton
{
    public class SingletonRecognizer : PatternRecognizerBase
    {
        public SingletonRecognizer() : base()
        {
            this.Criteria.Add(new PrivateCtorCriterion());
            //this.Criteria.Add(new CtorCallCountCriterion());
            this.Criteria.Add(new FieldCriterion());
        }
    }
}

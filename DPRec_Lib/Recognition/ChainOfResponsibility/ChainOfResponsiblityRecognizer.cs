using DPRec_Lib.Recognition.ChainOfResponsibility;
using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.ChainOfResponbility
{
    public class ChainOfResponsiblityRecognizer : PatternRecognizerBase
    {
        public ChainOfResponsiblityRecognizer() : base()
        {
            this.Criteria.Add(new ChainOfResponsiblityComplexCriterion());
        }
    }
}

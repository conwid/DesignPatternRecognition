using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Proxy
{
    public class ProxyRecognizer : PatternRecognizerBase
    {
        public ProxyRecognizer() : base()
        {
            this.Criteria.Add(new ProxyComplexCriterion());
        }
    }
}

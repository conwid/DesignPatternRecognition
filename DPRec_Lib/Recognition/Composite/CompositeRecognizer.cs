using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Composite
{
    public class CompositeRecognizer : PatternRecognizerBase
    {
        public CompositeRecognizer()
        {
            this.Criteria.Add(new CollectionCriterion());
        }
    }
}

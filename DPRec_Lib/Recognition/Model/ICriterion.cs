using DPRec_Lib.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Model
{
    public interface ICriterion
    {
        bool Check(ICodeElement elementToAnalyze, RecognitionContextBase context);
    }
}

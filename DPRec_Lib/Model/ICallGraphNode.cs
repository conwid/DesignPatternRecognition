using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Model
{
    public interface ICallGraphNode
    {
         IMethod Caller { get; set; }
         IMethod Callee { get; set; }
    }
}

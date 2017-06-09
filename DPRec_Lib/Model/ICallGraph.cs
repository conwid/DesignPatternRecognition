using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Model
{    
    public interface ICallGraph
    {
        List<ICallGraphNode> Nodes { get; set; }
    }
}

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using DPRec_Lib.Model;
using DPRec_Lib.Logging;
using System;

namespace DPRec_Lib.Model
{
    [Serializable]
    public class CallGraphBase : ICallGraph
    {       
        private List<ICallGraphNode> nodes = new List<ICallGraphNode>();

        public List<ICallGraphNode> Nodes
        {
            get
            {
                return this.nodes;
            }

            set
            {
                this.nodes = value;
            }
        }
    }
}

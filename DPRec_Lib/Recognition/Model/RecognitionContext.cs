using DPRec_Lib.CSharpImplementation;
using DPRec_Lib.Logging;
using DPRec_Lib.Model;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Recognition.Model
{
    public abstract class RecognitionContextBase
    {
        public string Name { get; private set; }
        public List<NamedTypeBase> Types { get; protected set; }

        public LoggerProvider Logger { get; set; }

        public CallGraphBase CallGraph { get; protected set; }       

        public bool Initialized { get; protected set; }

        public abstract void Init(LoggerProvider p, bool reInit=false);

        public RecognitionContextBase(string name)
        {
            this.Name = name;
        }
    }
}

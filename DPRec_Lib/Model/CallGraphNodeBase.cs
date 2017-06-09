using System;

namespace DPRec_Lib.Model
{
    [Serializable]
    public class CallGraphNodeBase : ICallGraphNode
    {
        private MethodBase caller { get; set; }
        private MethodBase callee { get; set; }

        public IMethod Callee
        {
            get
            {
                return callee;
            }

            set
            {
                callee=(MethodBase)value;
            }
        }

        public IMethod Caller
        {
            get
            {
                return caller;
            }

            set
            {
                caller = (MethodBase)value;
            }
        }

        public override string ToString()
        {
            return string.Concat(caller.ToString(), " --> ", callee.ToString());
        }
    }
}

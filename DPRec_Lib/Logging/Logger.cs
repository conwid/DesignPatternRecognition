using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Logging
{
    public class Logger
    {
        protected event Action<string> LogEvent;
        public virtual void Log(string message)
        {
            RaiseLogEvent(message);
        }
        private  void RaiseLogEvent(string v)
        {
            var temp = LogEvent;
            if (temp!=null)
            {
                temp(v);
            }
        }
    }
}

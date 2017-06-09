using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Logging
{
    public class DebugLogger : Logger
    {
        public DebugLogger()
        {
            this.LogEvent += Logger_LogEvent;
        }

        private void Logger_LogEvent(string obj)
        {
            Debug.WriteLine(obj);
        }
    }
}

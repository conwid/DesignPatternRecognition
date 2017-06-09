using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Logging
{
    public class CustomizableLogger : Logger
    {
        public CustomizableLogger(Action<string> log) : base()
        {
            this.LogEvent += log;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Logging
{
    public class ConsoleLogger : Logger
    {
     
        public ConsoleLogger()
        {
            this.LogEvent += Logger_LogEvent;
        }

        private void Logger_LogEvent(string obj)
        {
            Console.WriteLine(obj);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Logging
{
    public class LoggerProvider
    {
        private List<Logger> loggers=new List<Logger>();

        public LoggerProvider()
        {
            loggers = new List<Logger>();              
        }


        public void AddLogger(Logger l)
        {
            loggers.Add(l);
        }

        public void Remove(Logger l)
        {
            loggers.Remove(l);
        }

        public void WriteToLog(string message, [CallerMemberName]string method = "", [CallerLineNumber] int lineNumber = 0)
        {
            string msg;
            if (lineNumber==0)
            {
                msg = string.Format("{0}: {1}", DateTime.Now, message);
            }
            else
            {
                msg = string.Format("{0}: {1} in {2} at line {3}", DateTime.Now, message, method, lineNumber);
            }
            foreach (var logger in loggers)
            {
                logger.Log(msg);
            }
        }
    }
}

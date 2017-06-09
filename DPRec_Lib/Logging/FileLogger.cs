using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Logging
{
    public class FileLogger :Logger
    {
        private string fileName;
        public FileLogger(string fileName)
        {
            this.LogEvent += Logger_LogEvent;
            this.fileName = fileName;
        }

        private void Logger_LogEvent(string obj)
        {
            using (StreamWriter sw=new StreamWriter(fileName,true))
            {
                sw.WriteLine(obj);
            }            
        }
    }
}

using DPRec_Lib.CSharpImplementation;
using DPRec_Lib.Logging;
using DPRec_Lib.Recognition.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRecDiag
{
    public class RecognitionContextManager
    {
        public static RecognitionContextBase ctx = new CSharpRecognitionContext("");

        private static LoggerProvider lp = new LoggerProvider();

        private static bool isInitialized;

        public static LoggerProvider GetLogger()
        {
            if (!isInitialized)
            {
                throw new NotSupportedException("You have to initialize the context manager first!");
            }
            return lp;
        }     

        public static void InitLogger()
        {
            if (isInitialized) { return; }
            lp.AddLogger(new FileLogger(@"C: \Users\Akos\Desktop\out.txt"));
            isInitialized = true;
        }
    }
}

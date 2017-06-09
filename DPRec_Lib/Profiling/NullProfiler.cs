using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPRec_Lib.Profiling
{
    public class NullProfiler : IProfiler
    {

        public NullProfiler(Action<string,TimeSpan> logger)
        {

        }

        public IDisposable Method(string name)
        {
            return null;
        }

        public IDisposable Section(string name)
        {
            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace DPRec_Lib.Profiling
{
    public class CustomizableProfiler : IProfiler
    {
        private Action<string,TimeSpan> logger;
        public CustomizableProfiler(Action<string,TimeSpan> logger)
        {
            this.logger = logger;
        }

        public IDisposable Section(string name)
        {
            return new ProfileContext(name, false, this.logger);
        }

        public IDisposable Method([CallerMemberName]string name = "")
        {
            return new ProfileContext(name, true, this.logger);
        }


        class ProfileContext : IDisposable
        {
            string text;
            bool isMethod;
            Stopwatch watch;
            Action<string, TimeSpan> logger;

            [ThreadStatic]
            static int indent = -1;

            public ProfileContext(string text, bool isMethod, Action<string,TimeSpan> logger)
            {

                this.text = text;
                this.isMethod = isMethod;
                watch = Stopwatch.StartNew();
                this.logger = logger;
                indent++;
                StringBuilder sb = new StringBuilder();
                sb.Append("".PadLeft(indent * 4, ' '));
                if (this.isMethod)
                {
                    sb.Append(string.Format("Method {0} started at {1}", this.text, DateTime.Now.ToString()));
                }
                else
                {
                    sb.Append(string.Format("Section {0} started at {1}", this.text, DateTime.Now.ToString()));
                }
                if (logger != null)
                {
                    this.logger(sb.ToString(), watch.Elapsed);
                }
            }

            public void Dispose()
            {
                watch.Stop();
                StringBuilder sb = new StringBuilder();
                sb.Append("".PadLeft(indent * 4, ' '));
                if (this.isMethod)
                {
                    sb.Append(string.Format("Method {0} finished in {1}", this.text, watch.Elapsed.ToString()));
                }
                else
                {
                    sb.Append(string.Format("Section {0} finished in {1}", this.text, watch.Elapsed.ToString()));
                }
                if (logger!=null)
                {
                    this.logger(sb.ToString(),watch.Elapsed);
                }
                indent--;
            }
        }

    }
}

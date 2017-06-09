
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPRec_Lib.Profiling
{
    public static class ProfilerService
    {
        private static IProfiler instance = null;
        private static Action<string, TimeSpan> logger { get; set; }

        private static bool isInitialized = false;


        public static void Init(Type type, Action<string, TimeSpan> loggerAction)
        {
            instance = (IProfiler)Activator.CreateInstance(type, loggerAction);
            logger = loggerAction;
            isInitialized = true;
        }

        public static IProfiler Current
        {
            get
            {
                if (!isInitialized)
                {
                    throw new InvalidOperationException("ProfilerService is not initialized");
                }
                return instance;
            }
            set
            {
                if (!isInitialized)
                {
                    throw new InvalidOperationException("ProfilerService is not initialized");
                }
                instance = value;
            }
        }

        public static IDisposable Section(string name)
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException("ProfilerService is not initialized");
            }
            return Current.Section(name);
        }


        public static IDisposable Method(string name)
        {
            if (!isInitialized)
            {
                throw new InvalidOperationException("ProfilerService is not initialized");
            }
            return Current.Method(name);
        }

    }
}

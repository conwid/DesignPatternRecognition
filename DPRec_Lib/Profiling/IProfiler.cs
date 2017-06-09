using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DPRec_Lib.Profiling
{
	public interface IProfiler
	{
		IDisposable Section( string name);
		IDisposable Method( string name);
	}
}

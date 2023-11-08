#if PCL || ((!UNITY_EDITOR) && (ENABLE_DOTNET))
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Diagnostics
{
	internal class Stopwatch
	{
		DateTime startTime, stopTime;

		public void Start() 
		{
			startTime = DateTime.UtcNow;
		}
		
		public void Stop() 
		{
			stopTime = DateTime.UtcNow;
		}

		public static Stopwatch StartNew()
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();
			return sw;
		}

		public long ElapsedMilliseconds
		{
			get
			{
				return (long)((stopTime - startTime).TotalMilliseconds);
			}
		}


	}
}
#endif
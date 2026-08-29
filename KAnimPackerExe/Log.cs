using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _KAnimPackerExe
{
	internal class Log
	{
		public static bool HasLoggedErrors = false;
		public static void LogError(string message)
		{
			Console.Write("[ERROR]: ");
			Console.WriteLine(message);
			HasLoggedErrors = true;
		}
		public static void LogErrorFromException(Exception ex, bool showStackTrace = false)
		{
			LogError(ex.Message);
			if(showStackTrace)
				Console.WriteLine(ex.StackTrace);
		}
		public static void LogMessage(string message)
		{
			Console.Write(value: "[INFO]: ");
			Console.WriteLine(message);
		}
	}
}

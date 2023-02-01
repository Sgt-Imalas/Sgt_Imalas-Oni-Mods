using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UtilLibs
{
    public static class SgtLogger
    {
        public static void DebugLog(string message) 
        {
            string messageToLog = string.Concat("["+System.DateTime.Now.ToString("HH:mm:tt")+"] ["+Assembly.GetExecutingAssembly()+"]: ",message);
#if DEBUG
            Console.WriteLine(messageToLog);
#endif
        }  
    }
}

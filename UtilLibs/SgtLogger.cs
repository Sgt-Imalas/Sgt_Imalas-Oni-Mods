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
        public static void debuglog(string message) 
        {
            string messageToLog = string.Concat("["+ TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss") +"] ["+Assembly.GetExecutingAssembly().GetName().Name+"]: ",message);
#if DEBUG
            Console.WriteLine(messageToLog);
#endif
        }  
    }
}

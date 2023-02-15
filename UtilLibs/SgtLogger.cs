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
        public static void l(string message)
        {
            debuglog(message);
        }
        public static void debuglog(string message, string assemblyOverride = "") 
        {
            if(assemblyOverride == "")
                assemblyOverride= Assembly.GetExecutingAssembly().GetName().Name;
            string messageToLog = string.Concat("["+ TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss") + "] [INFO] [" + assemblyOverride+"]: ",message);
#if DEBUG
            Console.WriteLine(messageToLog);
#endif
        }

        public static void logwarning(string message)
        {
            dlogwarn(message);
        }


        public static void dlogwarn(string message)
        {
            string messageToLog = string.Concat("[" + TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss") + "] [WARNING] [" + Assembly.GetExecutingAssembly().GetName().Name + "]: ", message);
#if DEBUG
            Console.WriteLine(messageToLog);
#endif
        }
    }
}

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

        public static void l(string message, string assemblyOverride = "")
        {
            debuglog(message, assemblyOverride);
        }
        public static void debuglog(string message, string assemblyOverride = "") 
        {
            if(assemblyOverride == "")
                assemblyOverride= Assembly.GetExecutingAssembly().GetName().Name;
            string messageToLog = string.Concat("["+ TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss.fff") + "] [INFO] [" + assemblyOverride+"]: ",message);

            Console.WriteLine(messageToLog);

        }


        public static void log(string message, string assemblyOverride = "") => debuglog(message, assemblyOverride);
        public static void warning(string message, string assemblyOverride = "") => dlogwarn(message, assemblyOverride);
        public static void error(string message, string assemblyOverride = "") => dlogerror(message, assemblyOverride);

        public static void logwarning(string message, string assemblyOverride = "") => dlogwarn(message, assemblyOverride);
        public static void logerror(string message, string assemblyOverride = "") =>  dlogerror(message, assemblyOverride);


        public static void dlogwarn(string message, string assemblyOverride = "")
        {
            if (assemblyOverride == "")
                assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
            string messageToLog = string.Concat("[" + TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss.fff") + "] [WARNING] [" + assemblyOverride + "]: ", message);

            Console.WriteLine(messageToLog, assemblyOverride);
        }
        public static void dlogerror(string message, string assemblyOverride = "")
        {
            if (assemblyOverride == "")
                assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
            string messageToLog = string.Concat("[" + TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss.fff") + "] [ERROR] [" + assemblyOverride + "]: ", message);

            Console.WriteLine(messageToLog, assemblyOverride);
        }

        public static void logError(string v)
        {
            throw new NotImplementedException();
        }
    }
}

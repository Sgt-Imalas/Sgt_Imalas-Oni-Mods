using KMod;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs.ModVersionCheck;

namespace UtilLibs
{
    public static class SgtLogger
    {
        public static void LogVersion(UserMod2 usermod)
        {
            VersionChecker.HandleVersionChecking(usermod);
            debuglog($"{usermod.mod.staticID} - Mod Version: {usermod.mod.packagedModInfo.version} ");
        }
        public static void l(string message, string assemblyOverride = "")
        {
            debuglog(message, assemblyOverride);
        }
        public static void Assert(string name, object arg)
        {
            if (arg == null)
            {
                warning($"Assert failed, {name} is null");
            }
        }

        public static void debuglog(object a,object b = null, object c = null, object d = null)
        {
            var message = a.ToString() + b !=null? " "+b.ToString() : string.Empty + c != null ? " " + c.ToString() : string.Empty + d != null ? " " + d.ToString() : string.Empty;


              string assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
            string messageToLog = string.Concat("[" + TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss.fff") + "] [INFO] [" + assemblyOverride + "]: ", message);

            Console.WriteLine(messageToLog);

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
            
              var  assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
            string messageToLog = string.Concat("[" + TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss.fff") + "] [ERROR] [" + assemblyOverride + "]: ", v);

            Console.WriteLine(messageToLog, assemblyOverride);
        }
    }
}

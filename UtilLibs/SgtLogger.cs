using HarmonyLib;
using KMod;
using PeterHan.PLib.AVC;
using System;
using System.Reflection;
using System.Threading;

namespace UtilLibs
{
	public class SgtLogger
	{

		public SgtLogger(string log)
		{
			l(log);
		}

		static Harmony harmony;

		public static void LogVersionAndInitUpdating(UserMod2 usermod, Harmony _harmony)
		{
			LogVersion(usermod, _harmony);
			// ModUpdatingState.UpdatingInstance = true;
		}

		public static void LogVersion(UserMod2 usermod, Harmony _harmony, bool VersionChecking = true)
		{
			harmony = _harmony;
			if (VersionChecking)
			{
				ModVersionCheck.VersionChecker.HandleVersionChecking(usermod, harmony);
				var VersionChecker = new PVersionCheck();
				//VersionChecker.Register(usermod, new JsonURLVersionChecker("https://raw.githubusercontent.com/Sgt-Imalas/Sgt_Imalas-Oni-Mods/master/ModVersionData.json")); //Currently partially broken
				VersionChecker.Register(usermod, new SteamVersionChecker());
			}
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

		public static void debuglog(object a, object b = null, object c = null, object d = null)
		{
			var message = a.ToString() + b != null ? " " + b.ToString() : string.Empty + c != null ? " " + c.ToString() : string.Empty + d != null ? " " + d.ToString() : string.Empty;


			string assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
			string messageToLog = string.Concat(TimeStamp()," [INFO] [" , assemblyOverride , "]: ", message);

			Console.WriteLine(messageToLog);

		}

		public static void debuglog(string message, string assemblyOverride = "")
		{
			if (assemblyOverride == "")
				assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
			string messageToLog = string.Concat(TimeStamp(), " [INFO] [",assemblyOverride,"]: ", message);
			//Debug.Log(messageToLog);
			Console.WriteLine(messageToLog);

		}
		public static string TimeStamp() => string.Concat("[" + TimeZoneInfo.ConvertTimeToUtc(System.DateTime.Now).ToString("HH:mm:ss.fff"), "] [", Thread.CurrentThread.ManagedThreadId, "]");


		public static void log(string message, string assemblyOverride = "") => debuglog(message, assemblyOverride);
		public static void warning(string message, string assemblyOverride = "") => dlogwarn(message, assemblyOverride);
		public static void error(string message, string assemblyOverride = "") => dlogerror(message, assemblyOverride);

		public static void logwarning(string message, string assemblyOverride = "") => dlogwarn(message, assemblyOverride);
		public static void logerror(string message, string assemblyOverride = "") => dlogerror(message, assemblyOverride);


		public static void dlogwarn(string message, string assemblyOverride = "")
		{
			if (assemblyOverride == "")
				assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
			string messageToLog = string.Concat(TimeStamp()," [WARNING] [" , assemblyOverride , "]: ", message);

			Console.WriteLine(messageToLog, assemblyOverride);
		}
		public static void dlogerror(string message, string assemblyOverride = "")
		{
			if (assemblyOverride == "")
				assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
			string messageToLog = string.Concat(TimeStamp(), " [ERROR] [" ,assemblyOverride , "]: ", message);

			Console.WriteLine(messageToLog, assemblyOverride);
		}

		public static void logError(string v)
		{

			var assemblyOverride = Assembly.GetExecutingAssembly().GetName().Name;
			string messageToLog = string.Concat(TimeStamp(), " [ERROR] [" , assemblyOverride , "]: ", v);

			Console.WriteLine(messageToLog, assemblyOverride);
		}
    }
}

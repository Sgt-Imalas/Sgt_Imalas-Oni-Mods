using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace UtilLibs.SharedTweaks
{
	internal class SearchUtilCrashFix : PForwardedComponent
	{
		public static void Register()
		{
			new SearchUtilCrashFix().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethod = AccessTools.Method(typeof(Db), nameof(Db.Initialize));
				var prefix = AccessTools.Method(typeof(SearchUtilCrashFix), nameof(Prefix));
				plibInstance.Patch(targetMethod, new(prefix));
				Debug.Log(Assembly.GetExecutingAssembly().GetName().Name + ": " + this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}
		/// <summary>
		/// fixes translations of things falling back to english due to loading too early from mods
		/// </summary>
		public static bool Prefix(string s, ref string __result)
		{
			__result = string.Empty;
			return !s.IsNullOrWhiteSpace();
		}
	}

}

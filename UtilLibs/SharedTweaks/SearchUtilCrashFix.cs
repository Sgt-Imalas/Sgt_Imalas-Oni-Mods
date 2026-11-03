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
		public override Version Version => new Version(2, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethod = AccessTools.Method(typeof(SearchUtil), nameof(SearchUtil.CanonicalizePhrase));
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
		/// Skip SearchUtil.CanonicalizePhrase if the string is null or whitespace to prevent a crash in it.
		/// </summary>
		/// <param name="s"></param>
		/// <param name="__result"></param>
		/// <returns></returns>
		public static bool Prefix(string s, ref string __result)
		{
			__result = string.Empty;
			return !s.IsNullOrWhiteSpace();
		}
	}

}

using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace ItemDropPrevention
{
	public class Mod : UserMod2
	{
		public static Harmony Harmony;
		public override void OnLoad(Harmony harmony)
		{
			Harmony = harmony;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.RemoveCrashingIncompatibility(harmony, mods, "NoDrop");
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

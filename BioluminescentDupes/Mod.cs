using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace BioluminescentDupes
{
	public class Mod : UserMod2
	{
		public static Harmony Harmony;
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
			Harmony = harmony;
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

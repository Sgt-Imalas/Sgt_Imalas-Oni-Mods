using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;

namespace SkillsInfoScreen
{
	public class Mod : UserMod2
	{
		public static bool CleanHUDEnabled = false;
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			CleanHUDEnabled = mods.Any(mod => mod.staticID == "Aze.CleanHUD" && mod.IsEnabledForActiveDlc());
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;
using UtilLibs.SharedTweaks;

namespace AkisSnowThings
{
	public class Mod : UserMod2
	{
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			HarmonyInstance = harmony;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
			AttachmentPointTagNameFix.Register();
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

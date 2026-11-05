using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace ShapeProjector
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			SgtLogger.LogVersion(this, harmony);
			base.OnLoad(harmony);
			ModAssets.LoadAssets();
			LocalisationUtil.RegisterForLocalization(harmony, typeof(STRINGS));
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

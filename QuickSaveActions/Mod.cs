using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace QuickSaveActions
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);
			ModAssets.InitActions();
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
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

using HarmonyLib;
using KMod;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace BionicBoostersPlus
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			harmony.RegisterForLocalization(typeof(STRINGS), true);
			SgtLogger.LogVersion(this, harmony);
			if(!DlcManager.IsContentSubscribed(DlcManager.DLC3_ID))
			{
				SgtLogger.error("Bionic booster pack not owned!");
				return;
			}
			base.OnLoad(harmony);
			UtilLibs.SharedTweaks.SelectedRecipeQueueScreenSizeFix.Register();
			UtilLibs.SharedTweaks.SkillsWidgetBetterConnectionLines.Register();
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

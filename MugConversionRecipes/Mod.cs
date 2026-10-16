using HarmonyLib;
using KMod;
using System.Collections.Generic;
using UtilLibs;
using UtilLibs.SharedTweaks;

namespace MugConversionRecipes
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);

			SelectedRecipeQueueScreenSizeFix.Register();
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

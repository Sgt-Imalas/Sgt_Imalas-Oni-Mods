using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System.Collections.Generic;
using UtilLibs;

namespace UL_UniversalLyzer
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);

			new POptions().RegisterOptions(this, typeof(Config));
			base.OnLoad(harmony);
			ModAssets.InitializeOrUpdateLyzerPowerCosts();

			//GameTags.MaterialBuildingElements.Add(ModAssets.Tags.RadiationShielding);
			//GameTags.MaterialBuildingElements.Add(ModAssets.Tags.NeutroniumDust);

			SgtLogger.debuglog("Initialized");
			SgtLogger.LogVersion(this, harmony);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}
	}
}

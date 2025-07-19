using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing
{
	public class Mod : UserMod2
	{
		public static Mod Instance;
		public static Harmony HarmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			Instance = this;
			
			ModAssets.LoadAssets();
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));

			BuildingDatabase.RegisterBuildings();
			HarmonyInstance = harmony;

			base.OnLoad(harmony);

			SgtLogger.LogVersion(this, harmony);
			ConduitDisplayPortPatching.PatchAll(harmony);
			BuildingDatabase.RegisterAdditionalBuildingElements();
			AdditionalRecipes.RegisterTags();

			SharedTweaks.ResearchNotificationMessageFix.ExecutePatch(harmony); 
			SharedTweaks.ResearchScreenCollapseEntries.ExecutePatch(harmony);
			SharedTweaks.ElementConverterDescriptionImprovement.ExecutePatch(harmony);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
		}

		internal static void RegisterLocalizedDescription()
		{
			Instance.mod.title = Strings.Get("STRINGS.RONIVANSLEGACY_AIO_NAME");
			Instance.mod.description = Strings.Get("STRINGS.RONIVANSLEGACY_AIO_DESC");
		}
	}
}

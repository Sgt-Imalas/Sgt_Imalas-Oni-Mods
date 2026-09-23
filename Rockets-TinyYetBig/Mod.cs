using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Content.ModDb.ModIntegrations;
using Rockets_TinyYetBig.Patches;
using System.Collections.Generic;
using UtilLibs;
using UtilLibs.SharedTweaks;
using static Rockets_TinyYetBig.Patches.BugfixPatches;
using static Rockets_TinyYetBig.Patches.ModIntegration_Patches.Rocketry_Interior_WeightLimit;

namespace Rockets_TinyYetBig
{
	public class Mod : UserMod2
	{
		public static bool IsDev => Instance.mod.IsDev;
		public static Harmony harmonyInstance;

		public static Mod Instance;
		public override void OnLoad(Harmony harmony)
		{
			Instance = this;

			SgtLogger.l("RE.OnLoad");
			SgtLogger.LogVersion(this, harmony);
			harmonyInstance = harmony;
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			//ModuleConfigManager.Init();

			SgtLogger.log("Current Config Settings:");
			UtilMethods.ListAllPropertyValues(Config.Instance, (s) => s.Contains("System.Action"));

			base.OnLoad(harmony);
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.RadiationShieldingRocketConstructionMaterial);
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.NeutroniumAlloy);
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.CarbonFibreMaterial);

			ModAssets.LoadAssets();

			SgtLogger.debuglog("Initialized");
			///mod applies fix to rocketConduitports leaking
			PRegistry.PutData("Bugs.RocketConduitPorts", true);
			ElementUtilNamespace.SgtElementUtil.ExecuteElementEnumPatches(harmony);


			ResearchScreenBetterConnectionLines.Register();
			AttachmentPointTagNameFix.Register();
			TranslationFix.Register();
		}


		[HarmonyPatch(typeof(Clustercraft), nameof(Clustercraft.GetSpeed))]
		public class Clustercraft_GetSpeed_Patch
		{
			public static void Postfix(Clustercraft __instance, ref float __result)
			{
				if (IsDev)
					__result *= 100;
			}
		}

		static HashSet<string> HydroCarbonRockets = [
			"Noobs:Rocketry_Companion","TC-1000's:Hydrocarbon_Rocket_Engines"
			];
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			SgtLogger.l("On all mods loaded");
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
			bool FreeGridSpaceFixed = PRegistry.GetData<bool>("Bugs.FreeGridSpace");
			if (!FreeGridSpaceFixed)
			{
				PRegistry.PutData("Bugs.FreeGridSpace", true);
				harmony.Patch(AccessTools.Method(typeof(Grid), nameof(Grid.FreeGridSpace)), new HarmonyMethod(AccessTools.Method(typeof(Grid_FreeGridSpace_BugfixPatch), "Prefix")));
			}

			//only iterate once
			foreach (var mod in mods)
			{
				if (!mod.IsEnabledForActiveDlc())
					continue;

				if (HydroCarbonRockets.Contains(mod.staticID))
				{
					ModIntegration_Patches.Hydrocarbon_Rocket_Engines.ExecutePatch(harmony);
				}
				else if (mod.staticID == "BlueprintsV2")
				{
					BlueprintsV2.InitTypes();
				}
			}			
			RocketInteriorWeightLimitApi.TryInitialize();
		}
	}
}

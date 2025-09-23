using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Patches;
using System.Collections.Generic;
using System.Linq;
using UtilLibs;
using static Rockets_TinyYetBig.Patches.BugfixPatches;
using static Rockets_TinyYetBig.Patches.ModIntegration_Patches.Rocketry_Interior_WeightLimit;

namespace Rockets_TinyYetBig
{
	public class Mod : UserMod2
	{
		public static Harmony harmonyInstance;

		public static Mod Instance;
		public override void OnLoad(Harmony harmony)
		{
			Instance = this;

			SgtLogger.l("RE.OnLoad");
			harmonyInstance = harmony;
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));
			//ModuleConfigManager.Init();

			base.OnLoad(harmony);
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.RadiationShieldingRocketConstructionMaterial);
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.NeutroniumAlloy);

			ModAssets.LoadAssets();

			SgtLogger.debuglog("Initialized");
			SgtLogger.LogVersion(this, harmony);

			///mod applies fix to rocketConduitports leaking
			PRegistry.PutData("Bugs.RocketConduitPorts", true);

		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			SgtLogger.l("On all mods loaded");
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
			bool FreeGridSpaceFixed = PRegistry.GetData<bool>("Bugs.FreeGridSpace");


			BugfixPatches.AttemptOxidizerTaskBugfixPatch(harmony, FreeGridSpaceFixed);

			if (!FreeGridSpaceFixed)
			{
				PRegistry.PutData("Bugs.FreeGridSpace", true);
				harmony.Patch(AccessTools.Method(typeof(Grid), nameof(Grid.FreeGridSpace)), new HarmonyMethod(AccessTools.Method(typeof(Grid_FreeGridSpace_BugfixPatch), "Prefix")));
			}

			if (mods.Any(mod => mod.staticID == "TC-1000's:Hydrocarbon_Rocket_Engines" && mod.IsEnabledForActiveDlc()))
			{
				ModIntegration_Patches.Hydrocarbon_Rocket_Engines.ExecutePatch(harmony);
			}
			else
				SgtLogger.l("TC-1000's:Hydrocarbon_Rocket_Engines not found");

			RocketInteriorWeightLimitApi.TryInitialize();
		}
	}
}

using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Buildings;
using System;
using System.Collections.Generic;
using UtilLibs;
using static UtilLibs.RocketryUtils;
using static Rockets_TinyYetBig.STRINGS;
using static PeterHan.PLib.AVC.JsonURLVersionChecker;
using PeterHan.PLib.AVC;
using Rockets_TinyYetBig.Patches;
using Rockets_TinyYetBig._ModuleConfig;
using static Rockets_TinyYetBig.Patches.BugfixPatches;
using Klei;
using System.Runtime.CompilerServices;
using System.Linq;
using static Rockets_TinyYetBig.Patches.CompatibilityPatches.Rocketry_Interior_WeightLimit;
using System.Reflection;
using static Rockets_TinyYetBig.Patches.AnimationFixes;
using UnityEngine;

namespace Rockets_TinyYetBig
{
    public class Mod : UserMod2
    {
        public static Harmony harmonyInstance;


        public override void OnLoad(Harmony harmony)
        {
            SgtLogger.l("RE.OnLoad");
            harmonyInstance = harmony;
            PUtil.InitLibrary(false);            
            new POptions().RegisterOptions(this, typeof(Config));
            //ModuleConfigManager.Init();

            base.OnLoad(harmony);
            //GameTags.MaterialBuildingElements.Add(ModAssets.Tags.RadiationShielding);
            //GameTags.MaterialBuildingElements.Add(ModAssets.Tags.NeutroniumDust);

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

            bool FreeGridSpaceFixed = PRegistry.GetData<bool>("Bugs.FreeGridSpace");


            BugfixPatches.AttemptOxidizerTaskBugfixPatch(harmony, FreeGridSpaceFixed);

            if (!FreeGridSpaceFixed)
            {
                PRegistry.PutData("Bugs.FreeGridSpace", true);
                harmony.Patch(AccessTools.Method(typeof(Grid), nameof(Grid.FreeGridSpace)),new HarmonyMethod(AccessTools.Method(typeof(Grid_FreeGridSpace_BugfixPatch), "Prefix")));                
            }

            if (mods.Any(mod => mod.staticID == "TC-1000's:Hydrocarbon_Rocket_Engines" && mod.IsEnabledForActiveDlc()))
            {
                CompatibilityPatches.Hydrocarbon_Rocket_Engines.ExecutePatch(harmony);
            }
            else
                SgtLogger.l("TC-1000's:Hydrocarbon_Rocket_Engines not found");

            RocketInteriorWeightLimitApi.TryInitialize();
        }
    }
}

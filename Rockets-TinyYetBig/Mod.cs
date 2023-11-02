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

namespace Rockets_TinyYetBig
{
    public class Mod : UserMod2
    {
        public static Harmony haromy;
        public static Dictionary<int, string> Tooltips = new Dictionary<int, string>();
        public override void OnLoad(Harmony harmony)
        {
            haromy = harmony;
            PUtil.InitLibrary(false);
            
            new POptions().RegisterOptions(this, typeof(Config));
            //ModuleConfigManager.Init();

            base.OnLoad(harmony);
            CreateTooltipDictionary();

            //GameTags.MaterialBuildingElements.Add(ModAssets.Tags.RadiationShielding);
            //GameTags.MaterialBuildingElements.Add(ModAssets.Tags.NeutroniumDust);

            ModAssets.LoadAssets();

            SgtLogger.debuglog("Initialized");
            SgtLogger.LogVersion(this);
            new PVersionCheck().Register(this, new SteamVersionChecker());

            ///mod applies fix to rocketConduitports leaking
            PRegistry.PutData("Bugs.RocketConduitPorts", true);
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            CompatibilityNotifications.FlagLoggingPrevention(mods);
            BugfixPatches.AttemptPatch(harmony);

        }

        void CreateTooltipDictionary()
        {
            Tooltips.Add(0, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.ENGINES);
            Tooltips.Add(1, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.HABITATS);
            Tooltips.Add(2, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.NOSECONES);
            Tooltips.Add(3, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.DEPLOYABLES);
            Tooltips.Add(4, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.FUEL);
            Tooltips.Add(5, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.CARGO);
            Tooltips.Add(6, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.POWER);
            Tooltips.Add(7, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.PRODUCTION);
            Tooltips.Add(8, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.UTILITY);
            Tooltips.Add(-1, STRINGS.ROCKETBUILDMENUCATEGORIES.CATEGORYTOOLTIPS.UNCATEGORIZED);

            //engines = 0,
            //    habitats = 1,
            //nosecones = 2,
            //deployables = 3,
            //fuel = 4,
            //cargo = 5,
            //power = 6,
            //production = 7,
            //utility = 8,
            //uncategorized = -1
        }
    }
}

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

namespace Rockets_TinyYetBig
{
    public class Mod : UserMod2
    {
        public static Dictionary<int, string> Tooltips = new Dictionary<int, string>();
        public override void OnLoad(Harmony harmony)
        {
            PUtil.InitLibrary(false);
            
            new POptions().RegisterOptions(this, typeof(Config));
            base.OnLoad(harmony);
            CreateTooltipDictionary();

            //GameTags.MaterialBuildingElements.Add(ModAssets.Tags.RadiationShielding);
            //GameTags.MaterialBuildingElements.Add(ModAssets.Tags.NeutroniumDust);

            SgtLogger.debuglog("Rocketry Expanded - Initialized");
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);

            
        }

        void CreateTooltipDictionary()
        {
            Tooltips.Add(0, CATEGORYTOOLTIPS.ENGINES);
            Tooltips.Add(1, CATEGORYTOOLTIPS.HABITATS);
            Tooltips.Add(2, CATEGORYTOOLTIPS.NOSECONES);
            Tooltips.Add(3, CATEGORYTOOLTIPS.DEPLOYABLES);
            Tooltips.Add(4, CATEGORYTOOLTIPS.FUEL);
            Tooltips.Add(5, CATEGORYTOOLTIPS.CARGO);
            Tooltips.Add(6, CATEGORYTOOLTIPS.POWER);
            Tooltips.Add(7, CATEGORYTOOLTIPS.PRODUCTION);
            Tooltips.Add(8, CATEGORYTOOLTIPS.UTILITY);
            Tooltips.Add(-1, CATEGORYTOOLTIPS.UNCATEGORIZED);

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

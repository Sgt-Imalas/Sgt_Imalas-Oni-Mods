using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Buildings;
using System;
using System.Collections.Generic;
using UtilLibs;
using static UtilLibs.RocketryUtils;
using static Rockets_TinyYetBig.STRINGS

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

            Debug.Log("Rocketry Expanded - Initialized");
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);

            
        }

        void CreateTooltipDic()
        {
            Tooltips.Add

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

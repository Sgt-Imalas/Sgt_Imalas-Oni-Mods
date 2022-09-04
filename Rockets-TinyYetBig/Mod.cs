using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using Rockets_TinyYetBig.Buildings;
using System;
using System.Collections.Generic;
using UtilLibs;
using static UtilLibs.RocketryUtils;

namespace Rockets_TinyYetBig
{
    public class Mod : UserMod2
    {
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
    }
}

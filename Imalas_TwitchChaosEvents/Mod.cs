using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace Imalas_TwitchChaosEvents
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            SgtLogger.LogVersion(this);
            base.OnLoad(harmony);
            ModAssets.LoadAll();
        }
    }
}

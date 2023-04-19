using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace CustomGameSettingsModifier
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            ModAssets.LoadAssets();
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
        }
    }
}

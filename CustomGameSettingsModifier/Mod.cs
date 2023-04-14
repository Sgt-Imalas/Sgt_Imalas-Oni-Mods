using HarmonyLib;
using KMod;
using System;

namespace CustomGameSettingsModifier
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            ModAssets.LoadAssets();
            base.OnLoad(harmony);
        }
    }
}

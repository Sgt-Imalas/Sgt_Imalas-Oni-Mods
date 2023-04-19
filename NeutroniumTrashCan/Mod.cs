using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace NeutroniumTrashCan
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
        }
    }
}

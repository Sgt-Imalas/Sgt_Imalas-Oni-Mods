using HarmonyLib;
using KMod;
using System;
using UtilLibs;

namespace SGTIM_NotificationManager
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

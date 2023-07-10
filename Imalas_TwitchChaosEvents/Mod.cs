using HarmonyLib;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using UtilLibs;

namespace Imalas_TwitchChaosEvents
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Config));

            ModAssets.LoadAll();
            ModAssets.HotKeys.Register();
        }
    }
}

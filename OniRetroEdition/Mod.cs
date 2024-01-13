using HarmonyLib;
using KMod;
using OniRetroEdition.BuildingDefModification;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using UtilLibs;

namespace OniRetroEdition
{
    public class Mod : UserMod2
    {
        public static Harmony HarmonyInstance;
        public override void OnLoad(Harmony harmony)
        {
            HarmonyInstance = harmony;
            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Config));
            BuildingModifications.InitializeFolderPath();
            SkinsAdder.InitializeFolderPath();

            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
        }
    }
}

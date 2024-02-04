using HarmonyLib;
using KMod;
using OniRetroEdition.BuildingDefModification;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using UtilLibs;
using static StatusItem;

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


            var overlayBitsField = typeof(StatusItem).GetFieldSafe("overlayBitfieldMap", true);
            if (overlayBitsField != null && overlayBitsField.GetValue(null) is
                    IDictionary<HashedString, StatusItemOverlays> overlayBits)
                overlayBits.Add(OverlayModes.Sound.ID, StatusItemOverlays.None);
        }
    }
}

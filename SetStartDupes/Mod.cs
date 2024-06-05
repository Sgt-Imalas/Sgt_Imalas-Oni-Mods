using HarmonyLib;
using Klei;
using KMod;
using PeterHan.PLib.AVC;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.IO;
using UtilLibs;

namespace SetStartDupes
{
    public class Mod : UserMod2
    {
        public static Harmony harmonyInstance;
        public override void OnLoad(Harmony harmony)
        {
            harmonyInstance = harmony;
            ModApi.RegisteringJorge();

            ModAssets.LoadAssets();
            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Config));

            SgtLogger.debuglog("Initializing file paths..");
            ModAssets.DupeTemplatePath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config"),"DuplicantStatPresets"));
            ModAssets.DupeTearTemplatePath = FileSystem.Normalize(Path.Combine(ModAssets.DupeTemplatePath, "TearTravelers"));
            ModAssets.DupeGroupTemplatePath = FileSystem.Normalize(Path.Combine(ModAssets.DupeTemplatePath, "StartingLayoutPresets"));
            SgtLogger.debuglog(ModAssets.DupeTemplatePath,"Stat Preset Folder");
            SgtLogger.debuglog("Initializing folders..");
            try
            {
                System.IO.Directory.CreateDirectory(ModAssets.DupeTemplatePath);
                System.IO.Directory.CreateDirectory(ModAssets.DupeTearTemplatePath);
                System.IO.Directory.CreateDirectory(ModAssets.DupeGroupTemplatePath);
            }
            catch (Exception e)
            {
                SgtLogger.error("Could not create folder, Exception:\n" + e);
            }
            SgtLogger.log("Folders succesfully initialized");


            SgtLogger.log("Current Config Settings:");
            UtilMethods.ListAllPropertyValues(Config.Instance);

            SgtLogger.LogVersion(this, harmony);
            base.OnLoad(harmony);
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            CompatibilityNotifications.FlagLoggingPrevention(mods);
            CompatibilityNotifications.CheckAndAddIncompatibles("DGSM2", "Duplicant Stat Selector","DGSM - Duplicants Generation Settings Manager");
            CompatibilityNotifications.CheckAndAddIncompatibles("RePrint", "Duplicant Stat Selector","Reprint");
            ModAssets.RemoveCrashingIncompatibility(mods);
            //CheckAndAddIncompatibles(".Mod.WGSM", "WGSM");
        }
    }
}

using HarmonyLib;
using Klei;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.IO;
using UtilLibs;

namespace DupePrioPresetManager
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            ModAssets.LoadAssets();
            //PUtil.InitLibrary(false);
            //new POptions().RegisterOptions(this, typeof(ModConfig));

            SgtLogger.debuglog("Initializing file paths..");
            ModAssets.DupeTemplatePath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config/"), "DuplicantPriorityPresets/"));
            //SgtLogger.debuglog(ModAssets.DupeTemplatePath, "Priority Preset Folder");
            ModAssets.FoodTemplatePath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config/"), "DuplicantConsumablePresets/"));
            ModAssets.ScheduleTemplatePath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config/"), "DuplicantSchedulePresets/"));


            SgtLogger.debuglog("Initializing folders..");
            try
            {
                System.IO.Directory.CreateDirectory(ModAssets.DupeTemplatePath);
                System.IO.Directory.CreateDirectory(ModAssets.FoodTemplatePath);
                System.IO.Directory.CreateDirectory(ModAssets.ScheduleTemplatePath);
            }
            catch (Exception e)
            {
                SgtLogger.error("Could not create folder, Exception:\n" + e);
            }
            SgtLogger.log("Folders succesfully initialized");

            SgtLogger.LogVersion(this, harmony);
            base.OnLoad(harmony);
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
            CompatibilityNotifications.FlagLoggingPrevention(mods);

        }
    }
}

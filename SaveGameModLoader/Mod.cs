using HarmonyLib;
using Klei;
using KMod;
using SaveGameModLoader.FastTrack_VirtualScroll;
using SaveGameModLoader.ModFilter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UtilLibs;
using static SaveGameModLoader.AllPatches;
using Directory = System.IO.Directory;

namespace SaveGameModLoader
{
    public class Mod : UserMod2
    {
        public static Harmony harmonyInstance;
        public static UserMod2 ThisMod;
        public override void OnLoad(Harmony harmony)
        {
            harmonyInstance = harmony;
            ThisMod = this;
            var LegacyModPath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "[ModSync]StoredModConfigs/"));
            var LegacyModPacksPath = FileSystem.Normalize(Path.Combine(LegacyModPath, "[StandAloneModLists]/"));


            SgtLogger.debuglog("Initializing file paths..");
            ModAssets.ModPath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config/"), "[ModSync]StoredModConfigs/"));
            ModAssets.ModPacksPath = FileSystem.Normalize(Path.Combine(ModAssets.ModPath, "[StandAloneModLists]/"));
            ModAssets.ConfigPath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config/"), "MPM_Config.json"));

            SgtLogger.debuglog(ModAssets.ModPath);
            SgtLogger.debuglog(ModAssets.ModPacksPath);

            SgtLogger.debuglog("Initializing folders..");
            try
            {
                System.IO.Directory.CreateDirectory(ModAssets.ModPath);
                System.IO.Directory.CreateDirectory(ModAssets.ModPacksPath);
            }
            catch (Exception e)
            {
                SgtLogger.error("Could not create folders, Exception:\n" + e);
            }
            bool migrationSuccessful = true;

            if (Directory.Exists(LegacyModPath))
            {
                var files = new DirectoryInfo(LegacyModPath).GetFiles();
                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.CopyTo(Path.Combine(ModAssets.ModPath, file.Name), false);
                    }
                    catch (Exception e)
                    {
                        migrationSuccessful = false;
                        // SgtLogger.logError("Error while moving file from legacy folder, Error: " + e);
                    }
                }
                if (migrationSuccessful)
                    SgtLogger.l("Migrated save mod profiles to new directory");
            }
            else
                migrationSuccessful = false;
            if (Directory.Exists(LegacyModPacksPath))
            {
                var files = new DirectoryInfo(LegacyModPacksPath).GetFiles();
                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.CopyTo(Path.Combine(ModAssets.ModPacksPath, file.Name), false);
                    }
                    catch (Exception e)
                    {
                        migrationSuccessful = false;
                        // SgtLogger.logError("Error while moving file from legacy folder, Error: " + e);
                    }
                }
                if (migrationSuccessful)
                    SgtLogger.l("Migrated custom mod profiles to new directory");
            }
            else
                migrationSuccessful = false;

            if (migrationSuccessful)
            {
                //SgtLogger.l("deleting legacy directories");
                //Directory.Delete(LegacyModPath, true);
                SgtLogger.l("legacy directories can be deleted now");
            }



            SgtLogger.log("Folders succesfully initialized");

            //ModAssets.ModID = this.mod.label.defaultStaticID;
            SgtLogger.log("Retrieving Modlists");
            ModlistManager.Instance.GetAllStoredModlists();
            ModlistManager.Instance.GetAllModPacks();
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);


            ModAssets.SecureLog(harmony);

            harmony.UnpatchAll("Ony.OxygenNotIncluded.DebugConsole");
            harmony.UnpatchAll("OxygenNotIncluded.DebugConsole");
            harmony.UnpatchAll("DebugConsole");
            harmony.UnpatchAll("Ony.OxygenNotIncluded.DebugConsole.Loader");
            harmony.UnpatchAll("OxygenNotIncluded.DebugConsole.Loader");
            harmony.UnpatchAll("DebugConsole.Loader");
            harmony.UnpatchAll("Release_DLC1.Mod.DebugConsole");

            SgtLogger.log(harmony.Id, "HARMONYID");

            bool FastTrackActive = mods.Any(mod => mod.staticID.Contains("PeterHan.FastTrack") && mod.IsEnabledForActiveDlc());
            bool ModFilterActive = mods.Any(mod => mod.staticID.Contains("asquared31415.ModsFilter") && mod.IsEnabledForActiveDlc());
            ModAssets.FastTrackActive = FastTrackActive;
            ModAssets.ModsFilterActive = ModFilterActive;
            if (!FastTrackActive)
            {
                SgtLogger.l("Fast Track not active, executing virtual scroll patches");
                DragMe_OnBeginDrag_Patch.ExecutePatch(harmony);
                DragMe_OnEndDrag_Patch.ExecutePatch(harmony);
                ModsScreen_OnActivate_Patch.ExecutePatch(harmony);
                ModsScreen_BuildDisplay_Patch.ExecutePatch(harmony);
            }
            if (!ModFilterActive)
            {
                SgtLogger.l("Mod Filter not active, executing virtual scroll patches");
                FilterPatches.ModsScreen_OnActivate_SearchBar_Patch.ExecutePatch(harmony);
            }


            ModsScreen_BuildDisplay_Patch_Pin_Button.ExecutePatch(harmony);
            CompatibilityNotifications.FlagLoggingPrevention(mods);
        }
    }

}

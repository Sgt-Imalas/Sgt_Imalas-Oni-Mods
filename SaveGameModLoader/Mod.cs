using HarmonyLib;
using Klei;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using SaveGameModLoader.FastTrack_VirtualScroll;
using SaveGameModLoader.ModFilter;
using SaveGameModLoader.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UtilLibs;
using UtilLibs.ModSyncing;
using static SaveGameModLoader.AllPatches;
using Directory = System.IO.Directory;

namespace SaveGameModLoader
{
    public class Mod : UserMod2
    {
        public static Harmony harmonyInstance;
        public override void OnLoad(Harmony harmony)
        {

            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Config));

            SgtLogger.LogVersion(this, harmony);

            harmonyInstance = harmony;
            //ThisMod = this;
            var LegacyModPath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "[ModSync]StoredModConfigs"));
            var LegacyModPacksPath = FileSystem.Normalize(Path.Combine(LegacyModPath, "[StandAloneModLists]"));


            SgtLogger.debuglog("Initializing file paths..");
            ModAssets.ModPath = Config.Instance.UseCustomFolderPath ? Config.Instance.ModProfileFolder : FileSystem.Normalize(Path.Combine(Path.Combine(KMod.Manager.GetDirectory(), "config"), "[ModSync]StoredModConfigs"));
            ModAssets.ModPacksPath = FileSystem.Normalize(Path.Combine(ModAssets.ModPath, "[StandAloneModLists]"));
            ModAssets.ConfigPath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config"), "MPM_Config.json"));

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

            ModSyncUtils.RegisterModAsSyncMod(this.mod);
            ModAssets.ReadOrRegisterBrowserSetting();
            base.OnLoad(harmony);
            Steam_MakeMod.TryPatchingSteam(harmony);

        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);

            bool FastTrackActive = mods.Any(mod => mod.staticID.Contains("PeterHan.FastTrack") && mod.IsEnabledForActiveDlc());
            bool ModFilterActive = mods.Any(mod => mod.staticID.Contains("asquared31415.ModsFilter") && mod.IsEnabledForActiveDlc());

            CompatibilityNotifications.RemoveCrashingIncompatibility(harmony, mods, "ModManager");
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
            else 
                SgtLogger.l("Fast Track active, leaving virtual scroll to that");
            if (!ModFilterActive)
            {
                SgtLogger.l("Mod Filter not active, executing search bar patches");
                FilterPatches.ModsScreen_OnActivate_SearchBar_Patch.ExecutePatch(harmony);
            }
            else
                SgtLogger.l("Mod Filter active, leaving the search bar to that");

            ModsScreen_BuildDisplay_Patch_Pin_Button.ExecutePatch(harmony);
            CompatibilityNotifications.FlagLoggingPrevention(mods);
        }
    }

}

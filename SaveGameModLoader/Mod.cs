using HarmonyLib;
using Klei;
using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using UtilLibs;
using Directory = System.IO.Directory;

namespace SaveGameModLoader
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {

            var LegacyModPath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "[ModSync]StoredModConfigs/"));
            var LegacyModPacksPath = FileSystem.Normalize(Path.Combine(LegacyModPath, "[StandAloneModLists]/"));


            SgtLogger.debuglog("Initializing file paths..");
            ModAssets.ModPath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config/"), "[ModSync]StoredModConfigs/"));
            ModAssets.ModPacksPath = FileSystem.Normalize(Path.Combine(ModAssets.ModPath ,"[StandAloneModLists]/"));
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
            bool migrationSuccessful= true;

            if (Directory.Exists(LegacyModPath))
            {
                var files = new DirectoryInfo(LegacyModPath).GetFiles();
                foreach (FileInfo file in files)
                {
                    try
                    {
                        file.CopyTo(Path.Combine(ModAssets.ModPath, file.Name), true);
                    }
                    catch (Exception e)
                    {
                        migrationSuccessful = false;
                        SgtLogger.logError("Error while moving file from legacy folder, Error: " + e);
                    }
                }
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
                        file.CopyTo(Path.Combine(ModAssets.ModPacksPath,file.Name),true);
                    }
                    catch (Exception e)
                    {
                        migrationSuccessful = false;
                        SgtLogger.logError("Error while moving file from legacy folder, Error: " + e);
                    }
                }
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

            ModAssets.ModID = this.mod.label.id;
            SgtLogger.log("Retrieving Modlists");
            ModlistManager.Instance.GetAllStoredModlists();
            ModlistManager.Instance.GetAllModPacks();
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this);
        }
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);

            CompatibilityNotifications.FlagLoggingPrevention(mods);
            //ModlistManager.Instance.UpdateModDict();
        }
    }
    
}

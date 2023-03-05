using HarmonyLib;
using Klei;
using KMod;
using System;
using System.IO;
using UtilLibs;

namespace SaveGameModLoader
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            SgtLogger.debuglog("Initializing file paths..");
            ModAssets.ModPath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "[ModSync]StoredModConfigs/"));
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

            ModAssets.ModID = this.mod.label.id;
            //SgtLogger.log(ModAssets.ModID+"");
            ModlistManager.Instance.GetAllStoredModlists();
            base.OnLoad(harmony);
        }
    }
}

using HarmonyLib;
using Klei;
using KMod;
using System;
using System.IO;
using UtilLibs;
using UtilLibs.ModSyncing;

namespace ModProfileManager_Addon
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);

            ModAssets.ModPath =  FileSystem.Normalize(Path.Combine(Path.Combine(KMod.Manager.GetDirectory(), "config"), "[ModSync]StoredModConfigs"));
            ModAssets.ModPacksPath = FileSystem.Normalize(Path.Combine(ModAssets.ModPath, "[StandAloneModLists]"));

            System.IO.Directory.CreateDirectory(ModAssets.ModPath);
            System.IO.Directory.CreateDirectory(ModAssets.ModPacksPath);
            ModAssets.LoadAssets();

            ModSyncUtils.RegisterModAsSyncMod(this.mod);
        }
    }
}

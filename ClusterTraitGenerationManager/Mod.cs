using HarmonyLib;
using Klei;
using KMod;
using System;
using System.IO;
using UtilLibs;

namespace ClusterTraitGenerationManager
{
    public class Mod : UserMod2
    {
        public override void OnLoad(Harmony harmony)
        {
            base.OnLoad(harmony);
            ModAssets.LoadAssets(); 
            
            SgtLogger.debuglog("Initializing file paths..");
            ModAssets.CustomClusterTemplatesPath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config/"), "CustomClusterPresetTemplates/"));


            SgtLogger.debuglog("Initializing folders..");
            try
            {
                System.IO.Directory.CreateDirectory(ModAssets.CustomClusterTemplatesPath);
            }
            catch (Exception e)
            {
                SgtLogger.error("Could not create folder, Exception:\n" + e);
            }
            SgtLogger.log("Folders succesfully initialized");

            SgtLogger.LogVersion(this);
#if DEBUG
            Debug.LogError("Error THIS IS NOT RELEASE");
#endif
        }
    }
}

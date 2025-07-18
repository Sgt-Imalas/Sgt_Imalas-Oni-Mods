﻿using Database;
using HarmonyLib;
using Klei;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using SetStartDupes.CarePackageEditor;
using System;
using System.Collections.Generic;
using System.IO;
using UtilLibs;
using UtilLibs.ModVersionCheck;
using static Database.Personalities;

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
            ModAssets.DupeTemplatePath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DuplicantStatPresets"));
			ModAssets.ExtraCarePackageFileInfo = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DSS_ExtraCarePackages.json"));
			ModAssets.DisabledVanillaCarePackages = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DSS_DisabledVanillaPackages.json"));


			ModAssets.DupeTearTemplatePath = FileSystem.Normalize(Path.Combine(ModAssets.DupeTemplatePath, "TearTravelers"));
            ModAssets.DupeGroupTemplatePath = FileSystem.Normalize(Path.Combine(ModAssets.DupeTemplatePath, "StartingLayoutPresets"));
            SgtLogger.debuglog(ModAssets.DupeTemplatePath, "Stat Preset Folder");
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

            //var translationPatch = AccessTools.Method(this.GetType(), "PatchTranslationsRegularly");
            //var manualPatch = AccessTools.Method(this.GetType(), "DoManualPatching");
            //var assetsPatch = AccessTools.Method(this.GetType(), "AssetsPatchPrefix");


            //var personalitypatch = AccessTools.Method(this.GetType(), "UnlockDupesPostfix");
            //var personalityConstructor = AccessTools.Constructor(typeof(Personalities));

            //harmonyInstance.Patch(typeof(Assets), "OnPrefabInit", new(assetsPatch));
            //harmonyInstance.Patch(typeof(Localization), "Initialize", postfix: new(translationPatch));
            //harmonyInstance.Patch(typeof(Db), "Initialize", postfix: new(manualPatch));

            //harmonyInstance.Patch(personalityConstructor, postfix: new(personalitypatch));
            //.ManualTranslationPatch(harmony, typeof(STRINGS));
            //OnAssetPrefabPatch.InitiatePatch(harmony);
        }

        //public static void PatchTranslationsRegularly()
        //{
        //    LocalisationUtil.Translate(typeof(STRINGS), true);
        //}
        //public static void DoManualPatching()
        //{
        //    harmonyInstance.PatchAll();
        //    Patches.OnAssetPrefabPatch.ExecuteManualPatches();
        //}
        //public static void AssetsPatchPrefix(Assets __instance)
        //{
        //    InjectionMethods.AddSpriteToAssets(__instance, ModAssets.UnlockIcon);
        //}        
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
            base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
            CompatibilityNotifications.CheckAndAddIncompatibles("RePrint", "Duplicant Stat Selector", "Reprint");
            ModAssets.RemoveCrashingIncompatibility(mods);
        }
    }
}

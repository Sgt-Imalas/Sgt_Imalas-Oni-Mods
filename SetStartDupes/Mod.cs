using Database;
using HarmonyLib;
using Klei;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using SetStartDupes.CarePackageEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UtilLibs;
using UtilLibs.ModVersionCheck;
using static Database.Personalities;

namespace SetStartDupes
{
    public class Mod : UserMod2
    {
        public static Harmony harmonyInstance;

		public static bool SharingIsCaringInstalled { get; internal set; }

		public override void OnLoad(Harmony harmony)
		{
			SgtLogger.debuglog("Initializing file paths..");
			ModAssets.ExtraCarePackageFileInfo = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DSS_ExtraCarePackages.json"));
			ModAssets.DisabledVanillaCarePackages = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DSS_DisabledVanillaPackages.json"));

			ModAssets.DupeTemplatePath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DuplicantStatPresets"));
			ModAssets.DupeTearTemplatePath = FileSystem.Normalize(Path.Combine(ModAssets.DupeTemplatePath, "TearTravelers"));
			ModAssets.DupeGroupTemplatePath = FileSystem.Normalize(Path.Combine(ModAssets.DupeTemplatePath, "StartingLayoutPresets"));
			//SgtLogger.debuglog(ModAssets.DupeTemplatePath, "Stat Preset Folder");
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

			harmonyInstance = harmony;
            ModApi.RegisteringJorge();

            ModAssets.LoadAssets();
            PUtil.InitLibrary(false);
            new POptions().RegisterOptions(this, typeof(Config));

            SgtLogger.log("Current Config Settings:");
            UtilMethods.ListAllPropertyValues(Config.Instance);
            SgtLogger.LogVersion(this, harmony);
            base.OnLoad(harmony);
        }
   
        public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
        {
			SharingIsCaringInstalled = mods.Any(mod => mod.staticID == "SharingIsCaring" && mod.IsEnabledForActiveDlc());


			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
            CompatibilityNotifications.CheckAndAddIncompatibles("RePrint", "Duplicant Stat Selector", "Reprint");
            ModAssets.RemoveCrashingIncompatibility(mods);
        }
    }
}

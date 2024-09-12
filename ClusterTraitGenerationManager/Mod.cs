using HarmonyLib;
using Klei;
using KMod;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using System.Collections.Generic;
using System.IO;
using UtilLibs;

namespace ClusterTraitGenerationManager
{
	public class Mod : UserMod2
	{
		public static Harmony harmonyInstance;
		public override void OnLoad(Harmony harmony)
		{
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));

			harmonyInstance = harmony;
			base.OnLoad(harmony);
			ModAssets.LoadAssets();

			SgtLogger.debuglog("Initializing file paths..");
			ModAssets.CustomClusterTemplatesPath = FileSystem.Normalize(Path.Combine(Path.Combine(Manager.GetDirectory(), "config"), "CustomClusterPresetTemplates"));


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
			SgtLogger.LogVersion(this, harmony);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			//SgtLogger.l("CGM_OnAllModsLoaded");
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.RemoveCrashingIncompatibility(harmony, mods, "CGSMMerged");
			CompatibilityNotifications.RemoveCrashingIncompatibility(harmony, mods, "WGSM");
		}
	}
}

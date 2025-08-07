using HarmonyLib;
using Klei;
using KMod;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			//SgtLogger.debuglog(ModAssets.DupeTemplatePath, "Priority Preset Folder");
			ModAssets.DupeTemplatePath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DuplicantPriorityPresets"));
			ModAssets.FoodTemplatePath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DuplicantConsumablePresets"));
			ModAssets.ScheduleTemplatePath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "DuplicantSchedulePresets"));
			ModAssets.ResearchTemplatePath = FileSystem.Normalize(Path.Combine(Manager.GetDirectory(), "config", "ResearchQueuePresets"));

			SgtLogger.debuglog("Initializing folders..");
			try
			{
				System.IO.Directory.CreateDirectory(ModAssets.DupeTemplatePath);
				System.IO.Directory.CreateDirectory(ModAssets.FoodTemplatePath);
				System.IO.Directory.CreateDirectory(ModAssets.ScheduleTemplatePath);
				System.IO.Directory.CreateDirectory(ModAssets.ResearchTemplatePath);
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
			if(mods.Any(mod => mod.IsEnabledForActiveDlc() && mod.staticID == "PeterHan.ResearchQueue"))
			{
				SgtLogger.l("ResearchQueue is enabled, activating presets.");
				ResearchScreenPatches.ExecuteAll(harmony);
			}


			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);

		}

	}
}

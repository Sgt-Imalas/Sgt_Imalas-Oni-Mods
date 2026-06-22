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
using TUNING;
using UnityEngine;
using UtilLibs;
using static Database.Personalities;
using static TUNING.DUPLICANTSTATS;

namespace SetStartDupes
{
	public class Mod : UserMod2
	{
		public static Harmony harmonyInstance;

		public static bool SharingIsCaringInstalled { get; internal set; }

		public override void OnLoad(Harmony harmony)
		{
			ModAssets.AddExtraTraitTooltipKey("PrefersWarmer", "STRINGS.DSS_PREFERSWARMER_WARNING");

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
			ModApi.RegisterHiddenDupes();

			ModAssets.LoadAssets();
			PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this, typeof(Config));

			SgtLogger.log("Current Config Settings:");
			UtilMethods.ListAllPropertyValues(Config.Instance);
			SgtLogger.LogVersion(this, harmony);
			base.OnLoad(harmony);

			string workaholic_ID = "Workaholic";
			if (!DUPLICANTSTATS.NEEDTRAITS.Any(traitval => traitval.id == workaholic_ID))
				DUPLICANTSTATS.NEEDTRAITS.Add(new TraitVal
				{
					id = workaholic_ID,
					rarity = RARITY_COMMON
				});
			FixSkinny();
		}

		public static void FixSkinny()
		{
			///prevents default insulation of 0.002 to dip below 0, causing sim crashes;
			DUPLICANTSTATS.STANDARD.Temperature.Conductivity_Barrier_Modification.SKINNY = -0.001f;
		}

		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			SharingIsCaringInstalled = mods.Any(mod => mod.staticID == "SharingIsCaring" && mod.IsEnabledForActiveDlc());
			base.OnAllModsLoaded(harmony, mods);
			CompatibilityNotifications.FixBrokenTimeout(harmony);
			CompatibilityNotifications.FlagLoggingPrevention(mods);
			CompatibilityNotifications.CheckAndAddIncompatibles("RePrint", "Duplicant Stat Selector", "Reprint");
			ModAssets.RemoveCrashingIncompatibility(mods);

			ApplyXPMods();
		}
		void ApplyXPMods()
		{
			int levelCapMod = Config.Instance.LevelingCapIncrease;
			if(levelCapMod != 20)
			{
				SgtLogger.l("Setting XP level cap for attributes to " + levelCapMod);
				TUNING.DUPLICANTSTATS.ATTRIBUTE_LEVELING.MAX_GAINED_ATTRIBUTE_LEVEL = levelCapMod;
				///adjust scaling target to keep xp/level roughly the same. (some slight alterations due to rounding)
				int defaultTargetCycle = 400;
				float defaultLevelCap = 20f;
				float defaultLevelPower = 1.7f;
				TUNING.DUPLICANTSTATS.ATTRIBUTE_LEVELING.TARGET_MAX_LEVEL_CYCLE = Mathf.RoundToInt(defaultTargetCycle * Mathf.Pow(levelCapMod / defaultLevelCap, defaultLevelPower));
			}

			float multiplier = Config.Instance.LevelingXPMultiplier_Attributes;
			if(!Mathf.Approximately(1, multiplier))
			{
				SgtLogger.l("Setting XP multiplier for attributes to " + multiplier);
				TUNING.DUPLICANTSTATS.ATTRIBUTE_LEVELING.FULL_EXPERIENCE *= multiplier;
				TUNING.DUPLICANTSTATS.ATTRIBUTE_LEVELING.ALL_DAY_EXPERIENCE *= multiplier;
				TUNING.DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE *= multiplier;
				TUNING.DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE *= multiplier;
				TUNING.DUPLICANTSTATS.ATTRIBUTE_LEVELING.BARELY_EVER_EXPERIENCE *= multiplier;
			}
			multiplier = Config.Instance.LevelingXPMultiplier_Skills;
			if (!Mathf.Approximately(1, multiplier))
			{
				SgtLogger.l("Setting XP multiplier for skill level to " + multiplier);
				TUNING.SKILLS.FULL_EXPERIENCE *= multiplier;
				TUNING.SKILLS.ALL_DAY_EXPERIENCE *= multiplier;
				TUNING.SKILLS.MOST_DAY_EXPERIENCE *= multiplier;
				TUNING.SKILLS.PART_DAY_EXPERIENCE *= multiplier;
				TUNING.SKILLS.BARELY_EVER_EXPERIENCE *= multiplier;
			}
		}
	}
}

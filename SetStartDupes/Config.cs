using Newtonsoft.Json;
using PeterHan.PLib.Options;
using SetStartDupes.CarePackageEditor.UI;
using System;
using System.Collections.Generic;

namespace SetStartDupes
{
	[Serializable]
	[RestartRequired]
	[ConfigFile(SharedConfigLocation: true)]
	[ModInfo("Duplicant Stat Selector")]
	class Config : SingletonOptions<Config>
	{


		[Option("STRINGS.UI.DSS_OPTIONS.DUPLICANTSTARTAMOUNT.NAME", "STRINGS.UI.DSS_OPTIONS.DUPLICANTSTARTAMOUNT.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.A_GAMESTART")]
		[Limit(1, 100)]
		[JsonProperty]
		public int DuplicantStartAmount { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.STARTUPRESOURCES.NAME", "STRINGS.UI.DSS_OPTIONS.STARTUPRESOURCES.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.A_GAMESTART")]
		[JsonProperty]
		public bool StartupResources { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.SUPPORTEDDAYS.NAME", "STRINGS.UI.DSS_OPTIONS.SUPPORTEDDAYS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.A_GAMESTART")]
		[JsonProperty]
		[Limit(0, 10)]
		public int SupportedDays { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGEEDITOR.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGEEDITOR.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonIgnore]
		public System.Action<object> Button_OpenCarepackageEditor => CarePackageEditor_MainScreen.ShowCarePackageEditor;

		[Option("STRINGS.UI.DSS_OPTIONS.MORECAREPACKAGES.NAME", "STRINGS.UI.DSS_OPTIONS.MORECAREPACKAGES.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public bool AddAdditionalCarePackages { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.MODIFYDURINGGAME_MINIONS.NAME", "STRINGS.UI.DSS_OPTIONS.MODIFYDURINGGAME_MINIONS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public bool ModifyDuringGame { get; set; } = true; 

		[Option("STRINGS.UI.DSS_OPTIONS.MODIFYDURINGGAME_CAREPACKAGES.NAME", "STRINGS.UI.DSS_OPTIONS.MODIFYDURINGGAME_CAREPACKAGES.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public bool ModifyDuringGame_CarePackage { get; set; } = true;

		[Option("STRINGS.UI.DSS_OPTIONS.REROLLDURINGGAME_MINIONS.NAME", "STRINGS.UI.DSS_OPTIONS.REROLLDURINGGAME_MINIONS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public bool RerollDuringGame { get; set; }
		[Option("STRINGS.UI.DSS_OPTIONS.REROLLDURINGGAME_CAREPACKAGES.NAME", "STRINGS.UI.DSS_OPTIONS.REROLLDURINGGAME_CAREPACKAGES.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public bool RerollDuringGame_CarePackage { get; set; } = true;

		[Option("STRINGS.UI.DSS_OPTIONS.SORTEDCAREPACKAGES.NAME", "STRINGS.UI.DSS_OPTIONS.SORTEDCAREPACKAGES.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		public bool CarePackageEntriesSorted { get; set; } = true;	

		[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGEMULTIPLIER.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGEMULTIPLIER.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		[Limit(0.1f, 10f)]
		public float CarePackageMultiplier { get; set; } = 1f;

		[Option("STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIME.NAME", "STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIME.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public float PrintingPodRechargeTime { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIMEFIRST.NAME", "STRINGS.UI.DSS_OPTIONS.PRINTINGPODRECHARGETIMEFIRST.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public float PrintingPodRechargeTimeFirst { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.PAUSEONREADYTOPRING.NAME", "STRINGS.UI.DSS_OPTIONS.PAUSEONREADYTOPRING.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public bool PauseOnReadyToPrint { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.SORTEDPRINTINGPOD.NAME", "STRINGS.UI.DSS_OPTIONS.SORTEDPRINTINGPOD.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public bool SortedPrintingPod { get; set; } = false;

		[Option("STRINGS.UI.DSS_OPTIONS.OVERRIDEPRINTERDUPECOUNT.NAME", "STRINGS.UI.DSS_OPTIONS.OVERRIDEPRINTERDUPECOUNT.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		[Limit(0, 10)]
		public int OverridePrinterDupeCount { get; set; } = 0;
		[Option("STRINGS.UI.DSS_OPTIONS.OVERRIDEPRINTERCAREPACKAGECOUNT.NAME", "STRINGS.UI.DSS_OPTIONS.OVERRIDEPRINTERCAREPACKAGECOUNT.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		[Limit(0, 10)]
		public int OverridePrinterCarePackageCount { get; set; } = 0;

		[Option("STRINGS.UI.DSS_OPTIONS.FORCEPRINTINGMODEL.NAME", "STRINGS.UI.DSS_OPTIONS.FORCEPRINTINGMODEL.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public MinionModelOverride OverridePrintingPodModels { get; set; } = MinionModelOverride.none;

		public Tag[] GetViablePrinterModels()
		{
			if (!Game.IsDlcActiveForCurrentSave(DlcManager.DLC3_ID))
			{
				return null;
			}

			switch (OverridePrintingPodModels)
			{
				case MinionModelOverride.normal:
					return [GameTags.Minions.Models.Standard];
				case MinionModelOverride.bionic:
					return [GameTags.Minions.Models.Bionic];
				case MinionModelOverride.none:
				default:
					return GameTags.Minions.Models.AllModels;
			}
		}

		internal static bool CarePackageOnlyConditionFulfilled()
		{
			var config = Config.Instance;

			if(config.CarePackagesOnlyMode == CarePackageLimiterMode.None)
				return false;

			if(config.CarePackagesOnlyMode == CarePackageLimiterMode.DupeCount)
				return Components.LiveMinionIdentities.Count >= config.CarePackagesOnlyDupeCap;

			int bedCount = Components.NormalBeds.GlobalCount;

			if (config.CarePackagesOnlyMode == CarePackageLimiterMode.NumberOfBeds)
				return Components.LiveMinionIdentities.Count >= bedCount;

			int scheduleCount = ScheduleManager.Instance?.GetSchedules()?.Count ?? 1;

			if (config.CarePackagesOnlyMode == CarePackageLimiterMode.NumberOfBeds_SharingIsCaring)
			{
				if(Mod.SharingIsCaringInstalled)
					return Components.LiveMinionIdentities.Count >= bedCount * scheduleCount;
				else
					return Components.LiveMinionIdentities.Count >= bedCount;
			}
			if(config.CarePackagesOnlyMode == CarePackageLimiterMode.PrinterCheckbox)
			{
				return PrintingPodCheckboxToggle.PrintOnlyCarePackages;
			}
			return false;
		}

		public enum MinionModelOverride
		{
			[Option("STRINGS.UI.CHARACTERCONTAINER_ALL_MODELS")]
			none = 0,
			[Option("STRINGS.DUPLICANTS.MODEL.STANDARD.NAME")]
			normal = 1,
			[Option("STRINGS.DUPLICANTS.MODEL.BIONIC.NAME")]
			bionic = 2,
		}

		[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		public CarePackageLimiterMode CarePackagesOnlyMode { get; set; } = CarePackageLimiterMode.None;

		public enum CarePackageLimiterMode
		{
			[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.NONE.NAME","STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.NONE.TOOLTIP")]
			None = 0,
			[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.DUPECOUNT.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.DUPECOUNT.TOOLTIP")]
			DupeCount = 1,
			[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.BEDCOUNT.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.BEDCOUNT.TOOLTIP")]
			NumberOfBeds = 2,
			[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.BEDCOUNT_SIC.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.BEDCOUNT_SIC.TOOLTIP")]
			NumberOfBeds_SharingIsCaring = 3,
			[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.PRINTERCHECKBOX.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.PRINTERCHECKBOX.TOOLTIP")]
			PrinterCheckbox = 4,
		}


		[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYDUPECAP.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYDUPECAP.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		[Limit(1, 200)]
		public int CarePackagesOnlyDupeCap { get; set; }


		[Option("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYPACKAGECAP.NAME", "STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLYPACKAGECAP.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.B_PRINTINGPOD")]
		[JsonProperty]
		[Limit(1, 5)]
		public int CarePackagesOnlyPackageCount { get; set; }


		[Option("STRINGS.UI.DSS_OPTIONS.LIVEDUPESKINCHANGE.NAME", "STRINGS.UI.DSS_OPTIONS.LIVEDUPESKINCHANGE.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.D_SKINSETTINGS")]
		[JsonProperty]
		public bool LiveDupeSkins { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.SKINSDOREACTS.NAME", "STRINGS.UI.DSS_OPTIONS.SKINSDOREACTS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.D_SKINSETTINGS")]
		[JsonProperty]
		public bool SkinsDoReactions { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.LIVEDUPESTATCHANGE.NAME", "STRINGS.UI.DSS_OPTIONS.LIVEDUPESTATCHANGE.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.E_UTIL")]
		[JsonProperty]
		public bool DuplicityDupeEditor { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.REROLLCRYOPODANDJORGE.NAME", "STRINGS.UI.DSS_OPTIONS.REROLLCRYOPODANDJORGE.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.E_UTIL")]
		[JsonProperty]
		public bool JorgeAndCryopodDupes { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.HERMITSKIN.NAME", "STRINGS.UI.DSS_OPTIONS.HERMITSKIN.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.D_SKINSETTINGS")]
		[JsonProperty]
		public bool HermitSkin { get; set; }


		[Option("STRINGS.UI.DSS_OPTIONS.ADDANDREMOVE.NAME", "STRINGS.UI.DSS_OPTIONS.ADDANDREMOVE.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
		[JsonProperty]
		public bool AddAndRemoveTraitsAndInterests { get; set; }
		[Option("STRINGS.UI.DSS_OPTIONS.DIRECTATTRIBUTEEDITING.NAME", "STRINGS.UI.DSS_OPTIONS.DIRECTATTRIBUTEEDITING.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
		[JsonProperty]
		public bool DirectAttributeEditing { get; set; } = false;

		[Option("STRINGS.UI.DSS_OPTIONS.ADDVACCILATORTRAITS.NAME", "STRINGS.UI.DSS_OPTIONS.ADDVACCILATORTRAITS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
		[JsonProperty]
		public bool AddVaccilatorTraits { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.NORMALTRAITSONBIONICS.NAME", "STRINGS.UI.DSS_OPTIONS.NORMALTRAITSONBIONICS.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
		[JsonProperty]
		public bool BionicNormalTraits { get; set; } = false;

		[Option("STRINGS.UI.DSS_OPTIONS.INTERESTPOINTSBALANCING.NAME", "STRINGS.UI.DSS_OPTIONS.INTERESTPOINTSBALANCING.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
		[JsonProperty]
		public bool BalanceAddRemove { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.NOJOYREACTION.NAME", "STRINGS.UI.DSS_OPTIONS.NOJOYREACTION.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
		[JsonProperty]
		public bool NoJoyReactions { get; set; }

		[Option("STRINGS.UI.DSS_OPTIONS.NOSTRESSREACTION.NAME", "STRINGS.UI.DSS_OPTIONS.NOSTRESSREACTION.TOOLTIP", "STRINGS.UI.DSS_OPTIONS.CATEGORIES.C_EXTRAS")]
		[JsonProperty]
		public bool NoStressReactions { get; set; }


		public Config()
		{
			DuplicantStartAmount = 3;
			PrintingPodRechargeTime = 3;
			PrintingPodRechargeTimeFirst = 2.5f;
			ModifyDuringGame = true;
			RerollDuringGame = true;
			PauseOnReadyToPrint = false;

			StartupResources = false;
			SupportedDays = 5;

			CarePackagesOnlyDupeCap = 16;
			CarePackagesOnlyPackageCount = 3;

			SkinsDoReactions = true;
			JorgeAndCryopodDupes = true;
			HermitSkin = true;

			AddAndRemoveTraitsAndInterests = true;
			AddVaccilatorTraits = false;
			BalanceAddRemove = true;
			NoJoyReactions = false;
			NoStressReactions = false;

			AddAdditionalCarePackages = true;
			LiveDupeSkins = false;
			DuplicityDupeEditor = false;
		}
	}
}

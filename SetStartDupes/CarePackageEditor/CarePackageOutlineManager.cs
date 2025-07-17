using Database;
using Klei.CustomSettings;
using Newtonsoft.Json;
using SetStartDupes.CarePackageEditor.UI;
using SetStartDupes.Patches;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS;

namespace SetStartDupes.CarePackageEditor
{
	public class CarePackageOutlineManager
	{
		public static Dictionary<string, List<CarePackageOutline>> VanillaCarePackagesByDlc = null;
		private static List<CarePackageOutline> VanillaCarePackages = null;

		private static List<CarePackageOutline> ExtraCarePackages = null;
		private static VanillaCarePackageInformation DisabledVanillaPackages = null;



		public static void FetchVanillaCarePackages()
		{
			ImmigrationPatch.GeneratingFrontendList = true;

			var managerCarrier = Util.NewGameObject(null, "ImmigrationCarrier");
			var immigrationInstance = managerCarrier.AddComponent<DummyImmigration>();
			VanillaCarePackagesByDlc = new();

			if (!DlcManager.IsExpansion1Active())
			{
				SgtLogger.l("fetching base game vanilla care packages");
				immigrationInstance.ConfigureBaseGameCarePackages();
				VanillaCarePackagesByDlc[DlcManager.VANILLA_ID] = immigrationInstance.carePackages.Select(package => new CarePackageOutline(package)).ToList();
				SgtLogger.l(VanillaCarePackagesByDlc[DlcManager.VANILLA_ID].Count() + " care packages in vanilla");
			}
			else
			{
				SgtLogger.l("fetching spaced out vanilla care packages");
				immigrationInstance.ConfigureMultiWorldCarePackages();
				VanillaCarePackagesByDlc[DlcManager.EXPANSION1_ID] = immigrationInstance.carePackages.Select(package => new CarePackageOutline(package)).ToList();
				SgtLogger.l(VanillaCarePackagesByDlc[DlcManager.EXPANSION1_ID].Count() + " care packages in SO");
			}

			immigrationInstance.SetupDLCCarePackages();

			foreach (var dlcPackages in immigrationInstance.carePackagesByDlc)
			{
				var dlcID = dlcPackages.Key;
				if (DlcManager.IsContentSubscribed(dlcID))
				{
					VanillaCarePackagesByDlc[dlcPackages.Key] = dlcPackages.Value.Select(package => new CarePackageOutline(package)).ToList();
					SgtLogger.l(VanillaCarePackagesByDlc[dlcPackages.Key].Count() + " care packages in " + dlcPackages.Key);
				}
			}

			Immigration.DestroyInstance();
			UnityEngine.Object.Destroy(managerCarrier);

			VanillaCarePackages = new();

			foreach (var entry in VanillaCarePackagesByDlc.Values)
			{
				VanillaCarePackages.AddRange(entry);
			}
			ImmigrationPatch.GeneratingFrontendList = false;
		}
		public static List<CarePackageOutline> GetVanillaCarePackageOutlines()
		{
			if (VanillaCarePackagesByDlc == null || VanillaCarePackages == null)
			{
				VanillaCarePackagesByDlc = new();
				FetchVanillaCarePackages();
			}
			return VanillaCarePackages;
		}
		public static void RemoveDisabledVanillaPackages(List<CarePackageInfo> currentList)
		{
			if (DisabledVanillaPackages == null)
				LoadDisabledVanillaCarePackagesFile();

			for (int i = currentList.Count - 1; i >= 0; i--)
			{
				var entry = currentList[i];
				if (DisabledVanillaPackages.CarePackageDisabled(entry))
				{
					SgtLogger.l($"removing vanilla care package that adds {entry.quantity}x {entry.id}")
					currentList.RemoveAt(i);
				}
			}
		}



		public static List<CarePackageInfo> GetAllAdditionalCarePackages()
		{
			if (ExtraCarePackages == null)
				CarePackageOutlineManager.LoadCarePackageFile();
			var result = new List<CarePackageInfo>();
			foreach (var entry in ExtraCarePackages)
			{
				if (entry.TryMakeCarePackageInfo(out var info))
				{
					SgtLogger.l($"adding extra carepackage: {entry.Amount} x {info.id}");
					result.Add(info);
				}

			}
			return result;
		}
		public static List<CarePackageOutline> GetExtraCarePackageOutlines()
		{
			if (ExtraCarePackages == null)
				CarePackageOutlineManager.LoadCarePackageFile();
			return ExtraCarePackages;
		}

		internal static void LoadCarePackageFile()
		{
			var file = new FileInfo(ModAssets.ExtraCarePackageFileInfo);
			if (file.Exists && IO_Utils.ReadFromFile<List<CarePackageOutline>>(file, out var outlines, converterSettings: new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto }))
			{
				SgtLogger.l("loaded care package file with " + outlines.Count + " entries");
				ExtraCarePackages = outlines;
			}
			else
			{
				SgtLogger.l("no care package file found or failed to load existing one, creating new one with DSS default care packages");
				ResetExtraCarePackages();
			}
			foreach (var entry in ExtraCarePackages)
			{
				entry.RefreshInfo();
			}
			SgtLogger.l("loaded " + ExtraCarePackages.Count + " care packages");
		}
		internal static void LoadDisabledVanillaCarePackagesFile()
		{
			var file = new FileInfo(ModAssets.DisabledVanillaCarePackages);
			if (file.Exists && IO_Utils.ReadFromFile<VanillaCarePackageInformation>(file, out var disabledVanillaPackageList, converterSettings: new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto }))
			{
				SgtLogger.l("loaded vanilla care package file with " + disabledVanillaPackageList.GetCount() + " entries");
				DisabledVanillaPackages = disabledVanillaPackageList;
			}
			else
			{
				SgtLogger.l("no care package file found or failed to load existing one, creating new empty one");
				DisabledVanillaPackages = new();
			}
			DisabledVanillaPackages.Initialize();
		}


		public static void ResetExtraCarePackages()
		{
			ExtraCarePackages = new();
			ExtraCarePackages.Clear();
			AddExtraCarePackages();
			SaveCarePackagesToFile();
			DisabledVanillaPackages.ClearAll();
			SaveDisabledVanillaPackagesToFile();
		}
		public static void AddExtraCarePackages()
		{
			///Base Game:


			///missing seeds:
			//Sporechid
			ExtraCarePackages.Add(new CarePackageOutline(EvilFlowerConfig.SEED_ID, 1).CycleCondition(95).DiscoverCondition());
			//Buddy Bud
			ExtraCarePackages.Add(new CarePackageOutline(BulbPlantConfig.SEED_ID, 1).CycleCondition(36).DiscoverCondition());
			//Nosh Bean
			ExtraCarePackages.Add(new CarePackageOutline(BeanPlantConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition());
			//Sleet Wheat
			ExtraCarePackages.Add(new CarePackageOutline(ColdWheatConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition());
			//Waterweed
			ExtraCarePackages.Add(new CarePackageOutline(SeaLettuceConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition());
			//GasGrass
			ExtraCarePackages.Add(new CarePackageOutline(GasGrassConfig.SEED_ID, 3).CycleCondition(96).DiscoverCondition());

			///missing minerals:
			ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Granite, 1000).CycleCondition(24).DiscoverCondition());
			ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Obsidian, 1000).CycleCondition(24).DiscoverCondition());
			//Abyssalite
			ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Katairite, 1000).CycleCondition(48).DiscoverCondition());
			///missing ores+metals
			ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.IronOre, 2000).CycleCondition(12).DiscoverCondition());
			ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Wolframite, 1000).CycleCondition(48).DiscoverCondition());
			ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.Tungsten, 200).CycleCondition(48).DiscoverCondition());


			///Spaced Out:

			//uranium ore
			ExtraCarePackages.Add(CarePackageOutline.ElementCarePackage(SimHashes.UraniumOre, 100).CycleCondition(48).DiscoverCondition().DlcRequired(DlcManager.EXPANSION1_ID));
			//Bog Bucket
			ExtraCarePackages.Add(new CarePackageOutline(SwampHarvestPlantConfig.SEED_ID, 3).CycleCondition(24).DiscoverCondition().DlcRequired(DlcManager.EXPANSION1_ID));
			//Tranquil Toes
			ExtraCarePackages.Add(new CarePackageOutline(ToePlantConfig.SEED_ID, 3).CycleCondition(48).DiscoverCondition().DlcRequired(DlcManager.EXPANSION1_ID));
			//Saturn Critter Trap
			ExtraCarePackages.Add(new CarePackageOutline("CritterTrapPlantSeed", 1).CycleCondition(96).DiscoverCondition().DlcRequired(DlcManager.EXPANSION1_ID));


			///missing critters
			//Beetiny
			ExtraCarePackages.Add(new CarePackageOutline(BabyBeeConfig.ID, 1).DlcRequired(DlcManager.EXPANSION1_ID) //discover either on uranium ore or beetiny, but only after cycle 24
				.CycleCondition(24).DiscoverCondition()
				.OR()
				.CycleCondition(24).DiscoverElementCondition(SimHashes.UraniumOre));

			///FP:

			//Carved Lumen Quartz
			ExtraCarePackages.Add(new CarePackageOutline(PinkRockCarvedConfig.ID, 1).CycleCondition(48).DiscoverCondition().DlcRequired(DlcManager.DLC2_ID));

		}

		internal static void TryDeleteOutline(CarePackageOutline targetOutline)
		{
			DialogUtil.CreateConfirmDialogFrontend(global::STRINGS.UI.FRONTEND.LOADSCREEN.DELETEBUTTON, string.Format(global::STRINGS.UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, targetOutline.GetDescriptionString()), null, () => DeleteOutline(targetOutline), null, () => { });
		}
		private static void DeleteOutline(CarePackageOutline outline)
		{
			ExtraCarePackages.Remove(outline);
			SaveCarePackagesToFile();
			CarePackageEditor_MainScreen.Instance?.RemoveOutlineEntry(outline);
		}
		public static void SaveCarePackagesToFile()
		{
			IO_Utils.WriteToFile(ExtraCarePackages, ModAssets.ExtraCarePackageFileInfo, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
		}
		public static void SaveDisabledVanillaPackagesToFile()
		{
			IO_Utils.WriteToFile(DisabledVanillaPackages, ModAssets.DisabledVanillaCarePackages, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.Auto });
		}
		public static void ToggleVanillaOutlineEnabled(CarePackageOutline vanillaOutline)
		{
			if(DisabledVanillaPackages == null)
				LoadDisabledVanillaCarePackagesFile();
			DisabledVanillaPackages.ToggleVanillaCarePackage(vanillaOutline);
			SaveDisabledVanillaPackagesToFile();
		}
		public static bool IsVanillaCarePackageEnabled(CarePackageOutline vanillaOutline)
		{
			if (DisabledVanillaPackages == null)
				LoadDisabledVanillaCarePackagesFile();
			return !DisabledVanillaPackages.CarePackageDisabled(vanillaOutline);
		}

		internal static void TrySelectOutline(CarePackageOutline targetOutline)
		{
			CarePackageEditor_MainScreen.Instance?.SelectOutline(targetOutline);
		}

		internal static void AddNewCarePackage(string itemID)
		{
			var newOutline = new CarePackageOutline(itemID);
			ExtraCarePackages.Add(newOutline);
			SaveCarePackagesToFile();
			CarePackageEditor_MainScreen.Instance?.AddOrGetCarePackageOutlineUIEntry(newOutline);
			CarePackageEditor_MainScreen.Instance?.SortEntryList();
			CarePackageEditor_MainScreen.Instance?.SelectOutline(newOutline);
		}

		internal static void HandleRepositioning(int oldIndex, int newIndex)
		{
			SgtLogger.l("Moving Care package entry from " + oldIndex + " to " + newIndex);
			Debug.Assert(oldIndex >= 0, "item index was <0");
			Debug.Assert(oldIndex < ExtraCarePackages.Count, "item index was larger than list size");
			var item = ExtraCarePackages[oldIndex];
			ExtraCarePackages.RemoveAt(oldIndex);
			ExtraCarePackages.Insert(newIndex, item);
			SaveCarePackagesToFile();
		}
	}
}

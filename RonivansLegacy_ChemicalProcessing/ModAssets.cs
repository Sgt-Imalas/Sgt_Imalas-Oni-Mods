using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing
{
	internal class ModAssets
	{
		public static List<Storage.StoredItemModifier> AllStorageMods => GetAllStorageMods();

		public static List<Storage.StoredItemModifier> GetAllStorageMods()
		{
			return [
			Storage.StoredItemModifier.Hide,
			Storage.StoredItemModifier.Preserve,
			Storage.StoredItemModifier.Insulate,
			Storage.StoredItemModifier.Seal
			];
		}


		public static ModHashes OnBuildingFacadeChanged = new ModHashes("RonivanAIO_OnBuildingFacadeChanged");

		public static void AddMachineAttachmentPort(GameObject go)
		{
			go.AddOrGet<BuildingAttachPoint>().points = [new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), Tags.AIO_StackableMachine, null)];
		}

		public class Tags
		{
			///group tag for metal sands
			public static Tag RandomSand = TagManager.Create("ChemicalProcessing_RandomSand");

			///group tag for guidance units
			public static Tag MineralProcessing_GuidanceUnit = TagManager.Create("MineralProcessing_GuidanceUnit");
			///group tag for drillbits
			public static Tag MineralProcessing_Drillbit = TagManager.Create("MineralProcessing_Drillbit");

			///Prevents free material cheesing from drill by destroying the drillbit on cancellation of the recipe
			public static Tag RandomRecipeIngredient_DestroyOnCancel = TagManager.Create("RandomRecipeIngredient_DestroyOnCancel");

			///material tag for steel substitute materials
			public static Tag AIO_HardenedAlloy = TagManager.Create("AIO_HardenedAlloy");

			///material tag for inert gases used as a carrier gas for plasma
			public static Tag AIO_CarrierGas = TagManager.Create("AIO_CarrierGas");

			///sulphuric acid as a buildable material, currently unused
			public static Tag AIO_SulphuricAcidBuildable = TagManager.Create("AIO_SulphuricAcidBuildable");

			///Element can be consumed by the RadEmitter
			public static Tag AIO_RadEmitterInputMaterial = TagManager.Create("AIO_RadEmitterInputMaterial");

			///allows stacking the large reformer machines and crude/raw gas refineries
			public static Tag AIO_StackableMachine = TagManager.Create("AIO_StackableMachine");
		}
		public static GameObject BuildingEditorWindowPrefab;

		public static void LoadAssets()
		{
			AssetBundle bundle = AssetUtils.LoadAssetBundle("ronivan_aio", platformSpecific: true);

			BuildingEditorWindowPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/BuildingEditor.prefab");

			SgtLogger.Assert(BuildingEditorWindowPrefab, "BuildingEditorWindowPrefab");


			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(BuildingEditorWindowPrefab);

		}
		public static List<Tag> GetNonLiquifiableSolids()
		{
			return STORAGEFILTERS.NOT_EDIBLE_SOLIDS.Where(item => item != GameTags.Liquifiable).ToList();
		}
	}
}

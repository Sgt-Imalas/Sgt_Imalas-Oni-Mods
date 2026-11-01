using HarmonyLib;
using PeterHan.PLib.Core;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class EntityTemplates_Patches
	{
		//attempt at generic recipe generation for expeller press...

		//[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendPlantToFertilizable), [typeof(GameObject), typeof(PlantElementAbsorber.ConsumeInfo[])])]
		//public class EntityTemplates_ExtendPlantToFertilizable_Patch
		//{
		//	public static void Postfix(GameObject template, PlantElementAbsorber.ConsumeInfo[] fertilizers) => CollectAbsorbtionInfo(template, fertilizers);
		//}

		//[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendPlantToIrrigated), [typeof(GameObject), typeof(PlantElementAbsorber.ConsumeInfo[])])]
		//public class EntityTemplates_ExtendPlantToIrrigated_Patch
		//{
		//	public static void Postfix(GameObject template, PlantElementAbsorber.ConsumeInfo[] consume_info) => CollectAbsorbtionInfo(template, consume_info);
		//}
		//static void CollectAbsorbtionInfo(GameObject go, PlantElementAbsorber.ConsumeInfo[] consumeInfos)
		//{
		//	if (go == null || !go.TryGetComponent<Crop>(out var crop))
		//		return;
		//	Tag CropId = crop.cropId;

		//	foreach (var info in consumeInfos)
		//	{
		//		AdditionalRecipes.AddOrGetPlantConsumptionInfo(CropId, info.tag, info.massConsumptionRate);
		//	}
		//}

		/// <summary>
		/// allow all "atmosphere" plants to also grow in nitrogen
		/// </summary>
		[HarmonyPatch]
		public class EntityTemplates_ExtendEntityToBasicPlant_Patch
		{
			private static MethodBase _target;
			[HarmonyTargetMethod]
			public static MethodBase TargetMethod() => _target;

			[HarmonyPrepare]
			public static bool Prepare()
			{
				_target = typeof(EntityTemplates).GetLatestOverloadPatchTarget(nameof(EntityTemplates.ExtendEntityToBasicPlant), true);
				if (_target == null)
					SgtLogger.error("ExtendEntityToBasicPlant not found!");
				return _target != null;
			}

			public static void Prefix(GameObject template, Crop.CropVal harvest, ref SimHashes[] safe_elements, bool can_tinker)
			{
				CodexDatabase.GenerateFertilizationInfo(template, ref safe_elements, harvest, can_tinker);
			}
		}
	}
}

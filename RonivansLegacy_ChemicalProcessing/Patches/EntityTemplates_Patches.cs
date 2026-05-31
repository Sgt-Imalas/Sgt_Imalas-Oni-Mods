using HarmonyLib;
using PeterHan.PLib.Core;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities;
using RonivansLegacy_ChemicalProcessing.Content.Defs.Entities.CodexInfoDummies;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
			[HarmonyTargetMethod]
			static MethodBase GetTarget()
			{
				return typeof(EntityTemplates).GetOverloadWithMostArguments(nameof(EntityTemplates.ExtendEntityToBasicPlant), true);
			}

			public static void Prefix(GameObject template, ref SimHashes[] safe_elements, string crop_id, bool can_tinker)
			{
				CodexDatabase.GenerateFertilizationInfo(template, ref safe_elements, crop_id, can_tinker);
			}
		}
	}
}

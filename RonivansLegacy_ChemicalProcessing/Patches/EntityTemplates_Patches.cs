using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
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
		[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBasicPlant))]
		public class EntityTemplates_ExtendEntityToBasicPlant_Patch
		{
			public static void Prefix(EntityTemplates __instance, ref SimHashes[] safe_elements)
			{
				if (safe_elements == null)
					return;

				if (safe_elements.Contains(SimHashes.Oxygen) && safe_elements.Contains(SimHashes.CarbonDioxide))
					safe_elements = safe_elements.Append(ModElements.Nitrogen_Gas);
			}
		}
	}
}

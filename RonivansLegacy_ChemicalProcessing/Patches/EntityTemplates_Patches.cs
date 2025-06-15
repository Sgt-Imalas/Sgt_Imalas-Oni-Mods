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

		[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendPlantToFertilizable), [typeof(GameObject), typeof(PlantElementAbsorber.ConsumeInfo[])])]
		public class EntityTemplates_ExtendPlantToFertilizable_Patch
		{
			public static void Postfix(GameObject template, PlantElementAbsorber.ConsumeInfo[] fertilizers) => CollectAbsorbtionInfo(template, fertilizers);
		}

		[HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendPlantToIrrigated), [typeof(GameObject), typeof(PlantElementAbsorber.ConsumeInfo[])])]
		public class EntityTemplates_ExtendPlantToIrrigated_Patch
		{
			public static void Postfix(GameObject template, PlantElementAbsorber.ConsumeInfo[] consume_info) => CollectAbsorbtionInfo(template, consume_info);
		}
		static void CollectAbsorbtionInfo(GameObject go, PlantElementAbsorber.ConsumeInfo[] consumeInfos)
		{
			if (go == null || !go.TryGetComponent<Crop>(out var crop))
				return;
			Tag CropId = crop.cropId;

			foreach (var info in consumeInfos)
			{
				AdditionalRecipes.AddOrGetPlantConsumptionInfo(CropId, info.tag, info.massConsumptionRate);
			}
		}
	}
}

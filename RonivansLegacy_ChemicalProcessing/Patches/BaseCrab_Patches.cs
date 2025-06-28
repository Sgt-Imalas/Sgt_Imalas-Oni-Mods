using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class BaseCrab_Patches
	{
		public static void RegisterSlagDiet(
				float caloriesPerKg,
				string diseaseId,
				float diseasePerKgProduced,
				ref List<Diet.Info> __result)
		{
			__result.Add(
				new Diet.Info(
					[ModElements.Slag_Solid.Tag],
					ModElements.AmmoniumSalt_Solid.Tag,
					caloriesPerKg,
					0.75f,
					diseaseId,
					diseasePerKgProduced));
		}
		public static void RegisterBiomassDiet(
				float caloriesPerKg,
				string diseaseId,
				float diseasePerKgProduced,
				ref List<Diet.Info> __result)
		{
			__result.Add(
				new Diet.Info(
					[ModElements.BioMass_Solid.Tag],
					SimHashes.Sand.CreateTag(),
					caloriesPerKg,
					0.75f,
					diseaseId,
					diseasePerKgProduced));
		}



		[HarmonyPatch(typeof(BaseCrabConfig), nameof(BaseCrabConfig.BasicDiet))]
		public class BaseCrabConfig_BasicDiet_Patch
		{
			public static void Postfix(float caloriesPerKg, string diseaseId, float diseasePerKgProduced, ref List<Diet.Info> __result)
			{
				if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				{
					RegisterSlagDiet(caloriesPerKg, diseaseId, diseasePerKgProduced, ref __result);
				}
				if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
				{
					RegisterBiomassDiet(caloriesPerKg, diseaseId, diseasePerKgProduced, ref __result);
				}
			}
		}
		[HarmonyPatch(typeof(BaseCrabConfig), nameof(BaseCrabConfig.DietWithSlime))]
		public class BaseCrabConfig_DietWithSlime_Patch
		{
			public static void Postfix(float caloriesPerKg, string diseaseId, float diseasePerKgProduced, ref List<Diet.Info> __result)
			{
				if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				{
					RegisterSlagDiet(caloriesPerKg, diseaseId, diseasePerKgProduced, ref __result);
				}
				if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
				{
					RegisterBiomassDiet(caloriesPerKg, diseaseId, diseasePerKgProduced, ref __result);
				}
			}
		}
	}
}

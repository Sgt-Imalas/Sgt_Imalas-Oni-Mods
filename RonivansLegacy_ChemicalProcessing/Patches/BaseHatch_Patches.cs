using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ResearchTypes;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class BaseHatch_Patches
    {

        [HarmonyPatch(typeof(BaseHatchConfig), nameof(BaseHatchConfig.VeggieDiet))]
        public class BaseHatchConfig_VeggieDiet_Patch
        {
            public static void Postfix(
				Tag poopTag, float caloriesPerKg, float producedConversionRate, string diseaseId, float diseasePerKgProduced,
				ref List<Diet.Info> __result)
			{
				//--[ Chloroschist Diet ]-----------------------------------------------------
				if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				{
					__result.Add(CritterDietsInfo.AddToList([HatchVeggieConfig.ID],SourceModInfo.ChemicalProcessing_IO, new Diet.Info([ModElements.Chloroschist_Solid.Tag], SimHashes.BleachStone.CreateTag(), caloriesPerKg, 0.25f, diseaseId, diseasePerKgProduced)));
					__result.Add(CritterDietsInfo.AddToList([HatchVeggieConfig.ID], SourceModInfo.ChemicalProcessing_IO, new Diet.Info([ModElements.Slag_Solid.Tag], ModElements.AmmoniumSalt_Solid.Tag, caloriesPerKg, 0.9f, diseaseId, diseasePerKgProduced)));
					__result.Add(CritterDietsInfo.AddToList([HatchVeggieConfig.ID], SourceModInfo.ChemicalProcessing_IO, new Diet.Info([SimHashes.CrushedRock.CreateTag()], poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced)));
				}
				if (Config.Instance.ChemicalProcessing_BioChemistry_Enabled)
				{
					__result.Add(CritterDietsInfo.AddToList([HatchVeggieConfig.ID], SourceModInfo.ChemicalProcessing_BioChemistry, new Diet.Info([ModElements.BioMass_Solid.Tag], SimHashes.Carbon.CreateTag(), caloriesPerKg, 1f, diseaseId, diseasePerKgProduced)));
				}
			}
		}


		[HarmonyPatch(typeof(BaseHatchConfig), nameof(BaseHatchConfig.HardRockDiet))]
		public class BaseHatchConfig_HardRockDiet_Patch
		{
			public static void Postfix(
				float caloriesPerKg,
				string diseaseId,
				float diseasePerKgProduced,
				ref List<Diet.Info> __result)
			{
				if(Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
					__result.Add(CritterDietsInfo.AddToList([HatchHardConfig.ID], SourceModInfo.ChemicalProcessing_IO, new Diet.Info([SimHashes.Salt.CreateTag()], ModElements.Borax_Solid.Tag, caloriesPerKg, 0.25f, diseaseId, diseasePerKgProduced)));
			}
		}

		[HarmonyPatch(typeof(BaseHatchConfig), nameof(BaseHatchConfig.MetalDiet))]
		public class BaseHatchConfig_MetalDiet_Patch
		{
			public static void Postfix(
				Tag poopTag,
				float caloriesPerKg, 
				float producedConversionRate, 
				string diseaseId,
				float diseasePerKgProduced,
				ref List<Diet.Info> __result)
			{
				if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				{
					string hatchTarget = (poopTag == GameTags.Metal) ? HatchMetalConfig.ID : HatchHardConfig.ID;
					//__result.Add(
						CritterDietsInfo.AddToList([hatchTarget], SourceModInfo.ChemicalProcessing_IO, new Diet.Info( new HashSet<Tag>([ModElements.Argentite_Solid.Tag]), (poopTag == GameTags.Metal) ? ModElements.Silver_Solid.Tag : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced))
					//	)
						;
					
					///added via starting metal tag now
					//__result.Add(
						CritterDietsInfo.AddToList([hatchTarget], SourceModInfo.ChemicalProcessing_IO, new Diet.Info( new HashSet<Tag>([ModElements.Aurichalcite_Solid.Tag]), (poopTag == GameTags.Metal) ? ModElements.Zinc_Solid.Tag : poopTag, caloriesPerKg, producedConversionRate, diseaseId, diseasePerKgProduced))
						//)
						;
				}
			}
		}
    }
}

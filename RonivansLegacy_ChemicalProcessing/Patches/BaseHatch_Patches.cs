using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class BaseHatch_Patches
    {

        [HarmonyPatch(typeof(BaseHatchConfig), nameof(BaseHatchConfig.VeggieDiet))]
        public class BaseHatchConfig_VeggieDiet_Patch
        {
            public static void Postfix(
				float caloriesPerKg,
				string diseaseId,
				float diseasePerKgProduced,
				ref List<Diet.Info> __result)
			{
				//--[ Chloroschist Diet ]-----------------------------------------------------
				__result.Add(new Diet.Info([ModElements.Chloroschist_Solid.Tag], SimHashes.BleachStone.CreateTag(), caloriesPerKg, 0.25f, diseaseId, diseasePerKgProduced));
				__result.Add(new Diet.Info([ModElements.Slag_Solid.Tag], ModElements.AmmoniumSalt_Solid.Tag, caloriesPerKg, 0.9f, diseaseId, diseasePerKgProduced));
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
				__result.Add(new Diet.Info([SimHashes.Salt.CreateTag()], ModElements.Borax_Solid.Tag, caloriesPerKg, 0.5f, diseaseId, diseasePerKgProduced));
			}
		}
    }
}

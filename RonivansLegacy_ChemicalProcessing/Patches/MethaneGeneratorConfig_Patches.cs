using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class MethaneGeneratorConfig_Patches
	{
		[HarmonyPatch(typeof(MethaneGeneratorConfig), nameof(MethaneGeneratorConfig.DoPostConfigureComplete))]
		public class MethaneGeneratorConfig_DoPostConfigureComplete_Patch
		{
			public static void Postfix(GameObject go)
			{
				go.GetComponent<EnergyGenerator>().formula.inputs = [new EnergyGenerator.InputItem(GameTags.CombustibleGas, 0.09f, 0.9f)];

				var dispenser = go.AddOrGet<ConduitDispenser>();
				dispenser.elementFilter = dispenser.elementFilter.Append(SimHashes.Propane);
			}
		}
	}
}

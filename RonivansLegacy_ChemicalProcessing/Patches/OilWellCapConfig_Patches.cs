using Dupes_Industrial_Overhaul.Chemical_Processing.Buildings;
using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	class OilWellCapConfig_Patches
	{
		/// <summary>
		/// instead of making the oil well cap a custom building; modify the existing oil well cap
		/// </summary>
		[HarmonyPatch(typeof(OilWellCapConfig), nameof(OilWellCapConfig.ConfigureBuildingTemplate))]
		public class OilWellCapConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				if (Config.Instance.ChemicalProcessing_IndustrialOverhaul_Enabled)
				{
					///any sort of water allowed
					ConduitConsumer waterConsumer = go.GetComponent<ConduitConsumer>();
					waterConsumer.capacityTag = GameTags.AnyWater;

					///Produce raw natural gas instead of regular natural gas
					OilWellCap cap = go.GetComponent<OilWellCap>();
					cap.gasElement = ModElements.RawNaturalGas_Gas;
					cap.gasTemperature = 393.15f;
					cap.addGasRate = 0.12f;
					cap.maxGasPressure = 80f;
					cap.releaseGasRate = 80f / OilWellCapConfig.PRESSURE_RELEASE_TIME;

					///adjust the conversion output amount
					ElementConverter converter = go.GetComponent<ElementConverter>();
					converter.consumedElements = [new ElementConverter.ConsumedElement(GameTags.AnyWater, 1f)];
					converter.outputElements = [new ElementConverter.OutputElement(3.4f, SimHashes.CrudeOil, 363.15f, false, true, outputElementOffsetx: 2f, outputElementOffsety: 1.5f, diseaseWeight: 0f)];

					///grab storage for oil and gas and seal it
					Storage standardStorage = go.GetComponent<Storage>();
					standardStorage.SetDefaultStoredItemModifiers(ModAssets.AllStorageMods);

					///===> Methane Output <==============================================================
					PipedConduitDispenser GasDispenser = go.AddComponent<PipedConduitDispenser>();
					GasDispenser.elementFilter = [ModElements.RawNaturalGas_Gas];
					GasDispenser.alwaysDispense = true;
					GasDispenser.SkipSetOperational = true;
					GasDispenser.AssignPort(Custom_OilWellCapConfig.GasOutputPort);

					PipedOptionalExhaust GasExhaust = go.AddComponent<PipedOptionalExhaust>();
					GasExhaust.dispenser = GasDispenser;
					GasExhaust.elementTag = ModElements.RawNaturalGas_Gas.Tag;
					GasExhaust.capacity = 80f;

					var storageLimiter = go.AddOrGet<ElementThresholdOperational>();
					storageLimiter.Threshold = 80f;
					storageLimiter.ThresholdTag = ModElements.RawNaturalGas_Gas.Tag;


					///===> Crude Oil Output <============================================================
					PipedConduitDispenser LiquidDispenser = go.AddComponent<PipedConduitDispenser>();
					LiquidDispenser.elementFilter = [SimHashes.CrudeOil];
					LiquidDispenser.AssignPort(Custom_OilWellCapConfig.LiquidOutputPort);
					LiquidDispenser.alwaysDispense = true;
					LiquidDispenser.SkipSetOperational = true;

					PipedOptionalExhaust LiquidExhaust = go.AddComponent<PipedOptionalExhaust>();
					LiquidExhaust.dispenser = LiquidDispenser;
					LiquidExhaust.elementTag = SimHashes.CrudeOil.CreateTag();
					LiquidExhaust.capacity = 10f;

					var storageLimiter2 = go.AddOrGet<ElementThresholdOperational>();
					storageLimiter2.Threshold = 10f;
					storageLimiter2.ThresholdTag = SimHashes.CrudeOil.CreateTag();

					Custom_OilWellCapConfig.AttachPorts(go);
				}
			}
		}
	}
}

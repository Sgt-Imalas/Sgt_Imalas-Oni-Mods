using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace Dupes_Industrial_Overhaul.Chemical_Processing.Buildings
{
	//==== [ CHEMICAL: ENDOTHERMIC UNIT CONFIG ] =================================================================
	public class Chemical_EndothermicUnitConfig : IBuildingConfig
	{
		//--[ Base Information ]-----------------------------------------------
		public static string ID = "Chemical_EndothermicUnit";
		

		//--[ Building Definitions ]-------------------------------------------
		public override BuildingDef CreateBuildingDef()
		{
			EffectorValues tier = NOISE_POLLUTION.NOISY.TIER0;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 2, 2, "endothermic_mixer_kanim", 100, 30f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 800f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, tier);
			buildingDef.RequiresPowerInput = true;
			buildingDef.Floodable = false;
			buildingDef.EnergyConsumptionWhenActive = 50f;
			buildingDef.ExhaustKilowattsWhenActive = -64f;
			buildingDef.SelfHeatKilowattsWhenActive = -236f;
			buildingDef.OverheatTemperature = 998.15f;
			buildingDef.ThermalConductivity = 10000f;
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.AudioCategory = "Metal";
			buildingDef.InputConduitType = ConduitType.Liquid;
			buildingDef.UtilityInputOffset = new CellOffset(0, 0);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
			SoundUtils.CopySoundsToAnim("endothermic_mixer_kanim", "liquidconditioner_kanim");
			return buildingDef;
		}

		//--[ Building Operation Definitions ]---------------------------------
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddOrGet<MassiveHeatSink>();
			go.AddOrGet<MinimumOperatingTemperature>().minimumTemperature = 255.15f;
			go.AddOrGet<LoopingSounds>();

			Storage standardStorage = go.AddOrGet<Storage>();
			standardStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			standardStorage.showCapacityStatusItem = true;
			standardStorage.showCapacityAsMainStatus = true;
			standardStorage.showDescriptor = true;

			ConduitConsumer waterInput = go.AddOrGet<ConduitConsumer>();
			waterInput.conduitType = ConduitType.Liquid;
			waterInput.consumptionRate = 10f;
			waterInput.capacityTag = SimHashes.Water.CreateTag();
			waterInput.forceAlwaysSatisfied = true;
			waterInput.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ManualDeliveryKG nitrateManualDelivery = go.AddComponent<ManualDeliveryKG>();
			nitrateManualDelivery.SetStorage(standardStorage);
			nitrateManualDelivery.RequestedItemTag = ModElements.AmmoniumSalt_Solid.Tag;
			nitrateManualDelivery.capacity = 200f;
			nitrateManualDelivery.refillMass = 50f;
			nitrateManualDelivery.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;

			//-----[ Element Converter Section ]---------------------------------
			ElementConverter endothermicReaction = go.AddComponent<ElementConverter>();
			endothermicReaction.consumedElements = [
				new ElementConverter.ConsumedElement(SimHashes.Water.CreateTag(), 0.89f),
				new ElementConverter.ConsumedElement(ModElements.AmmoniumSalt_Solid.Tag, 0.11f)];
			endothermicReaction.outputElements = [
				new ElementConverter.OutputElement(1f, ModElements.AmmoniumWater_Liquid, 255.15f, true, true, 0f, 0.5f, 0f, 0xff, 0)];
			//-------------------------------------------------------------------

			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.elementFilter = [ModElements.AmmoniumWater_Liquid];
			dispenser.alwaysDispense = true;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGet<LogicOperationalController>();
			go.AddOrGetDef<PoweredActiveController.Def>();
			go.GetComponent<KPrefabID>().prefabSpawnFn += delegate (GameObject game_object)
			{
				HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(game_object);
				StructureTemperaturePayload payload = GameComps.StructureTemperatures.GetPayload(handle);
				int cell = Grid.PosToCell(game_object);
				payload.OverrideExtents(new Extents(cell, Chemical_EndothermicUnitConfig.overrideOffsets));
				GameComps.StructureTemperatures.SetPayload(handle, ref payload);
			};
		}
		private static readonly CellOffset[] overrideOffsets = [new CellOffset(-1, -1), new CellOffset(1, -1), new CellOffset(-1, 1), new CellOffset(1, 1)];
	}

	
}

using BionicBoostersPlus.Content.ModDb;
using BionicBoostersPlus.Content.Scripts;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace BionicBoostersPlus.Content.Defs.Buildings
{
	internal class Mk3BoosterMakerConfig : IBuildingConfig
	{
		public const string INTERACT_MK3_BOOSTERMAKER = "anim_interacts_mk3_boostermaker_kanim";

		public static readonly string ID = "BBP_Mk3BoosterMaker";
		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
				ID,
				5, 3,
				"bbp_mk3_boostermaker_kanim",
				30, 60f,
				TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4,
				TUNING.MATERIALS.REFINED_METALS,
				2400f,
				BuildLocationRule.OnFloor,
				TUNING.BUILDINGS.DECOR.PENALTY.TIER2,
				NOISE_POLLUTION.NOISY.TIER6);

			buildingDef.RequiresPowerInput = true;
			buildingDef.EnergyConsumptionWhenActive = 480f;
			buildingDef.SelfHeatKilowattsWhenActive = 8f;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.PowerInputOffset = new CellOffset(0, 0);
			buildingDef.RequiredSkillPerkID = BB_SkillPerks.BB_CanCraftOverclocks.Id;
			if (DlcManager.IsExpansion1Active())
			{
				buildingDef.UseHighEnergyParticleInputPort = true;
				buildingDef.HighEnergyParticleInputOffset = new CellOffset(-2, 1);

				buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
				{
					LogicPorts.Port.OutputPort((HashedString) "HEP_STORAGE", new CellOffset(-2, 1), global::STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE, global::STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE)
				};
			}

			return buildingDef;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			var prefab = go.GetComponent<KPrefabID>();
			prefab.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

			go.AddOrGet<DropAllWorkable>();
			go.AddOrGet<Prioritizable>();
			ComplexFabricatorOverrideWorkSymbol fabricator = go.AddOrGet<ComplexFabricatorOverrideWorkSymbol>();
			fabricator.heatedTemperature = 318.15f;
			go.AddOrGet<FabricatorIngredientStatusManager>();
			go.AddOrGet<CopyBuildingSettings>();
			go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
			go.AddOrGet<ComplexFabricatorWorkable>().overrideAnims = [Assets.GetAnim(INTERACT_MK3_BOOSTERMAKER)];
			Prioritizable.AddRef(go);
			BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
			if (DlcManager.IsExpansion1Active())
			{
				HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
				energyParticleStorage.capacity = 200f;
				energyParticleStorage.autoStore = true;
				energyParticleStorage.PORT_ID = "HEP_STORAGE";
				energyParticleStorage.showCapacityStatusItem = true;
				go.AddOrGet<HEPStorageMeterHandler>();
			}
			SymbolOverrideControllerUtil.AddToPrefab(go);

		}
	}
}

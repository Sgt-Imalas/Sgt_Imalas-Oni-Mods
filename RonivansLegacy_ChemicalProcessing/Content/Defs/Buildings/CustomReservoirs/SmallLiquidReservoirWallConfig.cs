using PeterHan.PLib.Options;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomReservoirs
{
	class SmallLiquidReservoirWallConfig : IBuildingConfig, IHasConfigurableStorageCapacity
	{
		static float StorageCapacity = 2500 * 0.6f; //visually 60% of small liquid reservoir; 1500
		public float GetStorageCapacity() => StorageCapacity;
		public void SetStorageCapacity(float mass) => StorageCapacity = mass;

		public static string ID = "WallLiquidTank";
		public string KANIM = "wall_liquid_tank_kanim";
		public int Width = 3, Height = 1;


		public CellOffset UtilityInputOffset = new CellOffset(-1, 0);
		public CellOffset UtilityOutputOffset = new CellOffset(1, 0);
		public PermittedRotations Rotations = PermittedRotations.R360;
		public BuildLocationRule buildLocationRule = BuildLocationRule.Anywhere;

		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, Width, Height, KANIM, 50, 60f, TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER2, MATERIALS.ALL_METALS, 800f, buildLocationRule, TUNING.BUILDINGS.DECOR.PENALTY.TIER1, TUNING.NOISE_POLLUTION.NOISY.TIER0);
			def.InputConduitType = ConduitType.Liquid;
			def.OutputConduitType = ConduitType.Liquid;
			def.Floodable = false;
			def.Overheatable = false;
			def.PermittedRotations = Rotations;
			def.ViewMode = OverlayModes.LiquidConduits.ID;
			def.AudioCategory = "HollowMetal";
			def.UtilityInputOffset = UtilityInputOffset;
			def.UtilityOutputOffset = UtilityOutputOffset;
			List<LogicPorts.Port> list1 = new List<LogicPorts.Port>();
			list1.Add(LogicPorts.Port.OutputPort(SmartReservoir.PORT_ID, new CellOffset(0, 0), global::STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT, global::STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.SMARTRESERVOIR.LOGIC_PORT_INACTIVE, false, false));
			def.LogicOutputPorts = list1;
			GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, ID);
			def.DefaultAnimState = "off";
			return def;
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddOrGet<UserNameable>();
			go.AddOrGet<Reservoir>();
			Storage storage = BuildingTemplates.CreateDefaultStorage(go, false);
			storage.showDescriptor = true;
			storage.storageFilters = STORAGEFILTERS.LIQUIDS;
			storage.capacityKg = GetStorageCapacity();
			storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
			storage.showCapacityStatusItem = true;
			storage.showCapacityAsMainStatus = true;
			go.AddOrGet<SmartReservoir>();

			ConduitConsumer consumer = go.AddOrGet<ConduitConsumer>();
			consumer.conduitType = ConduitType.Liquid;
			consumer.ignoreMinMassCheck = true;
			consumer.forceAlwaysSatisfied = true;
			consumer.alwaysConsume = true;
			consumer.capacityKG = storage.capacityKg;
			ConduitDispenser dispenser = go.AddOrGet<ConduitDispenser>();
			dispenser.conduitType = ConduitType.Liquid;
			dispenser.elementFilter = null;
		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			go.AddOrGetDef<StorageController.Def>();
			go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayBehindConduits, false);
			go.AddOrGet<ContentBasedReservoirTint>();
		}
	}
}

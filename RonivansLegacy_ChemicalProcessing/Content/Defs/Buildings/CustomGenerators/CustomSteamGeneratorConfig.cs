using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.Buildings.ConfigInterfaces;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using UtilLibs.BuildingPortUtils;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.CustomGenerators
{
    class CustomSteamGeneratorConfig : IBuildingConfig
	{
		
		public const float SizeMultiplier = 1f / 3f; // 1/3 of the area
		public static float Wattage = 850f * SizeMultiplier;

		public static string ID = "CustomSteamGenerator";

		const float conduitInputRate = 1;

		public override BuildingDef CreateBuildingDef()
		{
			//hide coal gen slider
			GeneratorList.AddGeneratorToIgnore(ID);

			float[] construction_mass = [250, 50];
			string[] construction_materials = [GameTags.RefinedMetal.ToString(), GameTags.Plastic.ToString()];

			EffectorValues decor = TUNING.BUILDINGS.DECOR.NONE;
			EffectorValues noise = NOISE_POLLUTION.NOISY.TIER5;
			BuildingDef buildingDef =
				BuildingTemplates.CreateBuildingDef(ID, 1, 5,
				"custom_steam_generator_kanim",
				(int)(30 * SizeMultiplier),
				(int)(60f * SizeMultiplier),
				construction_mass, construction_materials,
				1600f, BuildLocationRule.OnFloor, decor, noise);
			buildingDef.OutputConduitType = ConduitType.Liquid;
			buildingDef.UtilityOutputOffset = new CellOffset(0, 4);
			buildingDef.GeneratorWattageRating = Wattage;
			buildingDef.GeneratorBaseCapacity = Wattage;
			buildingDef.Entombable = true;
			buildingDef.IsFoundation = false;
			buildingDef.PermittedRotations = PermittedRotations.FlipH;
			buildingDef.ViewMode = OverlayModes.Power.ID;
			buildingDef.AudioCategory = "Metal";
			buildingDef.RequiresPowerOutput = true;
			buildingDef.PowerOutputOffset = new CellOffset(0, 0);
			buildingDef.OverheatTemperature = 1273.15f;
			buildingDef.SelfHeatKilowattsWhenActive = 4f * SizeMultiplier;
			buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.POWER);
			buildingDef.AddSearchTerms((string)SEARCH_TERMS.STEAM);

			SoundUtils.CopySoundsToAnim("custom_steam_generator_kanim", "steamturbine2_kanim");
			return buildingDef;

		}
		public override void DoPostConfigureComplete(GameObject go)
		{
			Storage storage1 = go.AddComponent<Storage>();
			storage1.showDescriptor = false;
			storage1.showInUI = false;
			storage1.storageFilters = STORAGEFILTERS.LIQUIDS;
			storage1.SetDefaultStoredItemModifiers(SteamTurbineConfig2.StoredItemModifiers);
			storage1.capacityKg = 10f * SizeMultiplier;

			Storage storage2 = go.AddComponent<Storage>();
			storage2.showDescriptor = false;
			storage2.showInUI = false;
			storage2.storageFilters = STORAGEFILTERS.GASES;
			storage2.SetDefaultStoredItemModifiers(SteamTurbineConfig2.StoredItemModifiers);

			SteamTurbine steamTurbine = go.AddOrGet<SteamTurbine>();
			steamTurbine.srcElem = SimHashes.Steam;
			steamTurbine.destElem = SimHashes.Water;
			steamTurbine.pumpKGRate = 2f * SizeMultiplier;
			steamTurbine.maxSelfHeat = 64f * SizeMultiplier;
			steamTurbine.wasteHeatToTurbinePercent = 0.1f;

			ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
			conduitDispenser.elementFilter = [SimHashes.Water];
			conduitDispenser.conduitType = ConduitType.Liquid;
			conduitDispenser.storage = storage1;
			conduitDispenser.alwaysDispense = true;

			go.AddOrGet<LogicOperationalController>();
			Prioritizable.AddRef(go);

			go.GetComponent<KPrefabID>().prefabSpawnFn +=(game_object =>
			{
				HandleVector<int>.Handle handle = GameComps.StructureTemperatures.GetHandle(game_object);
				StructureTemperaturePayload payload = GameComps.StructureTemperatures.GetPayload(handle);
				Extents extents = game_object.GetComponent<Building>().GetExtents();
				Extents newExtents = new Extents(extents.x, extents.y - 1, extents.width, extents.height + 1);
				payload.OverrideExtents(newExtents);
				GameComps.StructureTemperatures.SetPayload(handle, ref payload);
				Storage[] components = game_object.GetComponents<Storage>();
				game_object.GetComponent<SteamTurbine>().SetStorage(components[1], components[0]);
			});

			go.AddOrGet<KBatchedAnimController>().randomiseLoopedOffset = true;
			Tinkerable.MakePowerTinkerable(go);
			AddVisualizer(go);
		}
		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			base.ConfigureBuildingTemplate(go, prefab_tag);

			var kprefab = go.GetComponent<KPrefabID>();
			kprefab.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			kprefab.AddTag(RoomConstraints.ConstraintTags.PowerBuilding);
			kprefab.AddTag(RoomConstraints.ConstraintTags.GeneratorType);
			kprefab.AddTag(RoomConstraints.ConstraintTags.HeavyDutyGeneratorType);
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => AddVisualizer(go);

		public override void DoPostConfigureUnderConstruction(GameObject go)
		{
			go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.CanPowerTinker.Id;
			AddVisualizer(go);
		}
		private static void AddVisualizer(GameObject go1)
		{
			RangeVisualizer rangeVisualizer = go1.AddOrGet<RangeVisualizer>();
			rangeVisualizer.RangeMin.x = 0;
			rangeVisualizer.RangeMin.y = -2;
			rangeVisualizer.RangeMax.x = 0;
			rangeVisualizer.RangeMax.y = -2;
			rangeVisualizer.TestLineOfSight = false;
			go1.GetComponent<KPrefabID>().instantiateFn += (go2 => go2.GetComponent<RangeVisualizer>().BlockingCb = new Func<int, bool>(SteamTurbineConfig2.SteamTurbineBlockingCB));
		}
	}
}


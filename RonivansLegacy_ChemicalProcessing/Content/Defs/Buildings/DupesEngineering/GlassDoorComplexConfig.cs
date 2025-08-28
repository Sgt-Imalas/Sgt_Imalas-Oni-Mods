using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Defs.Buildings.DupesEngineering
{
    class GlassDoorComplexConfig : IBuildingConfig
	{
		public static string ID = "GlassDoorComplex";		
		public override BuildingDef CreateBuildingDef()
		{
			float[] material_mass = [100f, 50f];
			string[] construction_materials = ["RefinedMetal", "Transparent"];

			EffectorValues nONE = NOISE_POLLUTION.NONE;
			BuildingDef def = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "glass_door_complex_kanim", 30, 60f, material_mass, construction_materials, 1600f, BuildLocationRule.Tile, TUNING.BUILDINGS.DECOR.BONUS.TIER1, nONE, 1f);
			def.Overheatable = false;
			def.RequiresPowerInput = true;
			def.EnergyConsumptionWhenActive = 120f;
			def.Floodable = false;
			def.Entombable = false;
			def.IsFoundation = true;
			def.ViewMode = OverlayModes.Power.ID;
			def.TileLayer = ObjectLayer.FoundationTile;
			def.AudioCategory = "Metal";
			def.PermittedRotations = PermittedRotations.R90;
			def.ForegroundLayer = Grid.SceneLayer.InteriorWall;
			def.SceneLayer = Grid.SceneLayer.TileMain;
			def.LogicInputPorts = DoorConfig.CreateSingleInputPortList(new CellOffset(0, 0));
			SoundEventVolumeCache.instance.AddVolume("glass_door_complex_kanim", "Open_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
			SoundEventVolumeCache.instance.AddVolume("glass_door_complex_kanim", "Close_DoorPressure", NOISE_POLLUTION.NOISY.TIER2);
			SoundUtils.CopySoundsToAnim("glass_door_complex_kanim", "door_external_kanim");
			return def;
		}

		public static List<LogicPorts.Port> CreateSingleInputPortList(CellOffset offset)
		{
			List<LogicPorts.Port> list1 = new List<LogicPorts.Port>();
			list1.Add(LogicPorts.Port.InputPort(Door.OPEN_CLOSE_PORT_ID, offset, global::STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN, global::STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN_ACTIVE, global::STRINGS.BUILDINGS.PREFABS.DOOR.LOGIC_OPEN_INACTIVE, false, false));
			return list1;
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			Door door = go.AddOrGet<Door>();
			door.hasComplexUserControls = true;
			door.unpoweredAnimSpeed = 0.65f;
			door.poweredAnimSpeed = 5f;
			door.doorClosingSoundEventName = "MechanizedAirlock_closing";
			door.doorOpeningSoundEventName = "MechanizedAirlock_opening";
			go.AddOrGet<ZoneTile>();
			go.AddOrGet<AccessControl>();
			go.AddOrGet<KBoxCollider2D>();
			Prioritizable.AddRef(go);
			go.AddOrGet<CopyBuildingSettings>().copyGroupTag = GameTags.Door;
			go.AddOrGet<Workable>().workTime = 5f;
			UnityEngine.Object.DestroyImmediate(go.GetComponent<BuildingEnabledButton>());
			go.GetComponent<AccessControl>().controlEnabled = true;
			go.GetComponent<KBatchedAnimController>().initialAnim = "closed";
		}
	}
}

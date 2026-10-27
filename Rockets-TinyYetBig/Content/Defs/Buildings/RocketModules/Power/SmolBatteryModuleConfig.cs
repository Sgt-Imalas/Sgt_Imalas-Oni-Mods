using Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations.BuildingConfigInterfaces;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.Buildings.Generators
{
	public class SmolBatteryModuleConfig : IBuildingConfig, IHasConfigurableStorageCapacity
	{
		public const string ID = "RTB_SmolBatteryModule";
		private static readonly CellOffset PLUG_OFFSET = new CellOffset(-1, 0);

		static float _storageCapacity = 40; //in KJ
		public static float StorageCapacity => _storageCapacity;
		public float GetStorageCapacity() => _storageCapacity;
		public string GetStorageLabel() => null;
		public void SetStorageCapacity(float mass) => _storageCapacity = mass;
		public Tuple<int, int> GetLimitsStorage() => new(10, 100);
		public string StorageUnitLabel() => global::STRINGS.UI.UNITSUFFIXES.ELECTRICAL.KILOJOULE;
		public string GetStorageTooltip() => null;

		public override string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public override BuildingDef CreateBuildingDef()
		{
			float[] hollowTieR2 = { 250f };
			string[] rawMetals = MATERIALS.RAW_METALS;
			EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
			EffectorValues none = BUILDINGS.DECOR.NONE;
			EffectorValues noise = tieR2;
			BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "rocket_battery_pack_small_kanim", 1000, 30f, hollowTieR2, rawMetals, 9999f, BuildLocationRule.Anywhere, none, noise);
			BuildingTemplates.CreateRocketBuildingDef(buildingDef);
			buildingDef.DefaultAnimState = "grounded";
			buildingDef.AttachmentSlotTag = GameTags.Rocket;
			buildingDef.SceneLayer = Grid.SceneLayer.Building;
			buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
			buildingDef.OverheatTemperature = 2273.15f;
			buildingDef.Floodable = false;
			buildingDef.PowerInputOffset = PLUG_OFFSET;
			buildingDef.PowerOutputOffset = PLUG_OFFSET;
			buildingDef.ObjectLayer = ObjectLayer.Building;
			buildingDef.RequiresPowerOutput = true;
			buildingDef.UseWhitePowerOutputConnectorColour = true;
			buildingDef.CanMove = true;
			buildingDef.Cancellable = false;
			return buildingDef;
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
			go.AddOrGet<LoopingSounds>();
			go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
			go.AddComponent<RequireInputs>();
			go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
			{
				new BuildingAttachPoint.HardPoint(new CellOffset(0, 1), GameTags.Rocket, (AttachableBuilding) null)
			};
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			Prioritizable.AddRef(go);
			ModuleBattery moduleBattery = go.AddOrGet<ModuleBattery>();
			moduleBattery.capacity = StorageCapacity * 1_000f;
			moduleBattery.joulesLostPerSecond = 1f;
			WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
			virtualNetworkLink.link1 = PLUG_OFFSET;
			virtualNetworkLink.visualizeOnly = true;
			BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, ROCKETRY.BURDEN.INSIGNIFICANT);
		}
	}

}

using AkisDecorPackB.Content.ModDb;
using AkisDecorPackB.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace AkisDecorPackB.Content.Defs.Buildings
{
	internal class OilLanternConfig : IBuildingConfig
	{
		public static string ID = "DecorPackB_OilLantern";

		public override BuildingDef CreateBuildingDef()
		{
			BuildingDef def = BuildingTemplates.CreateBuildingDef(
				ID,
				1,
				1,
				"dpii_oil_lantern_kanim",
				BUILDINGS.HITPOINTS.TIER2,
				BUILDINGS.CONSTRUCTION_TIME_SECONDS.TIER4,
				BUILDINGS.CONSTRUCTION_MASS_KG.TIER2,
				MATERIALS.ALL_METALS,
				BUILDINGS.MELTING_POINT_KELVIN.TIER1,
				Mod_Db.BuildLocationRules.OnAnyWall,
				DECOR.NONE,
				NOISE_POLLUTION.NONE
			);

			def.ExhaustKilowattsWhenActive = 0.5f;
			def.SelfHeatKilowattsWhenActive = 0.5f;

			def.UtilityInputOffset = new CellOffset(0, 0);
			def.InputConduitType = ConduitType.Liquid;

			def.BaseTimeUntilRepair = -1f;

			def.AudioCategory = "glass";
			def.ViewMode = OverlayModes.Light.ID;
			def.PermittedRotations = PermittedRotations.R360;

			def.ReplacementLayer = ObjectLayer.NumLayers;

			return def;
		}

		public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
		{
			var lightShapePreview = go.AddComponent<LightShapePreview>();
			lightShapePreview.lux = 1000;
			lightShapePreview.radius = 4f;
			lightShapePreview.shape = LightShape.Circle;

			go.AddComponent<AutoRotateToNearestWall>();
		}

		public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
		{
			go.AddTag(GameTags.LightSource);
		}

		public override void DoPostConfigureComplete(GameObject go)
		{
			go.GetComponent<Building>().Def.BuildingPreview.AddComponent<AutoRotateToNearestWall>();

			go.AddOrGet<LoopingSounds>();

			var brightColor = (Util.ColorFromHex("ff7300") * 2.1f) with { a = 1.0f };
			var darkColor = Util.ColorFromHex("ff4000");

			var light2D = go.AddOrGet<FlickeringLight2D>();
			light2D.overlayColour = LIGHT2D.FLOORLAMP_OVERLAYCOLOR;
			light2D.Color = brightColor;
			light2D.Range = 4f;
			light2D.Angle = 0.0f;
			light2D.Direction = LIGHT2D.FLOORLAMP_DIRECTION;
			light2D.Offset = new Vector2(0f, 0.5f);
			light2D.shape = LightShape.Circle;
			light2D.drawOverlay = true;
			light2D.darkColor = darkColor;
			light2D.brightColor = brightColor;
			light2D.minTime = 0.17f;
			light2D.maxTime = 0.75f;
			light2D.transitionSpeed = 1f;

			go.AddOrGet<OilLantern>();

			Storage storage = go.AddOrGet<Storage>();

			Tag oil = GameTags.LubricatingOil;

			storage.capacityKg = 10f;
			storage.showInUI = true;
			storage.allowItemRemoval = false;
			storage.storageFilters = new List<Tag> { oil };

			ManualDeliveryKG manualDeliveryKG = go.AddOrGet<ManualDeliveryKG>();
			manualDeliveryKG.SetStorage(storage);
			manualDeliveryKG.requestedItemTag = oil;
			manualDeliveryKG.capacity = 10f;
			manualDeliveryKG.refillMass = 1.5f;
			manualDeliveryKG.choreTypeIDHash = Db.Get().ChoreTypes.FetchCritical.IdHash;
			manualDeliveryKG.operationalRequirement = Operational.State.Functional;
			manualDeliveryKG.allowPause = true;

			ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
			conduitConsumer.conduitType = ConduitType.Liquid;
			conduitConsumer.consumptionRate = 0.1f;
			conduitConsumer.capacityTag = oil;
			conduitConsumer.capacityKG = 10f;
			conduitConsumer.forceAlwaysSatisfied = true;
			conduitConsumer.ignoreMinMassCheck = true;
			conduitConsumer.alwaysConsume = true;
			conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

			ElementConverter elementConverter = go.AddComponent<ElementConverter>();
			elementConverter.consumedElements =
			[
				new ElementConverter.ConsumedElement(oil, 0.01f)
			];
			elementConverter.outputElements =
			[
				new ElementConverter.OutputElement(0.01f, SimHashes.CarbonDioxide, 303.15f, outputElementOffsety: 1f)
			];

			go.AddOrGetDef<LightController.Def>();
			go.GetComponent<RequireInputs>().SetRequirements(false, false);

			Prioritizable.AddRef(go);
		}
	}
}

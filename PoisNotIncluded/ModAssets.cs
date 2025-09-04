using Klei.AI;
using PoisNotIncluded.Content.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static InventoryOrganization;
using static STRINGS.BUILDING.STATUSITEMS;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.UI.DETAILTABS;
using static UtilLibs.SupplyClosetUtils;

namespace PoisNotIncluded
{
	internal class ModAssets
	{
		public const string GeneratedIdPrefix = "PNI_";
		public const string LAEMP_Prefix = "LAEMP_";
		public const string NEWBUILD_Prefix = "NEWBUILD_";
		public const string DECONAME_Suffix = "_DECONAME";
		public const string POI_Category = "PNI_BuildMenuCategory_POI";

		///this is horrible......
		///next time use a builder pattern from the start
		///like the one in BuildingCreator.. thats not even finished yet...
		///
		public static bool TryMakeDefFromGravitasEntity(string entityID, BuildLocationRule rule, Tuple<int, int> dimensionOverride, out BuildingDef def, out string prefabID, string animOverride, bool brokenNameAdd, bool decorNameAdd)
		{
			prefabID = GeneratedIdPrefix + entityID;
			if (animOverride != null)
				prefabID += animOverride;


			def = null;
			var entity = Assets.GetPrefab(entityID);
			if (entity == null)
			{
				SgtLogger.l("could not find placed entity with id " + entityID);
				return false;
			}

			if (!entity.TryGetComponent<OccupyArea>(out var occupyArea))
			{
				SgtLogger.l("could not find occupyArea on " + entityID);
				return false;
			}

			if (!entity.TryGetComponent<PrimaryElement>(out var elementSource))
			{
				SgtLogger.l("could not find PrimaryElement on " + entityID);
				return false;
			}
			if (!entity.TryGetComponent<KSelectable>(out var selectable))
			{
				SgtLogger.l("could not find KSelectable on " + entityID);
				return false;
			}
			string desc = string.Empty;
			if (entity.TryGetComponent<InfoDescription>(out var description))
			{
				desc = description.description;
			}

			string name = selectable.GetName();

			if (brokenNameAdd)
			{
				name = Strings.Get("STRINGS.BUILDING.STATUSITEMS.BROKEN.NAME") + " " + name;
			}
			else if (decorNameAdd)
			{
				name += " (" + STRINGS.DUPLICANTS.ATTRIBUTES.DECOR.NAME + ")";
				prefabID += DECONAME_Suffix;
			}

			string upperPrefabID = prefabID.ToUpperInvariant();
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.NAME", name);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.DESC", desc);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.EFFECT", "");

			int width = occupyArea.GetWidthInCells();
			int height = occupyArea.GetHeightInCells();

			if (dimensionOverride != null)
			{
				if (dimensionOverride.first > 0)
					width = dimensionOverride.first;
				if (dimensionOverride.second > 0)
					height = dimensionOverride.second;
			}

			Tag material = elementSource.ElementID.CreateTag();
			var element = ElementLoader.GetElement(material);
			if (element.HasTag(GameTags.BuildableRaw))
				material = GameTags.BuildableRaw;
			else if (material == SimHashes.Creature.CreateTag() || material == SimHashes.Unobtanium.CreateTag())
				material = GameTags.Steel;
			else if (material == SimHashes.Polypropylene.CreateTag())
				material = GameTags.Plastic;
			else if (material == SimHashes.Glass.CreateTag() || material == SimHashes.Diamond.CreateTag())
				material = GameTags.Transparent;

			entity.TryGetComponent<KBatchedAnimController>(out var kbac);

			var anim = kbac.AnimFiles[0];
			string initialAnim = kbac.initialAnim;

			if (animOverride != null)
			{
				initialAnim = animOverride;
			}

			EffectorValues decorValues = new EffectorValues(0, 0);
			if (entity.TryGetComponent<DecorProvider>(out var decorator))
				decorValues = new EffectorValues((int)decorator.baseDecor, (int)decorator.baseRadius);
			float cost = 25 * (width * height);
			int hitpounts = 5 * (width * height);
			int time = 5 * (width * height);

			def = BuildingTemplates.CreateBuildingDef(prefabID, width, height, anim.name, hitpounts, time, [cost], [material.ToString()], element.highTemp, rule, decorValues, TUNING.NOISE_POLLUTION.NONE);
			def.DefaultAnimState = initialAnim;
			def.ViewMode = OverlayModes.Decor.ID;
			def.Overheatable = false;
			def.Floodable = false;
			if (entity.TryGetComponent<Rotatable>(out var rotatable))
				def.PermittedRotations = rotatable.permittedRotations;
			if (def.PermittedRotations == PermittedRotations.Unrotatable || def.PermittedRotations == PermittedRotations.R90)
				def.PermittedRotations = PermittedRotations.FlipH;

			return true;
		}

		public static BuildingDef MakeBuildingDef(string id, int width, int height, float[] cost, string[] mats, string kanim, string init, BuildLocationRule rule )
		{
			BuildingDef def;
			int decor = Math.Max(width, height) * 5;
			int range = Math.Max(width, height) * 2;

			int time = 5 * (width * height);
			int hitpounts = 5 * (width * height);
			def = BuildingTemplates.CreateBuildingDef(id, width, height, kanim, hitpounts, time, cost, mats, 1600, rule, new(decor, range), TUNING.NOISE_POLLUTION.NONE);
			def.DefaultAnimState = init;
			def.ViewMode = OverlayModes.Decor.ID;
			//def.Overheatable = false;
			def.Floodable = false;
			def.PermittedRotations = PermittedRotations.FlipH;
			return def;
		}


		public static bool TryRegisterDynamicGravitasBuilding_Ceiling(string entityId, string subcat, Tuple<int, int> dimensionOverride = null, string animName = null) => TryRegisterDynamicGravitasBuilding(entityId, subcat, BuildLocationRule.OnCeiling, dimensionOverride, animOverride: animName);
		public static bool TryRegisterDynamicGravitasBuilding_Floor(string entityId, string subcat, Tuple<int, int> dimensionOverride = null, string animName = null) => TryRegisterDynamicGravitasBuilding(entityId, subcat, BuildLocationRule.OnFloor, dimensionOverride, animOverride: animName);
		public static bool TryRegisterDynamicGravitasBuilding_Backwall(string entityId, string subcat, Tuple<int, int> dimensionOverride = null) => TryRegisterDynamicGravitasBuilding(entityId, subcat, BuildLocationRule.NotInTiles, dimensionOverride, true);

		public static bool TryRegisterDynamicGravitasBuilding_Lamp(string startId, string anim, Tuple<string, string> off_on_Anims, string name, string desc, int width, int height, string[] mats, float[] costs, int wattage, int lux, int luxRange, Vector2 lightOffset, CellOffset PowerOffset, LightShape shape)
		{
			string prefabID = GeneratedIdPrefix + LAEMP_Prefix+ startId;
			string upperPrefabID = prefabID.ToUpperInvariant();
			var buildingDef = MakeBuildingDef(prefabID, width, height, costs, mats, anim, off_on_Anims.first, BuildLocationRule.OnCeiling);

			SgtLogger.Assert(buildingDef, "building def for " + prefabID);

			buildingDef.PowerInputOffset = PowerOffset;
			buildingDef.RequiresPowerInput = true;
			buildingDef.AddLogicPowerPort = true;
			buildingDef.EnergyConsumptionWhenActive = wattage;
			buildingDef.SelfHeatKilowattsWhenActive = Mathf.Clamp(((float)wattage) / 60f, 0.5f,6f);
			buildingDef.ViewMode = OverlayModes.Light.ID;
			buildingDef.AudioCategory = "Metal";

			if (name.Contains("STRINGS."))
				name = Strings.Get(name);
			if (desc.Contains("STRINGS."))
				desc = Strings.Get(desc);

			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.NAME", name);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.DESC", desc);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.EFFECT", "");

			var mng = BuildingConfigManager.Instance;
			GameObject defPrefab = UnityEngine.Object.Instantiate<GameObject>(mng.baseTemplate);
			UnityEngine.Object.DontDestroyOnLoad(defPrefab);

			KPrefabID defKPrefabId = defPrefab.GetComponent<KPrefabID>();
			defKPrefabId.PrefabTag = buildingDef.Tag;
			defKPrefabId.SetDlcRestrictions(buildingDef);

			defPrefab.name = buildingDef.PrefabID + "Template";
			defPrefab.GetComponent<Building>().Def = buildingDef;
			defPrefab.GetComponent<OccupyArea>().SetCellOffsets(buildingDef.PlacementOffsets);
			defPrefab.AddTag(GameTags.RoomProberBuilding);

			//config.ConfigureBuildingTemplate(defPrefab, buildingDef.Tag);
			defPrefab.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource);

			buildingDef.BuildingComplete = BuildingLoader.Instance.CreateBuildingComplete(defPrefab, buildingDef);
			buildingDef.BuildingUnderConstruction = BuildingLoader.Instance.CreateBuildingUnderConstruction(buildingDef);
			buildingDef.BuildingUnderConstruction.name = BuildingConfigManager.GetUnderConstructionName(buildingDef.BuildingUnderConstruction.name);
			buildingDef.BuildingPreview = BuildingLoader.Instance.CreateBuildingPreview(buildingDef);
			buildingDef.BuildingPreview.name += "Preview";

			buildingDef.PostProcess();
			//config.DoPostConfigureComplete(buildingDef.BuildingComplete);
			ConfigureLightComplete(buildingDef.BuildingComplete, lux, luxRange, lightOffset, off_on_Anims, shape);
			//config.DoPostConfigurePreview(buildingDef, buildingDef.BuildingPreview);
			ConfigureLightPreview(buildingDef.BuildingPreview, lux, luxRange, lightOffset, shape);
			//config.DoPostConfigureUnderConstruction(buildingDef.BuildingUnderConstruction);


			AddToGame(buildingDef, prefabID,GameStrings.PlanMenuSubcategory.Lights);

			return true;
		}

		static void ConfigureLightPreview(GameObject go, int lux, int radius, Vector2 offset, LightShape shape)
		{
			LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
			lightShapePreview.lux = lux;
			lightShapePreview.radius = radius;
			lightShapePreview.shape = shape;
			lightShapePreview.offset = new(Mathf.FloorToInt(offset.x), Mathf.FloorToInt(offset.y));
		}
		static void ConfigureLightComplete(GameObject go, int lux, int radius, Vector2 offset, Tuple<string, string> off_on, LightShape shape)
		{
			go.AddOrGet<LoopingSounds>();
			Light2D light2D = go.AddOrGet<Light2D>();
			light2D.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
			light2D.Color = LIGHT2D.CEILINGLIGHT_COLOR;
			light2D.Range = radius;
			light2D.Angle = 2.6f;
			light2D.Direction = LIGHT2D.CEILINGLIGHT_DIRECTION;
			light2D.Offset = offset;
			light2D.shape = shape;
			light2D.drawOverlay = true;
			light2D.Lux = lux;
			var light = go.AddComponent<CustomLightController>();
			light.OffAnim = off_on.first;
			light.OnAnim = off_on.second;
		}


		public static void RegisterNewBuilding(string startId, string subcategory, BuildLocationRule rule, string anim, string intialAnim, string name, string desc, int width, int height, string[] mats, float[] costs, bool decorName = true, string[] altAnims = null)
		{
			if (Assets.GetAnim(anim) == null)
				return;

			string prefabID = GeneratedIdPrefix + NEWBUILD_Prefix + startId;
			string upperPrefabID = prefabID.ToUpperInvariant();
			var buildingDef = MakeBuildingDef(prefabID, width, height, costs, mats, anim, intialAnim, rule);

			SgtLogger.Assert(buildingDef, "building def for " + prefabID);

			buildingDef.ViewMode = OverlayModes.Decor.ID;
			buildingDef.AudioCategory = "Metal";

			if (name.Contains("STRINGS."))
				name = Strings.Get(name);
			if (desc.Contains("STRINGS."))
				desc = Strings.Get(desc);

			if (decorName)
				name += " (" + STRINGS.DUPLICANTS.ATTRIBUTES.DECOR.NAME + ")";

			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.NAME", name);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.DESC", desc);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{upperPrefabID}.EFFECT", "");
			RegisterDef(buildingDef, subcategory, rule);

			if (altAnims != null)
				buildingDef.BuildingComplete.AddOrGet<AnimSelector>().AvailableAnimNames = altAnims;

		}

		public static bool TryRegisterDynamicGravitasBuilding(string entityId, string subcategory, BuildLocationRule rule = BuildLocationRule.NotInTiles, Tuple<int, int> dimensionOverride = null, bool backwall = false, string animOverride = null, bool brokenName = false, bool isEntitySpawner = false, string[] materialOverride = null, float[] costOverride = null, bool decorName = false, string[] altAnims = null)
		{

			if (!TryMakeDefFromGravitasEntity(entityId, rule, dimensionOverride, out BuildingDef buildingDef, out string prefabId, animOverride, brokenName, decorName))
			{
				return false;
			}
			if (backwall)
			{
				buildingDef.ObjectLayer = ObjectLayer.Backwall;
				buildingDef.SceneLayer = Grid.SceneLayer.Backwall;
			}

			if (materialOverride != null)
				buildingDef.MaterialCategory = materialOverride;
			if (costOverride != null)
				buildingDef.Mass = costOverride;

			RegisterDef(buildingDef, subcategory, rule);

			if (isEntitySpawner)
				buildingDef.BuildingComplete.AddOrGet<EntitySpawner>().EntityToSpawn = entityId;
			if(altAnims != null)
				buildingDef.BuildingComplete.AddOrGet<AnimSelector>().AvailableAnimNames = altAnims;
			return true;
		}

		static void RegisterDef(BuildingDef buildingDef, string subcategory, BuildLocationRule rule = BuildLocationRule.NotInTiles)
		{//mirroring BuildingConfigManager.RegisterBuildin
			var mng = BuildingConfigManager.Instance;
			GameObject defPrefab = UnityEngine.Object.Instantiate<GameObject>(mng.baseTemplate);
			UnityEngine.Object.DontDestroyOnLoad(defPrefab);

			KPrefabID defKPrefabId = defPrefab.GetComponent<KPrefabID>();
			defKPrefabId.PrefabTag = buildingDef.Tag;
			defKPrefabId.SetDlcRestrictions(buildingDef);

			defPrefab.name = buildingDef.PrefabID + "Template";
			defPrefab.GetComponent<Building>().Def = buildingDef;
			defPrefab.GetComponent<OccupyArea>().SetCellOffsets(buildingDef.PlacementOffsets);
			defPrefab.AddTag(GameTags.RoomProberBuilding);
			defPrefab.AddTag(GameTags.Decoration);

			//config.ConfigureBuildingTemplate(defPrefab, buildingDef.Tag);

			if (rule == BuildLocationRule.Anywhere || rule == BuildLocationRule.NotInTiles)
			{
				BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), buildingDef.Tag);
			}

			buildingDef.BuildingComplete = BuildingLoader.Instance.CreateBuildingComplete(defPrefab, buildingDef);
			buildingDef.BuildingUnderConstruction = BuildingLoader.Instance.CreateBuildingUnderConstruction(buildingDef);
			buildingDef.BuildingUnderConstruction.name = BuildingConfigManager.GetUnderConstructionName(buildingDef.BuildingUnderConstruction.name);
			buildingDef.BuildingPreview = BuildingLoader.Instance.CreateBuildingPreview(buildingDef);
			buildingDef.BuildingPreview.name += "Preview";

			buildingDef.PostProcess();


			//config.DoPostConfigureComplete(buildingDef.BuildingComplete);
			//config.DoPostConfigurePreview(buildingDef, buildingDef.BuildingPreview);
			//config.DoPostConfigureUnderConstruction(buildingDef.BuildingUnderConstruction);

			AddToGame(buildingDef, buildingDef.PrefabID, subcategory);
		}

		static void AddToGame(BuildingDef buildingDef, string prefabId, string subcategory)
		{
			Assets.AddBuildingDef(buildingDef);
			InjectionMethods.AddBuildingToPlanScreen(POI_Category, prefabId, subcategory);
		}

		internal static void AddSkins()
		{
			SkinCollection.Create(LadderFastConfig.ID, PermitSubcategories.BUILDINGS_RECREATION)
				.Skin(GeneratedIdPrefix + "PoiLadder", PROPLADDER.NAME, PROPLADDER.DESC, "ladder_poi_kanim");

			if (DlcManager.IsExpansion1Active())
			{
				SkinCollection.Create(ItemPedestalConfig.ID, PermitSubcategories.BUILDINGS_RECREATION)
					.Skin(GeneratedIdPrefix + "PoiPedestal", GRAVITASPEDESTAL.NAME, GRAVITASPEDESTAL.DESC, "gravitas_pedestal_nice_kanim");
			}
			SgtLogger.l("Number of skinsss: " + SkinCollection.SkinSets.Count);
			SkinCollection.RegisterAllSkins();
		}
	}
}

using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace LogicSatellites.Buildings
{
    class LightScatteringLensConfig: IBuildingConfig
    {
        public const string ID = "LS_ScatteringLens";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = new float[2]
            {
                25f,
                75f
            };
            string[] materialType = new string[2]
            {
                "Metal",
                "Transparent"
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues decorValue = BUILDINGS.DECOR.BONUS.TIER0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 1,
                anim: "lense_concave_kanim",
                hitpoints: 100,
                construction_time: 40f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 800f,
                BuildLocationRule.Tile,
                decor: decorValue,
                noise: noiseLevel);

            BuildingTemplates.CreateFoundationTileDef(buildingDef);
            buildingDef.Floodable = false;
            buildingDef.Entombable = false;
            buildingDef.Overheatable = false;
            buildingDef.UseStructureTemperature = false;
            buildingDef.AudioCategory = "Glass";
            buildingDef.AudioSize = "small";
            buildingDef.BaseTimeUntilRepair = -1f;
            buildingDef.SceneLayer = Grid.SceneLayer.GlassTile;
            buildingDef.BlockTileIsTransparent = true;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            SimCellOccupier simCellOccupier = go.AddOrGet<SimCellOccupier>();
            simCellOccupier.setTransparent = true;
            simCellOccupier.notifyOnMelt = true;
            go.AddOrGet<TileTemperature>();
            go.AddOrGet<BuildingHP>().destroyOnDamaged = true;

            go.AddOrGet<UserNameable>().savedName = "Unnamed Lens Tile";
        }


        public override void DoPostConfigureComplete(GameObject go)
        {
            GeneratedBuildings.RemoveLoopingSounds(go);
            go.GetComponent<KPrefabID>().AddTag(GameTags.FloorTiles);

            Light2D light2D = go.AddOrGet<Light2D>();
            light2D.Lux = 0;
            light2D.overlayColour = LIGHT2D.SUNLAMP_OVERLAYCOLOR;
            light2D.Color = LIGHT2D.SUNLAMP_COLOR;
            light2D.Range = 15f;
            light2D.Angle = 0f;
            light2D.Direction = LIGHT2D.SUNLAMP_DIRECTION;
            light2D.shape = LightShape.Cone;
            light2D.drawOverlay = true;
            light2D.Offset = new Vector2(0f,0f);
    
            go.AddOrGet<ScatteringLens>().lightSource = light2D;
            go.AddOrGet<SolarReciever>();

            
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
           
        }
    }
}

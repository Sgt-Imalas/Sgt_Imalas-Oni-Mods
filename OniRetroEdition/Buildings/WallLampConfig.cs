using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace OniRetroEdition.Buildings
{
    internal class WallLampConfig : IBuildingConfig
    {
        public const string ID = "RetroOni_WallLamp";

        public override BuildingDef CreateBuildingDef()
        {
            float[] resourceCost = { 40, 10 };
            string[] buildingResources =
                {
                MATERIALS.REFINED_METAL,
                MATERIALS.TRANSPARENT
            };
            EffectorValues none = NOISE_POLLUTION.NONE;
            EffectorValues tieR1_2 = BUILDINGS.DECOR.BONUS.TIER0;
            EffectorValues noise = none;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 1, "walllamp_kanim", 10, 10f, resourceCost, buildingResources, 800f, BuildLocationRule.NotInTiles, tieR1_2, noise);
            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 10f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
            buildingDef.ViewMode = OverlayModes.Light.ID;
            buildingDef.AudioCategory = "Metal";
            buildingDef.AudioSize = "small";
            buildingDef.Floodable = false;
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
            lightShapePreview.lux = 1800;
            lightShapePreview.radius = 4f;
            lightShapePreview.shape = LightShape.Circle;
            lightShapePreview.offset = new CellOffset((int)def.BuildingComplete.GetComponent<Light2D>().Offset.x, (int)def.BuildingComplete.GetComponent<Light2D>().Offset.y);
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource);
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<EnergyConsumer>();
            go.AddOrGet<LoopingSounds>();
            Light2D light2D = go.AddOrGet<Light2D>();
            light2D.overlayColour = LIGHT2D.FLOORLAMP_OVERLAYCOLOR;
            light2D.Offset = new Vector2(0, 0.5f);
            light2D.Color = LIGHT2D.FLOORLAMP_COLOR;
            light2D.Range = 4f;
            light2D.Angle = 0.0f;
            light2D.Direction = LIGHT2D.FLOORLAMP_DIRECTION;
            light2D.Lux = 1800;
            //light2D.Offset = LIGHT2D.FLOORLAMP_OFFSET;
            light2D.shape = LightShape.Circle;
            light2D.drawOverlay = true;
            go.AddOrGetDef<LightController.Def>();
        }
    }
}

using Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace DupePodRailgun.Buildings
{/// <summary>
/// Building template: 
   // BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
/// </summary>
    class PodRailgunRailPieceConfig : IBuildingConfig
    {
        public const string ID = "DPR_RailgunPiece";
        public Tag RailgunAttachment = TagManager.Create(nameof(RailgunAttachment));

        public override BuildingDef CreateBuildingDef()
        {
            float[] construction_mass = new float[3]
            {
                25f,
                25f,
                500f
            };
            string[] construction_materials = new string[3]
            {
      SimHashes.Ceramic.ToString(),
      SimHashes.Polypropylene.ToString(),
      SimHashes.Steel.ToString()
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues incomplete = BUILDINGS.DECOR.BONUS.MONUMENT.INCOMPLETE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 3, "radbolt_battery_kanim", 1000, 30f, construction_mass, construction_materials, 9999f, BuildLocationRule.BuildingAttachPoint, incomplete, noise);
            BuildingTemplates.CreateMonumentBuildingDef(buildingDef);
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.AttachmentSlotTag = RailgunAttachment;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.PermittedRotations = PermittedRotations.Unrotatable;
            buildingDef.attachablePosition = new CellOffset(0, 0);
            buildingDef.RequiresPowerInput = true;
            buildingDef.CanMove = false;
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 3), RailgunAttachment, (AttachableBuilding) null)
            };
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<KBatchedAnimController>().initialAnim = "option_a";
            go.GetComponent<KPrefabID>();
        }
    }
}

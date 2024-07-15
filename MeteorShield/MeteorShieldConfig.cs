using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace MeteorShield
{
    class MeteorShieldConfig : IBuildingConfig
    {
        public const string ID = "MeteorShield_BubbleShield";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public override BuildingDef CreateBuildingDef()
        {
            float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER5;
            string[] allMetals = TUNING.MATERIALS.ALLOYS;
            EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
            EffectorValues noise = NOISE_POLLUTION.NOISY.TIER3;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 4, "missile_launcher_kanim", 250, 60f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;
            buildingDef.Floodable = false;
            buildingDef.Overheatable = false;
            buildingDef.AudioCategory = "Metal";
            buildingDef.BaseTimeUntilRepair = 400f;
            buildingDef.DefaultAnimState = "off";
            buildingDef.RequiresPowerInput = true;
            buildingDef.PowerInputOffset = new CellOffset(-1, 0);
            buildingDef.EnergyConsumptionWhenActive = 480f;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
            buildingDef.ExhaustKilowattsWhenActive = 1f;
            buildingDef.SelfHeatKilowattsWhenActive = 3f;
            return buildingDef;
        }

        public override void DoPostConfigurePreview(BuildingDef def, GameObject go) => this.AddVisualizer(go);

        public override void DoPostConfigureUnderConstruction(GameObject go) => this.AddVisualizer(go);

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<MeteorShield>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
        }

        private void AddVisualizer(GameObject go1)
        {
            //RangeVisualizer rangeVisualizer = go1.AddOrGet<RangeVisualizer>();
            //rangeVisualizer.OriginOffset = MissileLauncher.Def.LaunchOffset.ToVector2I();
            //rangeVisualizer.RangeMin.x = -MissileLauncher.Def.launchRange.x;
            //rangeVisualizer.RangeMax.x = MissileLauncher.Def.launchRange.x;
            //rangeVisualizer.RangeMin.y = 0;
            //rangeVisualizer.RangeMax.y = MissileLauncher.Def.launchRange.y;
            //rangeVisualizer.AllowLineOfSightInvalidCells = true;
            //go1.GetComponent<KPrefabID>().instantiateFn += (KPrefabID.PrefabFn)(go2 => go2.GetComponent<RangeVisualizer>().BlockingCb = new Func<int, bool>(MissileLauncherConfig.IsCellSkyBlocked));
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            SymbolOverrideControllerUtil.AddToPrefab(go);
        }
    }
}

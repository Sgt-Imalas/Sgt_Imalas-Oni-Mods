using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace DupeStations
{
    internal class PillDispenserConfig : IBuildingConfig
    {
        public static string ID = "DS_PillDispenser";
        public static float BathingTime = 36f;
        public static float WaterConsumedPerBath = 7 * 16;


        public override BuildingDef CreateBuildingDef()
        {
            SoundUtils.CopySoundsToAnim("bathtub_kanim", "hottub_kanim");
            float[] construction_mass = new float[2] { 500f, 200f };
            string[] construction_materials = new string[2]
            {
                "PreciousRock",
                "Metal"
            };
            EffectorValues tieR3 = NOISE_POLLUTION.NOISY.TIER3;
            EffectorValues tieR1 = BUILDINGS.DECOR.BONUS.TIER3;
            EffectorValues noise = tieR3;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 2, "hottub_kanim", 30, 10f, construction_mass, construction_materials, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
            buildingDef.SceneLayer = Grid.SceneLayer.BuildingFront;


            buildingDef.Overheatable = false;
            //buildingDef.EnergyConsumptionWhenActive = 240f;
            //buildingDef.SelfHeatKilowattsWhenActive = 2f;
            //buildingDef.ExhaustKilowattsWhenActive = 2f;
            buildingDef.AudioCategory = "Metal";
            //buildingDef.RequiresPowerInput = true;
            //buildingDef.PowerInputOffset = new CellOffset(-2, 0);
            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();

            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 200f;
            storage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);


            go.AddOrGetDef<RocketUsageRestriction.Def>();
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
        }
    }
}

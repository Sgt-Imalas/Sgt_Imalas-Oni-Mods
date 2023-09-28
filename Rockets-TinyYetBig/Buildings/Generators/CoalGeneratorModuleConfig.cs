using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig
{
    public class CoalGeneratorModuleConfig : IBuildingConfig
    {
        public const string ID = "RTB_GeneratorCoalModule";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] MatCosts = {
                500f
            };
            string[] Materials = MATERIALS.ALL_METALS;
            //{
            //    "Steel"
            //};
            EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "coal_generator_module_kanim", 1000, 90f, MatCosts, Materials, 9999f, BuildLocationRule.Anywhere, none, noise);
            BuildingTemplates.CreateRocketBuildingDef(buildingDef);
            buildingDef.DefaultAnimState = "grounded";
            buildingDef.AttachmentSlotTag = GameTags.Rocket;
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.ViewMode = OverlayModes.Radiation.ID;
            buildingDef.ForegroundLayer = Grid.SceneLayer.Front;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.CanMove = true;
            buildingDef.Cancellable = false;
            buildingDef.attachablePosition = new CellOffset(0, 0);

            buildingDef.GeneratorWattageRating = 280f;
            buildingDef.GeneratorBaseCapacity = 6666f;
            buildingDef.RequiresPowerInput = false;
            buildingDef.RequiresPowerOutput = false;

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 1), GameTags.Rocket, (AttachableBuilding) null)
            }; 
           
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<Prioritizable>();
            Prioritizable.AddRef(go);

            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = 300f;
            storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);

            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.RequestedItemTag = ElementLoader.FindElementByHash(SimHashes.Carbon).tag;
            manualDeliveryKg.capacity = storage.capacityKg;
            manualDeliveryKg.refillMass = storage.capacityKg; /// 8f;
            manualDeliveryKg.MinimumMass = 1;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;

            var del = go.AddOrGet<ManualDeliverablePercentageThreshold>();
            del.refillThreshold = storage.capacityKg;
            
            var generator = go.AddOrGet<RTB_ModuleGenerator>();



            generator.consumptionElement = SimHashes.Carbon.CreateTag();
            generator.consumptionRate = 0.125f;
            generator.consumptionMaxStoredMass = storage.capacityKg;

            generator.outputElement = SimHashes.CarbonDioxide;
            generator.outputProductionTemperature = UtilMethods.GetKelvinFromC(110f);
            generator.outputProductionRate = 0.050f;
            generator.ElementOutputCellOffset = new Vector3(2, 0);

            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, 2);
        }
    }
}

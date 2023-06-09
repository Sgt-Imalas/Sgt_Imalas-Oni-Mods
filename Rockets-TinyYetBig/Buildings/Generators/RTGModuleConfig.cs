using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using static Rockets_TinyYetBig.Behaviours.RTB_ModuleGenerator;

namespace Rockets_TinyYetBig
{
    public class RTGModuleConfig : IBuildingConfig
    {
        public static readonly Tag RTB_RTG = TagManager.Create("RTB_RTG");
        public const string ID = "RTB_RtgGeneratorModule";
        public const float UraniumCapacity = 50f;
        public const float energyProduction = 120f;
        public float ConsumptionRate =  (UraniumCapacity / Config.Instance.IsotopeDecayTime )/600f; 
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] MatCosts = {
                400f
            };
            string[] Materials =
            {
                "Steel"
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NONE;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 3, 1, "rtg_module_kanim", 1000, 30f, MatCosts, Materials, 9999f, BuildLocationRule.Anywhere, none, noise);
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

            buildingDef.GeneratorWattageRating = energyProduction;
            buildingDef.GeneratorBaseCapacity = 240f;
            buildingDef.RequiresPowerInput = false;
            buildingDef.RequiresPowerOutput = false;



          

            return buildingDef;
        }

        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof(RequiresFoundation), prefab_tag);
            go.AddOrGet<LoopingSounds>();
            go.GetComponent<KPrefabID>().AddTag(RTB_RTG);
            Storage storage = go.AddOrGet<Storage>();
            storage.capacityKg = UraniumCapacity;
            go.AddOrGet<FlushUranium>();
            RadiationEmitter radiationEmitter = go.AddOrGet<RadiationEmitter>();
            radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
            radiationEmitter.radiusProportionalToRads = false;
            radiationEmitter.emitRadiusX = (short)7;
            radiationEmitter.emitRadiusY = radiationEmitter.emitRadiusX;
            radiationEmitter.emitRads = 420f;
            radiationEmitter.emissionOffset = new Vector3(0.0f, 0.0f, 0.0f);

            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.SetStorage(storage);
            manualDeliveryKg.RequestedItemTag = ElementLoader.FindElementByHash(SimHashes.EnrichedUranium).tag;
            manualDeliveryKg.capacity = storage.capacityKg;
            manualDeliveryKg.refillMass = 0f;
            manualDeliveryKg.MinimumMass = UraniumCapacity;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.PowerFetch.IdHash;


            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);

            go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
            {
                new BuildingAttachPoint.HardPoint(new CellOffset(0, 1), GameTags.Rocket, (AttachableBuilding) null)
            }; 
           
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            Prioritizable.AddRef(go);
            var generator = go.AddOrGet<RTB_ModuleGenerator>();

            generator.consumptionElement = SimHashes.EnrichedUranium.CreateTag();
            generator.consumptionRate = ConsumptionRate;
            generator.consumptionMaxStoredMass = UraniumCapacity;

            generator.outputElement = SimHashes.DepletedUranium;
            generator.outputProductionRate = ConsumptionRate;
            generator.outputProductionTemperature = 363.15f;

            generator.AllowRefill = false;
            generator.AlwaysActive = true;
            generator.OutputToOwnStorage = true;

            //WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
            //virtualNetworkLink.visualizeOnly = true;
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, 3);
            //go.GetComponent<ReorderableBuilding>().buildConditions.Add(new OnlynRtgPerRocket());
        }
    }
}

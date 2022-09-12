using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.Behaviours.RTB_ModuleGenerator;

namespace Rockets_TinyYetBig
{
    public class SteamGeneratorModuleConfig : IBuildingConfig
    {
        public const string ID = "RTB_SteamGeneratorModule";
        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public override BuildingDef CreateBuildingDef()
        {
            float[] MatCosts = {
                600f
            };
            string[] Materials =
            {
                "Steel"
            };
            EffectorValues tieR2 = NOISE_POLLUTION.NONE;
            EffectorValues none = BUILDINGS.DECOR.NONE;
            EffectorValues noise = tieR2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 5, 1, "steam_generator_module_kanim", 1000, 30f, MatCosts, Materials, 9999f, BuildLocationRule.Anywhere, none, noise);
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

            buildingDef.GeneratorWattageRating = 400f;
            buildingDef.GeneratorBaseCapacity = 2400f;
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
            Prioritizable.AddRef(go);
            Storage storage = go.AddOrGet<Storage>();
            storage.showInUI = false;

            var generator = go.AddOrGet<RTB_ModuleGenerator>();

            generator.consumptionElement = SimHashes.Steam.CreateTag();
            generator.consumptionRate = 1f;
            generator.PullFromRocketStorageType = CargoBay.CargoType.Gasses;

            generator.outputElement = SimHashes.Water;
            generator.outputProductionRate = generator.consumptionRate;
            generator.outputProductionTemperature = UtilMethods.GetKelvinFromC(95);


            generator.AllowRefill = true;
            generator.AlwaysActive = false;
            generator.PushToRocketStorageType = CargoBay.CargoType.Liquids;
            generator.ElementOutputCellOffset = new Vector3(0, 0);

            //WireUtilitySemiVirtualNetworkLink virtualNetworkLink = go.AddOrGet<WireUtilitySemiVirtualNetworkLink>();
            //virtualNetworkLink.visualizeOnly = true;
            BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string)null, 2);
        }
    }
}

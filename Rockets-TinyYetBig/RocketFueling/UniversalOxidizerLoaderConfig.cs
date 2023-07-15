using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

using static ResearchTypes;
using static STRINGS.BUILDINGS.PREFABS;
using static STRINGS.DUPLICANTS.ATTRIBUTES;

namespace Rockets_TinyYetBig.RocketFueling
{
    public class UniversalOxidizerLoaderConfig : IBuildingConfig
    {
        public const string ID = "RTB_UniversalOxidizerLoader";
        private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));
        private ConduitPortInfo solidInputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(0, 0));

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;
        public override BuildingDef CreateBuildingDef()
        {
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(ID, 1, 2, "fuel_loader_oxidizer_kanim", 1000, 60f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER4, MATERIALS.REFINED_METALS, 9999f, BuildLocationRule.OnFloor, noise: NOISE_POLLUTION.NOISY.TIER2, decor: BUILDINGS.DECOR.NONE);
            buildingDef.SceneLayer = Grid.SceneLayer.Building;
            buildingDef.OverheatTemperature = 2273.15f;
            buildingDef.Floodable = false;
            buildingDef.ObjectLayer = ObjectLayer.Building;
            buildingDef.RequiresPowerInput = true;
            buildingDef.DefaultAnimState = "idle";
            buildingDef.CanMove = false;
            buildingDef.UtilityInputOffset = new CellOffset(0, 1);
            buildingDef.PowerInputOffset = new CellOffset(0, 1);
            buildingDef.EnergyConsumptionWhenActive = 120f;
            buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
            buildingDef.ExhaustKilowattsWhenActive = 0.125f;

            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<LoopingSounds>();
            KPrefabID component = go.GetComponent<KPrefabID>();
            component.AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery);
            component.AddTag(BaseModularLaunchpadPortConfig.LinkTag);
            component.AddTag(GameTags.ModularConduitPort);
            component.AddTag(GameTags.NotRocketInteriorBuilding);
            component.AddTag(ModAssets.Tags.SpaceStationOnlyInteriorBuilding);
            go.AddOrGetDef<ModularConduitPortController.Def>().mode = (ModularConduitPortController.Mode.Load);
            var LoaderComp = go.AddOrGet<FuelLoaderComponent>(); 
            LoaderComp.loaderType = FuelLoaderComponent.LoaderType.Oxidizer;

            //LoaderComp.gasPortInfo = this.gasInputPort;
            LoaderComp.liquidPortInfo = this.liquidInputPort;
            LoaderComp.solidPortInfo = this.solidInputPort;

            //LoaderComp.gasStorage = go.AddComponent<Storage>();
            //LoaderComp.gasStorage.showInUI = true; //dev
            //LoaderComp.gasStorage.capacityKg = 1f;
            //LoaderComp.gasStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            //LoaderComp.gasStorage.storageFilters = STORAGEFILTERS.GASES;

            LoaderComp.liquidStorage = go.AddComponent<Storage>();
            LoaderComp.liquidStorage.showInUI = true;//dev
            LoaderComp.liquidStorage.capacityKg = 10f;
            LoaderComp.liquidStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            LoaderComp.liquidStorage.storageFilters = STORAGEFILTERS.LIQUIDS;

            LoaderComp.solidStorage = go.AddComponent<Storage>();
            LoaderComp.solidStorage.showInUI = true;//dev
            LoaderComp.solidStorage.capacityKg = 20f;
            LoaderComp.solidStorage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
            LoaderComp.solidStorage.storageFilters = STORAGEFILTERS.NOT_EDIBLE_SOLIDS;


            ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
            def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
            def.linkBuildingTag = BaseModularLaunchpadPortConfig.LinkTag;
            def.objectLayer = ObjectLayer.Building;
            go.AddOrGet<LogicOperationalController>();

            DropAllWorkable dropAllWorkable = go.AddOrGet<DropAllWorkable>();
            dropAllWorkable.dropWorkTime = 15f;
        }

        public override void DoPostConfigureComplete(GameObject go)
        {
            go.AddOrGet<BuildingCellVisualizer>();
        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            go.AddOrGet<BuildingCellVisualizer>();
            this.AttachPorts(go);
        }

        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            go.AddOrGet<BuildingCellVisualizer>();
            this.AttachPorts(go);
        }
        private void AttachPorts(GameObject go)
        {
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.liquidInputPort;
           // go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;
            go.AddComponent<ConduitSecondaryInput>().portInfo = this.solidInputPort;
        }
    }
}

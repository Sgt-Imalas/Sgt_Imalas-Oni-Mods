using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static STRINGS.DUPLICANTS.CHORES;

namespace SubPlanetaryTransport
{
    internal class SubPlanetaryItemRailgunConfig : IBuildingConfig
    {
        public static readonly string ID = "SPT_ItemRailgun";
        public static readonly HashedString FireInputPort = "SPT_FireInputPort";
        public static readonly HashedString CanFireOutputPort = "SPT_CanFireOutputPort";

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;


        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = new float[]
            {
                2000f
            };
            string[] materialType = new string[]
            {
                MATERIALS.REFINED_METAL
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NOISY.TIER4;
            EffectorValues decorValue = BUILDINGS.DECOR.PENALTY.TIER2;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 6,
                height: 6,
                anim: "ItemShooterRailgun_kanim",
                hitpoints: 1500,
                construction_time: 300f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 1600f,
                BuildLocationRule.OnFloor,
                decor: decorValue,
                noise: noiseLevel);

            buildingDef.AudioCategory = "Metal";
            buildingDef.RequiresPowerInput = true;
            buildingDef.Entombable = true;
            buildingDef.PowerInputOffset = new CellOffset(0, 0);
            buildingDef.EnergyConsumptionWhenActive = 2400f;
            buildingDef.DefaultAnimState = "off";
            buildingDef.SelfHeatKilowattsWhenActive = 32f;
            buildingDef.Overheatable = true;
            buildingDef.OverheatTemperature = UtilMethods.GetKelvinFromC(100);

            buildingDef.PermittedRotations = PermittedRotations.FlipH;
            buildingDef.InputConduitType = ConduitType.Solid;
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.OutputConduitType = ConduitType.Liquid;
            buildingDef.UtilityOutputOffset = new CellOffset(2, 0);


            buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.InputPort(FireInputPort,
                new CellOffset(0, 0),
                (string) global::STRINGS.BUILDINGS.PREFABS.LAUNCHPAD.LOGIC_PORT_LAUNCH,
                (string)"",
                (string) "")
            };
            buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
            {
                LogicPorts.Port.OutputPort( CanFireOutputPort, new CellOffset(1, 0)
                ,
                (string) "Can fire",
                (string) "Can fire",
                (string)"Cannot fire")
            };


            return buildingDef;
        }
        //private ConduitPortInfo secondaryPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(-1, 0));
       // private void AttachPort(GameObject go) => go.AddComponent<ConduitSecondaryInput>().portInfo = this.secondaryPort;
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            //this.AddVisualizer(go);
        }
        public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
        {
            base.DoPostConfigurePreview(def, go);
            //this.AttachPort(go);
        }
        public override void DoPostConfigureUnderConstruction(GameObject go)
        {
            base.DoPostConfigureUnderConstruction(go);
            //this.AttachPort(go);
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            SymbolOverrideControllerUtil.AddToPrefab(go);

            Storage CargoInput = go.AddComponent<Storage>();
            CargoInput.capacityKg = 4000f;
            List<Storage.StoredItemModifier> modifiers = new List<Storage.StoredItemModifier>()
            {
                Storage.StoredItemModifier.Hide,
                Storage.StoredItemModifier.Seal,
                Storage.StoredItemModifier.Insulate,
                Storage.StoredItemModifier.Preserve
            };
            CargoInput.SetDefaultStoredItemModifiers(modifiers);

            //Storage CoolantStorage = go.AddComponent<Storage>();
            //CoolantStorage.capacityKg = 900f;
            //Storage CoolantOutputStorage = go.AddComponent<Storage>();
            //CoolantOutputStorage.capacityKg = 900f;

            Storage workingStorage = go.AddComponent<Storage>();


            SolidConduitConsumer solidConduitConsumer = go.AddOrGet<SolidConduitConsumer>();
            solidConduitConsumer.capacityTag = GameTags.Any;
            solidConduitConsumer.capacityKG = CargoInput.capacityKg;
            solidConduitConsumer.storage = CargoInput;
            solidConduitConsumer.alwaysConsume = true;

            //ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
            //conduitDispenser.storage = CoolantOutputStorage;
            //conduitDispenser.conduitType = ConduitType.Liquid;
            //conduitDispenser.elementFilter = (SimHashes[])null;
            //conduitDispenser.alwaysDispense = true;


            go.AddOrGet<LogicOperationalController>();
            go.AddOrGet<EnergyConsumerSelfSustaining>();
            SubPlanetaryItemRailgun subPlanetaryItemRailgun = go.AddOrGet<SubPlanetaryItemRailgun>();
            subPlanetaryItemRailgun.joulesPerLaunch = 20000f;
            subPlanetaryItemRailgun.jouleCapacity = subPlanetaryItemRailgun.joulesPerLaunch * 7f;
            //subPlanetaryItemRailgun.CoolantStorage = CoolantStorage;
            //subPlanetaryItemRailgun.CoolantOutputStorage = CoolantOutputStorage;
            subPlanetaryItemRailgun.CargoStorage = CargoInput;
            //subPlanetaryItemRailgun.portInfo = secondaryPort;
            subPlanetaryItemRailgun.WorkingStorage = workingStorage;
        }
        private void AddVisualizer(GameObject go)
        {
            RangeVisualizer rangeVisualizer = go.AddOrGet<RangeVisualizer>();
            rangeVisualizer.RangeMin.x = MissileLauncher.Def.LaunchOffset.x - MissileLauncher.Def.launchRange.x;
            rangeVisualizer.RangeMax.x = MissileLauncher.Def.LaunchOffset.x + MissileLauncher.Def.launchRange.x;
            rangeVisualizer.RangeMin.y = MissileLauncher.Def.LaunchOffset.y;
            rangeVisualizer.RangeMax.y = MissileLauncher.Def.LaunchOffset.y + MissileLauncher.Def.launchRange.y;
        }
    }
}
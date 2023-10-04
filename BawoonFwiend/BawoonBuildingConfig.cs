using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace BawoonFwiend
{
    internal class BawoonBuildingConfig : IBuildingConfig
    {
        public static readonly string ID = "BF_BalloonStation";
        private Chore.Precondition HasNoBalloon = new Chore.Precondition()
        {
            id = nameof(HasNoBalloon),
            description = "Duplicant doesn't have a balloon already",
            fn = (Chore.PreconditionFn)((ref Chore.Precondition.Context context, object data) => !((UnityEngine.Object)context.consumerState.consumer == (UnityEngine.Object)null) && !context.consumerState.gameObject.GetComponent<Effects>().HasEffect("HasBalloon"))
        };

        public override string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;


        private void MakeNewBalloonChore(Chore chore)
        {
            GetBalloonWorkable component = chore.target.GetComponent<GetBalloonWorkable>();
            WorkChore<GetBalloonWorkable> data = new WorkChore<GetBalloonWorkable>(Db.Get().ChoreTypes.JoyReaction, (IStateMachineTarget)component, on_complete: new System.Action<Chore>(this.MakeNewBalloonChore), schedule_block: Db.Get().ScheduleBlockTypes.Recreation, priority_class: PriorityScreen.PriorityClass.high, ignore_building_assignment: true);
            data.AddPrecondition(this.HasNoBalloon, (object)data);
            data.AddPrecondition(ChorePreconditions.instance.IsNotARobot, (object)data);
        }

        public override BuildingDef CreateBuildingDef()
        {
            float[] materialMass = new float[]
            {
                250f
            };
            string[] materialType = new string[]
            {
                MATERIALS.REFINED_METAL
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues decorValue = BUILDINGS.DECOR.BONUS.TIER1;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 2,
                anim: "bloon_maker_kanim",
                hitpoints: 30,
                construction_time: 20f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 380f,
                BuildLocationRule.OnFloor,
                decor: decorValue,
                noise: noiseLevel);

            buildingDef.AudioCategory = "Metal";
            buildingDef.UtilityInputOffset = new CellOffset(0, 0);
            buildingDef.InputConduitType = ConduitType.Gas;

            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 480f;
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            buildingDef.SelfHeatKilowattsWhenActive = 1.0f;

            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            go.AddOrGet<Operational>();
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
            storage.capacityKg=Config.Instance.GasMass*20f;
            
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.capacityTag = ModAssets.Tags.BalloonGas;
            conduitConsumer.capacityKG = Config.Instance.GasMass * 20f;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

            go.AddOrGet<LogicOperationalController>();

            ManualDeliveryKG manualDeliveryKg = go.AddOrGet<ManualDeliveryKG>();
            manualDeliveryKg.storage = storage;
            manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
            manualDeliveryKg.RequestedItemTag =  ModAssets.Tags.BalloonGas; //SimHashes.Hydrogen.CreateTag();//
            manualDeliveryKg.capacity = Config.Instance.GasMass * 20f;
            manualDeliveryKg.refillMass = Config.Instance.GasMass * 20f / 4f;
            //manualDeliveryKg.allowPause = true;
            manualDeliveryKg.MinimumMass = 1f;
            manualDeliveryKg.paused = true;
            manualDeliveryKg.operationalRequirement = Operational.State.None;

            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding);
            go.AddOrGet<Bawoongiver>();
            go.AddOrGet<BawoongiverWorkable>();
            go.AddOrGet<EnergyConsumer>();




            RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
            roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
            roomTracker.requirement = RoomTracker.Requirement.Recommended;
            //ColorIntegration(go);
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            SymbolOverrideControllerUtil.AddToPrefab(go);
        }
        //static void ColorIntegration(GameObject go)
        //{
        //    var VaricolouredBalloonsHelperType = Type.GetType("VaricolouredBalloons.VaricolouredBalloonsHelper, VaricolouredBalloons", false, false);

        //    if (VaricolouredBalloonsHelperType != null)
        //    {
        //        SgtLogger.debuglog("Varicoloured Balloons Integration applied");
        //        go.AddComponent(VaricolouredBalloonsHelperType);
        //        return;
        //    }

        //}
    }
}

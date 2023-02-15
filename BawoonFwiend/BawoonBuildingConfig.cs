using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

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
                150f
            };
            string[] materialType = new string[]
            {
                MATERIALS.REFINED_METAL
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues decorValue = BUILDINGS.DECOR.PENALTY.TIER0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 2,
                anim: "storagelocker_kanim",
                hitpoints: 30,
                construction_time: 20f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 380f,
                BuildLocationRule.OnFloor,
                decor: decorValue,
                noise: noiseLevel);

            buildingDef.AudioCategory = "Metal";
            buildingDef.UtilityInputOffset = new CellOffset(0, 1);
            buildingDef.InputConduitType = ConduitType.Gas;

            buildingDef.RequiresPowerInput = true;
            buildingDef.EnergyConsumptionWhenActive = 480f;
            buildingDef.ExhaustKilowattsWhenActive = 0.0f;
            buildingDef.SelfHeatKilowattsWhenActive = 1.0f;

            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            Storage storage = go.AddOrGet<Storage>();
            storage.SetDefaultStoredItemModifiers(Storage.StandardFabricatorStorage);
            ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
            conduitConsumer.conduitType = ConduitType.Gas;
            conduitConsumer.capacityTag = ModAssets.Tags.BalloonGas;
            conduitConsumer.capacityKG = 20f;
            conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;

            go.AddOrGet<LogicOperationalController>();
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding);
            go.AddOrGet<Bawoongiver>();
            go.AddOrGet<BawoongiverWorkable>();
            go.AddOrGet<EnergyConsumer>();

            RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
            roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
            roomTracker.requirement = RoomTracker.Requirement.Recommended;
        }
        public override void DoPostConfigureComplete(GameObject go)
        {

        }
    }
}

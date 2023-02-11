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

        public GameObject CreatePrefab()
        {
            GameObject entity = EntityTemplates.CreateEntity(BalloonStandConfig.ID, BalloonStandConfig.ID, false);
            KAnimFile[] kanimFileArray = new KAnimFile[1]
            {
                Assets.GetAnim((HashedString) "anim_interacts_balloon_receiver_kanim")
            };
            GetBalloonWorkable getBalloonWorkable = entity.AddOrGet<GetBalloonWorkable>();
            getBalloonWorkable.workTime = 2f;
            getBalloonWorkable.workLayer = Grid.SceneLayer.BuildingFront;
            getBalloonWorkable.overrideAnims = kanimFileArray;
            getBalloonWorkable.synchronizeAnims = false;
            return entity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }


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
                120f
            };
            string[] materialType = new string[]
            {
                MATERIALS.METAL
            };
            EffectorValues noiseLevel = NOISE_POLLUTION.NONE;
            EffectorValues decorValue = BUILDINGS.DECOR.PENALTY.TIER0;
            BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef(
                id: ID,
                width: 1,
                height: 2,
                anim: "storagelocker_kanim",
                hitpoints: 15,
                construction_time: 20f,
                construction_mass: materialMass,
                construction_materials: materialType,
                melting_point: 380f,
                BuildLocationRule.OnFloor,
                decor: decorValue,
                noise: noiseLevel);

            buildingDef.AudioCategory = "Metal";

            return buildingDef;
        }
        public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
        {
            GetBalloonWorkable component = go.AddOrGet<GetBalloonWorkable>(); WorkChore<GetBalloonWorkable> data = new WorkChore<GetBalloonWorkable>(
                Db.Get().ChoreTypes.JoyReaction,
                component,
                on_complete: new System.Action<Chore>(this.MakeNewBalloonChore),
                schedule_block: Db.Get().ScheduleBlockTypes.Recreation,
                priority_class: PriorityScreen.PriorityClass.high, ignore_building_assignment: true);
            data.AddPrecondition(this.HasNoBalloon, (object)data);
            data.AddPrecondition(ChorePreconditions.instance.IsNotARobot, (object)data);
        }
        public override void DoPostConfigureComplete(GameObject go)
        {
            go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding);

            
        }
    }
}

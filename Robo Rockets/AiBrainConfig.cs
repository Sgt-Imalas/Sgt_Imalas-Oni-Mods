using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KnastoronOniMods
{
    class AiBrainConfig : IEntityConfig
    {
        public const string ID = "AiBrain";
        public const string NAME = "Ai Brain";

        public static string ROVER_BASE_TRAIT_ID = "AIBrainBaseTrait";
        public const string DESCR = "A brain of not disclosed origin, somehow it knows how to fly this bucket";
        public const int MAXIMUM_TECH_CONSTRUCTION_TIER = 2;
        public const float MASS = 100f;
        private const float WIDTH = 1f;
        private const float HEIGHT = 2f;

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY; 
        public void OnPrefabInit(GameObject prefab)
        {
        }
        public void  OnSpawn(GameObject inst)
        {
            inst.GetSMI<CreatureFallMonitor.Instance>().anim = "idle_loop";
        }
        public GameObject CreatePrefab()
        {
            GameObject scout = AiBrainConfig.CreateBrain();
            return scout;
        }
        public static GameObject CreateBrain()
        {
            GameObject basicEntity = EntityTemplates.CreateBasicEntity("AiBrain", "AI Brain", DESCR, 100f, true, Assets.GetAnim((HashedString)"scout_bot_kanim"), "idle_loop", Grid.SceneLayer.Creatures);
            KBatchedAnimController component1 = basicEntity.GetComponent<KBatchedAnimController>();
            component1.isMovable = true;
            basicEntity.AddOrGet<Modifiers>();
            basicEntity.AddOrGet<LoopingSounds>();
            KBoxCollider2D kboxCollider2D = basicEntity.AddOrGet<KBoxCollider2D>();
            kboxCollider2D.size = new Vector2(1f, 2f);
            kboxCollider2D.offset = (Vector2)new Vector2f(0.0f, 1f);
            Modifiers component2 = basicEntity.GetComponent<Modifiers>();
            component2.initialAmounts.Add(Db.Get().Amounts.HitPoints.Id);
            component2.initialAmounts.Add(Db.Get().Amounts.InternalBattery.Id);
            component2.initialAttributes.Add(Db.Get().Attributes.Construction.Id);
            component2.initialAttributes.Add(Db.Get().Attributes.Digging.Id);
            component2.initialAttributes.Add(Db.Get().Attributes.CarryAmount.Id);
            component2.initialAttributes.Add(Db.Get().Attributes.Machinery.Id);
            component2.initialAttributes.Add(Db.Get().Attributes.Athletics.Id);
            ChoreGroup[] disabled_chore_groups = new ChoreGroup[12]
            {
      Db.Get().ChoreGroups.Basekeeping,
      Db.Get().ChoreGroups.Cook,
      Db.Get().ChoreGroups.Art,
      Db.Get().ChoreGroups.Dig,
      Db.Get().ChoreGroups.Farming,
      Db.Get().ChoreGroups.Ranching,
      Db.Get().ChoreGroups.Build,
      Db.Get().ChoreGroups.MedicalAid,
      Db.Get().ChoreGroups.Combat,
      Db.Get().ChoreGroups.LifeSupport,
      Db.Get().ChoreGroups.Recreation,
      Db.Get().ChoreGroups.Toggle
            };
            basicEntity.AddOrGet<Traits>();
            Trait trait = Db.Get().CreateTrait(AiBrainConfig.ROVER_BASE_TRAIT_ID, (string)STRINGS.ROBOTS.MODELS.SCOUT.NAME, (string)STRINGS.ROBOTS.MODELS.SCOUT.NAME, (string)null, false, disabled_chore_groups, true, true);
            trait.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, TUNING.ROBOTS.SCOUTBOT.ATHLETICS, (string)STRINGS.ROBOTS.MODELS.SCOUT.NAME));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, TUNING.ROBOTS.SCOUTBOT.HIT_POINTS, (string)STRINGS.ROBOTS.MODELS.SCOUT.NAME));
            trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.maxAttribute.Id, 9000f,"tba."));
            trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.deltaAttribute.Id, -17.14286f, "tba."));
            component2.initialTraits.Add(AiBrainConfig.ROVER_BASE_TRAIT_ID);
            basicEntity.AddOrGet<AttributeConverters>();
            GridVisibility gridVisibility = basicEntity.AddOrGet<GridVisibility>();
            gridVisibility.radius = 30;
            gridVisibility.innerRadius = 20f;
            basicEntity.AddOrGet<Worker>(); //RocketPiloting1
            basicEntity.AddOrGet<Effects>();
            basicEntity.AddOrGet<Traits>();
            basicEntity.AddOrGet<AnimEventHandler>();
            basicEntity.AddOrGet<Health>();
            MoverLayerOccupier moverLayerOccupier = basicEntity.AddOrGet<MoverLayerOccupier>();
            moverLayerOccupier.objectLayers = new ObjectLayer[2]
            {
      ObjectLayer.Rover,
      ObjectLayer.Mover
            };
            moverLayerOccupier.cellOffsets = new CellOffset[2]
            {
      CellOffset.none,
      new CellOffset(0, 1)
            };
            RobotBatteryMonitor.Def def = basicEntity.AddOrGetDef<RobotBatteryMonitor.Def>();
            def.batteryAmountId = Db.Get().Amounts.InternalBattery.Id;
            def.canCharge = true;
            def.lowBatteryWarningPercent = 0.2f;
            basicEntity.AddOrGetDef<CreatureDebugGoToMonitor.Def>();
            basicEntity.AddOrGetDef<RobotAi.Def>();
            ChoreTable.Builder chore_table = new ChoreTable.Builder().Add((StateMachine.BaseDef)new RobotDeathStates.Def()).Add((StateMachine.BaseDef)new FallStates.Def()).Add((StateMachine.BaseDef)new DebugGoToStates.Def()).Add((StateMachine.BaseDef)new IdleStates.Def(), forcePriority: Db.Get().ChoreTypes.Idle.priority);
            EntityTemplates.AddCreatureBrain(basicEntity, chore_table, GameTags.Robots.Models.ScoutRover, (string)null);
            basicEntity.AddOrGet<KPrefabID>().RemoveTag(GameTags.CreatureBrain);
            basicEntity.AddOrGet<KPrefabID>().AddTag(GameTags.DupeBrain);
            Navigator navigator = basicEntity.AddOrGet<Navigator>();
            navigator.NavGridName = "RobotNavGrid";
            navigator.CurrentNavType = NavType.Floor;
            navigator.defaultSpeed = 2f;
            navigator.updateProber = true;
            navigator.sceneLayer = Grid.SceneLayer.Creatures;
            basicEntity.AddOrGet<Sensors>();
            basicEntity.AddOrGet<Pickupable>().SetWorkTime(5f);
            basicEntity.AddOrGet<SnapOn>();
            component1.SetSymbolVisiblity((KAnimHashedString)"snapto_pivot", false);
            component1.SetSymbolVisiblity((KAnimHashedString)"snapto_radar", false);
            return basicEntity;
        }
    }
}

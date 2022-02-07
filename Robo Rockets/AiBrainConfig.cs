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
        public void OnPrefabInit(GameObject inst)
        {
            ChoreConsumer component = inst.GetComponent<ChoreConsumer>();
            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                component.AddProvider((ChoreProvider)GlobalChoreProvider.Instance);
        }
        public void OnSpawn(GameObject inst)
        {
            Sensors component1 = inst.GetComponent<Sensors>();
            component1.Add((Sensor)new PathProberSensor(component1));
            component1.Add((Sensor)new PickupableSensor(component1));
            Navigator component2 = inst.GetComponent<Navigator>();
            component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new BipedTransitionLayer(component2, 3.325f, 2.5f));
            component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new DoorTransitionLayer(component2));
            component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new LadderDiseaseTransitionLayer(component2));
            component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new SplashTransitionLayer(component2));
            component2.SetFlags(PathFinder.PotentialPath.Flags.None);
            component2.CurrentNavType = NavType.Floor;
            PathProber component3 = inst.GetComponent<PathProber>();
            if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
                component3.SetGroupProber((IGroupProber)MinionGroupProber.Get());
            Effects effects = inst.GetComponent<Effects>();
            if ((UnityEngine.Object)inst.transform.parent == (UnityEngine.Object)null)
            {
                if (effects.HasEffect("ScoutBotCharging"))
                    effects.Remove("ScoutBotCharging");
            }
            else if (!effects.HasEffect("ScoutBotCharging"))
                effects.Add("ScoutBotCharging", false);
            inst.Subscribe(856640610, (System.Action<object>)(data =>
            {
                if ((UnityEngine.Object)inst.transform.parent == (UnityEngine.Object)null)
                {
                    if (!effects.HasEffect("ScoutBotCharging"))
                        return;
                    effects.Remove("ScoutBotCharging");
                }
                else
                {
                    if (effects.HasEffect("ScoutBotCharging"))
                        return;
                    effects.Add("ScoutBotCharging", false);
                }
            }));
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
            ChoreGroup[] disabled_chore_groups = new ChoreGroup[11]
             {
      Db.Get().ChoreGroups.Basekeeping,
      Db.Get().ChoreGroups.Cook,
      Db.Get().ChoreGroups.Art,
      Db.Get().ChoreGroups.Research,
      Db.Get().ChoreGroups.Farming,
      Db.Get().ChoreGroups.Ranching,
      Db.Get().ChoreGroups.MedicalAid,
      Db.Get().ChoreGroups.Combat,
      Db.Get().ChoreGroups.LifeSupport,
      Db.Get().ChoreGroups.Recreation,
      Db.Get().ChoreGroups.Toggle
             };
            Deconstructable deconstructable = basicEntity.AddOrGet<Deconstructable>();
            deconstructable.enabled = false;
            deconstructable.audioSize = "medium";
            deconstructable.looseEntityDeconstructable = true;
            basicEntity.AddOrGet<Traits>();
            Trait trait = Db.Get().CreateTrait(AiBrainConfig.ROVER_BASE_TRAIT_ID, "a Brain", NAME, (string)null, false, disabled_chore_groups, true, true);
            trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, (string)NAME));
            trait.Add(new AttributeModifier(Db.Get().Attributes.Digging.Id, TUNING.ROBOTS.SCOUTBOT.DIGGING, NAME));
            trait.Add(new AttributeModifier(Db.Get().Attributes.Construction.Id, TUNING.ROBOTS.SCOUTBOT.CONSTRUCTION,NAME));
            trait.Add(new AttributeModifier(Db.Get().Attributes.Athletics.Id, TUNING.ROBOTS.SCOUTBOT.ATHLETICS, (string)NAME));
            trait.Add(new AttributeModifier(Db.Get().Attributes.Machinery.Id, TUNING.ROBOTS.SCOUTBOT.ATHLETICS, (string)NAME));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, TUNING.ROBOTS.SCOUTBOT.HIT_POINTS, (string)NAME));
            trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.maxAttribute.Id, 9000f,"tba."));
            trait.Add(new AttributeModifier(Db.Get().Amounts.InternalBattery.deltaAttribute.Id, -17.14286f, "tba."));
            component2.initialTraits.Add(AiBrainConfig.ROVER_BASE_TRAIT_ID);
            basicEntity.AddOrGet<AttributeConverters>();
            GridVisibility gridVisibility = basicEntity.AddOrGet<GridVisibility>();
            gridVisibility.radius = 30;
            gridVisibility.innerRadius = 20f;
            basicEntity.AddOrGet<Worker>(); //RocketPiloting1
            basicEntity.AddOrGet<Effects>();//Db.Get().SkillPerks.CanUseRocketControlStation
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
            ChoreTable.Builder chore_table = new ChoreTable.Builder().
                Add((StateMachine.BaseDef)new RobotDeathStates.Def())
                .Add((StateMachine.BaseDef)new FallStates.Def())
                .Add((StateMachine.BaseDef)new DebugGoToStates.Def())
                .Add((StateMachine.BaseDef)new IdleStates.Def(), forcePriority: Db.Get().ChoreTypes.Idle.priority);
            EntityTemplates.AddCreatureBrain(basicEntity, chore_table, AiBrain,(string)null);
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
        public static readonly Tag AiBrain = TagManager.Create(nameof(AiBrain));
    }
}

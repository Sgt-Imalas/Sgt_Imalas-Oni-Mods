using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

namespace KnastoronOniMods
{
	class AiBrainConfig : IEntityConfig
	{
		public const string ID = "AiBrain";
		public const string NAME = "Debug.BrainWorker";

		public static string ROVER_BASE_TRAIT_ID = "AIBrainBaseTrait";
		public const string DESCR = "The Worker behind the Control Tasks. If atleast one of these exists, your Rocket should work.";
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
			component2.SetFlags(PathFinder.PotentialPath.Flags.None);
			component2.CurrentNavType = NavType.Floor;
			PathProber component3 = inst.GetComponent<PathProber>();
			if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
				component3.SetGroupProber((IGroupProber)MinionGroupProber.Get());
		}

		public GameObject CreatePrefab()
		{
			GameObject scout = AiBrainConfig.CreateBrain();
			SymbolOverrideControllerUtil.AddToPrefab(scout);
			return scout;
		}
		public static GameObject CreateBrain()
		{
			GameObject basicEntity = EntityTemplates.CreateBasicEntity("AiBrain", "AI Brain", DESCR, 100f, true, Assets.GetAnim((HashedString)"brain_bot_kanim"), "idle", Grid.SceneLayer.Creatures);
			KBatchedAnimController component1 = basicEntity.GetComponent<KBatchedAnimController>();
			component1.isMovable = true;
			component1.SetVisiblity(false);
			basicEntity.AddOrGet<Modifiers>();
			basicEntity.AddOrGet<LoopingSounds>();
			KBoxCollider2D kboxCollider2D = basicEntity.AddOrGet<KBoxCollider2D>();
			kboxCollider2D.size = new Vector2(1f, 1f);
			kboxCollider2D.offset = (Vector2)new Vector2f(0.0f, 0.5f);
			Modifiers component2 = basicEntity.GetComponent<Modifiers>();
			//component2.initialAttributes.Add(Db.Get().Attributes.Construction.Id);
			//component2.initialAttributes.Add(Db.Get().Attributes.Digging.Id);
			component2.initialAttributes.Add(Db.Get().Attributes.CarryAmount.Id);
			component2.initialAttributes.Add(Db.Get().Attributes.Machinery.Id);
			component2.initialAttributes.Add(Db.Get().Attributes.Athletics.Id);
			ChoreGroup[] disabled_chore_groups = new ChoreGroup[]
			 {
	  Db.Get().ChoreGroups.Basekeeping,
	  Db.Get().ChoreGroups.Hauling,
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
			basicEntity.AddOrGet<Traits>();
			KSelectable kselectable = basicEntity.AddOrGet<KSelectable>();
			kselectable.IsSelectable = false; //DEBUG : needs false on release
			Trait trait = Db.Get().CreateTrait(AiBrainConfig.ROVER_BASE_TRAIT_ID, "a Brain", NAME, (string)null, false, disabled_chore_groups, true, true);
			trait.Add(new AttributeModifier(Db.Get().Attributes.CarryAmount.Id, 200f, (string)NAME));
			trait.Add(new AttributeModifier(Db.Get().Attributes.Machinery.Id, TUNING.ROBOTS.SCOUTBOT.ATHLETICS, (string)NAME));
			trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 100f, (string)NAME));
			component2.initialTraits.Add(AiBrainConfig.ROVER_BASE_TRAIT_ID);
			basicEntity.AddOrGet<AttributeConverters>();
			basicEntity.AddOrGet<WorkerBase>(); //RocketPiloting1
			basicEntity.AddOrGet<Effects>();//Db.Get().SkillPerks.CanUseRocketControlStation
			basicEntity.AddOrGet<Traits>();
			basicEntity.AddOrGet<AnimEventHandler>();
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

			Storage storage = basicEntity.AddOrGet<Storage>();
			storage.fxPrefix = Storage.FXPrefix.PickedUp;
			storage.dropOnLoad = true;
			storage.SetDefaultStoredItemModifiers(new List<Storage.StoredItemModifier>()
	{
	  Storage.StoredItemModifier.Preserve,
	  Storage.StoredItemModifier.Seal
	});

			basicEntity.AddOrGetDef<CreatureDebugGoToMonitor.Def>();
			basicEntity.AddOrGetDef<RobotAi.Def>();
			basicEntity.AddOrGet<SelfDestructInWrongEnvironmentComponent>();

			ChoreTable.Builder chore_table = new ChoreTable.Builder()
				.Add((StateMachine.BaseDef)new DebugGoToStates.Def())
				.Add((StateMachine.BaseDef)new IdleStates.Def(), forcePriority: Db.Get().ChoreTypes.Idle.priority);
			EntityTemplates.AddCreatureBrain(basicEntity, chore_table, AiBrain, (string)null);
			basicEntity.AddOrGet<KPrefabID>().RemoveTag(GameTags.CreatureBrain);
			basicEntity.AddOrGet<KPrefabID>().AddTag(GameTags.DupeBrain);

			Navigator navigator = basicEntity.AddOrGet<Navigator>();
			navigator.NavGridName = "RobotNavGrid";
			navigator.CurrentNavType = NavType.Floor;
			navigator.defaultSpeed = 1f;
			navigator.updateProber = true;
			navigator.sceneLayer = Grid.SceneLayer.Creatures;
			basicEntity.AddOrGet<Sensors>();
			basicEntity.AddOrGet<SnapOn>();
			return basicEntity;
		}

		public static readonly Tag AiBrain = TagManager.Create(nameof(AiBrain));
	}
}

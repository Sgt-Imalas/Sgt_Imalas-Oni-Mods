using Klei.AI;
using UnityEngine;

namespace AmogusMorb.TwitchEvents.TwitchEventAddons
{
	internal class CrewMateConfig : IEntityConfig
	{
		public const string ID = "AMOGUS_Twitch_Crewmate";
		public static string BASE_TRAIT_ID = "AMOGUS_Twitch_Crewmate";


		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public GameObject CreatePrefab()
		{
			string name = (string)"Crewmate";
			string desc = (string)"best fwiend";
			string anim = "twitch_imposter_kanim";
			EffectorValues decor = TUNING.DECOR.BONUS.TIER2;
			EffectorValues noise = new EffectorValues();
			GameObject placedEntity = EntityTemplates.CreatePlacedEntity(ID, name, desc, 25f, Assets.GetAnim((HashedString)anim), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
			Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, (string)null, false, (ChoreGroup[])null, true, true).Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 10f, name));
			KPrefabID component = placedEntity.GetComponent<KPrefabID>();
			component.AddTag(GameTags.Creatures.Walker);
			component.AddTag(GameTags.OriginalCreature);
			component.AddTag(GameTags.Amphibious);
			component.prefabInitFn += (KPrefabID.PrefabFn)(inst => inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost));
			EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Friendly, BASE_TRAIT_ID, "RobotNavGrid", onDeathDropCount: 0, warningLowTemperature: 250.15f, warningHighTemperature: 2393.15f, lethalLowTemperature: 200.15f, lethalHighTemperature: 2623.15f);


			placedEntity.AddOrGetDef<ThreatMonitor.Def>();
			placedEntity.AddOrGet<Trappable>();
			placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();

			placedEntity.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
			placedEntity.AddOrGet<LoopingSounds>().updatePosition = true;
			//SoundEventVolumeCache.instance.AddVolume(anim, "Morb_movement_short", NOISE_POLLUTION.CREATURES.TIER2);
			//SoundEventVolumeCache.instance.AddVolume(anim, "Morb_jump", NOISE_POLLUTION.CREATURES.TIER3);
			//SoundEventVolumeCache.instance.AddVolume(anim, "Morb_land", NOISE_POLLUTION.CREATURES.TIER3);
			//SoundEventVolumeCache.instance.AddVolume(anim, "Morb_expel", NOISE_POLLUTION.CREATURES.TIER4);
			EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, false);
			EntityTemplates.AddCreatureBrain(placedEntity,
				new ChoreTable.Builder()
				.Add((StateMachine.BaseDef)new DeathStates.Def())
				.Add((StateMachine.BaseDef)new BaggedStates.Def())
				.Add((StateMachine.BaseDef)new FallStates.Def())
				.Add((StateMachine.BaseDef)new StunnedStates.Def())
				.Add((StateMachine.BaseDef)new DrowningStates.Def())
				.Add((StateMachine.BaseDef)new DebugGoToStates.Def())
				.Add((StateMachine.BaseDef)new DropElementStates.Def())
				.Add(new FleeStates.Def())
				.Add((StateMachine.BaseDef)new IdleStates.Def()), GameTags.Creatures.Species.GlomSpecies, (string)null);


			Navigator navigator = placedEntity.AddOrGet<Navigator>();
			navigator.CurrentNavType = NavType.Floor;
			navigator.defaultSpeed = 2f;
			navigator.updateProber = true;

			return placedEntity;
		}


		public void OnPrefabInit(GameObject inst)
		{

		}

		public void OnSpawn(GameObject inst)
		{
			//inst.AddOrGet<CrewColoursSetter>();
			Navigator component2 = inst.AddOrGet<Navigator>();
			component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new BipedTransitionLayer(component2, 3.325f, 2.5f));
			component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new DoorTransitionLayer(component2));
			component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new LadderDiseaseTransitionLayer(component2));
			component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new SplashTransitionLayer(component2));
			component2.SetFlags(PathFinder.PotentialPath.Flags.None);
			component2.CurrentNavType = NavType.Floor;
		}

	}
}

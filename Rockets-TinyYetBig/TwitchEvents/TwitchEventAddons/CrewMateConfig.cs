using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace Rockets_TinyYetBig.TwitchEvents.TwitchEventAddons
{
    internal class CrewMateConfig : IEntityConfig
    {
        public const string ID = "RTB_Twitch_Crewmate";
        public static string imposta_trait = "RTB_Twitch_Crewmate";
        public const float MASS = 100f;
        private const float WIDTH = 1f;
        private const float HEIGHT = 1f;


        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public GameObject CreatePrefab()
        {
            string name1 = (string)"Crewmate";
            string name2 = name1;
            string desc = (string)"best fwiend";
            EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER2;
            string anim = "twitch_imposter_kanim";
            EffectorValues decor = tieR0;
            EffectorValues noise = new EffectorValues();
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity(ID, name2, desc, 25f, Assets.GetAnim((HashedString)anim), "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
            Db.Get().CreateTrait(imposta_trait, name1, name1, (string)null, false, (ChoreGroup[])null, true, true).Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 10f, name1));
            KPrefabID component = placedEntity.GetComponent<KPrefabID>();
            component.AddTag(GameTags.Creatures.Walker);
            component.AddTag(GameTags.OriginalCreature);
            component.AddTag(GameTags.Amphibious);
            component.prefabInitFn += (KPrefabID.PrefabFn)(inst => inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost));
            EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Friendly, imposta_trait, "RobotNavGrid", onDeathDropCount: 0, warningLowTemperature: 250.15f, warningHighTemperature: 2393.15f, lethalLowTemperature: 200.15f, lethalHighTemperature: 2623.15f);
            
            placedEntity.AddOrGetDef<ThreatMonitor.Def>();
            placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();

            placedEntity.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
            placedEntity.AddOrGet<LoopingSounds>().updatePosition = true;
            SoundEventVolumeCache.instance.AddVolume(anim, "Morb_movement_short", NOISE_POLLUTION.CREATURES.TIER2);
            SoundEventVolumeCache.instance.AddVolume(anim, "Morb_jump", NOISE_POLLUTION.CREATURES.TIER3);
            SoundEventVolumeCache.instance.AddVolume(anim, "Morb_land", NOISE_POLLUTION.CREATURES.TIER3);
            SoundEventVolumeCache.instance.AddVolume(anim, "Morb_expel", NOISE_POLLUTION.CREATURES.TIER4);
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

            inst.AddOrGet<CrewColoursSetter>();
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

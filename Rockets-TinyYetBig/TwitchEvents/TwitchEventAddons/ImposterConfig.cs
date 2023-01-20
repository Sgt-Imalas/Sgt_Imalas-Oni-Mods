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
    internal class ImposterConfig : IEntityConfig
    {
        public const string ID = "RTB_Twitch_Imposter";
        public static string ROVER_BASE_TRAIT_ID = "RTB_Twitch_ImposterBaseTrait";
        public const int MAXIMUM_TECH_CONSTRUCTION_TIER = 2;
        public const float MASS = 100f;
        private const float WIDTH = 1f;
        private const float HEIGHT = 1f;


        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public GameObject CreatePrefab()
        {
            string name1 = (string)"Imposter";
            string name2 = name1;
            string desc = (string)"not sus at all";
            EffectorValues tieR0 = TUNING.DECOR.BONUS.TIER0;
            KAnimFile anim = Assets.GetAnim((HashedString)"glom_kanim");
            EffectorValues decor = tieR0;
            EffectorValues noise = new EffectorValues();
            GameObject placedEntity = EntityTemplates.CreatePlacedEntity("Glom", name2, desc, 25f, anim, "idle_loop", Grid.SceneLayer.Creatures, 1, 1, decor, noise);
            Db.Get().CreateTrait(ROVER_BASE_TRAIT_ID, name1, name1, (string)null, false, (ChoreGroup[])null, true, true).Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name1));
            KPrefabID component = placedEntity.GetComponent<KPrefabID>();
            component.AddTag(GameTags.Creatures.Walker);
            component.AddTag(GameTags.OriginalCreature);
            component.prefabInitFn += (KPrefabID.PrefabFn)(inst => inst.GetAttributes().Add(Db.Get().Attributes.MaxUnderwaterTravelCost));
            EntityTemplates.ExtendEntityToBasicCreature(placedEntity, FactionManager.FactionID.Hostile, ROVER_BASE_TRAIT_ID, onDeathDropID: "", onDeathDropCount: 0, warningLowTemperature: 250.15f, warningHighTemperature: 393.15f, lethalLowTemperature: 200.15f, lethalHighTemperature: 423.15f);
            placedEntity.AddWeapon(1f, 1f);
            placedEntity.AddOrGet<Trappable>();
            placedEntity.AddOrGetDef<ThreatMonitor.Def>();
            placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();

            placedEntity.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = 0;
            placedEntity.AddOrGet<LoopingSounds>();
            placedEntity.GetComponent<LoopingSounds>().updatePosition = true;
            SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_movement_short", NOISE_POLLUTION.CREATURES.TIER2);
            SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_jump", NOISE_POLLUTION.CREATURES.TIER3);
            SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_land", NOISE_POLLUTION.CREATURES.TIER3);
            SoundEventVolumeCache.instance.AddVolume("glom_kanim", "Morb_expel", NOISE_POLLUTION.CREATURES.TIER4);
            EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, false);
            EntityTemplates.AddCreatureBrain(placedEntity,
                new ChoreTable.Builder()
                .Add((StateMachine.BaseDef)new DeathStates.Def()).
                Add((StateMachine.BaseDef)new TrappedStates.Def())
                .Add((StateMachine.BaseDef)new BaggedStates.Def())
                .Add((StateMachine.BaseDef)new FallStates.Def())
                .Add((StateMachine.BaseDef)new StunnedStates.Def()).
                Add((StateMachine.BaseDef)new DrowningStates.Def())
                .Add((StateMachine.BaseDef)new DebugGoToStates.Def())
                .Add((StateMachine.BaseDef)new FleeStates.Def())
                .Add((StateMachine.BaseDef)new DropElementStates.Def())
                .Add((StateMachine.BaseDef)new AttackStates.Def())
                .Add((StateMachine.BaseDef)new IdleStates.Def()), GameTags.Creatures.Species.GlomSpecies, (string)null);
            placedEntity.AddTag(GameTags.Amphibious);
            return placedEntity;
        }


        public void OnPrefabInit(GameObject inst)
        {
            ChoreConsumer component = inst.GetComponent<ChoreConsumer>();
            if ((UnityEngine.Object)component != (UnityEngine.Object)null)
                component.AddProvider((ChoreProvider)GlobalChoreProvider.Instance);
            AmountInstance amountInstance = Db.Get().Amounts.InternalChemicalBattery.Lookup(inst);
            amountInstance.value = amountInstance.GetMax();
        }

        public void OnSpawn(GameObject inst)
        {
            Sensors component1 = inst.GetComponent<Sensors>();
            component1.Add((Sensor)new PathProberSensor(component1));
            Navigator component2 = inst.GetComponent<Navigator>();
            component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new BipedTransitionLayer(component2, 4.5f, 4.5f));
            component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new DoorTransitionLayer(component2));
            component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new LadderDiseaseTransitionLayer(component2));
            component2.transitionDriver.overrideLayers.Add((TransitionDriver.OverrideLayer)new SplashTransitionLayer(component2));
            component2.SetFlags(PathFinder.PotentialPath.Flags.None);
            component2.CurrentNavType = NavType.Floor;
            PathProber component3 = inst.GetComponent<PathProber>();
            if ((UnityEngine.Object)component3 != (UnityEngine.Object)null)
                component3.SetGroupProber((IGroupProber)MinionGroupProber.Get());
            Effects effects = inst.GetComponent<Effects>();
        }

        public struct LaserEffect
        {
            public string id;
            public string animFile;
            public string anim;
            public HashedString context;
        }
    }
}

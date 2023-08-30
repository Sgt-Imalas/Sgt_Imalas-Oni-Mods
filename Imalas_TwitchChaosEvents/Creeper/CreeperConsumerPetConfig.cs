using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Creeper
{
    internal class CreeperConsumerPetConfig// : IEntityConfig
    {
        public const string ID = "ITCE_CREEPERCONSUMER";
        public const string BASE_TRAIT_ID = "ITCE_CREEPERCONSUMER_BASETRAIT";
        public const int WIDTH = 3;
        public const int HEIGHT = 3;

        public GameObject CreatePrefab()
        {
            var placedEntity = EntityTemplates.CreatePlacedEntity(
                ID,
                STRINGS.CREATURES.SPECIES.ITCE_CREEPEREATER.NAME,
                STRINGS.CREATURES.SPECIES.ITCE_CREEPEREATER.DESC,
                10000f,
                Assets.GetAnim("creeper_eater_kanim"),
                "idle_loop",
                Grid.SceneLayer.Creatures,
                WIDTH,
                HEIGHT,
                TUNING.DECOR.BONUS.TIER5);

            EntityTemplates.ExtendEntityToBasicCreature(
                 placedEntity, FactionManager.FactionID.Friendly,
                  BASE_TRAIT_ID,
                  "FloaterNavGrid",
                  NavType.Hover,                  
                  drownVulnerable: false,
                  entombVulnerable: false,
                  warningLowTemperature: 288.15f,
                  warningHighTemperature: 343.15f,
                  lethalHighTemperature: 373.15f);

            placedEntity.AddOrGet<SymbolOverrideController>().ApplySymbolOverridesByAffix(Assets.GetAnim((HashedString)"creeper_eater_kanim"), "hot_");
            placedEntity.AddOrGet<Trappable>();
            placedEntity.AddOrGet<LoopingSounds>();
            placedEntity.AddOrGetDef<CreatureFallMonitor.Def>();

            var def = placedEntity.AddOrGetDef<ThreatMonitor.Def>();
            def.fleethresholdState = Health.HealthState.Dead;
            def.friendlyCreatureTags = new[]
            {
                GameTags.Creatures.CrabFriend
            };

            EntityTemplates.CreateAndRegisterBaggedCreature(placedEntity, true, true);

            var kPrefabId = placedEntity.GetComponent<KPrefabID>();
            kPrefabId.AddTag(GameTags.Creatures.Hoverer);
            kPrefabId.AddTag(GameTags.Creatures.CrabFriend);
            kPrefabId.AddTag(GameTags.Amphibious);

            ConfigureBrain(placedEntity);
            ConfigureTraits();
            //ConfigureDiet(placedEntity);

            //if (placedEntity.TryGetComponent(out Butcherable butcherable))
            //{
            //    var meats = 30;
            //    var shells = 40;

            //    var drops = new List<string>();

            //    for (int i = 0; i < meats; i++)
            //        drops.Add(ShellfishMeatConfig.ID);

            //    for (int i = 0; i < shells; i++)
            //        drops.Add(CrabShellConfig.ID);

            //    butcherable.SetDrops(drops.ToArray());
            //}

            placedEntity.AddComponent<CreeperConverter>();
            placedEntity.AddOrGet<OilFloaterMovementSound>().sound = "OilFloaterBaby_move_LP";

            return placedEntity;
        }

        //private void ConfigureDiet(GameObject placedEntity)
        //{
        //    var dietInfo = BaseCrabConfig.BasicDiet(
        //        SimHashes.Sand.CreateTag(),
        //        CALORIES_PER_KG_OF_ORE,
        //        CREATURES.CONVERSION_EFFICIENCY.NORMAL,
        //        null,
        //        0.0f);

        //    BaseCrabConfig.SetupDiet(
        //        placedEntity,
        //        dietInfo,
        //        CALORIES_PER_KG_OF_ORE,
        //        MIN_POOP_SIZE_IN_KG);
        //}

        private void ConfigureTraits()
        {
            string name = STRINGS.CREATURES.SPECIES.ITCE_CREEPEREATER.NAME;
            var trait = Db.Get().CreateTrait(
                BASE_TRAIT_ID,
                name,
                name,
                null,
                false,
                null,
                true,
                true);

            var amounts = Db.Get().Amounts;
            //trait.Add(new AttributeModifier(amounts.Calories.maxAttribute.Id, CrabTuning.STANDARD_STOMACH_SIZE * 10f, name));
            //trait.Add(new AttributeModifier(amounts.Calories.deltaAttribute.Id, (float)(-STANDARD_CALORIES_PER_CYCLE / Consts.CYCLE_LENGTH), name));
            trait.Add(new AttributeModifier(amounts.HitPoints.maxAttribute.Id, 666f, name));
            trait.Add(new AttributeModifier(amounts.Age.maxAttribute.Id, float.PositiveInfinity, name));
            //trait.Add(new AttributeModifier(Db.Get().CritterAttributes.Metabolism.Id, 100f, name));
            trait.Add(new AttributeModifier(Db.Get().CritterAttributes.Happiness.Id, 50f, name));
        }

        private void ConfigureBrain(GameObject prefab)
        {
            var choreTable = new ChoreTable.Builder()
                .Add(new DeathStates.Def())
                .Add(new AnimInterruptStates.Def())
                .Add(new TrappedStates.Def())
                .Add(new BaggedStates.Def())
                .Add(new FallStates.Def())
                .Add(new StunnedStates.Def())
                .Add(new DebugGoToStates.Def())
                .Add(new FleeStates.Def())
                .PushInterruptGroup()
                .Add(new CreatureSleepStates.Def())
                .Add(new FixedCaptureStates.Def())
                //.Add(new EatStates.Def())
                //.Add(new PlayAnimsStates.Def(
                //    GameTags.Creatures.Poop,
                //    false,
                //    "poop",
                //    global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.NAME,
                //    global::STRINGS.CREATURES.STATUSITEMS.EXPELLING_SOLID.TOOLTIP))
                .PopInterruptGroup()
                .Add(new IdleStates.Def());

            EntityTemplates.AddCreatureBrain(prefab, choreTable, GameTags.Creatures.Species.OilFloaterSpecies, "");
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst) { }

        public void OnSpawn(GameObject inst)
        {
            inst.TryGetComponent(out KBatchedAnimController kbac);
            kbac.animScale *= 1f;
        }
    }
}

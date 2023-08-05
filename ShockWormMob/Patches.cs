using Database;
using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Core;
using ShockWormMob.OreDeposits;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static ShockWormMob.ModAssets;

namespace ShockWormMob
{
    internal class Patches
    {
        /// <summary>
     /// Test of old Decomposer component
     /// </summary>
        [HarmonyPatch(typeof(RationalAi.Instance))]
        [HarmonyPatch(nameof(RationalAi.Instance.RefreshUserMenu))]
        public static class AddRot
        {

            [HarmonyPriority(Priority.VeryLow)]
            public static void Postfix(RationalAi.Instance __instance)
            {
                if (__instance.master.gameObject.HasTag(GameTags.Dead))
                    __instance.master.gameObject.AddOrGet<Decomposer>();
            }
        }
        [HarmonyPatch(typeof(Db))]
        [HarmonyPatch(nameof(Db.Initialize))]
        public static class MagmaDrink
        {

            [HarmonyPriority(Priority.Low)]
            public static void Postfix()
            {
                WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS = WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS.AddToArray(new Tuple<Tag, string>(SimHashes.Magma.CreateTag(), "HotStuff"));
                WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS = WaterCoolerConfig.BEVERAGE_CHOICE_OPTIONS.AddToArray(new Tuple<Tag, string>(SimHashes.Oxygen.CreateTag(), "VerticalWindTunnel"));
            }
        }

        [HarmonyPatch(typeof(RoleStation))]
        [HarmonyPatch(nameof(RoleStation.OnStopWork))]
        public static class FixCrash
        {

            public static bool Prefix(RoleStation __instance)
            {
                Telepad.StatesInstance sMI = __instance.GetSMI<Telepad.StatesInstance>();
                return sMI != null;
            }
        }


        [HarmonyPatch(typeof(MainMenu))]
        [HarmonyPatch(nameof(MainMenu.OnSpawn))]
        public static class patch1
        {

            [HarmonyPriority(Priority.Low)]
            public static void Prefix(RoleStation __instance)
            {
                SgtLogger.l("I have Prio Low");
            }
        }
        [HarmonyPatch(typeof(MainMenu))]
        [HarmonyPatch(nameof(MainMenu.OnSpawn))]
        public static class patch4
        {

            [HarmonyPriority(Priority.High)]
            public static void Prefix(RoleStation __instance)
            {
                SgtLogger.l("I have Prio high");
            }
        }
        [HarmonyPatch(typeof(MainMenu))]
        [HarmonyPatch(nameof(MainMenu.OnSpawn))]
        public static class patch2
        {

            [HarmonyPriority(Priority.Last)]
            public static void Prefix(RoleStation __instance)
            {
                SgtLogger.l("I have Prio Last");
            }
        }
        [HarmonyPatch(typeof(MainMenu))]
        [HarmonyPatch(nameof(MainMenu.OnSpawn))]
        public static class patch3
        {

            [HarmonyPriority(Priority.First)]
            public static void Prefix(RoleStation __instance)
            {
                SgtLogger.l("I have Prio First");
            }
        }




        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                //InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);

                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, MinerSolidMk1Config.ID);
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Utilities, EventDebugTileConfig.ID);
                InjectionMethods.AddBuildingToPlanScreenBehindNext(GameStrings.PlanMenuCategory.Base, JobBoardConfig.ID, ExobaseHeadquartersConfig.ID);
                ModAssets.InitEventTest(); 
            }
        }
        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }

        [HarmonyPatch(typeof(ShockwormConfig), "CreatePrefab")]
        public static class Make_Creature
        {
            public static void Postfix(GameObject __result)
            {

                __result.AddOrGetDef<OvercrowdingMonitor.Def>().spaceRequiredPerCreature = CREATURES.SPACE_REQUIREMENTS.TIER1;
                ChoreTable.Builder chore_table =
                    new ChoreTable.Builder()
                    .Add((StateMachine.BaseDef)new DeathStates.Def())
                    .Add((StateMachine.BaseDef)new AnimInterruptStates.Def())
                    .Add((StateMachine.BaseDef)new TrappedStates.Def())
                    .Add((StateMachine.BaseDef)new BaggedStates.Def())
                    .Add((StateMachine.BaseDef)new StunnedStates.Def())
                    .Add((StateMachine.BaseDef)new FallStates.Def())
                    .Add((StateMachine.BaseDef)new DebugGoToStates.Def())
                    .Add((StateMachine.BaseDef)new DrowningStates.Def())
                    .Add((StateMachine.BaseDef)new AttackStates.Def("attack_loop", "attack_pst"))
                    .Add((StateMachine.BaseDef)new IdleStates.Def());
                EntityTemplates.AddCreatureBrain(__result, chore_table, ShockwormConfigSpeciesID, "");
                KPrefabID component = __result.GetComponent<KPrefabID>(); 
                component.AddTag(GameTags.Creatures.Flyer);

                __result.AddOrGetDef<ThreatMonitor.Def>();
                __result.AddOrGetDef<AgeMonitor.Def>();

                Trait trait = Db.Get().CreateTrait("ShockWormBaseBaseTrait", "Shock Worm", "Shock Worm", (string)null, false, (ChoreGroup[])null, true, true);
                trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 15f, "Shock Worm"));
                trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 25f, "Shock Worm"));

                Modifiers modifiers = __result.AddOrGet<Modifiers>();
                modifiers.initialTraits.Add("ShockWormBaseBaseTrait");
                SoundEventVolumeCache.instance.AddVolume("shockworm_kanim", "attack_loop", NOISE_POLLUTION.CREATURES.TIER6);

            }
        }
        [HarmonyPatch(typeof(CodexEntryGenerator), "GenerateCreatureEntries")]
        public class CodexEntryGenerator_GenerateCreatureEntries_Patch
        {
            internal delegate void GenerateImageContainers(Sprite[] sprites,
            List<ContentContainer> containers, ContentContainer.ContentLayout layout);

            internal delegate void GenerateCreatureDescriptionContainers(GameObject creature,
            List<ContentContainer> containers);

            internal static readonly GenerateImageContainers GENERATE_IMAGE_CONTAINERS =
            typeof(CodexEntryGenerator).CreateStaticDelegate<GenerateImageContainers>(
            nameof(GenerateImageContainers), typeof(Sprite[]), typeof(List<ContentContainer>),
            typeof(ContentContainer.ContentLayout));


            internal static readonly GenerateCreatureDescriptionContainers GENERATE_DESC =
                typeof(CodexEntryGenerator).CreateStaticDelegate<GenerateCreatureDescriptionContainers>(
                nameof(GenerateCreatureDescriptionContainers), typeof(GameObject),
                typeof(List<ContentContainer>));

            private static void AddToCodex(Tag speciesTag, string name,
                IDictionary<string, CodexEntry> results)
            {
                string tagStr = speciesTag.ToString();
                var brains = Assets.GetPrefabsWithComponent<CreatureBrain>();
                var entry = new CodexEntry(nameof(global::STRINGS.CREATURES), new List<ContentContainer> {
                new ContentContainer(new List<ICodexWidget> {
                    new CodexSpacer(),
                    new CodexSpacer()
                }, ContentContainer.ContentLayout.Vertical)
            }, name)
                {
                    parentId = nameof(global::STRINGS.CREATURES)
                };
                CodexCache.AddEntry(tagStr, entry);
                results.Add(tagStr, entry);
                // Find all critters with this tag
                foreach (var prefab in brains)
                    if (prefab.GetDef<BabyMonitor.Def>() == null && prefab.TryGetComponent(
                            out CreatureBrain brain) && brain.species == speciesTag)
                    {
                        Sprite babySprite = null;
                        string prefabID = prefab.PrefabID().Name;
                        var baby = Assets.TryGetPrefab(prefabID + "Baby");
                        var contentContainerList = new List<ContentContainer>(4);
                        var first = Def.GetUISprite(prefab, brain.symbolPrefix + "ui").first;
                        if (baby != null)
                            babySprite = Def.GetUISprite(baby).first;
                        if (babySprite != null)
                            GENERATE_IMAGE_CONTAINERS.Invoke(new[] { first, babySprite },
                                contentContainerList, ContentContainer.ContentLayout.Horizontal);
                        else
                            contentContainerList.Add(new ContentContainer(new List<ICodexWidget> {
                            new CodexImage(128, 128, first)
                        }, ContentContainer.ContentLayout.Vertical));
                        GENERATE_DESC.Invoke(prefab, contentContainerList);
                        entry.subEntries.Add(new SubEntry(prefabID, tagStr, contentContainerList,
                                prefab.GetProperName())
                        {
                            icon = first,
                            iconColor = Color.white
                        });
                    }
            }
            public static void Postfix(Dictionary<string, CodexEntry> __result)
            {
                AddToCodex(ShockwormConfigSpeciesID, "Shock worms", __result);
                AddToCodex(GroneHogSpeciesID, "Volgus", __result);
            }
        }

    }
}

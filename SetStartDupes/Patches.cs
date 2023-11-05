using Database;
using Epic.OnlineServices;
using HarmonyLib;
using Klei.AI;
using ProcGen.Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using UtilLibs;
using static FetchManager;
using static KAnim;
using static KCompBuilder;
using static SetStartDupes.DupeTraitManager;
using static SetStartDupes.ModAssets;
using static SetStartDupes.STRINGS.UI;
using static STRINGS.DUPLICANTS;
using static STRINGS.DUPLICANTS.CHORES;
using static STRINGS.UI.DETAILTABS;
using static UnityEngine.GraphicsBuffer;

namespace SetStartDupes
{
    class Patches
    {
        [HarmonyPatch(typeof(Traits))]
        [HarmonyPatch(nameof(Traits.OnSpawn))]
        public class FixDupesWithoutDupeTrait
        {
            public static void Postfix(Traits __instance)
            {
                if (__instance.TryGetComponent<MinionIdentity>(out _)
                    && __instance.TryGetComponent<Health>(out var helt)
                    && helt.hitPoints == 0
                    && Db.Get().Amounts.Calories.Lookup(__instance.gameObject) != null
                    && Db.Get().Amounts.Calories.Lookup(__instance.gameObject).value == 0
                    )
                {
                    SgtLogger.l("Someone was ded on arrival, fixing that...");

                    helt.hitPoints = 100f;
                    helt.State = Health.HealthState.Perfect;

                    var cals = Db.Get().Amounts.Calories.Lookup(__instance.gameObject);
                    cals.SetValue(3550000);
                }
            }
        }




        [HarmonyPatch(typeof(CryoTank))]
        [HarmonyPatch(nameof(CryoTank.DropContents))]
        //[HarmonyPatch(nameof(CryoTank.OnSidescreenButtonPressed))] 
        public class AddToCryoTank
        {
            public static void Prefix()
            {
                if (ModConfig.Instance.JorgeAndCryopodDupes)
                {
                    ModAssets.EditingSingleDupe = true;
                    ImmigrantScreen.InitializeImmigrantScreen(null);
                }
            }
            public static void Postfix(CryoTank __instance)
            {
                if (ModConfig.Instance.JorgeAndCryopodDupes)
                {
                    SgtLogger.l("Getting CryoDupe gameobject");
                    CryoDupeToApplyStatsOn = __instance.smi.sm.defrostedDuplicant.Get(__instance.smi);
                }
            }
        }
        [HarmonyPatch(typeof(CharacterContainer))]
        [HarmonyPatch(nameof(CharacterContainer.GenerateCharacter))]
        public class OverwriteRngGeneration
        {
            public static bool Prefix(CharacterContainer __instance, KButton ___selectButton)
            {
                if (ModAssets.EditingSingleDupe)
                {
                    SgtLogger.l("editingSingleDupe");

                    if (CryoDupeToApplyStatsOn != null
                        && CryoDupeToApplyStatsOn.TryGetComponent<MinionIdentity>(out var minionIdentity)
                        && Db.Get().Personalities.Get(minionIdentity.personalityResourceId) != null)
                    {
                        var originPersonality = Db.Get().Personalities.Get(minionIdentity.personalityResourceId);
                        __instance.stats = new MinionStartingStats(originPersonality, guaranteedTraitID: "AncientKnowledge");
                        //ModAssets.ApplySkinFromPersonality(originPersonality, __instance.stats);
                        //__instance.characterNameTitle.OnEndEdit(originPersonality.Name);
                    }
                    else
                    {
                        __instance.stats = new MinionStartingStats(is_starter_minion: false, guaranteedTraitID: "AncientKnowledge");
                    }
                    if (EditingJorge)
                    {
                        Trait chatty = Db.Get().traits.TryGet("Chatty");
                        if (chatty != null)
                        {
                            __instance.stats.Traits.Add(chatty);
                        }
                    }
                    __instance.stats.voiceIdx = ModApi.GetVoiceIdxOverrideForPersonality(__instance.stats.NameStringKey);

                    //Trait ancientKnowledgeTrait = Db.Get().traits.TryGet("AncientKnowledge");
                    //if (ancientKnowledgeTrait != null)
                    //{
                    //   // __instance.stats.Traits.Add(ancientKnowledgeTrait);
                    //}
                    __instance.SetReshufflingState(true);
                    __instance.SetAnimator();
                    __instance.SetInfoText();
                    __instance.StartCoroutine(__instance.SetAttributes());
                    ___selectButton.ClearOnClick();
                    ___selectButton.interactable = false;
                    SgtLogger.l(__instance.stats.Name + " <- cryopod dupe");
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(CharacterContainer))]
        [HarmonyPatch(nameof(CharacterContainer.GenerateCharacter))]
        public class ApplyCrewPresetIfAvailable
        {
            public static void Postfix(CharacterContainer __instance, KButton ___selectButton)
            {
                if (MinionCrewPreset.OpenPresetAssignments.Count > 0)
                {
                    MinionCrewPreset.ApplySingleMinion(MinionCrewPreset.OpenPresetAssignments.First(), __instance);
                    MinionCrewPreset.OpenPresetAssignments.RemoveAt(0);
                }
            }
        }
        [HarmonyPatch(typeof(MinionStartingStats))]
        [HarmonyPatch(nameof(MinionStartingStats.GenerateStats))]
        public class RecalculateStatBoni
        {
            [HarmonyPriority(Priority.LowerThanNormal)]

            public static void Postfix(MinionStartingStats __instance)
            {
                if (ModAssets.DupeTraitManagers.ContainsKey(__instance))
                    ModAssets.DupeTraitManagers[__instance].RecalculateAll();
                //else
                //SgtLogger.warning("no mng for " + __instance + " found!");
            }
        }



        [HarmonyPatch(typeof(ImmigrantScreen))]
        [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
        public class CustomSingleForNoTelepad
        {
            static GameObject Spacer = null;
            public static bool Prefix(Telepad telepad, ImmigrantScreen __instance)
            {
                EditingSingleDupe = telepad == null;

                if ((EditingSingleDupe && ModConfig.Instance.JorgeAndCryopodDupes) || ModConfig.Instance.RerollDuringGame)
                {
                    if (Spacer == null)
                    {
                        var container = __instance.transform.Find("Layout");
                        var spacer = Util.KInstantiateUI(__instance.transform.Find("Layout/Title").gameObject, container.gameObject, true).rectTransform();

                        spacer.SetSiblingIndex(2);
                        if (spacer.TryGetComponent<LayoutElement>(out var layoutElement))
                        {
                            layoutElement.minHeight = 42;
                        }
                        UIUtils.FindAndDestroy(spacer, "TitleLabel");
                        UIUtils.FindAndDestroy(spacer, "CloseButton");

                        //UIUtils.ListAllChildrenWithComponents(spacer.transform);

                        if (spacer.transform.Find("BG").TryGetComponent<KImage>(out var image))
                        {
                            var ColorStyle = (ColorStyleSetting)ScriptableObject.CreateInstance("ColorStyleSetting");
                            ColorStyle.inactiveColor = UIUtils.rgb(37, 37, 41);
                            ColorStyle.hoverColor = UIUtils.rgb(37, 37, 41);
                            ColorStyle.activeColor = UIUtils.rgb(37, 37, 41);
                            ColorStyle.disabledColor = UIUtils.rgb(37, 37, 41);
                            image.colorStyleSetting = ColorStyle;
                            image.ApplyColorStyleSetting();

                        }
                        Spacer = spacer.gameObject;
                    }
                }
                else
                {
                    if (Spacer != null)
                    {
                        UnityEngine.Object.Destroy(Spacer);
                        Spacer = null;
                    }
                }



                if (EditingSingleDupe)
                {
                    if (__instance.containers != null && __instance.containers.Count > 0)
                    {
                        foreach (var container in __instance.containers)
                        {
                            container.GetGameObject().SetActive(false);
                        }
                    }
                    if (__instance.rejectButton != null)
                    {
                        __instance.rejectButton.gameObject.SetActive(false);
                    }
                    if (__instance.closeButton != null)
                    {
                        __instance.closeButton.gameObject.SetActive(false);
                    }

                    if (SingleCharacterContainer != null && SingleCharacterContainer.gameObject != null)
                    {
                        SingleCharacterContainer.gameObject.SetActive(true);
                    }
                    else
                    {
                        SingleCharacterContainer = Util.KInstantiateUI<CharacterContainer>(__instance.containerPrefab.gameObject, __instance.containerParent, true);
                    }
                    SingleCharacterContainer.SetController(__instance);
                    __instance.EnableProceedButton();

                    return false;
                }
                else
                {
                    if (__instance.containers != null && __instance.containers.Count > 0)
                    {
                        foreach (var container in __instance.containers)
                        {
                            container.GetGameObject().SetActive(true);
                        }
                    }
                    if (__instance.rejectButton != null && __instance.rejectButton.gameObject != null)
                    {
                        __instance.rejectButton.gameObject.SetActive(true);
                    }
                    if (__instance.closeButton != null)
                    {
                        __instance.closeButton.gameObject.SetActive(true);
                    }
                    if (SingleCharacterContainer != null && SingleCharacterContainer.gameObject != null)
                    {
                        SingleCharacterContainer.gameObject.SetActive(false);
                    }
                }

                return true;
            }
            public static void Postfix(ImmigrantScreen __instance)
            {
                ModAssets.ParentScreen = PauseScreen.Instance.transform.parent.gameObject;
            }
        }

        [HarmonyPatch(typeof(ImmigrantScreen))]
        [HarmonyPatch(nameof(ImmigrantScreen.OnProceed))]
        public class SkipTelepadActionsForCryoDupes
        {

            public static bool Prefix(Telepad ___telepad, ImmigrantScreen __instance)
            {
                if (EditingSingleDupe)
                {
                    MinionStartingStats DupeToDeliver = (MinionStartingStats)ModAssets.SingleCharacterContainer.stats;
                    SgtLogger.l(DupeToDeliver.personality.IdHash.ToString(), "resourceID");
                    SgtLogger.l(DupeToDeliver.Name + " <- cryopod dupe´fin");

                    foreach (var trait in DupeToDeliver.Traits)
                        SgtLogger.l(trait.Name, "Trait ToApply");

                    if (CryoDupeToApplyStatsOn != null)
                    {
                        foreach (var trait in CryoDupeToApplyStatsOn.GetComponent<Traits>().GetTraitIds())
                        {
                            SgtLogger.l("purging existing trait: " + trait);
                            PurgingTraitComponentIfExists(trait, CryoDupeToApplyStatsOn);
                        }

                        CryoDupeToApplyStatsOn.GetComponent<Traits>().Clear();


                        MinionResume component = CryoDupeToApplyStatsOn.GetComponent<MinionResume>();

                        var keys = component.AptitudeBySkillGroup.Keys.ToList();
                        for (int i = keys.Count - 1; i >= 0; --i)
                        {
                            var skillAptitude = keys[i];
                            component.SetAptitude(skillAptitude, 0);
                        }

                        if (EditingJorge)
                        {
                            foreach (string key in DUPLICANTSTATS.ALL_ATTRIBUTES)
                                DupeToDeliver.StartingLevels[key] += 7;
                        }


                        DupeToDeliver.Apply(CryoDupeToApplyStatsOn);
                        ///These symbols get overidden at dupe creation, as we are editing already spawned dupes, we have to remove the old overrides and add the new overrides
                        if (CryoDupeToApplyStatsOn.TryGetComponent<SymbolOverrideController>(out var symbolOverride) && CryoDupeToApplyStatsOn.TryGetComponent<Accessorizer>(out var accessorizer))
                        {
                            var headshape_symbolName = (KAnimHashedString)HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.HeadShape).symbol.hash).Replace("headshape", "cheek");
                            var cheek_symbol_snapTo = (HashedString)"snapto_cheek";
                            var hair_symbol_snapTo = (HashedString)"snapto_hair_always";

                            symbolOverride.RemoveSymbolOverride(headshape_symbolName);
                            symbolOverride.RemoveSymbolOverride(cheek_symbol_snapTo);
                            symbolOverride.RemoveSymbolOverride(hair_symbol_snapTo);

                            symbolOverride.AddSymbolOverride(cheek_symbol_snapTo, Assets.GetAnim((HashedString)"head_swap_kanim").GetData().build.GetSymbol((KAnimHashedString)headshape_symbolName), 1);
                            symbolOverride.AddSymbolOverride(hair_symbol_snapTo, accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol, 1);
                            symbolOverride.AddSymbolOverride((HashedString)Db.Get().AccessorySlots.HatHair.targetSymbolId, Db.Get().AccessorySlots.HatHair.Lookup("hat_" + HashCache.Get().Get(accessorizer.GetAccessory(Db.Get().AccessorySlots.Hair).symbol.hash)).symbol, 1);

                        }



                        if (SingleCharacterContainer.gameObject != null)
                        {
                            UnityEngine.Object.Destroy(SingleCharacterContainer.gameObject);
                            SingleCharacterContainer = null;
                        }
                        CryoDupeToApplyStatsOn = null;
                        EditingJorge = false;
                    }


                    //dupe.GetComponent<MinionIdentity>().arrivalTime = UnityEngine.Random.Range(-2000, -1000);


                    __instance.Show(false);
                    //AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
                    //AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
                    //MusicManager.instance.PlaySong("Stinger_NewDuplicant");

                    EditingSingleDupe = false;
                    return false;
                }
                return true;
            }
        }

        [HarmonyPatch(typeof(ImmigrantScreen))]
        [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
        public class AddRerollButtonIfEnabled
        {
            public static void Postfix(Telepad telepad, ImmigrantScreen __instance)
            {
                if (ModConfig.Instance.RerollDuringGame)
                {
                    if (__instance.containers != null && __instance.containers.Count > 0)
                    {
                        foreach (ITelepadDeliverableContainer container in __instance.containers)
                        {
                            CharacterContainer characterContainer = container as CharacterContainer;
                            CarePackageContainer carePackContainer = container as CarePackageContainer;
                            if (characterContainer != null)
                            {
                                characterContainer.SetReshufflingState(true);
                                characterContainer.reshuffleButton.onClick += () =>
                                {
                                    //Prevents multiple selections
                                    characterContainer.controller.RemoveLast();
                                };
                            }
                            if (carePackContainer != null)
                            {
                                carePackContainer.SetReshufflingState(true);
                                carePackContainer.reshuffleButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 20, 120f);
                                carePackContainer.reshuffleButton.onClick += () =>
                                {
                                    carePackContainer.controller.RemoveLast();
                                    carePackContainer.Reshuffle(false);
                                };
                                UIUtils.AddSimpleTooltipToObject(carePackContainer.reshuffleButton.transform, STRINGS.UI.BUTTONS.REROLLCAREPACKAGE, true, onBottom: true);
                            }
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CarePackageContainer))]
        [HarmonyPatch(nameof(CarePackageContainer.OnSpawn))]
        public class CarePackageContainer_Add_SelectPackageButton
        {
            public static void Postfix(CarePackageContainer __instance)
            {
                List<CarePackageInfo> carePackageInfos = null;

                var BioInks_ModApi_Type = Type.GetType("PrintingPodRecharge.ModAPI, PrintingPodRecharge", false, false);
                if (BioInks_ModApi_Type != null)
                {
                    var currentPool = Traverse.Create(BioInks_ModApi_Type).Method("GetCurrentPool").GetValue() as List<CarePackageInfo>;
                    carePackageInfos = currentPool;
                }

                if (carePackageInfos != null)
                    SgtLogger.l("Bio Inks Pool loaded");



                if (__instance.reshuffleButton == null || !ModConfig.Instance.RerollDuringGame)
                    return;

                var selectButton = Util.KInstantiateUI<KButton>(__instance.reshuffleButton.gameObject, __instance.reshuffleButton.transform.parent.gameObject, true);
                selectButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 20, 33f);
                UIUtils.FindAndDestroy(selectButton.transform, "Text");
                if (selectButton.transform.Find("FG").TryGetComponent<Image>(out var image))
                {
                    image.sprite = Assets.GetSprite("icon_gear");
                }

                //UIUtils.ListAllChildren(selectButton.transform);
                selectButton.onClick += () =>
                {
                    UnityCarePackageScreen.ShowWindow(__instance, () => { }, carePackageInfos);
                };
            }
        }


        [HarmonyPatch(typeof(ImmigrantScreen))]
        [HarmonyPatch(nameof(ImmigrantScreen.OnPressBack))]
        public class CatchCryopodDupeException
        {
            public static bool Prefix(ImmigrantScreen __instance)
            {
                return !(__instance.containers == null || __instance.containers.Count == 0);
            }
        }
        [HarmonyPatch(typeof(CharacterContainer))]
        [HarmonyPatch(nameof(CharacterContainer.Reshuffle))]
        public class PreventCrashForSingleDupes
        {

            public static bool Prefix(CharacterContainer __instance, ref bool is_starter)
            {
                is_starter = __instance.controller is MinionSelectScreen;
                if (EditingSingleDupe)
                {
                    if (__instance.fxAnim != null)
                    {
                        __instance.fxAnim.Play("loop");
                    }
                    //SgtLogger.l(__instance.guaranteedAptitudeID, "archetypeID");
                    __instance.GenerateCharacter(is_starter, __instance.guaranteedAptitudeID);
                    return false;
                }
                return true;
            }
        }
        [HarmonyPatch(typeof(DetailsScreen))]
        [HarmonyPatch(nameof(DetailsScreen.OnPrefabInit))]
        public class AddSkinButtonToDetailScreen
        {
            public static GameObject SkinButtonGO = null;
            public static void Postfix(DetailsScreen __instance)
            {
                if (ModConfig.Instance.LiveDupeSkins)
                {
                    SgtLogger.l("adding skin button to detailsScreen");

                    if (SkinButtonGO != null)
                        UnityEngine.Object.Destroy(SkinButtonGO);

                    var SkinButton = Util.KInstantiateUI<KButton>(__instance.ChangeOutfitButton.gameObject, __instance.ChangeOutfitButton.transform.parent.gameObject);
                    //SkinButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 20, 33f);
                    SkinButton.ClearOnClick();
                    SkinButton.name = "DupeSkinButtonSideScreen";
                    UIUtils.AddSimpleTooltipToObject(SkinButton.transform, STRINGS.UI.BUTTONS.DUPESKINBUTTONTOOLTIP, true, onBottom: true);
                    if (SkinButton.transform.Find("Image").TryGetComponent<Image>(out var image))
                    {
                        image.sprite = Assets.GetSprite("ic_dupe");
                    }

                    SkinButton.onClick += () =>
                    {
                        DupeSkinScreenAddon.ShowSkinScreen(null, null, __instance.target);
                    };
                    SkinButtonGO = SkinButton.gameObject;
                    SkinButtonGO.SetActive(false);
                }
            }
        }
        [HarmonyPatch(typeof(DetailsScreen))]
        [HarmonyPatch(nameof(DetailsScreen.SetTitle))]
        [HarmonyPatch(new Type[] { typeof(int) })]
        public class ToggleSkinButtonVisibility
        {
            public static void Prefix(DetailsScreen __instance)
            {
                if (AddSkinButtonToDetailScreen.SkinButtonGO != null && __instance.target != null)
                {
                    AddSkinButtonToDetailScreen.SkinButtonGO.SetActive(__instance.target.GetComponent<MinionIdentity>());
                    //SgtLogger.l("AddSkinButtonToDetailScreen.SkinButtonGO.Active: " + AddSkinButtonToDetailScreen.SkinButtonGO.activeSelf);
                }
                //else
                //    SgtLogger.warning("skin button go was null!");
            }
        }

        [HarmonyPatch(typeof(MinionBrowserScreenConfig))]
        [HarmonyPatch(nameof(MinionBrowserScreenConfig.Personalities))]
        public class AddHiddenPersonalitiesToSkinSelection
        {
            public static void Postfix(ref MinionBrowserScreenConfig __result, Option<Personality> defaultSelectedPersonality = default(Option<Personality>))
            {
                var personalities = Db.Get().Personalities;
                List<MinionBrowserScreen.GridItem> HiddenPersonalityTargets = new List<MinionBrowserScreen.GridItem>();
                SgtLogger.l("Adding hidden personalities to dupe screen");

                foreach (var HiddenPersonalityUnlock in ModApi.HiddenPersonalitiesWithUnlockCondition)
                {
                    SgtLogger.l($"Trying to add {HiddenPersonalityUnlock.Key}");
                    bool isUnlocked = false;
                    try
                    {
                        isUnlocked = HiddenPersonalityUnlock.Value.Invoke();
                    }
                    catch (Exception e)
                    {
                        SgtLogger.error($"unlock condition method for {HiddenPersonalityUnlock.Key} failed to execute!\n\n" + e);
                    }

                    if (isUnlocked)
                    {
                        Personality hiddenPersonality = personalities.GetPersonalityFromNameStringKey(HiddenPersonalityUnlock.Key);
                        if (hiddenPersonality == null)
                        {
                            SgtLogger.warning($"{HiddenPersonalityUnlock.Key} was not found in the database!");
                            continue;
                        }
                        MinionBrowserScreen.GridItem.PersonalityTarget Target = MinionBrowserScreen.GridItem.Of(hiddenPersonality);
                        if (Target == null)
                        {
                            SgtLogger.warning($"no grid item found for {HiddenPersonalityUnlock.Key}!");
                            continue;
                        }
                        HiddenPersonalityTargets.Add(Target);
                        SgtLogger.l($"{HiddenPersonalityUnlock.Key} added");
                    }
                    else
                    {
                        SgtLogger.l($"{HiddenPersonalityUnlock.Key} not unlocked!");
                    }
                }

                HiddenPersonalityTargets.InsertRange(0, __result.items);

                __result = new MinionBrowserScreenConfig(HiddenPersonalityTargets.OrderBy(item => item.GetName()).ToArray(), __result.defaultSelectedItem);
            }
        }


        [HarmonyPatch(typeof(CharacterSelectionController))]
        [HarmonyPatch(nameof(CharacterSelectionController.AddDeliverable))]
        public class CatchErrorLogging
        {
            public static void Prefix(ITelepadDeliverable deliverable, CharacterSelectionController __instance)
            {
                if (!__instance.selectedDeliverables.Contains(deliverable)
                    && __instance.selectedDeliverables.Count >= __instance.selectableCount
                    && __instance.selectableCount > 0
                    )
                {
                    __instance.selectedDeliverables.RemoveAt(0);
                } //clear that
            }
        }



        /// <summary>
        /// Pauses Printing Pod
        /// </summary>
        [HarmonyPatch(typeof(Immigration))]
        [HarmonyPatch(nameof(Immigration.Sim200ms))]
        public class Add
        {
            static async Task DoWithDelay(System.Action task, int ms)
            {
                await Task.Delay(ms);
                task.Invoke();
            }
            public static void Prefix(Immigration __instance, float dt)
            {
                if (__instance.bImmigrantAvailable == false && Mathf.Approximately(Math.Max(__instance.timeBeforeSpawn - dt, 0.0f), 0.0f) && ModConfig.Instance.PauseOnReadyToPrint)
                {
                    SgtLogger.l("Paused the game - new printables available");
                    DoWithDelay(() => SpeedControlScreen.Instance.Pause(true), (3 - SpeedControlScreen.Instance.speed) * 500);
                }
            }
        }


        /// <summary>
        /// Applies custom printing pod cooldown
        /// </summary>
        [HarmonyPatch(typeof(Immigration))]
        [HarmonyPatch(nameof(Immigration.EndImmigration))]
        public class AdjustTImeOfReprint
        {
            public static void Prefix(Immigration __instance)
            {

                for (int i = 0; i < __instance.spawnInterval.Length; i++)
                {
                    __instance.spawnInterval[i] = Mathf.RoundToInt(ModConfig.Instance.PrintingPodRechargeTime * 600f);
                }
                //for(int i = 0; i < __instance.spawnInterval.Length; i++)
                //{
                //    SgtLogger.l(__instance.spawnInterval[i].ToString(), i.ToString());
                //}
            }
        }
        [HarmonyPatch(typeof(Immigration))]
        [HarmonyPatch(nameof(Immigration.OnPrefabInit))]
        public class AdjustTImeOfReprint_Initial
        {
            public static void Postfix(Immigration __instance)
            {

                for (int i = 0; i < __instance.spawnInterval.Length; i++)
                {
                    __instance.spawnInterval[i] = Mathf.RoundToInt(ModConfig.Instance.PrintingPodRechargeTime * 600f);
                }
                __instance.timeBeforeSpawn = Mathf.RoundToInt(ModConfig.Instance.PrintingPodRechargeTime * 600f);
                //for(int i = 0; i < __instance.spawnInterval.Length; i++)
                //{
                //    SgtLogger.l(__instance.spawnInterval[i].ToString(), i.ToString());
                //}
            }
        }
        [HarmonyPatch(typeof(MinionSelectScreen))]
        [HarmonyPatch(nameof(MinionSelectScreen.OnSpawn))]
        public class AddCrewPresetButton
        {
            public static void Postfix(MinionSelectScreen __instance)
            {
                var PresetButton = Util.KInstantiateUI(__instance.proceedButton.gameObject, __instance.proceedButton.transform.parent.gameObject, true);
                var btn = PresetButton.GetComponent<KButton>();

                PresetButton.GetComponentInChildren<LocText>().text = STRINGS.UI.PRESETWINDOW.TITLECREW.ToString().ToUpperInvariant();
                UIUtils.AddActionToButton(PresetButton.transform, "", () => UnityCrewPresetScreen.ShowWindow(__instance as CharacterSelectionController, null));


                var addOneDupeButton = Util.KInstantiateUI<KButton>(__instance.backButton.gameObject, __instance.proceedButton.transform.parent.gameObject, true);
                UIUtils.AddActionToButton(addOneDupeButton.transform, "", () =>
                {
                    CharacterContainer characterContainer = Util.KInstantiateUI<CharacterContainer>(__instance.containerPrefab.gameObject, __instance.containerParent);
                    characterContainer.SetController(__instance);
                    __instance.containers.Add(characterContainer);
                }
                );

                UIUtils.AddSimpleTooltipToObject(addOneDupeButton.transform, MODDEDIMMIGRANTSCREEN.ADDDUPETOOLTIP);
                UIUtils.TryChangeText(addOneDupeButton.transform, "Text", MODDEDIMMIGRANTSCREEN.ADDDUPE);
                addOneDupeButton.transform.Find("FG").GetComponent<Image>().sprite = Assets.GetSprite("icon_positive");


                addOneDupeButton.transform.SetSiblingIndex(1);
                PresetButton.transform.SetSiblingIndex(2);
            }
        }



        [HarmonyPatch(typeof(LonelyMinionHouse.Instance), nameof(LonelyMinionHouse.Instance.SpawnMinion))]
        public class MakeJorgeRerollable
        {
            public static void GrabJorgeGameObject(MinionIdentity minionIdentity)
            {
                if (ModConfig.Instance.JorgeAndCryopodDupes)
                {
                    SgtLogger.l("Getting Jorge Gameobject");
                    CryoDupeToApplyStatsOn = minionIdentity.gameObject;
                }
            }
            public static void Postfix()
            {
                SgtLogger.l("Start Editing Jorge");
                if (CryoDupeToApplyStatsOn && ModConfig.Instance.JorgeAndCryopodDupes)
                {

                    ModAssets.EditingSingleDupe = true;
                    ModAssets.EditingJorge = true;
                    ImmigrantScreen.InitializeImmigrantScreen(null);

                }
            }

            public static readonly MethodInfo GrabGameObjectOfJorge = AccessTools.Method(
               typeof(MakeJorgeRerollable),
               nameof(MakeJorgeRerollable.GrabJorgeGameObject));

            public static readonly FieldInfo immigrationInstance = AccessTools.Field(
                typeof(Immigration),
                nameof(Immigration.Instance)
            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex1 = code.FindIndex(ci => ci.opcode == OpCodes.Ldsfld && ci.operand is FieldInfo f && f == immigrationInstance);
                int locId = TranspilerHelper.FindIndexOfNextLocalIndex(code, insertionIndex1, false);

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex1 != -1)
                {
                    code.Insert(insertionIndex1, new CodeInstruction(OpCodes.Call, GrabGameObjectOfJorge));
                    code.Insert(insertionIndex1, new CodeInstruction(OpCodes.Ldloc_S, locId));

                }
                else
                {
                    SgtLogger.l("JORGE TRANSPILER INSERTION FAILED");
                }
                return code;
            }
        }



        /// <summary>
        /// Gets a prefab and applies "Care Packages Only"-Mode
        /// </summary>
        [HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.InitializeContainers))]
        public class controller2_patch
        {
            public static CharacterSelectionController instance;
            public static void Prefix(CharacterSelectionController __instance)
            {
                instance = __instance;
            }

            public static void Postfix(KButton ___proceedButton)
            {
                //Debug.Log("Creating PREFAB2");
                NextButtonPrefab = Util.KInstantiateUI(___proceedButton.gameObject);
                //UIUtils.ListAllChildren(NextButtonPrefab.transform);
                NextButtonPrefab.name = "CycleButtonPrefab";
            }
            public static void CarePackagesOnly()
            {
                if (ModConfig.Instance.CarePackagesOnly && Components.MinionIdentities.Count > ModConfig.Instance.CarePackagesOnlyDupeCap)
                {
                    instance.numberOfCarePackageOptions = ModConfig.Instance.CarePackagesOnlyPackageCap;
                    instance.numberOfDuplicantOptions = 0;
                }
            }

            private static readonly FieldInfo numberOfDupes = AccessTools.Field(
                typeof(CharacterSelectionController),
                "numberOfDuplicantOptions");

            private static readonly FieldInfo numberOfCarePacks = AccessTools.Field(
                typeof(CharacterSelectionController),
                "numberOfCarePackageOptions");

            public static readonly MethodInfo AdjustNumbers = AccessTools.Method(
               typeof(controller2_patch),
               nameof(controller2_patch.CarePackagesOnly));

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex1 = code.FindIndex(ci => ci.opcode == OpCodes.Stfld && ci.operand is FieldInfo fi && fi == numberOfCarePacks);
                var insertionIndex2 = code.FindLastIndex(ci => ci.opcode == OpCodes.Stfld && ci.operand is FieldInfo fi && fi == numberOfDupes);

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex1 != -1 && insertionIndex2 != -1)
                {
                    code.Insert(++insertionIndex2, new CodeInstruction(OpCodes.Call, AdjustNumbers));
                    code.Insert(++insertionIndex1, new CodeInstruction(OpCodes.Call, AdjustNumbers));

                    //TranspilerHelper.PrintInstructions(code);
                }
                return code;
            }
        }



        [HarmonyPatch(typeof(WattsonMessage))]
        [HarmonyPatch(nameof(WattsonMessage.OnActivate))]
        public class DupeSpawnAdjustmentNo2BecauseKleiIsKlei
        {
            const float OxilitePerDupePerDay = 0.1f * 600f; //in KG
            const float FoodBarsPerDupePerDay = 1000 / 800f; //in Units
            static void Postfix()
            {
                if (ModConfig.Instance.StartupResources && ModConfig.Instance.DuplicantStartAmount > 3)
                {
                    GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
                    float dupeCount = ModConfig.Instance.DuplicantStartAmount;

                    float OxiliteNeeded = OxilitePerDupePerDay * ModConfig.Instance.SupportedDays * (dupeCount - 3);
                    float FoodeNeeded = FoodBarsPerDupePerDay * ModConfig.Instance.SupportedDays * (dupeCount - 3);
                    Vector3 SpawnPos = telepad.transform.position;

                    while (OxiliteNeeded > 0)
                    {
                        var SpawnAmount = Math.Min(OxiliteNeeded, 25000f);
                        OxiliteNeeded -= SpawnAmount;
                        ElementLoader.FindElementByHash(SimHashes.OxyRock).substance.SpawnResource(SpawnPos, SpawnAmount, UtilLibs.UtilMethods.GetKelvinFromC(20f), byte.MaxValue, 0, false);
                    }

                    GameObject go = Util.KInstantiate(Assets.GetPrefab(FieldRationConfig.ID));
                    go.transform.SetPosition(SpawnPos);
                    PrimaryElement symbolOverride = go.GetComponent<PrimaryElement>();
                    symbolOverride.Units = FoodeNeeded;
                    go.SetActive(true);
                }
            }

            static void YeetOxilite(GameObject originGo, float amount)
            {

                GameObject go = Util.KInstantiate(Assets.GetPrefab(FieldRationConfig.ID));
                go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(originGo), Grid.SceneLayer.Ore));
                PrimaryElement symbolOverride = go.GetComponent<PrimaryElement>();
                symbolOverride.Units = amount;
                go.SetActive(true);


                Vector2 initial_velocity = new Vector2(UnityEngine.Random.Range(-2f, 2f) * 1f, (float)((double)UnityEngine.Random.value * 2.0 + 4.0));
                if (GameComps.Fallers.Has((object)go))
                    GameComps.Fallers.Remove(go);
                GameComps.Fallers.Add(go, initial_velocity);
            }


            public static float AdjustCellX(float OldX, GameObject printingPod, int index) ///int requirement to consume previous "3" on stack
            {
                int newCell = Grid.PosToCell(printingPod) + ((index + 1) % 4 - 1);
                //Debug.Log("Old CellPosX: " + OldX + ", New CellPos: " + Grid.CellToXY(newCell));
                //YeetOxilite(printingPod, 150f);
                return (float)Grid.CellToXY(newCell).x + 0.5f;
            }

            public static readonly MethodInfo NewCellX = AccessTools.Method(
               typeof(DupeSpawnAdjustmentNo2BecauseKleiIsKlei),
               nameof(DupeSpawnAdjustmentNo2BecauseKleiIsKlei.AdjustCellX));

            public static readonly MethodInfo GetPrintingPodInfo = AccessTools.Method(
               typeof(GameUtil),
               nameof(GameUtil.GetTelepad));

            public static readonly MethodInfo GetDupeFromComponentInfo = AccessTools.Method(
               typeof(Components.Cmps<MinionIdentity>),
               ("get_Item"));


            [HarmonyPriority(Priority.VeryLow)]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Sub);
                var insertionIndexPrintingPodInfo = code.FindIndex(ci => ci.opcode == OpCodes.Call && ci.operand is MethodInfo f && f == GetPrintingPodInfo);
                var minionGetterIndexInfo = code.FindIndex(ci => ci.opcode == OpCodes.Callvirt && ci.operand is MethodInfo f && f == GetDupeFromComponentInfo);

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                    int printingPodIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, insertionIndexPrintingPodInfo, false);
                    int IDXIndex = TranspilerHelper.FindIndexOfNextLocalIndex(code, minionGetterIndexInfo);

                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, printingPodIndex));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, IDXIndex));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, NewCellX));
                    // TranspilerHelper.PrintInstructions(code);
                }
                //foreach (var v in code) { Console.WriteLine(v.opcode + (v.operand != null ? ": " + v.operand : "")); };
                return code;
            }
        }

        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.OnSpawn))]
        public class AddDeletionButtonForStartScreen_TraitRerolling
        {
            public static void Postfix(CharacterContainer __instance)
            {
                bool IsStartDupe = __instance.controller is MinionSelectScreen;

                if (IsStartDupe)
                {
                    GameObject deleteBtn = Util.KInstantiateUI(__instance.reshuffleButton.gameObject, __instance.reshuffleButton.transform.parent.gameObject, true);
                    deleteBtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 80f);
                    var text = deleteBtn.transform.Find("Text");
                    text.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 2, 76f);


                    UIUtils.TryChangeText(text, "", MODDEDIMMIGRANTSCREEN.REMOVEDUPE);
                    UIUtils.AddSimpleTooltipToObject(deleteBtn.transform, MODDEDIMMIGRANTSCREEN.REMOVEDUPETOOLTIP);
                    UIUtils.AddActionToButton(deleteBtn.transform, "", () =>
                    {
                        if (__instance.controller.containers.Count > 1)
                        {
                            __instance.controller.containers.Remove(__instance);
                            UnityEngine.Object.Destroy(__instance.GetGameObject());
                        }
                    });
                }
                if (!EditingSingleDupe && (ModConfig.Instance.RerollDuringGame || IsStartDupe))
                {
                    GameObject rerollTraitBtn = Util.KInstantiateUI(__instance.reshuffleButton.gameObject, __instance.reshuffleButton.transform.parent.gameObject, true);
                    rerollTraitBtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 120, 80f);
                    var text = rerollTraitBtn.transform.Find("Text");
                    text.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 2, 76f);


                    ApplyTraitStyleByKey(rerollTraitBtn.GetComponent<KImage>(), default);
                    UIUtils.TryChangeText(text, "", CONGENITALTRAITS.NONE.NAME);
                    UIUtils.AddSimpleTooltipToObject(rerollTraitBtn.transform, MODDEDIMMIGRANTSCREEN.GUARANTEETRAIT);
                    UIUtils.AddActionToButton(rerollTraitBtn.transform, "", () =>
                    {
                        UnityTraitRerollingScreen.ShowWindow(() =>
                        {
                            __instance.Reshuffle(IsStartDupe);
                            var type = GetTraitListOfTrait(UnityTraitRerollingScreen.GetTraitId(__instance), out _);
                            ApplyTraitStyleByKey(rerollTraitBtn.GetComponent<KImage>(), type);
                            UIUtils.TryChangeText(text, "", UnityTraitRerollingScreen.GetTraitName(__instance));
                        },
                        __instance);
                    });

                    if (!buttonsToDeactivateOnEdit.ContainsKey(__instance))
                    {
                        buttonsToDeactivateOnEdit[__instance] = new List<KButton>();
                    }
                    buttonsToDeactivateOnEdit[__instance].Add(rerollTraitBtn.GetComponent<KButton>());
                }
            }
        }



        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.OnNameChanged))]
        public class FixPersonalityRenaming
        {
            static string Backup = "MISSING";

            public static void Prefix(CharacterContainer __instance, ref string __state)
            {
                __state = __instance.stats.personality.Name;
            }
            public static void Postfix(CharacterContainer __instance, string __state)
            {
                if (__state == null)
                    return;
                __instance.stats.personality.Name = __state;
                __instance.description.text = __instance.stats.personality.description;
            }
        }


        [HarmonyPatch(typeof(MinionStartingStats))]
        [HarmonyPatch(nameof(MinionStartingStats.GenerateTraits))]
        public class AllowCustomTraitAllignment
        {
            public static void Postfix(MinionStartingStats __instance)
            {
                if (ModConfig.Instance.NoJoyReactions)
                {
                    __instance.joyTrait = Db.Get().traits.Get("None");
                }
                if (ModConfig.Instance.NoStressReactions)
                {
                    __instance.stressTrait = Db.Get().traits.Get("None");
                }
            }

            ////consuming old value to always roll dupes with more than 2 traits on reroll
            //public static bool VariableTraits(bool isStarterMinion)
            //{
            //    return false;
            //}

            //public static readonly MethodInfo overrideStarterGeneration = AccessTools.Method(
            //   typeof(AllowCustomTraitAllignment),
            //   nameof(AllowCustomTraitAllignment.VariableTraits));

            //[HarmonyPriority(Priority.VeryLow)]
            //static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            //{
            //    var code = instructions.ToList();
            //    var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Ldfld && ci.operand.ToString().Contains("is_starter_minion"));

            //    if (insertionIndex != -1)
            //    {
            //        code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, overrideStarterGeneration));
            //    }
            //    return code;
            //}
        }

        [HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.InitializeContainers))]
        public class CharacterSelectionController_Patch
        {
            public static int CustomStartingDupeCount(int dupeCount) ///int requirement to consume previous "3" on stack
            {
                if (dupeCount == 3 && controller2_patch.instance is MinionSelectScreen)
                    return ModConfig.Instance.DuplicantStartAmount; ///push new value to the stack
                else return dupeCount;
            }

            public static readonly MethodInfo AdjustNumber = AccessTools.Method(
               typeof(CharacterSelectionController_Patch),
               nameof(CustomStartingDupeCount));

            [HarmonyPriority(Priority.VeryLow)]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Ldc_I4_3);

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, AdjustNumber));
                }
                return code;
            }

            /// <summary>
            /// Size Adjustment
            /// </summary>
            /// <param name="__instance"></param>
            public static void Prefix(CharacterSelectionController __instance)
            {
                GameObject parentToScale = (GameObject)typeof(CharacterSelectionController).GetField("containerParent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                CharacterContainer prefabToScale = (CharacterContainer)typeof(CharacterSelectionController).GetField("containerPrefab", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                ///If is starterscreen
                if (__instance.GetType() == typeof(MinionSelectScreen))
                {
#if DEBUG
                    Debug.Log("Manipulating Instance: " + __instance.GetType());


#endif

                    GridLayoutGroup[] objectsOfType2 = UnityEngine.Object.FindObjectsOfType<GridLayoutGroup>();
                    foreach (var layout in objectsOfType2)
                    {
                        if (layout.name == "CharacterContainers")
                        {
                            ///adding scroll
                            var scroll = layout.transform.parent.parent.FindOrAddComponent<ScrollRect>();
                            scroll.content = layout.transform.parent.rectTransform();
                            scroll.horizontal = false;
                            scroll.scrollSensitivity = 100;
                            scroll.movementType = ScrollRect.MovementType.Clamped;
                            scroll.inertia = false;
                            ///setting start pos
                            layout.transform.parent.rectTransform().pivot = new Vector2(0.5f, 0.99f);

                            ///top & bottom padding
                            layout.transform.parent.TryGetComponent<VerticalLayoutGroup>(out var verticalLayoutGroup);
                            verticalLayoutGroup.padding = new RectOffset(00, 00, 50, 50);
                            layout.childAlignment = TextAnchor.UpperCenter;
                            int countPerRow = ModConfig.Instance.DuplicantStartAmount;

                            layout.constraintCount = 5;
                        }
                    }
                    __instance.transform.Find("Content/BottomContent").TryGetComponent<VerticalLayoutGroup>(out var buttonGroup);
                    buttonGroup.childAlignment = TextAnchor.LowerCenter;

                    ModAssets.ParentScreen = __instance.transform.parent.gameObject;
                }

                else
                {
                    ModAssets.ParentScreen = PauseScreen.Instance.transform.parent.gameObject;
                }
                //SgtLogger.l(UnityPresetScreen.parentScreen.ToString(), "PRESET");
#if DEBUG
                //Debug.Log("PREFAB: " + size);
#endif
            }

            public static void Postfix(CharacterSelectionController __instance, CharacterContainer ___containerPrefab)
            {
                if (ModAssets.StartPrefab == null)
                {
                    StartPrefab = ___containerPrefab.transform.Find("Details").gameObject;
                    //StartPrefab.transform.Find("Top/PortraitContainer/PortraitContent").gameObject.SetActive(false);
                    //StartPrefab.transform.name = "ModifyDupeStats";

                }
                if (!__instance.IsStarterMinion)
                {
                    return;
                }

                LocText[] objectsOfType1 = UnityEngine.Object.FindObjectsOfType<LocText>();
                if (objectsOfType1 != null)
                {
                    foreach (LocText locText in objectsOfType1)
                    {
                        if (locText.key == "STRINGS.UI.IMMIGRANTSCREEN.SELECTYOURCREW" || locText.key == "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURLONECREWMAN")
                        {
                            locText.key = ModConfig.Instance.DuplicantStartAmount == 1 ? "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURLONECREWMAN" : "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURCREW";
                            break;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CharacterContainer), "OnCmpDisable")]
        public static class RestoreOnCLosing
        {
            public static void Prefix(CharacterContainer __instance, Transform ___aptitudeLabel)
            {
#if DEBUG
                //Debug.Log("Closing start");
                //UIUtils.ListAllChildren(__instance.transform);
                //Debug.Log("Closing Stop");
#endif


                __instance.transform.Find("Details").gameObject.SetActive(true);

                var skillMod = __instance.transform.Find("ModifyDupeStats");

                if (skillMod == null)
                    return;
                skillMod.gameObject.SetActive(false);
            }
        }

        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.GenerateCharacter))]
        public static class AddChangeButtonToCharacterContainer
        {

            public static MinionStartingStats GenerateWithGuaranteedSkill(bool is_starter_minion, string guaranteedAptitudeID = null, string guaranteedTraitID = null, bool isDebugMinion = false, CharacterContainer __instance = null)
            {
                if (__instance != null && UnityTraitRerollingScreen.GuaranteedTraitRoll.ContainsKey(__instance))
                {
                    return new MinionStartingStats(is_starter_minion, guaranteedAptitudeID, UnityTraitRerollingScreen.GuaranteedTraitRoll[__instance].Id, isDebugMinion);
                }
                return new MinionStartingStats(is_starter_minion, guaranteedAptitudeID, guaranteedTraitID, isDebugMinion);
            }

            public static readonly MethodInfo overrideStarterGeneration = AccessTools.Method(
               typeof(AddChangeButtonToCharacterContainer),
               nameof(AddChangeButtonToCharacterContainer.GenerateWithGuaranteedSkill));

            [HarmonyPriority(Priority.VeryLow)]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Newobj);

                if (insertionIndex != -1)
                {
                    code[insertionIndex] = new CodeInstruction(OpCodes.Call, overrideStarterGeneration);
                    code.Insert(insertionIndex, new CodeInstruction(OpCodes.Ldarg_0));
                }
                else
                    SgtLogger.warning("minionStartingStatsReplacer not found");

                //TranspilerHelper.PrintInstructions(code);
                return code;
            }



            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats, bool is_starter)
            {
                bool AllowModification = ModConfig.Instance.ModifyDuringGame || (EditingSingleDupe && ModConfig.Instance.JorgeAndCryopodDupes);
                if (!buttonsToDeactivateOnEdit.ContainsKey(__instance))
                {
                    buttonsToDeactivateOnEdit[__instance] = new List<KButton>();
                }


                var buttonPrefab = __instance.transform.Find("TitleBar/RenameButton").gameObject;
                var titlebar = __instance.transform.Find("TitleBar").gameObject;

                //28
                int insetBase = 4, insetA = 28, insetB = insetA * 2, insetC = insetA * 3;
                float insetDistance = (!is_starter && !AllowModification) ? insetBase + insetA : insetBase + insetC;

                //var TextInput = titlebar.transform.Find("LabelGroup/");
                //TextInput.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 3, 60);



                ///Make skin button
                var skinBtn = Util.KInstantiateUI(buttonPrefab, titlebar);
                skinBtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, insetDistance, skinBtn.rectTransform().sizeDelta.x);

                skinBtn.name = "DupeSkinButton";
                skinBtn.GetComponent<ToolTip>().toolTip = STRINGS.UI.BUTTONS.DUPESKINBUTTONTOOLTIP;

                skinBtn.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("ic_dupe");


                buttonsToDeactivateOnEdit[__instance].Add(skinBtn.FindComponent<KButton>());

                //var currentlySelectedIdentity = __instance.GetComponent<MinionIdentity>();

                System.Action RebuildDupePanel = () =>
                {
                    __instance.SetInfoText();
                    __instance.SetAttributes();
                    __instance.SetAnimator();
                };

                UIUtils.AddActionToButton(skinBtn.transform, "", () => DupeSkinScreenAddon.ShowSkinScreen(__instance, ___stats));

                if (!(!is_starter && !AllowModification))
                {
                    float insetDistancePresetButton = insetBase + insetB;
                    ///Make Preset button
                    var PresetButton = Util.KInstantiateUI(buttonPrefab, titlebar);
                    PresetButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, insetDistancePresetButton, PresetButton.rectTransform().sizeDelta.x);
                    PresetButton.name = "DupePresetButton";
                    PresetButton.GetComponent<ToolTip>().toolTip = STRINGS.UI.BUTTONS.PRESETWINDOWBUTTONTOOLTIP;

                    PresetButton.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconPaste");
                    //var currentlySelectedIdentity = __instance.GetComponent<MinionIdentity>();

                    //UIUtils.AddActionToButton(PresetButton.transform, "", () => DupePresetScreenAddon.ShowPresetScreen(__instance, ___stats)); 
                    UIUtils.AddActionToButton(PresetButton.transform, "", () => UnityPresetScreen.ShowWindow(___stats, RebuildDupePanel));
                    buttonsToDeactivateOnEdit[__instance].Add(PresetButton.FindComponent<KButton>());
                }

                if (!is_starter && !AllowModification)
                    return;
                ///Make modify button
                var changebtn = Util.KInstantiateUI(buttonPrefab, titlebar);
                changebtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, insetBase + insetA, changebtn.rectTransform().sizeDelta.x);
                changebtn.name = "ChangeDupeStatButton";
                changebtn.GetComponent<ToolTip>().toolTip = STRINGS.UI.BUTTONS.MODIFYBUTTONTOOLTIP;

                var img = changebtn.transform.Find("Image").GetComponent<KImage>();
                img.sprite = Assets.GetSprite("icon_gear");

                var button = __instance.transform.Find("ShuffleDupeButton").GetComponent<KButton>();
                var button2 = __instance.transform.Find("ArchetypeSelect").GetComponent<KButton>();

                buttonsToDeactivateOnEdit[__instance].Add(button);
                buttonsToDeactivateOnEdit[__instance].Add(button2);

                ChangeButton(false, changebtn, __instance, ___stats, RebuildDupePanel);

                AddNewToTraitsButtonPrefab = Util.KInstantiateUI(buttonPrefab);
                AddNewToTraitsButtonPrefab.GetComponent<ToolTip>().enabled = false;
                AddNewToTraitsButtonPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("icon_positive");
                AddNewToTraitsButtonPrefab.name = "AddButton";

                RemoveFromTraitsButtonPrefab = Util.KInstantiateUI(buttonPrefab);
                RemoveFromTraitsButtonPrefab.GetComponent<ToolTip>().enabled = false;
                RemoveFromTraitsButtonPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("icon_negative");
                RemoveFromTraitsButtonPrefab.name = "RemoveButton";

            }

            static void ChangeButton(bool isCurrentlyInEditMode, GameObject buttonGO, CharacterContainer parent, MinionStartingStats referencedStats, System.Action OnClose)
            {
                buttonGO.GetComponent<ToolTip>().SetSimpleTooltip(!isCurrentlyInEditMode ? STRINGS.UI.BUTTONS.MODIFYBUTTONTOOLTIP : STRINGS.UI.BUTTONS.MODIFYBUTTONTOOLTIP2);
                var img = buttonGO.transform.Find("Image").GetComponent<KImage>();
                img.sprite = Assets.GetSprite(!isCurrentlyInEditMode ? "icon_gear" : "iconSave");
                var button = buttonGO.GetComponent<KButton>();
                button.ClearOnClick();
                button.onClick += () =>
                {
                    ChangeButton(!isCurrentlyInEditMode, buttonGO, parent, referencedStats, OnClose);
                    if (isCurrentlyInEditMode)
                    {
                        InstantiateOrGetDupeModWindow(parent.gameObject, referencedStats, true);
                        OnClose.Invoke();
                    }
                    else
                    {
                        InstantiateOrGetDupeModWindow(parent.gameObject, referencedStats, false);
                    }
                    if (buttonsToDeactivateOnEdit.ContainsKey(parent))
                    {
                        foreach (var button in buttonsToDeactivateOnEdit[parent])
                        {
                            button.isInteractable = isCurrentlyInEditMode;
                        }
                    }
                };
                parent.transform.Find("Details").gameObject.SetActive(!isCurrentlyInEditMode);
            }

            static void InstantiateOrGetDupeModWindow(GameObject parent, MinionStartingStats referencedStats, bool hide)
            {

                bool ShouldInit = true;
                var ParentContainer = parent.transform.Find("ModifyDupeStats");


                if (ParentContainer == null)
                {
                    //Debug.Log("HAD TO MAKE NEW");
                    ParentContainer = Util.KInstantiateUI(StartPrefab, parent).transform;
                    ParentContainer.gameObject.name = "ModifyDupeStats";
                }
                else
                {
                    //Debug.Log("FOUND OLD");
                    var refOld = ParentContainer.gameObject;
                    ;
                    refOld.transform.SetAsLastSibling();
                    UnityEngine.Object.Destroy(refOld);
                    ParentContainer = Util.KInstantiateUI(StartPrefab, parent).transform;
                    ParentContainer.gameObject.name = "ModifyDupeStats";
                    //ShouldInit = false;
                }


                ///Building the Button window
                if (ShouldInit)
                {

                    //Debug.Log("FindScroll");
                    //UIUtils.ListAllChildren(ParentContainer.transform);
                    //Debug.Log("endFindScroll");

                    UIUtils.FindAndDestroy(ParentContainer.transform, "Top");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "AttributeScores");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "AttributeScores");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "Scroll/Content/TraitsAndAptitudes/AptitudeContainer");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "Scroll/Content/TraitsAndAptitudes/TraitContainer");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "Scroll/Content/ExpectationsGroupAlt");
                    UIUtils.FindAndDestroy(ParentContainer.transform, "Scroll/Content/DescriptionGroup");

                    var ContentContainer = ParentContainer.Find("Scroll/Content/TraitsAndAptitudes");
                    var overallSize = ParentContainer.Find("Scroll");
                    var SizeSetter = ParentContainer.Find("Scroll").GetComponent<LayoutElement>();
                    SizeSetter.flexibleHeight = 600;
                    var scrollerCmp = overallSize.GetComponent<KScrollRect>();

                    scrollerCmp.elasticity = 0;
                    scrollerCmp.inertia = false;
                    //scrollerCmp.decelerationRate = 100;
                    var vlg = ContentContainer.GetComponent<VerticalLayoutGroup>();
                    //SgtLogger.l(vlg.padding.ToString() + ", " + vlg.spacing);
                    vlg.spacing = 1;
                    vlg.padding = new RectOffset(3, 1, 0, 0);
                    //UIUtils.ListComponents(overallSize.gameObject);


                    var prefabParent = NextButtonPrefab;
                    if (prefabParent.transform.Find("NextButton") == null)
                    {
                        prefabParent.GetComponent<KButton>().enabled = true;
                        var right = Util.KInstantiateUI(RemoveFromTraitsButtonPrefab, prefabParent);
                        UIUtils.TryFindComponent<ToolTip>(right.transform).toolTip = STRINGS.UI.BUTTONS.REMOVEFROMSTATS;
                        //UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
                        right.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 2.5f, 25);
                        right.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 2.5f, 25);
                        right.SetActive(false);

                        var AddOnSpacerInterestUP = Util.KInstantiateUI(RemoveFromTraitsButtonPrefab, prefabParent);
                        //UIUtils.TryFindComponent<ToolTip>(prefabParent.transform).toolTip = STRINGS.UI.BUTTONS.ADDTOSTATS;
                        //UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
                        AddOnSpacerInterestUP.name = "InterestUP";
                        AddOnSpacerInterestUP.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 27.5f, 25);
                        AddOnSpacerInterestUP.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 2.5f, 25);
                        AddOnSpacerInterestUP.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconDown");
                        AddOnSpacerInterestUP.transform.Find("Image").rectTransform().Rotate(new Vector3(0, 0, 180));
                        AddOnSpacerInterestUP.SetActive(false);

                        var AddOnSpacerInterestDown = Util.KInstantiateUI(RemoveFromTraitsButtonPrefab, prefabParent);
                        //UIUtils.TryFindComponent<ToolTip>(prefabParent.transform).toolTip = STRINGS.UI.BUTTONS.ADDTOSTATS;
                        //UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
                        AddOnSpacerInterestDown.name = "InterestDOWN";
                        AddOnSpacerInterestDown.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 2.5f, 25);
                        AddOnSpacerInterestDown.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 2.5f, 25);
                        AddOnSpacerInterestDown.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconDown");
                        AddOnSpacerInterestDown.SetActive(false);

                    }


                    var renameLabel = prefabParent.transform.Find("SelectLabel");
                    if (renameLabel != null)
                    {
                        renameLabel.name = "Label";
                    }

                    prefabParent.GetComponent<LayoutElement>().minHeight = 30;
                    prefabParent.GetComponent<LayoutElement>().preferredHeight = 30;
                    var spacerParent = Util.KInstantiateUI(prefabParent.transform.Find("Label").gameObject);
                    spacerParent.AddOrGet<LayoutElement>().minHeight = 25;

                    var AddOnSpacer = Util.KInstantiateUI(AddNewToTraitsButtonPrefab, spacerParent);
                    UIUtils.TryFindComponent<ToolTip>(AddOnSpacer.transform).toolTip = STRINGS.UI.BUTTONS.ADDTOSTATS;
                    //UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
                    AddOnSpacer.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 2.5f, 25);
                    AddOnSpacer.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0f, 25);
                    AddOnSpacer.SetActive(false);

                    //skillMod.transform.Find("DetailsContainer").gameObject.SetActive(false);

                    if (!ModAssets.DupeTraitManagers.ContainsKey(referencedStats))
                    {
                        DupeTraitManagers[referencedStats] = new DupeTraitManager();
                        DupeTraitManagers[referencedStats].SetReferenceStats(referencedStats);
                    }
                    var DupeTraitMng = DupeTraitManagers[referencedStats];



                    var Spacer2AndInterestHolder = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);
                    UIUtils.AddSimpleTooltipToObject(Spacer2AndInterestHolder.transform, global::STRINGS.UI.CHARACTERCONTAINER_APTITUDES_TITLE_TOOLTIP, alignCenter: true, onBottom: true);

                    var InterestPointBonus = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);
                    UIUtils.TryChangeText(InterestPointBonus.transform, "", STRINGS.UI.DUPESETTINGSSCREEN.TRAITBONUSPOOL + " " + DupeTraitMng.PointPool);
                    string InterestBonusTooltip = string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAITBONUSPOOLTOOLTIP, DupeTraitMng.AdditionalSkillPoints);


                    UIUtils.TryChangeText(Spacer2AndInterestHolder.transform, "", global::STRINGS.UI.CHARACTERCONTAINER_APTITUDES_TITLE); //
                    Spacer2AndInterestHolder.transform.Find("AddButton").gameObject.SetActive(ModConfig.Instance.AddAndRemoveTraitsAndInterests);
                    UIUtils.AddActionToButton(Spacer2AndInterestHolder.transform, "AddButton", () =>
                    {
                        UnityTraitScreen.ShowWindow(referencedStats, () => InstantiateOrGetDupeModWindow(parent, referencedStats, hide), DupeTraitManager: DupeTraitMng, openedFrom: UnityTraitScreen.OpenedFrom.Interest);
                    });


                    foreach (var a in DupeTraitMng.GetInterestsWithStats())
                    {
                        var AptitudeEntry = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);
                        UIUtils.AddActionToButton(AptitudeEntry.transform, "", () =>
                        {
                            UnityTraitScreen.ShowWindow(referencedStats, () => InstantiateOrGetDupeModWindow(parent, referencedStats, hide), currentGroup: a, DupeTraitManager: DupeTraitMng);
                        });
                        UIUtils.AddSimpleTooltipToObject(AptitudeEntry.transform, ModAssets.GetSkillgroupDescription(a, referencedStats), true, onBottom: true);
                        AptitudeEntry.GetComponent<KButton>().enabled = true;
                        ApplyDefaultStyle(AptitudeEntry.GetComponent<KImage>());
                        UIUtils.TryChangeText(AptitudeEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, GetSkillGroupName(a), FirstSkillGroupStat(a), DupeTraitMng.GetBonusValue(a)));

                        AptitudeEntry.transform.Find("RemoveButton").gameObject.SetActive(ModConfig.Instance.AddAndRemoveTraitsAndInterests);
                        UIUtils.AddActionToButton(AptitudeEntry.transform, "RemoveButton", () =>
                        {
                            DupeTraitMng.RemoveInterest(a);
                            InstantiateOrGetDupeModWindow(parent, referencedStats, hide);
                        }
                        );

                        AptitudeEntry.transform.Find("InterestDOWN").gameObject.SetActive(true);
                        UIUtils.AddActionToButton(AptitudeEntry.transform, "InterestDOWN", () =>
                        {
                            DupeTraitMng.ReduceInterest(a);
                            InstantiateOrGetDupeModWindow(parent, referencedStats, hide);
                        });
                        AptitudeEntry.transform.Find("InterestUP").gameObject.SetActive(true);
                        UIUtils.AddActionToButton(AptitudeEntry.transform, "InterestUP", () =>
                        {
                            DupeTraitMng.IncreaseInterest(a);
                            InstantiateOrGetDupeModWindow(parent, referencedStats, hide);
                        });

                    }
                    ///EndAptitudes

                    var spacer3 = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);
                    UIUtils.TryChangeText(spacer3.transform, "", global::STRINGS.UI.CHARACTERCONTAINER_TRAITS_TITLE);
                    //Db.Get().traits.TryGet();
                    spacer3.transform.Find("AddButton").gameObject.SetActive(ModConfig.Instance.AddAndRemoveTraitsAndInterests);
                    UIUtils.AddActionToButton(spacer3.transform, "AddButton", () =>
                    {
                        UnityTraitScreen.ShowWindow(referencedStats, () => InstantiateOrGetDupeModWindow(parent, referencedStats, hide), DupeTraitManager: DupeTraitMng, openedFrom: UnityTraitScreen.OpenedFrom.Trait);
                    });


                    var TraitsToSort = new List<Tuple<GameObject, DupeTraitManager.NextType>>();


                    foreach (Trait v in referencedStats.Traits)
                    {
                        if (v.Id == MinionConfig.MINION_BASE_TRAIT_ID)
                            continue;
                        var traitEntry = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);

                        UIUtils.AddSimpleTooltipToObject(traitEntry.transform, ModAssets.GetTraitTooltip(v, v.Id), true, onBottom: true);
                        var type = ModAssets.GetTraitListOfTrait(v.Id, out var list);

                        TraitsToSort.Add(new Tuple<GameObject, DupeTraitManager.NextType>(traitEntry, type));

                        ApplyTraitStyleByKey(traitEntry.GetComponent<KImage>(), type);
                        var thisOnesInterest = GetTraitStatBonusTooltip(v, false);
                        if (thisOnesInterest != string.Empty)
                        {
                            InterestBonusTooltip += "\n" + string.Format(global::STRINGS.UI.MODIFIER_ITEM_TEMPLATE, v.Name, thisOnesInterest);
                        }

                        traitEntry.GetComponent<KButton>().enabled = true;
                        UIUtils.AddActionToButton(traitEntry.transform, "", () =>
                        {
                            UnityTraitScreen.ShowWindow(referencedStats, () => InstantiateOrGetDupeModWindow(parent, referencedStats, hide), currentTrait: v);
                        });

                        UIUtils.TryChangeText(traitEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, v.Name));
                        traitEntry.transform.Find("RemoveButton").gameObject.SetActive(ModConfig.Instance.AddAndRemoveTraitsAndInterests && type != NextType.undefined);

                        ApplyTraitStyleByKey(traitEntry.transform.Find("RemoveButton").gameObject.GetComponent<KImage>(), type);

                        UIUtils.AddActionToButton(traitEntry.transform, "RemoveButton", () =>
                        {
                            ModAssets.RemoveTrait(referencedStats, v);
                            InstantiateOrGetDupeModWindow(parent, referencedStats, hide);
                        }
                        );
                    }
                    if (DupeTraitMng.ExternalModPoints != 0)
                        InterestBonusTooltip += "\n" + string.Format(global::STRINGS.UI.MODIFIER_ITEM_TEMPLATE, STRINGS.UI.DUPESETTINGSSCREEN.OTHERMODORIGINNAME, UIUtils.ColorNumber(DupeTraitMng.ExternalModPoints));

                    UIUtils.AddSimpleTooltipToObject(InterestPointBonus.transform, InterestBonusTooltip, true, onBottom: true);

                    TraitsToSort = TraitsToSort.OrderBy(t => (int)t.second).ToList();
                    for (int i = 0; i < TraitsToSort.Count; i++)
                    {
                        TraitsToSort[i].first.transform.SetAsLastSibling();
                    }
                    if (!ModConfig.Instance.NoJoyReactions && referencedStats.joyTrait.Id != "None")
                    {
                        var spacer = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);
                        UIUtils.TryChangeText(spacer.transform, "", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_JOYTRAIT, string.Empty));

                        var JoyTrait = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);

                        JoyTrait.GetComponent<KButton>().enabled = true;
                        UIUtils.AddActionToButton(JoyTrait.transform, "", () =>
                        {
                            UnityTraitScreen.ShowWindow(referencedStats, () => InstantiateOrGetDupeModWindow(parent, referencedStats, hide), currentTrait: referencedStats.joyTrait);
                        });

                        ApplyTraitStyleByKey(JoyTrait.GetComponent<KImage>(), DupeTraitManager.NextType.joy);
                        UIUtils.TryChangeText(JoyTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, referencedStats.joyTrait.Name));
                        UIUtils.AddSimpleTooltipToObject(JoyTrait.transform, ModAssets.GetTraitTooltip(referencedStats.joyTrait, referencedStats.joyTrait.Id), true, onBottom: true);
                    }

                    if (!ModConfig.Instance.NoStressReactions && referencedStats.stressTrait.Id != "None")
                    {
                        var spacerStress = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);
                        UIUtils.TryChangeText(spacerStress.transform, "", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_STRESSTRAIT, string.Empty))
                            ;
                        var StressTrait = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);
                        StressTrait.GetComponent<KButton>().enabled = true;
                        UIUtils.AddActionToButton(StressTrait.transform, "", () =>
                        {
                            UnityTraitScreen.ShowWindow(referencedStats, () => InstantiateOrGetDupeModWindow(parent, referencedStats, hide), currentTrait: referencedStats.stressTrait);
                        });

                        ApplyTraitStyleByKey(StressTrait.GetComponent<KImage>(), DupeTraitManager.NextType.stress);

                        UIUtils.AddSimpleTooltipToObject(StressTrait.transform, ModAssets.GetTraitTooltip(referencedStats.stressTrait, referencedStats.stressTrait.Id), true, onBottom: true);
                        UIUtils.TryChangeText(StressTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, referencedStats.stressTrait.Name));
                    }
                }

                ParentContainer.gameObject.SetActive(!hide);

            }
            static string GetSkillGroupName(SkillGroup Group) => ModAssets.GetChoreGroupNameForSkillgroup(Group);
            static string FirstSkillGroupStat(SkillGroup Group) => Strings.Get("STRINGS.DUPLICANTS.ATTRIBUTES." + Group.relevantAttributes.First().Id.ToUpperInvariant() + ".NAME");


        }

        /// <summary>
        /// /// Init. auto translation
        /// /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }
    }
}

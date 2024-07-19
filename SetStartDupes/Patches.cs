using Database;
using Epic.OnlineServices;
using HarmonyLib;
using Klei.AI;
using PeterHan.PLib.Core;
using ProcGen.Noise;
using SetStartDupes.DuplicityEditing;
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
using static Database.MonumentPartResource;
using static FetchManager;
using static KAnim;
using static KCompBuilder;
using static SetStartDupes.DupeTraitManager;
using static SetStartDupes.ModAssets;
using static SetStartDupes.STRINGS.UI;
using static SetStartDupes.STRINGS.UI.DSS_OPTIONS;
using static SetStartDupes.STRINGS.UI.PRESETWINDOW.HORIZONTALLAYOUT.ITEMINFO;
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
                if (__instance.HasTrait("StickerBomber")
                    && __instance.TryGetComponent<MinionIdentity>(out var identity)
                    && (identity.stickerType == null || identity.stickerType.Length == 0))
                {
                    SgtLogger.l("fixing stickerType");
                    identity.stickerType = ModAssets.GetRandomStickerType();
                }

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
        //[HarmonyPatch(typeof(CarePackageInfo), nameof(CarePackageInfo.Deliver))]
        //public class AdditionalCarePackages_DLC
        //{
        //    public static void Postfix(GameObject __result, CarePackageInfo __instance)
        //    {
        //        if(__instance.id)
        //    }
        //}
        [HarmonyPatch(typeof(Immigration), nameof(Immigration.ConfigureCarePackages))]
        public class AdditionalCarePackages
        {
            [HarmonyPrepare]
            static bool Prepare() => Config.Instance.AddAdditionalCarePackages;
            public static void Postfix(Immigration __instance)
            {
                __instance.carePackages.AddRange(ModAssets.GetAdditionalCarePackages());
            }
        }


        [HarmonyPatch(typeof(CryoTank))]
        [HarmonyPatch(nameof(CryoTank.DropContents))]
        public class AddToCryoTank
        {
            public static void Prefix()
            {
                if (Config.Instance.JorgeAndCryopodDupes)
                {
                    ModAssets.EditingSingleDupe = true;
                    ImmigrantScreen.InitializeImmigrantScreen(null);
                }
            }
            public static void Postfix(CryoTank __instance)
            {
                if (Config.Instance.JorgeAndCryopodDupes)
                {
                    SgtLogger.l("Getting CryoDupe gameobject");
                    CryoDupeToApplyStatsOn = __instance.smi.sm.defrostedDuplicant.Get(__instance.smi);
                }
            }
        }
        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.GenerateCharacter))]
        public class OverwriteRngGeneration
        {
            public static bool Prefix(CharacterContainer __instance, KButton ___selectButton, string guaranteedAptitudeID)
            {
                if (ModAssets.EditingSingleDupe)
                {
                    SgtLogger.l("editingSingleDupe");

                    if (CryoDupeToApplyStatsOn != null
                        && CryoDupeToApplyStatsOn.TryGetComponent<MinionIdentity>(out var minionIdentity)
                        && Db.Get().Personalities.Get(minionIdentity.personalityResourceId) != null)
                    {
                        var originPersonality = Db.Get().Personalities.Get(minionIdentity.personalityResourceId);
                        __instance.stats = new MinionStartingStats(originPersonality, guaranteedAptitudeID, guaranteedTraitID: "AncientKnowledge");
                        //ModAssets.ApplySkinFromPersonality(originPersonality, __instance.stats);
                        //__instance.characterNameTitle.OnEndEdit(originPersonality.Name);
                    }
                    else
                    {
                        __instance.stats = new MinionStartingStats(is_starter_minion: false, guaranteedAptitudeID, guaranteedTraitID: "AncientKnowledge");
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
                    SgtLogger.l(__instance.stats.voiceIdx + " <- voiceidx");
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

        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.GenerateCharacter))]
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
        //[HarmonyPatch(typeof(Chatty))]
        //[HarmonyPatch(nameof(Chatty.OnStartedTalking))]
        //public class chattytest
        //{
        //    [HarmonyPriority(Priority.HigherThanNormal)]

        //    public static void Prefix(object data, Chatty __instance)
        //    {
        //        if(data is MinionIdentity other)
        //        {
        //            SgtLogger.l("smc other");
        //            other.TryGetComponent(out StateMachineController smc);

        //        }
        //        if(data is ConversationManager.StartedTalkingEvent evt)
        //        {
        //            SgtLogger.Assert("talker",evt.talker);
        //            SgtLogger.Assert("talker go",evt.talker.gameObject);
        //            SgtLogger.Assert("talker tryget", evt.talker.TryGetComponent(out other));
        //            SgtLogger.Assert("other go", other);
        //            SgtLogger.l("smc talker");
        //            other.TryGetComponent(out StateMachineController smc);
        //        }
        //    }
        //}
        /// <summary>
		/// Applied before OnStartedTalking runs.
		/// </summary>
		[HarmonyPriority(Priority.LowerThanNormal)]
        internal static bool Prefix(object data, Chatty __instance)
        {
            if ((data is MinionIdentity other || (data is ConversationManager.
                    StartedTalkingEvent evt && evt.talker != null && evt.talker.
                    TryGetComponent(out other))) &&
                    other != null && other != __instance.identity)
            {


                // Cannot talk to yourself (self)
                if (other.TryGetComponent(out StateMachineController smc))
                    smc.GetSMI<JoyBehaviourMonitor.Instance>()?.GoToOverjoyed();
                if (__instance.TryGetComponent(out smc))
                    smc.GetSMI<JoyBehaviourMonitor.Instance>()?.GoToOverjoyed();


            }
            __instance.conversationPartners.Clear();
            return false;
        }


        [HarmonyPatch(typeof(ImmigrantScreen))]
        [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
        public class CustomSingleForNoTelepad
        {
            static GameObject Spacer = null;
            public static bool Prefix(Telepad telepad, ImmigrantScreen __instance)
            {
                EditingSingleDupe = telepad == null;

                if ((EditingSingleDupe && Config.Instance.JorgeAndCryopodDupes) || Config.Instance.RerollDuringGame)
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
            [HarmonyPriority(Priority.Low)]
            public static bool Prefix(Telepad ___telepad, ImmigrantScreen __instance)
            {
                if (EditingSingleDupe)
                {
                    MinionStartingStats DupeToDeliver = (MinionStartingStats)ModAssets.SingleCharacterContainer.stats;
                    SgtLogger.l(DupeToDeliver.personality.IdHash.ToString(), "resourceID");
                    SgtLogger.l(DupeToDeliver.Name + " <- cryopod dupe´fin");

                    foreach (var trait in DupeToDeliver.Traits)
                        SgtLogger.l(trait.Name, "Trait ToApply");

                    if (CryoDupeToApplyStatsOn != null && CryoDupeToApplyStatsOn.TryGetComponent<Traits>(out var traits))
                    {
                        foreach (var trait in traits.GetTraitIds())
                        {
                            SgtLogger.l("purging existing trait: " + trait);
                            PurgingTraitComponentIfExists(trait, CryoDupeToApplyStatsOn);
                        }

                        traits.Clear();


                        if (CryoDupeToApplyStatsOn.TryGetComponent<MinionResume>(out var minionRes))
                        {
                            minionRes.AptitudeBySkillGroup.Clear();
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
                if (Config.Instance.RerollDuringGame)
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



                if (__instance.reshuffleButton == null || !Config.Instance.RerollDuringGame)
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
            public static GameObject DupeStatEditingButtonGO = null;
            public static void Postfix(DetailsScreen __instance)
            {
                if (true)
                {
                    SgtLogger.l("adding skin button to detailsScreen");

                    if (SkinButtonGO != null)
                        UnityEngine.Object.Destroy(SkinButtonGO);

                    var SkinButton = Util.KInstantiateUI<KButton>(__instance.CodexEntryButton.gameObject, __instance.CodexEntryButton.transform.parent.gameObject);
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
                        DupeSkinScreenAddon.ShowSkinScreen(null, __instance.target);
                    };
                    SkinButtonGO = SkinButton.gameObject;
                    SkinButtonGO.SetActive(false);
                }
                if (true)
                {
                    SgtLogger.l("adding edit button to detailsScreen");

                    if (DupeStatEditingButtonGO != null)
                        UnityEngine.Object.Destroy(DupeStatEditingButtonGO);

                    var EditButton = Util.KInstantiateUI<KButton>(__instance.CodexEntryButton.gameObject, __instance.CodexEntryButton.transform.parent.gameObject);
                    //SkinButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 20, 33f);
                    EditButton.ClearOnClick();
                    EditButton.name = "DupeStatEditingSideScreen";
                    UIUtils.AddSimpleTooltipToObject(EditButton.transform, STRINGS.UI.BUTTONS.DUPELICITYEDITINGBUTTONTOOLTIP, true, onBottom: true);
                    if (EditButton.transform.Find("Image").TryGetComponent<Image>(out var image))
                    {
                        image.sprite = Assets.GetSprite("icon_gear");
                    }

                    EditButton.onClick += () =>
                    {
                        DuplicityMainScreen.ShowWindow(__instance.target, () => { });
                    };
                    DupeStatEditingButtonGO = EditButton.gameObject;
                    DupeStatEditingButtonGO.SetActive(false);
                }
            }
        }
        [HarmonyPatch(typeof(DetailsScreen))]
        [HarmonyPatch(nameof(DetailsScreen.OnSelectObject))]
        public class ToggleSkinButtonVisibility
        {
            public static void Prefix(DetailsScreen __instance)
            {
                if (__instance.target == null || !__instance.target.TryGetComponent<MinionIdentity>(out _))
                {
                    AddSkinButtonToDetailScreen.SkinButtonGO?.SetActive(false);
                    AddSkinButtonToDetailScreen.DupeStatEditingButtonGO?.SetActive(false);
                    return;
                }

                bool debugActive = DebugHandler.InstantBuildMode || Game.Instance.SandboxModeActive;

                bool ShowDupeEditing = Config.Instance.DuplicityDupeEditor || debugActive;
                bool ShowSkinEditing = Config.Instance.LiveDupeSkins || debugActive;

                AddSkinButtonToDetailScreen.SkinButtonGO?.SetActive(ShowSkinEditing);
                AddSkinButtonToDetailScreen.DupeStatEditingButtonGO?.SetActive(ShowDupeEditing);
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
                if (__instance.bImmigrantAvailable == false && Mathf.Approximately(Math.Max(__instance.timeBeforeSpawn - dt, 0.0f), 0.0f) && Config.Instance.PauseOnReadyToPrint)
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
                    __instance.spawnInterval[i] = Mathf.RoundToInt(Config.Instance.PrintingPodRechargeTime * 600f);
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
            public static void Prefix(Immigration __instance)
            {

                if (__instance.spawnInterval.Length >= 2)
                {
                    __instance.spawnInterval[0] = Mathf.RoundToInt(Config.Instance.PrintingPodRechargeTimeFirst * 600f);
                    __instance.spawnInterval[1] = Mathf.RoundToInt(Config.Instance.PrintingPodRechargeTime * 600f);

                }
                //for (int i = 0; i < __instance.spawnInterval.Length; i++)
                //{
                //    __instance.spawnInterval[i] = Mathf.RoundToInt(ModConfig.Instance.PrintingPodRechargeTime * 600f);
                //}
                //__instance.timeBeforeSpawn = Mathf.RoundToInt(ModConfig.Instance.PrintingPodRechargeTime * 600f);
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
                if (Config.Instance.JorgeAndCryopodDupes)
                {
                    SgtLogger.l("Getting Jorge Gameobject");
                    CryoDupeToApplyStatsOn = minionIdentity.gameObject;
                }
            }
            public static void Postfix()
            {
                SgtLogger.l("Start Editing Jorge");
                if (CryoDupeToApplyStatsOn && Config.Instance.JorgeAndCryopodDupes)
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

        [HarmonyPatch(typeof(Assets), nameof(Assets.OnPrefabInit))]
        public static class OnASsetPrefabPatch
        {
            public static void Postfix()
            {
                SgtLogger.l("Manually patching CharacterSelectionController..");
                CharacterSelectionController_Patch2.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
                CharacterSelectionController_Patch.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
                MinionSelectScreen_SetDefaultMinionsRoutine.AssetOnPrefabInitPostfix(Mod.harmonyInstance);
            }
        }

        /// <summary>
        /// Gets a prefab and applies "Care Packages Only"-Mode
        /// </summary>
        //[HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.InitializeContainers))]
        public class CharacterSelectionController_Patch2
        {
            public static void AssetOnPrefabInitPostfix(Harmony harmony)
            {
                var m_TargetMethod = AccessTools.Method("CharacterSelectionController, Assembly-CSharp:InitializeContainers");
                var m_Transpiler = AccessTools.Method(typeof(CharacterSelectionController_Patch2), "Transpiler");
                var m_Prefix = AccessTools.Method(typeof(CharacterSelectionController_Patch2), "Prefix");
                //var m_Postfix = AccessTools.Method(typeof(CharacterSelectionController_Patch2), "Postfix");

                harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix),
                   null, //new HarmonyMethod(m_Postfix),
                    new HarmonyMethod(m_Transpiler));
            }


            public static CharacterSelectionController instance;
            public static void Prefix(CharacterSelectionController __instance, KButton ___proceedButton)
            {
                instance = __instance;
                NextButtonPrefab = Util.KInstantiateUI(___proceedButton.gameObject);
                //UIUtils.ListAllChildren(NextButtonPrefab.transform);
                NextButtonPrefab.name = "CycleButtonPrefab";
            }

            //public static void Postfix(KButton ___proceedButton)
            //{
            //    //Debug.Log("Creating PREFAB2");
            //    NextButtonPrefab = Util.KInstantiateUI(___proceedButton.gameObject);
            //    //UIUtils.ListAllChildren(NextButtonPrefab.transform);
            //    NextButtonPrefab.name = "CycleButtonPrefab";
            //}
            public static void CarePackagesOnly()
            {
                if (instance is MinionSelectScreen)
                {
                    SgtLogger.l("skipping care package only for start screen");
                    return;
                }

                if (Config.Instance.CarePackagesOnly && Components.LiveMinionIdentities.Count >= Config.Instance.CarePackagesOnlyDupeCap)
                {
                    instance.numberOfCarePackageOptions = Config.Instance.CarePackagesOnlyPackageCount;
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
               typeof(CharacterSelectionController_Patch2),
               nameof(CharacterSelectionController_Patch2.CarePackagesOnly));

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
                else
                {
                    SgtLogger.error("CarePackagesOnly Transpiler failed!");
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
                int dupeCount = Components.LiveMinionIdentities.Count;

                if (Config.Instance.StartupResources && dupeCount > 3)
                {
                    GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
                    float OxiliteNeeded = OxilitePerDupePerDay * Config.Instance.SupportedDays * (dupeCount - 3);
                    float FoodeNeeded = FoodBarsPerDupePerDay * Config.Instance.SupportedDays * (dupeCount - 3);
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
            [HarmonyPostfix]
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
                if (!EditingSingleDupe && (Config.Instance.RerollDuringGame || IsStartDupe))
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
                            var type = GetTraitListOfTrait(UnityTraitRerollingScreen.GetTraitId(__instance));
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


            /// <summary>
            /// Adding the dss screen to each characterContainer
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="___stats"></param>
            /// <param name="__state"></param>
            [HarmonyPostfix]
            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats)
            {
                bool is_starter = __instance.controller is MinionSelectScreen;

                bool AllowModification = Config.Instance.ModifyDuringGame || (EditingSingleDupe && Config.Instance.JorgeAndCryopodDupes);
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

                UIUtils.AddActionToButton(skinBtn.transform, "", () => DupeSkinScreenAddon.ShowSkinScreen(__instance));


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
                changebtn.TryGetComponent<ToolTip>(out var tt);
                changebtn.TryGetComponent<KButton>(out var btn);

                if (AddNewToTraitsButtonPrefab == null)
                {
                    AddNewToTraitsButtonPrefab = Util.KInstantiateUI(buttonPrefab);
                    AddNewToTraitsButtonPrefab.GetComponent<ToolTip>().enabled = false;
                    AddNewToTraitsButtonPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("icon_positive");
                    AddNewToTraitsButtonPrefab.name = "AddButton";
                }
                if (RemoveFromTraitsButtonPrefab == null)
                {
                    RemoveFromTraitsButtonPrefab = Util.KInstantiateUI(buttonPrefab);
                    RemoveFromTraitsButtonPrefab.GetComponent<ToolTip>().enabled = false;
                    RemoveFromTraitsButtonPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("cancel");
                    RemoveFromTraitsButtonPrefab.name = "RemoveButton";
                }

                var detailsSection = __instance.transform.Find("Details").gameObject;
                GameObject dssSection;

                if (__instance.transform.Find("ModifyDupeStats") != null)
                    dssSection = __instance.transform.Find("ModifyDupeStats").gameObject;
                else
                {
                    dssSection = Util.KInstantiateUI(StartPrefab, __instance.gameObject, true);
                    dssSection.AddOrGet<DupeTraitManager>().InitUI();
                    dssSection.name = "ModifyDupeStats";
                }

                var mng = dssSection.AddOrGet<DupeTraitManager>();

                ToggleEditButtonState(mng.CurrentlyEditing, __instance, detailsSection, dssSection, img, tt);

                btn.onClick += () =>
                {
                    mng.CurrentlyEditing = !mng.CurrentlyEditing;
                    ToggleEditButtonState(mng.CurrentlyEditing, __instance, detailsSection, dssSection, img, tt);

                    if (!mng.CurrentlyEditing)
                    {
                        __instance.SetInfoText();
                        __instance.SetAttributes();
                        __instance.SetAnimator();
                    }
                };

                float insetDistancePresetButton = insetBase + insetB;
                ///Make Preset button
                var PresetButton = Util.KInstantiateUI(buttonPrefab, titlebar);
                PresetButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, insetDistancePresetButton, PresetButton.rectTransform().sizeDelta.x);
                PresetButton.name = "DupePresetButton";
                PresetButton.GetComponent<ToolTip>().toolTip = STRINGS.UI.BUTTONS.PRESETWINDOWBUTTONTOOLTIP;

                PresetButton.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconPaste");
                //var currentlySelectedIdentity = __instance.GetComponent<MinionIdentity>();

                //UIUtils.AddActionToButton(PresetButton.transform, "", () => DupePresetScreenAddon.ShowPresetScreen(__instance, ___stats)); 

                UIUtils.AddActionToButton(PresetButton.transform, "", () => UnityPresetScreen.ShowWindow(mng.Stats, RebuildDupePanel));
                buttonsToDeactivateOnEdit[__instance].Add(PresetButton.FindComponent<KButton>());
            }
            static void ToggleEditButtonState(bool currentlyEditing, CharacterContainer characterContainer, GameObject detailsSection, GameObject DSS_Section, KImage btnImage, ToolTip tt)
            {
                tt.SetSimpleTooltip(currentlyEditing ? STRINGS.UI.BUTTONS.MODIFYBUTTONTOOLTIP2 : STRINGS.UI.BUTTONS.MODIFYBUTTONTOOLTIP);
                btnImage.sprite = Assets.GetSprite(currentlyEditing ? "iconSave" : "icon_gear");
                detailsSection.SetActive(!currentlyEditing);
                DSS_Section.SetActive(currentlyEditing);

                if (buttonsToDeactivateOnEdit.ContainsKey(characterContainer))
                {
                    foreach (var button in buttonsToDeactivateOnEdit[characterContainer])
                    {
                        button.isInteractable = !currentlyEditing;
                    }
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
                if (Config.Instance.NoJoyReactions)
                {
                    __instance.joyTrait = Db.Get().traits.Get("None");
                }
                if (Config.Instance.NoStressReactions)
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

        public class MinionSelectScreen_SetDefaultMinionsRoutine
        {
            public static void AssetOnPrefabInitPostfix(Harmony harmony)
            {
                var m_TargetMethod = AccessTools.Method("MinionSelectScreen, Assembly-CSharp:SetDefaultMinionsRoutine");
                //var m_Transpiler = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Transpiler");
                var m_Postfix = AccessTools.Method(typeof(MinionSelectScreen_SetDefaultMinionsRoutine), "Postfix");

                harmony.Patch(m_TargetMethod, null, new HarmonyMethod(m_Postfix)
                    // , new HarmonyMethod(m_Transpiler)
                    );
            }

            private static System.Collections.IEnumerator MinionNumberAdustmentRoutine(MinionSelectScreen __instance)
            {

                yield return (object)SequenceUtil.WaitForNextFrame;
                yield return (object)SequenceUtil.WaitForNextFrame;
                yield return (object)SequenceUtil.WaitForNextFrame;
                yield return (object)SequenceUtil.WaitForNextFrame;

                int currentCount = __instance.containers.Count;
                int targetCount = Math.Max(1, Config.Instance.DuplicantStartAmount);
                if (currentCount == targetCount)
                {
                    yield break;
                }
                else if (currentCount < targetCount)
                {
                    for (int i = 0; i < targetCount - currentCount; i++)
                    {
                        CharacterContainer characterContainer = Util.KInstantiateUI<CharacterContainer>(__instance.containerPrefab.gameObject, __instance.containerParent);
                        characterContainer.SetController(__instance);
                        __instance.containers.Add(characterContainer);
                    }
                }
                else if (currentCount > targetCount)
                {
                    for (int i = 0; i < currentCount - targetCount; i++)
                    {
                        if (__instance.containers.Count > 1)
                        {
                            var container = __instance.containers[0];
                            __instance.containers.Remove(container);
                            UnityEngine.Object.Destroy(container.GetGameObject());
                        }
                    }
                }
            }

            public static void Postfix(MinionSelectScreen __instance)
            {
                SgtLogger.l("MinionSelectScreen postfix");
                __instance.StartCoroutine(MinionNumberAdustmentRoutine(__instance));
            }
        }
        //[HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.InitializeContainers))]
        public class CharacterSelectionController_Patch
        {
            public static void AssetOnPrefabInitPostfix(Harmony harmony)
            {
                var m_TargetMethod = AccessTools.Method("CharacterSelectionController, Assembly-CSharp:InitializeContainers");
                //var m_Transpiler = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Transpiler");
                var m_Prefix = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Prefix");
                var m_Postfix = AccessTools.Method(typeof(CharacterSelectionController_Patch), "Postfix");

                harmony.Patch(m_TargetMethod, new HarmonyMethod(m_Prefix), new HarmonyMethod(m_Postfix)
                    // , new HarmonyMethod(m_Transpiler)
                    );
            }

            //public static int CustomStartingDupeCount(int dupeCount) ///int requirement to consume previous "3" on stack
            //{
            //    if (dupeCount == 3 && CharacterSelectionController_Patch2.instance is MinionSelectScreen)
            //        return Config.Instance.DuplicantStartAmount; ///push new value to the stack
            //    else return dupeCount;
            //}

            //public static readonly MethodInfo AdjustNumber = AccessTools.Method(
            //   typeof(CharacterSelectionController_Patch),
            //   nameof(CustomStartingDupeCount));

            //[HarmonyPriority(Priority.VeryLow)]
            //static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            //{
            //    var code = instructions.ToList();
            //    var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Ldc_I4_3);

            //    //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
            //    if (insertionIndex != -1)
            //    {
            //        code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, AdjustNumber));
            //    }
            //    return code;
            //}

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
                            scroll.scrollSensitivity = 150;
                            scroll.movementType = ScrollRect.MovementType.Clamped;
                            scroll.inertia = false;
                            ///setting start pos
                            layout.transform.parent.rectTransform().pivot = new Vector2(0.5f, 0.99f);

                            ///top & bottom padding
                            layout.transform.parent.TryGetComponent<VerticalLayoutGroup>(out var verticalLayoutGroup);
                            verticalLayoutGroup.padding = new RectOffset(00, 00, 50, 50);
                            layout.childAlignment = TextAnchor.UpperCenter;
                            int countPerRow = Config.Instance.DuplicantStartAmount;

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
                            locText.key = Config.Instance.DuplicantStartAmount == 1 ? "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURLONECREWMAN" : "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURCREW";
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


        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.SetInfoText))]
        public static class CharacterContainer_SetInfoText_Patch_ContainerSize
        {
            /// <summary>
            /// Remove prev height so additional traits extend the box indstead of going hidden
            /// </summary>
            /// <param name="__instance"></param>
            public static void Postfix(CharacterContainer __instance)
            {
                if (__instance.aptitudeEntry.transform.parent.parent.gameObject.TryGetComponent<LayoutElement>(out LayoutElement layoutElement))
                {
                    layoutElement.preferredHeight = -1;
                }
            }
        }
        [HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.SetMinion))]
        public static class RefreshStatsForFreyja
        {
            [HarmonyPriority(Priority.Low - 1)]
            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats)
            {
                var mngt = __instance.transform.Find("ModifyDupeStats");
                if (mngt == null)
                {
                    SgtLogger.log("StatManager not found, skipping assignment..")
                      ; return;
                }
                var mng = mngt.gameObject.GetComponent<DupeTraitManager>();
                if (mng != null)
                {
                    mng.SetReferenceStats(___stats);
                }
                else
                    SgtLogger.warning("dupe mng was null!");
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



            //public static void Prefix(CharacterContainer __instance, MinionStartingStats ___stats, bool is_starter, ref DupeTraitManager __state)
            //{
            //    __state = __instance.transform.Find("ModifyDupeStats").gameObject.GetComponent<DupeTraitManager>();

            //}
            [HarmonyPriority(Priority.Low - 1)]
            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats)
            {
                var mngt = __instance.transform.Find("ModifyDupeStats");
                if (mngt == null)
                {

                    SgtLogger.log("StatManager not found, skipping assignment..")
                      ; return;
                }
                var mng = mngt.gameObject.GetComponent<DupeTraitManager>();
                if (mng != null)
                {
                    mng.SetReferenceStats(___stats);
                }
                else
                    SgtLogger.warning("dupe mng was null!");
            }

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

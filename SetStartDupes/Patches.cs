using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.UI;
using UtilLibs;
using static SetStartDupes.ModAssets;

namespace SetStartDupes
{
    class Patches
    {

        //[HarmonyPatch(typeof(CryoTank))]
        //[HarmonyPatch(nameof(CryoTank.DropContents))]
        //public class AddToCryoTank
        //{
        //    public static void Prefix()
        //    {
        //        ModAssets.EditingSingleDupe = true;
        //        ImmigrantScreen.InitializeImmigrantScreen(null);
        //        //SingleDupeImmigrandScreen.InitializeSingleImmigrantScreen(null);
        //    }
        //    public static void Postfix(CryoTank.StatesInstance __instance)
        //    {
        //        Debug.Log("asssaaaaaaats " + _TargetStats);
        //        if (ModAssets._TargetStats != null)
        //        {
        //            var dupe = __instance.sm.defrostedDuplicant.Get(__instance);
        //            ModAssets._TargetStats.Apply(dupe);
        //            Debug.Log("Dupee " + dupe);

        //            dupe.GetComponent<MinionIdentity>().arrivalTime = UnityEngine.Random.Range(-2000, -1000);
        //            MinionResume component = dupe.GetComponent<MinionResume>();
        //            int num = 3;
        //            for (int i = 0; i < num; i++)
        //            {
        //                component.ForceAddSkillPoint();
        //            }


        //            ModAssets._TargetStats = null;
        //        }
        //    }
        //}
        //[HarmonyPatch(typeof(CharacterContainer))]
        //[HarmonyPatch(nameof(CharacterContainer.GenerateCharacter))]
        //public class OverwriteRngGeneration
        //{
        //    public static bool Prefix(CharacterContainer __instance, KButton ___selectButton)
        //    {
        //        if (ModAssets.EditingSingleDupe)
        //        {

        //            if (ModAssets._TargetStats != null)
        //            {
        //                __instance.stats = ModAssets._TargetStats;
        //            }
        //            else
        //            {
        //                __instance.stats = new MinionStartingStats(is_starter_minion: false, null, "AncientKnowledge");
        //            }

        //            __instance.SetAnimator();
        //            __instance.SetInfoText();
        //            __instance.StartCoroutine(__instance.SetAttributes());
        //            ___selectButton.ClearOnClick();
        //            ___selectButton.enabled = true;
        //            ___selectButton.onClick += delegate
        //            {
        //                __instance.SelectDeliverable();
        //            };



        //            ModAssets._TargetStats = __instance.stats;
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        //[HarmonyPatch(typeof(ImmigrantScreen))]
        //[HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
        //public class CustomSingleForNoTelepad
        //{
        //    public static bool Prefix(Telepad telepad, ImmigrantScreen __instance)
        //    {
        //        if (telepad == null && EditingSingleDupe)
        //        {
        //            __instance.DisableProceedButton();

        //            if (__instance.containers != null && __instance.containers.Count > 1)
        //            {
        //                foreach(var container in __instance.containers)
        //                {
        //                    UnityEngine.Object.Destroy(container.GetGameObject());
        //                }
        //                __instance.containers.Clear();
        //            }

        //            __instance.containers = new List<ITelepadDeliverableContainer>();

        //            CharacterContainer characterContainerZZZ = Util.KInstantiateUI<CharacterContainer>(__instance.containerPrefab.gameObject, __instance.containerParent);
        //            characterContainerZZZ.SetController(__instance);

        //            __instance.containers.Add(characterContainerZZZ);
        //            __instance.selectedDeliverables = new List<ITelepadDeliverable>();
        //            __instance.AddDeliverable(characterContainerZZZ.stats);

        //            foreach (ITelepadDeliverableContainer container in __instance.containers)
        //            {
        //                CharacterContainer characterContainer = container as CharacterContainer;
        //                if ((UnityEngine.Object)characterContainer != (UnityEngine.Object)null)
        //                    characterContainer.SetReshufflingState(false);
        //            }
        //            __instance.EnableProceedButton();
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        //[HarmonyPatch(typeof(ImmigrantScreen), "OnProceed")]
        //public class SkipTelepadStuff
        //{
        //    public static bool Prefix(Telepad ___telepad, ImmigrantScreen __instance)
        //    {
        //        if (___telepad == null && EditingSingleDupe)
        //        {
        //            var containerField = AccessTools.Field(typeof(CharacterSelectionController), "containers");
        //            var __containers = (List<ITelepadDeliverableContainer>)containerField.GetValue(__instance);
        //            var deliverablesField = AccessTools.Field(typeof(CharacterSelectionController), "selectedDeliverables");

        //            var DupeToDeliver = (MinionStartingStats)((List<ITelepadDeliverable>)deliverablesField.GetValue(__instance)).First();
        //            Debug.Log("AAAAAASSSSSSS " + DupeToDeliver);
        //            ModAssets._TargetStats = DupeToDeliver;

        //            __instance.Show(false);
        //            if (__containers != null)
        //            {
        //                __containers.ForEach((System.Action<ITelepadDeliverableContainer>)(cc => UnityEngine.Object.Destroy((UnityEngine.Object)cc.GetGameObject())));
        //                __containers.Clear();
        //            }
        //            AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
        //            AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
        //            MusicManager.instance.PlaySong("Stinger_NewDuplicant");
        //            EditingSingleDupe = false;
        //            return false;
        //        }
        //        return true;
        //    }
        //}

        [HarmonyPatch(typeof(ImmigrantScreen))]
        [HarmonyPatch(nameof(ImmigrantScreen.Initialize))]
        public class AddRerollButtonIfEnabled
        {
            public static void Postfix(Telepad telepad, ImmigrantScreen __instance)
            {
                if (ModConfig.Instance.RerollDuringGame)
                {
                    foreach (ITelepadDeliverableContainer container in __instance.containers)
                    {
                        CharacterContainer characterContainer = container as CharacterContainer;
                        CarePackageContainer carePackContainer = container as CarePackageContainer;
                        if (characterContainer != null)
                        {
                            characterContainer.SetReshufflingState(true);
                        }
                        if (carePackContainer != null)
                        {
                            carePackContainer.SetReshufflingState(true);
                            carePackContainer.reshuffleButton.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 120f);
                            carePackContainer.reshuffleButton.onClick += () =>
                            {
                                carePackContainer.Reshuffle(false);
                            };
                            UIUtils.AddSimpleTooltipToObject(carePackContainer.reshuffleButton.transform, STRINGS.UI.BUTTONS.REROLLCAREPACKAGE,true, onBottom:true);
                        }
                    }
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
                __instance.spawnInterval[__instance.spawnInterval.Length - 1] = Mathf.RoundToInt(ModConfig.Instance.PrintingPodRechargeTime * 600f);
                SgtLogger.l(__instance.timeBeforeSpawn.ToString());
                for(int i = 0; i < __instance.spawnInterval.Length; i++)
                {
                    SgtLogger.l(__instance.spawnInterval[i].ToString(), i.ToString());
                }
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
                    PrimaryElement component2 = go.GetComponent<PrimaryElement>();
                    component2.Units = FoodeNeeded;
                    go.SetActive(true);
                }
            }

            static void YeetOxilite(GameObject originGo, float amount)
            {

                GameObject go = Util.KInstantiate(Assets.GetPrefab(FieldRationConfig.ID));
                go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(originGo), Grid.SceneLayer.Ore));
                PrimaryElement component2 = go.GetComponent<PrimaryElement>();
                component2.Units = amount;
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


            [HarmonyPriority(Priority.Last)]
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

        [HarmonyPatch(typeof(MinionStartingStats), "GenerateTraits")]
        [HarmonyPatch(nameof(MinionStartingStats.GenerateTraits))]
        public class AllowCustomTraitAllignment
        {
            ///Rework
            public static bool VariableTraits(bool isStarterMinion) 
            {
                return false;
            }

            public static readonly MethodInfo overrideStarterGeneration = AccessTools.Method(
               typeof(AllowCustomTraitAllignment),
               nameof(AllowCustomTraitAllignment.VariableTraits));

            public static readonly MethodInfo PreviousCellXY = AccessTools.Method(
               typeof(Grid),
               nameof(Grid.XYToCell));

            [HarmonyPriority(Priority.Last)]
            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindLastIndex(ci => ci.opcode == OpCodes.Ldfld && ci.operand.ToString().Contains("is_starter_minion"));

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                    //++insertionIndex;
                    //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, 7));
                    code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, overrideStarterGeneration));
                    //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Stloc_S,  7));
                    //code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_S, 7));
                }
                //foreach (var v in code) { Console.WriteLine(v.opcode + (v.operand != null ? ": " + v.operand : "")); };
                return code;
            }
        }
        
        [HarmonyPatch(typeof(CharacterSelectionController), nameof(CharacterSelectionController.InitializeContainers))]
        public class CharacterSelectionController_Patch
        {
            public static int CustomStartingDupeCount(int dupeCount) ///int requirement to consume previous "3" on stack
            {
                if (dupeCount == 3&& controller2_patch.instance  is MinionSelectScreen)
                    return ModConfig.Instance.DuplicantStartAmount; ///push new value to the stack
                else return dupeCount;
            }

            public static readonly MethodInfo AdjustNumber = AccessTools.Method(
               typeof(CharacterSelectionController_Patch),
               nameof(CustomStartingDupeCount));

            [HarmonyPriority(Priority.Last)]
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

                Debug.Log(__instance.GetType());
                GameObject parentToScale = (GameObject)typeof(CharacterSelectionController).GetField("containerParent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                CharacterContainer prefabToScale = (CharacterContainer)typeof(CharacterSelectionController).GetField("containerPrefab", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);

                if (__instance.GetType() == typeof(MinionSelectScreen))
                {
#if DEBUG
                    Debug.Log("Manipulating Instance: " + __instance.GetType());

                    //UIUtils.ListAllChildren(__instance.transform);
                    // UIUtils.ListAllChildrenWithComponents(__instance.transform);

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

                    UnityPresetScreen.parentScreen = __instance.transform.parent.gameObject;
                }

                else
                {
                    UnityPresetScreen.parentScreen = PauseScreen.Instance.transform.parent.gameObject;
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
                    return;

                LocText[] objectsOfType1 = UnityEngine.Object.FindObjectsOfType<LocText>();
                if (objectsOfType1 != null)
                {
                    foreach (LocText locText in objectsOfType1)
                    {
                        if (locText.key == "STRINGS.UI.IMMIGRANTSCREEN.SELECTYOURCREW")
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

        [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
        public static class AddChangeButtonToCharacterContainer
        {
            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats, bool is_starter)
            {
                ///Only during startup when config is disabled

                //bool IsWhackyDupe = false;
                //Type BioInksCustomDupeType = Type.GetType("PrintingPodRecharge.Cmps.CustomDupe, PrintingPodRecharge", false, false);
                //if(BioInksCustomDupeType != null)
                //{

                //    //var obj = go.gameObject.GetComponent(VaricolouredBalloonsHelperType);
                //    ////foreach (var cmp in VaricolouredBalloonsHelperType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)) 
                //    ////   SgtLogger.l(cmp.Name.ToString(),"GET Field");
                //    ////foreach (var cmp in VaricolouredBalloonsHelperType.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic))
                //    ////    SgtLogger.l(cmp.Name.ToString(), "GET method");

                //    //var component = go.GetComponent(VaricolouredBalloonsHelperType);
                //    //var fieldInfo = (uint)Traverse.Create(component).Method("get_ArtistBalloonSymbolIdx").GetValue();
                //}


                List<KButton> ButtonsToDisableOnEdit = new List<KButton>();

                var buttonPrefab = __instance.transform.Find("TitleBar/RenameButton").gameObject;
                var titlebar = __instance.transform.Find("TitleBar").gameObject;

                //28
                int insetBase = 4, insetA = 28, insetB = insetA * 2, insetC = insetA * 3;
                float insetDistance = (!is_starter && !ModConfig.Instance.ModifyDuringGame) ? insetBase+ insetA : insetBase + insetC;

                //var TextInput = titlebar.transform.Find("LabelGroup/");
                //TextInput.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 3, 60);



                ///Make skin button
                var skinBtn = Util.KInstantiateUI(buttonPrefab, titlebar);
                skinBtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, insetDistance, skinBtn.rectTransform().sizeDelta.x);
               
                skinBtn.name = "DupeSkinButton";
                skinBtn.GetComponent<ToolTip>().toolTip = STRINGS.UI.BUTTONS.DUPESKINBUTTONTOOLTIP;

                skinBtn.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("ic_dupe");


                ButtonsToDisableOnEdit.Add(skinBtn.FindComponent<KButton>());

                //var currentlySelectedIdentity = __instance.GetComponent<MinionIdentity>();

                System.Action RebuildDupePanel = () =>
                {
                    __instance.SetInfoText();
                    __instance.SetAttributes();
                    __instance.SetAnimator();
                };

                UIUtils.AddActionToButton(skinBtn.transform, "", () => DupeSkinScreenAddon.ShowSkinScreen(__instance, ___stats));

                

                if(!(!is_starter && !ModConfig.Instance.ModifyDuringGame))
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
                    ButtonsToDisableOnEdit.Add(PresetButton.FindComponent<KButton>());
                }

                if (!is_starter && !ModConfig.Instance.ModifyDuringGame)
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

                ButtonsToDisableOnEdit.Add(button);
                ButtonsToDisableOnEdit.Add(button2);

                ChangeButton(false, changebtn, __instance, ___stats, ButtonsToDisableOnEdit, RebuildDupePanel);

                AddNewToTraitsButtonPrefab = Util.KInstantiateUI(buttonPrefab);
                AddNewToTraitsButtonPrefab.GetComponent<ToolTip>().enabled = false;
                AddNewToTraitsButtonPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("icon_positive");
                AddNewToTraitsButtonPrefab.name = "AddButton";

                RemoveFromTraitsButtonPrefab = Util.KInstantiateUI(buttonPrefab);
                RemoveFromTraitsButtonPrefab.GetComponent<ToolTip>().enabled = false;
                RemoveFromTraitsButtonPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("action_deconstruct");
                RemoveFromTraitsButtonPrefab.name = "RemoveButton";

            }

            static void ChangeButton(bool isCurrentlyInEditMode, GameObject buttonGO, CharacterContainer parent, MinionStartingStats referencedStats, List<KButton> ButtonsToDisable, System.Action OnClose)
            {
                buttonGO.GetComponent<ToolTip>().SetSimpleTooltip(!isCurrentlyInEditMode ? STRINGS.UI.BUTTONS.MODIFYBUTTONTOOLTIP : STRINGS.UI.BUTTONS.MODIFYBUTTONTOOLTIP2);
                var img = buttonGO.transform.Find("Image").GetComponent<KImage>();
                img.sprite = Assets.GetSprite(!isCurrentlyInEditMode ? "icon_gear" : "iconSave");
                var button = buttonGO.GetComponent<KButton>();
                button.ClearOnClick();
                button.onClick += () =>
                {
                    ChangeButton(!isCurrentlyInEditMode, buttonGO, parent, referencedStats, ButtonsToDisable, OnClose);
                    if (isCurrentlyInEditMode)
                    {
                        InstantiateOrGetDupeModWindow(parent.gameObject, referencedStats, true);
                        OnClose.Invoke();
                    }
                    else
                    {
                        InstantiateOrGetDupeModWindow(parent.gameObject, referencedStats, false);
                    }
                    foreach(var button in ButtonsToDisable)
                    {
                        button.isInteractable = isCurrentlyInEditMode;
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
                    ParentContainer.name = "oldd";
                    ParentContainer.gameObject.SetActive(false);

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
                    }


                    var renameLabel = prefabParent.transform.Find("SelectLabel");
                    if (renameLabel != null)
                    {
                        renameLabel.name = "Label";
                    }

                    prefabParent.GetComponent<LayoutElement>().minHeight = 25;
                    prefabParent.GetComponent<LayoutElement>().preferredHeight = 30;
                    var spacerParent = Util.KInstantiateUI(prefabParent.transform.Find("Label").gameObject);
                    var AddOnSpacer = Util.KInstantiateUI(AddNewToTraitsButtonPrefab, spacerParent);
                    UIUtils.TryFindComponent<ToolTip>(AddOnSpacer.transform).toolTip = STRINGS.UI.BUTTONS.ADDTOSTATS;
                    //UIUtils.TryFindComponent<ToolTip>(right.transform,"Image").toolTip="Cycle to next";
                    AddOnSpacer.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 0, 25);
                    AddOnSpacer.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -5, 25);
                    AddOnSpacer.SetActive(false);



                    //skillMod.transform.Find("DetailsContainer").gameObject.SetActive(false);

                    if (!ModAssets.DupeTraitManagers.ContainsKey(referencedStats))
                    {
                        DupeTraitManagers[referencedStats] = new DupeTraitManager();
                        DupeTraitManagers[referencedStats].SetReferenceStats(referencedStats);
                    }
                    var DupeTraitMng = DupeTraitManagers[referencedStats];



                    var Spacer2AndInterestHolder = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);

                    UIUtils.TryChangeText(Spacer2AndInterestHolder.transform, "", global::STRINGS.UI.CHARACTERCONTAINER_APTITUDES_TITLE);
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

                        UIUtils.AddSimpleTooltipToObject(traitEntry.transform, v.GetTooltip(), true,onBottom: true);
                        var type = ModAssets.GetTraitListOfTrait(v.Id, out var list);

                        TraitsToSort.Add(new Tuple<GameObject, DupeTraitManager.NextType>(traitEntry, type));

                        ApplyTraitStyleByKey(traitEntry.GetComponent<KImage>(), type);
                        traitEntry.GetComponent<KButton>().enabled = true;
                        UIUtils.AddActionToButton(traitEntry.transform, "", () =>
                        {
                            UnityTraitScreen.ShowWindow(referencedStats, ()=>InstantiateOrGetDupeModWindow(parent,referencedStats,hide), currentTrait: v);
                        });
                        
                        UIUtils.TryChangeText(traitEntry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, v.Name));
                        traitEntry.transform.Find("RemoveButton").gameObject.SetActive(ModConfig.Instance.AddAndRemoveTraitsAndInterests);

                        ApplyTraitStyleByKey(traitEntry.transform.Find("RemoveButton").gameObject.GetComponent<KImage>(), type);

                        UIUtils.AddActionToButton(traitEntry.transform, "RemoveButton", () =>
                        {
                            if(referencedStats.Traits.Contains(v))
                                referencedStats.Traits.Remove(v);
                            InstantiateOrGetDupeModWindow(parent, referencedStats, hide);
                        }
                        );
                    }

                    TraitsToSort = TraitsToSort.OrderBy(t => (int)t.second).ToList();
                    for (int i = 0; i < TraitsToSort.Count; i++)
                    {
                        TraitsToSort[i].first.transform.SetAsLastSibling();
                    }

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
                    UIUtils.AddSimpleTooltipToObject(JoyTrait.transform, referencedStats.joyTrait.GetTooltip(), true);

                    var spacerStress = Util.KInstantiateUI(spacerParent, ContentContainer.gameObject, true);
                    UIUtils.TryChangeText(spacerStress.transform, "", string.Format(global::STRINGS.UI.CHARACTERCONTAINER_STRESSTRAIT,string.Empty))
                        ;
                    var StressTrait = Util.KInstantiateUI(prefabParent, ContentContainer.gameObject, true);
                    StressTrait.GetComponent<KButton>().enabled = true;
                    UIUtils.AddActionToButton(StressTrait.transform, "", () =>
                    {
                        UnityTraitScreen.ShowWindow(referencedStats, () => InstantiateOrGetDupeModWindow(parent, referencedStats, hide), currentTrait: referencedStats.stressTrait);
                    });

                    ApplyTraitStyleByKey(StressTrait.GetComponent<KImage>(), DupeTraitManager.NextType.stress);

                    UIUtils.AddSimpleTooltipToObject(StressTrait.transform, referencedStats.stressTrait.GetTooltip(), true);
                    UIUtils.TryChangeText(StressTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, referencedStats.stressTrait.Name));                   
                }



                ParentContainer.gameObject.SetActive(!hide);
            }
            static  string GetSkillGroupName(SkillGroup Group) =>  Strings.Get("STRINGS.DUPLICANTS.SKILLGROUPS." + Group.Id.ToUpperInvariant() + ".NAME");
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

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
using UnityEngine.UI;
using UtilLibs;
using static SetStartDupes.ModAssets;

namespace SetStartDupes
{
    class Patches
    {
        //[HarmonyPatch(typeof(ImmigrantScreen), "OnPrefabInit")]
        //public class GrabButtpnPrefab
        //{
        //    public static void Postfix(KButton ___rejectButton)
        //    {
        //        Debug.Log("Creating PREFAB");
        //        NextButtonPrefab = Util.KInstantiateUI(___rejectButton.gameObject);
        //        UIUtils.ListAllChildren(NextButtonPrefab.transform);
        //        NextButtonPrefab.name = "CycleButtonPrefab";
        //    }
        //}
        [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
        public class GrabButtpnPrefab2
        {
            public static void Postfix(KButton ___proceedButton)
            {
                Debug.Log("Creating PREFAB2");
                NextButtonPrefab = Util.KInstantiateUI(___proceedButton.gameObject);
                UIUtils.ListAllChildren(NextButtonPrefab.transform);
                NextButtonPrefab.name = "CycleButtonPrefab";
            }
        }

        [HarmonyPatch(typeof(CharacterSelectionController), "InitializeContainers")]
        public class CharacterSelectionController_Patch
        {
            public static int CustomStartingDupeCount(int dupeCount) ///int requirement to consume previous "3" on stack
            {
                if (dupeCount == 3)
                    return StartDupeConfig.Instance.DuplicantStartAmount; ///push new value to the stack
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
                else

#if DEBUG
                    foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
#endif


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
                Debug.Log("Original Scale " + prefabToScale.baseCharacterScale);

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

                        int countPerRow = StartDupeConfig.Instance.DuplicantStartAmount;
                        if (countPerRow > 5)
                        {
                            if (countPerRow % 2 != 0)
                                countPerRow++;
                            countPerRow = countPerRow / 2;
                        }
                        layout.constraintCount = countPerRow;
#if DEBUG
                        Debug.Log("cellsize: " + layout.cellSize);
                        Debug.Log("Dupe COunt: " + StartDupeConfig.Instance.DuplicantStartAmount);
                        Debug.Log("Dupes Per Row " + countPerRow);

#endif                        //layout.cellSize = new(300, 400);
                    }
                }

                
                    //Debug.Log("PARENT: "+parentToScale.transform.localScale);
                    //prefabToScale.transform.localScale = new(0.8f,0.8f);
                    if (StartDupeConfig.Instance.DuplicantStartAmount > 5 && __instance.IsStarterMinion)
                    {
                        parentToScale.transform.parent.transform.localScale = new Vector3(0.6f, 0.6f);
                        //parentToScale.transform.localScale = new Vector3(0.6f, 0.6f);
                        prefabToScale.baseCharacterScale = 0.24f;
                        Debug.Log("Adjusting Scale to "+ prefabToScale.baseCharacterScale);
                        ModAssets.HasShrunkenDown = true;
                        ModAssets.PrefabToFix = prefabToScale;
                    }
                    //else
                    //{
                    //    //parentToScale.transform.parent.transform.localScale = new Vector3(0.6f, 0.6f);
                    //    if (HasShrunkenDown) 
                    //    {
                    //        parentToScale.transform.parent.transform.localScale = new Vector3(1f, 1f);
                    //        prefabToScale.baseCharacterScale = prefabToScale.baseCharacterScale * (0.6f/1f); HasShrunkenDown = false;
                    //    }
                    //}
                }
                else
                {
                    prefabToScale.baseCharacterScale = 0.4f;

#if DEBUG
                    Debug.Log("Adjusting Scale to " + prefabToScale.baseCharacterScale); 
#endif
                }

#if DEBUG
                //Debug.Log("PREFAB: " + size);
#endif
            }

            public static void Postfix(CharacterSelectionController __instance, CarePackageContainer ___carePackageContainerPrefab)
            {
                if (ModAssets.StartPrefab == null) { 
                    StartPrefab = ___carePackageContainerPrefab.transform.Find("Details").gameObject;
                    //StartPrefab.transform.Find("Top/PortraitContainer/PortraitContent").gameObject.SetActive(false);
                    StartPrefab.transform.name = "ModifyDupeStats";

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
                            locText.key = StartDupeConfig.Instance.DuplicantStartAmount == 1 ? "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURLONECREWMAN" : "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURCREW";
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
                Debug.Log("Closing start");
                UIUtils.ListAllChildren(__instance.transform);
                Debug.Log("Closing Stop");
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
            public static void Prefix(CharacterContainer __instance, Transform ___aptitudeLabel)
            {
            }
            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats, bool is_starter)
            {
                
                var buttonPrefab = __instance.transform.Find("TitleBar/RenameButton").gameObject;
                var titlebar = __instance.transform.Find("TitleBar").gameObject;
#if DEBUG
                Debug.Log("Start ChildrenList");
                UIUtils.ListAllChildren(__instance.transform);
                Debug.Log("Stop ChildrenList");
#endif

                var changebtn = Util.KInstantiateUI(buttonPrefab, titlebar);
                changebtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 40f, changebtn.rectTransform().sizeDelta.x);
                changebtn.name = "ChangeDupeStatButton";
                changebtn.GetComponent<ToolTip>().toolTip = "Adjust dupe stats";

                var img = changebtn.transform.Find("Image").GetComponent<KImage>();
                img.sprite = Assets.GetSprite("icon_gear");
                var button = changebtn.GetComponent<KButton>();
                ChangeButton(false, changebtn, __instance, ___stats);

                //NextButtonPrefab = Util.KInstantiateUI(buttonPrefab);
                ////NextButtonPrefab.transform.Find("Image").GetComponent<KImage>().sprite = Assets.GetSprite("iconRight");
                //UnityEngine.Object.Destroy(NextButtonPrefab.transform.Find("Image"));
                ////NextButtonPrefab.GetComponent<ToolTip>().toolTip = "Cycle to next";
                //UIUtils.ListAllChildren(NextButtonPrefab.transform);
                //NextButtonPrefab.name = "CycleButtonPrefab";
                ////NextButtonPrefab.AddComponent<LocText>().text = "FILLERTEXT";
            }

            static void ChangeButton(bool isCurrentlyInEditMode,GameObject buttonGO, CharacterContainer parent, MinionStartingStats referencedStats)
            {
                buttonGO.GetComponent<ToolTip>().toolTip = !isCurrentlyInEditMode ? "Adjust dupe stats":"Store Settings";
                var img = buttonGO.transform.Find("Image").GetComponent<KImage>();
                img.sprite = Assets.GetSprite(!isCurrentlyInEditMode ? "icon_gear": "iconSave");
                var button = buttonGO.GetComponent<KButton>();
                button.ClearOnClick();
                button.onClick += () =>
                {
                    ChangeButton(!isCurrentlyInEditMode, buttonGO,parent, referencedStats);
                    if (isCurrentlyInEditMode)
                    {
                        InstantiateOrGetDupeModWindow(parent.gameObject, referencedStats, true);
                        typeof(CharacterContainer).GetMethod("SetInfoText", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(parent, null );
                        typeof(CharacterContainer).GetMethod("SetAttributes", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(parent, null );
                    }
                    else
                    {
                        InstantiateOrGetDupeModWindow(parent.gameObject, referencedStats, false);
                    }
                };
                parent.transform.Find("Details").gameObject.SetActive(!isCurrentlyInEditMode);
            }

            static void InstantiateOrGetDupeModWindow(GameObject parent, MinionStartingStats referencedStats, bool hide )
            {
                bool ShouldInit = true;
                var ParentContainer = parent.transform.Find("ModifyDupeStats");
                if (ParentContainer == null)
                {
                    Debug.Log("HAD TO MAKE NEW");
                    ParentContainer = Util.KInstantiateUI(StartPrefab, parent).transform;
                    ParentContainer.gameObject.name = "ModifyDupeStats";
                }
                else
                {
                    Debug.Log("FOUND OLD");

                    ShouldInit = false;
                }


                
                if(ShouldInit) {
                    var one = ParentContainer.transform.Find("PortraitContainer");
                    if (one != null) UnityEngine.Object.Destroy(one.gameObject); 
                    var two = ParentContainer.transform.Find("DetailsContainer");
                    if (two != null) UnityEngine.Object.Destroy(two.gameObject);

                    var prefabParentTodo = ParentContainer.transform.Find("DescriptionGroup").gameObject;
                    prefabParentTodo.transform.Find("Description").gameObject.AddComponent<KButton>();


                    var prefabParent = NextButtonPrefab; //Util.KInstantiateUI(prefabParentTodo);


                    UIUtils.ListAllChildrenWithComponents(prefabParent.transform);

                    var renameLabel = prefabParent.transform.Find("SelectLabel");
                    if (renameLabel != null)
                    {
                        renameLabel.name = "Label";
                    }

                    Debug.Log(prefabParent.GetComponent<LayoutElement>().preferredHeight + "PREF HEIG");
                    Debug.Log(prefabParent.GetComponent<LayoutElement>().minHeight + "min HEIG");
                    prefabParent.GetComponent<LayoutElement>().minHeight = 20;
                    prefabParent.GetComponent<LayoutElement>().preferredHeight = 30;
                    var spacerParent = prefabParent.transform.Find("Label").gameObject;

                    //skillMod.transform.Find("DetailsContainer").gameObject.SetActive(false);
                    var UsedSkills = ParentContainer.FindOrAddComponent<HoldMyReferences>();

                    prefabParentTodo.SetActive(false);


                    var spacer2 = Util.KInstantiateUI(spacerParent, ParentContainer.gameObject, true);
                    UIUtils.TryChangeText(spacer2.transform, "", "INTERESTS");

                    foreach (var a in referencedStats.skillAptitudes)
                    {
                        for (int index2 = 0; index2 < a.Key.relevantAttributes.Count; ++index2)
                        {
                            UsedSkills.AddOrIncreaseToStat(a.Key.relevantAttributes[index2].Id);
                        }
                        var entry = Util.KInstantiateUI(prefabParent, ParentContainer.gameObject, true);
                        var name = entry.AddComponent<HoldMyString>();
                        name.Group = a.Key;

                        Klei.AI.Attribute plusAttribute = name.Group.relevantAttributes.First();
                        int plusstat = referencedStats.StartingLevels[plusAttribute.Id];



                        ApplyDefaultStyle(entry.GetComponent<KImage>());
                        UIUtils.TryChangeText(entry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, name.NAME(), name.Group.relevantAttributes.First().Id, plusstat));


                        UIUtils.AddActionToButton(entry.transform, "", () =>
                        {
                            //UsedSkills.InstantiateSingleStatView(null);
                            //return;
                        List<SkillGroup> list = new List<SkillGroup>((IEnumerable<SkillGroup>)Db.Get().SkillGroups.resources);
                        int i = list.FindIndex(item => item == name.Group);
                        ++i;
                        if (i == list.Count)
                            i = 0;

                        int counter = 0;
                        while (referencedStats.skillAptitudes.ContainsKey(list[i]))
                        {
                            ++i;
                            if (i == list.Count)
                                i = 0;
                            ++counter;
                            if (counter > 40) break;
                        }

                        referencedStats.skillAptitudes.Remove(name.Group);
                        referencedStats.skillAptitudes.Add(list[i], 1);
                            int statheight = 0;

                            foreach(var relevantStat in name.Group.relevantAttributes)
                            {
                                string statId = relevantStat.Id;
                                bool deleteOldBoost = UsedSkills.DoesRemoveReduceStats(statId, true);

                                statheight =  referencedStats.StartingLevels[statId] > statheight ? referencedStats.StartingLevels[statId] : statheight;
                                if (deleteOldBoost)
                                    referencedStats.StartingLevels[statId] = 0;
                            }
                            name.Group = list[i];

                            foreach (var relevantStat in name.Group.relevantAttributes)
                            {
                                string statId = relevantStat.Id;
                                referencedStats.StartingLevels[statId] = statheight;
                                UsedSkills.AddOrIncreaseToStat(relevantStat.Id);
                            }
                            UIUtils.TryChangeText(entry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.APTITUDEENTRY, name.NAME(), name.Group.relevantAttributes.First().Id, statheight));
                        }
                        );
                    }
                    var spacer3 = Util.KInstantiateUI(spacerParent, ParentContainer.gameObject, true);
                    UIUtils.TryChangeText(spacer3.transform, "", "TRAITS");
                    //Db.Get().traits.TryGet();

                    foreach (Trait v in referencedStats.Traits)
                    {
                        if (v.Name == "Duplicant")
                            continue;
                        var entry = Util.KInstantiateUI(prefabParent, ParentContainer.gameObject, true);
                        UsedSkills.AddTrait(v.Id);
                        var TraitHolder = entry.AddComponent<HoldMyString>();
                        TraitHolder.CurrentTrait = v;

                        if (v.PositiveTrait) 
                        {
                            ApplyGoodTraitStyle(entry.GetComponent<KImage>());
                            UIUtils.TryChangeText(entry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, v.Name));
                            UIUtils.AddActionToButton(entry.transform, "", () =>
                            {

                                string nextTraitId = UsedSkills.GetNextTraitId(TraitHolder.CurrentTrait.Id, HoldMyReferences.NextType.posTrait);
                                Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                                UsedSkills.ReplaceTrait(TraitHolder.CurrentTrait.Id, nextTraitId);
                                referencedStats.Traits.Remove(TraitHolder.CurrentTrait);
                                referencedStats.Traits.Add(NextTrait);
                                TraitHolder.CurrentTrait = NextTrait;

                                UIUtils.TryChangeText(entry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, TraitHolder.CurrentTrait.Name));
                            });
                        }
                        else
                        {
                            ApplyBadTraitStyle(entry.GetComponent<KImage>());
                            UIUtils.TryChangeText(entry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, v.Name));
                            UIUtils.AddActionToButton(entry.transform, "", () =>
                            {
                                string nextTraitId = UsedSkills.GetNextTraitId(TraitHolder.CurrentTrait.Id, HoldMyReferences.NextType.negTrait);
                                Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                                UsedSkills.ReplaceTrait(TraitHolder.CurrentTrait.Id, nextTraitId);
                                referencedStats.Traits.Remove(TraitHolder.CurrentTrait);
                                referencedStats.Traits.Add(NextTrait);
                                TraitHolder.CurrentTrait = NextTrait;

                                UIUtils.TryChangeText(entry.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.TRAIT, TraitHolder.CurrentTrait.Name));
                            });
                        }
                    }

                    var spacer = Util.KInstantiateUI(spacerParent, ParentContainer.gameObject, true);
                    UIUtils.TryChangeText(spacer.transform, "", "REACTIONS");

                    var JoyTrait = Util.KInstantiateUI(prefabParent, ParentContainer.gameObject, true);
                    UsedSkills.AddTrait(referencedStats.joyTrait.Id);

                    var JoyHolder = JoyTrait.AddComponent<HoldMyString>();
                    JoyHolder.CurrentTrait = referencedStats.joyTrait;
                    ApplyGoodTraitStyle(JoyTrait.GetComponent<KImage>());
                    UIUtils.TryChangeText(JoyTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.JOYREACTION, referencedStats.joyTrait.Name));
                    UIUtils.AddActionToButton(JoyTrait.transform, "", () =>
                    {
                        string nextTraitId = UsedSkills.GetNextTraitId(JoyHolder.CurrentTrait.Id, HoldMyReferences.NextType.joy);
                        Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                        UsedSkills.ReplaceTrait(JoyHolder.CurrentTrait.Id, nextTraitId);
                        referencedStats.joyTrait=NextTrait;
                        JoyHolder.CurrentTrait = NextTrait;

                        UIUtils.TryChangeText(JoyTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.JOYREACTION, referencedStats.joyTrait.Name));
                    }
                    );
                    
                    var StressTrait = Util.KInstantiateUI(prefabParent, ParentContainer.gameObject, true);

                    UsedSkills.AddTrait(referencedStats.stressTrait.Id);

                    ApplyBadTraitStyle(StressTrait.GetComponent<KImage>());
                    var StressHolder = JoyTrait.AddComponent<HoldMyString>();
                    StressHolder.CurrentTrait = referencedStats.stressTrait;

                    UIUtils.TryChangeText(StressTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.STRESSREACTION, referencedStats.stressTrait.Name));
                    UIUtils.AddActionToButton(StressTrait.transform, "", () =>
                    {
                        string nextTraitId = UsedSkills.GetNextTraitId(StressHolder.CurrentTrait.Id, HoldMyReferences.NextType.stress);
                        Trait NextTrait = Db.Get().traits.TryGet(nextTraitId);
                        UsedSkills.ReplaceTrait(StressHolder.CurrentTrait.Id, nextTraitId);
                        referencedStats.stressTrait = NextTrait;
                        StressHolder.CurrentTrait = NextTrait;

                        UIUtils.TryChangeText(StressTrait.transform, "Label", string.Format(STRINGS.UI.DUPESETTINGSSCREEN.STRESSREACTION, referencedStats.stressTrait.Name));
                    //UIUtils.TryChangeText(StressTrait.transform, "Label", "StressTrait: " + referencedStats.stressTrait.Name);
                }
                );
                }



                ParentContainer.gameObject.SetActive(!hide);
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

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
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static SetStartDupes.ModAssets;

namespace SetStartDupes
{
    class Patches
    {

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



        [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
        public static class AddChangeButtonToCharacterContainer
        {
            public static void Prefix(CharacterContainer __instance, Transform ___aptitudeLabel)
            {
            }
            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats)
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
                var skillMod = parent.transform.Find("ModifyDupeStats");
                if (skillMod == null)
                {
                    skillMod = Util.KInstantiateUI(StartPrefab, parent).transform;
                }

                StringBuilder sb = new();

                skillMod.transform.Find("PortraitContainer").gameObject.SetActive(false);
                var container = skillMod.transform.Find("DescriptionGroup").gameObject;//.gameObject.SetActive(false);
                skillMod.transform.Find("DetailsContainer").gameObject.SetActive(false);// .gameObject.SetActive(false);
                //container.name = "Entry";

                //skillMod.transform.Find("DetailsContainer").gameObject.SetActive(false);

                foreach (var a in referencedStats.skillAptitudes)
                {
                    //var entry = Util.KInstantiateUI(container, skillMod.gameObject);
                    //UIUtils.TryChangeText(entry.transform, "Description", a.Key.Name + ": "+a.Value);
                    //Debug.Log(a.Key.Name + "<->" + a.Value);
                    sb.Append("Skillaptitude in "); sb.Append(a.Key.Name); sb.Append(", base Skill: "); sb.AppendLine(a.Value.ToString());
                }
                foreach (Trait v in referencedStats.Traits)
                {
                    //var entry = Util.KInstantiateUI(container, container.gameObject);
                    //UIUtils.TryChangeText(entry.transform, "Description", v.Name);
                    sb.Append("Trait: "); sb.AppendLine(v.Name);
                }
                
                sb.Append("JoyTrait: ");sb.AppendLine(referencedStats.joyTrait.Name);
                sb.Append("StressTrait: ");sb.AppendLine(referencedStats.stressTrait.Name);


                UIUtils.TryChangeText(container.transform, "Description", sb.ToString());



                Debug.Log("Start PRefab");
                UIUtils.ListAllChildren(skillMod.transform);
                Debug.Log("Stop PRefab");
                //, parent.gameObject);
                skillMod.gameObject.SetActive(!hide);
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

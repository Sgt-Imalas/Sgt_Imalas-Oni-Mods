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
            public static int CustomStartingDupeCount(int dupeCount)
            {
                return StartDupeConfig.Instance.DuplicantStartAmount;
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
                    Debug.Log("ONY!!!!!");
#if DEBUG
                foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
#endif


                return code;
            }

            public static void Prefix(CharacterSelectionController __instance)
            {

                GridLayoutGroup[] objectsOfType2 = UnityEngine.Object.FindObjectsOfType<GridLayoutGroup>();
                foreach (var layout in objectsOfType2)
                {
                    if(layout.name== "CharacterContainers")
                    {
                        int countPerRow = StartDupeConfig.Instance.DuplicantStartAmount;
                        if (countPerRow > 5)
                        {
                            countPerRow = countPerRow / 2;
                        }
                        layout.constraintCount = countPerRow;

                        Debug.Log("cellsize: " + layout.cellSize);
                        //layout.cellSize = new(300, 400);
                    }
                }

                GameObject parentToScale = (GameObject)typeof(CharacterSelectionController).GetField("containerParent", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                CharacterContainer prefabToScale = (CharacterContainer)typeof(CharacterSelectionController).GetField("containerPrefab", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
                //Debug.Log("PARENT: "+parentToScale.transform.localScale);
                //prefabToScale.transform.localScale = new(0.8f,0.8f);
                if (StartDupeConfig.Instance.DuplicantStartAmount > 5)
                {
                    parentToScale.transform.parent.transform.localScale = new Vector3(0.6f, 0.6f);
                    prefabToScale.baseCharacterScale = prefabToScale.baseCharacterScale * 0.6f;

                }
#if DEBUG
               // Debug.Log("PREFAB: " + size);
# endif
            }

            public static void Postfix(CharacterSelectionController __instance)
            {
                if (!__instance.IsStarterMinion)
                    return;

                LocText[] objectsOfType1 = UnityEngine.Object.FindObjectsOfType<LocText>();
                if (objectsOfType1 != null)
                {
                    foreach (LocText locText in objectsOfType1)
                    {
                        if (locText.key == "STRINGS.UI.IMMIGRANTSCREEN.SELECTYOURCREW")
                        {
                            locText.key = StartDupeConfig.Instance.DuplicantStartAmount == 1? "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURLONECREWMAN" : "STRINGS.UI.MODDEDIMMIGRANTSCREEN.SELECTYOURCREW";
                            break;
                        }
                    }
                }
            }
        }

        [HarmonyPatch(typeof(CharacterContainer), "GenerateCharacter")]
        public static class AddChangeButtonToCharacterContainer
        {
            public static void Postfix(CharacterContainer __instance, MinionStartingStats ___stats)
            {
                var buttonPrefab = __instance.transform.Find("TitleBar/RenameButton").gameObject;
                var titlebar = __instance.transform.Find("TitleBar").gameObject;

                //UIUtils.ListAllChildren(titlebar.transform);
                var changebtn = Util.KInstantiateUI(buttonPrefab, titlebar);
                changebtn.rectTransform().SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 40f, changebtn.rectTransform().sizeDelta.x);
                changebtn.name = "ChangeDupeStatButton";
                changebtn.GetComponent<ToolTip>().toolTip = "Adjust dupe stats";

                var img = changebtn.transform.Find("Image").GetComponent<KImage>();
                img.sprite = Assets.GetSprite("icon_gear");
                var button = changebtn.GetComponent<KButton>();
                button.ClearOnClick();
                button.onClick += ()=> InstantiateDupeMod(__instance, ___stats);
            }

            private static void InstantiateDupeMod(CharacterContainer parent, MinionStartingStats referencedStats)
            {
                Debug.Log("TBA.");
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

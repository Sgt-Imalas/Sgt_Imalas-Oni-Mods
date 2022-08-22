using Database;
using HarmonyLib;
using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static SaveGameModLoader.ModAssets;

namespace SaveGameModLoader
{
    class Patches
    {


        [HarmonyPatch(typeof(SaveLoader), "Load")]
        [HarmonyPatch(new Type[] { typeof(IReader) })]

        public class CreateModSelectionScreen_Patch
        {
            internal class SaveFileRoot
            {
                public int WidthInCells;
                public int HeightInCells;
                public Dictionary<string, byte[]> streamed;
                public string clusterID;
                public List<ModInfo> requiredMods;
                public List<KMod.Label> active_mods;
                public SaveFileRoot() => this.streamed = new Dictionary<string, byte[]>();
            }
            public static SaveLoader instance;
            public static void Prefix(SaveLoader __instance)
            {
                instance = __instance;
            }
           
            public static void DebugMethod(SaveFileRoot saveFileRoot)
            {
                //foreach(var label in saveFileRoot.active_mods)
                //{
                //   Debug.Log("TRANSPILER PATCH; Modname 1 :" + label.id + " : " + label.title);
                //}
                var enabledMods = Global.Instance.modManager.mods.FindAll(mod => mod.IsActive() == true);//.Select(mod => mod.label).ToList();
                var enabledModLabels = Global.Instance.modManager.mods.FindAll(mod => mod.IsActive() == true).Select(mod => mod.label).ToList();
                
                Debug.Log("Enabled Mods count:" + enabledMods.Count);
                //foreach (var mod in enabledMods)
                //{
                //    Debug.Log("TRANSPILER PATCH; Enabled Mod:" + mod.label.id + " : "+ mod.label.title);
                //}



                var enabledButNotSavedMods = enabledModLabels.Except(saveFileRoot.active_mods).ToList(); 
                var savedButNotEnabledMods = saveFileRoot.active_mods.Except(enabledModLabels).ToList();

                enabledButNotSavedMods.Remove(enabledButNotSavedMods.Find(mod => mod.title == "SaveGameModLoader"));
                savedButNotEnabledMods.Remove(savedButNotEnabledMods.Find(mod => mod.title == "SaveGameModLoader"));

                var enabledIds = enabledButNotSavedMods.Select(label => label.id);
                var disabledIds = savedButNotEnabledMods.Select(label => label.id);

                
                
                foreach (var id in enabledIds)
                {

                    var modToDisable = Global.Instance.modManager.mods.Find(ListMod => ListMod.label.id == id);
                    {
                        if (modToDisable != null)
                        {
                            modToDisable.SetEnabledForActiveDlc(false);
                            Debug.Log("enabled but not stored in SaveGame, disabling: " + id + " : " + modToDisable.title);
                        }
                        else
                        {
                            Debug.LogWarning("Mod " + id + " : " + enabledButNotSavedMods.Find(m => m.id==id).title + " is not installed, how did this happen?");
                        }
                    }
                }
                foreach (var id in disabledIds)
                {

                    var modToEnable = Global.Instance.modManager.mods.Find(ListMod => ListMod.label.id == id);
                    if (modToEnable != null)
                    {
                        modToEnable.SetEnabledForActiveDlc(true);
                        Debug.Log("stored in SaveGame but not enabled, enabling: " + id + " : " + modToEnable.title);
                    }
                    else
                    {
                        Debug.LogWarning("Mod " + id + " : " + modToEnable.title + " is stored in this SaveGame, but not installed!");
                    }

                }


                if(enabledButNotSavedMods.Count>0 || savedButNotEnabledMods.Count > 0)
                {
                    Global.Instance.modManager.Save();
                    new System.Action(App.Quit).Invoke(); //App.Instance.Restart
                }

                //Util.KInstantiateUI(ScreenPrefabs.Instance.modsMenu.gameObject, 
                //    (UnityEngine.Object)FrontEndManager.Instance == (UnityEngine.Object)null 
                //    ? GameScreenManager.Instance.ssOverlayCanvas 
                //    : FrontEndManager.Instance.gameObject, true)
                //    .GetComponent<ConfirmDialogScreen>().PopupConfirmDialog("Change dem mods",
                //    new System.Action(App.instance.Restart),
                //    null);
            }

            public static readonly MethodInfo ScreenCreator = AccessTools.Method(
               typeof(CreateModSelectionScreen_Patch),
               nameof(DebugMethod)

            );

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
            {
                var code = instructions.ToList();
                var insertionIndex = code.FindIndex(ci => ci.opcode == OpCodes.Ldstr && (string)ci.operand == "Mod footprint of save file doesn't match current mod configuration");

                //foreach (var v in code) { Debug.Log(v.opcode + " -> " + v.operand); };
                if (insertionIndex != -1)
                {
                     code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Ldloc_1));
                     code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, ScreenCreator));
                }

                return code;
            }
        }
    }
}

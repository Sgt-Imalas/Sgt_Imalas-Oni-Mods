using BlueprintsV2.BlueprintsV2.Tools;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BlueprintsV2.BlueprintsV2.Patches
{
    internal class InputPatches
    {


        [HarmonyPatch(typeof(PlayerController), "OnPrefabInit")]
        public static class PlayerControllerOnPrefabInitPatch
        {
            public static void Postfix(PlayerController __instance)
            {
                //Добавляем инструменты (не конопки): Создать, Использовать, Снимок.
                var interfaceTools = new List<InterfaceTool>(__instance.tools);
                var createBlueprintTool = new GameObject(nameof(CreateBlueprintTool), typeof(CreateBlueprintTool));
                createBlueprintTool.transform.SetParent(__instance.gameObject.transform);
                createBlueprintTool.gameObject.SetActive(true);
                createBlueprintTool.gameObject.SetActive(false);

                interfaceTools.Add(createBlueprintTool.GetComponent<InterfaceTool>());

                var useBlueprintTool = new GameObject(typeof(UseBlueprintTool).Name, typeof(UseBlueprintTool));
                useBlueprintTool.transform.SetParent(__instance.gameObject.transform);
                useBlueprintTool.gameObject.SetActive(true);
                useBlueprintTool.gameObject.SetActive(false);

                interfaceTools.Add(useBlueprintTool.GetComponent<InterfaceTool>());

                var snapshotTool = new GameObject(typeof(SnapshotTool).Name, typeof(SnapshotTool));
                snapshotTool.transform.SetParent(__instance.gameObject.transform);
                snapshotTool.gameObject.SetActive(true);
                snapshotTool.gameObject.SetActive(false);

                interfaceTools.Add(snapshotTool.GetComponent<InterfaceTool>());

                __instance.tools = interfaceTools.ToArray();

                CreateBlueprintTool.Instance.OverlaySynced = Config.Instance.CreateBlueprintToolSync;
                SnapshotTool.Instance.OverlaySynced = Config.Instance.SnapshotToolSync;
            }
        }
    }
}

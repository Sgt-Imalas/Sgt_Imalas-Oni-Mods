using Database;
using HarmonyLib;
using Klei.AI;
using LightBridge.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static LightBridge.ModAssets;

namespace LightBridge
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.Utilities, LightBridgeConfig.ID);
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
        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(FallerComponents))]
        [HarmonyPatch(nameof(FallerComponents.OnSolidChanged))]
        public static class LightBridgeCatchesFallers
        {
            public static bool Prefix(HandleVector<int>.Handle handle)
            {
                FallerComponent fallerComponent = GameComps.Fallers.GetData(handle);
                if (fallerComponent.transform == null)
                {
                    return false;
                }

                Vector3 position = fallerComponent.transform.GetPosition();
                position.y = position.y - fallerComponent.offset - 0.1f;
                int num = Grid.PosToCell(position);
                if (!Grid.IsValidCell(num))
                {
                    return false;
                }
                bool flag = !Grid.Solid[num];
                if (Grid.FakeFloor[num])
                {
                    fallerComponent.isFalling = false;
                    FallerComponents.RemoveGravity(fallerComponent.transform);
                    Debug.Log("LightBridgeBelow");
                    return false;
                }
                if (flag != fallerComponent.isFalling)
                {
                    fallerComponent.isFalling = flag;
                    if (flag)
                    {
                        FallerComponents.AddGravity(fallerComponent.transform, Vector2.zero);
                    }
                    else
                    {
                        FallerComponents.RemoveGravity(fallerComponent.transform);
                    }
                }
                return false;
            }
        }
    }
}

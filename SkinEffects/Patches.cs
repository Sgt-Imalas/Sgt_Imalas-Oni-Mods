using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;
using static SkinEffects.ModAssets;

namespace SkinEffects
{
    internal class Patches
    {
        [HarmonyPatch(typeof(BuildingFacade), nameof(BuildingFacade.ChangeBuilding))]
        public class TriggerLampChange
        {
            public static void Postfix(BuildingFacade __instance)
            {
                if(__instance.gameObject.TryGetComponent<SkinLamp>(out var lampController))
                {
                    lampController.ToggleLamp();
                }
            }
        }


        [HarmonyPatch]
        public static class AddLampToTrimming
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<SkinLamp>();
                Light2D light2D = go.AddOrGet<Light2D>();
                light2D.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
                light2D.Color = LIGHT2D.CEILINGLIGHT_COLOR;
                light2D.Range = 4f;
                light2D.Angle = 2.6f;
                light2D.Direction = LIGHT2D.CEILINGLIGHT_DIRECTION;
                light2D.Offset = LIGHT2D.CEILINGLIGHT_OFFSET;
                light2D.shape = LightShape.Cone;
                light2D.drawOverlay = true;
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
                yield return typeof(CornerMouldingConfig).GetMethod(name);
                yield return typeof(CrownMouldingConfig).GetMethod(name);
            }
        }
    }
}

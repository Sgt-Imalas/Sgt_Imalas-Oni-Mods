using HarmonyLib;
using NaturalConstruction.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace NaturalConstruction.Patches
{
	internal class PlantablePlot_Patches
	{

        [HarmonyPatch(typeof(PlantablePlot), nameof(PlantablePlot.ConfigureOccupyingObject))]
        public class PlantablePlot_ConfigureOccupyingObject_Patch
        {
            ///Skips all registrations that make the plant domesticated
            public static bool Prefix(PlantablePlot __instance) => !__instance.TryGetComponent<MakePlantedPlantWild>(out _);
        }


        [HarmonyPatch(typeof(PlantablePlot), nameof(PlantablePlot.SpawnOccupyingObject))]
        public class PlantablePlot_SpawnOccupyingObject_Patch
        {
            public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
				var m_postToCell = AccessTools.Method(typeof(Grid), nameof(Grid.PosToCell), [typeof(KMonoBehaviour)]);

				foreach (var ci in orig)
				{
					yield return ci;
					if (ci.Calls(m_postToCell))
					{
                        yield return new(OpCodes.Ldarg_0);
						yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(PlantablePlot_SpawnOccupyingObject_Patch), nameof(ReplaceSpawnCell)));					}

				}
			}

            private static int ReplaceSpawnCell(int spawnCell, PlantablePlot instance)
            {
                if (!instance.TryGetComponent<MakePlantedPlantWild>(out _))
                    return spawnCell;

				Vector3 relativePosition = instance.GetOccupyingObjectRelativePosition();
				Vector3 targetPos;

				if (instance.rotatable != null)
					targetPos = (instance.gameObject.transform.GetPosition() + instance.rotatable.GetRotatedOffset(relativePosition));
				else
					targetPos = (instance.gameObject.transform.GetPosition() + relativePosition);
                var newCell = Grid.PosToCell(targetPos);
                SgtLogger.l("Moving natural plant from " + spawnCell + " to " + newCell);
				return newCell;
			}
        }

        [HarmonyPatch(typeof(PlantablePlot), nameof(PlantablePlot.SetPreview))]
        public class PlantablePlot_SetPreview_Patch
        {
            public static void Postfix(PlantablePlot __instance)
            {
                __instance.Trigger(ModAssets.PlantPreviewChanged);
			}
        }
	}
}

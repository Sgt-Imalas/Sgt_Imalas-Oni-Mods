using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	internal class BuildingDef_Patches
	{
		[HarmonyPatch]
		public static class BuildingDef_IsAreaClear_IsValidBuildLocation_Patch
		{
			[HarmonyPostfix]
			public static void Postfix(BuildingDef __instance, int cell, ref string fail_reason, ref bool __result)
			{
				if (__instance.BuildLocationRule != BuildLocationRule.BuildingAttachPoint)
					return;

				var slotTag = __instance.AttachmentSlotTag;
				var slotPos = __instance.attachablePosition;

				bool attachmentPosValid = false, upgradeProhibited = false;
				string upgradeFail = string.Empty;
				foreach (var attachable in RocketAttachableSocket.Components)
				{
					if (attachable.AcceptsAttachment(slotTag, Grid.OffsetCell(cell, slotPos)))
					{
						//SgtLogger.l("attachable accepts tag " + slotTag);

						if (!attachable.UpgradeProhibited(__instance.PrefabID, out upgradeFail))
							attachmentPosValid = true;
						else
							upgradeProhibited = true;
						break;
					}
				}

				if (attachmentPosValid)
				{
					fail_reason = upgradeFail;
					__result = !upgradeProhibited;
				}
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				yield return AccessTools.Method(typeof(BuildingDef), nameof(BuildingDef.IsAreaClear), [typeof(GameObject), typeof(int), typeof(Orientation), typeof(ObjectLayer), typeof(ObjectLayer), typeof(bool), typeof(bool), typeof(string).MakeByRefType(), typeof(bool)]);
				yield return AccessTools.Method(typeof(BuildingDef), nameof(BuildingDef.IsValidBuildLocation), [typeof(GameObject), typeof(int), typeof(Orientation), typeof(bool), typeof(string).MakeByRefType()]);
			}
		}
	}
}

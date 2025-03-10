using DupeModelAccessPermissions.Content.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static Grid;

namespace DupeModelAccessPermissions.Patches
{
	internal class Grid_Patches
	{
		[HarmonyPatch(typeof(Grid), nameof(Grid.HasPermission))]
		public class Grid_HasPermission_Patch
		{
			public static void Postfix(Grid __instance, int cell, int minionInstanceID, int fromCell, NavType fromNavType, ref bool __result)
			{
				if (!HasAccessDoor[cell])
					return;

				if (minionInstanceID == AccessControl_Extension.DefaultBionicsInstanceID)//prevent loop
					return;

				//dupe has specific permissions
				if (restrictions[cell].DirectionMasksForMinionInstanceID.TryGetValue(minionInstanceID, out _))
					return;

				if (!Mod.BionicMinionInstanceIds.Contains(minionInstanceID))
					return;
				//bionic minion without specific permissions; use default bionics permissions instead of regular ones
				__result = Grid.HasPermission(cell, AccessControl_Extension.DefaultBionicsInstanceID, fromCell, fromNavType);
			}
		}
	}
}

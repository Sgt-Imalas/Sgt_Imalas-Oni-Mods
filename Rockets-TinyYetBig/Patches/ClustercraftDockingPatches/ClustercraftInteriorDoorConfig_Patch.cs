using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches.ClustercraftDockingPatches
{
	internal class ClustercraftInteriorDoorConfig_Patch
	{
		/// <summary>
		/// add a docking tube attachment slot to the rocket interior door
		/// </summary>
		[HarmonyPatch(typeof(ClustercraftInteriorDoorConfig), nameof(ClustercraftInteriorDoorConfig.OnSpawn))]
		public static class AddDockingTubeAttachmentSlot
		{
			public static void Postfix(GameObject inst)
			{
				inst.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
				{
					new BuildingAttachPoint.HardPoint(new CellOffset(1, 0), ModAssets.Tags.AttachmentSlotDockingDoor, (AttachableBuilding) null)
				};
			}
		}
	}
}

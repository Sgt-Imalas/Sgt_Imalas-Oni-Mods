using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class SolidTransferArm_Patches
	{

		[HarmonyPatch(typeof(SolidTransferArm), nameof(SolidTransferArm.UpdateArmAnim))]
		public class SolidTransferArm_UpdateArmAnim_Patch
		{
			public static void Postfix(SolidTransferArm __instance)
			{
				var chore = __instance.choreDriver.GetCurrentChore();
				if (chore == null || chore.isNull || chore.isComplete)
					__instance.StopRotateSound();
			}
		}
	}
}

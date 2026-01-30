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
		/// <summary>
		/// Bugfix patch
		/// stops rotation sound after the rotating stops, this can bug out in vanilla
		/// </summary>
		[HarmonyPatch(typeof(SolidTransferArm), nameof(SolidTransferArm.UpdateArmAnim))]
		public class SolidTransferArm_UpdateArmAnim_Patch
		{
			public static void Postfix(SolidTransferArm __instance)
			{
				if (!__instance.rotateSoundPlaying)
					return;

				var chore = __instance.choreDriver.GetCurrentChore();

				if (chore == null || chore.isNull || chore.isComplete)
					__instance.StopRotateSound();
			}
		}
	}
}

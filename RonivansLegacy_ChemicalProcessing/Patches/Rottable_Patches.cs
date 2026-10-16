using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Text;
using static Rottable;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Rottable_Patches
	{

		[HarmonyPatch(typeof(Rottable), nameof(Rottable.AtmosphereQuality))]
		public class Rottable_AtmosphereQuality_Patch
		{
			public static bool Prefix(IRottable rottable, ref RotAtmosphereQuality __result)
			{
				if (rottable == null || rottable.gameObject.IsNullOrDestroyed())
					return true;

				if(FridgeAtmosphereController.IsSterilizedFreezerFood(rottable.gameObject))
				{
					__result = RotAtmosphereQuality.Sterilizing;
					return false;
				}
				return true;
			}
		}
	}
}

using AquaticMinnowMinion.Content.ModDb;
using AquaticMinnowMinion.Content.Scripts;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UtilLibs;
using static AquaticMinnowMinion.ModAssets;
using static GasBreatherFromWorldProvider;

namespace AquaticMinnowMinion.Patches
{
	internal class GasBreatherFromWorldProvider_Patches
	{

		[HarmonyPatch(typeof(GasBreatherFromWorldProvider), nameof(GasBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell), 
			[typeof(int), typeof(CellOffset[]), typeof(OxygenBreather), typeof(float)], [ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Out])]
		public class GasBreatherFromWorldProvider_GetBestBreathableCellAroundSpecificCell_Patch
		{
			public static bool Prefix(GasBreatherFromWorldProvider __instance, int theSpecificCell, CellOffset[] breathRange, OxygenBreather breather, ref float totalBreathableMassAroundCell, ref BreathableCellData __result)
			{
				if(breather != null && breather.PrefabID() == Tags.AquaticMinion)
				{
					__result = GasOrWaterBreatherFromWorldProvider.GetBestBreathableCellAroundSpecificCell(theSpecificCell, breathRange, breather, out totalBreathableMassAroundCell);
					return false;
				}
				return true;
			}
		}
	}
}

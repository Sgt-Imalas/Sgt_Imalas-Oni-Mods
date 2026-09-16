using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
	internal class Game_Patches
	{
		[HarmonyPatch(typeof(Game), nameof(Game.OnLoadLevel))]
		public class Game_OnLoadLevel_Patch
		{
			public static void Postfix()
			{
				HighPressureConduitRegistration.ClearEverything();
				LogisticConduit.ClearEverything();
				StructuralTileMarker.ClearEverything();
			}
		}
	}
}

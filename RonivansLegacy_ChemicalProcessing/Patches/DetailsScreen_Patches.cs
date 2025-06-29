using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class DetailsScreen_Patches
    {
		/// <summary>
		/// Inject custom sidescreen
		/// </summary>
		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class CustomSideScreenPatch_SatelliteCarrier
		{
			public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
			{
				UIUtils.AddClonedSideScreen<MiningGuidanceDeviceProgramSelectorSideScreen>("MiningGuidanceDeviceProgramSelectorSideScreen", "ArtableSelectionSideScreen", typeof(ArtableSelectionSideScreen));
			}
		}
	}
}

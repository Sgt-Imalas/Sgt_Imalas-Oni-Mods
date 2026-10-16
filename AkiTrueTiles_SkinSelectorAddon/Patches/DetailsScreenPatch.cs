using AkiTrueTiles_SkinSelectorAddon.UI;
using HarmonyLib;
using System.Collections.Generic;
using UtilLibs;

namespace AkiTrueTiles_SkinSelectorAddon.Patches
{
    class DetailsScreenPatch
    {
		/// <summary>
		/// Inject custom sidescreen
		/// </summary>
		[HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
		public static class CustomSideScreenPatch_SatelliteCarrier
		{
			public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
			{
				UIUtils.AddClonedSideScreen<TrueTilesSkinSelectorSideScreen>("TrueTilesSkinSelectorSideScreen", "ArtableSelectionSideScreen", typeof(ArtableSelectionSideScreen));
			}
		}
	}
}

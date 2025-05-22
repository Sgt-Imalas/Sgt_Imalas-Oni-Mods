using AkiTrueTiles_SkinSelectorAddon.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkiTrueTiles_SkinSelectorAddon.Patches
{
    class DetailsScreenPatch
    {
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

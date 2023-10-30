using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.UI_Unity
{
    internal class UI_Patches
    {
        [HarmonyPatch(typeof(DetailsScreen), "OnPrefabInit")]
        public static class CustomSideScreenPatch_DailyReset
        {
            public static void Postfix(List<DetailsScreen.SideScreenRef> ___sideScreens)
            {
                UIUtils.AddCustomSideScreen<DockingSidescreen>("RTB_DockingSidescreen", ModAssets.DockingSideScreenWindowPrefab);
                UIUtils.AddCustomSideScreen<SpaceConstructionSidescreen>("RTB_SpaceConstructionSidescreen", ModAssets.SpaceConstructionSideScreenWindowPrefab);
            }
        }

    }
}

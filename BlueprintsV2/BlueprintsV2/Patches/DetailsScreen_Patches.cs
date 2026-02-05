using BlueprintsV2.BlueprintsV2.UnityUI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.Patches
{
	internal class DetailsScreen_Patches
	{

        [HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.OnPrefabInit))]
        public class DetailsScreen_OnPrefabInit_Patch
        {
			public static void Postfix()
			{
				UIUtils.AddCustomSideScreen<TextNoteSideScreen>("BlueprintsV2_TextNoteSideScreen", Util.KInstantiateUI(ModAssets.NoteToolStateScreenGO));
			}
		}
	}
}

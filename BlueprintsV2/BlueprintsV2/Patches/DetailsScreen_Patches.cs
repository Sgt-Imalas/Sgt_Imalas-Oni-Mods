using BlueprintsV2.BlueprintsV2.UnityUI;
using HarmonyLib;
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

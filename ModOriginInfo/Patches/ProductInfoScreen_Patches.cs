using HarmonyLib;

namespace ModOriginInfo.Patches
{
	internal class ProductInfoScreen_Patches
	{

        [HarmonyPatch(typeof(ProductInfoScreen), nameof(ProductInfoScreen.SetDescription))]
        public class ProductInfoScreen_SetDescription_Patch
        {
            public static void Postfix(ProductInfoScreen __instance, BuildingDef def)
			{
				__instance.productFlavourText.text += ModAssets.GetModNameIfValid(def, 2);				
            }
        }
	}
}

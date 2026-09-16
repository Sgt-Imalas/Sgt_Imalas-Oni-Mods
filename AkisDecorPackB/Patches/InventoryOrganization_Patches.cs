using AkisDecorPackB.Content.ModDb;
using HarmonyLib;

namespace AkisDecorPackB.Patches
{
	internal class InventoryOrganization_Patches
	{

        [HarmonyPatch(typeof(InventoryOrganization), nameof(InventoryOrganization.GenerateSubcategories))]
        public class InventoryOrganization_GenerateSubcategories_Patch
		{
			public static void Postfix()
			{
				ModSkins.ConfigureSubCategories();
			}
		}
	}
}

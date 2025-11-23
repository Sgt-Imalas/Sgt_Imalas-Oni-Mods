using AkisDecorPackB.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KleiItems;

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

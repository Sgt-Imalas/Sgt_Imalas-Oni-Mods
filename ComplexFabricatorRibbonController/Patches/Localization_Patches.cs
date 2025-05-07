using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComplexFabricatorRibbonController.Patches
{
	public class Localization_Patches
	{
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				UtilLibs.LocalisationUtil.Translate(typeof(STRINGS), true); 
				Strings.Add("STRINGS.MISC.TAGS.CFRC_MICROCHIP_BUILDABLE", global::STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.POWER_STATION_TOOLS.NAME);
			}
		}
	}
}

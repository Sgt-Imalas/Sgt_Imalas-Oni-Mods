using HarmonyLib;
using Klei.AI;
using Rockets_TinyYetBig.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches.ElementPatches
{
	internal class LegacyModMain_Patch
	{
		[HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.ConfigElements))]
		public static class Add_NeutroniumAlloy_Effects
		{
			public static void Postfix()
			{
				var UnobtaniumAlloy = ModElements.UnobtaniumAlloy.Get();
				UnobtaniumAlloy.attributeModifiers.Add(new AttributeModifier("Decor", 1.0f, UnobtaniumAlloy.name, true));
				UnobtaniumAlloy.attributeModifiers.Add(new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, 2000f, UnobtaniumAlloy.name));
			}
		}
	}
}

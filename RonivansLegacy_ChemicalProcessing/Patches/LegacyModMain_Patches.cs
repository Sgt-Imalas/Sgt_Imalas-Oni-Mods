using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using static RonivansLegacy_ChemicalProcessing.Content.ModDb.ModElements;


namespace RonivansLegacy_ChemicalProcessing.Patches
{
    class LegacyModMain_Patches
    {

        [HarmonyPatch(typeof(LegacyModMain), nameof(LegacyModMain.ConfigElements))]
        public class LegacyModMain_ConfigElements_Patch
        {
            public static void Postfix(LegacyModMain __instance)
			{
				var attributeModifiers = Db.Get().BuildingAttributes;

				Element silver = ElementLoader.FindElementByHash(Silver_Solid.SimHash);
				Element brass = ElementLoader.FindElementByHash(Brass_Solid.SimHash);
				Element galena = ElementLoader.FindElementByHash(Galena_Solid.SimHash);
				Element carbonFiber = ElementLoader.FindElementByHash(CarbonFiber_Solid.SimHash);
				Element plasteel = ElementLoader.FindElementByHash(Plasteel_Solid.SimHash);

				//=: Giving Silver Decor and Temperature modifications :====================================================
				AttributeModifier silverDecorModifier = new AttributeModifier(attributeModifiers.Decor.Id, 0.4f, null, true, false, true);
				AttributeModifier silverTempModifier = new AttributeModifier(attributeModifiers.OverheatTemperature.Id, -30f, silver.name);
				silver.attributeModifiers.Add(silverDecorModifier);
				silver.attributeModifiers.Add(silverTempModifier);

				//=: Giving Brass Decor and Temperature modifications :=====================================================
				AttributeModifier BrassDecorModifier = new AttributeModifier(attributeModifiers.Decor.Id, 0.25f, null, true, false, true);
				AttributeModifier BrassTempModifier = new AttributeModifier(attributeModifiers.OverheatTemperature.Id, -10f, brass.name);
				brass.attributeModifiers.Add(BrassDecorModifier);
				brass.attributeModifiers.Add(BrassTempModifier);

				//=: Giving Galena Temperature modifications :==============================================================
				AttributeModifier GalenaDecorModifier = new AttributeModifier(attributeModifiers.Decor.Id, 0.1f, null, true, false, true);
				AttributeModifier GalenaTempModifier = new AttributeModifier(attributeModifiers.OverheatTemperature.Id, -30f, galena.name);
				galena.attributeModifiers.Add(GalenaDecorModifier);
				galena.attributeModifiers.Add(GalenaTempModifier);

				//=: Giving Carbon Fibre Temperature modifications :========================================================
				AttributeModifier carbonFibreTempModifier = new AttributeModifier(attributeModifiers.OverheatTemperature.Id, 5000f, carbonFiber.name);
				carbonFiber.attributeModifiers.Add(carbonFibreTempModifier);

				//=: Giving Plasteel Temperature modifications :============================================================
				AttributeModifier plasteelTempModifier = new AttributeModifier(attributeModifiers.OverheatTemperature.Id, 800f, plasteel.name);
				plasteel.attributeModifiers.Add(plasteelTempModifier);
			}
        }
    }
}

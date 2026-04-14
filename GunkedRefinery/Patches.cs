using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace GunkedRefinery
{
	internal class Patches
	{
		public static Tag RefinableOil = TagManager.Create("RefinableOil");
		[HarmonyPatch(typeof(ElementLoader), nameof(ElementLoader.Load))]
		public class ElementLoader_Load_Patch
		{
			public static void Postfix()
			{
				var gunk = ElementLoader.FindElementByHash(SimHashes.LiquidGunk);
				var oil = ElementLoader.FindElementByHash(SimHashes.CrudeOil);
				gunk.oreTags ??= [];
				gunk.oreTags = gunk.oreTags.AddToArray(RefinableOil);
				oil.oreTags ??= [];
				oil.oreTags = oil.oreTags.AddToArray(RefinableOil);				
			}
		}

		[HarmonyPatch(typeof(Localization), nameof(Localization.Initialize))]
		public class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				Strings.Add("STRINGS.MISC.TAGS.REFINABLEOIL", "Refinable Oil");
			}
		}


		[HarmonyPatch(typeof(OilRefineryConfig), nameof(OilRefineryConfig.ConfigureBuildingTemplate))]
		public class OilRefineryConfig_ConfigureBuildingTemplate_Patch
		{
			public static void Postfix(GameObject go)
			{
				ElementConverter elementConverter = go.GetComponent<ElementConverter>();
				elementConverter.inputIsCategory = true;
				var ele = elementConverter.consumedElements.First();
				ele.Tag = RefinableOil;
				elementConverter.consumedElements = [ele];

				ConduitConsumer conduitConsumer = go.GetComponent<ConduitConsumer>();
				conduitConsumer.capacityTag = RefinableOil;

				ConduitDispenser dispenser = go.GetComponent<ConduitDispenser>();
				dispenser.elementFilter = dispenser.elementFilter.AddToArray(SimHashes.LiquidGunk);

			}
		}
	}
}

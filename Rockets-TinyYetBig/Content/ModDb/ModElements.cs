using ElementUtilNamespace;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rockets_TinyYetBig.Elements
{
	public class ModElements
	{
		public static ElementInfo
			UnobtaniumDust = ElementInfo.Solid("UnobtaniumDust", Color.black),
			UnobtaniumAlloy = ElementInfo.Solid("UnobtaniumAlloy", Color.grey),
			SpaceStationForceField = ElementInfo.Solid("SpaceStationForceField", Color.blue);


		/// <summary>
		/// Add additionally needed mod tags to certain elements
		/// </summary>
		public static List<SimHashes> RadShieldingElements = [
				SimHashes.Lead,
				SimHashes.Gold,
				SimHashes.DepletedUranium,
				SimHashes.Tungsten,
				SimHashes.TempConductorSolid
			];

		public static void RegisterSubstances(List<Substance> list)
		{
			var shineFX = list.Find(e => e.elementID == SimHashes.Steel).material;
			var glassShineFx = list.Find(e => e.elementID == SimHashes.Glass).material;
			var newElements = new HashSet<Substance>()
			{
				UnobtaniumDust.CreateSubstance(),
				SpaceStationForceField.CreateSubstance(true, glassShineFx ),
				UnobtaniumAlloy.CreateSubstance(true, shineFX)
			};
			list.AddRange(newElements);
			//SgtLogger.debuglog("2," + list + ", " + list.Count);
		}

		internal static void RegisterAdditionalSubstanceTags()
		{
			foreach (var element in ElementLoader.elements)
			{
				if (element.HasTag(GameTags.CombustibleLiquid))
				{
					element.oreTags = element.oreTags.Append(ModAssets.Tags.RocketFuelTag);
				}
			}
			AddTagToElement(SimHashes.LiquidHydrogen, ModAssets.Tags.RocketFuelTag);

			foreach (var radShieldElemnt in RadShieldingElements)
			{
				AddTagToElement(radShieldElemnt, ModAssets.Tags.RadiationShieldingRocketConstructionMaterial);
			}

			AddTagsToElement(SimHashes.Chlorine, [ModAssets.Tags.CorrosiveOxidizer, ModAssets.Tags.OxidizerEfficiency_3]);
			AddTagsToElement(SimHashes.LiquidOxygen, [ModAssets.Tags.LOXTankOxidizer, ModAssets.Tags.OxidizerEfficiency_4]);

			//not entirely required, since those entries come included in the vanilla game, but if there is a custom solid oxidizer, it should be added like this:
			AddTagsToElement(SimHashes.Fertilizer, [ModAssets.Tags.RocketSolidOxidizerTag, ModAssets.Tags.OxidizerEfficiency_1]);
			AddTagsToElement(SimHashes.OxyRock, [ModAssets.Tags.RocketSolidOxidizerTag, ModAssets.Tags.OxidizerEfficiency_2]);
		}
		static void AddTagToElement(SimHashes element, Tag tag) => AddTagsToElement(element, [tag]);
		static void AddTagsToElement(SimHashes element, Tag[] tags)
		{
			var elem = ElementLoader.GetElement(element.CreateTag());
			if (elem == null)
			{
				Debug.LogError($"Element {element} not found in ElementLoader.");
				return;
			}

			if (elem.oreTags is null)
			{
				elem.oreTags = [];
			}
			foreach (var tag in tags)
			{
				if (!elem.oreTags.Contains(tag))
				{
					elem.oreTags = elem.oreTags.Append(tag);
				}
			}
		}

		internal static void RegisterElementEffects()
		{
			var UnobtaniumAlloy = ModElements.UnobtaniumAlloy.Get();
			UnobtaniumAlloy.attributeModifiers.Add(new AttributeModifier("Decor", 1.0f, UnobtaniumAlloy.name, true));
			UnobtaniumAlloy.attributeModifiers.Add(new AttributeModifier(Db.Get().BuildingAttributes.OverheatTemperature.Id, 2000f, UnobtaniumAlloy.name));
		}

		internal static void ModifyExistingElements(List<ElementLoader.ElementEntry> result)
		{
			foreach (var elem in result)
			{
				if (elem.elementId == SimHashes.Unobtanium.ToString())
				{
					elem.radiationAbsorptionFactor = 1.26f;//not .25 to avoid rounding issues
					break;
				}
			}
		}
	}
}

using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.BUILDINGS.PREFABS;

namespace UtilLibs.SharedTweaks
{
	/// <summary>
	/// Improve elementconverter description strings
	/// </summary>
	public sealed class ElementConverterDescriptionImprovement : PForwardedComponent
	{
		public static void Register()
		{
			new ElementConverterDescriptionImprovement().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{

				var targetMethod = AccessTools.Method(typeof(ElementConverter), nameof(ElementConverter.GetDescriptors));
				var replaceElementConverterPrefix = AccessTools.Method(typeof(ElementConverterDescriptionImprovement), nameof(ReplaceElementConverterDescriptorsPrefix));
				plibInstance.Patch(targetMethod, prefix: new(replaceElementConverterPrefix));
				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}
		public static bool ReplaceElementConverterDescriptorsPrefix(ElementConverter __instance, GameObject go, ref List<Descriptor> __result)
		{
			int visibleConverterCount = 0;
			var converters = go.GetComponents<ElementConverter>();
			foreach (var converter in converters)
			{
				if (converter.showDescriptors && converter.consumedElements != null)
					++visibleConverterCount;
			}

			//only go into effect when there are multiple elementconverters on the GO
			if (!__instance.showDescriptors || visibleConverterCount < 2 || __instance.consumedElements == null || !__instance.consumedElements.Any())
			{
				return true;
			}
			string consumptionString = string.Empty;
			string productionString = string.Empty;
			string tooltip = string.Empty;

			string elementFormatted = "{0} ({1})";

			if (__instance.consumedElements != null)
			{
				foreach (var consumedElement in __instance.consumedElements)
				{
					if (!consumptionString.IsNullOrWhiteSpace())
						consumptionString += ", ";
					if (!tooltip.IsNullOrWhiteSpace())
						tooltip += "\n";

					consumptionString += string.Format(elementFormatted, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.MassConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"));
					tooltip += string.Format(STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.MassConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"));

				}
				tooltip += "\n\n";
			}
			if (__instance.outputElements != null)
			{
				foreach (var outputElement in __instance.outputElements)
				{
					if (!productionString.IsNullOrWhiteSpace())
						productionString += ", ";

					if (outputElement.IsActive)
					{
						LocString productionEntryTooltip = STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_INPUTTEMP;
						if (outputElement.useEntityTemperature)
						{
							productionEntryTooltip = STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP;
						}
						else if (outputElement.minOutputTemperature > 0f)
						{
							productionEntryTooltip = STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINTEMP;
						}
						productionString += string.Format(elementFormatted, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature));
						tooltip += string.Format(productionEntryTooltip.Replace("\n\n", "\n"), outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature));

						tooltip += "\n";
					}
				}
			}

			//turn x to y
			string text = string.Format(CRAFTINGTABLE.RECIPE_DESCRIPTION, consumptionString, productionString);
			Descriptor item = default(Descriptor);
			item.SetupDescriptor(text, tooltip, Descriptor.DescriptorType.Effect);
			__result = [item];

			return false;
		}
	}
}

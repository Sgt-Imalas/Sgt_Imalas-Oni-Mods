using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static BlueprintsV2.STRINGS.BLUEPRINTS_BLUEPRINTNOTE;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements
{
	public class TemperatureController : ISingleSliderControl
	{
		ElementNote target;
		public TemperatureController(ElementNote target) { this.target = target; }
		public string SliderTitleKey => "STRINGS.BLUEPRINTS_BLUEPRINTNOTE.TEMPERATURECONFIG.TITLE";
		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.TEMPERATURE.CELSIUS;
		public int SliderDecimalPlaces(int index) => 1;

		public float GetSliderMin(int index)
		{
			var element = ElementLoader.GetElement(target.ElementId.CreateTag());
			return UtilMethods.GetCFromKelvin(element.lowTemp);
		}

		public float GetSliderMax(int index)
		{
			var element = ElementLoader.GetElement(target.ElementId.CreateTag());
			return UtilMethods.GetCFromKelvin(element.highTemp);
		}
		public void SetSliderValue(float percent, int index)
		{
			if (target.IsVacuum)
				return;
			target.ElementTemperature = UtilMethods.GetKelvinFromC(percent);
			target.OnChange();
		}
		public float GetSliderValue(int index)
		{
			return UtilMethods.GetCFromKelvin(target.ElementTemperature);
		}

		public string GetSliderTooltip(int index) => string.Format(TEMPERATURECONFIG.TOOLTIP, UtilMethods.GetCFromKelvin(target.ElementTemperature));

		public string GetSliderTooltipKey(int index) => "";
	}
}

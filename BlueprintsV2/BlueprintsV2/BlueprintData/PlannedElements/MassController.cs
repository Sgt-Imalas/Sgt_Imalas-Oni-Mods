using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BlueprintsV2.STRINGS.BLUEPRINTS_PLANNED_ELEMENT_PLACER;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlannedElements
{
	public class MassController : ISingleSliderControl
	{
		ElementPlanInfo target;
		public MassController(ElementPlanInfo target) { this.target = target; }
		public string SliderTitleKey => "STRINGS.BLUEPRINTS_PLANNED_ELEMENT_PLACER.MASSCONFIG.TITLE";
		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;
		public int SliderDecimalPlaces(int index) => 2;

		public float GetSliderMin(int index)
		{
			return 0.001f;
		}

		public float GetSliderMax(int index)
		{
			var element = ElementLoader.GetElement(target.ElementId.CreateTag());
			return element.maxMass * 2;
		}
		public void SetSliderValue(float percent, int index)
		{
			target.ElementAmount = percent;
			target.OnChange();
		}
		public float GetSliderValue(int index)
		{
			return target.ElementAmount;
		}

		public string GetSliderTooltip(int index) => string.Format(MASSCONFIG.TOOLTIP, target.ElementAmount);

		public string GetSliderTooltipKey(int index) => "";
	}
}

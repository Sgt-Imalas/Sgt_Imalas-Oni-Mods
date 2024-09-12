using KSerialization;

namespace Rockets_TinyYetBig.Behaviours
{
	internal class ManualDeliverablePercentageThreshold : KMonoBehaviour, ISingleSliderControl
	{
		[MyCmpGet]
		private ManualDeliveryKG delivery;

		[Serialize]
		public float refillThreshold = 100f;


		public override void OnSpawn()
		{
			base.OnSpawn();
			SetSliderValue(refillThreshold, 0);
		}
		public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.MANUALDELIVERYGENERATORSIDESCREEN.TITLE";

		public string SliderUnits => global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM;

		public float GetSliderMax(int index) => delivery.storage.capacityKg;

		public float GetSliderMin(int index) => 0.0f;

		public string GetSliderTooltip() => string.Format(STRINGS.BUILDINGS.PREFABS.RTB_GENERATORCOALMODULE.SIDESCREEN_TOOLTIP, delivery.RequestedItemTag.ProperName(), refillThreshold + global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM);
		public string GetSliderTooltip(int i) => string.Format(STRINGS.BUILDINGS.PREFABS.RTB_GENERATORCOALMODULE.SIDESCREEN_TOOLTIP, delivery.RequestedItemTag.ProperName(), refillThreshold + global::STRINGS.UI.UNITSUFFIXES.MASS.KILOGRAM);

		public string GetSliderTooltipKey(int index) => STRINGS.BUILDINGS.PREFABS.RTB_GENERATORCOALMODULE.SIDESCREEN_TOOLTIP;

		public float GetSliderValue(int index) => refillThreshold;

		public void SetSliderValue(float percent, int index)
		{
			refillThreshold = percent;
			delivery.refillMass = percent;
		}

		public int SliderDecimalPlaces(int index) => 0;
	}
}

using KSerialization;
using UtilLibs;

namespace Rockets_TinyYetBig.Buildings.Utility
{
	public class DrillConeModeHandler : KMonoBehaviour, ICheckboxControl
	{
		[MyCmpGet] public Storage DiamondStorage;
		[Serialize] bool IsAutoLoader = false;
		[Serialize] float ManualDeliveryOriginalCapacity;


		public bool LoadingAllowed => IsAutoLoader;

		public override void OnSpawn()
		{
			base.OnSpawn();
			if (gameObject.TryGetComponent<ManualDeliveryKG>(out var deliveryKG))
			{
				ManualDeliveryOriginalCapacity = deliveryKG.capacity;
			}
			ToggleBetweenAutoAndManual();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}

		void ToggleBetweenAutoAndManual()
		{
			if (gameObject.TryGetComponent<ManualDeliveryKG>(out var deliveryKG))
			{
				if (IsAutoLoader)
				{
					deliveryKG.AbortDelivery("Mode Toggled on drill module.");
					//deliveryKG.ClearRequests();
					ManualDeliveryOriginalCapacity = deliveryKG.capacity;
					deliveryKG.capacity = 0;
				}
				else
				{

					deliveryKG.capacity = ManualDeliveryOriginalCapacity;
				}

			}
			SgtLogger.debuglog("DeliveryEnabled? : " + !IsAutoLoader + ", Capacity of Manual: " + deliveryKG.capacity + ", Original:" + ManualDeliveryOriginalCapacity);
		}

		public void OnSidescreenButtonPressed()
		{
			IsAutoLoader = !IsAutoLoader;
			ToggleBetweenAutoAndManual();
		}


		public string CheckboxTitleKey => "";

		public string CheckboxLabel => STRINGS.UI.DRILLCONE_MODEHANDLER_SIDESCREEN.LABEL;

		public string CheckboxTooltip => STRINGS.UI.DRILLCONE_MODEHANDLER_SIDESCREEN.TOOLTIP;


		public bool GetCheckboxValue() => IsAutoLoader;

		public void SetCheckboxValue(bool value)
		{
			IsAutoLoader = value;
			ToggleBetweenAutoAndManual();

		}
	}
}

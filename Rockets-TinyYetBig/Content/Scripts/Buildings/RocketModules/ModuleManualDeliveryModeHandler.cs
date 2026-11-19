using KSerialization;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Buildings.Utility
{
	/// <summary>
	/// allows a manual delivery on a rocket module to be switched to use cargo port loading instead
	/// </summary>
	public class ModuleManualDeliveryModeHandler : KMonoBehaviour, ICheckboxControl
	{
		[SerializeField] public Storage TargetStorage;
		[Serialize] bool IsAutoLoader = false;
		private float ManualDeliveryOriginalCapacity;
		[MyCmpGet] public ManualDeliveryKG ManualDelivery;


		public bool LoadingAllowed => IsAutoLoader;

		public bool AllowedToStore(KPrefabID item)
		{
			if(TargetStorage.storageFilters == null ||TargetStorage.storageFilters.Count == 0) 
				return true;
			return item.HasAnyTags(TargetStorage.storageFilters);
		}

		public override void OnSpawn()
		{
			if (TargetStorage == null)
				TargetStorage = GetComponent<Storage>();

			base.OnSpawn();
			if (ManualDelivery != null)
			{
				ManualDeliveryOriginalCapacity = ManualDelivery.capacity;
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

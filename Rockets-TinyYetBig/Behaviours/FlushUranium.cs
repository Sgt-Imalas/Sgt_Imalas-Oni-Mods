using System;

namespace Rockets_TinyYetBig.Behaviours
{
	class FlushUranium : KMonoBehaviour, ISidescreenButtonControl
	{
		[MyCmpGet]
		private Storage storage;
		public string SidescreenButtonText => STRINGS.UI_MOD.FLUSHURANIUM.BUTTON;

		public string SidescreenButtonTooltip => STRINGS.UI_MOD.FLUSHURANIUM.BUTTONINFO;

		public int ButtonSideScreenSortOrder()
		{
			return 20;
		}

		public void OnSidescreenButtonPressed()
		{
			storage.DropAll();
		}

		public bool SidescreenButtonInteractable()
		=> storage.RemainingCapacity() < 1f;

		public bool SidescreenEnabled()
		{
			return true;
		}
		public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();

		public int HorizontalGroupID()
		{
			return -1;
		}
	}
}

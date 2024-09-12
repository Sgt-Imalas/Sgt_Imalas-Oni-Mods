using System;

namespace OniRetroEdition.Behaviors
{
	internal class JobBoardSkillbutton : KMonoBehaviour, ISidescreenButtonControl
	{
		public string SidescreenButtonText => global::STRINGS.UI.UISIDESCREENS.TELEPADSIDESCREEN.SKILLS_BUTTON;

		public string SidescreenButtonTooltip => null;

		public int ButtonSideScreenSortOrder() => 20;

		public int HorizontalGroupID() => -1;

		public void OnSidescreenButtonPressed() => ManagementMenu.Instance.ToggleSkills();

		public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();

		public bool SidescreenButtonInteractable() => true;

		public bool SidescreenEnabled() => true;
	}
}

using KSerialization;

namespace SetStartDupes
{
	internal class PrintingPodCheckboxToggle : KMonoBehaviour, ICheckboxControl
	{
		[Serialize]
		public bool IsChecked { get { return PrintOnlyCarePackages; } set { PrintOnlyCarePackages = value; } }


		public static bool PrintOnlyCarePackages = false;

		public string CheckboxTitleKey => "STRINGS.UI.FRONTEND.CUSTOMGAMESETTINGSSCREEN.SETTINGS.CAREPACKAGES.NAME";

		public string CheckboxLabel => Strings.Get("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.NAME");

		public string CheckboxTooltip => Strings.Get("STRINGS.UI.DSS_OPTIONS.CAREPACKAGESONLY.TOOLTIP");

		public bool GetCheckboxValue() => IsChecked;

		public void SetCheckboxValue(bool value) => IsChecked = value;
	}
}

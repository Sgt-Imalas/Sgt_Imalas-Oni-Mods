using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings
{
	internal interface IDisableableCheckboxControl
	{
		string CheckboxTitleKey { get; }
		string CheckboxLabel { get; }
		string CheckboxTooltip { get; }
		bool GetCheckboxValue();
		void SetCheckboxValue(bool value);
		bool GetIsCheckboxInteractable();

	}
}

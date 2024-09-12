using SetStartDupes.DuplicityEditing.Helpers;
using System.Collections.Generic;
using UnityEngine;
using UtilLibs;
using UtilLibs.UI.FUI;

namespace SetStartDupes.DuplicityEditing.ScreenComponents
{
	internal class MinionSelectButton : MonoBehaviour
	{

		//public CrewPortrait helper; 
		MinionPortraitHelper helper;
		LocText DupeName;
		FToggleButton toggle;
		ToolTip toolTip;

		public void Init(MinionPortraitHelper _helper, System.Action onClickAction)
		{
			helper = _helper;

			DupeName = transform.Find("Label").GetComponent<LocText>();
			toggle = this.gameObject.AddComponent<FToggleButton>();
			toggle.OnClick += onClickAction;
			toolTip = UIUtils.AddSimpleTooltipToObject(this.gameObject, string.Empty);
		}
		public void UpdatePortrait(List<KeyValuePair<string, string>> accessories)
		{
			helper.ApplyMinionAccessories(accessories);
		}

		internal void UpdateName(MinionIdentity identity, StoredMinionIdentity identityStored)
		{
			if (identity != null)
			{
				this.name = identity.name;
				DupeName.SetText(identity.name);
			}
			else if (identityStored != null)
			{

				this.name = identityStored.storedName;
				DupeName.SetText(identityStored.storedName);
			}
		}

		public void SetActiveState(bool active) =>
			toggle?.ChangeSelection(active);
		internal void UpdateState(bool storedMinion, bool highlighted)
		{
			toggle.SetInteractable(!storedMinion);
			toggle.ChangeSelection(highlighted);
			toolTip.SetSimpleTooltip(storedMinion ? "stored minion" : "");
		}
	}
}

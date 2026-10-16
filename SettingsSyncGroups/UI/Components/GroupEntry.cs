using System;
using UtilLibs.UIcmp;
using UtilLibs;

namespace SettingsSyncGroups.UI.Components
{
	internal class GroupEntry:KMonoBehaviour
	{
		public string GroupName;

		public Action<string> ApplyName;
		public LocText Label, NumLabel;
		FButton SelectButton;


		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			UpdateUI(0);

		}
		
		private bool init = false;

		private void InitUi()
		{
			if (init)
				return;
			init = true;
			Label = transform.Find("Label")?.gameObject.GetComponent<LocText>();
			NumLabel = transform.Find("NumLabel")?.gameObject.GetComponent<LocText>();
			SelectButton = gameObject.AddOrGet<FButton>();
			SelectButton.OnClick += CallApplyName;
		}
		void CallApplyName()
		{
			ApplyName?.Invoke(GroupName);
		}
		public void UpdateUI(int newCount)
		{
			if (!init)
			{
				InitUi();
			}
			//SgtLogger.l("Group Name: " + GroupName);
			
			Label.SetText(GroupName);
			NumLabel.SetText(UIUtils.ColorText("["+newCount+"]",UIUtils.rgb(192, 230, 255)));
		}
	}
}

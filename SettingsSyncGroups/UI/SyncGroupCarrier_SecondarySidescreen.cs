using SettingsSyncGroups.UI.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UIcmp;
using UtilLibs;
using SettingsSyncGroups.Scripts;

namespace SettingsSyncGroups.UI
{
	public class SyncGroupCarrier_SecondarySidescreen : KScreen
	{
		SyncGroupCarrier Target;
		public Action<string> OnConfirm;

		GameObject groupsContainer;
		GroupEntry groupEntryPrefab;
		Dictionary<string, GroupEntry> GroupEntries = new(); //group entries
		Dictionary<string, int> CurrentTagEntries = new();


		GroupEntry EmptyUIEntry;
		FInputField2 FilterInputBar;
		FButton ConfirmBtn;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Init();
		}
		public override void OnKeyDown(KButtonEvent e)
		{
			if (e.TryConsume(Action.ToggleEnabled)) //enter key
			{
				ConfirmClicked();
			}
			base.OnKeyDown(e);
		}

		void Init()
		{
			SgtLogger.l("SyncGroupCarrier_SecondarySidescreen OnPrefabInit");
			var groupEntryPrefabGO = transform.Find("ScrollArea/Content/GroupEntryPrefab").gameObject;
			groupsContainer = groupEntryPrefabGO.transform.parent.gameObject;
			groupEntryPrefabGO.SetActive(false);
			groupEntryPrefab = groupEntryPrefabGO.AddOrGet<GroupEntry>();
			;

			FilterInputBar = transform.Find("SearchBar/Input").FindOrAddComponent<FInputField2>();
			FilterInputBar.Text = string.Empty;
			ConfirmBtn = transform.Find("SearchBar/ConfirmButton").FindOrAddComponent<FButton>();
			ConfirmBtn.OnClick += ConfirmClicked;
		}
		void ConfirmClicked()
		{
			OnConfirm?.Invoke(FilterInputBar.Text);
		}
		void SetEmptyRow()
		{
			if(EmptyUIEntry == null)
			{
				EmptyUIEntry = AddOrGetGroupEntry(string.Empty);
				EmptyUIEntry.NumLabel.SetText(string.Empty);
				EmptyUIEntry.Label.SetText(STRINGS.UI.GROUPASSIGNMENT_SECONDARYSIDESCREEN.NO_GROUP_ASSIGNED);
			}
		}
		internal void SetOpenedFrom(SyncGroupCarrier targetComponent)
		{
			Target = targetComponent; 
			SetEmptyRow();
			CurrentTagEntries = Target.GetCurrentlyExistingGroups();
			Refresh();
		}
		void Refresh()
		{
			if (!Target)
				return;

			Target.TryGetGroupName(out var groupName);
			FilterInputBar.SetTextFromData(groupName);

			foreach (var entry in GroupEntries)
			{
				entry.Value.gameObject.SetActive(false);
			}
			foreach (var existingGroup in CurrentTagEntries)
			{
				var entry = AddOrGetGroupEntry(existingGroup.Key);
				entry.UpdateUI(existingGroup.Value);
			}
		}
		void InputGroupFromExisting(string existing)
		{
			OnConfirm?.Invoke(existing);
		}

		GroupEntry AddOrGetGroupEntry(string groupName)
		{
			if(GroupEntries.TryGetValue(groupName, out var entry))
			{
				entry.gameObject.SetActive(true);
				return entry;
			}


			var newEntry = Util.KInstantiateUI<GroupEntry>(groupEntryPrefab.gameObject, groupsContainer);
			newEntry.name = groupName;
			newEntry.GroupName = groupName;
			newEntry.ApplyName = InputGroupFromExisting;
			newEntry.gameObject.SetActive(true);
			if (!groupName.IsNullOrWhiteSpace())
			{
				GroupEntries[groupName] = newEntry;
			}

			return newEntry;
		}
	}
}

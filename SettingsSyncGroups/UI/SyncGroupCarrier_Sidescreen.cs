using SettingsSyncGroups.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static SettingsSyncGroups.STRINGS.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SettingsSyncGroups.UI
{
	internal class SyncGroupCarrier_Sidescreen : SideScreenContent
	{
		SyncGroupCarrier TargetComponent;

		LocText Label;
		FButton Button;

		public static SyncGroupCarrier_SecondarySidescreen SecondarySideScreen;
		public override string GetTitle() => STRINGS.UI.BUILDINGSETTINGSGROUP_SIDESCREEN.TITLE;
		
		public override bool IsValidForTarget(GameObject target)
		{
			return target.TryGetComponent<SyncGroupCarrier>(out var carrier) && carrier.IsValid;
		}
		public override int GetSideScreenSortOrder() => -900;

		public override void ClearTarget()
		{
			SyncGroupCarrier.SynchronizeAll(TargetComponent);			
			TargetComponent = null;

			base.ClearTarget();
			ClearSecondarySideScreen();
		}
		public override void SetTarget(GameObject target)
		{
			base.SetTarget(target);
			if(target.TryGetComponent<SyncGroupCarrier>(out var carrier))
				this.TargetComponent = carrier;
			Refresh();
		}
		void EnableSecondarySideScreen(bool enable)
		{
			if (SecondarySideScreen == null || enable)
			{
				SecondarySideScreen = (SyncGroupCarrier_SecondarySidescreen)DetailsScreen.Instance.SetSecondarySideScreen(ModAssets.GroupSelectionSidescreen, GROUPASSIGNMENT_SECONDARYSIDESCREEN.TITLE);
				SecondarySideScreen.OnConfirm = OnNewGroupName;
				SecondarySideScreen.SetOpenedFrom(TargetComponent);
			}
			else
				ClearSecondarySideScreen();
		}

		void OnNewGroupName(string newName)
		{
			ClearSecondarySideScreen();
			TargetComponent.SetAndSyncFromGroup(newName, true);
			Refresh();
		}

		void ClearSecondarySideScreen()
		{
			DetailsScreen.Instance.ClearSecondarySideScreen();
		}

		bool init = false;
		void Init()
		{
			if (init)
				return;
			init = true;
			Label = transform.Find("Content/Label").gameObject.GetComponent<LocText>();
			Label.SetText(STRINGS.UI.GROUPASSIGNMENT_SECONDARYSIDESCREEN.NO_GROUP_ASSIGNED);
			Button = transform.Find("Content/Button").gameObject.AddOrGet<FButton>();
			Button.OnClick += () =>
			{
				EnableSecondarySideScreen(true);
				SetEditButtonInteractable(false);
			};
		}
		public void Refresh()
		{
			Label?.SetText(TargetComponent.GetDescriptionText());
			SetEditButtonInteractable(true);
		}

		public void SetEditButtonInteractable(bool interactable) => Button?.SetInteractable(interactable);

		public override void OnShow(bool show)
		{
			base.OnShow(show);
			Init();

			if (!show)
			{
				ClearSecondarySideScreen();
			}
			else
			{
				Refresh();
			}
		}
	}
}

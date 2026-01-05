using KSerialization;
using PipPlantNotify.Content.Defs.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PipPlantNotify.Content.Scripts
{
	internal class PipNotificator : KMonoBehaviour, ICheckboxControl
	{
		[MyCmpGet] Notifier notifier;

		[Serialize]
		bool TriggerNotification = false;

		public string CheckboxTitleKey => "STRINGS.UI.PIPPLANTNOTIFICATION.TITLE";

		public string CheckboxLabel => STRINGS.UI.PIPPLANTNOTIFICATION.LABEL;

		public string CheckboxTooltip => STRINGS.UI.PIPPLANTNOTIFICATION.TOOLTIP;

		public bool GetCheckboxValue() => TriggerNotification;

		public void SetCheckboxValue(bool value) => TriggerNotification = value;

		int handle = -1;
		public override void OnSpawn()
		{
			base.OnSpawn();
			handle = Subscribe(ModAssets.PipHasPlantedSeed, OnSeedPlanted);
		}
		public override void OnCleanUp()
		{
			Unsubscribe(handle);
			base.OnCleanUp();
		}

		void OnSeedPlanted(object data)
		{
			if (!TriggerNotification)
				return;

			int cell = Boxed<int>.Unbox(data);

			string name = this.GetProperName();
			string title = string.Format(STRINGS.UI.PIPPLANTNOTIFICATION.NOTIFICATION.TITLE, name);
			string tooltip = string.Format(STRINGS.UI.PIPPLANTNOTIFICATION.NOTIFICATION.TOOLTIP, name);

			Notification notification = new Notification(
				title, 
				NotificationType.Event,
				(_,_)=> tooltip,
				custom_click_callback: _=> LookAtCellReallyCloseAndSpawnArrows(cell), 
				show_dismiss_button:true
				);

			notifier.Add(notification);
		}
		void LookAtCellReallyCloseAndSpawnArrows(int cell)
		{
			GameUtil.FocusCamera(cell);
			CameraController.Instance.targetOrthographicSize = 3;
			var worldPos = Grid.CellToPos(cell);
			worldPos.x += Grid.CellSizeInMeters / 2f;
			worldPos.y += Grid.CellSizeInMeters / 2f;
			GameUtil.KInstantiate(Assets.GetPrefab(MarkerConfig.ID), worldPos, Grid.SceneLayer.FXFront, gameLayer: LayerMask.NameToLayer("Place")).SetActive(true);
		}
	}
}

using FMOD;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace SetStartDupes.CarePackageEditor.UI
{
	public class CarePackageOutlineEntry : KMonoBehaviour
	{
		public CarePackageOutline TargetOutline;

		Image DisplayImage;
		LocText Label;
		GameObject WarningIcon;
		ToolTip WarningToolTip;
		GameObject ConditionIndicator;
		ToolTip ConditionToolTip;


		FButton DeleteButton;
		FButton SelectButton;


		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			UpdateUI();
		}

		private bool init = false;
		private void InitUi()
		{
			if (init)
				return;
			init = true;

			DisplayImage = transform.Find("DisplayImage")?.GetComponent<Image>();
			Label = transform.Find("Label")?.GetComponent<LocText>();
			WarningIcon = transform.Find("Warning")?.gameObject;

			if (WarningIcon)
				WarningToolTip = UIUtils.AddSimpleTooltipToObject(WarningIcon.gameObject, "");

			ConditionIndicator = transform.Find("HasCondition")?.gameObject;
			if (ConditionIndicator)
				ConditionToolTip = UIUtils.AddSimpleTooltipToObject(ConditionIndicator.gameObject, "");

			DeleteButton = transform.Find("DeleteButton")?.gameObject?.AddOrGet<FButton>();
			SelectButton = gameObject.AddOrGet<FButton>();
			SelectButton.OnClick += SelectOutline;
			SgtLogger.l("Ui initialized!");
		}

		void SelectOutline()
		{
			if (TargetOutline == null)
				return;
			CarePackageOutlineManager.TrySelectOutline(TargetOutline);
		}

		public void UpdateOutline(CarePackageOutline newOutline)
		{
			TargetOutline = newOutline;
			UpdateUI();
		}
		public void UpdateUI()
		{
			if (TargetOutline == null)
			{
				SgtLogger.l("aborting ui update, target was null");
				return;
			}
			if (!init)
			{
				InitUi();
			}
			Label?.SetText(TargetOutline.GetDescriptionString());

			var TargetItem = Assets.GetPrefab(TargetOutline.ItemId);
			if (TargetItem != null)
			{
				SgtLogger.l(TargetItem.GetProperName());
				var image = Def.GetUISprite(TargetItem);
				if (image != null)
				{
					DisplayImage.sprite = image.first;
					DisplayImage.color = image.second;
				}
				WarningToolTip.SetSimpleTooltip("");
			}
			else
			{
				DisplayImage.sprite = Assets.GetSprite("unknown");
				DisplayImage.color = Color.white;
				WarningToolTip.SetSimpleTooltip(STRINGS.UI.CAREPACKAGEEDITOR.UNKNOWN_ITEM_TOOLTIP);
			}


			ConditionToolTip.SetSimpleTooltip(TargetOutline.GetConditionsTooltip());
			WarningIcon?.SetActive(TargetItem == null);
			DeleteButton?.ClearOnClick();

			DeleteButton.OnClick += () => CarePackageOutlineManager.TryDeleteOutline(TargetOutline);
			SelectButton.ClearOnClick();
			SelectButton.OnClick += () => CarePackageOutlineManager.TrySelectOutline(TargetOutline);
		}
	}
}

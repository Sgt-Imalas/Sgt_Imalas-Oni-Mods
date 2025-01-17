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


		bool init = false;
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			if (init)
			{
				return;
			}
			init = true;
			DisplayImage = transform.Find("DisplayImage")?.GetComponent<Image>();
			Label = transform.Find("Label")?.GetComponent<LocText>();
			WarningIcon = transform.Find("Warning")?.gameObject;

			if (WarningToolTip)
				WarningToolTip = UIUtils.AddSimpleTooltipToObject(WarningToolTip.gameObject, "");

			ConditionIndicator = transform.Find("HasCondition")?.gameObject;
			if (ConditionIndicator)
				ConditionToolTip = UIUtils.AddSimpleTooltipToObject(ConditionIndicator.gameObject, "");

			DeleteButton = transform.Find("DeleteButton")?.gameObject?.AddOrGet<FButton>();
			SelectButton = gameObject.AddOrGet<FButton>();

		}

		public void UpdateOutline(CarePackageOutline newOutline)
		{
			TargetOutline = newOutline;
		}
		public void UpdateUI()
		{
			if (TargetOutline == null)
				return;

			Label?.SetText(TargetOutline.GetDescriptionString());

			var TargetItem = Assets.GetPrefab(TargetOutline.ItemId);
			if (TargetItem != null)
			{
				var image = Def.GetUISprite(TargetItem);
				if (image != null)
				{
					DisplayImage.sprite = image.first;
					DisplayImage.color = image.second;
				}
				bool hasConditions = TargetOutline.HasConditions();
				ConditionIndicator.SetActive(hasConditions);
				if (hasConditions)
				{
					ConditionToolTip.SetSimpleTooltip(TargetOutline.GetConditionsTooltip());
				}
			}
			WarningIcon?.SetActive(TargetItem == null);
			DeleteButton.ClearOnClick();
			DeleteButton.OnClick += ()=> CarePackageOutlineManager.TryDeleteOutline(TargetOutline);

			SelectButton.ClearOnClick();
			SelectButton.OnClick += () => CarePackageOutlineManager.TrySelectOutline(TargetOutline);
		}
	}
}

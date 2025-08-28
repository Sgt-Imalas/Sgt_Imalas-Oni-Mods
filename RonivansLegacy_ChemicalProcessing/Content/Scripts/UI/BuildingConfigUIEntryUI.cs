using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.UIcmp;
using UtilLibs;
using UnityEngine.UI;
using RonivansLegacy_ChemicalProcessing.Content.ModDb.BuildingConfigurations;
using static STRINGS.BUILDINGS.PREFABS.ROCKCRUSHER.FACADES;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.UI
{
	class BuildingConfigUIEntryUI : KMonoBehaviour
	{
		public BuildingConfigurationEntry TargetOutline;

		Image DisplayImage;
		LocText Label;
		FToggle EnabledCheckbox;
		ToolTip ToolTip;
		FButton SelectButton;
		GameObject Gear;

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
			DisplayImage = transform.Find("DisplayImageContainer/DisplayImage")?.GetComponent<Image>();
			Label = transform.Find("Label")?.GetComponent<LocText>();			
			ToolTip = UIUtils.AddSimpleTooltipToObject(gameObject, "");
			Gear = transform.Find("Gear")?.gameObject;
			UIUtils.AddSimpleTooltipToObject(Gear, STRINGS.UI.BUILDINGEDITOR.BUILDINGCONFIGURABLE);

			EnabledCheckbox = transform.Find("Checkbox").gameObject.AddOrGet<FToggle>();
			EnabledCheckbox.SetCheckmark("Checkmark");

			SelectButton = gameObject.AddOrGet<FButton>();
			SelectButton.OnClick += SelectOutline;
			EnabledCheckbox.OnClick += (on) =>
			{
				TargetOutline.SetBuildingEnabled(on);
				if (TargetOutline != null)
				{
					BuildingEditor_MainScreen.Instance?.SetOutlineEnabled(TargetOutline, on);
				}
			};
		}

		void SelectOutline()
		{
			if (TargetOutline == null)
				return;
			BuildingEditor_MainScreen.Instance?.SelectOutline(TargetOutline);
		}

		public void UpdateOutline(BuildingConfigurationEntry newOutline)
		{
			TargetOutline = newOutline;
			UpdateUI();
		}
		public void UpdateUI()
		{
			if (TargetOutline == null)
			{
				//SgtLogger.l("aborting ui update, target was null");
				return;
			}
			if (!init)
			{
				InitUi();
			}
			Label?.SetText(TargetOutline.GetDisplayName());

			Gear.SetActive(TargetOutline.HasConfigurables());
			EnabledCheckbox.SetOnFromCode(TargetOutline.IsBuildingEnabled());
			EnabledCheckbox.SetInteractable(TargetOutline.IsInjected);
			DisplayImage.sprite = Def.GetUISprite(TargetOutline.BuildingID).first;
			ToolTip.SetSimpleTooltip(TargetOutline.IsInjected ? "" : STRINGS.UI.BUILDINGEDITOR.PARENT_MOD_DISABLED);
			SelectButton.SetInteractable(TargetOutline.IsInjected);			
			SelectButton.ClearOnClick();
			SelectButton.OnClick += () => BuildingEditor_MainScreen.Instance?.SelectOutline(TargetOutline); 
		}
	}
}

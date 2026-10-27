using UnityEngine;
using UtilLibs.UIcmp;
using UtilLibs;
using UnityEngine.UI;
using Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations;

namespace Rockets_TinyYetBig.Content.Scripts.UI.UIComponents
{
	class BuildingConfigUIEntryUI : KMonoBehaviour
	{
		public BuildingConfigurationEntry TargetOutline;

		Image DisplayImage;
		LocText Label;
		FToggle EnabledCheckbox;
		Image CheckboxBG;
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
			UIUtils.AddSimpleTooltipToObject(Gear, STRINGS.UI.RE_BUILDINGEDITOR.BUILDINGCONFIGURABLE);

			EnabledCheckbox = transform.Find("Checkbox").gameObject.AddOrGet<FToggle>();
			CheckboxBG = EnabledCheckbox.gameObject.GetComponent<Image>();
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
			EnabledCheckbox.SetOnFromCode(TargetOutline.ShowBuildingEnabled(out string reason));
			bool catDisabled = TargetOutline.DisabledByCategory();
			EnabledCheckbox.SetInteractable(TargetOutline.IsInjected && !catDisabled);

			if (catDisabled)
				CheckboxBG.color = UIUtils.rgb(204, 204, 204);
			else
				CheckboxBG.color = Color.white;

			DisplayImage.sprite = Def.GetUISprite(TargetOutline.BuildingID).first;
			ToolTip.SetSimpleTooltip(reason);
			SelectButton.SetInteractable(TargetOutline.IsInjected);			
			SelectButton.ClearOnClick();
			SelectButton.OnClick += () => BuildingEditor_MainScreen.Instance?.SelectOutline(TargetOutline); 
		}
	}
}

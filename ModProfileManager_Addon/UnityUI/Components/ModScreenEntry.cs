using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;

namespace ModProfileManager_Addon.UnityUI.Components
{
	internal class ModScreenEntry : KMonoBehaviour
	{
		FButton ChangeSortOrderBt;
		FToggle ModEnabled;
		public KMod.Mod TargetMod;
		public KMod.Label? MissingLabel = null;
		LocText ModName;

		GameObject PlibConfigHighlight;
		ToolTip plibTooltip;

		public string Name = string.Empty;


		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			ModName = transform.Find("Label").gameObject.GetComponent<LocText>();
			ModEnabled = transform.Find("Background").gameObject.AddComponent<FToggle>();
			ModEnabled.SetCheckmark("Checkmark");
			PlibConfigHighlight = transform.Find("HasPLibData").gameObject;
			plibTooltip = UIUtils.AddSimpleTooltipToObject(PlibConfigHighlight, "");
			ChangeSortOrderBt = transform.Find("ReorderButton").gameObject.AddComponent<FButton>();

			var TypeGO = transform.Find("ModType").gameObject;
			if (TargetMod != null)
			{
				Name = TargetMod.label.title;
				this.gameObject.name = Name;

				ChangeSortOrderBt.OnClick += () =>
				{
					ModAssets.ShowModIndexShiftDialogue(TargetMod, ChangeSortOrderBt.gameObject);
				};

				bool devMod = TargetMod.label.distribution_platform == KMod.Label.DistributionPlatform.Dev;

				var bt =
					TypeGO.AddOrGet<FButton>();
				bt.OnClick += TargetMod.on_managed;
				Color buttonColor = ModAssets.Colors.Red;
				if (TargetMod.IsDev)
					buttonColor = ModAssets.Colors.Yellow;
				else if (TargetMod.IsLocal)
					buttonColor = ModAssets.Colors.Blue;
				bt.normalColor = buttonColor;
				bt.hoverColor = UIUtils.Lighten(buttonColor, 20);

				ModName?.SetText(Name);
				var label = transform.Find("ModType/Label").gameObject.GetComponent<LocText>();
				if (TargetMod.IsLocal)
				{
					TypeGO.gameObject.GetComponent<Image>().color = buttonColor;
					label.SetText(devMod ? STRINGS.UI.MOD_FILTER_DROPDOWN.DEV : STRINGS.UI.MOD_FILTER_DROPDOWN.LOCAL);
				}
				else
				{
					label.SetText(global::STRINGS.UI.PLATFORMS.STEAM);
				}
				ModEnabled.OnClick += (active) =>
				{
					ModAssets.ToggleModActive(TargetMod.label, active);
				};
			}
			else if (MissingLabel != null)
			{
				var m_missingLabel = MissingLabel.Value;
				bool isMissingSteam = ulong.TryParse(m_missingLabel.id, out ulong steamId);
				ChangeSortOrderBt.SetInteractable(false);

				Name = m_missingLabel.title;
				this.gameObject.name = Name;

				Color buttonColor = ModAssets.Colors.DarkRed;
				if (m_missingLabel.distribution_platform == KMod.Label.DistributionPlatform.Dev)
					buttonColor = ModAssets.Colors.DarkYellow;
				else if (m_missingLabel.distribution_platform == KMod.Label.DistributionPlatform.Local)
					buttonColor = ModAssets.Colors.DarkBlue;


				ModName?.SetText(Name);
				TypeGO.GetComponent<Image>().color = buttonColor;

				var label = transform.Find("ModType/Label").gameObject.GetComponent<LocText>();
				label.SetText(STRINGS.UI.MOD_FILTER_DROPDOWN.MISSING);
				UIUtils.AddSimpleTooltipToObject(label.gameObject, isMissingSteam ? STRINGS.UI.STEAM_MISSING_TOOLTIP : STRINGS.UI.LOCAL_MISSING_TOOLTIP);

				if (isMissingSteam)
				{
					var bt =
					TypeGO.AddOrGet<FButton>();
					bt.OnClick += () => ModAssets.SubToMissingMod(steamId);
					bt.normalColor = ModAssets.Colors.DarkRed;
					bt.hoverColor = UIUtils.Lighten(ModAssets.Colors.Red, 10);
				}

				ModEnabled.SetInteractable(false);
			}
		}

		public void Refresh(bool enabled, bool hasPlibConfig, string plibData)
		{
			PlibConfigHighlight.SetActive(hasPlibConfig);
			if (hasPlibConfig)
			{
				plibTooltip.SetSimpleTooltip(STRINGS.UI.PLIB_CONFIG_FOUND + "\n" + plibData);
			}
			ModEnabled.SetOnFromCode(enabled);
			ChangeSortOrderBt.SetInteractable(enabled);

		}
		public override void OnSpawn()
		{
			base.OnSpawn();
		}
	}
}

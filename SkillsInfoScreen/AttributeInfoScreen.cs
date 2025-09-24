using Database;
using Epic.OnlineServices.Lobby;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using UtilLibs.UIcmp;
using Attribute = Klei.AI.Attribute;

namespace SkillsInfoScreen
{
	internal class AttributeInfoScreen : TableScreen
	{
		static Dictionary<string, float> MaxSkillLevels;
		bool hookedUp = false;

		bool RainbowGradient = false;

		bool IncludeTempEffects = true;

		Color Bad, Medium, Good;

		//Gradient ColorGradient;

		KButton OptionsButton = null;
		GameObject OptionsPanel = null;
		FToggle EffectToggle;

		public override void OnActivate()
		{
			int colorBlindnessMode = KPlayerPrefs.GetInt(GraphicsOptionsScreen.ColorModeKey);
			RainbowGradient = colorBlindnessMode == 0;
			//ColorGradient = new Gradient();

			///grabbing colorblindness colors
			Good = (Color)GlobalAssets.Instance.colorSet.cropGrown;
			Good.a = 1; //a is 0 for these by default, but that doesnt allow tinting the symbols here
			Good = UIUtils.Darken(Good, 10);

			Medium = (Color)GlobalAssets.Instance.colorSet.cropGrowing;
			Medium.a = 1;
			Medium = UIUtils.Darken(Medium, 10);

			Bad = (Color)GlobalAssets.Instance.colorSet.cropHalted;
			Bad.a = 1;
			Bad = UIUtils.Darken(Bad, 10);


			this.has_default_duplicant_row = false;
			this.title = (string)global::STRINGS.UI.CHARACTERCONTAINER_SKILLS_TITLE;
			HookupReferences();
			base.OnActivate();
			this.AddPortraitColumn("Portrait", on_load_portrait, null);
			this.AddButtonLabelColumn("Names",
				on_load_name_label,
				get_value_name_label,
				(widget_go => GetWidgetRow(widget_go).SelectMinion()),
				(widget_go => GetWidgetRow(widget_go).SelectAndFocusMinion()),
				compare_rows_alphabetical,
				on_tooltip_name,
				on_tooltip_sort_alphabetically);

			var attributeDb = Db.Get().Attributes;
			MaxSkillLevels = [];

			var stats = DUPLICANTSTATS.ALL_ATTRIBUTES.OrderBy(id => STRINGS.UI.StripLinkFormatting(attributeDb.TryGet(id)?.Name ?? "unknown"));

			foreach (var attributeId in stats)
			{
				if (attributeId == "SpaceNavigation" && !DlcManager.IsExpansion1Active())
					continue;
				MaxSkillLevels[attributeId] = 20;

				var attribute = attributeDb.TryGet(attributeId);
				SgtLogger.l("Adding column for " + attribute);

				this.AddLabelColumn(attributeId,
					(a, b) => on_load_attribute(a, b, attribute),
					(a, b) => get_value_attribute_label(a, b, attribute),
					(a, b) => this.compare_rows_attribute(a, b, attribute),
					(a, b, c) => this.on_tooltip_attribute(a, b, c, attribute),
					(a, b, c) => this.on_tooltip_sort_attribute(a, b, c, attribute),
					50,
					true);
			}

			int size = 700;
			var rect = this.rectTransform();
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			GetComponentInChildren<KScrollRect>().GetComponentInParent<LayoutElement>().preferredHeight = size;
		}

		public override void OnCmpDisable()
		{
			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
			base.OnCmpDisable();
			this.OptionsPanel.SetActive(false);
		}

		public void GetPrefabRefs(VitalsTableScreen source)
		{
			if (source.prefab_row_empty)
				this.prefab_row_empty = Util.KInstantiateUI(source.prefab_row_empty);
			if (source.prefab_row_header)
				this.prefab_row_header = Util.KInstantiateUI(source.prefab_row_header);
			if (source.prefab_scroller_border)
				this.prefab_scroller_border = Util.KInstantiateUI(source.prefab_scroller_border);
			if (source.prefab_world_divider)
				this.prefab_world_divider = Util.KInstantiateUI(source.prefab_world_divider);
		}

		void HookupReferences()
		{
			if (hookedUp)
				return;
			hookedUp = true;
			//UIUtils.ListAllChildrenPath(this.transform);
			this.title_bar = transform.Find("Title/Label").gameObject.GetComponent<LocText>();
			this.CloseButton = transform.Find("Title/CloseButton").gameObject.GetComponent<KButton>();
			this.header_content_transform = transform.Find("HeaderContent");
			this.scroll_content_transform = transform.Find("Content/Scroll View/Viewport/Content/ScrollContent");
			this._canvas = GetComponent<Canvas>();
			var vlg = scroll_content_transform.gameObject.GetComponent<VerticalLayoutGroup>();
			vlg.padding.left = 8;
			vlg.padding.right = 8;
		}

		protected void on_tooltip_name(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
		{
			tooltip.ClearMultiStringTooltip();
			switch (this.GetWidgetRow(widget_go).rowType)
			{
				case TableRow.RowType.Minion:
					if (minion == null)
						break;
					tooltip.AddMultiStringTooltip(string.Format(STRINGS.UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, (object)minion.GetProperName()), (TextStyleSetting)null);
					break;
			}
		}

		private void on_load_attribute(IAssignableIdentity minion, GameObject widget_go, Attribute attribute)
		{
			TableRow widgetRow = this.GetWidgetRow(widget_go);
			LocText LocText = widget_go.GetComponentInChildren<LocText>(true);
			var HLG = widgetRow.GetComponent<HorizontalLayoutGroup>();
			HLG.padding = new(1, 1, 0, 0);
			LocText.transform.parent.gameObject.GetComponent<LayoutElement>().minWidth = 128;

			if (minion != null)
			{
				var horizontal = widgetRow.GetComponent<HorizontalLayoutGroup>();

				LocText.fontSize = 32f;
				LocText.alignment = TMPro.TextAlignmentOptions.Center;
				LocText.text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
				LocText.enableWordWrapping = false;
			}
			else
			{
				LocText.text = widgetRow.isDefault ? "" : attribute.Name;
				LocText.enableWordWrapping = false;
				OptionsButton.transform.SetAsLastSibling();
			}
		}

		const string formattedDisplay = "{0} ({1})";
		private string get_value_attribute_label(IAssignableIdentity identity, GameObject widget_go, Attribute attribute)
		{
			int levelTotal = GetAttributeLevel(identity, attribute);
			int levelWithoutMods = GetAttributeLevel(identity, attribute, false);
			string attributeId = attribute.Id;

			string totalLevel = levelTotal.ToString();
			string baseLevel = levelWithoutMods.ToString();

			if (IncludeTempEffects)
				totalLevel = ColorActiveText(levelTotal.ToString(), levelTotal, attributeId);
			else
				baseLevel = ColorActiveText(baseLevel.ToString(), levelWithoutMods, attributeId);

			return string.Format(formattedDisplay,totalLevel,baseLevel);
		}

		string ColorActiveText(string text, int value, string attributeId) => UIUtils.EmboldenText(UIUtils.ColorText(value.ToString(), GetIntensityColor(value, attributeId)));

		Color GetIntensityColor(float level, string attributeId)
		{
			if (RainbowGradient)
			{
				level = Mathf.Clamp(level, 0, 45);

				//return ColorGradient.Evaluate(level / 45f);

				float hsvShift = (level * 6f) / 360f;
				return UIUtils.Darken(UIUtils.HSVShift(Color.red, hsvShift * 100), 25);
			}
			float maxLevel = MaxSkillLevels[attributeId];
			float midPoint = maxLevel / 2;

			///This is a way less resource intensive lerp than color.lerp
			float levelMinusMidPoint = level - midPoint;

			float r_bad = Bad.r * Bad.r;
			float g_bad = Bad.g * Bad.g;
			float b_bad = Bad.b * Bad.b;

			float r_mid = Medium.r * Medium.r;
			float g_mid = Medium.g * Medium.g;
			float b_mid = Medium.b * Medium.b;

			float r_gud = Good.r * Good.r;
			float g_gud = Good.g * Good.g;
			float b_gud = Good.b * Good.b;

			float r_final, g_final, b_final;

			if (level >= midPoint)
			{
				float lerp = (levelMinusMidPoint / midPoint);
				float lerpInv = 1 - lerp;

				r_final = Mathf.Sqrt(r_mid * lerpInv + r_gud * lerp);
				g_final = Mathf.Sqrt(g_mid * lerpInv + g_gud * lerp);
				b_final = Mathf.Sqrt(b_mid * lerpInv + b_gud * lerp);
			}
			else
			{
				float lerp = (level / midPoint);
				float lerpInv = 1 - lerp;

				r_final = Mathf.Sqrt(r_bad * lerpInv + r_mid * lerp);
				g_final = Mathf.Sqrt(g_bad * lerpInv + g_mid * lerp);
				b_final = Mathf.Sqrt(b_bad * lerpInv + b_mid * lerp);
			}
			return new Color(r_final, g_final, b_final);
		}

		private int compare_rows_attribute(IAssignableIdentity a, IAssignableIdentity b, Attribute attribute)
		{
			int aVal = GetAttributeLevel(a, attribute, IncludeTempEffects);
			int bVal = GetAttributeLevel(b, attribute, IncludeTempEffects);

			if (aVal == bVal)
				return 0;
			if (aVal < bVal)
				return 1;
			return -1;
		}

		protected void on_tooltip_attribute(
		  IAssignableIdentity minion,
		  GameObject widget_go,
		  ToolTip tooltip, Attribute attribute)
		{
			tooltip.ClearMultiStringTooltip();
			int lvl = GetAttributeLevel(minion, attribute);

			//if (lvl < 0)
			//	return;
			//string tooltipString = STRINGS.UI.JOBSSCREEN.MINION_SKILL_TOOLTIP.Replace("{Name}", minion.GetProperName()).Replace("{Attribute}", attribute.Name) + lvl;
			tooltip.AddMultiStringTooltip(GetAttributeTooltip(minion, attribute), null);
		}

		protected void on_tooltip_sort_attribute(
		  IAssignableIdentity minion,
		  GameObject widget_go,
		  ToolTip tooltip,
		  Attribute attribute)
		{
			tooltip.ClearMultiStringTooltip();
			string sortByTooltip = global::STRINGS.UI.MINION_IDENTITY_SORT.TITLE;
			sortByTooltip += ": ";
			sortByTooltip += attribute.Name;

			switch (this.GetWidgetRow(widget_go).rowType)
			{
				case TableRow.RowType.Header:
					tooltip.AddMultiStringTooltip(sortByTooltip, null);
					break;
			}
		}

		string GetAttributeTooltip(IAssignableIdentity identity, Attribute attribute)
		{
			string tooltip = string.Empty;
			if (identity is MinionIdentity minion)
			{
				if (minion.TryGetComponent<Modifiers>(out var modifiers))
				{
					var instance = modifiers.attributes.Get(attribute.Id);

					tooltip = instance.GetAttributeValueTooltip();
				}
			}
			else if (identity is StoredMinionIdentity storedMinion)
			{
				tooltip = storedMinion.minionModifiers.attributes.Get(attribute.Id).GetAttributeValueTooltip();
			}
			return tooltip;
		}

		int GetAttributeLevel(IAssignableIdentity identity, Attribute attribute, bool includeTmpEffects = true)
		{
			int level = -1;
			if (identity is MinionIdentity minion && minion.TryGetComponent<Modifiers>(out var modifiers))
			{
				AttributeInstance instance = modifiers.attributes.Get(attribute.Id);
				level = (int)GetTotalDisplayValue(instance, includeTmpEffects);

			}
			else if (identity is StoredMinionIdentity storedMinion)
			{
				AttributeInstance instance = storedMinion.minionModifiers.attributes.Get(attribute.Id);
				level = (int)GetTotalDisplayValue(instance, includeTmpEffects);

			}

			if (level > MaxSkillLevels[attribute.Id])
				MaxSkillLevels[attribute.Id] = level;
			return level;
		}

		static Dictionary<AttributeModifier, bool> IsTraitModifierCache = [];


		static bool IsTraitModifier(AttributeModifier modifier)
		{
			if (IsTraitModifierCache.TryGetValue(modifier, out var result))
				return result;


			string description = modifier.GetDescription();
			if (description == Strings.Get("STRINGS.DUPLICANTS.MODIFIERS.SKILLLEVEL.NAME"))
				return true;

			foreach (var trait in Db.Get().traits.resources)
			{
				if (trait.GetName().Contains(description))
				{
					IsTraitModifierCache[modifier] = true;
					return true;
				}
			}
			IsTraitModifierCache[modifier] = false;
			return false;
		}
		static float GetTotalDisplayValue(AttributeInstance instance, bool includeNonTraitEffects)
		{
			float value = instance.GetBaseValue();
			float multiplier = 0f;
			for (int i = 0; i != instance.Modifiers.Count; i++)
			{
				AttributeModifier attributeModifier = instance.Modifiers[i];
				SgtLogger.l(attributeModifier.Description+" "+attributeModifier.Value);
				if (!includeNonTraitEffects && !IsTraitModifier(attributeModifier))
				{
					SgtLogger.l(attributeModifier.GetDescription() + " is not a trait modifier?");
					continue;
				}

				if (!attributeModifier.IsMultiplier)
				{
					value += attributeModifier.Value;
				}
				else
				{
					multiplier += attributeModifier.Value;
				}
			}

			if (multiplier != 0f)
			{
				value += Mathf.Abs(value) * multiplier;
			}

			return value;
		}

		internal void FetchOptionsPanel(JobsTableScreen jobsScreen)
		{
			var settingsButtonOriginal = jobsScreen.transform.Find("prefab_row_header/OptionsButton").gameObject;

			OptionsButton = Util.KInstantiateUI<KButton>(settingsButtonOriginal.gameObject, transform.Find("HeaderContent").gameObject, true);
			OptionsButton.ClearOnClick();
			OptionsButton.onClick += OnSettingsButtonClicked;

			OptionsPanel = Util.KInstantiateUI(jobsScreen.transform.Find("OptionsPanel").gameObject, this.gameObject, true);
			//SgtLogger.l("AAAAAAAAAAAAAAAAA");
			//UIUtils.ListAllChildrenPath(OptionsPanel.transform);
			OptionsPanel.transform.Find("Reset").gameObject.SetActive(false);


			UIUtils.TryChangeText(OptionsPanel.transform, "Advanced Mode/Text", MOD_STRINGS.SKILLINFO_TEMP_EFFECT_TOGGLE);
			UIUtils.AddSimpleTooltipToObject(OptionsPanel, MOD_STRINGS.SKILLINFO_TEMP_EFFECT_TOGGLE_TOOLTIP);
			EffectToggle = OptionsPanel.transform.Find("Advanced Mode/Checkbox").gameObject.AddOrGet<FToggle>();
			EffectToggle.SetCheckmark("Checkbox");
			EffectToggle.SetOnFromCode(IncludeTempEffects);
			EffectToggle.OnChange += SetEffectImpactEnabled;
			EffectToggle.GetComponent<ToolTip>().enabled = false;

			OptionsPanel.SetActive(false);
		}

		void SetEffectImpactEnabled(bool enabled)
		{
			IncludeTempEffects = enabled;
			this.RefreshRows();
		}

		private void OnSettingsButtonClicked()
		{
			this.OptionsPanel.gameObject.SetActive(true);
			this.OptionsPanel.GetComponent<Selectable>().Select();
		}
	}
}

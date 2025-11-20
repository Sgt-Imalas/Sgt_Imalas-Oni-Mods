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
using static KSerialization.DebugLog;

namespace SkillsInfoScreen
{
	internal class SkillsOverviewInfoScreen : TableScreen
	{
		const string skillScrollerId = "skillsScroller";
		bool hookedUp = false;

		bool RainbowGradient = false;

		Color Bad, Medium, Good;

		//Gradient ColorGradient;

		public override void OnActivate()
		{
			int colorBlindnessMode = KPlayerPrefs.GetInt(GraphicsOptionsScreen.ColorModeKey);
			RainbowGradient = colorBlindnessMode == 0;
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
			this.title = Patches.SkillsOverviewName;

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

			AddLabelColumn("QOLExpectations",
				(this.on_load_qualityoflife_expectations),
				(this.get_value_qualityoflife_label),
				(this.compare_rows_qualityoflife_expectations),
				(this.on_tooltip_qualityoflife_expectations),
				(this.on_tooltip_sort_qualityoflife_expectations),
				96,
				true);


			var skills = Db.Get().Skills.resources;
			SkillGroups groupsDb = Db.Get().SkillGroups;
			skills.RemoveAll(skill => skill.deprecated);
			skills = skills.OrderBy(skill => STRINGS.UI.StripLinkFormatting(groupsDb.TryGet(skill.skillGroup).Name)).ToList();

			string lastGroup = null;


			List<TableColumn> allItemColumns = [];
			List<DividerColumn> dividerColumnList = [];
			List<TableColumn> currentSubgroupColumns = [];

			this.StartScrollableContent(skillScrollerId);

			foreach (var skill in skills)
			{
				if (!Game.IsCorrectDlcActiveForCurrentSave(skill))
				{
					continue;
				}
				if (lastGroup != null && lastGroup != skill.skillGroup)
				{
					string dividerID = "SkillgroupDivider_" + skill.skillGroup;
					TableColumn[] skillgroup_group_columns = currentSubgroupColumns.ToArray();
					DividerColumn new_column = new DividerColumn((Func<bool>)(() =>
					{
						if (skillgroup_group_columns == null || skillgroup_group_columns.Length == 0)
							return true;
						foreach (TableColumn tableColumn in skillgroup_group_columns)
						{
							if (tableColumn.isRevealed)
								return true;
						}
						return false;
					}), skillScrollerId);
					dividerColumnList.Add(new_column);
					this.RegisterColumn(dividerID, new_column);
					currentSubgroupColumns.Clear();
				}

				SgtLogger.l("Adding column for " + skill.Name);


				var skillTableColumn = AddLabelColumn(skill.Id,
					(a, b) => on_load_skill(a, b, skill),
					(a, b) => get_value_skill_label(a, b, skill),
					(a, b) => this.compare_rows_skill(a, b, skill),
					(a, b, c) => this.on_tooltip_skill(a, b, c, skill),
					(a, b, c) => this.on_tooltip_sort_skill(a, b, c, skill),
					50,
					true);
				skillTableColumn.scrollerID = skillScrollerId;
				allItemColumns.Add(skillTableColumn);
				lastGroup = skill.skillGroup;
				currentSubgroupColumns.Add(skillTableColumn);


				//CheckboxTableColumn skillTableColumn =
				//	AddCheckboxColumn(skill.Id,
				//	(a, b) => on_load_skill(a, b, skill),
				//	(a, b) => this.get_value_skill_info(a, b, skill),
				//	(a) => this.on_click_skill_info(a, skill),
				//	(a, b) => this.set_value_skill_info(a, b, skill),
				//	(a, b) => this.compare_rows_skill(a, b, skill),
				//	(a, b, c) => this.on_tooltip_skill(a, b, c, skill),
				//	(a, b, c) => this.on_tooltip_sort_skill(a, b, c, skill)
				//	);
				//skillTableColumn.scrollerID = skillScrollerId;
				//
				//allItemColumns.Add(skillTableColumn);
				//lastGroup = skill.skillGroup;
				//currentSubgroupColumns.Add(skillTableColumn);
			}

			int size = 700;

			var rect = this.rectTransform(); 
			refresh_scrollers();
			this.MarkRowsDirty();
			rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
			GetComponentInChildren<KScrollRect>().GetComponentInParent<LayoutElement>().preferredHeight = size;
		}

		private string get_value_skill_label(IAssignableIdentity identity, GameObject widget_go, Skill skill)
		{
			if (identity is MinionIdentity minion)
			{
				if (minion.TryGetComponent<MinionResume>(out var resume))
				{
					return resume.HasMasteredSkill(skill.Id) ? "✓" : "X";
				}
			}
			else if (identity is StoredMinionIdentity storedMinion)
			{
				return storedMinion.HasMasteredSkill(skill.Id) ? "✓" : "X";
			}
			return "?";
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
			UIUtils.ListAllChildrenPath(this.transform);
			this.title_bar = transform.Find("Title/Label").gameObject.GetComponent<LocText>();
			this.CloseButton = transform.Find("Title/CloseButton").gameObject.GetComponent<KButton>();
			this.header_content_transform = transform.Find("HeaderContent");
			//consumable screen
			//this.scroll_content_transform = transform.Find("Content/Scroll View/Viewport/Content");
			//vitals screen
			this.scroll_content_transform = transform.Find("Content/Scroll View/Viewport/Content/ScrollContent");
			this._canvas = GetComponent<Canvas>();
			var vlg = scroll_content_transform.gameObject.GetComponent<VerticalLayoutGroup>();
			vlg.padding.left = 8;
			vlg.padding.right = 8;
		}


		private void on_click_skill_info(GameObject widget_go, Skill skill)
		{
			TableRow widgetRow = this.GetWidgetRow(widget_go);
			IAssignableIdentity identity = widgetRow.GetIdentity();
		}

		private TableScreen.ResultValues get_value_skill_info(
		  IAssignableIdentity minion,
		  GameObject widget_go,
			Skill skill)
		{

			return ResultValues.NotApplicable;
		}

		private void set_value_skill_info(GameObject widget_go, TableScreen.ResultValues new_value, Skill skill)
		{
			return;
			TableRow widgetRow = this.GetWidgetRow(widget_go);
			if ((UnityEngine.Object)widgetRow == (UnityEngine.Object)null)
			{
				Debug.LogWarning("Row is null");
			}
			else
			{
				CheckboxTableColumn widgetColumn = this.GetWidgetColumn(widget_go) as CheckboxTableColumn;
				IAssignableIdentity identity = widgetRow.GetIdentity();
				switch (widgetRow.rowType)
				{
					case TableRow.RowType.Header:
						this.set_value_skill_info(this.default_row.GetComponent<TableRow>().GetWidget(widgetColumn), new_value, skill);
						this.StartCoroutine(this.CascadeSetColumnCheckBoxes(this.all_sortable_rows, widgetColumn, new_value, widget_go));
						break;
					case TableRow.RowType.Default:
						//if (new_value == TableScreen.ResultValues.True)
						//	ConsumerManager.instance.DefaultForbiddenTagsList.Remove(consumableInfo.ConsumableId.ToTag());
						//else
						//	ConsumerManager.instance.DefaultForbiddenTagsList.Add(consumableInfo.ConsumableId.ToTag());
						widgetColumn.on_load_action(identity, widget_go);
						using (Dictionary<TableRow, GameObject>.Enumerator enumerator = widgetColumn.widgets_by_row.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<TableRow, GameObject> current = enumerator.Current;
								if (current.Key.rowType == TableRow.RowType.Header)
								{
									widgetColumn.on_load_action((IAssignableIdentity)null, current.Value);
									break;
								}
							}
							break;
						}
					case TableRow.RowType.Minion:
						MinionIdentity minionIdentity = identity as MinionIdentity;
						if (!((UnityEngine.Object)minionIdentity != (UnityEngine.Object)null))
							break;
						ConsumableConsumer component = minionIdentity.GetComponent<ConsumableConsumer>();
						if ((UnityEngine.Object)component == (UnityEngine.Object)null)
						{
							Debug.LogError("Could not find minion identity / row associated with the widget");
							break;
						}
						switch (new_value)
						{
							case TableScreen.ResultValues.False:
							case TableScreen.ResultValues.Partial:
								//component.SetPermitted(consumableInfo.ConsumableId, false);
								break;
							case TableScreen.ResultValues.True:
							case TableScreen.ResultValues.ConditionalGroup:
								//component.SetPermitted(consumableInfo.ConsumableId, true);
								break;
						}
						widgetColumn.on_load_action(widgetRow.GetIdentity(), widget_go);
						using (Dictionary<TableRow, GameObject>.Enumerator enumerator = widgetColumn.widgets_by_row.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								KeyValuePair<TableRow, GameObject> current = enumerator.Current;
								if (current.Key.rowType == TableRow.RowType.Header)
								{
									widgetColumn.on_load_action((IAssignableIdentity)null, current.Value);
									break;
								}
							}
							break;
						}
				}
			}
		}


		private void on_load_qualityoflife_expectations(IAssignableIdentity minion, GameObject widget_go)
		{
			TableRow widgetRow = this.GetWidgetRow(widget_go);
			LocText componentInChildren = widget_go.GetComponentInChildren<LocText>(true);
			if (minion != null)
				componentInChildren.text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
			else
				componentInChildren.text = widgetRow.isDefault ? "" : STRINGS.UI.VITALSSCREEN.QUALITYOFLIFE_EXPECTATIONS.ToString();
		}

		private string get_value_qualityoflife_label(IAssignableIdentity minion, GameObject widget_go)
		{
			string qualityoflifeLabel = "";
			TableRow widgetRow = this.GetWidgetRow(widget_go);
			if (widgetRow.rowType == TableRow.RowType.Minion)
				qualityoflifeLabel = Db.Get().Attributes.QualityOfLife.Lookup((Component)(minion as MinionIdentity)).GetFormattedValue();
			else if (widgetRow.rowType == TableRow.RowType.StoredMinon)
				qualityoflifeLabel = (string)STRINGS.UI.TABLESCREENS.NA;
			return qualityoflifeLabel;
		}

		private int compare_rows_qualityoflife_expectations(IAssignableIdentity a, IAssignableIdentity b)
		{
			MinionIdentity cmp1 = a as MinionIdentity;
			MinionIdentity cmp2 = b as MinionIdentity;
			if ((UnityEngine.Object)cmp1 == (UnityEngine.Object)null && (UnityEngine.Object)cmp2 == (UnityEngine.Object)null)
				return 0;
			if ((UnityEngine.Object)cmp1 == (UnityEngine.Object)null)
				return -1;
			return (UnityEngine.Object)cmp2 == (UnityEngine.Object)null ? 1 : Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component)cmp1).GetTotalValue().CompareTo(Db.Get().Attributes.QualityOfLifeExpectation.Lookup((Component)cmp2).GetTotalValue());
		}
		protected void on_tooltip_qualityoflife_expectations(
			IAssignableIdentity minion,
			GameObject widget_go,
			ToolTip tooltip)
		{
			tooltip.ClearMultiStringTooltip();
			switch (this.GetWidgetRow(widget_go).rowType)
			{
				case TableRow.RowType.Minion:
					MinionIdentity cmp = minion as MinionIdentity;
					if (!((UnityEngine.Object)cmp != (UnityEngine.Object)null))
						break;
					tooltip.AddMultiStringTooltip(Db.Get().Attributes.QualityOfLife.Lookup((Component)cmp).GetAttributeValueTooltip(), (TextStyleSetting)null);
					break;
				case TableRow.RowType.StoredMinon:
					this.StoredMinionTooltip(minion, tooltip);
					break;
			}
		}

		protected void on_tooltip_sort_qualityoflife_expectations(
		  IAssignableIdentity minion,
		  GameObject widget_go,
		  ToolTip tooltip)
		{
			tooltip.ClearMultiStringTooltip();
			switch (this.GetWidgetRow(widget_go).rowType)
			{
				case TableRow.RowType.Header:
					tooltip.AddMultiStringTooltip((string)STRINGS.UI.TABLESCREENS.COLUMN_SORT_BY_EXPECTATIONS, (TextStyleSetting)null);
					break;
			}
		}

		protected void on_tooltip_name(IAssignableIdentity minion, GameObject widget_go, ToolTip tooltip)
		{
			tooltip.ClearMultiStringTooltip();
			switch (this.GetWidgetRow(widget_go).rowType)
			{
				case TableRow.RowType.Minion:
					if (minion == null)
						break;
					tooltip.AddMultiStringTooltip(string.Format(STRINGS.UI.TABLESCREENS.GOTO_DUPLICANT_BUTTON, minion.GetProperName()), (TextStyleSetting)null);
					break;
			}
		}

		private void refresh_scrollers()
		{
			bool enableScroller = true;
			foreach (TableRow row in this.rows)
			{
				GameObject scroller = row.GetScroller(skillScrollerId);
				if ((UnityEngine.Object)scroller != (UnityEngine.Object)null)
				{
					ScrollRect component = scroller.transform.parent.GetComponent<ScrollRect>();
					if ((UnityEngine.Object)component.horizontalScrollbar != (UnityEngine.Object)null)
					{
						component.horizontalScrollbar.gameObject.SetActive(enableScroller);
						row.GetScrollerBorder(skillScrollerId).gameObject.SetActive(enableScroller);
					}
					component.horizontal = enableScroller;
					component.enabled = enableScroller;
				}
			}
		}
		private void on_load_skill(IAssignableIdentity minion, GameObject widget_go, Skill skill)
		{
			TableRow widgetRow = this.GetWidgetRow(widget_go);
			LocText LocText = widget_go.GetComponentInChildren<LocText>(true);
			var HLG = widgetRow.GetComponent<HorizontalLayoutGroup>();
			if(HLG.padding.left != 1)
			{
				HLG.padding = new(1, 1, 0, 0);
				LocText.transform.parent.gameObject.GetComponent<LayoutElement>().minWidth = 128;
			}

			if (minion != null)
			{
				var horizontal = widgetRow.GetComponent<HorizontalLayoutGroup>();
				if (LocText.fontSize < 32f)
				{
					LocText.fontSize = 32f;
					LocText.alignment = TMPro.TextAlignmentOptions.Center;
				}
				LocText.text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
			}
			else
			{
				LocText.text = widgetRow.isDefault ? "" : skill.Name;
				//LocText.enableWordWrapping = false;
			}
			return;
			//TableRow widgetRow = this.GetWidgetRow(widget_go);
			//TableColumn widgetColumn = this.GetWidgetColumn(widget_go);
			//MultiToggle multiToggle = widget_go.GetComponent<MultiToggle>();
			//if (!widgetColumn.isRevealed)
			//{
			//	widget_go.SetActive(false);
			//}
			//else
			//{
			//	if (!widget_go.activeSelf)
			//		widget_go.SetActive(true);
			//	switch (widgetRow.rowType)
			//	{
			//		case TableRow.RowType.Header:
			//			Image reference = widget_go.GetComponent<HierarchyReferences>().GetReference("PortraitImage") as Image;
			//			reference.sprite = Assets.GetSprite(skill.badge);
			//			reference.color = Color.white;
			//			//reference.material = (double)ClusterManager.Instance.activeWorld.worldInventory.GetAmount(consumableInfo.ConsumableId.ToTag(), false) > 0 ? Assets.UIPrefabs.TableScreenWidgets.DefaultUIMaterial : Assets.UIPrefabs.TableScreenWidgets.DesaturatedUIMaterial;
			//			break;
			//		case TableRow.RowType.Default:
			//			switch (this.get_value_skill_info(minion, widget_go, skill))
			//			{
			//				case TableScreen.ResultValues.False:
			//					multiToggle.ChangeState(0);
			//					break;
			//				case TableScreen.ResultValues.True:
			//					multiToggle.ChangeState(1);
			//					break;
			//				case TableScreen.ResultValues.ConditionalGroup:
			//					multiToggle.ChangeState(2);
			//					break;
			//			}
			//			break;
			//		case TableRow.RowType.Minion:
			//		case TableRow.RowType.StoredMinon:
			//			MinionIdentity minionIdentity = minion as MinionIdentity;
			//			bool flag = false;
			//			switch (this.get_value_skill_info(minion, widget_go, skill))
			//			{
			//				case TableScreen.ResultValues.False:
			//					multiToggle.ChangeState(0);
			//					break;
			//				case TableScreen.ResultValues.True:
			//					multiToggle.ChangeState(1);
			//					break;
			//				case TableScreen.ResultValues.ConditionalGroup:
			//					multiToggle.ChangeState(2);
			//					break;
			//				case TableScreen.ResultValues.NotApplicable:
			//					flag = true;
			//					break;
			//			}
			//			if (flag)
			//			{
			//				multiToggle.ChangeState(3);
			//				(widget_go.GetComponent<HierarchyReferences>().GetReference("BGImage") as Image).color = Color.clear;
			//				break;
			//			}
			//			//color level
			//			if ((minion as MinionIdentity) != null)
			//			{
			//				//(widget_go.GetComponent<HierarchyReferences>().GetReference("BGImage") as Image).color = new Color(0.7215686f, 0.4431373f, 0.5803922f, Mathf.Max((float)((double)foodInfo.Quality - (double)Db.Get().Attributes.FoodExpectation.Lookup((Component)(minion as MinionIdentity)).GetTotalValue() + 1.0), 0.0f) * 0.25f);
			//				break;
			//			}
			//			break;
			//	}
			//	this.refresh_scrollers();
			//	return;
			//	//TableRow widgetRow = this.GetWidgetRow(widget_go);
			//	//LocText LocText = widget_go.GetComponentInChildren<LocText>(true);
			//	//var HLG = widgetRow.GetComponent<HorizontalLayoutGroup>();
			//	//HLG.padding = new(1, 1, 0, 0);
			//	//LocText.transform.parent.gameObject.GetComponent<LayoutElement>().minWidth = 128;

			//	//if (minion != null)
			//	//{
			//	//	var horizontal = widgetRow.GetComponent<HorizontalLayoutGroup>();

			//	//	LocText.fontSize = 32f;
			//	//	LocText.alignment = TMPro.TextAlignmentOptions.Center;
			//	//	LocText.text = (this.GetWidgetColumn(widget_go) as LabelTableColumn).get_value_action(minion, widget_go);
			//	//}
			//	//else
			//	//{
			//	//	LocText.text = widgetRow.isDefault ? "" : skill.Name;
			//	//	LocText.enableWordWrapping = false;
			//	//}
			//}
		}

		Color GetIntensityColor(float level, string attributeId)
		{
			if (RainbowGradient)
			{
				level = Mathf.Clamp(level, 0, 45);

				//return ColorGradient.Evaluate(level / 45f);

				float hsvShift = (level * 6f) / 360f;
				return UIUtils.Darken(UIUtils.HSVShift(Color.red, hsvShift * 100), 25);
			}
			float maxLevel = 20;// MaxSkillLevels[attributeId];
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


		private int compare_rows_skill(IAssignableIdentity a, IAssignableIdentity b, Skill skill)
		{
			int aVal = 0;// GetAttributeLevel(a, attribute);
			int bVal = 0;//GetAttributeLevel(b, attribute);

			if (aVal == bVal)
				return 0;
			if (aVal < bVal)
				return 1;
			return -1;
		}

		protected void on_tooltip_skill(
		  IAssignableIdentity minion,
		  GameObject widget_go,
		  ToolTip tooltip, Skill skill)
		{
			tooltip.ClearMultiStringTooltip();

			//if (lvl < 0)
			//	return;
			//string tooltipString = STRINGS.UI.JOBSSCREEN.MINION_SKILL_TOOLTIP.Replace("{Name}", minion.GetProperName()).Replace("{Attribute}", attribute.Name) + lvl;
			tooltip.AddMultiStringTooltip(skill.description, null);
		}

		protected void on_tooltip_sort_skill(
		  IAssignableIdentity minion,
		  GameObject widget_go,
		  ToolTip tooltip,
		  Skill skill)
		{
			tooltip.ClearMultiStringTooltip();
			string sortByTooltip = global::STRINGS.UI.MINION_IDENTITY_SORT.TITLE;
			sortByTooltip += ": ";
			sortByTooltip += skill.Name;
			sortByTooltip += " ";
			sortByTooltip += STRINGS.DUPLICANTS.NEEDS.QUALITYOFLIFE.EXPECTATION_MOD_NAME;

			switch (this.GetWidgetRow(widget_go).rowType)
			{
				case TableRow.RowType.Header:
					tooltip.AddMultiStringTooltip(sortByTooltip, null);
					break;
			}
		}

		private void StoredMinionTooltip(IAssignableIdentity minion, ToolTip tooltip)
		{
			StoredMinionIdentity storedMinionIdentity = minion as StoredMinionIdentity;
			if (!((UnityEngine.Object)storedMinionIdentity != (UnityEngine.Object)null))
				return;
			tooltip.AddMultiStringTooltip(string.Format((string)STRINGS.UI.TABLESCREENS.INFORMATION_NOT_AVAILABLE_TOOLTIP, storedMinionIdentity.GetStorageReason(), storedMinionIdentity.GetProperName()), (TextStyleSetting)null);
		}
	}
}

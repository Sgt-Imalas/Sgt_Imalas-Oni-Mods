using HarmonyLib;
using System.Collections.Generic;
using UnityEngine.UI;
using UtilLibs;

namespace DupePrioPresetManager
{
	internal class Patches
	{
		/// <summary>
		/// attach prio manager to each row
		/// </summary>
		[HarmonyPatch(typeof(ConsumablesTableScreen), nameof(ConsumablesTableScreen.refresh_scrollers))]
		public static class AddButtonFunctionalityForConsumables
		{
			public static void Postfix(ConsumablesTableScreen __instance)
			{
				ModAssets.ParentScreen = __instance.transform.parent.gameObject;
				foreach (var row in __instance.rows)
				{
					if (row.rowType == TableRow.RowType.Minion || row.rowType == TableRow.RowType.StoredMinon || row.rowType == TableRow.RowType.Default)
					{

						HashSet<Tag> forbiddenConsumables = null;
						System.Action<HashSet<Tag>> ApplyAction = null;

						switch (row.rowType)
						{
							case TableRow.RowType.Default:
								forbiddenConsumables = new HashSet<Tag>(ConsumerManager.instance.DefaultForbiddenTagsList);
								ApplyAction = (hashSet) =>
								{
									ConsumerManager.instance.DefaultForbiddenTagsList.Clear();
									ConsumerManager.instance.DefaultForbiddenTagsList.AddRange(hashSet);
								};
								break;
							case TableRow.RowType.Minion:
								MinionIdentity identity = row.GetIdentity() as MinionIdentity;
								if (identity != null)
								{
									if (identity.TryGetComponent<ConsumableConsumer>(out var consumer))
										forbiddenConsumables = new HashSet<Tag>(consumer.forbiddenTagSet);
									ApplyAction = (hashSet) =>
									{
										if (identity.TryGetComponent<ConsumableConsumer>(out var consumer))
										{
											consumer.forbiddenTagSet = hashSet;
										}
									};
									break;
								}
								break;
							case TableRow.RowType.StoredMinon:
								var minionIdentity = (row.GetIdentity() as StoredMinionIdentity);
								if (minionIdentity != null)
								{
									forbiddenConsumables = new HashSet<Tag>(minionIdentity.forbiddenTagSet);
									ApplyAction = (hashSet) =>
									{
										minionIdentity.forbiddenTagSet = hashSet;
									};
									break;
								}
								break;
						}

						System.Action openPresetMenu = () =>
						{
							UnityPresetScreen_Consumables.ShowWindow(forbiddenConsumables, ApplyAction, () =>
							{
								__instance.MarkRowsDirty();
							});
						};

						var btnTransform = row.transform.Find("PresetButton");
						if (btnTransform == null)
						{
							btnTransform = Util.KInstantiateUI(__instance.transform.Find("Title/CloseButton").gameObject, row.transform.Find("").gameObject, true).transform; //.GetComponent<KButton>();
							btnTransform.name = "PresetButton";
							btnTransform.SetSiblingIndex(2);
							btnTransform.TryGetComponent<LayoutElement>(out var ele);

							ele.flexibleHeight = -1;
							ele.preferredHeight = 40;
							ele.preferredWidth = 20;
							ele.minWidth = 20;
							UIUtils.AddSimpleTooltipToObject(btnTransform, STRINGS.UI.PRESETWINDOWDUPEPRIOS.OPENPRESETWINDOW, true, onBottom: true);

							row.transform.Find("MinionPortrait").TryGetComponent<LayoutElement>(out var minionPortrait);
							minionPortrait.minWidth = 30;

							btnTransform.TryGetComponent<KButton>(out var button);
							button.onClick += openPresetMenu;
							btnTransform.Find("GameObject").TryGetComponent<Image>(out var image);
							image.sprite = Assets.GetSprite("iconPaste");
						}

						//UIUtils.AddActionToButton(row.transform, "MinionPortrait", openPresetMenu);
						//UIUtils.AddActionToButton(row.transform, "LabelHeader/SortToggle(Clone)", openPresetMenu, false);
					}
				}
			}
		}

		[HarmonyPatch(typeof(JobsTableScreen), nameof(JobsTableScreen.RefreshRows))]
		public static class AddButtonFunctionalityForFood
		{
			public static void Postfix(JobsTableScreen __instance)
			{
				ModAssets.ParentScreen = __instance.transform.parent.gameObject;
				foreach (var row in __instance.rows)
				{
					if (row.rowType == TableRow.RowType.Minion || row.rowType == TableRow.RowType.StoredMinon || row.rowType == TableRow.RowType.Default)
					{

						IPersonalPriorityManager priorityManager = (IPersonalPriorityManager)null;
						IAssignableIdentity assignableIdentity = null;

						switch (row.rowType)
						{
							case TableRow.RowType.Default:
								priorityManager = (IPersonalPriorityManager)Immigration.Instance;
								break;
							case TableRow.RowType.Minion:
								MinionIdentity identity = row.GetIdentity() as MinionIdentity;
								if (identity != null)
								{
									assignableIdentity = identity;
									priorityManager = (IPersonalPriorityManager)identity.GetComponent<ChoreConsumer>();
									break;
								}
								break;
							case TableRow.RowType.StoredMinon:
								priorityManager = (row.GetIdentity() as StoredMinionIdentity);
								assignableIdentity = row.GetIdentity();
								break;
						}

						System.Action openPresetMenu = () =>
						{
							UnityPresetScreen_Priorities.ShowWindow(priorityManager, () =>
							{
								foreach (KeyValuePair<string, TableColumn> column in __instance.columns)
								{
									if (column.Value == null || column.Value.on_load_action == null)
									{
										continue;
									}

									if (column.Value.widgets_by_row.ContainsKey(row))
									{
										if (column.Value.widgets_by_row[row] != null)
										{
											column.Value.on_load_action(assignableIdentity, column.Value.widgets_by_row[row]);
										}
									}
									column.Value.on_load_action(null, __instance.rows[0].GetWidget(column.Value));
								}
							}
						);
						};

						var btnTransform = row.transform.Find("PresetButton");
						if (btnTransform == null)
						{
							btnTransform = Util.KInstantiateUI(__instance.transform.Find("Title/CloseButton").gameObject, row.transform.Find("").gameObject, true).transform; //.GetComponent<KButton>();
							btnTransform.name = "PresetButton";
							btnTransform.SetSiblingIndex(2);
							btnTransform.TryGetComponent<LayoutElement>(out var ele);
							ele.flexibleHeight = -1;
							ele.preferredHeight = 48;
							ele.preferredWidth = 20;
							ele.minWidth = 20;

							UIUtils.AddSimpleTooltipToObject(btnTransform, STRINGS.UI.PRESETWINDOWDUPEPRIOS.OPENPRESETWINDOW, true, onBottom: true);

							row.transform.Find("MinionPortrait").TryGetComponent<LayoutElement>(out var minionPortrait);
							minionPortrait.minWidth = 30;



							btnTransform.TryGetComponent<KButton>(out var button);
							button.onClick += openPresetMenu;
							btnTransform.Find("GameObject").TryGetComponent<Image>(out var image);
							image.sprite = Assets.GetSprite("iconPaste");
						}

						//UIUtils.AddActionToButton(row.transform, "LabelHeader/SortToggle(Clone)", openPresetMenu, false);
					}
					// UIUtils.ListAllChildrenWithComponents(row.transform);
				}
			}
		}


		[HarmonyPatch(typeof(JobsTableScreen), nameof(JobsTableScreen.OnKeyDown))]
		public static class DisableGrabbing
		{
			public static bool Prefix(JobsTableScreen __instance, KButtonEvent e)
			{
				if (UnityPresetScreen_Priorities.Instance != null && UnityPresetScreen_Priorities.Instance.CurrentlyActive)
				{
					e.Consumed = true;
					return false;
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(ScheduleScreen), "OnPrefabInit")]
		public static class ScheduleScreen_OnPrefabInit_Patch
		{
			/// <summary>
			/// Grab them Colours
			/// </summary>

			[HarmonyPriority(Priority.LowerThanNormal)]
			internal static void Postfix(ScheduleScreen __instance)
			{
				UnityScreen_ScheduleShifterPopup.RefreshAllAction = () => { __instance.OnSchedulesChanged(ScheduleManager.Instance.schedules); };
			}
		}
		[HarmonyPatch(typeof(ScheduleScreenEntry), nameof(ScheduleScreenEntry.Setup))]
		public static class AddPresetButtonToScheduleEntry
		{
			/// <summary>
			/// Grab them Colours
			/// </summary>

			internal static void Postfix(ScheduleScreenEntry __instance, Schedule schedule)
			{
				UIUtils.ListAllChildrenPath(__instance.duplicateScheduleButton.transform);
				//UIUtils.ListAllChildrenWithComponents(__instance.transform);
				var btn = __instance.duplicateScheduleButton;
				var ButtonPresets = Util.KInstantiateUI(btn.gameObject, btn.transform.parent.gameObject).GetComponent<KButton>();
				ButtonPresets.transform.SetSiblingIndex(3);
				ButtonPresets.name = "PresetButton";
				ButtonPresets.transform.Find("FG").TryGetComponent<Image>(out var imageBT);
				UIUtils.AddSimpleTooltipToObject(ButtonPresets.transform, STRINGS.UI.PRESETWINDOWDUPEPRIOS.OPENPRESETWINDOW, true, onBottom: true);

				imageBT.sprite = Assets.GetSprite("iconPaste");
				ButtonPresets.onClick += () => UnityPresetScreen_Schedule.ShowWindow(schedule,
					() =>
					{
						__instance.OnScheduleChanged(schedule);
						__instance.title.SetTitle(schedule.name);
					}, schedule.name);


				var ButtonShift = Util.KInstantiateUI(btn.gameObject, btn.transform.parent.gameObject).GetComponent<KButton>();
				ButtonShift.transform.SetSiblingIndex(4);
				ButtonShift.name = "ShiftScheduleButton";
				ButtonShift.transform.Find("FG").TryGetComponent<Image>(out var imageShift);
				UIUtils.AddSimpleTooltipToObject(ButtonShift.transform, STRINGS.UI.PRESETWINDOWDUPEPRIOS.OPENSHIFTCLONE, true, onBottom: true);

				imageShift.sprite = Assets.GetSprite("action_direction_both");
				ButtonShift.onClick += () => UnityScreen_ScheduleShifterPopup.ShowWindow(schedule,
					//ButtonShift.gameObject
					ModAssets.ParentScreen
					 , () =>
					{
						__instance.OnScheduleChanged(schedule);
						__instance.title.SetTitle(schedule.name);
					}
					);
			}
		}

		[HarmonyPatch(typeof(ScheduleScreen), "OnSpawn")]
		public static class ScheduleScreen_OnSpawn_Patch
		{
			internal static void Postfix(ScheduleScreen __instance)
			{
				ModAssets.ParentScreen = __instance.transform.parent.gameObject;

				var Button = Util.KInstantiateUI(__instance.addScheduleButton.gameObject, __instance.addScheduleButton.transform.parent.gameObject).GetComponent<KButton>();
				//Button.transform.SetSiblingIndex(2);
				UIUtils.TryChangeText(Button.transform, "Label", STRINGS.UI.PRESETWINDOWDUPEPRIOS.SCHEDULESTRINGS.GENERATEALL);
				UIUtils.AddSimpleTooltipToObject(Button.transform, STRINGS.UI.PRESETWINDOWDUPEPRIOS.SCHEDULESTRINGS.GENERATEALLTOOLTIP, true, onBottom: true);
				Button.name = "AddAllPresetSchedules";
				Button.onClick += () => UnityPresetScreen_Schedule.GenerateAllDefaultPresets();

			}
		}

		/// <summary>
		/// Init. auto translation
		/// </summary>
		[HarmonyPatch(typeof(Localization), "Initialize")]
		public static class Localization_Initialize_Patch
		{
			public static void Postfix()
			{
				LocalisationUtil.Translate(typeof(STRINGS), true);
			}
		}
	}
}

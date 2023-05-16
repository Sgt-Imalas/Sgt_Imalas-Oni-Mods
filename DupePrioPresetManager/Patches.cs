using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static DupePrioPresetManager.ModAssets;
using static ResearchTypes;

namespace DupePrioPresetManager
{
    internal class Patches
    {
        /// <summary>
        /// attach prio manager to each row
        /// </summary>
        [HarmonyPatch(typeof(ConsumablesTableScreen))]
        [HarmonyPatch(nameof(ConsumablesTableScreen.refresh_scrollers))]
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
                                    if(identity.TryGetComponent<ConsumableConsumer>(out var consumer))
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
                            UnityPresetScreen_Consumables.ShowWindow(forbiddenConsumables,ApplyAction, () => 
                            { 
                                __instance.MarkRowsDirty();
                            });
                        };
                        UIUtils.AddActionToButton(row.transform, "MinionPortrait", openPresetMenu);
                        //UIUtils.AddActionToButton(row.transform, "LabelHeader/SortToggle(Clone)", openPresetMenu, false);
                    }
                }
            }
        }

        [HarmonyPatch(typeof(JobsTableScreen))]
        [HarmonyPatch(nameof(JobsTableScreen.RefreshRows))]
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
                        UIUtils.AddActionToButton(row.transform, "MinionPortrait", openPresetMenu);
                        //UIUtils.AddActionToButton(row.transform, "LabelHeader/SortToggle(Clone)", openPresetMenu, false);
                    }
                    // UIUtils.ListAllChildrenWithComponents(row.transform);
                }
            }
        }


        [HarmonyPatch(typeof(JobsTableScreen))]
        [HarmonyPatch(nameof(JobsTableScreen.OnKeyDown))]
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

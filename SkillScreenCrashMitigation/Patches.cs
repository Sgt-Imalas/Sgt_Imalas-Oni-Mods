using HarmonyLib;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace SkillScreenCrashMitigation
{
	internal class Patches
	{
		[HarmonyPatch(typeof(SkillsScreen))]
		[HarmonyPatch(nameof(SkillsScreen.SortRows))]
		public class ToggleSkinButtonVisibility
		{
			public static bool Prefix(SkillsScreen __instance, Comparison<IAssignableIdentity> comparison)
			{
				__instance.active_sort_method = comparison;
				Dictionary<IAssignableIdentity, SkillMinionWidget> dictionary = new Dictionary<IAssignableIdentity, SkillMinionWidget>();

				HashSet<IAssignableIdentity> distincts = new();
				foreach (SkillMinionWidget sortableRow in __instance.sortableRows)
				{
					distincts.Add(sortableRow.assignableIdentity);
					dictionary.Add(sortableRow.assignableIdentity, sortableRow);
				}
				Dictionary<int, List<IAssignableIdentity>> minionsByWorld = ClusterManager.Instance.MinionsByWorld;
				__instance.sortableRows.Clear();
				Dictionary<int, int> dictionary2 = new Dictionary<int, int>();
				int num = 0;
				int num2 = 0;

				foreach (KeyValuePair<int, List<IAssignableIdentity>> item2 in minionsByWorld)
				{
					dictionary2.Add(item2.Key, num);
					num++;
					List<IAssignableIdentity> list = new List<IAssignableIdentity>();
					foreach (IAssignableIdentity item3 in item2.Value)
					{
						if (!distincts.Contains(item3))
							continue;

						SgtLogger.l("adding proxy for " + item3.GetProperName() + " to list");
						list.Add(item3);
					}

					if (comparison != null)
					{
						list.Sort(comparison);
						if (__instance.sortReversed)
						{
							list.Reverse();
						}
					}

					num += list.Count;
					num2 += list.Count;
					for (int i = 0; i < list.Count; i++)
					{
						SgtLogger.l("Trying to get assignable identity at index" + i);
						IAssignableIdentity key = list[i];
						SgtLogger.l("Trying to get widged for " + key.GetProperName());
						SkillMinionWidget item = dictionary[key];
						__instance.sortableRows.Add(item);
					}
				}

				for (int j = 0; j < __instance.sortableRows.Count; j++)
				{
					__instance.sortableRows[j].gameObject.transform.SetSiblingIndex(j);
				}

				foreach (KeyValuePair<int, int> item4 in dictionary2)
				{
					__instance.worldDividers[item4.Key].transform.SetSiblingIndex(item4.Value);
				}
				return false;
			}
		}
	}
}

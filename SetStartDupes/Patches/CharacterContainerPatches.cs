using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace SetStartDupes.Patches
{
	internal class CharacterContainerPatches
	{
		/// <summary>
		/// Reorder the aptitude entries so entries with the same skillgroup end up on top of each other.
		/// also remove preferred height so it stretches with more aptitude entries
		/// </summary>
		[HarmonyPatch(typeof(CharacterContainer), nameof(CharacterContainer.SetInfoText))]
		public class CharacterContainer_SetInfoText_Patch
		{
			public static void Postfix(CharacterContainer __instance)
			{
				if (__instance.aptitudeEntry.transform.parent.parent.gameObject.TryGetComponent<LayoutElement>(out LayoutElement layoutElement))
				{
					/// Remove prev height so additional traits extend the box indstead of going hidden
					layoutElement.preferredHeight = -1;
				}

				if (__instance.stats.personality.model == GameTags.Minions.Models.Bionic) //no aptitude entries for bionic dupes
					return;

				var skillgroups = Db.Get().SkillGroups;
				Dictionary<SkillGroup, GameObject> AptitudeEntries = new(16);

				int index = 0;
				foreach (KeyValuePair<SkillGroup, float> skillAptitude in __instance.stats.skillAptitudes)
				{
					if (skillAptitude.Value == 0f)
					{
						continue;
					}

					SkillGroup skillGroup = skillgroups.Get(skillAptitude.Key.IdHash);
					if (skillGroup == null)
					{
						Debug.LogWarningFormat("Role group not found for aptitude: {0}", skillAptitude.Key);
						continue;
					}
					//store the aptitude entries associated gameobjects
					AptitudeEntries[skillGroup] = __instance.aptitudeEntries[index++];
				}

				var sortedEntries = AptitudeEntries.OrderBy(entry => entry.Key.relevantAttributes.First().Id).ToList();
				sortedEntries.ForEach(entry => entry.Value.transform.SetAsLastSibling());
			}
		}
	}
}

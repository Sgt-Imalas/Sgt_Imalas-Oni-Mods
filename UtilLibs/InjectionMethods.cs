using System;
using System.Collections.Generic;
using STRINGS;
using TUNING;

namespace UtilLibs
{
    public static class InjectionMethods
	{
		public static void AddBuildingToPlanScreen(HashedString category, string buildingId, string addAfterBuildingId = null)
		{
			var index = TUNING.BUILDINGS.PLANORDER.FindIndex(x => x.category == category);

			if (index == -1)
				return;

			if (!(TUNING.BUILDINGS.PLANORDER[index].data is IList<string> planOrderList))
			{
				Debug.Log($"Could not add {buildingId} to the building menu.");
				return;
			}

			var neighborIdx = planOrderList.IndexOf(addAfterBuildingId);

			if (neighborIdx != -1)
				planOrderList.Insert(neighborIdx + 1, buildingId);
			else
				planOrderList.Add(buildingId);
		}

		public static void AddBuildingToTechnology(string techId, string buildingId)
		{
			Db.Get().Techs.Get(techId).unlockedItemIDs.Add(buildingId);
		}
		public static void AddBuildingStrings(string buildingId, string name, string description = "tba", string effect = "tba")
		{
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.NAME", UI.FormatAsLink(name, buildingId));
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.DESC", description);
			Strings.Add($"STRINGS.BUILDINGS.PREFABS.{buildingId.ToUpperInvariant()}.EFFECT", effect);
		}


		public static void AddCreatureStrings(string creatureId, string name)
		{
			Strings.Add($"STRINGS.CREATURES.FAMILY.{creatureId.ToUpperInvariant()}", UI.FormatAsLink(name, creatureId));
			Strings.Add($"STRINGS.CREATURES.FAMILY_PLURAL.{creatureId.ToUpperInvariant()}", UI.FormatAsLink(name, creatureId));
		}
	}
}

using Database;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb
{
	class SpaceStationRoomsDB
	{
		public static string SpaceStationDeepSpaceResearchRoom = "RTB_SpaceStationDeepSpaceResearchRoom";

		public static void PatchRoomsConstructor()
		{
			var roomTypesType = AccessTools.TypeByName("Database.RoomTypes, Assembly-CSharp");
			if (roomTypesType == null)
			{
				SgtLogger.error("Room patcher failed");
				return;
			}

			var targetMethod = AccessTools.Constructor(roomTypesType, [typeof(ResourceSet)]);
			if (targetMethod == null)
			{
				SgtLogger.error("Room patcher failed, target method not found");
				return;
			}
			MethodInfo postfix = AccessTools.Method(typeof(SpaceStationRoomsDB), "RoomTypes_Postfix");
			Mod.harmonyInstance.Patch(targetMethod, postfix: new HarmonyMethod(postfix));
		}
		public static void RoomTypes_Postfix(ref RoomTypes __instance)
		{
			var MORE_SCIENCE_BUILDINGS = new RoomConstraints.Constraint((KPrefabID bc) => bc.HasTag(RoomConstraints.ConstraintTags.ScienceBuilding), null, 4, STRINGS.ROOMS.TYPES.RTB_SPACESTATIONRESEARCHROOM.ROOMCONSTRAINT.NAME, STRINGS.ROOMS.TYPES.RTB_SPACESTATIONRESEARCHROOM.ROOMCONSTRAINT.DESCRIPTION);
			var stationResearchRoom = new RoomType(
				SpaceStationDeepSpaceResearchRoom
				, STRINGS.ROOMS.TYPES.RTB_SPACESTATIONRESEARCHROOM.NAME
				, STRINGS.ROOMS.TYPES.RTB_SPACESTATIONRESEARCHROOM.TOOLTIP
				, STRINGS.ROOMS.TYPES.RTB_SPACESTATIONRESEARCHROOM.TOOLTIP
				, STRINGS.ROOMS.TYPES.RTB_SPACESTATIONRESEARCHROOM.EFFECT
				, Db.Get().RoomTypeCategories.Science
				, MORE_SCIENCE_BUILDINGS
				, [RoomConstraints.NO_INDUSTRIAL_MACHINERY,RoomConstraints.DECORATIVE_ITEM,RoomConstraints.MINIMUM_SIZE_32,RoomConstraints.MAXIMUM_SIZE_120]
				,[RoomDetails.SIZE,RoomDetails.BUILDING_COUNT]
				, single_assignee: true
				, priority_building_use: true
				, sortKey: 49
				);
			__instance.Add(stationResearchRoom);
		}
	}
}

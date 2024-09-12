using HarmonyLib;

namespace RoboRockets.Patches
{
	internal class StatusItemsAdder
	{/// <summary>
	 /// register custom status items
	 /// </summary>
		[HarmonyPatch(typeof(Database.BuildingStatusItems), "CreateStatusItems")]
		public static class Database_BuildingStatusItems_CreateStatusItems_Patch
		{
			public static void Postfix()
			{
				ModAssets.RegisterStatusItems();
			}
		}
	}
}

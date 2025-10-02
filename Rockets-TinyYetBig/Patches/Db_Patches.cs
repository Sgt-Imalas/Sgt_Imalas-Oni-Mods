using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.Patches
{
	internal class Db_Patches
	{

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
			public static void Prefix()
			{
				//SpaceStationRoomsDB.PatchRoomsConstructor();
			}
            public static void Postfix(Db __instance)
			{
				BuildingDatabase.AddBuildingsToTech();
				CarbonField_AnimationFix.ExecutePatch();
				CustomOxidizers.RegisterCustomOxidizers();
			}
        }
	}
}

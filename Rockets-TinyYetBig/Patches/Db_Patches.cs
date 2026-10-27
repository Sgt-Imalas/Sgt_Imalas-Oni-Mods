using HarmonyLib;
using Rockets_TinyYetBig.Content.ModDb;
using Rockets_TinyYetBig.Content.ModDb.BuildingConfigurations;

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
				BuildingManager.AddBuildingsToTechs();
				CarbonField_AnimationFix.ExecutePatch();
				CustomOxidizers.RegisterCustomOxidizers();
				ModStatusItems.Register();
			}
        }
	}
}

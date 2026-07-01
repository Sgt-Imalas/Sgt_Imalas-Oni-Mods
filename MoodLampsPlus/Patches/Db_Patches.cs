using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoodLampsPlus.Patches
{
	internal class Db_Patches
	{

        [HarmonyPatch(typeof(Db), nameof(Db.Initialize))]
        public class Db_Initialize_Patch
        {
            public static void Postfix(Db __instance)
            {
				ModAssets.RegisterMoodLamps();
            }
        }
	}
}

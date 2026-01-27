using BigSmallSculptures.Content.Scripts;
using FMOD;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigSmallSculptures.Patches
{
	internal class Artable_Patches
	{

        [HarmonyPatch(typeof(Sculpture), nameof(Sculpture.OnPrefabInit))]
        public class Sculpture_OnPrefabInit_Patch
        {
            public static void Postfix(Sculpture __instance)
            {
                __instance.gameObject.AddOrGet<SculptureAnimScaler>();
			}   
        }
	}
}

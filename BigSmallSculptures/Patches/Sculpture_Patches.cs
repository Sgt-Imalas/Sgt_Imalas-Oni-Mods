using BigSmallSculptures.Content.Scripts;
using HarmonyLib;

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

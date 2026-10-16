using HarmonyLib;

namespace AkisDecorPackB.Patches
{
	internal class Artable_Patches
	{

        [HarmonyPatch(typeof(Artable), nameof(Artable.SetDefault))]
        public class Artable_SetDefault_Patch
        {
            public static void Postfix(Artable __instance)
			{
				__instance.gameObject.Trigger(ModAssets.Hashes.FossilStageUnset);
			}
        }
	}
}

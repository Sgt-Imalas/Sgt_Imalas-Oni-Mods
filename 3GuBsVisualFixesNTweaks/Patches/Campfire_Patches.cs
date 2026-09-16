using HarmonyLib;

namespace _3GuBsVisualFixesNTweaks.Patches
{
    class Campfire_Patches
	{
		[HarmonyPatch(typeof(Campfire), nameof(Campfire.InitializeStates))]
		public class Campfire_InitializeStates_Patch
		{
			public static Campfire.State working_post;
			public static void Postfix(Campfire __instance)
			{
				working_post = new();
				working_post.sm = __instance;

				__instance.operational.working
					.PlayAnim("working_pre")
					.QueueAnim("on", true);
				__instance.operational.Enter(Campfire.DisableHeatEmission);
				__instance.operational.working.transitions.Clear();
				__instance.operational.working.EventTransition(GameHashes.OnStorageChange, working_post, Campfire.Not(Campfire.HasFuel));
				working_post.PlayAnim("working_pst").OnAnimQueueComplete(__instance.operational.needsFuel);

			}
		}
	}
}

using HarmonyLib;

namespace AquaticMinnowMinion.Patches
{
	internal class ConsumableConsumer_Patches
	{

        [HarmonyPatch(typeof(ConsumableConsumer), nameof(ConsumableConsumer.SetModelDietaryRestrictions))]
        public class ConsumableConsumer_SetModelDietaryRestrictions_Patch
        {
            public static void Postfix(ConsumableConsumer __instance)
			{
				if (__instance.HasTag(ModAssets.Tags.AquaticMinion))
				{
					__instance.dietaryRestrictionTagSet = [.. ConsumerManager.instance.StandardDuplicantDietaryRestrictions];
				}
			}
        }
	}
}

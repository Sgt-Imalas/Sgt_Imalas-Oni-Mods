using AkisSnowThings.Content.Scripts.Entities;
using HarmonyLib;

namespace AkisSnowThings.Patches.Plants
{
	internal class BuildingAttachPointPatch
	{

        [HarmonyPatch(typeof(BuildingAttachPoint), nameof(BuildingAttachPoint.AcceptsAttachment))]
        public class BuildingAttachPoint_AcceptsAttachment_Patch
		{
            public static void Postfix(ref bool __result, BuildingAttachPoint __instance)
            {
				if (!__result)
					return;

				if (!__instance.TryGetComponent<TreeAttachment>(out var treeAttachment))
					return;

				if (!treeAttachment.CanAttachToTree())
					__result = false;

			}
        }
	}
}

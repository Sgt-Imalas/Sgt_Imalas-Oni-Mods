using AkisDecorPackB.Content.Defs.Items;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkisDecorPackB.Patches
{
	internal class SandboxToolParameterMenu_Patches
	{
		private static readonly HashSet<Tag> items = new()
		{
			FossilNoduleConfig.ID,
			GiantFossilFragmentConfigs.BRONTO,
			GiantFossilFragmentConfigs.LIVAYATAN,
			GiantFossilFragmentConfigs.TREX,
			GiantFossilFragmentConfigs.TRICERATOPS,
		};
		[HarmonyPatch(typeof(SandboxToolParameterMenu), nameof(SandboxToolParameterMenu.ConfigureEntitySelector))]
		public static class SandboxToolParameterMenu_ConfigureEntitySelector_Patch
		{
			public static void Postfix(SandboxToolParameterMenu __instance)
			{

				var menu = SandboxUtil.AddModMenu(
					__instance,
					"Decor Pack II",
					Def.GetUISprite(Assets.GetPrefab(FossilNoduleConfig.ID)),
					obj => obj is KPrefabID id && items.Contains(id.PrefabTag));

				SandboxUtil.UpdateOptions(__instance);
			}
		}
	}
}

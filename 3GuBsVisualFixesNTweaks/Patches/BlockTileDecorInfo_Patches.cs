using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Patches
{
	class BlockTileDecorInfo_Patches
	{
		[HarmonyPatch(typeof(BlockTileDecorInfo), nameof(BlockTileDecorInfo.PostProcess))]
		public class RemoveDecors
		{

			private static readonly List<string> bitPieces = new List<string> { "tops" };
			private static readonly HashSet<string> fullTops = ["tops", "top_single", "tops_center", "tops_left_corner", "tops_right_corner"];
			[HarmonyPriority(Priority.Low)]
			static void Postfix(ref BlockTileDecorInfo __instance)
			{
				var bitsConfig = Config.Instance.HiddenTileBits;

				if (bitsConfig == Config.TileBitChange.None)
					return;

				for (int i = 0; i < __instance.decor.Count(); i++)
				{
					var name = __instance.decor[i].name;

					switch (bitsConfig)
					{
						case Config.TileBitChange.BitsOnly:
							if (bitPieces.Contains(name))
								__instance.decor[i].probabilityCutoff = float.MaxValue;
							break;
						case Config.TileBitChange.BitsAndTops:
							if (fullTops.Contains(name))
								__instance.decor[i].probabilityCutoff = float.MaxValue;
							break;
						case Config.TileBitChange.Everything:
							__instance.decor[i].probabilityCutoff = float.MaxValue;
							break;
					}
				}
			}
		}
	}
}

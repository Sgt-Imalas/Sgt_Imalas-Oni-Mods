using HarmonyLib;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORS;

namespace DemoliorStoryTrait.Patches
{
	class MutatedWorldData_Patches
	{

		[HarmonyPatch(typeof(MutatedWorldData), MethodType.Constructor, [typeof(ProcGen.World), typeof(List<WorldTrait>), typeof(List<WorldTrait>)])]
		public class TargetType_TargetMethod_Patch
		{
			public static void Postfix(ProcGen.World world, List<WorldTrait> worldTraits, List<WorldTrait> storyTraits)
			{
				if(storyTraits.Any())
					foreach (var item in storyTraits)
					{
						SgtLogger.l("Story on "+world.filePath+"; "+ item.name);
					}

				if (storyTraits.Any(storyTrait => storyTrait.filePath == Stories_Patches.CGM_Impactor_Path))
				{
					world.AddSeasons(["LargeImpactor"]);
				}
			}
		}
	}
}

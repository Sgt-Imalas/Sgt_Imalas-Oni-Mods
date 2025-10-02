using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketPlatforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	internal class LaunchPadConfig_Patches
	{
		[HarmonyPatch(typeof(LaunchPadConfig), nameof(LaunchPadConfig.ConfigureBuildingTemplate))]
		public static class LaunchPadConfig_ConfigureBuildingTemplate_Patch
		{
			/// <summary>
			/// replace the chained building head tag with a custom tag to allow alternative rocket platforms
			/// </summary>
			/// <param name="go"></param>
			public static void Postfix(GameObject go)
			{
				go.GetComponent<KPrefabID>().AddTag(ModAssets.Tags.RocketPlatformTag);

				ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
				def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;

				//fixes landed rockets not being recognized on launchpads on saveload
				go.AddOrGet<LandedStateFixer>();
			}
		}
	}
}

using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketPlatforms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

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
			[HarmonyPriority(Priority.LowerThanNormal)] 
			public static void Postfix(GameObject go)
			{
				UnfuckMaterialDistributor(go);
				go.GetComponent<KPrefabID>().AddTag(ModAssets.Tags.RocketPlatformTag);
				ChainedBuilding.Def def = go.AddOrGetDef<ChainedBuilding.Def>();
				def.headBuildingTag = ModAssets.Tags.RocketPlatformTag;
				//fixes landed rockets not being recognized on launchpads on saveload
				go.AddOrGet<LandedStateFixer>();
			}
			static void UnfuckMaterialDistributor(GameObject go)
			{
				///RocketryCompanion does this fuckery where it just deletes the normal material distributor def from the launchpad and replaces it with one that crashes the game in several situations.
				///cant have that, put it back to normal
				if (go.TryGetComponent<StateMachineController>(out var smc))
				{
					bool removeThatCrasher = false;

					foreach (StateMachine.BaseDef localDef in smc.cmpdef.defs)
					{
						//SgtLogger.l("LaunchPadDef: " + localDef.GetStateMachineType().Name);
						if (localDef.GetStateMachineType().Name == "LaunchPadMaterialDistributorExt")
						{
							smc.cmpdef.defs.Remove(localDef);
							removeThatCrasher = true;
							break;
						}
					}
					if (removeThatCrasher)
						go.AddOrGetDef<LaunchPadMaterialDistributor.Def>();

				}
			}
		}
	}
}

using HarmonyLib;
using Rockets_TinyYetBig.Content.Defs.StarmapEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	class SelectTool_Patches
	{
		static GameObject RocketExhaustIndicator = null;


		[HarmonyPatch(typeof(SelectTool), nameof(SelectTool.Select))]
		public class SelectTool_Select_Patch
		{
			public static void Postfix(SelectTool __instance, KSelectable new_selected)
			{
				if (IsRocketEngine(new_selected))
				{
					AddOrCreateExhaustIndicator(new_selected.transform.position);
				}
				else
				{
					if (RocketExhaustIndicator != null)
					{
						RocketExhaustIndicator.SetActive(false);
					}
				}
			}
		}
		static bool IsRocketEngine(KSelectable selectable)
		{
			if (selectable == null)
				return false;

			if (selectable.TryGetComponent<RocketEngineCluster>(out _))
				return true;

			if (selectable.TryGetComponent<Building>(out var building) && IsRocketEngine(building.Def))
				return true;

			return false;
		}

		static Dictionary<BuildingDef, bool> CachedRocketEngines = new();
		static bool IsRocketEngine(BuildingDef def)
		{
			if (def == null)
				return false;
			if (CachedRocketEngines.TryGetValue(def, out bool isEngine))
				return isEngine;
			bool isEngineInComplete = def.BuildingComplete.TryGetComponent<RocketEngineCluster>(out _);
			CachedRocketEngines[def] = isEngineInComplete;
			return isEngineInComplete;
		}

		static void AddOrCreateExhaustIndicator(Vector3 worldPos)
		{
			if (RocketExhaustIndicator == null)
				RocketExhaustIndicator = GameUtil.KInstantiate(Assets.GetPrefab(RocketExhaustIndicatorConfig.ID), worldPos, Grid.SceneLayer.FXFront, gameLayer: LayerMask.NameToLayer("Place"));
			RocketExhaustIndicator.transform.SetPosition(worldPos);
			RocketExhaustIndicator.SetActive(true);
		}
	}
}

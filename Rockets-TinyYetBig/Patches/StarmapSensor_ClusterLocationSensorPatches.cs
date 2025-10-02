using HarmonyLib;
using Rockets_TinyYetBig.NonRocketBuildings;
using Rockets_TinyYetBig.SpaceStations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rockets_TinyYetBig.Patches
{
	/// <summary>
	/// Patches to prevent crash on starmap sensor position addition and sidescreen adjustments for it
	/// </summary>
	public class StarmapSensor_ClusterLocationSensorPatches
	{
		static readonly AxialI DisabledLocation = new (999, 999);

		[HarmonyPatch(typeof(LogicClusterLocationSensorConfig))]
		[HarmonyPatch(nameof(LogicClusterLocationSensorConfig.DoPostConfigureComplete))]
		public static class AddClusterSelector
		{
			public static void Postfix(GameObject go)
			{
				ClusterDestinationSelector destinationSelector = go.AddOrGet<ClusterDestinationSelector>();
				destinationSelector.assignable = true;
				destinationSelector.requireAsteroidDestination = false;
				destinationSelector.m_destination = (DisabledLocation);

			}
		}

		[HarmonyPatch(typeof(ClusterDestinationSelector))]
		[HarmonyPatch(nameof(ClusterDestinationSelector.OnPrefabInit))]
		public static class DefaultDisabled
		{
			public static void Postfix(ClusterDestinationSelector __instance)
			{
				if (__instance.TryGetComponent<LogicClusterLocationSensor>(out var sensor))
				{
					if (__instance.m_destination == new AxialI(0, 0))
						__instance.SetDestination(DisabledLocation);
				}
			}
		}

		[HarmonyPatch(typeof(LogicClusterLocationSensor))]
		[HarmonyPatch(nameof(LogicClusterLocationSensor.CheckCurrentLocationSelected))]
		public static class AdditionalCheck
		{
			public static bool Prefix(LogicClusterLocationSensor __instance, ref bool __result)
			{
				if (__instance.TryGetComponent<ClusterDestinationSelector>(out var selector))
				{
					if (selector.GetDestination() == selector.gameObject.GetMyWorldLocation() && selector.GetDestination() != DisabledLocation)
					{
						__result = true;
						return false;
					}
				}
				return true;
			}
		}

		[HarmonyPatch(typeof(ClusterDestinationSideScreen))]
		[HarmonyPatch(nameof(ClusterDestinationSideScreen.OnClickClearDestination))]
		public static class ProperClear
		{
			public static bool Prefix(ClusterDestinationSideScreen __instance)
			{
				if (__instance.targetSelector.TryGetComponent<LogicClusterLocationSensor>(out var logicSensor))
				{
					__instance.targetSelector.SetDestination(DisabledLocation);
					return false;
				}
				return true;
			}
		}


		[HarmonyPatch(typeof(ClusterGrid))]
		[HarmonyPatch(nameof(ClusterGrid.GetLocationDescription))]
		public static class POIs_in_RocketTargetSelector
		{
			public static bool Prefix(AxialI location, out Sprite sprite, out string label, out string sublabel)
			{
				BetterViewOfLocation.GetLocationDescriptionWithPOIs(location, out sprite, out label, out sublabel);
				return false;
			}
		}



		[HarmonyPatch(typeof(ClusterDestinationSideScreen))]
		[HarmonyPatch(nameof(ClusterDestinationSideScreen.Refresh))]
		public static class BetterViewOfLocation
		{
			public static void GetLocationDescriptionWithPOIs(AxialI location, out Sprite sprite, out string label, out string sublabel)
			{
				sublabel = string.Empty;
				ClusterGridEntity clusterGridEntity = null;
				List<ClusterGridEntity> visibleEntitiesAtCell = ClusterGrid.Instance.GetVisibleEntitiesAtCell(location);

				if (visibleEntitiesAtCell.Count > 0)
					clusterGridEntity = visibleEntitiesAtCell.Find((ClusterGridEntity x) => x.Layer == EntityLayer.Asteroid || x.Layer == EntityLayer.POI || x.TryGetComponent<SpaceStation>(out _));

				ClusterGridEntity visibleAsteroidAtAdjacentCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(location, EntityLayer.Asteroid);
				if (clusterGridEntity != null)
				{
					sprite = clusterGridEntity.Layer == EntityLayer.POI ?
						Def.GetUISpriteFromMultiObjectAnim(clusterGridEntity.AnimConfigs.First().animFile, clusterGridEntity.AnimConfigs.First().initialAnim)
						: clusterGridEntity.GetUISprite();

					label = clusterGridEntity.Name;
					if (clusterGridEntity.TryGetComponent<WorldContainer>(out var world))
						sublabel = Strings.Get(world.name);
				}
				else if (visibleAsteroidAtAdjacentCell != null)
				{
					sprite = visibleAsteroidAtAdjacentCell.GetUISprite();
					label = global::STRINGS.UI.SPACEDESTINATIONS.ORBIT.NAME_FMT.Replace("{Name}", visibleAsteroidAtAdjacentCell.Name);
					if (visibleAsteroidAtAdjacentCell.TryGetComponent<WorldContainer>(out var world))
						sublabel = Strings.Get(world.name);
				}
				else if (ClusterGrid.Instance.IsCellVisible(location))
				{
					sprite = Assets.GetSprite("hex_unknown");
					label = global::STRINGS.UI.SPACEDESTINATIONS.EMPTY_SPACE.NAME;
				}
				else
				{
					sprite = Assets.GetSprite("unknown_far");
					label = global::STRINGS.UI.SPACEDESTINATIONS.FOG_OF_WAR_SPACE.NAME;
				}
			}

			public static bool Prefix(ClusterDestinationSideScreen __instance)
			{
				if (__instance.targetSelector == null)
					return true;

				bool isClusterLocationSensor = __instance.targetSelector.TryGetComponent<LogicClusterLocationSensor>(out _);

				if (isClusterLocationSensor || __instance.targetSelector.TryGetComponent<POICapacitySensorSM>(out _))
				{
					//SgtLogger.l("replacing Icon");
					var selector = __instance.targetSelector;
					if (!(selector.GetDestination() == DisabledLocation))
					{
						GetLocationDescriptionWithPOIs(selector.GetDestination(), out var sprite, out var label, out _);
						__instance.destinationImage.sprite = sprite;
						__instance.destinationLabel.text = (isClusterLocationSensor ? (string)STRINGS.UI.CLUSTERLOCATIONSENSORADDON.TITLE : (string)global::STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE) + ": " + label;
						__instance.clearDestinationButton.isInteractable = true;
					}
					else
					{
						__instance.destinationImage.sprite = Assets.GetSprite((HashedString)"hex_unknown");
						__instance.destinationLabel.text = (isClusterLocationSensor ? (string)STRINGS.UI.CLUSTERLOCATIONSENSORADDON.TITLE : (string)global::STRINGS.UI.UISIDESCREENS.CLUSTERDESTINATIONSIDESCREEN.TITLE) + ": " + (string)global::STRINGS.UI.SPACEDESTINATIONS.NONE.NAME;
						__instance.clearDestinationButton.isInteractable = false;
					}
					__instance.launchPadDropDown.gameObject.SetActive(false);
					__instance.repeatButton.gameObject.SetActive(false);
					return false;
				}
				return true;
			}
		}
	}
}

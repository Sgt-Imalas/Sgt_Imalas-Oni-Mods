using _WorldGenStateCapture.WorldStateData.Starmap.SpacemapItems;
using HarmonyLib;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace _WorldGenStateCapture.WorldStateData.WorldPOIs
{
	internal class WorldPOI_OnSpawn_Patches
	{
		[HarmonyPatch(typeof(SpacecraftManager), "OnSpawn")]
		public static class GettingStarmapItems_Basegame
		{
			public static void Postfix(SpacecraftManager __instance)
			{
				if (DlcManager.IsExpansion1Active())
				{
					return;

				}
				__instance.destinations
					.ForEach((data) =>
					{
						ModAssets.baseStarmapItems.Add(new VanillaMap_Entry()
						{
							Id = data.type,
							// Name = data.GetDestinationType().Name,
							Distance = data.distance
						});
					});


			}
		}
		[HarmonyPatch(typeof(ClusterGridEntity), nameof(ClusterGridEntity.OnSpawn))]
		public static class GettingStarmapItems_Dlc
		{
			public static void Postfix(ClusterGridEntity __instance)
			{
				if (!DlcManager.IsExpansion1Active())
				{
					return;
				}
				string prefabid = __instance.PrefabID().ToString();
				if (__instance is AsteroidGridEntity)
				{
					prefabid = Path.GetFileName((__instance as AsteroidGridEntity).m_worldContainer.worldName);
				}

				//var harvestInfo = __instance.GetSMI<HarvestablePOIStates.Instance>();
				//float rechargeTime = -1, maxMass = -1;

				//if (harvestInfo != null)
				//{
				//    rechargeTime = harvestInfo.configuration.GetRechargeTime();
				//    maxMass = harvestInfo.configuration.GetMaxCapacity();
				//}

				ModAssets.dlcStarmapItems.Add(new HexMap_Entry()
				{
					Id = prefabid,
					// Name = locationEntity.Name,
					Q = __instance.Location.Q,
					R = __instance.Location.R,
				}); ;
			}
		}


		[HarmonyPatch(typeof(Geyser), "OnSpawn")]
		public static class AddGeyserToList
		{
			public static void Postfix(Geyser __instance)
			{
				GeyserConfigurator.GeyserInstanceConfiguration configuration = __instance.configuration;
				Vector3 position = __instance.transform.GetPosition();


				var myWorld = __instance.GetMyWorld();
				if (!ModAssets.currentGeysers.ContainsKey(myWorld))
					ModAssets.currentGeysers[myWorld] = new List<MapGeyser>();


				ModAssets.currentGeysers[myWorld].Add(new MapGeyser()
				{
					Id = configuration.geyserType.id,
					PosX = (int)position.x,
					PosY = (int)position.y,

					ActivePeriod_IdleTime = configuration.GetOffDuration(),
					ActivePeriod_EruptionTime = configuration.GetOnDuration(),
					DormancyCycles = configuration.GetYearOffDuration() / 600f,
					ActiveCycles = configuration.GetYearOnDuration() / 600f,
					EmitRate = configuration.GetEmitRate() * 1000f,
				});
			}
		}
		[HarmonyPatch(typeof(OilWellConfig), "OnSpawn")]
		public static class AddOilwellToList
		{
			public static void Postfix(GameObject inst)
			{
				Vector3 position = inst.transform.GetPosition();

				var myWorld = inst.GetMyWorld();
				if (!ModAssets.currentGeysers.ContainsKey(myWorld))
					ModAssets.currentGeysers[myWorld] = new List<MapGeyser>();


				ModAssets.currentGeysers[myWorld].Add(new MapGeyser()
				{
					Id = OilWellConfig.ID,
					PosX = (int)position.x,
					PosY = (int)position.y,

					ActivePeriod_IdleTime = 0,
					ActivePeriod_EruptionTime = 1,
					DormancyCycles = 0f,
					ActiveCycles = 1,
					EmitRate = 3333.33f,
				});
			}
		}
		[HarmonyPatch]
		public static class AddBuildingsToPOIList
		{
			[HarmonyPostfix]
			public static void Postfix(GameObject go)
			{
				go.TryGetComponent<KPrefabID>(out var kprefab);
				go.AddOrGet<POITracker>().targetId = kprefab.PrefabID().ToString();
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
				yield return typeof(HeadquartersConfig).GetMethod(name);


				yield return typeof(WarpConduitSenderConfig).GetMethod(name);
				yield return typeof(WarpConduitReceiverConfig).GetMethod(name);

				yield return typeof(MassiveHeatSinkConfig).GetMethod(name);
			}
		}
		[HarmonyPatch]
		public static class AddEntitiesToPOIList
		{
			[HarmonyPostfix]
			public static void Postfix(GameObject __result)
			{
				__result.TryGetComponent<KPrefabID>(out var kprefab);
				__result.AddOrGet<POITracker>().targetId = kprefab.PrefabID().ToString();
			}
			[HarmonyTargetMethods]
			internal static IEnumerable<MethodBase> TargetMethods()
			{
				const string name = nameof(IEntityConfig.CreatePrefab);
				yield return typeof(GeneShufflerConfig).GetMethod(name);

				yield return typeof(SapTreeConfig).GetMethod(name);

				yield return typeof(WarpPortalConfig).GetMethod(name);
				yield return typeof(WarpReceiverConfig).GetMethod(name);

			}
		}

	}
}

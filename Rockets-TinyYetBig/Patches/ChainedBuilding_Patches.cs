using HarmonyLib;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketPlatforms;
using Rockets_TinyYetBig.RocketFueling;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.RocketLoadingPatches
{
	class ChainedBuilding_Patches
	{
		[HarmonyPatch(typeof(ChainedBuilding.StatesInstance),nameof(ChainedBuilding.StatesInstance.CollectNeighbourToChain))]
		public static class ChainedBuilding_StatesInstance_CollectNeighbourToChain_Patch
		{
			/// <summary>
			/// Skip and replace original method for vertical port loaders to allow vertical connections
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="cell"></param>
			/// <param name="chain"></param>
			/// <param name="foundHead"></param>
			/// <param name="ignoredLink"></param>
			/// <returns></returns>
			public static bool Prefix(
				ChainedBuilding.StatesInstance __instance,
				int cell,
				ref HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain,
				ref bool foundHead,
				ChainedBuilding.StatesInstance ignoredLink = null)
			{
				GameObject otherGO = Grid.Objects[cell, (int)__instance.def.objectLayer];
				if (otherGO == null)
					return false;

				otherGO.TryGetComponent<KPrefabID>(out var component);

				var instancePosXY = Grid.CellToXY(Grid.PosToCell(__instance));
				var otherGOPosXY = Grid.CellToXY(Grid.PosToCell(otherGO));

				//SgtLogger.l($"Checking if {__instance.gameObject.name} is connected to {otherGO.name} ({cell}), Coordinates are: {instancePosXY.ToString()} and {otherGOPosXY}");


				if (__instance.def.headBuildingTag != ModAssets.Tags.RocketPlatformTag
					|| !component.HasTag(__instance.def.linkBuildingTag) && !component.HasTag(__instance.def.headBuildingTag)
					|| instancePosXY.Y != otherGOPosXY.Y && instancePosXY.X != otherGOPosXY.X
					)
					return false;



				if (instancePosXY.Y != otherGOPosXY.Y && (__instance.gameObject.TryGetComponent<VerticalPortAttachment>(out _) != otherGO.TryGetComponent<VerticalPortAttachment>(out _)))
				{
					return false;
				}

				otherGO.GetSMI<ChainedBuilding.StatesInstance>()?.CollectToChain(ref chain, ref foundHead, ignoredLink);
				return false;
			}
		}

		[HarmonyPatch(typeof(ChainedBuilding.StatesInstance), nameof(ChainedBuilding.StatesInstance.StartSM))]
		public class ChainedBuilding_StatesInstance_StartSM_Patch //constructor not directly patchable (?)
		{
			/// <summary>
			/// update the neighbor positions of vertical port attachments
			/// </summary>
			/// <param name="__instance"></param>
			public static void Prefix(ChainedBuilding.StatesInstance __instance)
			{
				__instance.Subscribe((int)GameHashes.ChainedNetworkChanged, (_) => StopWorkingOnDisconnect(__instance));

				if (__instance.gameObject.TryGetComponent<VerticalPortAttachment>(out var portAttachment))
				{
					if (portAttachment.CrossPiece)
					{
						if (!__instance.neighbourCheckCells.Contains(portAttachment.TopCell))
							__instance.neighbourCheckCells.Add(portAttachment.TopCell);
						if (!__instance.neighbourCheckCells.Contains(portAttachment.BottomCell))
							__instance.neighbourCheckCells.Add(portAttachment.BottomCell);
						
						SgtLogger.l($"added crosspiece vertical cells to chained building");
						foreach (var item in __instance.neighbourCheckCells)
						{
							SgtLogger.l("Cell in list: " + item);
						}
					}
					else
					{
						__instance.neighbourCheckCells = [portAttachment.TopCell, portAttachment.BottomCell];
						SgtLogger.l($"replaced neigbor cells with vertical cells in chained building");
						foreach (var item in __instance.neighbourCheckCells)
						{
							SgtLogger.l("Cell in list: " + item);
						}
					}
				}
			}
		}

		static void StopWorkingOnDisconnect(ChainedBuilding.StatesInstance instance)
		{
			var smi = instance.master.gameObject.GetSMI<ModularConduitPortController.Instance>();
			if (smi == null)
				return;
			if(instance.sm.isConnectedToHead.Get(instance) == false)
			{
				smi.SetRocket(false);
			}
		}

		[HarmonyPatch(typeof(ChainedBuilding.StatesInstance), nameof(ChainedBuilding.StatesInstance.OnCleanUp))]
		public class ChainedBuilding_StatesInstance_TargetMethod_Patch
		{
			public static void Postfix(ChainedBuilding.StatesInstance __instance)
			{
				__instance.Unsubscribe((int)GameHashes.ChainedNetworkChanged, (_) => StopWorkingOnDisconnect(__instance));
			}
		}
	}
}

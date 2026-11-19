using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static STRINGS.DUPLICANTS.ATTRIBUTES;

namespace Rockets_TinyYetBig.Patches
{
	internal class WorldContainer_Patches
	{

        [HarmonyPatch(typeof(WorldContainer), nameof(WorldContainer.TransferResourcesToDebris))]
		
        public class WorldContainer_TransferResourcesToDebris_Patch
        {
			/// <summary>
			/// simply delete interior debris when the container element is void
			/// this is used by derelict stations to skip spawning materials
			/// </summary>
			/// <param name="__instance"></param>
			/// <param name="debrisContainerElement"></param>
			/// <returns></returns>

            public static bool Prefix(WorldContainer __instance, SimHashes debrisContainerElement)
            {
                if (debrisContainerElement != SimHashes.Void)
                    return true;
				ListPool<ScenePartitionerEntry, FunctionScope>.PooledList pooledList = ListPool<ScenePartitionerEntry, FunctionScope>.Allocate();
				GameScenePartitioner.Instance.GatherEntries((int)__instance.minimumBounds.x, (int)__instance.minimumBounds.y, __instance.Width, __instance.Height, GameScenePartitioner.Instance.pickupablesLayer, pooledList);
				foreach (ScenePartitionerEntry item in pooledList)
				{
					if (item.obj == null)
					{
						continue;
					}

					Pickupable pickupable = item.obj as Pickupable;
					if (!(pickupable != null))
					{
						continue;
					}
					Util.KDestroyGameObject(pickupable.gameObject);
				}

				pooledList.Recycle();

				return false;
            }
        }
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisSnowThings.Content.Scripts.Buildings
{
	public class GlassCaseSealable : KMonoBehaviour
	{
		[MyCmpReq]
		private Building building;

		[MyCmpReq]
		private KSelectable kSelectable;

		[MyCmpReq]
		private KBatchedAnimController kbac;

		private Guid statusItem;

		public static Vector3 animOffset = new Vector3(0, 0.3f);

		public bool IsCased => glassCase != null;

		public GlassCase glassCase;

		public override void OnSpawn()
		{
			base.OnSpawn();
			CheckForCase();
			Subscribe((int)GameHashes.Rotated, OnRotated);
		}

		private void OnRotated(object obj)
		{
			// rotation resets offset
			kbac.Offset += animOffset;
		}

		private void CheckForCase()
		{
			var pooledList = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
			GameScenePartitioner.Instance.GatherEntries(building.GetExtents(), GameScenePartitioner.Instance.completeBuildings, pooledList);

			foreach (var scenePartitionerEntry in pooledList)
			{
				if (scenePartitionerEntry.obj is KMonoBehaviour kMonoBehaviour && kMonoBehaviour.TryGetComponent(out GlassCase glassCase))
				{
					Seal(glassCase);
					break;
				}
			}
		}

		public void Seal(GlassCase glassCase)
		{
			var handle = GameComps.StructureTemperatures.GetHandle(gameObject);
			if (handle.IsValid() && GameComps.StructureTemperatures.IsEnabled(handle))
			{
				GameComps.StructureTemperatures.Disable(handle);
			}

			statusItem = kSelectable.AddStatusItem(SnowStatusItems.sealedStatus, this);
			kbac.Offset += animOffset;
			kbac.SetDirty();

			this.glassCase = glassCase;

			Trigger(ModAssets.Hashes.Sealed, glassCase);
		}

		public void Unseal()
		{
			var handle = GameComps.StructureTemperatures.GetHandle(gameObject);
			if (handle.IsValid() && !GameComps.StructureTemperatures.IsEnabled(handle))
			{
				GameComps.StructureTemperatures.Enable(handle);
			}

			kSelectable.RemoveStatusItem(statusItem);
			kbac.Offset -= animOffset;

			Trigger(ModAssets.Hashes.UnSealed, glassCase);

			glassCase = null;
		}

		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}
	}
}

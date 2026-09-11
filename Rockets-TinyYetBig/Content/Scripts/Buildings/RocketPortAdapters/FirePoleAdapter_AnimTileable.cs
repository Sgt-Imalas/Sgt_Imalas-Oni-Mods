using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketPortAdapters
{
	internal class FirePoleAdapter_AnimTileable : AnimTileable
	{
		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] AnimTileable at;
		[MyCmpReq] KPrefabID kpid;


		public const string topLeftCap = "cap_top_left", topRightCap = "cap_top_right", capCentral = "cap_top";

		public HandleVector<int>.Handle partitionerEntry2;

		public override void OnSpawn()
		{
			base.OnSpawn();
			Extents extents = new Extents(this.extents.x - 1, this.extents.y - 1, this.extents.width + 2, this.extents.height + 2);
			partitionerEntry2 = GameScenePartitioner.Instance.Add("FirePoleAdapter_AnimTileable.OnSpawn", base.gameObject, extents, GameScenePartitioner.Instance.objectLayers[(int)objectLayer], OnNeighbourCellsUpdated2);
			UpdateEndCaps2();
		}
		public override void OnCleanUp()
		{
			GameScenePartitioner.Instance.Free(ref partitionerEntry2);
			base.OnCleanUp();
		}

		public void OnNeighbourCellsUpdated2(object data)
		{
			if (!(this == null) && !(base.gameObject == null) && partitionerEntry2.IsValid())
			{
				UpdateEndCaps2();
			}
		}
		public void UpdateEndCaps2()
		{

			bool ownCapOn = kbac.GetSymbolVisiblity(capCentral);

			if (!ownCapOn)
			{
				kbac.SetSymbolVisiblity(topLeftCap, false);
				kbac.SetSymbolVisiblity(topRightCap, false);
			}
			else
			{
				int cell = Grid.PosToCell(this);
				int left = Grid.CellLeft(cell);
				int right = Grid.CellRight(cell);

				GameObject leftGO = Grid.Objects[left, (int)at.objectLayer];
				GameObject rightGO = Grid.Objects[right, (int)at.objectLayer];

				bool hasLadderLeft = leftGO != null && leftGO.TryGetComponent<KPrefabID>(out var kprefLeft) && kprefLeft.PrefabID() == kpid.PrefabID();
				bool hasLadderRight = rightGO != null && rightGO.TryGetComponent<KPrefabID>(out var kprefRight) && kprefRight.PrefabID() == kpid.PrefabID();

				if (!hasLadderRight)
				{
					//SgtLogger.l("no ladder right, enabling cap");
					kbac.SetSymbolVisiblity(topRightCap, true);
				}
				else if (rightGO.TryGetComponent<KBatchedAnimController>(out var rightKbac))
				{
					bool rightCapOn = rightKbac.GetSymbolVisiblity(capCentral);
					//SgtLogger.l("rightCapOn: " + rightCapOn);
					kbac.SetSymbolVisiblity(topRightCap, !rightCapOn);
				}

				if (!hasLadderLeft)
				{
					//SgtLogger.l("no ladder left, enabling cap");
					kbac.SetSymbolVisiblity(topLeftCap, true);
				}
				else if (leftGO.TryGetComponent<KBatchedAnimController>(out var leftKbac))
				{
					bool leftCapOn = leftKbac.GetSymbolVisiblity(capCentral);
					//SgtLogger.l("leftCapOn: " + leftCapOn);
					kbac.SetSymbolVisiblity(topLeftCap, !leftCapOn);
				}
			}
			//SgtLogger.l($"LadderAnimTileable.UpdateEndCaps: {gameObject.name} {cell} {left} {right} {ladderLeft?.name} {ladderRight?.name} {ownCapOn}, left: {kbac.GetSymbolVisiblity(topLeftCap)}, right: {kbac.GetSymbolVisiblity(topRightCap)}");
		}
	}
}

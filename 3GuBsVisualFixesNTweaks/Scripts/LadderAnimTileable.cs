using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UtilLibs;

namespace _3GuBsVisualFixesNTweaks.Scripts
{
    class LadderAnimTileable:KMonoBehaviour 
    {
        [MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] AnimTileable at;


		public const string topLeftCap = "cap_top_left", topRightCap = "cap_top_right", capCentral = "cap_top";

		public HandleVector<int>.Handle partitionerEntry;

		public override void OnSpawn()
		{
			UpdateEndCaps();
		}

		public void UpdateEndCaps()
		{
			int cell = Grid.PosToCell(this);
			int left = Grid.CellLeft(cell);
			int right = Grid.CellRight(cell);

			GameObject ladderLeft = Grid.Objects[left, (int)at.objectLayer];
			GameObject ladderRight = Grid.Objects[right, (int)at.objectLayer];
			bool ownCapOn = kbac.GetSymbolVisiblity(capCentral);
			if (!ownCapOn)
			{
				kbac.SetSymbolVisiblity(topLeftCap, false);
				kbac.SetSymbolVisiblity(topRightCap, false);
			}
			else
			{
				if(ladderRight == null)
				{
					//SgtLogger.l("no ladder right, enabling cap");
					kbac.SetSymbolVisiblity(topRightCap, true);
				}
				else if(ladderRight.TryGetComponent<KBatchedAnimController>(out var rightKbac))
				{
					bool rightCapOn = rightKbac.GetSymbolVisiblity(capCentral);
					//SgtLogger.l("rightCapOn: " + rightCapOn);
					kbac.SetSymbolVisiblity(topRightCap, !rightKbac.GetSymbolVisiblity(capCentral));
				}

				if (ladderLeft == null)
				{
					//SgtLogger.l("no ladder left, enabling cap");
					kbac.SetSymbolVisiblity(topLeftCap, true);
				}
				else if (ladderLeft.TryGetComponent<KBatchedAnimController>(out var leftKbac))
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

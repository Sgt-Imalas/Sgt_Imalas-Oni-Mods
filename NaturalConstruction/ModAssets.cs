using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace NaturalConstruction
{
	internal class ModAssets
	{
		public static void MovePickupables(int cell)
		{
			List<int> possibleDisplacementTargets = [
				Grid.CellAbove(cell),
				Grid.CellBelow(cell),
				Grid.CellRight(cell),
				Grid.CellLeft(cell),
				Grid.CellUpRight(cell),
				Grid.CellDownRight(cell),
				Grid.CellUpLeft(cell),
				Grid.CellDownLeft(cell)
				];

			int target = -1;
			for (int i = 0; i < possibleDisplacementTargets.Count; i++)
			{
				var possibleDisplacement = possibleDisplacementTargets[i];

				if (!Grid.IsSolidCell(possibleDisplacement) && !Grid.Foundation[possibleDisplacement] && !Grid.HasDoor[possibleDisplacement])
				{
					target = possibleDisplacement;
					break;
				}

			}
			//no valid displacement cell found
			if (target == -1)
				return;

			var pos = Grid.CellToPosCCC(target, Grid.SceneLayer.Ore);

			var gameObject = Grid.Objects[cell, (int)ObjectLayer.Pickupables];
			if (gameObject != null)
			{
				// item in a linked list of all debris in layer 3/ObjectLayer.Pickupables
				var objectListNode = gameObject.GetComponent<Pickupable>().objectLayerListItem;
				while (objectListNode != null)
				{
					var content = objectListNode.gameObject;
					var pickupable = objectListNode.pickupable;
					objectListNode = objectListNode.nextItem;
					// Ignore Duplicants
					if (content != null && content.GetComponent<MinionIdentity>() == null)
					{
						content.transform.SetPosition(pos);
						pickupable.RemoveFaller();
						pickupable.AddFaller(Vector2.up);
					}
				}
			}
		}

	}
}

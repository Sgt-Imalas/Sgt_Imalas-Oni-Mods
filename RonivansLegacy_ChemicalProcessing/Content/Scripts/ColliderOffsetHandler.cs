using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class ColliderOffsetHandler : KMonoBehaviour
	{
		[MyCmpReq] KBoxCollider2D collider;
		[SerializeField]
		public int ColliderOffsetY = 0, ColliderOffsetX = 0;
		public override void OnSpawn()
		{
			base.OnSpawn();
			collider.offset = collider.offset + new Vector2(ColliderOffsetX, ColliderOffsetY);
		}

		internal static void GenerateBuildingDefOffsets(BuildingDef buildingDef, int yOffset, int xOffset)		
		{
			int width = buildingDef.WidthInCells;
			int height = buildingDef.HeightInCells;

			int num = width / 2 - width + 1;
			buildingDef.PlacementOffsets = new CellOffset[width * height];
			for (int i = 0; i != height; i++)
			{
				int num2 = i * width;
				for (int j = 0; j != width; j++)
				{
					int num3 = num2 + j;
					buildingDef.PlacementOffsets[num3].x = j + num +xOffset;
					buildingDef.PlacementOffsets[num3].y = i + yOffset;
				}
			}

		}
	}
}

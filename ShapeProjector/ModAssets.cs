using Klei.AI;
using KSerialization;
using ShapeProjector.Content.Scripts;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace ShapeProjector
{
	internal class ModAssets
	{
		public static GameObject ShapePickerSidescreenPrefab = null;
		internal static Dictionary<string, Sprite> SymbolMap = [];

		public const string
			Shape_Circle_ID = "shapeproject_circle",
			Shape_Rect_ID = "shapeproject_rect",
			Shape_Diamond_ID = "shapeproject_diamond",
			Shape_Hex_H_ID = "shapeproject_hex_horizontal",
			Shape_Hex_V_ID = "shapeproject_hex_vertical",
			Shape_Octagon = "shapeproject_octagon";


		public static void LoadAssets()
		{
			AssetBundle bundle = AssetUtils.LoadAssetBundle("shapeprojector_ui_assets", platformSpecific: true);
			ShapePickerSidescreenPrefab = bundle.LoadAsset<GameObject>("Assets/UIs/ShapeProjectionSideScreen.prefab");

			SgtLogger.Assert("ShapePickerSidescreenPrefab", ShapePickerSidescreenPrefab);

			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(ShapePickerSidescreenPrefab);
		}

		internal static Dictionary<string, Sprite> GetShapeIdWithSprite()
		{
			return SymbolMap;
		}

		public static void CalculateShape(int cell, string shapeId, ShapeProjectorLogic.ShapeData data, HashSet<int> cellsResult)
		{
			switch (shapeId)
			{
				case Shape_Circle_ID:
					CalculateCircle(cell, data, cellsResult);
					break;
				case Shape_Rect_ID:
					CalculateRect(cell, data, cellsResult);
					break;
				case Shape_Diamond_ID:
					CalculateDiamond(cell, data, cellsResult);
					break;
				case Shape_Hex_H_ID:
					CalculateHexagon(cell, data, cellsResult, flatTop: true);
					break;
				case Shape_Hex_V_ID:
					CalculateHexagon(cell, data, cellsResult, flatTop: false);
					break;
				case Shape_Octagon:
					CalculateOctagon(cell, data, cellsResult);
					break;
				default:
					SgtLogger.warning("Unknown shapeId: " + shapeId);
					break;
			}
		}
		internal static int GetLinkedHeight(string shapeId, int x)
		{
			return shapeId switch
			{
				Shape_Hex_H_ID => Mathf.RoundToInt(x * 0.86602540378f),// sqrt(3)/2
				Shape_Hex_V_ID => Mathf.RoundToInt(x * 1.15470053838f),// 2/sqrt(3)
				_ => x,
			};
		}

		static void CalculateRect(int cell, ShapeProjectorLogic.ShapeData data, HashSet<int> cellsResult)
		{
			if (!Grid.IsValidCell(cell))
				return;
			var origin = Grid.CellToXY(cell);
			int width = data.width;
			int height = data.height;
			if (width < 1 || height < 1)
				return;
			int worldId = Grid.WorldIdx[cell];
			int left = origin.x - width / 2;
			int bottom = origin.y - height / 2;

			for (int y = 0; y < height; y++)
			{
				int originOffset_y = bottom + y;
				if (originOffset_y < 0 || originOffset_y >= Grid.HeightInCells)
					continue;

				bool rowIsBorder = y == 0 || y == height - 1;

				for (int x = 0; x < width; x++)
				{
					if (!rowIsBorder && x != 0 && x != width - 1)
						continue;

					int originOffset_x = left + x;
					if (originOffset_x < 0 || originOffset_x >= Grid.WidthInCells)
						continue;

					int borderCell = Grid.XYToCell(originOffset_x, originOffset_y);
					if (Grid.IsValidCellInWorld(borderCell, worldId))
						cellsResult.Add(borderCell);
				}
			}
		}
		static void CalculateCircle(int cell, ShapeProjectorLogic.ShapeData data, HashSet<int> cellsResult)
		{
			if(!Grid.IsValidCell(cell))
				return;

			var origin = Grid.CellToXY(cell);

			int width = data.width;
			int height = data.height;
			bool thiccLines = data.isThicc;

			int worldId = Grid.WorldIdx[cell];

			if (width < 1 || height < 1)
				return;

			long w2h2 = (long)width * width * height * height;
			long w2 = (long)width * width;
			long h2 = (long)height * height;

			// cell centre in doubled coordinates relative to the ellipse centre:
			// X = 2x + 1 - width, Y = 2y + 1 - height
			// inside <=> X^2 * h^2 + Y^2 * w^2 <= w^2 * h^2
			bool InsideCircle(int x, int y)
			{
				long X = 2L * x + 1 - width;
				long Y = 2L * y + 1 - height;
				return X * X * h2 + Y * Y * w2 <= w2h2;
			}

			int left = origin.x - width / 2;
			int bottom = origin.y - height / 2;

			for (int y = 0; y < height; y++)
			{
				int originOffset_y = bottom + y;
				if (originOffset_y < 0 || originOffset_y >= Grid.HeightInCells)
					continue;

				for (int x = 0; x < width; x++)
				{
					int originOffset_x = left + x;
					if (originOffset_x < 0 || originOffset_x >= Grid.WidthInCells)
						continue;

					if (!InsideCircle(x, y))
						continue;

					bool isBorderCell = !InsideCircle(x + 1, y) || !InsideCircle(x - 1, y) || !InsideCircle(x, y + 1) || !InsideCircle(x, y - 1);

					// thick: diagonal neighbours count as well
					if (!isBorderCell && thiccLines)
						isBorderCell = !InsideCircle(x + 1, y + 1) || !InsideCircle(x - 1, y + 1) || !InsideCircle(x + 1, y - 1) || !InsideCircle(x - 1, y - 1);

					int borderCell = Grid.XYToCell(originOffset_x, originOffset_y);

					if (isBorderCell && Grid.IsValidCellInWorld(borderCell, worldId))
						cellsResult.Add(borderCell);
				}
			}
		}
		static void CalculateDiamond(int cell, ShapeProjectorLogic.ShapeData data, HashSet<int> cellsResult)
		{
			if (!Grid.IsValidCell(cell))
				return;

			var origin = Grid.CellToXY(cell);

			int width = data.width;
			int height = data.height;
			bool thiccLines = data.isThicc;

			if (width < 1 || height < 1)
				return;

			int worldId = Grid.WorldIdx[cell];

			long wh = (long)width * height;

			// cell centre in doubled coordinates relative to the diamond centre:
			// X = 2x + 1 - width, Y = 2y + 1 - height
			// inside <=> |X| * height + |Y| * width <= width * height
			bool InsideDiamond(int x, int y)
			{
				long X = 2L * x + 1 - width;
				long Y = 2L * y + 1 - height;
				return Math.Abs(X) * height + Math.Abs(Y) * width <= wh;
			}

			int left = origin.x - width / 2;
			int bottom = origin.y - height / 2;

			for (int y = 0; y < height; y++)
			{
				int originOffset_y = bottom + y;
				if (originOffset_y < 0 || originOffset_y >= Grid.HeightInCells)
					continue;

				for (int x = 0; x < width; x++)
				{
					int originOffset_x = left + x;
					if (originOffset_x < 0 || originOffset_x >= Grid.WidthInCells)
						continue;

					if (!InsideDiamond(x, y))
						continue;

					bool isBorderCell = !InsideDiamond(x + 1, y) || !InsideDiamond(x - 1, y) || !InsideDiamond(x, y + 1) || !InsideDiamond(x, y - 1);

					// thick: diagonal neighbours count as well
					if (!isBorderCell && thiccLines)
						isBorderCell = !InsideDiamond(x + 1, y + 1) || !InsideDiamond(x - 1, y + 1) || !InsideDiamond(x + 1, y - 1) || !InsideDiamond(x - 1, y - 1);

					int borderCell = Grid.XYToCell(originOffset_x, originOffset_y);

					if (isBorderCell && Grid.IsValidCellInWorld(borderCell, worldId))
						cellsResult.Add(borderCell);
				}
			}
		}
		static void CalculateHexagon(int cell, ShapeProjectorLogic.ShapeData data, HashSet<int> cellsResult, bool flatTop)
		{
			if (!Grid.IsValidCell(cell))
				return;

			var origin = Grid.CellToXY(cell);

			int width = data.width;
			int height = data.height;
			bool thiccLines = data.isThicc;

			int worldId = Grid.WorldIdx[cell];

			if (width < 1 || height < 1)
				return;

			long wh2 = 2L * width * height;

			// cell centre in doubled coordinates relative to the hexagon centre:
			// X = 2x + 1 - width, Y = 2y + 1 - height
			// flatTop:  vertices (+-width, 0), (+-width/2, +-height)
			// flatSide: vertices (0, +-height), (+-width, +-height/2)
			bool InsideHexagon(int x, int y)
			{
				long X = System.Math.Abs(2L * x + 1 - width);
				long Y = System.Math.Abs(2L * y + 1 - height);

				if (flatTop)
					return Y <= height && 2L * X * height + Y * width <= wh2;

				return X <= width && 2L * Y * width + X * height <= wh2;
			}

			int left = origin.x - width / 2;
			int bottom = origin.y - height / 2;

			for (int y = 0; y < height; y++)
			{
				int originOffset_y = bottom + y;
				if (originOffset_y < 0 || originOffset_y >= Grid.HeightInCells)
					continue;

				for (int x = 0; x < width; x++)
				{
					int originOffset_x = left + x;
					if (originOffset_x < 0 || originOffset_x >= Grid.WidthInCells)
						continue;

					if (!InsideHexagon(x, y))
						continue;

					bool isBorderCell = !InsideHexagon(x + 1, y) || !InsideHexagon(x - 1, y) || !InsideHexagon(x, y + 1) || !InsideHexagon(x, y - 1);

					// thick: diagonal neighbours count as well
					if (!isBorderCell && thiccLines)
						isBorderCell = !InsideHexagon(x + 1, y + 1) || !InsideHexagon(x - 1, y + 1) || !InsideHexagon(x + 1, y - 1) || !InsideHexagon(x - 1, y - 1);

					int borderCell = Grid.XYToCell(originOffset_x, originOffset_y);

					if (isBorderCell && Grid.IsValidCellInWorld(borderCell, worldId))
						cellsResult.Add(borderCell);
				}
			}
		}
		static void CalculateOctagon(int cell, ShapeProjectorLogic.ShapeData data, HashSet<int> cellsResult)
		{
			if (!Grid.IsValidCell(cell))
				return;

			var origin = Grid.CellToXY(cell);

			int width = data.width;
			int height = data.height;
			bool thiccLines = data.isThicc;

			int worldId = Grid.WorldIdx[cell];

			if (width < 1 || height < 1)
				return;

			// corner cut line: |X|/width + |Y|/height <= sqrt(2), sqrt(2) ~ 99/70
			// gives 45 degree corners on a square bounding box and a regular octagon
			const long CUT_NUM = 99;
			const long CUT_DEN = 70;

			long wh = (long)width * height;

			// cell centre in doubled coordinates relative to the octagon centre:
			// X = 2x + 1 - width, Y = 2y + 1 - height
			// inside <=> |X| <= width && |Y| <= height && (|X| * height + |Y| * width) * CUT_DEN <= CUT_NUM * width * height
			bool InsideOctagon(int x, int y)
			{
				long X = System.Math.Abs(2L * x + 1 - width);
				long Y = System.Math.Abs(2L * y + 1 - height);
				return X <= width && Y <= height && (X * height + Y * width) * CUT_DEN <= CUT_NUM * wh;
			}

			int left = origin.x - width / 2;
			int bottom = origin.y - height / 2;

			for (int y = 0; y < height; y++)
			{
				int originOffset_y = bottom + y;
				if (originOffset_y < 0 || originOffset_y >= Grid.HeightInCells)
					continue;

				for (int x = 0; x < width; x++)
				{
					int originOffset_x = left + x;
					if (originOffset_x < 0 || originOffset_x >= Grid.WidthInCells)
						continue;

					if (!InsideOctagon(x, y))
						continue;

					bool isBorderCell = !InsideOctagon(x + 1, y) || !InsideOctagon(x - 1, y) || !InsideOctagon(x, y + 1) || !InsideOctagon(x, y - 1);

					// thick: diagonal neighbours count as well
					if (!isBorderCell && thiccLines)
						isBorderCell = !InsideOctagon(x + 1, y + 1) || !InsideOctagon(x - 1, y + 1) || !InsideOctagon(x + 1, y - 1) || !InsideOctagon(x - 1, y - 1);

					int borderCell = Grid.XYToCell(originOffset_x, originOffset_y);

					if (isBorderCell && Grid.IsValidCellInWorld(borderCell, worldId))
						cellsResult.Add(borderCell);
				}
			}
		}

		
	}
}

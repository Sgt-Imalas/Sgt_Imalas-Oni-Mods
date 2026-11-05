using Klei.AI;
using KSerialization;
using ShapeProjector.Content.Scripts;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.DUPLICANTS.ATTRIBUTES;
using static UnityEngine.UI.Image;

namespace ShapeProjector
{
	internal static class ModAssets
	{
		public static GameObject ShapePickerSidescreenPrefab = null;
		internal static Dictionary<string, Sprite> ShapeSymbolMap = [];
		internal static Dictionary<string, Sprite> OriginSymbolMap = [];


		public const string
			Shape_Circle_ID = "shapeproject_circle",
			Shape_Rect_ID = "shapeproject_rect",
			Shape_Diamond_ID = "shapeproject_diamond",
			Shape_Hex_H_ID = "shapeproject_hex_horizontal",
			Shape_Hex_V_ID = "shapeproject_hex_vertical",
			Shape_Octagon = "shapeproject_octagon",
			Shape_Triangle_Eq_ID = "shapeproject_triangle_equilateral",
			Shape_Triangle_ID = "shapeproject_triangle_rightangle"
			;

		public const string
			Origin_Center = "shape_origin_center",
			Origin_BottomLeft = "shape_origin_bl",
			Origin_Left = "shape_origin_l",
			Origin_TopLeft = "shape_origin_tl",
			Origin_Top = "shape_origin_t",
			Origin_TopRight = "shape_origin_tr",
			Origin_Right = "shape_origin_r",
			Origin_BottomRight = "shape_origin_br",
			Origin_Bottom = "shape_origin_b";

		static readonly List<string> OriginIdsSorted = [
			Origin_Center,
			Origin_BottomLeft,
			Origin_Left,
			Origin_TopLeft,
			Origin_Top,
			Origin_TopRight,
			Origin_Right,
			Origin_BottomRight,
			Origin_Bottom
			];

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
			return ShapeSymbolMap;
		}
		internal static Dictionary<string, Sprite> GetOriginIdWithSprite()
		{
			return OriginSymbolMap;
		}

		public static int OriginIconSort(string x, string y)
		{
			int xIndex = OriginIdsSorted.IndexOf(x);
			int yIndex = OriginIdsSorted.IndexOf(y);

			if (xIndex >= 0 && yIndex >= 0)
				return xIndex.CompareTo(yIndex);

			if (xIndex >= 0)
				return -1;
			if (yIndex >= 0)
				return 1;
			return x.CompareTo(y);
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
					bool flatTop = true;
					if(data.rotationState == 1 || data.rotationState == 3)
						flatTop = false;
					CalculateHexagon(cell, data, cellsResult, flatTop);
					break;
				case Shape_Hex_V_ID:
					flatTop = false;
					if (data.rotationState == 1 || data.rotationState == 3)
						flatTop = true;
					CalculateHexagon(cell, data, cellsResult, flatTop);
					break;
				case Shape_Octagon:
					CalculateOctagon(cell, data, cellsResult);
					break;
				case Shape_Triangle_Eq_ID:
					CalculateTriangle(cell, data, cellsResult, data.rotationState);
					break;
				case Shape_Triangle_ID:
					bool flipX = data.rotationState == 2 || data.rotationState == 3;
					bool flipY = data.rotationState == 1 || data.rotationState == 2;
					CalculateRightTriangle(cell, data, cellsResult,flipX, flipY);
					break;
				default:
					SgtLogger.warning("Unknown shapeId: " + shapeId);
					break;
			}
		}
		internal static int GetLinkedHeight(ShapeProjectorLogic.ShapeData data, int x)
		{
			if (data.shapeId == Shape_Hex_H_ID)
			{
				if(data.rotationState % 2 == 0)
					return Mathf.RoundToInt(x * 0.86602540378f);// sqrt(3)/2
				else
					return Mathf.RoundToInt(x * 1.15470053838f);// 2/sqrt(3)
			}
			if (data.shapeId == Shape_Hex_V_ID)
			{
				if (data.rotationState % 2 != 0)
					return Mathf.RoundToInt(x * 0.86602540378f);// sqrt(3)/2
				else
					return Mathf.RoundToInt(x * 1.15470053838f);// 2/sqrt(3)
			}
			if (data.shapeId == Shape_Triangle_Eq_ID)
				return (data.rotationState % 2 == 0) ? Mathf.RoundToInt(x * 0.86602540378f) : Mathf.RoundToInt(x * 1.15470053838f);

			return x;
			//	return data.shapeId switch
			//{
			//	Shape_Hex_H_ID => Mathf.RoundToInt(x * 0.86602540378f),// sqrt(3)/2
			//	Shape_Hex_V_ID => Mathf.RoundToInt(x * 1.15470053838f),// 2/sqrt(3)
			//	Shape_Triangle_Eq_ID => Mathf.RoundToInt(x * 0.86602540378f),// sqrt(3)/2
			//	_ => x,
			//};
		}

		static void CalculateOffsetOrigin(Vector2I origin, ShapeProjectorLogic.ShapeData data, out int bottom, out int left)
		{
			switch (data.originId)
			{
				default:
				case Origin_Center:
					left = origin.x - data.width / 2;
					bottom = origin.y - data.height / 2;
					break;
				case Origin_BottomLeft:
					left = origin.x;
					bottom = origin.y;
					break;
				case Origin_Left:
					left = origin.x;
					bottom = origin.y - data.height / 2;
					break;
				case Origin_TopLeft:
					left = origin.x;
					bottom = origin.y - (data.height - 1);
					break;
				case Origin_Top:
					left = origin.x - data.width / 2;
					bottom = origin.y - (data.height - 1);
					break;
				case Origin_TopRight:
					left = origin.x - (data.width - 1);
					bottom = origin.y - (data.height - 1);
					break;
				case Origin_Right:
					left = origin.x - (data.width - 1);
					bottom = origin.y - data.height / 2;
					break;
				case Origin_BottomRight:
					left = origin.x - (data.width - 1);
					bottom = origin.y;
					break;
				case Origin_Bottom:
					left = origin.x - data.width / 2;
					bottom = origin.y;
					break;
			}
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

			CalculateOffsetOrigin(origin, data, out int bottom, out int left);

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
			if (!Grid.IsValidCell(cell))
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

			CalculateOffsetOrigin(origin, data, out int bottom, out int left);

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

			CalculateOffsetOrigin(origin, data, out int bottom, out int left);

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

			CalculateOffsetOrigin(origin, data, out int bottom, out int left);

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

			CalculateOffsetOrigin(origin, data, out int bottom, out int left);

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
		
		// right angle in the corner selected by flipX / flipY
		// flipX = false, flipY = false: right angle at bottom left
		static void CalculateRightTriangle(int cell, ShapeProjectorLogic.ShapeData data, HashSet<int> cellsResult, bool flipX = false, bool flipY = false)
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

			// legs run along the left and bottom edge of the bounding box, hypotenuse from (0, height) to (width, 0)
			// cell centre in doubled coordinates from the right angle corner: X = 2x + 1, Y = 2y + 1
			// inside <=> X * height + Y * width <= 2 * width * height
			bool InsideTriangle(int x, int y)
			{
				if (flipX)
					x = width - 1 - x;
				if (flipY)
					y = height - 1 - y;

				if (x < 0 || x >= width || y < 0 || y >= height)
					return false;

				return (2L * x + 1) * height + (2L * y + 1) * width <= wh2;
			}

			CalculateOffsetOrigin(origin, data, out int bottom, out int left);

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

					if (!InsideTriangle(x, y))
						continue;

					bool isBorderCell = !InsideTriangle(x + 1, y) || !InsideTriangle(x - 1, y) || !InsideTriangle(x, y + 1) || !InsideTriangle(x, y - 1);

					// thick: diagonal neighbours count as well
					if (!isBorderCell && thiccLines)
						isBorderCell = !InsideTriangle(x + 1, y + 1) || !InsideTriangle(x - 1, y + 1) || !InsideTriangle(x + 1, y - 1) || !InsideTriangle(x - 1, y - 1);

					int borderCell = Grid.XYToCell(originOffset_x, originOffset_y);

					if (isBorderCell && Grid.IsValidCellInWorld(borderCell, worldId))
						cellsResult.Add(borderCell);
				}
			}
		}

		static void CalculateTriangle(int cell, ShapeProjectorLogic.ShapeData data, HashSet<int> cellsResult, int apex = 0)
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

			apex = ((apex % 4) + 4) % 4;
			bool vertical = apex == 0 || apex == 2;

			// base length and depth (base to apex) in the rotated frame
			long baseLen = vertical ? width : height;
			long depth = vertical ? height : width;

			// cell centre in doubled coordinates relative to the box centre:
			// X = 2x + 1 - width, Y = 2y + 1 - height
			// rotated frame: a = distance from the axis through the apex, b = position along that axis (towards the apex)
			// the allowed |a| shrinks linearly from baseLen at the base (b = -depth) to 0 at the apex (b = depth)
			// inside <=> |a| * 2 * depth <= baseLen * (depth - b)
			bool InsideTriangle(int x, int y)
			{
				if (x < 0 || x >= width || y < 0 || y >= height)
					return false;

				long X = 2L * x + 1 - width;
				long Y = 2L * y + 1 - height;

				long a, b;
				switch (apex)
				{
					case 0: // top
						a = X;
						b = Y;
						break;
					case 1: // right
						a = Y;
						b = X;
						break;
					case 2: // bottom
						a = X;
						b = -Y;
						break;
					default: // 3, left
						a = Y;
						b = -X;
						break;
				}

				return System.Math.Abs(a) * 2L * depth <= baseLen * (depth - b);
			}

			CalculateOffsetOrigin(origin, data, out int bottom, out int left);

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

					if (!InsideTriangle(x, y))
						continue;

					bool isBorderCell = !InsideTriangle(x + 1, y) || !InsideTriangle(x - 1, y) || !InsideTriangle(x, y + 1) || !InsideTriangle(x, y - 1);

					// thick: diagonal neighbours count as well
					if (!isBorderCell && thiccLines)
						isBorderCell = !InsideTriangle(x + 1, y + 1) || !InsideTriangle(x - 1, y + 1) || !InsideTriangle(x + 1, y - 1) || !InsideTriangle(x - 1, y - 1);

					int borderCell = Grid.XYToCell(originOffset_x, originOffset_y);

					if (isBorderCell && Grid.IsValidCellInWorld(borderCell, worldId))
						cellsResult.Add(borderCell);
				}
			}
		}
	}
}

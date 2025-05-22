using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.Image;

namespace AkiTrueTiles_SkinSelectorAddon
{
	internal class ModAssets
	{
		public static class Tags
		{
			public static readonly Tag texturedTile = TagManager.Create("truetiles_texturedTile");
		}

		/// <summary>
		/// mirror structure from TrueTiles to avoid reflection
		/// </summary>
		public static int[] TrueTilesElementIdx = [-1];


		public static HashSet<int> DirtyCells = new HashSet<int>();

		static bool RefreshScheduled = false;

		public static void RemoveAll(int cell)
		{
			TT_Remove(cell);
		}
		public static void ScheduleCellRefresh(int cell)
		{
			if (Grid.IsValidCell(cell))
			{
				DirtyCells.Add(cell);
				if (!RefreshScheduled)
				{
					GameScheduler.Instance.ScheduleNextFrame("refresh dirty cells", obj => RefreshCells());
					RefreshScheduled = true;
				}
			}
		}
		private static void RefreshCells()
		{
			foreach (int cell in DirtyCells)
			{
				TileVisualizer.RefreshCell(cell, ObjectLayer.FoundationTile, ObjectLayer.ReplacementTile);
			}
			DirtyCells.Clear();
			RefreshScheduled = false;
		}


		public static void TT_Add(int cell, SimHashes element)
		{
			TrueTilesElementIdx[cell] = SimMessages.GetElementIndex(element);
		}
		public static void TT_Remove(int cell)
		{
			TrueTilesElementIdx[cell] = -1;
		}
		public static void TT_Initialize()
		{
			TrueTilesElementIdx = new int[Grid.CellCount];

			for (var i = 0; i < TrueTilesElementIdx.Length; i++)
				TrueTilesElementIdx[i] = -1;
		}

		#region sprite util
		static Dictionary<BuildingDef, Dictionary<SimHashes, Sprite>> AtlasSprites = new Dictionary<BuildingDef, Dictionary<SimHashes, Sprite>>();

		public static bool GetSpriteForTile(BuildingDef def, SimHashes element, out Sprite sprite)
		{
			sprite = null;
			if (AtlasSprites.TryGetValue(def, out var dic) && dic.TryGetValue(element, out sprite))
			{
				return true;
			}
			return sprite != null;
		}

		/// <summary>
		/// From Backwalls
		/// </summary>
		/// <param name="atlas"></param>
		/// <returns></returns>
		public static Sprite GetSpriteForAtlas(TextureAtlas atlas)
		{

			var uvBox = GetUVBox(atlas);

			var tex = atlas.texture;
			var y = 1f - Mathf.FloorToInt(uvBox.y);
			y = Mathf.Clamp01(y);
			y *= tex.height;
			y -= 208;
			y = Mathf.Clamp(y, 0, 1024 - 208);

			return Sprite.Create(tex, new Rect(0, y, 208, 208), Vector3.zero, 100);

		}
		private static Vector4 GetUVBox(TextureAtlas atlas)
		{
			var num3 = atlas.items[0].name.Length - 4 - 8;
			var startIndex = num3 - 1 - 8;
			var uvBox = Vector4.zero;

			for (var k = 0; k < atlas.items.Length; k++)
			{
				var item = atlas.items[k];

				var value = item.name.Substring(startIndex, 8);
				var requiredConnections = Convert.ToInt32(value, 2);

				if (requiredConnections == 0)
				{
					uvBox = item.uvBox;
					break;
				}
			}

			return uvBox;
		}

		internal static void RegisterSprite(BuildingDef def, Texture2D mainTex, SimHashes element)
		{
			var defaultAtlas = Assets.GetTextureAtlas("tiles_solid");
			var atlas = CreateAtlas(defaultAtlas, mainTex);
			if(!AtlasSprites.ContainsKey(def))
				AtlasSprites[def] = new Dictionary<SimHashes, Sprite>();
			AtlasSprites[def][element] = GetSpriteForAtlas(atlas);
		}
		private static TextureAtlas CreateAtlas(TextureAtlas original, Texture2D texture)
		{
			var atlas = ScriptableObject.CreateInstance<TextureAtlas>();
			atlas.texture = texture;
			atlas.scaleFactor = original.scaleFactor;
			atlas.items = original.items;

			return atlas;
		}
		#endregion
	}
}

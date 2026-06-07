using BlueprintsV2.BlueprintData;
using Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static Rendering.BlockTileRenderer.RenderInfo;
using static STRINGS.DUPLICANTS.ATTRIBUTES;

namespace BlueprintsV2.BlueprintsV2.UnityUI.Components.PreviewVisualizers
{
	internal class Vis_TilePreview : KMonoBehaviour
	{
		string buildingDefId;
		//[MyCmpGet]
		//SpriteRenderer TilespriteRenderer;
		Image TilespriteRenderer;

		static Dictionary<BuildingDef, BlockTileRenderer.RenderInfo> _tileInfos = [];
		static Dictionary<BuildingDef, Dictionary<int, Sprite>> _Tilesprites = [];

		static string[,] Tiles = null;
		static List<Vis_TilePreview> previews = new List<Vis_TilePreview>();
		BuildingConfig _config;
		RectMask2D _mask;

		internal void Init(BuildingConfig building)
		{
			TilespriteRenderer = transform.Find("TileMask/TileVis").gameObject.GetComponent<Image>();
			_mask = transform.Find("TileMask").GetComponent<RectMask2D>();
			TilespriteRenderer.gameObject.SetActive(true);

			_config = building;
			var offset = building.Offset;
			Tiles[offset.X, offset.Y] = building.BuildingDefId;
			previews.Add(this);
		}

		internal static void ClearTileArray(Vector2I newDimensions)
		{
			Tiles = new string[newDimensions.X, newDimensions.Y];
			previews.Clear();
		}

		//runs after all tiles have been initialized via .Init()
		internal static void ConnectAll()
		{
			foreach (var preview in previews)
			{
				var offset = preview._config.Offset;
				var def = preview._config.BuildingDef;
				preview.UpdateTileTexture(def, offset);
			}
		}

		void UpdateTileTexture(BuildingDef def, Vector2I position)
		{
			BlockTileRenderer.Bits connection_bits = GetConnectionBits(position.X, position.Y);
			int variantInt = (int)connection_bits;

			if (!_Tilesprites.TryGetValue(def, out var spriteDict) || !spriteDict.ContainsKey(variantInt))
			{
				if (!_tileInfos.TryGetValue(def, out var renderInfo))
				{
					renderInfo = _tileInfos[def] = new BlockTileRenderer.RenderInfo(World.Instance.blockTileRenderer, (int)def.TileLayer, LayerMask.NameToLayer("Place"), def, SimHashes.COMPOSITION); //using composition here to always get the default look, even with true tiles
				}
				//SgtLogger.l("Trying to get tile variant for " + def.Name + " with variant " + GetConnectionBits(position.X, position.Y));
				var tex = renderInfo.material.mainTexture as Texture2D;

				Vector4 uv = renderInfo.atlasInfo.First().uvBox; //do AddVertexInfo trimming for other tile variants

				for (int index = 0; index < renderInfo.atlasInfo.Length; index++)
				{
					var info = renderInfo.atlasInfo[index];
					bool requiredConnectionsFulfilled = (connection_bits & info.requiredConnections) == info.requiredConnections;
					bool forbidddenConnectionsTriggered = (connection_bits & info.forbiddenConnections) != 0;
					if (requiredConnectionsFulfilled && !forbidddenConnectionsTriggered)
					{
						uv = info.uvBox;
						//SgtLogger.l("Uv box for " + info.name + ": " + info.uvBox.ToString() + ", required: " + info.requiredConnections+"; forbidden: "+info.forbiddenConnections);
						break;
					}
				}
				float uMin = uv.x;
				float vMin = uv.y;
				float uMax = uv.z;
				float vMax = uv.w;

				UnityEngine.Rect rect = new UnityEngine.Rect(
					uMin * tex.width,
					vMin * tex.height,
					(uMax - uMin) * tex.width,
					(vMax - vMin) * tex.height
				);

				if (spriteDict == null)
					spriteDict = new();
				spriteDict[variantInt] = Sprite.Create(tex, rect, new(0.5f, 0.5f), 128); // 128 ppu ;

				bool connectedLeft = (connection_bits & BlockTileRenderer.Bits.Left) != 0;
				bool connectedRight = (connection_bits & BlockTileRenderer.Bits.Right) != 0;
				bool connectedTop = (connection_bits & BlockTileRenderer.Bits.Up) != 0;
				bool connectedBottom = (connection_bits & BlockTileRenderer.Bits.Down) != 0;

				var padding = _mask.padding;
				padding.x = connectedLeft ? -1 : -50;
				padding.y = connectedTop ? -1 : -50;
				padding.z = connectedRight  ? -1 : -50;
				padding.w = connectedBottom ? -1 : -50;

				_mask.padding = padding;
			}



			TilespriteRenderer.sprite = spriteDict[variantInt];
		}
		public virtual BlockTileRenderer.Bits GetConnectionBits(int x, int y)
		{
			BlockTileRenderer.Bits connectionBits = (BlockTileRenderer.Bits)0;
			var tileID = Tiles[x, y];
			int width = Tiles.GetLength(0) - 1;
			int height = Tiles.GetLength(1) - 1;
			if (y > 0)
			{
				if (Tiles[x, y - 1] == tileID)
					connectionBits |= BlockTileRenderer.Bits.Up; //idk why this is the correct way, but im not attempting to understand it
			}
			if (x > 0 && Tiles[x - 1, y] == tileID)
				connectionBits |= BlockTileRenderer.Bits.Left;
			if (x < width && Tiles[x + 1, y] == tileID)
				connectionBits |= BlockTileRenderer.Bits.Right;
			if (y < height)
			{
				if (Tiles[x, y + 1] == tileID)
					connectionBits |= BlockTileRenderer.Bits.Down;
			}
			return connectionBits;
		}
	}
}

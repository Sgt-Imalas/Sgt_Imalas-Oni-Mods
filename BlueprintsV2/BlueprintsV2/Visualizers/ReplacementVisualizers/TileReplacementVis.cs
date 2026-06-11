using Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.Visualizers.ReplacementVisualizers
{
	internal class TileReplacementVis : ReplacementVis
	{
		[MyCmpGet] KBoxCollider2D collider;
		static Dictionary<BuildingDef, BlockTileRenderer.RenderInfo> _tileInfos = [];
		static Dictionary<BuildingDef, Dictionary<int, Sprite>> _Tilesprites = [];
		protected override void UpdateVisualState()
		{
			base.UpdateVisualState();
			UpdateTileTexture(def);
		}

		void UpdateTileTexture(BuildingDef def)
		{
			SgtLogger.Assert(tileSpriteRenderer, "tileSpriteRenderer");

			BlockTileRenderer.Bits connection_bits = GetConnectionBits(def, cell, (int)def.ObjectLayer);
			int variantInt = (int)connection_bits;

			if (!_Tilesprites.TryGetValue(def, out var spriteDict))
				_Tilesprites[def] = spriteDict = new Dictionary<int, Sprite>();

			if (!spriteDict.ContainsKey(variantInt))
			{
				if (!_tileInfos.TryGetValue(def, out var renderInfo))
				{
					renderInfo = _tileInfos[def] = new BlockTileRenderer.RenderInfo(World.Instance.blockTileRenderer, (int)def.TileLayer, LayerMask.NameToLayer("Place"), def, SimHashes.Void, false);
				}
				//SgtLogger.l("Trying to get tile variant for " + def.Name + " with variant " + connection_bits);
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

				spriteDict[variantInt] = Sprite.Create(tex, rect, new(0.5f, 0.5f), 128); // 128 ppu ;

				bool connectedLeft = (connection_bits & BlockTileRenderer.Bits.Left) != 0;
				bool connectedRight = (connection_bits & BlockTileRenderer.Bits.Right) != 0;
				bool connectedTop = (connection_bits & BlockTileRenderer.Bits.Up) != 0;
				bool connectedBottom = (connection_bits & BlockTileRenderer.Bits.Down) != 0;

				//var padding = _mask.padding;
				//padding.x = connectedLeft ? -1 : -50;
				//padding.y = connectedTop ? -1 : -50;
				//padding.z = connectedRight ? -1 : -50;
				//padding.w = connectedBottom ? -1 : -50;

				//_mask.padding = padding;
			}
			tileSpriteRenderer.sprite = spriteDict[variantInt];
			transform.SetPosition(Grid.CellToPosCCC(cell, Grid.SceneLayer.FXFront));
			collider.offset = Vector2.zero;
		}
	}
}

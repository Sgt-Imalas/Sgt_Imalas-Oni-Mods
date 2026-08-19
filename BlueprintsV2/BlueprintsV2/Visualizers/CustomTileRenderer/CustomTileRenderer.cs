using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration;
using BlueprintsV2.Visualizers;
using HarmonyLib;
using PeterHan.PLib;
using Rendering;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UtilLibs;
using static HarmonyLib.Code;

namespace BlueprintsV2.BlueprintsV2.Visualizers.CustomTileRenderer
{
	internal class CustomTileRenderer : BlockTileRenderer
	{
		static int 
			//OnPlayerJoin = -1,
			OnPlayerLeave = -1, OnPlayerCursorMade = -1;
		static readonly Dictionary<ulong, CustomTileRenderer> customTileRenderers = [];
		/// <summary>
		/// update id for this specific player
		/// </summary>
		public ulong PlayerId = BlueprintState.PlayerId_DefaultTilePreviews;

		static void OnPlayerLeft(object boxedId)
		{
			if (boxedId is not Boxed<ulong> b)
			{
				SgtLogger.warning(boxedId + " was not as expected an ulong, instead it was " + boxedId.GetType());
				return;
			}

			ulong playerId = Boxed<ulong>.Unbox(boxedId);
			SgtLogger.l("Player Left: " + playerId);

			if (!customTileRenderers.TryGetValue(playerId, out var renderer))
			{
				SgtLogger.warning($"player {playerId} left but had no tile renderer!");
				return;
			}
			renderer.FreeResources();
			customTileRenderers.Remove(playerId);
			BlueprintState.RemoveCachesForPlayer(playerId);
			TileVisual.OnPlayerRemoved(playerId);
		}
		//static void OnPlayerCursorCreated(object boxedId)
		//{
		//	if (boxedId is not Boxed<ulong> b)
		//	{
		//		SgtLogger.warning(boxedId + " was not as expected an ulong, instead it was " + boxedId.GetType());
		//		return;
		//	}

		//	ulong playerId = Boxed<ulong>.Unbox(boxedId);
		//	SgtLogger.l("Player Cursor created: " + playerId);
		//	BlueprintState.CachePlayerColor(playerId);
		//}

		/// <summary>
		/// Using the create cursor event as that triggers for all types of players for all others, regardless if its client or host
		/// </summary>
		/// <param name="boxedId"></param>
		static void OnPlayerJoined(object boxedId)
		{
			if (boxedId is not Boxed<ulong> b)
			{
				SgtLogger.warning(boxedId + " was not as expected an ulong, instead it was " + boxedId.GetType());
				return;
			}

			ulong playerId = Boxed<ulong>.Unbox(boxedId);
			SgtLogger.l("Player Joined: " + playerId);

			if (customTileRenderers.TryGetValue(playerId, out var renderer))
			{
				SgtLogger.warning($"player {playerId} joined but had a tile renderer already cached!");
				return;
			}
			renderer = World.Instance.gameObject.AddComponent<CustomTileRenderer>();
			renderer.PlayerId = playerId;
			customTileRenderers.Add(playerId, renderer);
			BlueprintState.AddCachesForPlayer(playerId); 
			BlueprintState.CachePlayerColor(playerId);
			TileVisual.OnPlayerAdded(playerId);
		}

		[HarmonyPatch(typeof(World), nameof(World.OnPrefabInit))]
		public class World_OnPrefabInit_Patch
		{
			public static void Postfix(World __instance)
			{
				SgtLogger.l("World.OnPrefabInit");
				customTileRenderers[BlueprintState.PlayerId_DefaultTilePreviews] = __instance.gameObject.AddComponent<CustomTileRenderer>();

				var replacementRenderer = __instance.gameObject.AddComponent<CustomTileRenderer>();
				replacementRenderer.PlayerId = BlueprintState.PlayerId_ReplacementTiles;
				customTileRenderers[BlueprintState.PlayerId_ReplacementTiles] = replacementRenderer;
			}
		}

		[HarmonyPatch(typeof(Game), nameof(Game.OnPrefabInit))]
		public class Game_OnPrefabInit_Patch
		{
			public static void Postfix()
			{
				//OnPlayerJoin = Game.Instance.Subscribe(MP_Mod_Hashes.OnPlayerJoined, OnPlayerJoined);
				OnPlayerLeave = Game.Instance.Subscribe(MP_Mod_Hashes.OnPlayerLeft, OnPlayerLeft);
				OnPlayerCursorMade = Game.Instance.Subscribe(MP_Mod_Hashes.OnPlayerCursorCreated, OnPlayerJoined);
			}
		}

		[HarmonyPatch(typeof(World), nameof(World.OnLoadLevel))]
		public class World_OnLoadLevel_Patch
		{
			public static void Postfix(World __instance)
			{
				//if (OnPlayerJoin != -1)
				//{
				//	Game.Instance?.Unsubscribe(OnPlayerJoin);
				//	OnPlayerJoin = -1;
				//}
				if (OnPlayerLeave != -1)
				{
					Game.Instance?.Unsubscribe(OnPlayerLeave);
					OnPlayerLeave = -1;
				}
				if (OnPlayerCursorMade != -1)
				{
					Game.Instance?.Unsubscribe(OnPlayerCursorMade);
					OnPlayerCursorMade = -1;
				}

				foreach (var r in customTileRenderers.Values)
					r.FreeResources();
				customTileRenderers.Clear();
				SgtLogger.l("World.OnLoadLevel");
			}
		}

		public static void RefreshCellInternal(ulong playerId, int cell, ObjectLayer tile_layer)
		{
			if (Game.IsQuitting() || !Grid.IsValidCell(cell))
			{
				return;
			}
			if (!customTileRenderers.TryGetValue(playerId, out var r))
				return;
			r.Rebuild(tile_layer, cell);

			GameObject gameObject = Grid.Objects[cell, (int)tile_layer];
			if (gameObject != null)
			{
				KAnimGraphTileVisualizer componentInChildren = gameObject.GetComponentInChildren<KAnimGraphTileVisualizer>();
				if (componentInChildren != null)
				{
					componentInChildren.Refresh();
				}
			}
		}

		public static void RefreshCell(ulong playerId, int cell, ObjectLayer tile_layer)
		{
			if (tile_layer != ObjectLayer.NumLayers)
			{
				RefreshCellInternal(playerId, cell, tile_layer);
				RefreshCellInternal(playerId, Grid.CellAbove(cell), tile_layer);
				RefreshCellInternal(playerId, Grid.CellBelow(cell), tile_layer);
				RefreshCellInternal(playerId, Grid.CellLeft(cell), tile_layer);
				RefreshCellInternal(playerId, Grid.CellRight(cell), tile_layer);
			}
		}

		public static void RefreshCell(ulong playerId, int cell, ObjectLayer tile_layer, ObjectLayer replacement_layer)
		{
			RefreshCell(playerId, cell, tile_layer);
			RefreshCell(playerId, cell, replacement_layer);
		}



		public static void AddTileBlock(ulong playerId, int renderLayer, BuildingDef def, bool isReplacement, SimHashes element, int cell, bool isBlueprint = true)
		{
			if (customTileRenderers.TryGetValue(playerId, out var r))
			{
				r.AddBlock(renderLayer, def, isReplacement, element, cell, isBlueprint);
			}
			else
			{
				SgtLogger.warning("Tried adding tile block for " + playerId + ", but there was no valid tile renderer for it!");
			}
		}
		public static void RemoveTileBlock(ulong playerId, BuildingDef def, bool isReplacement, SimHashes element, int cell)
		{
			if (customTileRenderers.TryGetValue(playerId, out var r))
			{
				r.RemoveBlock(def, isReplacement, element, cell);
			}
			else
			{
				SgtLogger.warning("Tried removing tile block for " + playerId + ", but there was no valid tile renderer for it!");
			}
		}

		public void GetCachedCellColor(ref Color current, int cell, SimHashes element)
		{
			if (BlueprintState.ColoredCells.TryGetValue(PlayerId, out var cache) && cache.TryGetValue(cell, out var data))
			{
				current = data;
			}
		}

		public override Bits GetConnectionBits(int x, int y, int query_layer)
		{
			///dont connect to existing things, looks weird
			//var realTileBits = base.GetConnectionBits(x, y, query_layer);
			//realTileBits |= GetVisualizerConnectionBits(x, y, query_layer);
			//return realTileBits;
			return GetVisualizerConnectionBits(x, y, query_layer);
		}
		public override Bits GetDecorConnectionBits(int x, int y, int query_layer)
		{
			///dont connect to existing things, looks weird
			//var realTileBits = base.GetDecorConnectionBits(x, y, query_layer);
			//realTileBits |= GetVisualizerConnectionBits(x, y, query_layer);
			return GetVisualizerConnectionBits(x, y, query_layer);
		}
		bool MatchesDefVis(int cell, BuildingDef def)
		{
			if (!TileVisual.HasTileAt(PlayerId, cell, out var vis))
				return false;
			return vis == def;
		}

		public virtual Bits GetVisualizerConnectionBits(int x, int y, int query_layer)
		{
			Bits bits = (Bits)0;
			int cell = y * Grid.WidthInCells + x;
			if (!TileVisual.HasTileAt(PlayerId, cell, out var def))
				return bits;

			if (y > 0)
			{
				int cellDown = (y - 1) * Grid.WidthInCells + x;
				if (x > 0 && MatchesDefVis(cellDown - 1, def))
				{
					bits |= Bits.DownLeft;
				}

				if (MatchesDefVis(cellDown, def))
				{
					bits |= Bits.Down;
				}

				if (x < Grid.WidthInCells - 1 && MatchesDefVis(cellDown + 1, def))
				{
					bits |= Bits.DownRight;
				}
			}

			if (x > 0 && MatchesDefVis(cell - 1, def))
			{
				bits |= Bits.Left;
			}

			if (x < Grid.WidthInCells - 1 && MatchesDefVis(cell + 1, def))
			{
				bits |= Bits.Right;
			}

			if (y < Grid.HeightInCells - 1)
			{
				int cellAbove = (y + 1) * Grid.WidthInCells + x;
				if (x > 0 && MatchesDefVis(cellAbove - 1, def))
				{
					bits |= Bits.UpLeft;
				}

				if (MatchesDefVis(cellAbove, def))
				{
					bits |= Bits.Up;
				}

				if (x < Grid.WidthInCells + 1 && MatchesDefVis(cellAbove + 1, def))
				{
					bits |= Bits.UpRight;
				}
			}

			return bits;
		}
	}
}

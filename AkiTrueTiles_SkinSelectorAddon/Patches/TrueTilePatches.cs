using HarmonyLib;
using Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rendering.BlockTileRenderer;

namespace AkiTrueTiles_SkinSelectorAddon.Patches
{
    class TrueTilesPatches
	{
		public static void PatchAll(Harmony harmony)
		{
			TrueTiles_BlockTileRendererPatch_Patch.ExecutePatch(harmony);	
			TileAssets_Add_Patch.ExecutePatch(harmony);
		}

		/// <summary>
		/// Reevaluate the tile connections to include the override skins
		/// </summary>
		public class TrueTiles_BlockTileRendererPatch_Patch
		{
			public static int lastCheckedCell = -1;
			//mirrored from TrueTiles
			// > save last looked at tile for connection				
			// > this is called by the GetConnectionBits, which i always call the above transpiler right after
			[HarmonyPatch(typeof(BlockTileRenderer), "MatchesDef")]
			public class BlockTileRenderer_MatchesDef_Patch
			{
				public static void Prefix(GameObject go, BuildingDef def, ref bool __result)
				{
					if (go == null)
					{
						lastCheckedCell = -1;
						return;
					}

					lastCheckedCell = Grid.PosToCell(go);
				}
			}
			public static void ExecutePatch(Harmony harmony)
			{
				var m_TargetType = AccessTools.TypeByName("TrueTiles.Patches.BlockTileRendererPatch+BlockTileRenderer_GetConnectionBits_Patch");

				var prefix = AccessTools.Method(typeof(TrueTiles_BlockTileRendererPatch_Patch), "Postfix");
				if (m_TargetType != null)
				{
					var targetMethod = AccessTools.Method(m_TargetType, "MatchesElement", [typeof(bool), typeof(int), typeof(int), typeof(int)]);
					if (targetMethod == null)
					{
						SgtLogger.warning("Could not find RenderInfo_Ctor_Patch.MatchesElement!");
						return;
					}
					harmony.Patch(targetMethod, null,new HarmonyMethod(prefix));
				}
				else
				{
					SgtLogger.l("TrueTiles.Patches.RenderInfoPatch.RenderInfo_Ctor_Patch type not found.");
				}
			}
			public static void Postfix(bool matchesDef, int x, int y, int layer, ref bool __result)
			{
				if (!matchesDef || lastCheckedCell == -1)
					return;

				if (layer == (int)ObjectLayer.ReplacementTile)
					return;

				var cell = Grid.XYToCell(x, y);

				if(TrueTiles_OverrideStorage.TryGetElement(cell, out var element) 
				&& TrueTiles_OverrideStorage.TryGetElement(lastCheckedCell, out var secondElement))
				{
					bool equalElement = element == secondElement;
					if (equalElement != __result)
					{
						__result = equalElement;
					}
				}
			}
		}

		/// <summary>
		/// grabbing all the ui sprites from the individual true tiles tile sheets
		/// </summary>
		public class TileAssets_Add_Patch
		{
			public static void ExecutePatch(Harmony harmony)
			{
				var t_TileAssets = Type.GetType("TrueTiles.Cmps.TileAssets, TrueTiles", false, false);
				if (t_TileAssets != null)
				{
					var m_Add = t_TileAssets.GetMethod("Add");
					var postfix = typeof(TileAssets_Add_Patch).GetMethod("Postfix");
					harmony.Patch(m_Add, null, new HarmonyMethod(postfix));
				}
			}

			public static void Postfix(string def, SimHashes material, object asset)
			{
				if (asset != null)
				{
					var traverse = Traverse.Create(asset);
					var mainTex = traverse.Field<Texture2D>("main").Value;
					var specularTex = traverse.Field<Texture2D>("specular").Value;
					var buildingDef = Assets.GetBuildingDef(def);

					if (def == null || mainTex == null)
					{
						return;
					}
					ModAssets.RegisterSprite(buildingDef, mainTex,material);

				}
			}
		}
	}
}

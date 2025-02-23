using ElementUtilNamespace;
using HarmonyLib;
using ONITwitchLib.Logger;
using Rendering.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static STRINGS.UI.DETAILTABS;

namespace Rockets_TinyYetBig.RocketFueling
{
	internal class WallAdapter_TrueTilesHandler : KMonoBehaviour
	{
		[MyCmpReq]
		KBatchedAnimController kbac;
		[MyCmpReq]
		SymbolOverrideController soc;
		[MyCmpGet]
		BuildingComplete BuildingComplete;

		SpriteRenderer TrueTileWall_Bottom, TrueTileWall_Top;

		public override void OnSpawn()
		{
			base.OnSpawn();
			HandleTrueTilesSymbolOverrides();

			Subscribe((int)GameHashes.SelectObject, OnSelectionChanged);
			Subscribe((int)GameHashes.HighlightObject, OnHighlightChanged);
			//ApplyBiomeTint();
		}
		public override void OnCleanUp()
		{
			Unsubscribe((int)GameHashes.SelectObject, OnSelectionChanged);
			Unsubscribe((int)GameHashes.HighlightObject, OnHighlightChanged);
			base.OnCleanUp();
			if (TrueTileWall_Bottom != null && TrueTileWall_Bottom.gameObject)
			{
				Destroy(TrueTileWall_Bottom.gameObject);
			}
			if (TrueTileWall_Top != null && TrueTileWall_Top.gameObject)
			{
				Destroy(TrueTileWall_Top.gameObject);
			}
		}
		private void OnSelectionChanged(object data)
		{
			var enabled = (bool)data;
			isSelected = enabled;
			SetRendererTints();
		}

		private void OnHighlightChanged(object data)
		{
			var enabled = (bool)data;
			isHighlighted = enabled;
			SetRendererTints();
		}
		private bool isSelected, isHighlighted;
		//private Color highlightColour = new(1.25f, 1.25f, 1.25f, 1f);
		private static Color highlightColour = new(2f, 2f, 2f, 1f);
		private static Color selectColour = highlightColour;
		void SetRendererTints()
		{
			var color = GetBiomeTint();
			var finalColor = isSelected ? selectColour* color : isHighlighted ? highlightColour* color : color;
			if (TrueTileWall_Bottom != null)
			{
				TrueTileWall_Bottom.color = finalColor;
			}
			if (TrueTileWall_Top != null)
			{
				TrueTileWall_Top.color = finalColor;
			}
		}

		Color GetBiomeTint()
		{
			Color baseColor = Color.white;
			float biomeTint = 0.2f;

			var zoneType = World.Instance.zoneRenderData.GetSubWorldZoneType(Grid.PosToCell(this));
			var zoneColor = World.Instance.zoneRenderData.zoneColours[(int)zoneType];

			Color color = new Color32(zoneColor.r, zoneColor.g, zoneColor.b, 255);
			color = Color.Lerp(baseColor, color, biomeTint);
			color.a = baseColor.a;

			return UIUtils.Darken(color, 50); //mimicing the tile shader darkening built tiles
		}
		void HandleTrueTilesSymbolOverrides()
		{
			bool HasValidTrueTileTexture = TryGetTrueTilesTexture(out var texture);
			if (HasValidTrueTileTexture)
			{
				TrueTileWall_Bottom = InstantiateTrueTilesBackground(texture, "bottomTile", 0f);
				TrueTileWall_Top = InstantiateTrueTilesBackground(texture, "topTile", 1f);
			}
			kbac.SetSymbolVisiblity("insulation_empty", HasValidTrueTileTexture);
			kbac.SetSymbolVisiblity("insulation", !HasValidTrueTileTexture);

		}

		public static bool trueTilesInitialized = false;
		public static bool TrueTilesEnabled = false;
		bool TryGetTrueTilesTexture(out Texture2D texture)
		{
			texture = null;
			if (trueTilesInitialized && !TrueTilesEnabled)
			{
				return false;
			}
			trueTilesInitialized = true;
			var type = Type.GetType("TrueTiles.Cmps.TileAssets, TrueTiles");

			if (type == null)
			{
				SgtLogger.l("TrueTiles TileAssets class does not exist.");
				return false;
			}
			var m_Instance = AccessTools.Property(type, "Instance")?.GetValue(null);
			if (m_Instance == null)
			{
				SgtLogger.l("TrueTiles TileAssets.Instance is not a valid property.");
				return false;
			}
			var m_Get = AccessTools.Method(type, "Get", [typeof(string), typeof(SimHashes)]);
			if (m_Get == null)
			{
				SgtLogger.l("TrueTiles TileAssets.Get is not a valid method.");
				return false;
			}
			var m_textureAsset = m_Get.Invoke(m_Instance, [TileConfig.ID, BuildingComplete.primaryElement.Element.id]);
			if (m_textureAsset == null)
			{
				SgtLogger.l("TrueTiles TileAssets.Get returned null.");
				return false;
			}
			var m_main = AccessTools.Field(m_textureAsset.GetType(), "main")?.GetValue(m_textureAsset);
			if (m_main == null)
			{
				SgtLogger.l("TrueTiles TileAssets.main is not a valid field.");
				return false;
			}
			if (m_main is not Texture2D tx)
			{
				SgtLogger.l("TrueTiles TileAssets.main is not a Texture2D.");
				return false;
			}
			TrueTilesEnabled = true;
			var tileSampleTexture = (Texture2D)m_main;
			SgtLogger.l(tileSampleTexture.height + " " + tileSampleTexture.width);
			texture = tileSampleTexture;

			return true;

		}
		static Dictionary<SimHashes, Sprite> CachedElementSprites = new();
		SpriteRenderer InstantiateTrueTilesBackground(Texture2D trueTilesTexture, string name, float yOffset)
		{
			//testGo = Instantiate(ModAssets.Prefabs.slimyPulse);
			var textureGO = new GameObject(name);
			var renderer = textureGO.AddComponent<SpriteRenderer>();

			//textureGO.transform.SetParent(transform);

			//var renderer = testGo.transform.Find("Quad").GetComponent<MeshRenderer>();
			var testMaterial = renderer.material;

			testMaterial = new Material(Shader.Find("TextMeshPro/Sprite"))
			{
				renderQueue = 3500
			};

			testMaterial.SetInt("_ZWrite", 1);

			renderer.material = testMaterial;
			var pos = this.transform.position;
			pos.y += yOffset;
			pos.x -= 0.5f;
			pos.z = Grid.GetLayerZ(Grid.SceneLayer.Ground);
			textureGO.transform.position = pos; //z value is one layer above liquid
			Sprite elementSprite;
			if (!CachedElementSprites.TryGetValue(BuildingComplete.primaryElement.ElementID, out var sprite))
			{
				var atlas = Assets.GetBuildingDef(TileConfig.ID).BlockTileAtlas;
				var all = (int)(Rendering.BlockTileRenderer.Bits.Right
					| Rendering.BlockTileRenderer.Bits.Down
					| Rendering.BlockTileRenderer.Bits.Left
					| Rendering.BlockTileRenderer.Bits.Up);

				var uvBox = GetUVBox(atlas, all);
				var tex = trueTilesTexture;

				var y = uvBox.y * tex.height;
				y -= 208f;
				y = Mathf.Clamp(y, 0, 1024 - 208);

				// total rect
				var rect = new Rect(uvBox.x * tex.width, y, 208, 208);

				// crop the middle part, which is the repeating piece
				rect.xMin += 40f;
				rect.xMax -= 40f;
				rect.yMin += 40f;
				rect.yMax -= 40f;

				rect.xMax -= 20f; //crop the right border for the connector

				elementSprite = Sprite.Create(tex, rect, Vector3.zero, 128); // 128 ppu here is why tiles are at a different scale to kanims, kanims use 100
			}
			else
			{
				elementSprite = sprite;
			}
			renderer.sprite = elementSprite;
			renderer.color = GetBiomeTint(); // set to biome color + bit of extra dark for blending better. use KAnimBatchedController.OnHighlightChanged on the main kbac to listen to changes for building selection

			textureGO.SetActive(true);
			return renderer;
		}
		private static Vector4 GetUVBox(TextureAtlas atlas, int connections)
		{
			var num3 = atlas.items[0].name.Length - 4 - 8;
			var startIndex = num3 - 1 - 8;
			var uvBox = Vector4.zero;

			for (var k = 0; k < atlas.items.Length; k++)
			{
				var item = atlas.items[k];

				var value = item.name.Substring(startIndex, 8);
				var requiredConnections = Convert.ToInt32(value, 2);

				if (requiredConnections == connections)
				{
					uvBox = item.uvBox;
					break;
				}
			}

			return uvBox;
		}
	}
}

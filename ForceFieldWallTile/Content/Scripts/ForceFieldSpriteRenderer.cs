using ElementUtilNamespace;
using ForceFieldWallTile.Content.Scripts.MeshGen;
using HarmonyLib;
using PeterHan.PLib.Core;
using Rendering.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rendering.BlockTileRenderer;
using static STRINGS.UI.DETAILTABS;

namespace ForceFieldWallTile.Content.Scripts
{
	/// <summary>
	/// Unused and probably broken, based on the true tiles version of the insulated wall adapter from RE
	/// </summary>
	internal class ForceFieldSpriteRenderer : KMonoBehaviour
	{
		[MyCmpGet]
		BuildingComplete BuildingComplete;

		SpriteRenderer ForceField;

		public override void OnSpawn()
		{
			base.OnSpawn();

			return;
			var connectionBits = (int)
					(Bits.Right
					| Bits.Down
					| Bits.Left
					| Bits.Up);
			ForceField = InstantiateTrueTilesBackground(Assets.GetBuildingDef(TilePOIConfig.ID).BlockTileAtlas.texture, "ForceField", connectionBits);

			//Subscribe((int)GameHashes.SelectObject, OnSelectionChanged);
			//Subscribe((int)GameHashes.HighlightObject, OnHighlightChanged);

			//ApplyBiomeTint();
		}
		public override void OnCleanUp()
		{
			//Unsubscribe((int)GameHashes.SelectObject, OnSelectionChanged);
			//Unsubscribe((int)GameHashes.HighlightObject, OnHighlightChanged);
			base.OnCleanUp();
			if (ForceField != null && ForceField.gameObject)
			{
				Destroy(ForceField.gameObject);
			}
		}
		private void OnSelectionChanged(object data)
		{
			bool enabled = ((Boxed<bool>)data).value;
			isSelected = enabled;
			SetRendererTints();
		}

		private void OnHighlightChanged(object data)
		{
			bool enabled = ((Boxed<bool>)data).value;
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
			var finalColor = isSelected ? selectColour * color : isHighlighted ? highlightColour* color : color;
			if (ForceField != null)
			{
				ForceField.color = finalColor;
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


			color = Color.Lerp(color, Color.blue, 0.5f);
			color.a = 0.6f;

			return color;
			//return UIUtils.Darken(color, 50); //mimicing the tile shader darkening built tiles
		}

		static Dictionary<int, Sprite> CachedConnectionSprites = new();
		SpriteRenderer InstantiateTrueTilesBackground(Texture2D trueTilesTexture, string name, int connectionBits)
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
			//pos.y += yOffset;
			pos.x -= 0.5f;
			pos.z = Grid.GetLayerZ(Grid.SceneLayer.Ground);

			//pos.x -= 0.14f;
			//pos.y -= 0.14f;

			textureGO.transform.position = pos; //z value is one layer above liquid
			Sprite elementSprite;

			if (!CachedConnectionSprites.TryGetValue(connectionBits, out var sprite))
			{
				var atlas = Assets.GetBuildingDef(TileConfig.ID).BlockTileAtlas;			

				var uvBox = GetUVBox(atlas, connectionBits);
				//var tex = trueTilesTexture;
				var tex = atlas.texture;

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

				//rect.xMax -= 20f; //crop the right border for the connector

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

using Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using static LogicGateVisualizer;

namespace BlueprintsV2
{
	/// <summary>
	/// these visualizers run fully independent from the games' building system
	/// I wrote this for the OniTogether mod, keeping it here for if I want 
	/// </summary>
	internal class CustomTileVisualizer
	{
		public enum VisualizerType
		{
			BUILDING,
			UTILITY,
			TILE,

			INVALID = -1
		}
		VisualizerType DermineBuildingType(string prefabId)
		{
			var def = Assets.GetBuildingDef(prefabId);
			if (def == null || def.BuildingPreview == null)
				return VisualizerType.INVALID;

			if (def.IsTilePiece
				&& def.isKAnimTile
				&& !def.BuildingComplete.TryGetComponent<Door>(out _)
				&& def.TileLayer != ObjectLayer.LadderTile)
			{
				if (def.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null)
				{
					return VisualizerType.UTILITY;
				}
				else if (!def.BuildingComplete.TryGetComponent<KBatchedAnimController>(out _))
				{
					return VisualizerType.TILE;
				}
			}
			return VisualizerType.BUILDING;
		}


		SpriteRenderer TileSpriteRenderer;
		static Dictionary<BuildingDef, BlockTileRenderer.RenderInfo> _tileInfos = [];
		static Dictionary<BuildingDef, Sprite> _placeSprites = [];
		void SetTileRenderer(BuildingDef tileDef)
		{
			if (TileSpriteRenderer == null)
				InstantiateTileRenderer();
			UpdateTileTexture(tileDef);
		}
		void MoveTileRenderer(int cell)
		{
			var pos = Grid.CellToPosCCC(cell, Grid.SceneLayer.FXFront);
			TileSpriteRenderer.transform.SetPosition(pos);
			//UpdateVisualColor(cell);
			TileSpriteRenderer.color = Color.white;
		}

		void UpdateTileTexture(BuildingDef def)
		{
			if (!_placeSprites.TryGetValue(def, out var sprite))
			{
				if (!_tileInfos.TryGetValue(def, out var renderInfo))
				{
					renderInfo = _tileInfos[def] = new BlockTileRenderer.RenderInfo(World.Instance.blockTileRenderer, (int)def.TileLayer, LayerMask.NameToLayer("Place"), def, SimHashes.Void);
				}
				var tex = renderInfo.material.mainTexture as Texture2D;
				var uv = renderInfo.atlasInfo.First().uvBox; //do AddVertexInfo trimming for other tile variants
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
				sprite = Sprite.Create(tex, rect, new(0.5f, 0.5f), 128); // 128 ppu 
			}
			TileSpriteRenderer.sprite = sprite;
		}

		void InstantiateTileRenderer()
		{
			var textureGO = new GameObject("TileRenderer");
			var renderer = textureGO.AddComponent<SpriteRenderer>();
			var mat = new Material(Shader.Find("TextMeshPro/Sprite"))
			{
				renderQueue = 4501
			};
			mat.SetInt("_ZWrite", 1);
			renderer.material = mat;
			TileSpriteRenderer = renderer;
		}
	}
	internal class BuildingVisualizer
	{
		private BuildingDef CurrentDef = null;
		private GameObject _visualizer;
		private void UpdateBuildingVisual(int cell)
		{
			var pos = Grid.CellToPosCBC(cell, CurrentDef.SceneLayer);
			_visualizer.transform.SetPosition(pos);
			UpdateRotation();
			if (_visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
			{
				//UpdateVisualColor(cell);
				//kbac.TintColour = currentColor;
			}
		}

		void InstantiateNewVisualizer(Vector3 targetPos)
		{
			int posCell = Grid.PosToCell(targetPos);
			Vector3 pos = Grid.CellToPosCBC(posCell, CurrentDef.SceneLayer);
			//_visualizer = GameUtil.KInstantiate(CurrentDef.BuildingPreview, pos, Grid.SceneLayer.FXFront, "OtherPlayerBuildingVisualizer", LayerMask.NameToLayer("Place"));

			_visualizer = new GameObject();
			_visualizer.SetActive(false);
			var anim = _visualizer.AddComponent<KBatchedAnimController>();
			anim.isMovable = true;
			anim.sceneLayer = Grid.SceneLayer.FXFront;
			anim.AnimFiles = CurrentDef.AnimFiles;
			anim.defaultAnim = "place";
			_visualizer.transform.SetPosition(pos);
			_visualizer.SetActive(true);
			SetSize(CurrentDef.WidthInCells, CurrentDef.HeightInCells);
			UpdateRotation();
			if (_visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
			{
				kbac.visibilityType = KAnimControllerBase.VisibilityType.Always;
				kbac.isMovable = true;
				kbac.Offset = Vector3.zero;
				//kbac.TintColour = visualColor;
				kbac.SetLayer(LayerMask.NameToLayer("Place"));
				if (CurrentDef.isKAnimTile && CurrentDef.BuildingComplete.GetComponent<IHaveUtilityNetworkMgr>() != null && kbac.HasAnimation("None_place"))
					kbac.Play("None_place"); //default non-connected pipe, change "place" to connection bit
				else
					kbac.Play("place");
			}
			else
			{
				_visualizer.SetLayerRecursively(LayerMask.NameToLayer("Place"));
			}
			//UpdatePosition(targetPos, true);
		}
		private void UpdateRotation()
		{
			if (_visualizer.TryGetComponent<KBatchedAnimController>(out var kbac))
			{
				kbac.Pivot = this.GetVisualizerPivot();
				kbac.Rotation = this.GetVisualizerRotation();
				kbac.Offset = this.GetVisualizerOffset();
				kbac.FlipX = this.GetVisualizerFlipX();
				kbac.FlipY = this.GetVisualizerFlipY();
			}
		}
		private Orientation CurrentOrientation = Orientation.Neutral;

		#region rotatableClone
		int width, height;
		private Vector3 pivot = Vector3.zero;
		private Vector3 visualizerOffset = Vector3.zero;

		public bool GetVisualizerFlipX() => this.CurrentOrientation == Orientation.FlipH;

		public bool GetVisualizerFlipY() => this.CurrentOrientation == Orientation.FlipV;
		public float GetVisualizerRotation()
		{
			switch (CurrentDef.PermittedRotations)
			{
				case PermittedRotations.R90:
				case PermittedRotations.R360:
					return -90f * (float)this.CurrentOrientation;
				default:
					return 0.0f;
			}
		}
		public Vector3 GetVisualizerPivot()
		{
			Vector3 pivot = this.pivot;
			switch (this.CurrentOrientation)
			{
				case Orientation.FlipH:
					pivot.x = -this.pivot.x;
					break;
			}
			return pivot;
		}
		private Vector3 GetVisualizerOffset()
		{
			Vector3 visualizerOffset;
			switch (this.CurrentOrientation)
			{
				case Orientation.FlipH:
					visualizerOffset = new Vector3(-this.visualizerOffset.x, this.visualizerOffset.y, this.visualizerOffset.z);
					break;
				case Orientation.FlipV:
					visualizerOffset = new Vector3(this.visualizerOffset.x, 1f, this.visualizerOffset.z);
					break;
				default:
					visualizerOffset = this.visualizerOffset;
					break;
			}
			return visualizerOffset;
		}

		public void SetSize(int width, int height)
		{
			this.width = width;
			this.height = height;
			if (width % 2 == 0)
			{
				this.pivot = new Vector3(-0.5f, 0.5f, 0.0f);
				this.visualizerOffset = new Vector3(0.5f, 0.0f, 0.0f);
			}
			else
			{
				this.pivot = new Vector3(0.0f, 0.5f, 0.0f);
				this.visualizerOffset = Vector3.zero;
			}
		}
		#endregion
	}
}

using BlueprintsV2.BlueprintData;
using BlueprintsV2.BlueprintsV2.BlueprintData.OniTogether_Integration;
using KSerialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;
using static BlueprintsV2.STRINGS.BLUEPRINTS_BLUEPRINTNOTE;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.NoteToolPlacedEntities
{
	public class BlueprintNote : KMonoBehaviour
	{
		public static string FILTERLAYER = ("BLUEPRINTV2_FILTER_NOTES");
		[Serialize]
		public bool SeatIndicator = false;
		[MyCmpReq] protected InfoDescription description;
		[MyCmpReq] protected KSelectable selectable;
		//protected MeshRenderer renderer;
		protected SpriteRenderer renderer;
		protected Color Tint = Color.white;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			SpawnSpriteRenderer();
		}

		public static void TriggerNoteVisibilityChange(bool on)
		{
			OnNoteVisiblityChanged?.Invoke(on);
		}
		public static void TriggerOpacityChange()
		{
			OnNoteOpacityChanged?.Invoke();
		}
		static event Action<bool> OnNoteVisiblityChanged;
		static event System.Action OnNoteOpacityChanged;
		int refreshHandle = -1, cancelHandle = -1;
		protected void SpawnSpriteRenderer()
		{
			GameObject offsetObject = new GameObject();
			offsetObject.layer = LayerMask.NameToLayer("Place");
			renderer = offsetObject.AddComponent<SpriteRenderer>();
			renderer.color = Color.white;
			renderer.sprite = ModAssets.BLUEPRINTS_CREATE_VISUALIZER_SPRITE;
			var testMaterial = renderer.material;
			testMaterial = new Material(Shader.Find("TextMeshPro/Sprite"))
			{
				renderQueue = RenderQueues.BlockTiles +1
			};
			testMaterial.SetInt("_ZWrite", 0); 
			var pos = this.transform.position;
			//pos.y += yOffset;
			pos.x -= Grid.HalfCellSizeInMeters;
			pos.z = Grid.GetLayerZ(Grid.SceneLayer.FXFront2);

			offsetObject.transform.position = pos; //z value is one layer above liquid
			renderer.material = testMaterial;
			offsetObject.transform.SetParent(this.transform);
			var sprite = renderer.sprite;
			offsetObject.transform.localScale = new Vector3(
				Grid.CellSizeInMeters / (sprite.texture.width / sprite.pixelsPerUnit),
				Grid.CellSizeInMeters / (sprite.texture.height / sprite.pixelsPerUnit)
			);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			if (SeatIndicator)
				Seat();

			refreshHandle = Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
			cancelHandle = Subscribe((int)GameHashes.Cancel, Cancel);

			if (SeatIndicator)
			{
				OnNoteVisiblityChanged += ChangeVisibility;
				ChangeVisibility(BlueprintState.NoteVisibility);
				RefreshTint();
			}
			OnNoteOpacityChanged += RefreshTint;
		}

		public override void OnCleanUp()
		{
			Unsubscribe(cancelHandle);
			Unsubscribe(refreshHandle);
			if (SeatIndicator)
			{
				OnNoteVisiblityChanged -= ChangeVisibility;
			}
			OnNoteOpacityChanged -= RefreshTint;
			if(!renderer.IsNullOrDestroyed() && !renderer.gameObject.IsNullOrDestroyed())
			{
				UnityEngine.Object.Destroy(renderer.gameObject);
				renderer = null;
			}
			base.OnCleanUp();
		}
		protected void RefreshTint()
		{
			var tintColor = Tint;
			tintColor.a = BlueprintState.NoteOpacity;
			renderer?.color = tintColor;
		}

		private void ChangeVisibility(bool visible)
		{
			renderer.enabled = (visible);
		}

		private void OnRefreshUserMenu(object data)
		{
			Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_cancel", DELETE_NOTE.NAME, new System.Action(this.OnCancel), tooltipText: DELETE_NOTE.TOOLTIP));
		}
		protected void Cancel(object _ = null) => OnCancel();
		protected void OnCancel()
		{
			DetailsScreen.Instance.Show(false);
			MP_Helpers.HandleNoteDeletion(this);
			this.DeleteObject();
		}
		void Seat()
		{
			SetDescription();
			Grid.Objects[Grid.PosToCell(this), (int)ModAssets.BlueprintNotesLayer] = this.gameObject;
			//gameObject.SetLayerRecursively(LayerMask.NameToLayer("Default"));
		}
		public virtual void SetDescription()
		{

		}
		protected void RefreshSelection()
		{
			if (selectable.IsSelected)
			{
				DetailsScreen.Instance.target = null;
				DetailsScreen.Instance.Refresh(gameObject);///should refresh screen, make sure to not have infinite loop by having selection changed event trigger this again with no changes
			}
		}
		public virtual BlueprintNoteData GetNoteData(Vector2I? newPosition = null)
		{
			return new BlueprintNoteData();
		}

		public static void ClearExistingNote(int cell)
		{
			if (!Grid.IsValidCell(cell))
				return;

			var existingItem = Grid.Objects[cell, (int)ModAssets.BlueprintNotesLayer];

			if (existingItem != null)
			{
				existingItem.DeleteObject();
				Grid.Objects[cell, (int)ModAssets.BlueprintNotesLayer] = null;
			}
		}
	}
}

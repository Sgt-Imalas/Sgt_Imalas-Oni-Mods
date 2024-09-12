using BlueprintsV2.BlueprintData;
using HarmonyLib;
using PeterHan.PLib.Options;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.Tools
{
	public sealed class CreateBlueprintTool : MultiFilteredDragTool
	{
		public static CreateBlueprintTool Instance { get; private set; }

		public CreateBlueprintTool()
		{
			Instance = this;
		}

		public static void DestroyInstance()
		{
			Instance = null;
		}
		bool toolActive = false;
		public override void OnDeactivateTool(InterfaceTool newTool)
		{
			toolActive = false;
			UnlockCam();
			base.OnDeactivateTool(newTool);
		}
		public override void OnActivateTool()
		{
			toolActive = true;
			base.OnActivateTool();
		}
		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

			visualizer = new GameObject("CreateBlueprintVisualizer");
			visualizer.SetActive(false);

			GameObject offsetObject = new GameObject();
			SpriteRenderer spriteRenderer = offsetObject.AddComponent<SpriteRenderer>();
			spriteRenderer.color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
			spriteRenderer.sprite = ModAssets.BLUEPRINTS_CREATE_VISUALIZER_SPRITE;

			offsetObject.transform.SetParent(visualizer.transform);
			offsetObject.transform.localPosition = new Vector3(0, Grid.HalfCellSizeInMeters);
			offsetObject.transform.localPosition = new Vector3(-Grid.HalfCellSizeInMeters, 0);
			var sprite = spriteRenderer.sprite;
			offsetObject.transform.localScale = new Vector3(
				Grid.CellSizeInMeters / (sprite.texture.width / sprite.pixelsPerUnit),
				Grid.CellSizeInMeters / (sprite.texture.height / sprite.pixelsPerUnit)
			);

			offsetObject.SetLayerRecursively(LayerMask.NameToLayer("Overlay"));
			visualizer.transform.SetParent(transform);

			FieldInfo areaVisualizerField = AccessTools.Field(typeof(DragTool), "areaVisualizer");
			FieldInfo areaVisualizerSpriteRendererField = AccessTools.Field(typeof(DragTool), "areaVisualizerSpriteRenderer");

			GameObject areaVisualizer = Util.KInstantiate(Traverse.Create(DeconstructTool.Instance).Field("areaVisualizer").GetValue<GameObject>());
			areaVisualizer.SetActive(false);

			areaVisualizer.name = "CreateBlueprintAreaVisualizer";
			areaVisualizerSpriteRendererField.SetValue(this, areaVisualizer.GetComponent<SpriteRenderer>());
			areaVisualizer.transform.SetParent(transform);
			areaVisualizer.GetComponent<SpriteRenderer>().color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;
			areaVisualizer.GetComponent<SpriteRenderer>().material.color = ModAssets.BLUEPRINTS_COLOR_BLUEPRINT_DRAG;

			areaVisualizerField.SetValue(this, areaVisualizer);

			gameObject.AddComponent<CreateBlueprintToolHoverCard>();
		}
		public void LockCam()
		{
			Task.Run(() =>
			{
				Task.Delay(25);
				if (Instance.toolActive)
				{
					CameraController.Instance.DisableUserCameraControl = true;
				}
			});
		}
		public void UnlockCam()
		{
			Task.Run(() =>
			{
				Task.Delay(30);
				CameraController.Instance.DisableUserCameraControl = false;
			});
		}

		public override void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
		{
			base.OnDragComplete(cursorDown, cursorUp);

			if (hasFocus)
			{
				Grid.PosToXY(cursorDown, out int x0, out int y0);
				Grid.PosToXY(cursorUp, out int x1, out int y1);

				if (x0 > x1)
				{
					Util.Swap(ref x0, ref x1);
				}

				if (y0 < y1)
				{
					Util.Swap(ref y0, ref y1);
				}

				var blueprint = BlueprintState.CreateBlueprint(new Vector2I(x0, y0), new Vector2I(x1, y1), MultiToolParameterMenu.Instance);
				if (blueprint.IsEmpty())
				{
					PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, STRINGS.UI.TOOLS.CREATE_TOOL.EMPTY, null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
				}

				else
				{
					void OnConfirmDelegate(string blueprintName)
					{
						blueprint.Rename(blueprintName, true);
						ModAssets.BlueprintFileHandling.HandleBlueprintLoading(blueprint.FilePath);

						SpeedControlScreen.Instance.Unpause(false);

						CameraController.Instance.DisableUserCameraControl = false;
						PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, STRINGS.UI.TOOLS.CREATE_TOOL.CREATED, null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
						UnlockCam();
					}
					void OnCancelDelegate()
					{
						SpeedControlScreen.Instance.Unpause(false);

						PopFXManager.Instance.SpawnFX(ModAssets.BLUEPRINTS_CREATE_ICON_SPRITE, STRINGS.UI.TOOLS.CREATE_TOOL.CANCELLED, null, PlayerController.GetCursorPos(KInputManager.GetMousePos()), Config.Instance.FXTime);
						UnlockCam();
					};
					SpeedControlScreen.Instance.Pause(false);
					FileNameDialog blueprintNameDialog = DialogUtil.CreateTextInputDialog(STRINGS.UI.DIALOGUE.NAMEBLUEPRINT_TITLE, blueprint.Folder, null, true, OnConfirmDelegate, OnCancelDelegate);

					blueprintNameDialog.Activate();
				}
			}
		}

		public override void OnSyncChanged(bool synced)
		{
			base.OnSyncChanged(synced);

			Config.Instance.CreateBlueprintToolSync = synced;
			POptions.WriteSettings(Config.Instance);
		}
	}
}

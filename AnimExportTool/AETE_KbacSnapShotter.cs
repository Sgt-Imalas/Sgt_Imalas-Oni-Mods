using KMod;
using PeterHan.PLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;
using static STRINGS.DUPLICANTS.CHORES;
using static STRINGS.UI.SPACEDESTINATIONS.CLUSTERMAPMETEORSHOWERS;

namespace AnimExportTool
{
	/// <summary>
	/// courtesy of Aki, https://github.com/aki-art/ONI-Mods/blob/master/AkisExtraTwitchEvents/Content/Scripts/AETE_KbacSnapShotter.cs
	/// </summary>
	public class AETE_KbacSnapShotter : KMonoBehaviour
	//, ISidescreenButtonControl
	{
		public static Dictionary<string, Vector2I> DimensionAddition = new() {
			{LiquidPumpingStationConfig.ID, new(0,4)},
		};


		private void OnRefreshUserMenu(object data)
		{
			Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_control", "Export Image", () => SnapShot(), tooltipText: "store current looks of this entity as a png"));
			Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_control", "Export Anim Sequence", () => SnapShotSequence(), tooltipText: "store current anim of this entity as a png sequence"));
		}


		private Camera camera;
		private RenderTexture targetTexture;
		private Texture2D debugWaterTex;
		private static readonly int DrawLayer = 30;

		[SerializeField] public bool AutoSnapshot = false;

		[MyCmpReq] public KBatchedAnimController kbac;
		[MyCmpReq] KPrefabID kPrefabID;
		[MyCmpReq] KBoxCollider2D Collider2D;

		public static AETE_KbacSnapShotter Instance { get; set; }

		public string SidescreenButtonText => "Snapshot";

		public string SidescreenButtonTooltip => "";
		string subDirectory = string.Empty;
		string suffix = string.Empty;

		public override void OnPrefabInit()
		{
			Instance = this;
			if (TryGetComponent<TreeFilterable>(out var filter))
				filter.tintOnNoFiltersSet = false;
		}

		public override void OnSpawn()
		{
			if (AutoSnapshot)
			{
				SnapShot();
				Util.KDestroyGameObject(gameObject);
			}
			else
				handle = Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);
		}
		int handle = -1;
		public override void OnCleanUp()
		{
			if (handle != -1)
				Unsubscribe(handle);
			Instance = null;
		}
		//static void RenderKanims()
		//{
		//	foreach (BatchSet activeBatchSet in KAnimBatchManager.Instance().activeBatchSets)
		//	{
		//		DebugUtil.Assert(activeBatchSet != null);
		//		DebugUtil.Assert(activeBatchSet.group != null);
		//		SgtLogger.l("Batch Set: " + activeBatchSet.key);

		//		Mesh mesh = activeBatchSet.group.mesh;
		//		for (int i = 0; i < activeBatchSet.batchCount; i++)
		//		{
		//			KAnimBatch batch = activeBatchSet.GetBatch(i);

		//			SgtLogger.l("	layer: " + batch.layer);

		//			float num = 0.01f / (float)(1 + batch.id % 256);
		//			if (batch.size != 0 && batch.active && batch.materialType != KAnimBatchGroup.MaterialType.UI)
		//			{
		//				Vector3 zero = Vector3.zero;
		//				zero.z = batch.position.z + num;
		//				int layer = batch.layer;
		//				Graphics.DrawMesh(mesh, zero, Quaternion.identity, activeBatchSet.group.GetMaterial(batch.materialType), layer, null, 0, batch.matProperties);
		//			}
		//		}
		//	}
		//}
		static void RenderBatch(KAnimBatch batch, Camera camera = null, int layerOverride = -1)
		{
			if (batch == null) return;

			float num = 0.01f / (float)(1 + batch.id % 256);
			if (batch.size != 0 && batch.active && batch.materialType != KAnimBatchGroup.MaterialType.UI)
			{
				Vector3 zero = Vector3.zero;
				zero.z = batch.position.z + num;
				int layer = batch.layer;
				if (layerOverride >= 0)
					layer = layerOverride;
				var activeBatchSet = KAnimBatchManager.Instance().activeBatchSets.Find(set => set.batches.Contains(batch));
				if (activeBatchSet == null)
				{
					SgtLogger.warning("could not find batch for " + batch.id);
					return;
				}
				var mesh = activeBatchSet.group.mesh;
				//SgtLogger.l("Rendering mesh for batch " + batch + " in " + activeBatchSet + " on layer " + layer);
				Graphics.DrawMesh(mesh, zero, Quaternion.identity, activeBatchSet.group.GetMaterial(batch.materialType), layer, camera, 0, batch.matProperties);
			}
		}

		public void SnapShotSequence()
		{
			StartCoroutine(SnapShotSequenceCor());
		}
		public string GetID()
		{
			string id = kPrefabID.PrefabTag.ToString();
			if (gameObject.TryGetComponent<MinionIdentity>(out var identity))
				id += "_" + identity.nameStringKey;
			return id;
		}

		IEnumerator SnapShotSequenceCor()
		{
			var currentAnim = kbac.currentAnim;
			subDirectory = GetID() + "_" + currentAnim.ToString();
			int frameCount = kbac.curAnim.numFrames;
			bool waspaused = SpeedControlScreen.Instance.IsPaused;
			if (waspaused)
				SpeedControlScreen.Instance.Unpause(false);

			for (int i = 0; i < frameCount; i++)
			{
				float percentage = (float)i / (float)frameCount;
				kbac.SetPositionPercent(percentage);
				suffix = $"_{i:D4}";
				yield return null;
				SnapShot();
			}
			subDirectory = string.Empty;
			suffix = string.Empty;
			if(waspaused)
				SpeedControlScreen.Instance.Pause(false);
		}

		public void SnapShot()
		{
			SelectTool.Instance.Select(null);
			camera = null;
			if (camera == null)
				InitCamera();

			KAnimBatchManager.Instance().UpdateActiveArea(new Vector2I(-9999, -9999), new Vector2I(9999, 9999));
			KAnimBatchManager.Instance().UpdateDirty(Time.frameCount);
			CameraController.Instance.baseCamera.enabled = false;
			camera.enabled = true;


			var pos = transform.position;

			//KBatchedAnimController kbacClone = CopyAnim(kbac.gameObject);
			//kbacClone.transform.position = pos;
			//kbacClone.gameObject.SetActive(true);


			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = targetTexture;

			//var mask = camera.cullingMask;
			//SgtLogger.l("Camera Culling Mask: " + camera.cullingMask+", batch layer: "+ kbacClone.batch.layer);

			//camera.targetTexture = targetTexture;
			//camera.cullingMask = LayerMask.NameToLayer(layer);

			//bool renderFGfirst = (building.Def.ForegroundLayer < building.Def.SceneLayer);

			//if (!renderFGfirst)
			//	RenderBatch(kbac.batch, camera, DrawLayer);
			/////render fg kbac
			//if (kbac.layering != null && kbac.layering.foregroundController != null)
			//{
			//	KBatchedAnimController fbKbac = ((KBatchedAnimController)kbac.layering.foregroundController);
			//	KAnimBatch fgBatch = fbKbac.batch;
			//	RenderBatch(fgBatch, camera, DrawLayer);
			//}
			//if (renderFGfirst)
			//	RenderBatch(kbac.batch, camera, DrawLayer);

			var kbacs = gameObject.GetComponentsInChildren<KBatchedAnimController>()
				.OrderBy(kbac => kbac.transform.position.z);

			foreach (var kbac in kbacs)
			{
				RenderBatch(kbac.batch, camera, DrawLayer);
			}


			//KAnimBatchManager.Instance().Render();
			//for (int i = 0; i<32; i++)
			//{
			//	var name = LayerMask.LayerToName(i);
			//	SgtLogger.l("Layer "+i+": "+ name);
			//}
			//camera.cullingMask = LayerMask.NameToLayer("ForceDraw");
			camera.Render();
			Texture2D tex = new Texture2D(targetTexture.width, targetTexture.height);
			tex.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
			tex.Apply();

			var imageBytes = tex.EncodeToPNG();
			

			var path = System.IO.Path.Combine(IO_Utils.ModPath, "_FullsizeImagesById", subDirectory, $"{GetID() + suffix}.png");
			var dir = System.IO.Directory.GetParent(path);
			System.IO.Directory.CreateDirectory(dir.FullName);
			File.WriteAllBytes(path, imageBytes);

			//camera.enabled = false;
			RenderTexture.active = previous;

			//Destroy(kbacClone.gameObject);
			camera.enabled = false;
			CameraController.Instance.baseCamera.enabled = true;
		}
		private void InitCamera()
		{
			int widthInt = Mathf.RoundToInt(Collider2D.size.x);
			int heightInt = Mathf.RoundToInt(Collider2D.size.y);
			Vector2I addition = new(0, 0);
			if (DimensionAddition.TryGetValue(kPrefabID.PrefabTag.ToString(), out addition))
			{
				widthInt += addition.X;
				heightInt += addition.Y;
			}

			float pixelsPerUnit = 100f;
			float paddingPx = 100f;


			float worldWidth = widthInt;
			float worldHeight = heightInt;



			int textureWidth = Mathf.CeilToInt(worldWidth * pixelsPerUnit + 2 * paddingPx);
			int textureHeight = Mathf.CeilToInt(worldHeight * pixelsPerUnit + 2 * paddingPx);

			var yOffset = ((heightInt / 2f) - addition.Y);
			float xOffset = 0;

			//if (building.Def.HeightInCells % 2 == 0)
			//	yOffset -= 0.5f;
			if (widthInt % 2 == 0)
				xOffset += 0.5f;


			//SgtLogger.l("Building Dims: " + textureWidth + "x" + textureHeight + "; offsets: " + xOffset+","+ yOffset);
			//targetTexture = new RenderTexture((int)width, (int)height, 24);
			targetTexture = new RenderTexture(textureWidth, textureHeight, 24);
			targetTexture.Create();
			//Screen.currentResolution.width

			//SgtLogger.l("TargetTextureDimensions: " + targetTexture.width + "x" + targetTexture.height);
			var reference = CameraController.Instance.baseCamera;

			camera = CameraController.CloneCamera(reference, "AETE_SnapshotCamera");
			camera.transform.parent = reference.transform.parent;
			camera.targetTexture = targetTexture;
			camera.enabled = false;
			camera.backgroundColor = Color.clear;
			camera.cullingMask = (1 << DrawLayer);

			var pos = transform.GetPosition();
			//SgtLogger.l("Current Camera Position: " + camera.transform.position);
			//SgtLogger.l("Object Position: " + pos);
			pos.z = -100;
			pos.y += yOffset;
			pos.x += xOffset;

			camera.transform.position = pos;
			camera.orthographicSize = textureHeight / (2f * pixelsPerUnit);
			camera.aspect = (float)textureWidth / textureHeight;
			//SgtLogger.l("Camera is orthographic? " + camera.orthographic + ", size: " + camera.orthographicSize + ", aspect: " + camera.aspect);
		}

		private KBatchedAnimController CopyAnim(GameObject original)
		{
			var go = new GameObject("AETE_TempKbac");
			go.transform.SetParent(original.transform.parent);
			go.transform.localPosition = original.transform.localPosition;
			go.SetActive(false);

			original.TryGetComponent(out KBatchedAnimController mKbac);

			var kbac = go.AddComponent<KBatchedAnimController>();

			bool hasSOC = original.TryGetComponent(out SymbolOverrideController mController);
			SymbolOverrideController controller = null;
			if (hasSOC)
				controller = SymbolOverrideControllerUtil.AddToPrefab(go);

			kbac.SwapAnims(mKbac.animFiles);

			if (mKbac.OverrideAnimFiles != null)
			{
				foreach (var animOverride in mKbac.OverrideAnimFiles)
				{
					kbac.AddAnimOverrides(animOverride.file, animOverride.priority);
				}
			}
			if (hasSOC)
			{
				foreach (var symbol in mController.symbolOverrides)
				{
					controller.AddSymbolOverride(symbol.targetSymbol, symbol.sourceSymbol);
				}
			}

			if (mKbac.animFiles != null)
			{
				foreach (var mAnim in mKbac.animFiles)
				{
					foreach (var anim in kbac.animFiles)
					{
						if (anim?.data.name == anim.name)
						{
							var build = anim.GetData().build;

							foreach (var symbol in build.symbols)
							{
								kbac.SetSymbolVisiblity(symbol.hash, mKbac.GetSymbolVisiblity(symbol.hash));
							}

							break;
						}
					}
				}
			}

			kbac.animScale = mKbac.animScale;
			kbac.TintColour = mKbac.TintColour;
			kbac.offset = mKbac.offset;
			kbac.FlipX = mKbac.flipX;
			kbac.FlipY = mKbac.flipY;
			kbac.Rotation = mKbac.Rotation;
			kbac.sceneLayer = mKbac.sceneLayer;
			//kbac.SetLayer(layer);
			//go.SetLayerRecursively(layer);

			go.SetActive(true);

			kbac.SetDirty();

			kbac.Play(mKbac.currentAnim, KAnim.PlayMode.Paused);
			kbac.SetPositionPercent(mKbac.GetPositionPercent());

			return kbac;
		}

		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
		}

		public bool SidescreenEnabled() => DebugHandler.enabled;

		public bool SidescreenButtonInteractable() => true;

		public void OnSidescreenButtonPressed()
		{
			SnapShot();
		}

		public int HorizontalGroupID() => -1;

		public int ButtonSideScreenSortOrder() => -99999;
	}
}

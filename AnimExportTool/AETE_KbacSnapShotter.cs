using PeterHan.PLib.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace AnimExportTool
{
	/// <summary>
	/// courtesy of Aki, https://github.com/aki-art/ONI-Mods/blob/master/AkisExtraTwitchEvents/Content/Scripts/AETE_KbacSnapShotter.cs
	/// </summary>
	public class AETE_KbacSnapShotter : KMonoBehaviour
		, ISidescreenButtonControl
	{
		private Camera camera;
		private Vector3 voidPosition = new(-100, 0);
		private RenderTexture targetTexture;
		private Texture2D debugWaterTex;
		private static readonly string layer = "Default";//"Pickupable");

		[MyCmpReq] KBatchedAnimController kbac;
		[MyCmpReq] KPrefabID kPrefabID;

		public static AETE_KbacSnapShotter Instance { get; set; }

		public string SidescreenButtonText => "Snapshot";

		public string SidescreenButtonTooltip => "";

		public override void OnPrefabInit()
		{
			Instance = this;
		}

		public override void OnCleanUp()
		{
			Instance = null;
		}

		public void SnapShot()
		{
			if (camera == null)
				InitCamera();

			CameraController.Instance.baseCamera.enabled =false;
			camera.enabled = true;


			var pos = transform.position;

			var kanimControllerGOClone = CopyAnim(kbac.gameObject);
			kanimControllerGOClone.transform.position = pos;
			kanimControllerGOClone.gameObject.SetActive(true);


			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = targetTexture;

			var prev = camera.targetTexture;
			//var mask = camera.cullingMask;
			//SgtLogger.l("Camera Culling Mask: " + mask + " - " + LayerMask.LayerToName(mask));

			//camera.targetTexture = targetTexture;
			//camera.cullingMask = LayerMask.NameToLayer(layer);

			camera.Render();

			//camera.cullingMask = mask;
			//camera.targetTexture = prev;

			Texture2D tex = new Texture2D(targetTexture.width, targetTexture.height);
			tex.ReadPixels(new Rect(0, 0, targetTexture.width, targetTexture.height), 0, 0);
			tex.Apply();

			var imageBytes = tex.EncodeToPNG();
			File.WriteAllBytes(System.IO.Path.Combine(IO_Utils.ModPath, $"TestImageCamExperiment_{kPrefabID.PrefabTag}.png"), imageBytes);

			//camera.enabled = false;
			RenderTexture.active = previous;

			Destroy(kanimControllerGOClone.gameObject);
			camera.enabled = false;
			CameraController.Instance.baseCamera.enabled = true;
		}

		private void InitCamera()
		{
			targetTexture = new RenderTexture(200, 200, 24);
			targetTexture.Create();

			SgtLogger.l("TargetTextureDimensions: " + targetTexture.width + "x" + targetTexture.height);
			var reference = CameraController.Instance.baseCamera;

			camera = CameraController.CloneCamera(reference, "AETE_SnapshotCamera");
			camera.transform.parent = reference.transform.parent;
			//camera.cullingMask = LayerMask.NameToLayer(layer);
			camera.targetTexture = targetTexture;

			camera.enabled = false;
			//camera.transform.position = voidPosition;
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

			go.SetLayerRecursively(LayerMask.NameToLayer(layer));

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

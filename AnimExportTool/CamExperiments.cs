using ProcGen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace AnimExportTool
{
	internal class CamExperiments
	{
		public static void DoCamThing()
		{
			//var source = ScreenPrefabs.Instance.RetiredColonyInfoScreen;
			//var prefab = source.duplicantPrefab;

			var kbacGO = new GameObject();
			kbacGO.SetActive(false);
			kbacGO.transform.SetPosition(new(0, 0, 0));
			var anim = kbacGO.AddComponent<KBatchedAnimController>();
			
			anim.AnimFiles = new KAnimFile[1]
			{
				Assets.GetAnim("storagelocker_kanim")
			};
			kbacGO.layer = 2;
			kbacGO.SetActive(true);
			anim.Play("on");


			Camera cam = new GameObject("KanimCamera").AddComponent<Camera>();
			cam.cullingMask = 2;
			RenderTexture renderTexture = new RenderTexture(1920, 1080, 24);
			renderTexture.Create();
			cam.targetTexture = renderTexture;
			cam.gameObject.SetActive(true);
			cam.transform.position = new Vector3(0, 0, -10);

			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTexture;

			cam.Render();

			Texture2D tex = new Texture2D(renderTexture.width, renderTexture.height);
			tex.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
			tex.Apply();

			var imageBytes = tex.EncodeToPNG();
			File.WriteAllBytes(System.IO.Path.Combine(IO_Utils.ModPath,"TestImageCamExperiment.png"), imageBytes);

			RenderTexture.active = previous;

			UnityEngine.Object.Destroy(kbacGO);
			UnityEngine.Object.Destroy(cam.gameObject);
		}
	}
}

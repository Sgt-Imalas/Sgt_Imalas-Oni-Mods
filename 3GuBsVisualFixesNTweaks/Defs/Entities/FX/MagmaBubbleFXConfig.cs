using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace _3GuBsVisualFixesNTweaks.Defs.Entities.FX
{
	internal class MagmaBubbleFXConfig : IEntityConfig
	{
		public const string ID = "MagmaBubbleFx";

		public GameObject CreatePrefab()
		{
			var go = EntityTemplates.CreateEntity(ID, ID, false);
			var kbac = go.AddOrGet<KBatchedAnimController>();
			kbac.AnimFiles = new KAnimFile[] { Assets.GetAnim("magma_bubble_fx_kanim") };
			kbac.materialType = KAnimBatchGroup.MaterialType.Simple;
			kbac.initialAnim = "idle";
			kbac.initialMode = KAnim.PlayMode.Once;
			kbac.isMovable = true;
			kbac.destroyOnAnimComplete = true;
			kbac.TintColour = ElementLoader.FindElementByHash(SimHashes.Magma).substance.colour;
			go.AddOrGet<LoopingSounds>();
			return go;
		}

		public string[] GetDlcIds() => null;
		public void OnPrefabInit(GameObject inst) { }
		public void OnSpawn(GameObject inst) { }
	}
}

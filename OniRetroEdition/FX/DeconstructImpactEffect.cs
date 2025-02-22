using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace OniRetroEdition.FX
{
	internal class DeconstructImpactEffect : IEntityConfig
	{
		public const string ID = "DeconstructImpactFX";

		public GameObject CreatePrefab()
		{
			var go = EntityTemplates.CreateEntity(ID, ID, false);
			var kbac = go.AddOrGet<KBatchedAnimController>();
			kbac.AnimFiles = new KAnimFile[] { Assets.GetAnim("deconstruct_impact_kanim") };
			kbac.materialType = KAnimBatchGroup.MaterialType.Simple;
			kbac.initialAnim = "deconstruct_impact";
			kbac.initialMode = KAnim.PlayMode.Loop;
			kbac.isMovable = true;
			kbac.destroyOnAnimComplete = false;
			//kbac.TintColour = ElementLoader.FindElementByHash(SimHashes.Petroleum).substance.colour;
			go.AddOrGet<LoopingSounds>();
			return go;
		}

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;
		public void OnPrefabInit(GameObject inst) { }
		public void OnSpawn(GameObject inst) { }
	}
}

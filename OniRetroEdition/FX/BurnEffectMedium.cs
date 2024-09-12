using UnityEngine;

namespace OniRetroEdition.FX
{
	internal class BurnEffectMedium : IEntityConfig
	{
		public const string ID = "pyroFireMediumFx";

		public GameObject CreatePrefab()
		{
			var go = EntityTemplates.CreateEntity(ID, ID, false);
			var kbac = go.AddOrGet<KBatchedAnimController>();
			kbac.AnimFiles = new KAnimFile[] { Assets.GetAnim("pyro_fire_fx_kanim") };
			kbac.materialType = KAnimBatchGroup.MaterialType.Simple;
			kbac.initialAnim = "fire_med_loop";
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

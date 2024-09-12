using UnityEngine;

namespace OniRetroEdition.FX
{
	internal class BurnEffectExtinguish : IEntityConfig
	{
		public const string ID = "pyroFireExtinguishFx";

		public GameObject CreatePrefab()
		{
			var go = EntityTemplates.CreateEntity(ID, ID, false);
			var kbac = go.AddOrGet<KBatchedAnimController>();
			kbac.AnimFiles = new KAnimFile[] { Assets.GetAnim("pyro_fire_fx_kanim") };
			kbac.materialType = KAnimBatchGroup.MaterialType.Simple;
			kbac.initialAnim = "fire_extinguish";
			kbac.initialMode = KAnim.PlayMode.Once;
			kbac.isMovable = true;
			kbac.destroyOnAnimComplete = true;
			//kbac.TintColour = ElementLoader.FindElementByHash(SimHashes.Petroleum).substance.colour;
			go.AddOrGet<LoopingSounds>();
			return go;
		}

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;
		public void OnPrefabInit(GameObject inst) { }
		public void OnSpawn(GameObject inst) { }
	}
}

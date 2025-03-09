using UnityEngine;

namespace OniRetroEdition.FX
{
	internal class DigIceEffect : IEntityConfig
	{
		public const string ID = "DigIceFx";

		public GameObject CreatePrefab()
		{
			var go = EntityTemplates.CreateEntity(ID, ID, false);
			var kbac = go.AddOrGet<KBatchedAnimController>();
			kbac.AnimFiles = new KAnimFile[] { Assets.GetAnim("dig_ice_kanim") };
			kbac.materialType = KAnimBatchGroup.MaterialType.Simple;
			kbac.initialAnim = "idle";
			kbac.initialMode = KAnim.PlayMode.Loop;
			kbac.isMovable = true;
			kbac.destroyOnAnimComplete = false;
			//kbac.TintColour = ElementLoader.FindElementByHash(SimHashes.Petroleum).substance.colour;
			go.AddOrGet<LoopingSounds>();
			return go;
		}

		public string[] GetDlcIds() => null;
		public void OnPrefabInit(GameObject inst) { }
		public void OnSpawn(GameObject inst) { }
	}
}

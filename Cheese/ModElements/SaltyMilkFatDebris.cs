using UnityEngine;

namespace Cheese.ModElements
{
	internal class SaltyMilkFatDebris : IOreConfig
	{
		public SimHashes ElementID => ModElementRegistration.SaltyMilkFat.SimHash;

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

		public GameObject CreatePrefab()
		{
			GameObject OreEntity = EntityTemplates.CreateSolidOreEntity(this.ElementID);
			OreEntity.AddOrGet<ElementDissolver>();
			return OreEntity;
		}
	}
}

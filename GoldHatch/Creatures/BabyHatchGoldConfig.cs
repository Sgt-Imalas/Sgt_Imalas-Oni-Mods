using UnityEngine;

namespace GoldHatch.Creatures
{
	[EntityConfigOrder(1)]
	internal class BabyHatchGoldConfig : IEntityConfig
	{
		public GameObject CreatePrefab()
		{
			GameObject hatch = HatchGoldConfig.CreateHatch(HatchGoldConfig.ID_BABY, (string)STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.BABY.NAME, (string)STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.BABY.DESC, "baby_hatch_gold_kanim", true);
			EntityTemplates.ExtendEntityToBeingABaby(hatch, (Tag)HatchGoldConfig.ID);
			return hatch;
		}
		public string[] GetDlcIds() => null;


		public void OnPrefabInit(GameObject inst)
		{
			;
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}

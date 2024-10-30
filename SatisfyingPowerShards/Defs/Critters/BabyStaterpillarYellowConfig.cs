using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SatisfyingPowerShards.Defs.Critters
{

	[EntityConfigOrder(2)]
	internal class BabyStaterpillarYellowConfig : IEntityConfig
	{
		public const string ID = "StaterpillarYellowBaby";

		public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

		public GameObject CreatePrefab()
		{
			GameObject staterpillar = StaterpillarYellowConfig.CreateStaterpillar(ID, STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_YELLOW.BABY.NAME, STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_YELLOW.BABY.DESC, "baby_caterpillar_yellow_kanim", true);
			EntityTemplates.ExtendEntityToBeingABaby(staterpillar, StaterpillarYellowConfig.ID);
			return staterpillar;
		}

		public void OnPrefabInit(GameObject prefab)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
	}
}

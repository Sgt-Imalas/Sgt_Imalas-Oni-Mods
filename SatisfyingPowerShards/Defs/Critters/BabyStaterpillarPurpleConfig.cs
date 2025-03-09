using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SatisfyingPowerShards.Defs.Critters
{
	[EntityConfigOrder(2)]
	internal class BabyStaterpillarPurpleConfig : IEntityConfig, IHasDlcRestrictions
	{
		public const string ID = "StaterpillarPurpleBaby";

		public string[] GetDlcIds() => null;

		public GameObject CreatePrefab()
		{
			GameObject staterpillar = StaterpillarYellowConfig.CreateStaterpillar(ID, STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_PURPLE.BABY.NAME, STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_PURPLE.BABY.DESC, "baby_caterpillar_purple_kanim", true);
			EntityTemplates.ExtendEntityToBeingABaby(staterpillar, StaterpillarPurpleConfig.ID);
			return staterpillar;
		}

		public void OnPrefabInit(GameObject prefab)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}

		public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public string[] GetForbiddenDlcIds() => null;
	}
}

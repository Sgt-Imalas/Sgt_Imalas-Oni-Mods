using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Defs.Entities
{
	class DeepSpaceInsightConfig : IEntityConfig, IHasDlcRestrictions
	{
		public string[] GetDlcIds() => null;

		public const string ID = "RTB_DeepSpaceInsight";
		public static readonly Tag TAG = TagManager.Create(ID);
		public const float MASS = 1f;

		public string[] GetRequiredDlcIds() => DlcManager.EXPANSION1;

		public string[] GetForbiddenDlcIds() => (string[])null;

		public GameObject CreatePrefab()
		{
			GameObject looseEntity = EntityTemplates.CreateLooseEntity(ID, (string)STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RTB_DEEPSPACEINSIGHT.NAME, (string)STRINGS.ITEMS.INDUSTRIAL_PRODUCTS.RTB_DEEPSPACEINSIGHT.DESC, 1f, true, Assets.GetAnim((HashedString)"floppy_disc_kanim"), "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.CIRCLE, 0.35f, 0.35f, true, additionalTags: new List<Tag>()
			{
				GameTags.IndustrialProduct,
				GameTags.Experimental
			});
			looseEntity.AddOrGet<EntitySplitter>().maxStackSize = (float)TUNING.ROCKETRY.DESTINATION_RESEARCH.BASIC;
			return looseEntity;
		}

		public void OnPrefabInit(GameObject inst)
		{
		}

		public void OnSpawn(GameObject inst)
		{
		}
		public string[] GetAnyRequiredDlcIds()
		{
			return null;
		}

	}
}

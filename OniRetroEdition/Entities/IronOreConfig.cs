using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.Entities
{
	internal class IronOreConfig : IOreConfig
	{
		public SimHashes ElementID => SimHashes.IronOre;

		public GameObject CreatePrefab()
		{
			GameObject solidOreEntity = EntityTemplates.CreateSolidOreEntity(this.ElementID);
			var targetAnim = Config.Instance.IronOreTexture == Config.EarlierVersion.Beta ? "hematite_thermal_kanim" : "hematite_alpha_kanim";

			if (solidOreEntity.TryGetComponent<KBatchedAnimController>(out var kbac) && Assets.TryGetAnim(targetAnim, out var kanimFile))
				kbac.animFiles = [kanimFile];
			return solidOreEntity;
		}
	}
}

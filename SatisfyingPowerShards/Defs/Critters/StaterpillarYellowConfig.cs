using Klei.AI;
using SatisfyingPowerShards.Components;
using SatisfyingPowerShards.Defs.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SatisfyingPowerShards.Defs.Critters
{
	internal class StaterpillarYellowConfig : IEntityConfig, IHasDlcRestrictions
	{
		public const string ID = "StaterpillarYellow";
		public const string BASE_TRAIT_ID = "StaterpillarYellowBaseTrait";
		public const string EGG_ID = "StaterpillarYellowEgg";
		public const int EGG_SORT_ORDER = 0;
		private static float KG_ORE_EATEN_PER_CYCLE = 60f;
		private static float CALORIES_PER_KG_OF_ORE = StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE / StaterpillarConfig.KG_ORE_EATEN_PER_CYCLE;

		public static GameObject CreateStaterpillar(
		  string id,
		  string name,
		  string desc,
		  string anim_file,
		  bool is_baby)
		{
			GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseStaterpillarConfig.BaseStaterpillar(id, name, desc, anim_file, BASE_TRAIT_ID, is_baby, ObjectLayer.Wire, StaterpillarYellowGeneratorConfig.ID, Tag.Invalid, warningHighTemperature: 313.15f, lethalLowTemperature: 173.15f, lethalHighTemperature: 373.15f), TUNING.CREATURES.SPACE_REQUIREMENTS.TIER4);
			Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, (string)null, false, (ChoreGroup[])null, true, true);
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, StaterpillarTuning.STANDARD_STOMACH_SIZE, name));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float)(-(double)StaterpillarTuning.STANDARD_CALORIES_PER_CYCLE * 1.75f / 600.0), global::STRINGS.UI.TOOLTIPS.BASE_VALUE));
			trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 100f, name));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 150f, name));
			List<Diet.Info> infoList = new List<Diet.Info>();
			infoList.AddRange((IEnumerable<Diet.Info>)BaseStaterpillarConfig.RawMetalDiet(SimHashes.Hydrogen.CreateTag(), CALORIES_PER_KG_OF_ORE, StaterpillarTuning.POOP_CONVERSTION_RATE, (string)null, 0.0f));
			infoList.AddRange((IEnumerable<Diet.Info>)BaseStaterpillarConfig.RefinedMetalDiet(SimHashes.Hydrogen.CreateTag(), CALORIES_PER_KG_OF_ORE, StaterpillarTuning.POOP_CONVERSTION_RATE, (string)null, 0.0f));
			List<Diet.Info> diet_infos = infoList;
			GameObject go = BaseStaterpillarConfig.SetupDiet(wildCreature, diet_infos);

			if (!is_baby)
			{
				go.AddComponent<PowerShardGrowthMonitor>();
			}
			return go;
		}

		public string[] GetDlcIds() => null;

		public virtual GameObject CreatePrefab() =>
			EntityTemplates.ExtendEntityToFertileCreature(
				StaterpillarConfig.CreateStaterpillar(
					ID,
					STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_YELLOW.NAME,
					STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_YELLOW.DESC,
					"caterpillar_yellow_kanim",
					false), null,
				EGG_ID,
				STRINGS.CREATURES.SPECIES.STATERPILLAR.VARIANT_YELLOW.EGG_NAME,
				global::STRINGS.CREATURES.SPECIES.STATERPILLAR.DESC, "egg_caterpillar_yellow_kanim",
				StaterpillarTuning.EGG_MASS,
				BabyStaterpillarYellowConfig.ID,
				60f,
				20f,
				StaterpillarTuning.EGG_CHANCES_BASE,
				4);

		public void OnPrefabInit(GameObject prefab) => prefab.GetComponent<KBatchedAnimController>().SetSymbolVisiblity((KAnimHashedString)"gulp", false);

		public void OnSpawn(GameObject inst)
		{
		}

		public string[] GetRequiredDlcIds() => [DlcManager.EXPANSION1_ID];

		public string[] GetForbiddenDlcIds() => null;
		public string[] GetAnyRequiredDlcIds() => null;
	}
}

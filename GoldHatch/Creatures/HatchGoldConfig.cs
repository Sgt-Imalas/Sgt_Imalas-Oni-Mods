using Klei.AI;
using System.Collections.Generic;
using UnityEngine;

namespace GoldHatch.Creatures
{
	internal class HatchGoldConfig : IEntityConfig
	{

		public const string ID = "HatchGold";
		public const string ID_BABY = "HatchGoldBaby";
		public const string BASE_TRAIT_ID = "HatchGoldBaseTrait";
		public const string EGG_ID = "HatchGoldEgg";

		public GameObject CreatePrefab()
		{

			var entity = EntityTemplates.ExtendEntityToFertileCreature(
				CreateHatch(ID, (string)STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.NAME, (string)STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.DESC, "hatch_gold_kanim", false), EGG_ID,
				(string)STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.EGG_NAME, (string)STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.DESC, "egg_hatch_gold_kanim", HatchTuning.EGG_MASS, ID_BABY, 60f, 20f, GoldHatchTuning.EGG_CHANCES_GOLD, this.GetDlcIds(), HatchHardConfig.EGG_SORT_ORDER + 1);
			return entity;
		}

		public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;
		public static GameObject CreateHatch(
			string id,
			string name,
			string desc,
			string anim_file,
			bool is_baby)
		{
			GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseHatchConfig.BaseHatch(id, name, desc, anim_file, BASE_TRAIT_ID, is_baby), HatchTuning.PEN_SIZE_PER_CREATURE);
			Trait trait = Db.Get().CreateTrait(BASE_TRAIT_ID, name, name, (string)null, false, (ChoreGroup[])null, true, true);
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, HatchTuning.STANDARD_STOMACH_SIZE, name));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, (float)(-(double)HatchTuning.STANDARD_CALORIES_PER_CYCLE / 600.0), (string)global::STRINGS.UI.TOOLTIPS.BASE_VALUE));
			trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 400f, name));
			trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
			List<Diet.Info> diet_infos = GoldMetalDiet(HatchMetalConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, (string)null, 0.0f);
			double caloriesPerKgOfOre = (double)HatchMetalConfig.CALORIES_PER_KG_OF_ORE;
			double minPoopSizeInKg = (double)HatchMetalConfig.MIN_POOP_SIZE_IN_KG;
			return BaseHatchConfig.SetupDiet(wildCreature, diet_infos, (float)caloriesPerKgOfOre, (float)minPoopSizeInKg);
		}
		public static List<Diet.Info> GoldMetalDiet(
			float caloriesPerKg,
			float producedConversionRate,
			string diseaseId,
			float diseasePerKgProduced)
		{
			List<Diet.Info> infoList = new List<Diet.Info>();

			var goldTag = SimHashes.Gold.CreateTag();

			foreach (var element in ElementLoader.elements
				.FindAll(e => e.IsSolid && (e.HasTag(GameTags.Metal) ^ e.HasTag(GameTags.RefinedMetal)))) //no alloys (both tags)
			{
				if (element.id != SimHashes.Gold //no eating gold
					&& element.id != SimHashes.GoldAmalgam //no eating gold
					&& element.id != SimHashes.Lead) //separate rate for lead
				{
					infoList.Add(
						new(new HashSet<Tag>() { element.id.CreateTag() },
						goldTag,
						caloriesPerKg,
						producedConversionRate,
						diseaseId,
						diseasePerKgProduced)
					 );
				}
			}
			//lead -> gold with 100% conversion rate
			infoList.Add(new(new HashSet<Tag>() { SimHashes.Lead.CreateTag() },
						goldTag,
						caloriesPerKg,
						1,
						diseaseId,
						diseasePerKgProduced)
					 );
			return infoList;
		}


		public void OnPrefabInit(GameObject inst)
		{
		}
		public void OnSpawn(GameObject inst)
		{

		}
	}
}

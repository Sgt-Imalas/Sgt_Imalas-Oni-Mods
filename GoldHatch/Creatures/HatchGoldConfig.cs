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
				CreateHatch(ID, (string)STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.NAME, (string)STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.DESC, "hatch_gold_build_kanim", false),
				null, EGG_ID, STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.EGG_NAME, STRINGS.CREATURES.SPECIES.HATCH.VARIANT_GOLD.DESC, "egg_hatch_gold_kanim", HatchTuning.EGG_MASS, ID_BABY, 60f, 20f, GoldHatchTuning.EGG_CHANCES_GOLD, HatchHardConfig.EGG_SORT_ORDER + 1);
			return entity;
		}

		public string[] GetDlcIds() => null;
		public static GameObject CreateHatch(
			string id,
			string name,
			string desc,
			string anim_file,
			bool is_baby)
		{
			string animationKanim = is_baby ? anim_file : "hatch_kanim";
			string symbol_override_prefix = is_baby ? null : "goldhatch_";

			GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseHatchConfig.BaseHatch(id, name, desc, animationKanim, BASE_TRAIT_ID, is_baby, symbol_override_prefix), HatchTuning.PEN_SIZE_PER_CREATURE);
			
			///fix klei mistake in their methods
			if(!is_baby)
			{
				var buildKanim = Assets.GetAnim(anim_file);
				var animKanim = Assets.GetAnim("hatch_kanim");
				///place the proper build anim in the kbac because klei hardcoded "hatch_build" in BaseHatch...
				if (wildCreature.TryGetComponent<KBatchedAnimController>(out var kbac))
				{
					kbac.AnimFiles = new KAnimFile[] {
					buildKanim //build file - custom gold hatch icons
					,animKanim //anim file - default hatch animations
					};
				}
				///apply proper symbol overrides
				if(wildCreature.TryGetComponent<SymbolOverrideController>(out var soc))
				{
					soc.ApplySymbolOverridesByAffix((buildKanim == null) ? animKanim : buildKanim, symbol_override_prefix);
				}
			}

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

			var goldTag = SimHashes.Gold.CreateTag(); //refined metal
			var goldAmalgamTag = SimHashes.GoldAmalgam.CreateTag(); //ore

			foreach (var element in ElementLoader.elements
				.FindAll(e => e.IsSolid && (e.HasTag(GameTags.Metal) ^ e.HasTag(GameTags.RefinedMetal)))) //no alloys (both tags)
			{
				bool isRefined = element.HasTag(GameTags.RefinedMetal);


				if (element.id != SimHashes.Gold //no eating gold
					&& element.id != SimHashes.GoldAmalgam //no eating gold amalgam
					&& element.id != SimHashes.Lead) //separate rate for lead
				{
					infoList.Add(
						new(new HashSet<Tag>() { element.id.CreateTag() },
						isRefined ? goldTag : goldAmalgamTag,  //ores get converted to amalgam, refined metals to gold
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

using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace CompressedCritters.Critters.Hatches
{
    [EntityConfigOrder(1)]
    internal class CompressedHatchHardConfig : IEntityConfig
    {
        public static string ID = "CC_Compressed" + HatchHardConfig.ID;
        

        public static GameObject CreateHatch(
            string id,
            string name,
            string desc,
            string anim_file,
            bool is_baby)
        {
            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseHatchConfig.BaseHatch(id, name, desc, anim_file, ID + "BaseTrait", is_baby, "hvy_"), CompressedHatchTuning.PEN_SIZE_PER_CREATURE);
            ModAssets.MultiplyButcherable(wildCreature, Mathf.RoundToInt(CompressedHatchTuning.SIZEMULTIPLIER));
            Trait trait = Db.Get().CreateTrait(ID + "BaseTrait", name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, CompressedHatchTuning.STANDARD_STOMACH_SIZE, name));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -CompressedHatchTuning.STANDARD_CALORIES_PER_CYCLE / 600.0f, (string)global::STRINGS.UI.TOOLTIPS.BASE_VALUE));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
            List<Diet.Info> diet_infos = BaseHatchConfig.HardRockDiet(SimHashes.Carbon.CreateTag(), HatchHardConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0.0f);
            diet_infos.AddRange(BaseHatchConfig.MetalDiet(SimHashes.Carbon.CreateTag(), HatchHardConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0.0f));
            return BaseHatchConfig.SetupDiet(wildCreature, diet_infos, HatchConfig.CALORIES_PER_KG_OF_ORE, HatchConfig.MIN_POOP_SIZE_IN_KG);
        }
        public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(CreateHatch(ID, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.NAME, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.DESC, "hatch_kanim", false), ID + "Egg", STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.EGG_NAME, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.DESC, "egg_hatch_kanim", CompressedHatchTuning.EGG_MASS, ID + "Baby", 60f, 20f, CompressedHatchTuning.EGG_CHANCES_HARD, HatchHardConfig.EGG_SORT_ORDER);

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}

using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CompressedCritters.Critters.Hatches
{
    [EntityConfigOrder(1)]
    internal class CompressedHatchConfig : IEntityConfig
    {
        public static string ID = "CC_Compressed" + HatchConfig.ID;
        

        public static GameObject CreateHatch(
            string id,
            string name,
            string desc,
            string anim_file,
            bool is_baby)
        {
            GameObject wildCreature = EntityTemplates.ExtendEntityToWildCreature(BaseHatchConfig.BaseHatch(id, name, desc, anim_file, ID + "BaseTrait", is_baby), CompressedHatchTuning.PEN_SIZE_PER_CREATURE);
            ModAssets.MultiplyButcherable(wildCreature, Mathf.RoundToInt(CompressedHatchTuning.SIZEMULTIPLIER));
            Trait trait = Db.Get().CreateTrait(ID + "BaseTrait", name, name, null, false, null, true, true);
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, CompressedHatchTuning.STANDARD_STOMACH_SIZE, name));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Calories.deltaAttribute.Id, -CompressedHatchTuning.STANDARD_CALORIES_PER_CYCLE / 600.0f, (string)global::STRINGS.UI.TOOLTIPS.BASE_VALUE));
            trait.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, 25f, name));
            trait.Add(new AttributeModifier(Db.Get().Amounts.Age.maxAttribute.Id, 100f, name));
            List<Diet.Info> diet_infos = BaseHatchConfig.BasicRockDiet(SimHashes.Carbon.CreateTag(), HatchConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.NORMAL, null, 0.0f);
            diet_infos.AddRange(BaseHatchConfig.FoodDiet(SimHashes.Carbon.CreateTag(), HatchConfig.CALORIES_PER_KG_OF_ORE, TUNING.CREATURES.CONVERSION_EFFICIENCY.GOOD_1, null, 0.0f));
            GameObject go = BaseHatchConfig.SetupDiet(wildCreature, diet_infos, HatchConfig.CALORIES_PER_KG_OF_ORE, HatchConfig.MIN_POOP_SIZE_IN_KG);
            go.AddTag(GameTags.OriginalCreature);
            return go;
        }
        public GameObject CreatePrefab() => EntityTemplates.ExtendEntityToFertileCreature(CreateHatch(ID, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.NAME, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.DESC, "hatch_kanim", false), ID + "Egg", STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.EGG_NAME, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.DESC, "egg_hatch_kanim", CompressedHatchTuning.EGG_MASS, "HatchBaby", 60f, 20f, CompressedHatchTuning.EGG_CHANCES_BASE, HatchConfig.EGG_SORT_ORDER);

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}

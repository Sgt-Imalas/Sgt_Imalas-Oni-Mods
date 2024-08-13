using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;


        public void OnPrefabInit(GameObject inst)
        {
            ;
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}

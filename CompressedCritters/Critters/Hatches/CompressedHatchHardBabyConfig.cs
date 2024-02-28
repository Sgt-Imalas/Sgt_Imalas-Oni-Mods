using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CompressedCritters.Critters.Hatches
{
    [EntityConfigOrder(2)]
    internal class CompressedHatchHardBabyConfig : IEntityConfig
    {
        public static string ID = CompressedHatchHardConfig.ID + "Baby";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            GameObject hatch = CompressedHatchHardConfig.CreateHatch(ID, (string)STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.BABY.NAME, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_HARD.BABY.DESC, "baby_hatch_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(hatch, CompressedHatchHardConfig.ID);
            return hatch;
        }

        public void OnPrefabInit(GameObject prefab)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }
    }
}

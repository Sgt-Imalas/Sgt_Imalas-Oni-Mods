using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CompressedCritters.Critters.Hatches
{
    [EntityConfigOrder(2)]
    internal class CompressedHatchVeggyBabyConfig : IEntityConfig
    {
        public static string ID = CompressedHatchVeggyConfig.ID + "Baby";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            GameObject hatch = CompressedHatchVeggyConfig.CreateHatch(ID, (string)STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.BABY.NAME, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.BABY.DESC, "baby_hatch_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(hatch, CompressedHatchVeggyConfig.ID);
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

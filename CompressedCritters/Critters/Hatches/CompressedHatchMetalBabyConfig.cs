using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CompressedCritters.Critters.Hatches
{
    [EntityConfigOrder(2)]
    internal class CompressedHatchMetalBabyConfig : IEntityConfig
    {
        public static string ID = CompressedHatchMetalConfig.ID + "Baby";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            GameObject hatch = CompressedHatchMetalConfig.CreateHatch(ID, (string)STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.BABY.NAME, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.VARIANT_METAL.BABY.DESC, "baby_hatch_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(hatch, CompressedHatchMetalConfig.ID);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CompressedCritters.Critters.Hatches
{
    [EntityConfigOrder(2)]
    internal class CompressedHatchBabyConfig : IEntityConfig
    {
        public static string ID = CompressedHatchConfig.ID + "Baby";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            GameObject hatch = CompressedHatchConfig.CreateHatch(ID, (string)STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.BABY.NAME, STRINGS.COMPRESSEDCRITTER + global::STRINGS.CREATURES.SPECIES.HATCH.BABY.DESC, "baby_hatch_kanim", true);
            EntityTemplates.ExtendEntityToBeingABaby(hatch, CompressedHatchConfig.ID);
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

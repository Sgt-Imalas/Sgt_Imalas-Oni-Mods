using OniRetroEdition.ModPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.SlurpTool
{
    internal class SlurpPlacerConfig : CommonPlacerConfig, IEntityConfig
    {
        public static string ID = "SlurpPlacer";
        static Material slurpPlacerMaterial;
        public GameObject CreatePrefab()
        {
            slurpPlacerMaterial = new Material(Assets.instance.mopPlacerAssets.material);
            slurpPlacerMaterial.mainTexture = Assets.GetSprite(SpritePatch.SlurpIcon).texture;

            GameObject prefab = this.CreatePrefab(SlurpPlacerConfig.ID, STRINGS.MISC.PLACERS.SLURPPLACER.NAME, slurpPlacerMaterial);
            prefab.AddTag(GameTags.NotConversationTopic);
            Slurpable moppable = prefab.AddOrGet<Slurpable>();
            moppable.synchronizeAnims = false;
            moppable.amountMoppedPerTick = 20f;
            prefab.AddOrGet<Cancellable>();
            return prefab;
        }

        public void OnPrefabInit(GameObject go)
        {
        }

        public void OnSpawn(GameObject go)
        {
        }
    }
}

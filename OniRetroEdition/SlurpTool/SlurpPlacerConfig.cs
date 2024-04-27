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

        public GameObject CreatePrefab()
        {
            GameObject prefab = this.CreatePrefab(SlurpPlacerConfig.ID, STRINGS.MISC.PLACERS.SLURPPLACER.NAME, Assets.instance.mopPlacerAssets.material);
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

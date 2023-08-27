using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Imalas_TwitchChaosEvents.Creeper
{
    internal class ITCE_CreepyLiquid : IOreConfig
    {
        public SimHashes ElementID => ModElements.Creeper.SimHash;

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

        public GameObject CreatePrefab()
        {
            GameObject liquidOreEntity = EntityTemplates.CreateLiquidOreEntity(this.ElementID, new List<Tag>() { ExtraTags.OniTwitchSurpriseBoxForceDisabled });
            Sublimates sublimates = liquidOreEntity.AddOrGet<Sublimates>();
            sublimates.spawnFXHash = SpawnFXHashes.BuildingLeakLiquid;
            sublimates.decayStorage = true;
            sublimates.info = new Sublimates.Info(5f, 0.005f, 4000f, 0f, this.ElementID);
            return liquidOreEntity;
        }
    }
}


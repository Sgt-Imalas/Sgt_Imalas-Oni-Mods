using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Rockets_TinyYetBig.SpaceStations.Construction
{
    internal class SpaceConstructionSiteConfig : IEntityConfig
    {
        public const string ID = "RTB_StationConstructionSite";

        public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

        public GameObject CreatePrefab()
        {
            var enity = EntityTemplates.CreateEntity(
                   id: ID,
                   name: "Station construction site",
                   true);
            enity.AddOrGet<CharacterOverlay>().shouldShowName = true;


            enity.AddOrGet<Storage>();
            var site = enity.AddOrGet<SpaceConstructionSite>();
            

            return enity;
        }

        public void OnPrefabInit(GameObject inst)
        {
        }

        public void OnSpawn(GameObject inst)
        {
        }

    }
}

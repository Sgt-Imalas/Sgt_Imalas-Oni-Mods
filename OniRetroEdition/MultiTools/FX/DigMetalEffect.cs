using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.MultiTools.FX
{
    internal class DigMetalEffect : IEntityConfig
    {
        public const string ID = "DigMetalFx";

        public GameObject CreatePrefab()
        {
            var go = EntityTemplates.CreateEntity(ID, ID, false);
            var kbac = go.AddOrGet<KBatchedAnimController>();
            kbac.AnimFiles = new KAnimFile[] { Assets.GetAnim("dig_metal_kanim") };
            kbac.materialType = KAnimBatchGroup.MaterialType.Simple;
            kbac.initialAnim = "loop";
            kbac.initialMode = KAnim.PlayMode.Loop;
            kbac.isMovable = true;
            kbac.destroyOnAnimComplete = false;
            //kbac.TintColour = ElementLoader.FindElementByHash(SimHashes.Petroleum).substance.colour;
            go.AddOrGet<LoopingSounds>();
            return go;
        }

        public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;
        public void OnPrefabInit(GameObject inst) { }
        public void OnSpawn(GameObject inst) { }
    }
}

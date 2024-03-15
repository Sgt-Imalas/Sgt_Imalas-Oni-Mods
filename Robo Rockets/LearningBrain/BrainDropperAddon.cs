using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.BUILDINGS.PREFABS;
using static UnityEngine.UI.Image;
using UnityEngine;
using KSerialization;

namespace RoboRockets.LearningBrain
{
    internal class BrainDropperAddon:KMonoBehaviour
    {
        [Serialize]
        public bool BrainDropsResources = false;

        public override void OnCleanUp()
        {
            if (gameObject.GetComponent("ObjectCanMove"))
                return;
               

            GameObject go = Util.KInstantiate(Assets.GetPrefab(BrainConfig.ID));
            if(go.TryGetComponent<DemolishableDroppable>(out var dropper))
            {
                dropper.ShouldDrop = BrainDropsResources;
            }

            go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore));
            go.SetActive(true);
            base.OnCleanUp();
        }
    }
}

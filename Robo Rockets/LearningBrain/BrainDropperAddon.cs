using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static STRINGS.BUILDINGS.PREFABS;
using static UnityEngine.UI.Image;
using UnityEngine;

namespace RoboRockets.LearningBrain
{
    internal class BrainDropperAddon:KMonoBehaviour
    {
        public override void OnCleanUp()
        {
            GameObject go = Util.KInstantiate(Assets.GetPrefab(BrainConfig.ID));
            go.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore));
            go.SetActive(true);
            base.OnCleanUp();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _WorldGenStateCapture.WorldStateData.WorldPOIs
{
    internal class POITracker:KMonoBehaviour
    {
        public string targetId;
        public override void OnSpawn()
        {
            base.OnSpawn();
            var position = this.transform.GetPosition();

            var myWorld = this.GetMyWorld();
            if(!ModAssets.currentPOIs.ContainsKey(myWorld))
                ModAssets.currentPOIs[myWorld] = new List<MapPOI>();

            ModAssets.currentPOIs[myWorld].Add(new MapPOI()
            {
                Id = targetId,
                PosX = (int)position.x,
                PosY = (int)position.y,
            });
        }
    }
}

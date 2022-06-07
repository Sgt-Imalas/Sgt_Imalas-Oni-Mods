using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CannedFoods.Foods
{
    class CanRecycler : KMonoBehaviour
    {
        //drop the material used for the can on eat :D
        protected override void OnCleanUp() 
        {

            var element = ElementLoader.FindElementByHash(SimHashes.Copper);
            var temperature = gameObject.GetComponent<PrimaryElement>().Temperature;
            var pos = Grid.CellToPosCCC(Grid.PosToCell(gameObject.transform.GetPosition()), Grid.SceneLayer.Ore);
            element.substance.SpawnResource(pos, 0.5f, temperature, 0, 0);
            base.OnCleanUp();
        }
    }
}

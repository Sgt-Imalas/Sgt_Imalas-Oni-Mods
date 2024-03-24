using Cheese.ModElements;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cheese.Foods
{
    internal class CheeseEncrustable : KMonoBehaviour, ISim1000ms
    {
        public static readonly float EncrustingMass = 10f;

        [Serialize]
        bool cheeseEncrusted = false;
        public bool CheeseEncrusted => cheeseEncrusted;

        public void Sim1000ms(float dt)
        {
            if(cheeseEncrusted) { return; }


            var currentPos = (Grid.PosToCell(this));
            if(    Grid.IsLiquid(currentPos) 
                && Grid.Element[currentPos].id == ModElementRegistration.CheeseMolten.SimHash
                && Grid.Mass[currentPos] >= EncrustingMass
                )
            {
                HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add((mass_cb_info, data) =>
                {
                    if ((double)mass_cb_info.mass <= 0.0)
                        return;
                    cheeseEncrusted = true;
                }, (object)null, "Cheese encrusted");
                SimMessages.ConsumeMass(currentPos, ModElementRegistration.CheeseMolten.SimHash, EncrustingMass, (byte)1, handle.index);
            }
        }
    }
}

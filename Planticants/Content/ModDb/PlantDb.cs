using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planticants.Content.ModDb
{
    class PlantDb
    {
        public static void Init(Db db)
        {
            PlantAccessories.Register(db.Accessories, db.AccessorySlots);
			PlantPersonalities.RegisterPersonalities(db.Personalities);

            PLANT_TUNING.RegisterType();
		}
    }
}

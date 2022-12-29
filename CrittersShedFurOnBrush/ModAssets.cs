
using Klei.AI;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CrittersShedFurOnBrush
{
    public class ModAssets
    {
        /// <summary>
        /// Creature + Shed amount / cycle
        /// </summary>
        public static Dictionary<Tag, float> SheddableCritters = new Dictionary<Tag, float>();
        public static void InitSheddables()
        {
            if(Config.Instance.Drecko)
                SheddableCritters.Add((Tag)DreckoConfig.ID, 1f / 8f );

            if (Config.Instance.CuddlePip)
                SheddableCritters.Add((Tag)SquirrelHugConfig.ID, 1f / 5f);

            if (Config.Instance.SageHatch)
                SheddableCritters.Add((Tag)(Tag)HatchVeggieConfig.ID, 1f/10f);

            if (Config.Instance.OilFloaterFur)
                SheddableCritters.Add((Tag)OilFloaterDecorConfig.ID, 1f / 6f);
        }
    }
}

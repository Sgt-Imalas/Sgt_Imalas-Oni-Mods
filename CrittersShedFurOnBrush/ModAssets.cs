
using Klei.AI;
using MonoMod.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace CrittersShedFurOnBrush
{
    public class ModAssets
    {
        /// <summary>
        /// Creature + Shed amount / cycle
        /// </summary>
        public static Dictionary<Tag,FloofCarrier> SheddableCritters = new Dictionary<Tag, FloofCarrier>();
        public static void InitSheddables()
        {
            if(Config.Instance.Drecko)
                AddFluffyCritter((Tag)DreckoConfig.ID, 1f / 8f );

            if (Config.Instance.CuddlePip)
                AddFluffyCritter((Tag)SquirrelHugConfig.ID, 1f / 5f, UIUtils.rgb(254, 193, 173));

            if (Config.Instance.SageHatch)
                AddFluffyCritter((Tag)(Tag)HatchVeggieConfig.ID, 1f/10f, UIUtils.rgb(76, 129, 103));

            if (Config.Instance.OilFloaterFur)
                AddFluffyCritter((Tag)OilFloaterDecorConfig.ID, 1f / 6f, UIUtils.rgb(86, 102, 208));
        }
        public static void AddFluffyCritter(Tag critterId, float floofPerCycle) => AddFluffyCritter(critterId, floofPerCycle, Color.white);
        public static void AddFluffyCritter(Tag critterId, float floofPerCycle, Color floofColour)
        {
            var FloofInfo = new FloofCarrier(critterId, floofPerCycle, floofColour);
            SheddableCritters.Add(critterId,FloofInfo);

        }
        public struct FloofCarrier
        {
            public Tag ID;
            public float FloofPerCycle;
            public Color FloofColour;
            public FloofCarrier(Tag _id, float _floofPerCycle, Color _floofColor)
            {
                ID = _id;  
                FloofPerCycle = _floofPerCycle;
                FloofColour = _floofColor;
            }
        }
    }
}

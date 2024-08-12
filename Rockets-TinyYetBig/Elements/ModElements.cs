using ElementUtilNamespace;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.ELEMENTS;

namespace Rockets_TinyYetBig.Elements
{
    public class ModElements
    {
        public static ElementInfo
            UnobtaniumDust,
            SpaceStationForceField,
            UnobtaniumAlloy;

        public static void RegisterSubstances(List<Substance> list, ref System.Collections.Hashtable substanceList)
        {
            //var gem = list.Find(e => e.elementID == SimHashes.Diamond).material;
            var refined = list.Find(e => e.elementID == SimHashes.Steel).material;
            var glass = list.Find(e => e.elementID == SimHashes.Diamond).material;

            UnobtaniumDust = ElementInfo.Solid("UnobtaniumDust", Color.black);
            UnobtaniumAlloy = ElementInfo.Solid("UnobtaniumAlloy", Color.grey);
            SpaceStationForceField = ElementInfo.Solid("SpaceStationForceField", Color.blue);

            var dustSubstance = UnobtaniumDust.CreateSubstance();
            dustSubstance.idx = substanceList.Count;
            substanceList.Add(UnobtaniumDust.SimHash, dustSubstance);

            var forceFieldSubstance = SpaceStationForceField.CreateSubstance(true, glass);
            forceFieldSubstance.idx = substanceList.Count;
            substanceList.Add(SpaceStationForceField.SimHash, forceFieldSubstance);

            var alloySubstance = UnobtaniumAlloy.CreateSubstance(true, glass);
            alloySubstance.idx = substanceList.Count;
            substanceList.Add(UnobtaniumAlloy.SimHash, alloySubstance);

            var newElements = new HashSet<Substance>()
            {
                dustSubstance, forceFieldSubstance, alloySubstance
            };
            list.AddRange(newElements);

            //SgtLogger.debuglog("2," + list + ", " + list.Count);

        }
    }
}

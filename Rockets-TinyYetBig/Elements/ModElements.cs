using ElementUtilNamespace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Elements
{
    internal class ModElements
    {
        public static ElementInfo
            UnobtaniumDust,
            UnobtaniumAlloy;

        public static void RegisterSubstances(List<Substance> list)
        {
            //var gem = list.Find(e => e.elementID == SimHashes.Diamond).material;
            var refined = list.Find(e => e.elementID == SimHashes.Steel).material;

            UnobtaniumDust = ElementInfo.Solid("UnobtaniumDust", Color.black);
            UnobtaniumAlloy = ElementInfo.Solid("UnobtaniumAlloy", Color.grey);
            SgtLogger.debuglog("1," + list + ", " + list.Count);
            var newElements = new HashSet<Substance>()
            {
                UnobtaniumDust.CreateSubstance(),
                UnobtaniumAlloy.CreateSubstance(true, refined)
            };
            list.AddRange(newElements);
            SgtLogger.debuglog("2," + list + ", " + list.Count);

        }
    }
}

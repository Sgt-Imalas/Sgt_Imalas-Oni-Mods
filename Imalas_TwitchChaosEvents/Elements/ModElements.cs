using ElementUtilNamespace;
using ONITwitchLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Imalas_TwitchChaosEvents.Elements
{
    public class ModElements
    {
        public static ElementInfo
            InverseIce,
            InverseWater,
            InverseSteam;

        public static void RegisterSubstances(List<Substance> list)
        {
            //var gem = list.Find(e => e.elementID == SimHashes.Diamond).material;
            var water = list.Find(e => e.elementID == SimHashes.Water);
            var ice = list.Find(e => e.elementID == SimHashes.Ice);
            var steam = list.Find(e => e.elementID == SimHashes.Steam);
            Color.RGBToHSV(steam.colour, out var steamH, out var steamS, out var steamV);
            Color.RGBToHSV(ice.colour, out var iceH, out var iceS, out var iceV);
            Color.RGBToHSV(water.colour, out var waterH, out var waterS, out var waterV);

            steamH = steamH + 0.5f % 1f;
            waterH = waterH + 0.5f % 1f;
            iceH = iceH + 0.5f % 1f;

            

            InverseIce = ElementInfo.Solid("ITCE_Inverse_Ice", Color.HSVToRGB(iceH, iceS,iceV));
            InverseWater = ElementInfo.Liquid("ITCE_Inverse_Water", Color.HSVToRGB(waterH, waterS,waterV));
            InverseSteam = ElementInfo.Gas("ITCE_Inverse_Steam", Color.HSVToRGB(steamH, steamS,steamV));


            var newElements = new HashSet<Substance>()
            {
                InverseIce.CreateSubstance(),
                InverseWater.CreateSubstance(),
                InverseSteam.CreateSubstance()
            };
            list.AddRange(newElements);

        }
    }
}

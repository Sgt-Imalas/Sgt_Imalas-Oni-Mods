using HarmonyLib;
using LogicSatelites.Buildings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace LogicSatelites
{
    class Patches
    {
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                InjectionMethods.AddBuildingStrings(SateliteCarrierModuleConfig.ID, SateliteCarrierModuleConfig.Name);
                RocketryUtils.AddRocketModuleToBuildList(SateliteCarrierModuleConfig.ID);
            }
        }
    }
}

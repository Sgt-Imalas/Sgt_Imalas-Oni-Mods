using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.Behaviors
{
    internal class OneRecipeOnlyPatches
    {
        //[HarmonyPatch(typeof(ComplexFabricatorSideScreen), "AnyRecipeRequirementsDiscovered")]
        //public class ComplexFabricatorSideScreen_AnyRecipeRequirementsDiscovered_Patch
        //{
        //    public static void Postfix(ComplexRecipe recipe, ref bool __result, ComplexFabricator ___targetFab)
        //    {
        //       if(___targetFab is ComplexFabricator)
        //        {
        //            if(___targetFab.recipeQueueCounts.Values.Any(count => count > 0))
        //            {
        //                __result = false;
        //            }
        //        }
        //    }
            
        //}
        //[HarmonyPatch(typeof(ComplexFabricatorSideScreen), "RefreshQueueCountDisplay")]
        //public class ComplexFabricatorSideScreen_RefreshOnCountChange
        //{
        //    public static void Postfix(GameObject entryGO, ComplexFabricator fabricator, ComplexFabricator ___targetFab, Dictionary<GameObject, ComplexRecipe> ___recipeMap)
        //    {
        //        if (___targetFab is ComplexFabricator)//change to custom fabricator type
        //        {

        //            bool hasRecipeQueued = fabricator.GetRecipeQueueCount(___recipeMap[entryGO]) != 0; //1-99 is amount queued, -1 is infinite queued
                    
        //            foreach(var entry in ___recipeMap) 
        //            { 
        //                if(hasRecipeQueued)
        //                {
        //                    if(entry.Key == entryGO)
        //                    {
        //                        entry.Key.SetActive(true);
        //                    }
        //                    else
        //                    {
        //                        entry.Key.SetActive(false);
        //                    }
        //                }
        //                else
        //                {
        //                    entry.Key.SetActive(true);
        //                }
        //            }
        //        }
        //    }
        //}
    }
}

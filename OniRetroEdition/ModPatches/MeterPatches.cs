using HarmonyLib;
using OniRetroEdition.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace OniRetroEdition.ModPatches
{
    internal class MeterPatches
    {
        [HarmonyPatch]
        public static class AddStorageMeters
        {
            [HarmonyPostfix]
            public static void Postfix(GameObject go)
            {
                go.AddOrGet<StoragesMeter>();
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
                yield return typeof(FarmStationConfig).GetMethod(name);
                yield return typeof(CompostConfig).GetMethod(name);
            }            
        }
    }
}

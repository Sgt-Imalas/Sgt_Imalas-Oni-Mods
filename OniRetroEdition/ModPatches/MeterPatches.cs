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
                var meter = go.AddOrGet<StoragesMeter>();
                meter.inFront = go.PrefabID().Name.Contains("FarmStation");
                if(go.TryGetComponent<ManualDeliveryKG>(out var manualDelivery))
                {
                    meter.MaxMassOverride = manualDelivery.capacity;
                }
            }
            [HarmonyTargetMethods]
            internal static IEnumerable<MethodBase> TargetMethods()
            {
                const string name = nameof(IBuildingConfig.DoPostConfigureComplete);
                yield return typeof(FarmStationConfig).GetMethod(name);
                yield return typeof(CompostConfig).GetMethod(name);
            }
        }
        //[HarmonyPatch(typeof(FarmStationConfig), nameof(FarmStationConfig.DoPostConfigureComplete))]
        //public static class AddStorageMeter_FarmStationConfig
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(GameObject go)
        //    {
        //        var meter = go.AddOrGet<GenericStorageMeter>();
        //        meter.inFront = false;
        //        meter.storage = go.GetComponent<Storage>();
        //        meter.maxValueOverride = 300;
        //    }
        //}
        //[HarmonyPatch(typeof(CompostConfig), nameof(CompostConfig.DoPostConfigureComplete))]
        //public static class AddStorageMeter_Compost
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(GameObject go)
        //    {
        //        var meter = go.AddOrGet<GenericStorageMeter>();
        //        meter.inFront = false;
        //        meter.storage = go.GetComponent<Storage>();
        //        meter.maxValueOverride = 300;
        //    }
        //}
    }
}

using Database;
using FMOD;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static LaserMeteorBlasterCannon.ModAssets;
using static STRINGS.UI.CLUSTERMAP;
using static UnityEngine.UI.Image;

namespace LaserMeteorBlasterCannon
{
    internal class Patches
    {
        /// <summary>
        /// add buildings to plan screen
        /// </summary>
        [HarmonyPatch(typeof(GeneratedBuildings))]
        [HarmonyPatch(nameof(GeneratedBuildings.LoadGeneratedBuildings))]
        public static class GeneratedBuildings_LoadGeneratedBuildings_Patch
        {

            public static void Prefix()
            {
                //ModUtil.AddBuildingToPlanScreen(GameStrings.PlanMenuCategory.XXXX, XXXX.ID);
            }
        }
        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Localization), "Initialize")]
        public static class Localization_Initialize_Patch
        {
            public static void Postfix()
            {
                LocalisationUtil.Translate(typeof(STRINGS), true);
            }
        }


        /// <summary>
        /// Init. auto translation
        /// </summary>
        [HarmonyPatch(typeof(Comet))]
        [HarmonyPatch(nameof(Comet.OnCleanUp))]
        public static class Yeet_Materials
        {
            public static Vector3 GetPointOnUnitSphereCap(Quaternion targetDirection, float angle)
            {
                var angleInRad = UnityEngine.Random.Range(0.0f, angle) * Mathf.Deg2Rad;
                var PointOnCircle = (UnityEngine.Random.insideUnitCircle.normalized) * Mathf.Sin(angleInRad);
                var V = new Vector3(PointOnCircle.x, PointOnCircle.y, Mathf.Cos(angleInRad));
                return targetDirection * V;
            }
            public static Vector3 GetPointOnUnitSphereCap(Vector3 targetDirection, float angle)
            {
                return GetPointOnUnitSphereCap(Quaternion.LookRotation(targetDirection), angle);
            }

            public static void Prefix(MissileProjectile.StatesInstance __instance)
            {
                if (!__instance.smi.sm.meteorTarget.IsNullOrDestroyed())
                {
                    Comet CometToDropMats = __instance.smi.sm.meteorTarget.Get(__instance.smi);

                    if (CometToDropMats.TryGetComponent<PrimaryElement>(out var primElement))
                    {

                        SgtLogger.l(primElement.Element.ToString() + ", " + primElement.ElementID.ToString());

                        SgtLogger.l(primElement.Mass.ToString() + ", " + primElement.Temperature.ToString());
                        SgtLogger.l(CometToDropMats.explosionMass.ToString() + "<- expl. mass, tile mass ->" + CometToDropMats.addTileMass.ToString());


                        int numberOfSplinters = UnityEngine.Random.Range(CometToDropMats.explosionOreCount.x, CometToDropMats.explosionOreCount.y + 1) * 2;
                        float TotalMeteorMass = CometToDropMats.explosionMass + CometToDropMats.addTileMass;
                        float temperature = UnityEngine.Random.Range(CometToDropMats.explosionTemperatureRange.x, CometToDropMats.explosionTemperatureRange.y);
                        bool isMooComet = false;

                        if(CometToDropMats is GassyMooComet)
                        {
                            TotalMeteorMass = 10f;
                            numberOfSplinters = UnityEngine.Random.Range(7, 16);
                            isMooComet = true;
                        }


                        if (numberOfSplinters == 0)
                            numberOfSplinters = Mathf.Min(Mathf.RoundToInt(TotalMeteorMass / 0.2f), 14);

                        
                        TotalMeteorMass *= 0.5f;

                        float SplinterMass = TotalMeteorMass / numberOfSplinters ;


                        for (int splinterIndex = 0; splinterIndex < numberOfSplinters; splinterIndex++)
                        {
                            float speed = CometToDropMats.velocity.magnitude;
                            Vector3 randomizedDirection = GetPointOnUnitSphereCap((Vector3)CometToDropMats.velocity, 45f)* speed;
                            GameObject splinter = null;

                            if(primElement.ElementID != SimHashes.Creature)
                            {
                                splinter = primElement.Element.substance.SpawnResource(CometToDropMats.previousPosition, SplinterMass, temperature, primElement.DiseaseIdx, disease_count: Mathf.RoundToInt(primElement.DiseaseCount / SplinterMass));
                            }
                            else if(isMooComet)
                            {
                                splinter = Util.KInstantiate(Assets.GetPrefab(MeatConfig.ID), CometToDropMats.previousPosition);
                                splinter.SetActive(true);
                                //splinter.transform.SetPosition();

                                SgtLogger.l(splinter.transform.position.ToString());

                                if (splinter.TryGetComponent<PrimaryElement>(out var meat))
                                {
                                    meat.Temperature = temperature;
                                    meat.Mass = SplinterMass;

                                }
                            }
                            if (splinter != null)
                            {
                                if (GameComps.Fallers.Has(splinter))
                                {
                                    GameComps.Fallers.Remove(splinter);
                                }

                                GameComps.Fallers.Add(splinter, randomizedDirection);

                            }
                        }
                    }
                }
            }
        }
        }
}

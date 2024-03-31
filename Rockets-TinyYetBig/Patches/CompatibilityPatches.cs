using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static AttackProperties;

namespace Rockets_TinyYetBig.Patches
{
    internal class CompatibilityPatches
    {
        public class Hydrocarbon_Rocket_Engines
        {
            public static void ExecutePatch(Harmony harmony)
            {
                //add fuel tags to elements
                var m_TargetMethod = AccessTools.Method(typeof(ElementLoader), nameof(ElementLoader.Load));
                var m_postfix = AccessTools.Method(typeof(Hydrocarbon_Rocket_Engines), nameof(Hydrocarbon_Rocket_Engines.AddFuelTagsToElements));
                harmony.Patch(m_TargetMethod, postfix: new HarmonyMethod(m_postfix, Priority.LowerThanNormal));

                //disable that stupid prefix skip patch
                var parentType = AccessTools.TypeByName("HydrocarbonRocketEngines.HydrocarbonRocketEnginesPatches");
                if (parentType == null)
                    return;

                var m_TargetType_1 = parentType.GetNestedType("ClusterCraftPatches");
                var m_TargetType_2 = parentType.GetNestedType("CraftModuleInterfacePatches");

                if (m_TargetType_1 != null)
                {
                    m_TargetMethod = AccessTools.Method(m_TargetType_1, "BurnFuelForTravel");
                    if (m_TargetMethod == null)
                    {
                        SgtLogger.warning("HydrocarbonRocketEngines mod target method ClusterCraftPatches not found on type HydrocarbonRocketEngines.HydrocarbonRocketEnginesPatches");
                        return;
                    }                                        
                    var methodToUnpatch = AccessTools.Method(typeof(Clustercraft), nameof(Clustercraft.BurnFuelForTravel));
                    //removing the prefix skip patch
                    harmony.Unpatch(methodToUnpatch, HarmonyPatchType.Prefix, "TC-1000's:Hydrocarbon_Rocket_Engines");

                    //Using transpiler for logic instead
                    harmony.Patch(AccessTools.Method(typeof(Clustercraft), nameof(Clustercraft.BurnFromTank)),
                        new HarmonyMethod(AccessTools.Method(typeof(Hydrocarbon_Rocket_Engines), nameof(BurnFromTankPrefix))),
                        new HarmonyMethod(AccessTools.Method(typeof(Hydrocarbon_Rocket_Engines), nameof(BurnFromTankPostfix))),
                        new HarmonyMethod(AccessTools.Method(typeof(Hydrocarbon_Rocket_Engines), nameof(BurnFromTankTranspiler))));                        
                }
                else
                {
                    SgtLogger.l("HydrocarbonRocketEngines mod target type HydrocarbonRocketEngines.HydrocarbonRocketEnginesPatches not found.");
                }

                if (m_TargetType_2 != null)
                {
                    m_TargetMethod = AccessTools.Method(m_TargetType_2, "BurnFuelForTravel");
                    if (m_TargetMethod == null)
                    {
                        SgtLogger.warning("HydrocarbonRocketEngines mod target method CraftModuleInterfacePatches not found on type HydrocarbonRocketEngines.HydrocarbonRocketEnginesPatches");
                        return;
                    }
                    SgtLogger.l("disabling that prefix skip method for fuel remaining");
                    var methodToUnpatch = AccessTools.Method(typeof(CraftModuleInterface), "get_FuelRemaining");
                    //removing the prefix skip patch
                    harmony.Unpatch(methodToUnpatch, HarmonyPatchType.Prefix, "TC-1000's:Hydrocarbon_Rocket_Engines");
                    //injecting transpiler instead that does the same without skipping
                    harmony.Patch(AccessTools.Method(typeof(CraftModuleInterface), "get_FuelRemaining"), transpiler: new HarmonyMethod(AccessTools.Method(typeof(Hydrocarbon_Rocket_Engines),nameof(FuelRemainingTranspiler))));
                }

            }

            /// <summary>
            /// Transpiler to inject the check into the original getter method instead of rewriting it
            /// </summary>
            public static IEnumerable<CodeInstruction> FuelRemainingTranspiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                var IStorageGetAmountAvailable = AccessTools.Method(typeof(IStorage),nameof(IStorage.GetAmountAvailable));

                // find injection point
                var index = codes.FindIndex(ci => ci.Calls(IStorageGetAmountAvailable));

                if (index == -1)
                {
                    Debug.LogError("FuelRemainingTranspiler broke");
                    return codes;
                }

                var m_InjectedMethod = AccessTools.DeclaredMethod(typeof(Hydrocarbon_Rocket_Engines), "InjectedMethod");

                //replace
                codes[index] = CodeInstruction.Call(typeof(Hydrocarbon_Rocket_Engines), "GetEffectiveFuelTankCapacity");

                return codes;
            }

            /// <summary>
            /// Static vars to hold "local" values accessible in the transpiler
            /// </summary>
            static bool ConsumedLiquidMethane=false;
            static float ActualOxidizerConsumption = -1; 

            ///Prefix to initialize the "local vars"
            public static void BurnFromTankPrefix(Clustercraft __instance, RocketEngineCluster engine)
            {
                ConsumedLiquidMethane = false;
                ActualOxidizerConsumption = -1;
            }

            ///assign better engine power if Liquid Methane was used. get default value from def
            public static void BurnFromTankPostfix(Clustercraft __instance, RocketEngineCluster engine)
            {
                engine.TryGetComponent<Building>(out var building);
                engine.TryGetComponent<RocketModuleCluster>(out var moduleCluster);
                var defaultEnginePower = building.Def.BuildingComplete.GetComponent<RocketModuleCluster>().performanceStats.enginePower;
                moduleCluster.performanceStats.enginePower = ConsumedLiquidMethane ? defaultEnginePower * 1.4f : defaultEnginePower;
            }

            ///Transpiler replaces calls to reach the same method result as the original without the prefix skip
            public static IEnumerable<CodeInstruction> BurnFromTankTranspiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
            {
                var codes = orig.ToList();

                var IStorageConsumeIgnoringDisease = AccessTools.Method(typeof(IStorage), nameof(IStorage.ConsumeIgnoringDisease));
                var IStorageGetAmountAvailable = AccessTools.Method(typeof(IStorage), nameof(IStorage.GetAmountAvailable));
                //var ClustercraftBurnOxidizer = AccessTools.Method(typeof(Clustercraft), nameof(Clustercraft.BurnOxidizer));

                // find injection points
                var getAmountAvailableIndex = codes.FindIndex(ci => ci.Calls(IStorageGetAmountAvailable));
                var ConsumeIndex = codes.FindIndex(ci => ci.Calls(IStorageConsumeIgnoringDisease));
                //var burnOxidizerIndex = codes.FindIndex(ci => ci.Calls(ClustercraftBurnOxidizer));

                if (ConsumeIndex == -1)
                {
                    Debug.LogError("BurnFromTankTranspiler broke: IStorageConsumeIgnoringDisease not found");
                    return codes;
                }
                if (getAmountAvailableIndex == -1 )
                {
                    Debug.LogError("BurnFromTankTranspiler broke: GetAmountAvailable not found");
                    return codes;
                }
                //if (burnOxidizerIndex == -1)
                //{
                //    Debug.LogError("BurnFromTankTranspiler broke: ClustercraftBurnOxidizer not found");
                //    return codes;
                //}

                //replace
                codes[getAmountAvailableIndex] = CodeInstruction.Call(typeof(Hydrocarbon_Rocket_Engines), "GetEffectiveFuelTankCapacity");
                codes[ConsumeIndex] = CodeInstruction.Call(typeof(Hydrocarbon_Rocket_Engines), "ConsumeDifferentFuelElements");
                
                ///only use if oxidizer amount consumed is based on actual fuel consumption instead of effective fuel consumption (imo it should be based on effective fuel consumption, otherwise you multiply the 2 efficiencies)
                //codes.Insert(burnOxidizerIndex, CodeInstruction.Call(typeof(Hydrocarbon_Rocket_Engines), "ReplaceActualOxidizerConsumptionValue"));

                return codes;
            }

            static float ReplaceActualOxidizerConsumptionValue(float oldValue)
            {
                if(ActualOxidizerConsumption != -1)
                {
                    return ActualOxidizerConsumption;
                }
                return oldValue;
            }
            static void ConsumeDifferentFuelElements(IStorage storage, Tag fuelTag, float remainingAmountToConsume)
            {
                if(fuelTag != SimHashes.Petroleum.CreateTag() && fuelTag != GameTags.CombustibleLiquid)
                {
                    storage.ConsumeIgnoringDisease(fuelTag, remainingAmountToConsume);
                    return;
                }
                ActualOxidizerConsumption = 0;
                foreach (var potentialFuel in CombustibleElements)
                {
                    var effectivefuelAmount = storage.GetAmountAvailable(potentialFuel.first.CreateTag()) * potentialFuel.second;

                    var effectiveToBurn = Mathf.Min(remainingAmountToConsume, effectivefuelAmount);

                    var actualToBurn = effectiveToBurn / potentialFuel.second;
                    storage.ConsumeIgnoringDisease(potentialFuel.first.CreateTag(), actualToBurn);
                    

                    if (potentialFuel.first == SimHashes.LiquidMethane)
                        ConsumedLiquidMethane = true;

                    remainingAmountToConsume -= effectiveToBurn;
                    ActualOxidizerConsumption += actualToBurn;

                    if (remainingAmountToConsume <= 0)
                        break;
                }
            }

            static List<Tuple<SimHashes, float>> CombustibleElements;

            public static float EffectiveFuelAmount(IStorage storage)
            {
                float totalEffectiveFuelAmount = 0;
                foreach (var potentialFuel in CombustibleElements)
                {
                    totalEffectiveFuelAmount += storage.GetAmountAvailable(potentialFuel.first.CreateTag()) * potentialFuel.second;
                }
                return totalEffectiveFuelAmount;
            }

            public static float GetEffectiveFuelTankCapacity(IStorage fueltankStorage, Tag fuelTag)
            {
                if (CombustibleElements != null && (fuelTag == GameTags.CombustibleLiquid || fuelTag == SimHashes.Petroleum.CreateTag()))
                {
                    return EffectiveFuelAmount(fueltankStorage);
                }
                return fueltankStorage.GetAmountAvailable(fuelTag);
            }

            /// <summary>
            /// initializes Hydrocarbon Rocket fuel element efficiency list
            /// </summary>
            static void AddFuelTagsToElements()
            {
                AddMissingTags(ElementLoader.GetElement(SimHashes.Naphtha.CreateTag()));
                AddMissingTags(ElementLoader.GetElement(SimHashes.CrudeOil.CreateTag()));
                AddMissingTags(ElementLoader.GetElement(SimHashes.LiquidMethane.CreateTag()));

                CombustibleElements = new List<Tuple<SimHashes, float>>
                {
                    new Tuple<SimHashes, float> ( SimHashes.CrudeOil, 0.6f ),
                    new Tuple<SimHashes, float>( SimHashes.Ethanol, 0.8f ),
                    new Tuple<SimHashes, float> ( SimHashes.Petroleum, 1f),
                    new Tuple<SimHashes, float> ( SimHashes.Naphtha, 0.9f ),
                    new Tuple<SimHashes, float>( SimHashes.LiquidMethane, 1.4f)
                };
                CombustibleElements = CombustibleElements.OrderByDescending(itm => itm.second).ToList(); //consume higher value mats first
            }

            /// <summary>
            /// adds missing tags to fuel elements
            /// </summary>
            /// <param name="element"></param>
            static void AddMissingTags(Element element)
            {
                if (element.oreTags is null)
                {
                    element.oreTags = new Tag[] { };
                }
                element.oreTags = element.oreTags.Append(ModAssets.Tags.RocketFuelTag);
                element.oreTags = element.oreTags.Append(GameTags.CombustibleLiquid);
            }
        }
    }
}

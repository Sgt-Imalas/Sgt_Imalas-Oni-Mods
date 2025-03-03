using HarmonyLib;
using Rockets_TinyYetBig.SpaceStations;
using System.Collections.Generic;

namespace Rockets_TinyYetBig.Patches.SpaceStationPatches
{
    internal class StationDiagnosticPatches
    {
        [HarmonyPatch(typeof(ColonyDiagnosticUtility), nameof(ColonyDiagnosticUtility.AddWorld))]
        public static class AddSpaceStationDiagnostic
        {
            public static bool Prefix(int worldID, ColonyDiagnosticUtility __instance)
            {
                if (SpaceStationManager.WorldIsSpaceStationInterior(worldID))
                {
                    bool newlyAdded = false;
                    if (!__instance.diagnosticDisplaySettings.ContainsKey(worldID))
                    {
                        __instance.diagnosticDisplaySettings.Add(worldID, new Dictionary<string, ColonyDiagnosticUtility.DisplaySetting>());
                        newlyAdded = true;
                    }
                    List<ColonyDiagnostic> newWorldDiagnostics = new List<ColonyDiagnostic>();
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new BreathabilityDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new FoodDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new StressDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new RadiationDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new ReactorDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new IdleDiagnostic(worldID));

                    //__instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, (ColonyDiagnostic)new FloatingRocketDiagnostic(worldID));
                    //__instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, (ColonyDiagnostic)new RocketFuelDiagnostic(worldID));
                    //__instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, (ColonyDiagnostic)new RocketOxidizerDiagnostic(worldID));

                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new BedDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new ToiletDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new PowerUseDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new BatteryDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new TrappedDuplicantDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new FarmDiagnostic(worldID));
                    __instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, new EntombedDiagnostic(worldID));
                    //__instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, (ColonyDiagnostic)new RocketsInOrbitDiagnostic(worldID));
                    //__instance.TryAddDiagnosticToWorldCollection(ref newWorldDiagnostics, (ColonyDiagnostic)new MeteorDiagnostic(worldID));

                    __instance.worldDiagnostics.Add(worldID, newWorldDiagnostics);
                    foreach (ColonyDiagnostic colonyDiagnostic in newWorldDiagnostics)
                    {
                        if (!__instance.diagnosticDisplaySettings[worldID].ContainsKey(colonyDiagnostic.id))
                            __instance.diagnosticDisplaySettings[worldID].Add(colonyDiagnostic.id, ColonyDiagnosticUtility.DisplaySetting.AlertOnly);
                        if (!__instance.diagnosticCriteriaDisabled[worldID].ContainsKey(colonyDiagnostic.id))
                            __instance.diagnosticCriteriaDisabled[worldID].Add(colonyDiagnostic.id, new List<string>());
                    }

                    if (newlyAdded)
                    {
                        __instance.diagnosticDisplaySettings[worldID]["BreathabilityDiagnostic"] = ColonyDiagnosticUtility.DisplaySetting.Always;
                        __instance.diagnosticDisplaySettings[worldID]["FoodDiagnostic"] = ColonyDiagnosticUtility.DisplaySetting.Always;
                        __instance.diagnosticDisplaySettings[worldID]["StressDiagnostic"] = ColonyDiagnosticUtility.DisplaySetting.Always;
                        __instance.diagnosticDisplaySettings[worldID]["IdleDiagnostic"] = ColonyDiagnosticUtility.DisplaySetting.AlertOnly;
                    }
                    return false;
                }
                return true;
            }
        }
    }
}

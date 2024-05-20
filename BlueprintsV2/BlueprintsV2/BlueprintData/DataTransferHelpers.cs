using BlueprintsV2.BlueprintsV2.ModAPI;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static HoverTextDrawer;
using static ModInfo;
using static PixelPack;

namespace BlueprintsV2.BlueprintsV2.BlueprintData
{
    internal class DataTransferHelpers
    {

        internal class DataTransfer_LogicClusterLocationSensor
        {
            //active locations are disabled since those vary for each game
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<LogicClusterLocationSensor>(out var component))
                {
                    return new JObject()
                    {
                        //{ "activeLocations", JsonConvert.SerializeObject(component.activeLocations.Select(axial => new Tuple<int,int>(axial.Q,axial.R)))},
                        { "activeInSpace", component.activeInSpace},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<LogicClusterLocationSensor>(out var targetComponent))
                {

                    var t1 = jObject.GetValue("activeInSpace");
                    if (t1 == null)
                        return;
                    var activeInSpace = t1.Value<bool>();


                    //var t2 = jObject.GetValue("activeLocations");
                    //if (t2 == null)
                    //    return;
                    //var activeLocationsJson = t2.Value<string>();
                    //var activeLocations = JsonConvert.DeserializeObject<List<Tuple<int, int>>>(activeLocationsJson);

                    //applying values
                    targetComponent.activeLocations.Clear();
                    targetComponent.activeInSpace = activeInSpace;

                    //activeLocations.ForEach(entry => targetComponent.SetLocationEnabled(new(entry.first, entry.second), true));
                }
            }
        }
        internal class DataTransfer_PixelPack
        {
            class PixelPackColorData
            {
                public float r1, g1, b1, a1;
                public float r2, g2, b2, a2;

                public PixelPackColorData(Color a, Color b)
                {
                    r1 = a.r; g1 = a.g; b1 = a.b; a1 = a.a;

                    r2 = b.r; g2 = b.g; b2 = b.b; a2 = b.a;
                }
                public PixelPack.ColorPair GetData()
                {
                    return new PixelPack.ColorPair()
                    {
                        activeColor = new Color(r1, g1, b1, a1),
                        standbyColor = new Color(r2, g2, b2, a2)
                    };
                }
            }

            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<PixelPack>(out var component))
                {
                    PixelPackColorData[] transferedData = new PixelPackColorData[component.colorSettings.Count];
                    for (int i = 0; i < component.colorSettings.Count; ++i)
                    {
                        var col = component.colorSettings[i];
                        transferedData[i] = new PixelPackColorData(col.activeColor, col.standbyColor);
                    }
                    SgtLogger.l(JsonConvert.SerializeObject(transferedData));
                    return new JObject()
                    {
                        { "colorSettings", JsonConvert.SerializeObject(transferedData)},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<PixelPack>(out var targetComponent))
                {

                    var t1 = jObject.GetValue("colorSettings");
                    if (t1 == null)
                        return;
                    var colorSettingsJson = t1.Value<string>();
                    var colorSettings = JsonConvert.DeserializeObject<PixelPackColorData[]>(colorSettingsJson);

                    SgtLogger.l(colorSettingsJson);
                    //applying values
                    if (targetComponent.colorSettings == null)
                    {
                        //filling with temp empty items
                        var p1 = new PixelPack.ColorPair();
                        p1.activeColor = targetComponent.defaultActive;
                        p1.standbyColor = targetComponent.defaultStandby;
                        var p2 = p1;
                        var p3 = p1;
                        var p4 = p1;
                        targetComponent.colorSettings = new List<PixelPack.ColorPair>
                        {
                            p1,
                            p2,
                            p3,
                            p4
                        };
                    }


                    for (int index = 0; index < colorSettings.Length; ++index)
                    {
                        targetComponent.colorSettings[index] = colorSettings[index].GetData();
                    }
                    targetComponent.UpdateColors();
                }
            }
        }
        internal class DataTransfer_LogicTimeOfDaySensor
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<LogicTimeOfDaySensor>(out var component))
                {
                    return new JObject()
                    {
                        { "startTime", component.startTime},
                        { "duration", component.duration},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<LogicTimeOfDaySensor>(out var targetComponent))
                {

                    var t1 = jObject.GetValue("startTime");
                    if (t1 == null)
                        return;
                    var startTime = t1.Value<float>();


                    var t2 = jObject.GetValue("duration");
                    if (t2 == null)
                        return;
                    var duration = t2.Value<float>();

                    //applying values
                    targetComponent.startTime = startTime;
                    targetComponent.duration = duration;
                }
            }
        }

        internal class DataTransfer_TreeFilterable
        {
            //active locations are disabled since those vary for each game
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<TreeFilterable>(out var component))
                {
                    if (!component.copySettingsEnabled)
                        return null;
                    return new JObject()
                    {
                        //{ "activeLocations", JsonConvert.SerializeObject(component.activeLocations.Select(axial => new Tuple<int,int>(axial.Q,axial.R)))},
                        { "acceptedTagSet", JsonConvert.SerializeObject(component.GetTags())},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<TreeFilterable>(out var targetComponent))
                {
                    if (!targetComponent.copySettingsEnabled)
                        return;

                    var t1 = jObject.GetValue("acceptedTagSet");
                    if (t1 == null)
                        return;
                    var acceptedTagSetJson = t1.Value<string>();
                    var acceptedTagSet = JsonConvert.DeserializeObject<HashSet<Tag>>(acceptedTagSetJson);
                    targetComponent.UpdateFilters(acceptedTagSet);
                }
            }
        }
        internal class DataTransfer_Filterable
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<Filterable>(out var component))
                {
                    return new JObject()
                    {
                        { "SelectedTag", component.SelectedTag.ToString()}
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<Filterable>(out var targetComponent))
                {
                    var t1 = jObject.GetValue("SelectedTag");
                    if (t1 == null)
                        return;
                    var selectedTag = (Tag)t1.Value<string>();


                    //applying values
                    targetComponent.SelectedTag = selectedTag;
                }
            }
        }

        internal class DataTransfer_LogicTimerSensor
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<LogicTimerSensor>(out var sourceComponent))
                {
                    return new JObject()
                    {
                        { "onDuration", sourceComponent.onDuration},
                        { "offDuration", sourceComponent.offDuration},
                        { "timeElapsedInCurrentState", sourceComponent.timeElapsedInCurrentState},
                        { "displayCyclesMode", sourceComponent.displayCyclesMode},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<LogicTimerSensor>(out var targetComponent))
                {
                    var t1 = jObject.GetValue("onDuration");
                    if (t1 == null)
                        return;
                    var onDuration = t1.Value<float>();


                    var t2 = jObject.GetValue("offDuration");
                    if (t2 == null)
                        return;
                    var offDuration = t2.Value<float>();

                    var t3 = jObject.GetValue("timeElapsedInCurrentState");
                    if (t3 == null)
                        return;
                    var timeElapsedInCurrentState = t3.Value<float>();

                    var t4 = jObject.GetValue("displayCyclesMode");
                    if (t4 == null)
                        return;
                    var displayCyclesMode = t4.Value<bool>();

                    //applying values
                    targetComponent.onDuration = onDuration;
                    targetComponent.offDuration = offDuration;
                    targetComponent.timeElapsedInCurrentState = timeElapsedInCurrentState;
                    targetComponent.displayCyclesMode = displayCyclesMode;
                }
            }
        }

        internal class DataTransfer_LogicCritterCountSensor
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<LogicCritterCountSensor>(out var sourceComponent))
                {
                    return new JObject()
                    {
                        { "countThreshold", sourceComponent.countThreshold},
                        { "activateOnGreaterThan", sourceComponent.activateOnGreaterThan},
                        { "countCritters", sourceComponent.countCritters},
                        { "countEggs", sourceComponent.countEggs},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<LogicCritterCountSensor>(out var targetComponent))
                {
                    var t1 = jObject.GetValue("countThreshold");
                    if (t1 == null)
                        return;
                    var countThreshold = t1.Value<int>();


                    var t2 = jObject.GetValue("activateOnGreaterThan");
                    if (t2 == null)
                        return;
                    var activateAboveThreshold = t2.Value<bool>();

                    var t3 = jObject.GetValue("countCritters");
                    if (t3 == null)
                        return;
                    var countCritters = t3.Value<bool>();

                    var t4 = jObject.GetValue("countEggs");
                    if (t4 == null)
                        return;
                    var countEggs = t4.Value<bool>();

                    //applying values
                    targetComponent.countThreshold = countThreshold;
                    targetComponent.ActivateAboveThreshold = activateAboveThreshold;
                    targetComponent.countCritters = countCritters;
                    targetComponent.countEggs = countEggs;
                }
            }
        }

        internal class DataTransfer_GenericThresholdSensor<T>
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<T>(out var sourceComponent))
                {
                    bool wholeNumbers = Traverse.Create(sourceComponent).Property("Threshold").GetValueType() == typeof(int);

                    if (wholeNumbers)
                        return new JObject()
                    {
                        { "Threshold", (int)Traverse.Create(sourceComponent).Property("Threshold").GetValue()},
                        { "ActivateAboveThreshold", (bool)Traverse.Create(sourceComponent).Property("ActivateAboveThreshold").GetValue()}
                    };

                    else
                        return new JObject()
                    {
                        { "Threshold", (float)Traverse.Create(sourceComponent).Property("Threshold").GetValue()},
                        { "ActivateAboveThreshold", (bool)Traverse.Create(sourceComponent).Property("ActivateAboveThreshold").GetValue()}
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<T>(out var targetComponent))
                {
                    var t1 = jObject.GetValue("Threshold");
                    if (t1 == null)
                        return;
                    var Threshold = t1.Value<float>();


                    var t2 = jObject.GetValue("ActivateAboveThreshold");
                    if (t2 == null)
                        return;
                    var activateAboveThreshold = t2.Value<bool>();


                    //applying values
                    if (Traverse.Create(targetComponent).Property("Threshold").GetValueType() == typeof(int))
                        Traverse.Create(targetComponent).Property("Threshold").SetValue(Mathf.RoundToInt(Threshold));
                    else
                        Traverse.Create(targetComponent).Property("Threshold").SetValue(Threshold);

                    Traverse.Create(targetComponent).Property("ActivateAboveThreshold").SetValue(activateAboveThreshold);
                }
            }
        }

        internal class DataTransfer_GenericLogicGateDelay<T>
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<T>(out var sourceComponent))
                {
                    return new JObject()
                    {
                        { "DelayAmount", (float)Traverse.Create(sourceComponent).Property("DelayAmount").GetValue()}
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<T>(out var targetComponent))
                {
                    var t1 = jObject.GetValue("DelayAmount");
                    if (t1 == null)
                        return;
                    var DelayAmount = t1.Value<float>();

                    //applying values
                    Traverse.Create(targetComponent).Property("DelayAmount").SetValue(DelayAmount);
                }
            }
        }

        internal class DataTransfer_GenericRibbonData<T>
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<T>(out var sourceComponent))
                {
                    return new JObject()
                    {
                        { "selectedBit", (float)Traverse.Create(sourceComponent).Property("selectedBit").GetValue()}
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<T>(out var targetComponent))
                {
                    var t1 = jObject.GetValue("selectedBit");
                    if (t1 == null)
                        return;
                    var selectedBit = t1.Value<int>();

                    //applying values
                    Traverse.Create(targetComponent).Method("SetBitSelection").SetValue(selectedBit);
                }
            }
        }


        internal class DataTransfer_HighEnergyParticleSpawner
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<HighEnergyParticleSpawner>(out var component))
                {
                    return new JObject()
                    {
                        { "Direction", (int)component.Direction},
                        { "particleThreshold", component.particleThreshold},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<HighEnergyParticleSpawner>(out var targetComponent))
                {

                    var t1 = jObject.GetValue("Direction");
                    if (t1 == null)
                        return;
                    var Direction = t1.Value<int>();


                    var t2 = jObject.GetValue("particleThreshold");
                    if (t2 == null)
                        return;
                    var particleThreshold = t2.Value<float>();

                    //applying values
                    targetComponent.Direction = (EightDirection)Direction;
                    targetComponent.particleThreshold = particleThreshold;
                }
            }
        }
        internal class DataTransfer_HighEnergyParticleRedirector
        {
            internal static JObject TryGetData(GameObject arg)
            {
                if (arg.TryGetComponent<HighEnergyParticleRedirector>(out var component))
                {
                    return new JObject()
                    {
                        { "Direction", (int)component.Direction},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;
                if (building.TryGetComponent<HighEnergyParticleRedirector>(out var targetComponent))
                {

                    var t1 = jObject.GetValue("Direction");
                    if (t1 == null)
                        return;
                    var Direction = t1.Value<int>();

                    //applying values
                    targetComponent.Direction = (EightDirection)Direction;
                }
            }
        }
        internal class DataTransfer_HEPBattery
        {
            internal static JObject TryGetData(GameObject arg)
            {
                var component = arg.GetSMI<HEPBattery.Instance>();
                if (component != null)
                {
                    return new JObject()
                    {
                        { "particleThreshold", component.particleThreshold},
                    };
                }
                return null;
            }
            public static void TryApplyData(GameObject building, JObject jObject)
            {
                if (jObject == null)
                    return;

                var targetComponent = building.GetSMI<HEPBattery.Instance>();
                if (targetComponent != null)
                {

                    var t1 = jObject.GetValue("particleThreshold");
                    if (t1 == null)
                        return;
                    var particleThreshold = t1.Value<float>();

                    //applying values
                    targetComponent.particleThreshold = particleThreshold;
                }
            }
        }
    }
}

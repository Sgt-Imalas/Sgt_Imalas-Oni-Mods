using Klei.AI;
using Klei;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MineralizerReborn
{
    internal class ElementConverterWithInputsAndOutput : StateMachineComponent<ElementConverterWithInputsAndOutput.StatesInstance>, IGameObjectEffectDescriptor
    {
        [Serializable]
        [DebuggerDisplay("{tag} {massConsumptionRate}")]
        public struct ConsumedElement
        {
            public Storage targetStorage;

            public Tag Tag;

            public float MassConsumptionRate;

            public bool IsActive;

            public HandleVector<int>.Handle Accumulator;

            public string Name => Tag.ProperName();

            public float Rate => Game.Instance.accumulators.GetAverageRate(Accumulator);

            public ConsumedElement(Tag tag, float kgPerSecond, Storage storage, bool isActive = true)
            {
                Tag = tag;
                MassConsumptionRate = kgPerSecond;
                IsActive = isActive;
                targetStorage = storage;
                Accumulator = HandleVector<int>.InvalidHandle;
            }
        }

        [Serializable]
        public struct OutputElement
        {
            public bool IsActive;

            public SimHashes elementHash;

            public float minOutputTemperature;

            public bool useEntityTemperature;

            public float massGenerationRate;

            public bool storeOutput;

            public Vector2 outputElementOffset;

            public HandleVector<int>.Handle accumulator;

            public float diseaseWeight;

            public byte addedDiseaseIdx;

            public int addedDiseaseCount;

            public string Name => ElementLoader.FindElementByHash(elementHash).tag.ProperName();

            public float Rate => Game.Instance.accumulators.GetAverageRate(accumulator);

            public OutputElement(float kgPerSecond, SimHashes element, float minOutputTemperature, bool useEntityTemperature = false, bool storeOutput = false, float outputElementOffsetx = 0f, float outputElementOffsety = 0.5f, float diseaseWeight = 1f, byte addedDiseaseIdx = byte.MaxValue, int addedDiseaseCount = 0, bool isActive = true)
            {
                elementHash = element;
                this.minOutputTemperature = minOutputTemperature;
                this.useEntityTemperature = useEntityTemperature;
                this.storeOutput = storeOutput;
                massGenerationRate = kgPerSecond;
                outputElementOffset = new Vector2(outputElementOffsetx, outputElementOffsety);
                accumulator = HandleVector<int>.InvalidHandle;
                this.diseaseWeight = diseaseWeight;
                this.addedDiseaseIdx = addedDiseaseIdx;
                this.addedDiseaseCount = addedDiseaseCount;
                IsActive = isActive;
            }
        }

        public class StatesInstance : GameStateMachine<States, StatesInstance, ElementConverterWithInputsAndOutput, object>.GameInstance
        {
            public KSelectable selectable = null;

            public StatesInstance(ElementConverterWithInputsAndOutput smi)
                : base(smi)
            {
                selectable = GetComponent<KSelectable>();
            }

            public void AddStatusItems()
            {
                if (!base.master.ShowInUI)
                {
                    return;
                }

                ConsumedElement[] consumedElements = base.master.consumedElements;
                for (int i = 0; i < consumedElements.Length; i++)
                {
                    ConsumedElement element = consumedElements[i];
                    if (element.IsActive)
                    {
                        AddStatusItem(element, element.Tag, ElementConverterWithInputsAndOutputInput, base.master.consumedElementStatusHandles);
                    }
                }

                OutputElement[] outputElements = base.master.outputElements;
                for (int j = 0; j < outputElements.Length; j++)
                {
                    OutputElement element2 = outputElements[j];
                    if (element2.IsActive)
                    {
                        AddStatusItem(element2, element2.elementHash, ElementConverterWithInputsAndOutputOutput, base.master.outputElementStatusHandles);
                    }
                }
            }

            public void RemoveStatusItems()
            {
                if (base.master.ShowInUI)
                {
                    for (int i = 0; i < base.master.consumedElements.Length; i++)
                    {
                        ConsumedElement consumedElement = base.master.consumedElements[i];
                        RemoveStatusItem(consumedElement.Tag, base.master.consumedElementStatusHandles);
                    }

                    for (int j = 0; j < base.master.outputElements.Length; j++)
                    {
                        OutputElement outputElement = base.master.outputElements[j];
                        RemoveStatusItem(outputElement.elementHash, base.master.outputElementStatusHandles);
                    }

                    base.master.consumedElementStatusHandles.Clear();
                    base.master.outputElementStatusHandles.Clear();
                }
            }

            public void AddStatusItem<ElementType, IDType>(ElementType element, IDType id, StatusItem status, Dictionary<IDType, Guid> collection)
            {
                Guid guid2 = (collection[id] = selectable.AddStatusItem(status, element));
            }

            public void RemoveStatusItem<IDType>(IDType id, Dictionary<IDType, Guid> collection)
            {
                if (collection.TryGetValue(id, out var value))
                {
                    selectable.RemoveStatusItem(value);
                }
            }

            public void OnOperationalRequirementChanged(object data)
            {
                Operational operational = data as Operational;
                bool value = ((operational == null) ? ((bool)data) : operational.IsActive);
                base.sm.canConvert.Set(value, this);
            }
        }

        public class States : GameStateMachine<States, StatesInstance, ElementConverterWithInputsAndOutput>
        {
            public State disabled;

            public State converting;

            public BoolParameter canConvert;

            public bool ValidateStateTransition(StatesInstance smi, bool _)
            {
                bool flag = smi.GetCurrentState() == smi.sm.disabled;
                if (smi.master.operational == null)
                {
                    return flag;
                }

                bool flag2 = smi.master.consumedElements.Length == 0;
                bool flag3 = canConvert.Get(smi);
                int num = 0;
                while (!flag2 && num < smi.master.consumedElements.Length)
                {
                    ConsumedElement consumedElement = smi.master.consumedElements[num];
                    flag2 = consumedElement.IsActive;
                    num++;
                }

                if (flag3 && !flag2)
                {
                    canConvert.Set(value: false, smi, silenceEvents: true);
                    return false;
                }

                bool flag4 = smi.master.operational.MeetsRequirements(smi.master.OperationalRequirement);
                return flag4 == flag;
            }

            public void OnEnterRoot(StatesInstance smi)
            {
                int eventForState = (int)Operational.GetEventForState(smi.master.OperationalRequirement);
                smi.Subscribe(eventForState, smi.OnOperationalRequirementChanged);
            }

            public void OnExitRoot(StatesInstance smi)
            {
                int eventForState = (int)Operational.GetEventForState(smi.master.OperationalRequirement);
                smi.Unsubscribe(eventForState, smi.OnOperationalRequirementChanged);
            }

            public override void InitializeStates(out BaseState default_state)
            {
                default_state = disabled;
                root.Enter(OnEnterRoot).Exit(OnExitRoot);
                disabled.ParamTransition(canConvert, converting, ValidateStateTransition);
                converting.Enter("AddStatusItems", delegate (StatesInstance smi)
                {
                    smi.AddStatusItems();
                }).Exit("RemoveStatusItems", delegate (StatesInstance smi)
                {
                    smi.RemoveStatusItems();
                }).ParamTransition(canConvert, disabled, ValidateStateTransition)
                    .Update("ConvertMass", delegate (StatesInstance smi, float dt)
                    {
                        smi.master.ConvertMass();
                    }, UpdateRate.SIM_1000ms, load_balance: true);
            }
        }

        [MyCmpGet]
        public Operational operational;

        [MyCmpReq]
        public Storage OutputStorage;

        public Action<float> onConvertMass;

        public float totalDiseaseWeight = float.MaxValue;

        public Operational.State OperationalRequirement = Operational.State.Active;

        public AttributeInstance machinerySpeedAttribute;

        public float workSpeedMultiplier = 1f;

        public bool showDescriptors = true;

        public const float BASE_INTERVAL = 1f;

        public ConsumedElement[] consumedElements;

        public OutputElement[] outputElements;

        public bool ShowInUI = true;

        public float outputMultiplier = 1f;

        public Dictionary<Tag, Guid> consumedElementStatusHandles = new Dictionary<Tag, Guid>();

        public Dictionary<SimHashes, Guid> outputElementStatusHandles = new Dictionary<SimHashes, Guid>();

        public static StatusItem ElementConverterWithInputsAndOutputInput = null;

        public static StatusItem ElementConverterWithInputsAndOutputOutput = null;

        public float OutputMultiplier
        {
            get
            {
                return outputMultiplier;
            }
            set
            {
                outputMultiplier = value;
            }
        }

        public float AverageConvertRate => Game.Instance.accumulators.GetAverageRate(outputElements[0].accumulator);

        public void SetWorkSpeedMultiplier(float speed)
        {
            workSpeedMultiplier = speed;
        }

        public void SetConsumedElementActive(Tag elementId, bool active)
        {
            for (int i = 0; i < consumedElements.Length; i++)
            {
                if (consumedElements[i].Tag != elementId)
                {
                    continue;
                }

                consumedElements[i].IsActive = active;
                if (ShowInUI)
                {
                    ConsumedElement element = consumedElements[i];
                    if (active)
                    {
                        base.smi.AddStatusItem(element, element.Tag, ElementConverterWithInputsAndOutputInput, consumedElementStatusHandles);
                    }
                    else
                    {
                        base.smi.RemoveStatusItem(element.Tag, consumedElementStatusHandles);
                    }
                }

                break;
            }
        }

        public void SetOutputElementActive(SimHashes element, bool active)
        {
            for (int i = 0; i < outputElements.Length; i++)
            {
                if (outputElements[i].elementHash == element)
                {
                    outputElements[i].IsActive = active;
                    OutputElement element2 = outputElements[i];
                    if (active)
                    {
                        base.smi.AddStatusItem(element2, element2.elementHash, ElementConverterWithInputsAndOutputOutput, outputElementStatusHandles);
                    }
                    else
                    {
                        base.smi.RemoveStatusItem(element2.elementHash, outputElementStatusHandles);
                    }

                    break;
                }
            }
        }

        public void SetOutputStorage(Storage storage)
        {
            this.OutputStorage = storage;
        }

        public bool HasEnoughMass(Tag tag, bool includeInactive = false)
        {
            bool result = false;
            //List<GameObject> items = storage.items;
            ConsumedElement[] array = consumedElements;
            for (int i = 0; i < array.Length; i++)
            {
                ConsumedElement consumedElement = array[i];
                if (tag != consumedElement.Tag || (!includeInactive && !consumedElement.IsActive))
                {
                    continue;
                }

                float num = 0f;
                for (int j = 0; j < consumedElement.targetStorage.items.Count; j++)
                {
                    GameObject gameObject = consumedElement.targetStorage.items[j];
                    if (!(gameObject == null) && gameObject.HasTag(tag))
                    {
                        num += gameObject.GetComponent<PrimaryElement>().Mass;
                    }
                }

                result = num >= consumedElement.MassConsumptionRate;
                break;
            }

            return result;
        }

        public bool HasEnoughMassToStartConverting(bool includeInactive = false)
        {
            float speedMultiplier = GetSpeedMultiplier();
            float num = 1f * speedMultiplier;
            bool flag = includeInactive || consumedElements.Length == 0;
            bool flag2 = true;

            for (int i = 0; i < consumedElements.Length; i++)
            {
                ConsumedElement consumedElement = consumedElements[i];
                flag |= consumedElement.IsActive;
                if (!includeInactive && !consumedElement.IsActive)
                {
                    continue;
                }

                float num2 = 0f;
                for (int j = 0; j < consumedElement.targetStorage.items.Count; j++)
                {
                    GameObject gameObject = consumedElement.targetStorage.items[j];
                    if (!(gameObject == null) && gameObject.HasTag(consumedElement.Tag))
                    {
                        num2 += gameObject.GetComponent<PrimaryElement>().Mass;
                    }
                }

                if (num2 < consumedElement.MassConsumptionRate * num)
                {
                    flag2 = false;
                    break;
                }
            }

            return flag && flag2;
        }

        public bool CanConvertAtAll()
        {
            bool flag = consumedElements.Length == 0;
            bool flag2 = true;

            for (int i = 0; i < consumedElements.Length; i++)
            {
                ConsumedElement consumedElement = consumedElements[i];
                flag |= consumedElement.IsActive;
                if (!consumedElement.IsActive)
                {
                    continue;
                }

                bool flag3 = false;
                for (int j = 0; j < consumedElement.targetStorage.items.Count; j++)
                {
                    GameObject gameObject = consumedElement.targetStorage.items[j];
                    if (!(gameObject == null) && gameObject.HasTag(consumedElement.Tag) && gameObject.GetComponent<PrimaryElement>().Mass > 0f)
                    {
                        flag3 = true;
                        break;
                    }
                }

                if (!flag3)
                {
                    flag2 = false;
                    break;
                }
            }

            return flag && flag2;
        }

        public float GetSpeedMultiplier()
        {
            return machinerySpeedAttribute.GetTotalValue() * workSpeedMultiplier;
        }

        public void ConvertMass()
        {
            float speedMultiplier = GetSpeedMultiplier();
            float num = 1f * speedMultiplier;
            bool flag = consumedElements.Length == 0;
            float num2 = 1f;
            for (int i = 0; i < consumedElements.Length; i++)
            {
                ConsumedElement consumedElement = consumedElements[i];
                flag |= consumedElement.IsActive;
                if (!consumedElement.IsActive)
                {
                    continue;
                }

                float num3 = consumedElement.MassConsumptionRate * num * num2;
                if (num3 <= 0f)
                {
                    num2 = 0f;
                    break;
                }

                float num4 = 0f;
                for (int j = 0; j < consumedElement.targetStorage.items.Count; j++)
                {
                    GameObject gameObject = consumedElement.targetStorage.items[j];
                    if (!(gameObject == null) && gameObject.HasTag(consumedElement.Tag))
                    {
                        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
                        float num5 = Mathf.Min(num3, component.Mass);
                        num4 += num5 / num3;
                    }
                }

                num2 = Mathf.Min(num2, num4);
            }

            if (!flag || num2 <= 0f)
            {
                return;
            }

            SimUtil.DiseaseInfo diseaseInfo = SimUtil.DiseaseInfo.Invalid;
            diseaseInfo.idx = byte.MaxValue;
            diseaseInfo.count = 0;
            float num6 = 0f;
            float num7 = 0f;
            float num8 = 0f;
            for (int k = 0; k < consumedElements.Length; k++)
            {
                ConsumedElement consumedElement2 = consumedElements[k];
                if (!consumedElement2.IsActive)
                {
                    continue;
                }

                float num9 = consumedElement2.MassConsumptionRate * num * num2;
                Game.Instance.accumulators.Accumulate(consumedElement2.Accumulator, num9);
                for (int l = 0; l < consumedElement2.targetStorage.items.Count; l++)
                {
                    GameObject gameObject2 = consumedElement2.targetStorage.items[l];
                    if (gameObject2 == null)
                    {
                        continue;
                    }

                    if (gameObject2.HasTag(consumedElement2.Tag))
                    {
                        PrimaryElement component2 = gameObject2.GetComponent<PrimaryElement>();
                        component2.KeepZeroMassObject = true;
                        float num10 = Mathf.Min(num9, component2.Mass);
                        float num11 = num10 / component2.Mass;
                        int num12 = (int)(num11 * (float)component2.DiseaseCount);
                        float num13 = num10 * component2.Element.specificHeatCapacity;
                        num8 += num13;
                        num7 += num13 * component2.Temperature;
                        component2.Mass -= num10;
                        component2.ModifyDiseaseCount(-num12, "ElementConverterWithInputsAndOutput.ConvertMass");
                        num6 += num10;
                        diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(diseaseInfo.idx, diseaseInfo.count, component2.DiseaseIdx, num12);
                        num9 -= num10;
                        if (num9 <= 0f)
                        {
                            break;
                        }
                    }

                    if (num9 <= 0f)
                    {
                        Debug.Assert(num9 <= 0f);
                    }
                }
            }

            float num14 = ((num8 > 0f) ? (num7 / num8) : 0f);
            if (onConvertMass != null && num6 > 0f)
            {
                onConvertMass(num6);
            }

            for (int m = 0; m < outputElements.Length; m++)
            {
                OutputElement outputElement = outputElements[m];
                if (!outputElement.IsActive)
                {
                    continue;
                }

                SimUtil.DiseaseInfo a = diseaseInfo;
                if (totalDiseaseWeight <= 0f)
                {
                    a.idx = byte.MaxValue;
                    a.count = 0;
                }
                else
                {
                    float num15 = outputElement.diseaseWeight / totalDiseaseWeight;
                    a.count = (int)((float)a.count * num15);
                }

                if (outputElement.addedDiseaseIdx != byte.MaxValue)
                {
                    a = SimUtil.CalculateFinalDiseaseInfo(a, new SimUtil.DiseaseInfo
                    {
                        idx = outputElement.addedDiseaseIdx,
                        count = outputElement.addedDiseaseCount
                    });
                }

                float num16 = outputElement.massGenerationRate * OutputMultiplier * num * num2;
                Game.Instance.accumulators.Accumulate(outputElement.accumulator, num16);
                float num17 = 0f;
                num17 = ((!outputElement.useEntityTemperature && (num14 != 0f || outputElement.minOutputTemperature != 0f)) ? Mathf.Max(outputElement.minOutputTemperature, num14) : GetComponent<PrimaryElement>().Temperature);
                Element element = ElementLoader.FindElementByHash(outputElement.elementHash);
                if (outputElement.storeOutput)
                {
                    PrimaryElement primaryElement = OutputStorage.AddToPrimaryElement(outputElement.elementHash, num16, num17);
                    if (primaryElement == null)
                    {
                        if (element.IsGas)
                        {
                            OutputStorage.AddGasChunk(outputElement.elementHash, num16, num17, a.idx, a.count, keep_zero_mass: true);
                        }
                        else if (element.IsLiquid)
                        {
                            OutputStorage.AddLiquid(outputElement.elementHash, num16, num17, a.idx, a.count, keep_zero_mass: true);
                        }
                        else
                        {
                            GameObject go = element.substance.SpawnResource(base.transform.GetPosition(), num16, num17, a.idx, a.count, prevent_merge: false);
                            OutputStorage.Store(go, hide_popups: true);
                        }
                    }
                    else
                    {
                        primaryElement.AddDisease(a.idx, a.count, "ElementConverterWithInputsAndOutput.ConvertMass");
                    }
                }
                else
                {
                    Vector3 vector = new Vector3(base.transform.GetPosition().x + outputElement.outputElementOffset.x, base.transform.GetPosition().y + outputElement.outputElementOffset.y, 0f);
                    int num18 = Grid.PosToCell(vector);
                    if (element.IsLiquid)
                    {
                        FallingWater.instance.AddParticle(num18, element.idx, num16, num17, a.idx, a.count, skip_sound: true);
                    }
                    else if (element.IsSolid)
                    {
                        element.substance.SpawnResource(vector, num16, num17, a.idx, a.count);
                    }
                    else
                    {
                        SimMessages.AddRemoveSubstance(num18, outputElement.elementHash, CellEventLogger.Instance.OxygenModifierSimUpdate, num16, num17, a.idx, a.count);
                    }
                }

                if (outputElement.elementHash == SimHashes.Oxygen || outputElement.elementHash == SimHashes.ContaminatedOxygen)
                {
                    ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, num16, base.gameObject.GetProperName());
                }
            }

            OutputStorage.Trigger(-1697596308, base.gameObject);
        }

        public override void OnPrefabInit()
        {
            base.OnPrefabInit();
            Attributes attributes = base.gameObject.GetAttributes();
            machinerySpeedAttribute = attributes.Add(Db.Get().Attributes.MachinerySpeed);
            if (ElementConverterWithInputsAndOutputInput == null)
            {
                ElementConverterWithInputsAndOutputInput = new StatusItem("ElementConverterWithInputsAndOutputInput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID).SetResolveStringCallback(delegate (string str, object data)
                {
                    ConsumedElement consumedElement = (ConsumedElement)data;
                    str = str.Replace("{ElementTypes}", consumedElement.Name);
                    str = str.Replace("{FlowRate}", GameUtil.GetFormattedByTag(consumedElement.Tag, consumedElement.Rate, GameUtil.TimeSlice.PerSecond));
                    return str;
                });
            }

            if (ElementConverterWithInputsAndOutputOutput == null)
            {
                ElementConverterWithInputsAndOutputOutput = new StatusItem("ElementConverterWithInputsAndOutputOutput", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, allow_multiples: true, OverlayModes.None.ID).SetResolveStringCallback(delegate (string str, object data)
                {
                    OutputElement outputElement = (OutputElement)data;
                    str = str.Replace("{ElementTypes}", outputElement.Name);
                    str = str.Replace("{FlowRate}", GameUtil.GetFormattedMass(outputElement.Rate, GameUtil.TimeSlice.PerSecond));
                    return str;
                });
            }
        }

        public void SetAllConsumedActive(bool active)
        {
            for (int i = 0; i < consumedElements.Length; i++)
            {
                consumedElements[i].IsActive = active;
            }

            base.smi.sm.canConvert.Set(active, base.smi);
        }

        public void SetConsumedActive(Tag id, bool active)
        {
            bool flag = consumedElements.Length == 0;
            for (int i = 0; i < consumedElements.Length; i++)
            {
                ref ConsumedElement reference = ref consumedElements[i];
                if (reference.Tag == id)
                {
                    reference.IsActive = active;
                    if (active)
                    {
                        flag = true;
                        break;
                    }
                }

                flag |= reference.IsActive;
            }

            base.smi.sm.canConvert.Set(flag, base.smi);
        }

        public override void OnSpawn()
        {
            base.OnSpawn();
            for (int i = 0; i < consumedElements.Length; i++)
            {
                consumedElements[i].Accumulator = Game.Instance.accumulators.Add("ElementsConsumed", this);
            }

            totalDiseaseWeight = 0f;
            for (int j = 0; j < outputElements.Length; j++)
            {
                outputElements[j].accumulator = Game.Instance.accumulators.Add("OutputElements", this);
                totalDiseaseWeight += outputElements[j].diseaseWeight;
            }

            base.smi.StartSM();
        }

        public override void OnCleanUp()
        {
            for (int i = 0; i < consumedElements.Length; i++)
            {
                Game.Instance.accumulators.Remove(consumedElements[i].Accumulator);
            }

            for (int j = 0; j < outputElements.Length; j++)
            {
                Game.Instance.accumulators.Remove(outputElements[j].accumulator);
            }

            base.OnCleanUp();
        }

        public List<Descriptor> GetDescriptors(GameObject go)
        {
            List<Descriptor> list = new List<Descriptor>();
            if (!showDescriptors)
            {
                return list;
            }

            if (consumedElements != null)
            {
                ConsumedElement[] array = consumedElements;
                for (int i = 0; i < array.Length; i++)
                {
                    ConsumedElement consumedElement = array[i];
                    if (consumedElement.IsActive)
                    {
                        Descriptor item = default(Descriptor);
                        item.SetupDescriptor(string.Format(global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.MassConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), string.Format(global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMED, consumedElement.Name, GameUtil.GetFormattedMass(consumedElement.MassConsumptionRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}")), Descriptor.DescriptorType.Requirement);
                        list.Add(item);
                    }
                }
            }

            if (outputElements != null)
            {
                OutputElement[] array2 = outputElements;
                for (int j = 0; j < array2.Length; j++)
                {
                    OutputElement outputElement = array2[j];
                    if (outputElement.IsActive)
                    {
                        LocString locString = global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTEMITTED_INPUTTEMP;
                        LocString locString2 = global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_INPUTTEMP;
                        if (outputElement.useEntityTemperature)
                        {
                            locString = global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTEMITTED_ENTITYTEMP;
                            locString2 = global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_ENTITYTEMP;
                        }
                        else if (outputElement.minOutputTemperature > 0f)
                        {
                            locString = global::STRINGS.UI.BUILDINGEFFECTS.ELEMENTEMITTED_MINTEMP;
                            locString2 = global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_MINTEMP;
                        }

                        Descriptor item2 = new Descriptor(string.Format(locString, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature)), string.Format(locString2, outputElement.Name, GameUtil.GetFormattedMass(outputElement.massGenerationRate, GameUtil.TimeSlice.PerSecond, GameUtil.MetricMassFormat.UseThreshold, includeSuffix: true, "{0:0.##}"), GameUtil.GetFormattedTemperature(outputElement.minOutputTemperature)));
                        list.Add(item2);
                    }
                }
            }

            return list;
        }
    }
}


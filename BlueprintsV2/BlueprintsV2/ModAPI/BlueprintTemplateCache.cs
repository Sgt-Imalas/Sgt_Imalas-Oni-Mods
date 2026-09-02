using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.ModAPI
{
    public static class BlueprintTemplateCache
    {
        private static readonly Dictionary<string, GameObject> _cache = new Dictionary<string, GameObject>();

        public static GameObject GetOrCreateTemplate(BuildingDef def)
        {
            if (def == null) throw new ArgumentNullException(nameof(def));

            string key = def.PrefabID;
            if (_cache.TryGetValue(key, out GameObject template) && template != null)
            {
                ResetTemplateData(template);
                return template;
            }

            var prefab = def.BuildingComplete;
            if (prefab == null)
            {
                SgtLogger.error($"BuildingComplete prefab is null for {def.PrefabID}");
                return null;
            }

            Vector3 hiddenPos = new Vector3(-10000f, -10000f, 0f);
            template = GameUtil.KInstantiate(prefab, hiddenPos, Grid.SceneLayer.Background, null, 0);
            template.SetActive(false);
            template.GetComponent<KPrefabID>().AddTag(GameTags.TemplateBuilding);

            if (template.GetComponent<CopyBuildingSettings>() == null)
                template.AddComponent<CopyBuildingSettings>();

            _cache[key] = template;
            return template;
        }

        private static void ResetTemplateData(GameObject go)
        {
            // 1. Storage
            var storage = go.GetComponent<Storage>();
            if (storage != null)
            {
                storage.storageFilters?.Clear();
                storage.SetOnlyFetchMarkedItems(false);
            }

            // 2. TreeFilterable
            var treeFilter = go.GetComponent<TreeFilterable>();
            if (treeFilter != null)
            {
                treeFilter.AcceptedTags?.Clear();
            }

            // 3. Filterable
            var filterable = go.GetComponent<Filterable>();
            if (filterable != null)
            {
                filterable.SelectedTag = Tag.Invalid;
            }

            // 4. FlatTagFilterable
            var flatFilter = go.GetComponent<FlatTagFilterable>();
            if (flatFilter != null)
            {
                flatFilter.selectedTags?.Clear();
                // 它还会更新 TreeFilterable
                treeFilter = go.GetComponent<TreeFilterable>();
                treeFilter?.AcceptedTags?.Clear();
            }

            // 5. AccessControl
            var accessControl = go.GetComponent<AccessControl>();
            if (accessControl != null)
            {
                accessControl.savedPermissionsById?.Clear();
                accessControl.defaultPermissionByTag?.Clear();
            }

            // 6. Valve
            var valve = go.GetComponent<Valve>();
            if (valve != null)
            {
                valve.ChangeFlow(0f);
            }

            // 7. LimitValve
            var limitValve = go.GetComponent<LimitValve>();
            if (limitValve != null)
            {
                limitValve.Limit = 0f;
            }

            // 8. LogicTimerSensor
            var timerSensor = go.GetComponent<LogicTimerSensor>();
            if (timerSensor != null)
            {
                timerSensor.onDuration = 10f;
                timerSensor.offDuration = 10f;
                timerSensor.timeElapsedInCurrentState = 0f;
                timerSensor.displayCyclesMode = false;
            }

            // 9. LogicCritterCountSensor
            var critterSensor = go.GetComponent<LogicCritterCountSensor>();
            if (critterSensor != null)
            {
                critterSensor.countThreshold = 0;
                critterSensor.activateOnGreaterThan = true;
                critterSensor.countCritters = true;
                critterSensor.countEggs = true;
            }

            // 10. LogicCounter
            var counter = go.GetComponent<LogicCounter>();
            if (counter != null)
            {
                counter.maxCount = 10;
                counter.resetCountAtMax = false;
                counter.advancedMode = false;
                counter.currentCount = 0;
            }

            // 11. LogicTimeOfDaySensor
            var timeOfDaySensor = go.GetComponent<LogicTimeOfDaySensor>();
            if (timeOfDaySensor != null)
            {
                timeOfDaySensor.startTime = 0f;
                timeOfDaySensor.duration = 0f;
            }

            // 12. LogicAlarm
            var alarm = go.GetComponent<LogicAlarm>();
            if (alarm != null)
            {
                alarm.notificationName = "";
                alarm.notificationTooltip = "";
                alarm.notificationType = NotificationType.Neutral;
                alarm.pauseOnNotify = false;
                alarm.zoomOnNotify = false;
                alarm.cooldown = 0f;
            }

            // 13. LogicClusterLocationSensor
            var clusterSensor = go.GetComponent<LogicClusterLocationSensor>();
            if (clusterSensor != null)
            {
                clusterSensor.activeInSpace = false;
                clusterSensor.activeLocations?.Clear();
            }

            // 14. IUserControlledCapacity
            var userCapacity = go.GetComponent<IUserControlledCapacity>();
            if (userCapacity != null)
            {
                userCapacity.UserMaxCapacity = 0f;
            }

            // 15. IActivationRangeTarget
            var rangeTarget = go.GetComponent<IActivationRangeTarget>();
            if (rangeTarget != null)
            {
                rangeTarget.ActivateValue = 0;
                rangeTarget.DeactivateValue = 0;
            }

            // 16. IThresholdSwitch
            var thresholdSwitch = go.GetComponent<IThresholdSwitch>();
            if (thresholdSwitch != null)
            {
                thresholdSwitch.Threshold = 0f;
                thresholdSwitch.ActivateAboveThreshold = true;
            }

            // 17. Switch
            var sw = go.GetComponent<Switch>();
            if (sw != null && sw.IsSwitchedOn)
            {
                sw.Toggle();
            }

            // 18. Door
            var door = go.GetComponent<Door>();
            if (door != null)
            {
                door.QueueStateChange(Door.ControlState.Auto);
            }

            // 19. BuildingEnabledButton
            var btn = go.GetComponent<BuildingEnabledButton>();
            if (btn != null)
            {
                btn.IsEnabled = true;
            }

            // 20. Repairable – 默认允许修复，无需重置

            // 21. EnergyGenerator
            var gen = go.GetComponent<EnergyGenerator>();
            if (gen != null && !gen.ignoreBatteryRefillPercent)
            {
                gen.batteryRefillPercent = 0.5f;
            }

            // 22. Automatable
            var auto = go.GetComponent<Automatable>();
            if (auto != null)
            {
                auto.SetAutomationOnly(true);
            }

            // 23. FoodStorage
            var foodStorage = go.GetComponent<FoodStorage>();
            if (foodStorage != null)
            {
                foodStorage.SpicedFoodOnly = false;
            }

            // 24. SingleEntityReceptacle
            var receptacle = go.GetComponent<SingleEntityReceptacle>();
            if (receptacle != null)
            {
                receptacle.autoReplaceEntity = false;
                receptacle.requestedEntityTag = Tag.Invalid;
                receptacle.requestedEntityAdditionalFilterTag = Tag.Invalid;
            }

            // 25. StorageTile (SMI)
            var storageTileSmi = go.GetSMI<StorageTile.Instance>();
            if (storageTileSmi != null)
            {
                storageTileSmi.SetTargetItem(Tag.Invalid);
                storageTileSmi.UserMaxCapacity = 0f;
            }

            // 26. DirectionControl
            var dirControl = go.GetComponent<DirectionControl>();
            if (dirControl != null)
            {
                dirControl.SetAllowedDirection(WorkableReactable.AllowedDirection.Any);
            }

            // 27. Clinic
            var clinic = go.GetComponent<Clinic>();
            if (clinic != null)
            {
                (clinic as ISliderControl)?.SetSliderValue(70f, 0);
            }

            // 28. SpaceHeater
            var heater = go.GetComponent<SpaceHeater>();
            if (heater != null && heater.produceHeat)
            {
                heater.SetUserSpecifiedPowerConsumptionValue(0f);
            }

            // 29. AutoDisinfectable
            var disinfect = go.GetComponent<AutoDisinfectable>();
            if (disinfect != null)
            {
                disinfect.EnableAutoDisinfect();
            }

            // 30. LogicGateBuffer / LogicGateFilter
            TryResetGenericLogicGateDelay(go);

            // 31. LogicRibbonWriter / Reader
            var writer = go.GetComponent<LogicRibbonWriter>();
            if (writer != null)
                writer.SetBitSelection(0);

            var reader = go.GetComponent<LogicRibbonReader>();
            if (reader != null)
                reader.SetBitSelection(0);

            // 32. PixelPack
            var pixelPack = go.GetComponent<PixelPack>();
            if (pixelPack != null)
            {
                if (pixelPack.colorSettings != null)
                {
                    var defaultPair = new PixelPack.ColorPair
                    {
                        activeColor = pixelPack.defaultActive,
                        standbyColor = pixelPack.defaultStandby
                    };
                    for (int i = 0; i < pixelPack.colorSettings.Count; i++)
                    {
                        pixelPack.colorSettings[i] = defaultPair;
                    }
                }
                pixelPack.UpdateColors();
            }

            // 33. HighEnergyParticleSpawner
            var spawner = go.GetComponent<HighEnergyParticleSpawner>();
            if (spawner != null)
            {
                spawner.Direction = EightDirection.Right;
                spawner.particleThreshold = 50f;
            }

            // 34. HighEnergyParticleRedirector
            var redirector = go.GetComponent<HighEnergyParticleRedirector>();
            if (redirector != null)
            {
                redirector.Direction = EightDirection.Right;
            }

            // 35. HEPBattery (SMI)
            var hepBattery = go.GetSMI<HEPBattery.Instance>();
            if (hepBattery != null)
            {
                hepBattery.particleThreshold = 50f;
            }

            // 36. UserNameable
            var nameable = go.GetComponent<UserNameable>();
            if (nameable != null)
            {
                nameable.SetName("");
            }

            // 37. Prioritizable
            var prioritizable = go.GetComponent<Prioritizable>();
            if (prioritizable != null)
            {
                prioritizable.SetMasterPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, 5));
            }
        }

        private static void TryResetGenericLogicGateDelay(GameObject go)
        {
            var buffer = go.GetComponent<LogicGateBuffer>();
            if (buffer != null)
            {
                var prop = typeof(LogicGateBuffer).GetProperty("DelayAmount");
                prop?.SetValue(buffer, 5f, null);
            }

            var filter = go.GetComponent<LogicGateFilter>();
            if (filter != null)
            {
                var prop = typeof(LogicGateFilter).GetProperty("DelayAmount");
                prop?.SetValue(filter, 5f, null);
            }
        }

        public static void Clear()
        {
            foreach (var kvp in _cache)
            {
                if (kvp.Value != null)
                    UnityEngine.Object.Destroy(kvp.Value);
            }
            _cache.Clear();
            SgtLogger.l("BlueprintTemplateCache cleared.");
        }
    }
}
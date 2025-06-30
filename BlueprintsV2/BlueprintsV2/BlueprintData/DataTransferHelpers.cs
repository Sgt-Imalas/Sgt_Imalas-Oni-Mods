using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;

namespace BlueprintsV2.BlueprintData
{
	internal class DataTransferHelpers
	{
		internal class DataTransfer_UserNameable
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<UserNameable>(out var component))
				{
					return new JObject()
					{
						{ "savedName", component.savedName},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<UserNameable>(out var targetComponent))
				{
					var t1 = jObject.GetValue("savedName");
					if (t1 == null)
						return;
					var savedName = t1.Value<string>();
					targetComponent.SetName(savedName);
				}
			}
		}
		internal class DataTransfer_BuildingEnabledButton
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<BuildingEnabledButton>(out var component))
				{
					bool shouldBeEnabled = component.IsEnabled;
					if(component.queuedToggle)
						shouldBeEnabled = !shouldBeEnabled; //queued toggle means it will be toggled to the opposite state, so we need to invert it here
					SgtLogger.l("Is enabled: "+component.IsEnabled+", toggled: "+ component.queuedToggle);

					return new JObject()
					{
						{ "IsEnabled", shouldBeEnabled},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<BuildingEnabledButton>(out var targetComponent))
				{

					var t1 = jObject.GetValue("IsEnabled");
					if (t1 == null)
						return;
					var IsEnabled = t1.Value<bool>();
					targetComponent.IsEnabled = IsEnabled;
				}
			}
		}
		internal class DataTransfer_Repairable
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<Repairable>(out var component))
				{
					bool repairForbiddenState = false;
					if (component.smi != null)
					{
						try
						{
							repairForbiddenState = component.smi?.GetCurrentState() == component.smi?.sm?.forbidden;
						}
						catch (Exception e)
						{
							SgtLogger.l("Error getting repairable state: " + e.Message);
						}
					}
					return new JObject()
					{
						{ "ForbiddenRepair", repairForbiddenState},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<Repairable>(out var targetComponent))
				{

					var t1 = jObject.GetValue("ForbiddenRepair");
					if (t1 == null)
						return;
					var RepairForbidden = t1.Value<bool>();
					if(targetComponent.smi == null)
					{
						SgtLogger.l("Repairable component has no state machine, skipping repair state transfer.");
						return;
					}

					if (RepairForbidden)
						targetComponent.CancelRepair();
					else
						targetComponent.AllowRepair();
				}
			}
		}
		internal class DataTransfer_SingleEntityReceptacle
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<SingleEntityReceptacle>(out var component))
				{
					var occupant = component.Occupant;

					Tag requestedEntityTag = component.requestedEntityTag;
					Tag additionalFilterTag = component.requestedEntityAdditionalFilterTag;
					if (occupant != null && occupant.TryGetComponent<KPrefabID>(out var occupantPrefabID))
					{
						requestedEntityTag = occupantPrefabID.PrefabTag;
						if (occupantPrefabID.TryGetComponent<SeedProducer>(out var seedProducer))
						{
							requestedEntityTag = TagManager.Create(seedProducer.seedInfo.seedId);

							if (occupant.TryGetComponent<MutantPlant>(out var mutant))
								additionalFilterTag = mutant.SubSpeciesID;
							else
								additionalFilterTag = Tag.Invalid;
						}
					}

					if (requestedEntityTag == null)
						requestedEntityTag = Tag.Invalid;
					if (additionalFilterTag == null)
						additionalFilterTag = Tag.Invalid;

					string requestedEntityTagString = requestedEntityTag == Tag.Invalid ? string.Empty : requestedEntityTag.ToString();
					string additionalFilterTagString = additionalFilterTag == Tag.Invalid ? string.Empty : additionalFilterTag.ToString();


					return new JObject()
					{
						{ "requestedEntityTag", requestedEntityTagString},
						{ "requestedEntityAdditionalFilterTag", additionalFilterTagString},
						{ "autoReplaceEntity", component.autoReplaceEntity }
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<SingleEntityReceptacle>(out var targetComponent))
				{

					var t1 = jObject.GetValue("requestedEntityTag");
					if (t1 == null)
						return;
					var requestedEntityTag = t1.Value<string>();

					var t2 = jObject.GetValue("requestedEntityAdditionalFilterTag");
					if (t2 == null)
						return;
					var requestedEntityAdditionalFilterTag = t2.Value<string>();

					var t3 = jObject.GetValue("autoReplaceEntity");
					if (t3 == null)
						return;
					var autoReplaceEntity = t3.Value<bool>();

					SgtLogger.l("Requested Entity Tag: " + requestedEntityTag + ", extra filter: " + requestedEntityAdditionalFilterTag);

					targetComponent.autoReplaceEntity = autoReplaceEntity;

					var tagParsed = requestedEntityTag.IsNullOrWhiteSpace() ? Tag.Invalid : TagManager.Create(requestedEntityTag);
					var extraTagParsed = requestedEntityAdditionalFilterTag.IsNullOrWhiteSpace() ? Tag.Invalid : TagManager.Create(requestedEntityAdditionalFilterTag);

					if (targetComponent.Occupant == null && tagParsed.IsValid)
						targetComponent.CreateOrder(tagParsed, extraTagParsed);
				}
			}
		}
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
		internal class DataTransfer_LogicCounter
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<LogicCounter>(out var component))
				{
					return new JObject()
					{
						{ "maxCount", component.maxCount},
						{ "resetCountAtMax", component.resetCountAtMax},
						{ "advancedMode", component.advancedMode},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<LogicCounter>(out var targetComponent))
				{

					var t1 = jObject.GetValue("maxCount");
					if (t1 == null)
						return;
					targetComponent.maxCount = t1.Value<int>();

					var t2 = jObject.GetValue("resetCountAtMax");
					if (t2 == null)
						return;
					targetComponent.resetCountAtMax = t2.Value<bool>();

					var t3 = jObject.GetValue("advancedMode");
					if (t3 == null)
						return;
					targetComponent.advancedMode = t3.Value<bool>();
				}
			}
		}
		internal class DataTransfer_PixelPack
		{
			class PixelPackColor
			{
				public float r, g, b, a;
				public PixelPackColor(Color original)
				{
					r = original.r;
					g = original.g;
					b = original.b;
					a = original.a;
				}
				public Color ToColor()
				{
					return new Color(r, g, b, a);
				}
			}
			class PixelPackColorData
			{
				public PixelPackColor activeColor, standbyColor;

				public PixelPackColorData(Color a, Color b)
				{
					activeColor = new PixelPackColor(a);
					standbyColor = new PixelPackColor(b);
				}
				public PixelPack.ColorPair GetData()
				{
					return new PixelPack.ColorPair()
					{
						activeColor = this.activeColor.ToColor(),
						standbyColor = this.standbyColor.ToColor()
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
		internal class DataTransfer_IUserControlledCapacity
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<IUserControlledCapacity>(out var component))
				{
					return new JObject()
					{
						{ "UserMaxCapacity", component.UserMaxCapacity},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<IUserControlledCapacity>(out var targetComponent))
				{
					var t1 = jObject.GetValue("UserMaxCapacity");
					if (t1 == null)
						return;
					targetComponent.UserMaxCapacity = t1.Value<float>();
				}
			}
		}
		internal class DataTransfer_Automatable
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<Automatable>(out var component))
				{
					return new JObject()
					{
						{ "automationOnly", component.automationOnly},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<Automatable>(out var targetComponent))
				{
					var t1 = jObject.GetValue("automationOnly");
					if (t1 == null)
						return;
					targetComponent.automationOnly = t1.Value<bool>();
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
		internal class DataTransfer_IActivationRangeTarget
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<IActivationRangeTarget>(out var component))
				{
					return new JObject()
					{
						{ "ActivateValue", component.ActivateValue},
						{ "DeactivateValue", component.DeactivateValue},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<IActivationRangeTarget>(out var targetComponent))
				{
					var t1 = jObject.GetValue("DeactivateValue");
					if (t1 == null)
						return;
					targetComponent.DeactivateValue = t1.Value<int>();

					var t2 = jObject.GetValue("ActivateValue");
					if (t2 == null)
						return;
					targetComponent.ActivateValue = t2.Value<int>();
				}
			}
		}
		internal class DataTransfer_EnergyGenerator
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<EnergyGenerator>(out var component))
				{
					if (component.ignoreBatteryRefillPercent)
						return null;
					return new JObject()
					{
						{ "batteryRefillPercent", component.batteryRefillPercent},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<EnergyGenerator>(out var targetComponent))
				{
					if (targetComponent.ignoreBatteryRefillPercent)
						return;

					var t1 = jObject.GetValue("batteryRefillPercent");
					if (t1 == null)
						return;

					var batteryRefillPercent = t1.Value<float>();
					targetComponent.batteryRefillPercent = batteryRefillPercent;
				}
			}
		}
		internal class DataTransfer_FoodStorage
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<FoodStorage>(out var component))
				{
					return new JObject()
					{
						{ "SpicedFoodOnly", component.SpicedFoodOnly},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<FoodStorage>(out var targetComponent))
				{
					var t1 = jObject.GetValue("SpicedFoodOnly");
					if (t1 != null)
					{
						targetComponent.SpicedFoodOnly = t1.Value<bool>();
					}
				}
			}
		}
		internal class DataTransfer_AutoDisinfectable
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<AutoDisinfectable>(out var component))
				{
					return new JObject()
					{
						{ "enableAutoDisinfect", component.enableAutoDisinfect},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<AutoDisinfectable>(out var targetComponent))
				{
					var t1 = jObject.GetValue("enableAutoDisinfect");
					if (t1 != null)
					{
						bool enableAutoDisinfect = t1.Value<bool>();
						if (enableAutoDisinfect)
							targetComponent.EnableAutoDisinfect();
						else
							targetComponent.DisableAutoDisinfect();
					}
				}
			}
		}
		internal class DataTransfer_DirectionControl
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<DirectionControl>(out var component))
				{
					return new JObject()
					{
						{ "allowedDirection", (int)component.allowedDirection},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<DirectionControl>(out var targetComponent))
				{
					var t1 = jObject.GetValue("allowedDirection");
					if (t1 != null)
					{
						WorkableReactable.AllowedDirection allowedDirection = (WorkableReactable.AllowedDirection)t1.Value<int>();
						targetComponent.SetAllowedDirection(allowedDirection);
					}
				}
			}
		}

		internal class DataTransfer_Prioritizable
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<Prioritizable>(out var component))
				{
					var prio = component.GetMasterPriority();
					SgtLogger.l("Getting prio " + prio.priority_value + " from " + arg.name);
					return new JObject()
					{
						{ "masterPrioritySetting", JsonConvert.SerializeObject(prio)},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<Prioritizable>(out var targetComponent))
				{
					var t1 = jObject.GetValue("masterPrioritySetting");
					if (t1 != null)
					{
						var masterPrioritySettingJson = t1.Value<string>();
						var masterPrioritySetting = JsonConvert.DeserializeObject<PrioritySetting>(masterPrioritySettingJson);
						SgtLogger.l("applying prio: " + masterPrioritySetting.priority_value);
						targetComponent.SetMasterPriority(masterPrioritySetting);
					}
				}
			}
		}
		internal class DataTransfer_TreeFilterable
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<TreeFilterable>(out var component))
				{
					if (!component.copySettingsEnabled)
						return null;

					var tags = component.GetTags();
					var targetStorage = component.GetFilterStorage();
					bool onlyFetchMarkedItems = targetStorage != null && targetStorage.allowSettingOnlyFetchMarkedItems ? targetStorage.onlyFetchMarkedItems : false;

					return new JObject()
					{
						{ "acceptedTagSet", JsonConvert.SerializeObject(tags)},
						{ "onlyFetchMarkedItems", onlyFetchMarkedItems},
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
					if (t1 != null)
					{
						var acceptedTagSetJson = t1.Value<string>();
						var acceptedTagSet = JsonConvert.DeserializeObject<HashSet<Tag>>(acceptedTagSetJson);
						targetComponent.UpdateFilters(acceptedTagSet);
					}
					var t2 = jObject.GetValue("onlyFetchMarkedItems");
					if (t2 != null)
					{
						var storage = targetComponent.GetFilterStorage();
						if (storage.allowSettingOnlyFetchMarkedItems)
						{
							var onlyFetchMarkedItems = t2.Value<bool>();
							storage.SetOnlyFetchMarkedItems(onlyFetchMarkedItems);
						}
					}
				}
			}
		}
		internal class DataTransfer_FlatTagFilterable
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<FlatTagFilterable>(out var component))
				{
					if (!component.currentlyUserAssignable)
						return null;

					var selectedTags = component.selectedTags;

					return new JObject()
					{
						{ "selectedTags", JsonConvert.SerializeObject(selectedTags)},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<FlatTagFilterable>(out var targetComponent))
				{
					if (!targetComponent.currentlyUserAssignable)
						return;
					var t1 = jObject.GetValue("selectedTags");
					if (t1 != null)
					{
						var selectedTagstJson = t1.Value<string>();
						var selectedTags = JsonConvert.DeserializeObject<HashSet<Tag>>(selectedTagstJson);

						targetComponent.selectedTags.Clear();
						foreach (Tag selectedTag in selectedTags)
						{
							if (!targetComponent.tagOptions.Contains(selectedTag))
								targetComponent.tagOptions.Add(selectedTag);
							targetComponent.SelectTag(selectedTag, state: true);
						}
						targetComponent.GetComponent<TreeFilterable>().UpdateFilters([.. selectedTags]);
					}

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
		internal class DataTransfer_AccessControl
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<AccessControl>(out var component) && component.controlEnabled)
				{
					var customPermissions = component.savedPermissions
						.Where(entry => entry.Key != null && entry.Key.GetId() != 0)
						.ToDictionary(entry => entry.Key.GetId(), entry => (int)entry.Value);

					foreach (var item in customPermissions)
					{
						SgtLogger.l("" + item.Key + " " + item.Value);
					}

					return new JObject()
					{
						{ "DefaultPermission", (int)component.DefaultPermission},
						{ "savedPermissions", JsonConvert.SerializeObject(customPermissions)}
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<AccessControl>(out var targetComponent) && targetComponent.controlEnabled)
				{
					var t1 = jObject.GetValue("DefaultPermission");
					if (t1 == null)
						return;
					var DefaultPermission = t1.Value<int>();

					//applying values
					targetComponent.DefaultPermission = (AccessControl.Permission)DefaultPermission;

					var t2 = jObject.GetValue("savedPermissions");
					if (t2 == null)
						return;
					try
					{
						var customPermissions = JsonConvert.DeserializeObject<Dictionary<int, AccessControl.Permission>>(t2.Value<string>());

						foreach (var item in customPermissions)
						{
							SgtLogger.l("" + item.Key + " " + item.Value);
						}
						bool customPermissionSet = false;

						foreach (var entry in customPermissions)
						{
							var targetMinionProxy = Components.MinionAssignablesProxy.FirstOrDefault(x => x.GetComponent<KPrefabID>()?.InstanceID == entry.Key);
							if (targetMinionProxy == null)
								continue;

							SgtLogger.l("minion found: " + targetMinionProxy.target.GetProperName());
							targetComponent.SetPermission(targetMinionProxy, entry.Value);
							customPermissionSet = true;
						}
						if (customPermissionSet)
						{

						}
					}
					catch (Exception e)
					{
						SgtLogger.error("Error while applying saved door permissions:\n" + e);
					}

				}
			}
		}
		internal class DataTransfer_LimitValve
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<LimitValve>(out var component))
				{
					return new JObject()
					{
						{ "Limit", component.Limit}
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<LimitValve>(out var targetComponent))
				{
					var t1 = jObject.GetValue("Limit");
					if (t1 == null)
						return;
					var Limit = t1.Value<float>();

					//applying values
					targetComponent.Limit = Limit;
				}
			}
		}
		internal class DataTransfer_Valve
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<Valve>(out var component))
				{
					return new JObject()
					{
						{ "DesiredFlow", component.DesiredFlow}
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<Valve>(out var targetComponent))
				{
					var t1 = jObject.GetValue("DesiredFlow");
					if (t1 == null)
						return;
					var DesiredFlow = t1.Value<float>();

					//applying values
					targetComponent.ChangeFlow(DesiredFlow);
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
		internal class DataTransfer_LogicAlarm
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<LogicAlarm>(out var component))
				{
					return new JObject()
					{
						{ "notificationName", component.notificationName},
						{ "notificationTooltip", component.notificationTooltip},
						{ "notificationType", (int)component.notificationType},
						{ "pauseOnNotify", component.pauseOnNotify},
						{ "zoomOnNotify", component.zoomOnNotify},
						{ "cooldown", component.cooldown},
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<LogicAlarm>(out var targetComponent))
				{
					var t1 = jObject.GetValue("notificationName");
					if (t1 != null)
					{
						string notificationName = t1.Value<string>();
						targetComponent.notificationName = notificationName;
					}
					var t2 = jObject.GetValue("notificationTooltip");
					if (t2 != null)
					{
						string notificationTooltip = t2.Value<string>();
						targetComponent.notificationTooltip = notificationTooltip;
					}
					var t3 = jObject.GetValue("notificationType");
					if (t3 != null)
					{
						NotificationType notificationType = (NotificationType)t3.Value<int>();
						targetComponent.notificationType = notificationType;
					}
					var t4 = jObject.GetValue("pauseOnNotify");
					if (t4 != null)
					{
						bool pauseOnNotify = t4.Value<bool>();
						targetComponent.pauseOnNotify = pauseOnNotify;
					}
					var t5 = jObject.GetValue("zoomOnNotify");
					if (t5 != null)
					{
						bool zoomOnNotify = t5.Value<bool>();
						targetComponent.zoomOnNotify = zoomOnNotify;
					}
					var t6 = jObject.GetValue("cooldown");
					if (t6 != null)
					{
						float cooldown = t6.Value<float>();
						targetComponent.cooldown = cooldown;
					}
					targetComponent.UpdateNotification(true);
				}
			}
		}
		internal class DataTransfer_Switch
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<Switch>(out var component))
				{
					return new JObject()
					{
						{ "switchedOn", component.switchedOn}
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<Switch>(out var targetComponent))
				{
					var t1 = jObject.GetValue("switchedOn");
					if (t1 == null)
						return;
					var switchedOn = t1.Value<bool>();

					//applying values
					targetComponent.SetState(switchedOn);
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

		internal class DataTransfer_IThresholdSwitch
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<IThresholdSwitch>(out var sourceComponent))
				{
					return new JObject()
					{
						{ "Threshold", sourceComponent.Threshold},
						{ "ActivateAboveThreshold", sourceComponent.ActivateAboveThreshold}
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<IThresholdSwitch>(out var targetComponent))
				{
					var t1 = jObject.GetValue("Threshold");
					if (t1 == null)
						return;
					var Threshold = t1.Value<float>();


					var t2 = jObject.GetValue("ActivateAboveThreshold");
					if (t2 == null)
						return;
					var activateAboveThreshold = t2.Value<bool>();
					targetComponent.ActivateAboveThreshold = activateAboveThreshold;
					targetComponent.Threshold = Threshold;
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

		internal class DataTransfer_LogicRibbonWriter
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<LogicRibbonWriter>(out var sourceComponent))
				{
					return new JObject()
					{
						{ "selectedBit", sourceComponent.GetBitSelection()}
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<LogicRibbonWriter>(out var targetComponent))
				{
					var t1 = jObject.GetValue("selectedBit");
					if (t1 == null)
						return;
					var selectedBit = t1.Value<int>();

					SgtLogger.l("bit: " + selectedBit);
					//applying values
					targetComponent.SetBitSelection(selectedBit);
				}
			}
		}
		internal class DataTransfer_LogicRibbonReader
		{
			internal static JObject TryGetData(GameObject arg)
			{
				if (arg.TryGetComponent<LogicRibbonReader>(out var sourceComponent))
				{
					return new JObject()
					{
						{ "selectedBit", sourceComponent.GetBitSelection()}
					};
				}
				return null;
			}
			public static void TryApplyData(GameObject building, JObject jObject)
			{
				if (jObject == null)
					return;
				if (building.TryGetComponent<LogicRibbonReader>(out var targetComponent))
				{
					var t1 = jObject.GetValue("selectedBit");
					if (t1 == null)
						return;
					var selectedBit = t1.Value<int>();

					SgtLogger.l("bit: " + selectedBit);
					//applying values
					targetComponent.SetBitSelection(selectedBit);
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
			public static JObject TryGetData(GameObject arg)
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

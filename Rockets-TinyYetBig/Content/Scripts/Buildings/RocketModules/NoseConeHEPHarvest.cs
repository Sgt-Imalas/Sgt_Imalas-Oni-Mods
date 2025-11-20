using Rockets_TinyYetBig.Behaviours;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UtilLibs;
using static Rockets_TinyYetBig.STRINGS.BUILDING.STATUSITEMS;

namespace Rockets_TinyYetBig.Buildings.Nosecones
{
	public class ResourceHarvestModuleHEP :
  GameStateMachine<ResourceHarvestModuleHEP, ResourceHarvestModuleHEP.StatesInstance, IStateMachineTarget, ResourceHarvestModuleHEP.Def>
	{
		public BoolParameter canHarvest;
		public FloatParameter lastHarvestTime;
		public State grounded;
		public NotGroundedStates not_grounded;
		public const float HEPConsumptionRate = 0.25f;
		public override void InitializeStates(out BaseState default_state)
		{
			default_state = grounded;
			root.Enter(smi => smi.CheckIfCanDrill());
			grounded.TagTransition(GameTags.RocketNotOnGround, not_grounded)
				.Update((smi, dt) => smi.UpdateMeter());
			not_grounded
				.DefaultState(not_grounded.not_drilling)
				.EventHandler(GameHashes.ClusterLocationChanged, smi => Game.Instance, smi => smi.CheckIfCanDrill()).EventHandler(GameHashes.OnStorageChange, smi => smi.CheckIfCanDrill()).TagTransition(GameTags.RocketNotOnGround, grounded, true);
			not_grounded.not_drilling
				.PlayAnim("loaded")
				.ParamTransition(canHarvest, not_grounded.drilling, IsTrue)
				.Enter(smi => StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject))
				.Update((smi, dt) => smi.CheckIfCanDrill(), UpdateRate.SIM_4000ms);
			not_grounded.drilling
				.PlayAnim("deploying")
				.Exit(smi =>
				{
					StatesInstance.RemoveHarvestStatusItems(smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.gameObject);
					smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().Trigger(939543986, null);
				}).Enter(smi =>
				{
					Clustercraft clustercraft = smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.m_clustercraft;

					clustercraft.AddTag(GameTags.RocketDrilling);
					try
					{
						var controlStations = Components.RocketControlStations.GetWorldItems(clustercraft.GetComponent<WorldContainer>().id);
						if (controlStations != null && controlStations.Count > 0)
						{
							var station = controlStations.First();
							if (station != null)
							{
								station.smi.sm.CreateLaunchChore(station.smi);
							}
						}
					}
					catch (Exception ex)
					{
						SgtLogger.warning("Laser drillcone has encountered an error:");
						SgtLogger.error(ex.Message);
					}
					clustercraft.Trigger(-1762453998, null);
					StatesInstance.AddHarvestStatusItems(clustercraft.ModuleInterface.gameObject,
						smi.def.harvestSpeed * ModAssets.GetMiningPilotSkillMultiplier(clustercraft),
						clustercraft
						);
				})
				.Exit((smi) =>
				{
					smi.master.gameObject.GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>().RemoveTag(GameTags.RocketDrilling);
				})
				.Update((smi, dt) =>
				{
					smi.HarvestFromPOI(dt);
					lastHarvestTime.Set(Time.time, smi);
				}, UpdateRate.SIM_4000ms)
				.ParamTransition(canHarvest, not_grounded.not_drilling, IsFalse)				
				;
		}

		public class Def : BaseDef
		{
			public float harvestSpeed;
		}

		public class NotGroundedStates :
		  State
		{
			public State not_drilling;
			public State drilling;
		}

		public class StatesInstance :
		  GameInstance
		{
			private MeterController meter;
			private HighEnergyParticleStorage storage;

			public StatesInstance(IStateMachineTarget master, Def def)
			  : base(master, def)
			{
				storage = GetComponent<HighEnergyParticleStorage>();
				GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new ConditionHasRadbolts(storage, Config.Instance.LaserDrillconeCapacity));
				Subscribe(-1697596308, UpdateMeter);
				meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", nameof(meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, []);
				meter.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
				UpdateMeter();
			}

			public override void OnCleanUp()
			{
				base.OnCleanUp();
				Unsubscribe(-1697596308, UpdateMeter);
			}

			public void UpdateMeter(object data = null)
			{
				meter.SetPositionPercent(storage.Particles / storage.Capacity());
			}



			public void HarvestFromPOI(float dt)
			{
				Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.m_clustercraft;
				if (!CheckIfCanDrill())
					return;
				ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
				if (atCurrentLocation == null || atCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>() == null)
					return;
				StarmapHexCellInventory hexCellInventory = ClusterGrid.Instance.AddOrGetHexCellInventory(component.Location);
				HarvestablePOIStates.Instance smi = atCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
				Dictionary<SimHashes, float> elementsWithWeights = smi.configuration.GetElementsWithWeights();
				float weightedMaxMass = 0.0f;
				foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
					weightedMaxMass += keyValuePair.Value;
				foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
				{
					Element elementByHash = ElementLoader.FindElementByHash(keyValuePair.Key);
					if (!DiscoveredResources.Instance.IsDiscovered(elementByHash.tag))
						DiscoveredResources.Instance.Discover(elementByHash.tag, elementByHash.GetMaterialCategoryTag());
				}
				float maxMassDrilled = Mathf.Min(this.GetMaxExtractKGFromHEPsAvailable(), this.def.harvestSpeed * dt);
				float consumptionMass = 0.0f;
				foreach (KeyValuePair<SimHashes, float> keyValuePair in elementsWithWeights)
				{
					if ((double)consumptionMass < (double)maxMassDrilled)
					{
						SimHashes key = keyValuePair.Key;
						float num4 = keyValuePair.Value / weightedMaxMass;
						float mass = this.def.harvestSpeed * dt * num4;
						Element elementByHash = ElementLoader.FindElementByHash(key);
						hexCellInventory.AddItem(elementByHash, mass);
						consumptionMass += mass;
					}
					else
						break;
				}
				smi.DeltaPOICapacity(-consumptionMass);
				ConsumeParticles(consumptionMass * HEPConsumptionRate);
				SaveGame.Instance.ColonyAchievementTracker.totalMaterialsHarvestFromPOI += consumptionMass;
			}

			public void ConsumeParticles(float amount) => GetComponent<HighEnergyParticleStorage>().ConsumeIgnoringDisease(GameTags.HighEnergyParticle, amount);

			public float GetMaxExtractKGFromHEPsAvailable() => GetComponent<HighEnergyParticleStorage>().Particles / HEPConsumptionRate;

			public bool CheckIfCanDrill()
			{
				Clustercraft component = GetComponent<RocketModuleCluster>().CraftInterface.GetComponent<Clustercraft>();
				if (component == null)
				{
					sm.canHarvest.Set(false, this);
					return false;
				}

				if ((double)GetMaxExtractKGFromHEPsAvailable() <= 0.0)
				{
					//UpdateMethod.Invoke(loopmaster, new[] { (System.Object)null });
					sm.canHarvest.Set(false, this);
					return false;
				}
				ClusterGridEntity atCurrentLocation = component.GetPOIAtCurrentLocation();
				bool flag = false;
				if (atCurrentLocation != null && (bool)atCurrentLocation.GetComponent<HarvestablePOIClusterGridEntity>())
				{
					HarvestablePOIStates.Instance smi = atCurrentLocation.GetSMI<HarvestablePOIStates.Instance>();
					if (!smi.POICanBeHarvested())
					{
						sm.canHarvest.Set(false, this);
						return false;
					}
					foreach (KeyValuePair<SimHashes, float> elementsWithWeight in smi.configuration.GetElementsWithWeights())
					{
						Element elementByHash = ElementLoader.FindElementByHash(elementsWithWeight.Key);
						CargoBay.CargoType stateToCargoType = CargoBay.ElementStateToCargoTypes[elementByHash.state & Element.State.Solid];
						List<CargoBayCluster> cargoBaysOfType = component.GetCargoBaysOfType(stateToCargoType);
						if (cargoBaysOfType != null && cargoBaysOfType.Count > 0)
						{
							foreach (CargoBayCluster cargoBayCluster in cargoBaysOfType)
							{
								if ((double)cargoBayCluster.storage.RemainingCapacity() > 0.0)
									flag = true;
							}
							if (flag)
								break;
						}
					}
				}
				sm.canHarvest.Set(flag, this);
				return flag;
			}

			public static void AddHarvestStatusItems(GameObject statusTarget, float harvestRate, Clustercraft craft)
			{
				if (statusTarget.TryGetComponent<KSelectable>(out var selectable))
				{
					selectable.AddStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting, new Tuple<Clustercraft,float>(craft,harvestRate));
					BuildBoostStatusString(statusTarget, harvestRate, selectable);
				}
			}

			public static void RemoveHarvestStatusItems(GameObject statusTarget)
			{
				if (statusTarget.TryGetComponent<KSelectable>(out var selectable))
				{
					selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.SpacePOIHarvesting);
					selectable.RemoveStatusItem(ModStatusItems.RTB_MiningInformationBoons);
				}
			}
			static void BuildBoostStatusString(GameObject statusTarget, float harvestRate, KSelectable selectable)
			{
				statusTarget.TryGetComponent<CraftModuleInterface>(out var CraftInterface);
				float pilotBoost = ModAssets.GetMiningPilotSkillMultiplier(CraftInterface.m_clustercraft);
				float totalBoostPercentage = pilotBoost;

				float boostPercentagePilot = pilotBoost < 1f ? (1f - pilotBoost) * -100f : (pilotBoost - 1) * 100f;
				float totalSupportBoostPercentage = totalBoostPercentage < 1f ? (1f - totalBoostPercentage) * -1 : (totalBoostPercentage - 1);

				string tooltipString = RTB_MININGINFORMATIONBOONS.TOOLTIPINFO
					.Replace("{RATEPERCENTAGE}", totalSupportBoostPercentage.ToString("0%"))
					.Replace("{YIELDMASS}", harvestRate.ToString("0.00 Kg"))
					.Replace("{DRILLMATERIALMASS}", (harvestRate * HEPConsumptionRate).ToString("0.0"))
					.Replace("{DRILLMATERIAL}", global::STRINGS.ITEMS.RADIATION.HIGHENERGYPARITCLE.NAME);

				if (!Mathf.Approximately(boostPercentagePilot, 0f))
				{
					tooltipString += RTB_MININGINFORMATIONBOONS.PILOTSKILL
						.Replace("{BOOSTPERCENTAGE}", boostPercentagePilot.ToString("0"));
					tooltipString += "\n";
				}


				selectable.AddStatusItem(ModStatusItems.RTB_MiningInformationBoons, new Tuple<float, string>(totalSupportBoostPercentage, tooltipString));

				//selectable.SetStatusItem(Db.Get().StatusItemCategories.Suffocation, ModAssets.StatusItems.RTB_MiningInformationBoons, new Tuple<float, string>(harvestRate, tooltipString));

			}
		}
	}
}

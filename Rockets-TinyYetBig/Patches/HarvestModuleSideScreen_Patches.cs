using HarmonyLib;
using Rockets_TinyYetBig.Buildings.Utility;
using Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static STRINGS.UI.STARMAP.LAUNCHCHECKLIST;

namespace Rockets_TinyYetBig.Patches
{
	internal class HarvestModuleSideScreen_Patches
	{

		/// <summary>
		/// caches the support modules for this drillcone on selection
		/// </summary>
		[HarmonyPatch(typeof(HarvestModuleSideScreen), "SetTarget")]
		public static class TargetSetterPatch
		{
			public static void Postfix(GameObject target)
			{
				CorrectInfoScreenForSupportModules.Flush();
				if (!target.TryGetComponent<Clustercraft>(out var craft))
					return;

				foreach (var otherModule in craft.ModuleInterface.ClusterModules)
				{
					GameObject gameObject = otherModule.Get().gameObject;
					if (gameObject.GetDef<ResourceHarvestModule.Def>() != null)
					{
						var instance = gameObject.GetSMI<ResourceHarvestModule.StatesInstance>();
						CorrectInfoScreenForSupportModules.moduleInstance = instance;
						if (instance.gameObject.TryGetComponent<Storage>(out var storageOnTarget))
						{
							CorrectInfoScreenForSupportModules.drillerStorage = storageOnTarget;
						}
					}
					if(gameObject.TryGetComponent<ResourceHarvestModuleHEPInjector>(out var inject))
					{
						CorrectInfoScreenForSupportModules.drillerStorage = inject.storage;
						CorrectInfoScreenForSupportModules.HEP_Nosecone = inject;
					}

					if (gameObject.TryGetComponent<DrillConeAssistentModuleHEP>(out var moduleHEP))
					{
						CorrectInfoScreenForSupportModules.hepStorages.Add(moduleHEP.HEPStorage);
					}

					if (gameObject.TryGetComponent<DrillConeAssistentModule>(out var assistantModule))
					{
						CorrectInfoScreenForSupportModules.helperModules.Add(assistantModule);
					}
				}
			}
		}
		/// <summary>
		/// Way more efficient replacement for drillcone sidescreen that also includes speedboost and capacity increase from support modules
		/// </summary>
		[HarmonyPatch(typeof(HarvestModuleSideScreen), nameof(HarvestModuleSideScreen.SimEveryTick))]
		public static class CorrectInfoScreenForSupportModules
		{
			public static void Flush()
			{
				lastPercentageState = -2f;
				lastMassStored = -2f;
				helperModules.Clear();
				drillerStorage = null;
				moduleInstance = null;
				HEP_Nosecone = null;
				hepStorages.Clear();
			}
			public static ResourceHarvestModuleHEPInjector HEP_Nosecone = null;
			public static HashSet<DrillConeAssistentModule> helperModules = [];
			public static HashSet<HighEnergyParticleStorage> hepStorages = [];
			static float globalDT = 0f;
			const float dtGate = 1 / 5f;
			static float lastPercentageState = -1f;
			static float lastMassStored = -1f;
			public static IStorage drillerStorage = null;
			public static ResourceHarvestModule.StatesInstance moduleInstance;
			static bool IsHEPNosecone => HEP_Nosecone != null;

			public static bool Prefix(float dt, HarvestModuleSideScreen __instance, Clustercraft ___targetCraft)
			{
				if (globalDT < dtGate)
				{
					globalDT += dt;
				}
				else
				{
					globalDT -= dtGate;

					if (___targetCraft.IsNullOrDestroyed())
						return false;

					float Capacity = 0, MassStored = 0;
					if (drillerStorage != null)
					{
						float storageCapacity = drillerStorage.Capacity();
						Capacity += storageCapacity;
						MassStored += storageCapacity - drillerStorage.RemainingCapacity();
					}
					if (IsHEPNosecone)
					{
						foreach (var module in hepStorages)
						{
							Capacity += module.Capacity();
							MassStored += module.Particles;
						}
					}
					else
					{
						foreach (var module in helperModules)
						{
							Capacity += module.DiamondStorage.Capacity();
							MassStored += module.DiamondStorage.MassStored();
						}
					}

					__instance.TryGetComponent(out HierarchyReferences hr);
					float miningProgress = moduleInstance.sm.canHarvest.Get(moduleInstance) ? moduleInstance.timeinstate % 4f / 4f : -1f;

					if (!Mathf.Approximately(miningProgress, lastPercentageState))
					{
						GenericUIProgressBar reference1 = hr.GetReference<GenericUIProgressBar>("progressBar");
						reference1.SetFillPercentage(miningProgress > -1f ? miningProgress : 0f);
						reference1.label.SetText(miningProgress > -1f ?
							global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_IN_PROGRESS :
							global::STRINGS.UI.UISIDESCREENS.HARVESTMODULESIDESCREEN.MINING_STOPPED);
						lastPercentageState = miningProgress;
					}
					if (!Mathf.Approximately(MassStored, lastMassStored))
					{
						GenericUIProgressBar reference2 = hr.GetReference<GenericUIProgressBar>("diamondProgressBar");
						reference2.SetFillPercentage(MassStored / Capacity);
						if(HEP_Nosecone != null)
						{
							reference2.label.SetText(global::STRINGS.UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES + ": " + MassStored.ToString("0.#"));
						}
						else
						{
							reference2.label.SetText(ElementLoader.GetElement(SimHashes.Diamond.CreateTag()).name + ": " + GameUtil.GetFormattedMass(MassStored));
						}
						lastMassStored = MassStored;
					}
				}
				return false;
			}
		}
	}
}

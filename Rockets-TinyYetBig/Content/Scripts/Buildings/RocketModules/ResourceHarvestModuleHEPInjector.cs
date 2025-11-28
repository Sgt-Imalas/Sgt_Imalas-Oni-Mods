using Rockets_TinyYetBig.Behaviours;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{

	/// <summary>
	/// unused atm, eventually replace custom NoseConeHEPHarvest code to reuse base harvest module
	/// issue: no custom drill speed possible without transpilers
	/// </summary>

	internal class ResourceHarvestModuleHEPInjector : KMonoBehaviour
	{
		public const float HEPConsumptionRate = 0.25f;
		[MyCmpReq]
		public HighEnergyParticleStorage storage;
		[MyCmpReq]
		private RocketModuleCluster rocketModule;
		private MeterController meter;
		public int onStorageChangeHandle = -1;

		public override void OnSpawn()
		{
			SgtLogger.l("ResourceHarvestModuleHEPInjector.OnSpawn");


			rocketModule.AddModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, new ConditionHasRadbolts(storage, Config.Instance.LaserDrillconeCapacity));
			onStorageChangeHandle = Subscribe((int)GameHashes.OnStorageChange, Inject_UpdateMeter);
			//meter = new MeterController(GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, []);

			//var kbat = meter.gameObject.GetComponent<KBatchedAnimTracker>();
			//kbat.matchParentOffset = true;
			//kbat.forceAlwaysAlive = true;
			base.OnSpawn();
			Inject_UpdateMeter();
		}

		public void Inject_Constructor(ResourceHarvestModule.StatesInstance smi, IStateMachineTarget master, ResourceHarvestModule.Def def)
		{
			SgtLogger.l("ResourceHarvestModuleHEP.OnInit");
			//smi.meter = meter;
			meter = smi.meter;
			var module = GetComponent<RocketModuleCluster>();
			module.RemoveModuleCondition(ProcessCondition.ProcessConditionType.RocketStorage, (condition) => condition is ConditionHasResource chr);
		}


		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe(onStorageChangeHandle);
		}

		public void Inject_UpdateMeter(object data = null)
		{
			if (storage == null)
			{
				SgtLogger.warning("storage was null!");
				return;
			}
			if (meter == null)
			{
				SgtLogger.warning("meter was null!");
				return;
			} 
			meter.SetPositionPercent(storage.Particles / storage.Capacity());
		}
		public bool HasAnyAmountOfHEP() => GetMaxExtractKGFromHEPsAvailable() > 0.0f;
		public void ConsumeParticles(float amount) => storage.ConsumeAndGet(amount);
		public float GetMaxExtractKGFromHEPsAvailable() => storage.Particles / HEPConsumptionRate;
		public bool Inject_HasAnyAmountOfDiamond() => HasAnyAmountOfHEP();

		public void Inject_ConsumeDiamond(float amount)
		{
			amount /= NoseconeHarvestConfig.DIAMOND_CONSUMED_PER_HARVEST_KG;
			amount *= HEPConsumptionRate;
			ConsumeParticles(amount);
		}
		public float Inject_GetMaxExtractKGFromDiamondAvailable()
		{
			float kgExtraction = GetMaxExtractKGFromHEPsAvailable();
			return kgExtraction;
		}
	}
}


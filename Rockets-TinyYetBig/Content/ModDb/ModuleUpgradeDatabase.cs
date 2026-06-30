using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.ModDb
{
	public static class ModuleUpgradeDatabase
	{
		static Dictionary<string, RocketModuleUpgrade> _upgrades = [];
		internal static void Add(RocketModuleUpgrade rocketModuleUpgrade) => _upgrades[rocketModuleUpgrade.ID] = rocketModuleUpgrade;

		//no add/remove action, is handled by the logic component in the cargo bay
		public static RocketModuleUpgrade CargoBayFilter =
			new RocketModuleUpgrade("RTB_CargoBayFilter")
			.Costs([GameTags.Steel, GameTags.RefinedMetal], [50, 50]);

		public static RocketModuleUpgrade CargoBayInsulation =
			new RocketModuleUpgrade("RTB_CargoBayInsulation")
			.Costs([GameTags.Insulator], [800])
			.OnAdd(module => ToggleCargoBayInsulation(module,true))
			.OnRemove(module => ToggleCargoBayInsulation(module, false));

		//Potential ideas:
		public static RocketModuleUpgrade 
			CargoBayCapacityLvl1,
			CargoBayCapacityLvl2,
			CargoBayCapacityLvl3,
			
			CargoBayCollectionSpeed
			;

		static void ToggleCargoBayInsulation(GameObject cargobayGO, bool enableInsulation)
		{
			if (!cargobayGO.TryGetComponent<CargoBayCluster>(out var cargoBay))
			{
				SgtLogger.warning("No cargo bay found on " + cargobayGO);
				return;
			}
			if (cargoBay.storageType == CargoBay.CargoType.Entities)
				return;
			var storage = cargoBay.storage;
			var targetModifiers = enableInsulation ? Storage.StandardInsulatedStorage : Storage.StandardSealedStorage;

			if (!storage.defaultStoredItemModifers.SequenceEqual(targetModifiers))
			{
				storage.SetDefaultStoredItemModifiers(targetModifiers);
				ApplyModifiedModifiers(storage);
				if(enableInsulation)
					SgtLogger.l("Adding Insulation to cargobay contents of "+cargobayGO);
				else
					SgtLogger.l("Removing Insulation from cargobay contents of " + cargobayGO);

			}
		}
		static void ApplyModifiedModifiers(Storage storage)
		{
			foreach (GameObject item in storage.items)
			{
				storage.ApplyStoredItemModifiers(item, is_stored: true, is_initializing: true);
				if (storage.sendOnStoreOnSpawn)
				{
					item.Trigger((int)GameHashes.OnStore, storage);
				}
			}
		}


		public static bool TryGetUpgrade(string item, out RocketModuleUpgrade upgrade) => _upgrades.TryGetValue(item, out upgrade);

	}
	[Serializable]
	[SerializationConfig(MemberSerialization.OptIn)]
	public struct RocketModuleUpgradeInstance
	{

		[Serialize][SerializeField] string _upgradeId = null;
		[Serialize][SerializeField] Tag[] _resourceTags = null;
		[Serialize][SerializeField] float[] _resourceAmounts;

		public RocketModuleUpgradeInstance() { }

		public RocketModuleUpgradeInstance(string partId, Tag[] resourceTags, float[] resourceAmountMass)
		{
			_upgradeId = partId;
			_resourceTags = resourceTags;
			_resourceAmounts = resourceAmountMass;
		}
		public float[] SerializedAmounts => _resourceAmounts;
		public Tag[] SerializedMaterials => _resourceTags;
		public string UpgradeId => _upgradeId;

		internal static RocketModuleUpgradeInstance Create(GameObject gameObject)
		{
			if (!gameObject.TryGetComponent<Building>(out var building))
			{
				SgtLogger.error("no Building component on " + gameObject.name);
				return default;
			}
			if (!gameObject.TryGetComponent<Deconstructable>(out var deconstructable))
			{
				SgtLogger.error("no Deconstructable component on " + gameObject.name);
				return default;
			}
			return new RocketModuleUpgradeInstance(building.Def.PrefabID, [.. deconstructable.constructionElements], [.. building.Def.Mass]);
		}

		internal void Dismantle(Vector3 pos)
		{
			if (SerializedMaterials == null || SerializedAmounts == null) return;

			for (int i = 0; i < SerializedAmounts.Length; i++)
			{
				Tag resource = SerializedMaterials[i];
				float amount = SerializedAmounts[i];
				ModAssets.SpawnItem(resource, amount, pos);
			}
		}

		internal RocketModuleUpgrade GetSource()
		{
			return ModuleUpgradeDatabase.TryGetUpgrade(UpgradeId, out var upgrade) ? upgrade : null;
		}
	}
	public class RocketModuleUpgrade
	{
		public string ID;
		private System.Action<GameObject> onUpgradeAdded, onUpgradeRemoved;
		public Tag[] BuildTags;
		public float[] BuildCosts;
		public RocketModuleUpgrade(string id)
		{
			ID = id;
			ModuleUpgradeDatabase.Add(this);
		}
		public RocketModuleUpgrade OnAdd(System.Action<GameObject> onAddedAction = null)
		{
			onUpgradeAdded = onAddedAction;
			return this;
		}
		public RocketModuleUpgrade OnRemove(System.Action<GameObject> onRemovedAction = null)
		{
			onUpgradeRemoved = onRemovedAction;
			return this;
		}
		public RocketModuleUpgrade Costs(Tag[] tags, float[] amounts)
		{
			Debug.Assert(tags.Count() == amounts.Count(), "Failure to create RocketModuleUpgrade: tags count was not equal to amounts count");
			BuildTags = tags;
			BuildCosts = amounts;
			return this;
		}

		internal void OnUpgradeAdded(GameObject go)
		{
			if (onUpgradeAdded != null)
				onUpgradeAdded.Invoke(go);
		}
		internal void OnUpgradeRemoved(GameObject go)
		{
			if (onUpgradeRemoved != null)
				onUpgradeRemoved.Invoke(go);
		}
		public static implicit operator RocketModuleUpgrade(RocketModuleUpgradeInstance instance) => instance.GetSource();
	}
	internal static class UpgradeActions
	{
	}
}

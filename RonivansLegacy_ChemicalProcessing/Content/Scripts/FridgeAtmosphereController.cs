using KSerialization;
using RonivansLegacy_ChemicalProcessing.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class FridgeAtmosphereController : KMonoBehaviour, ISim1000ms
	{
		[MyCmpReq] KSelectable _selectable;
		[MyCmpReq] Operational _operational;

		[SerializeField]
		public Storage FoodStorage;
		[SerializeField]
		public Storage GasStorage;

		private float _gasConsumptionRate = 1f / 1000f;

		[Serialize]
		private bool _isPreserving = false;

		static readonly HashSet<int> SterileAtmosphereFoods = [];
		string _lastSterileElement;
		int _operationalHandle = -1;
		static readonly float TenSecHalflifePerSec = Mathf.Pow(0.5f, 1 / 10f);

		public static bool IsSterilizedFreezerFood(GameObject go)
		{
			if (go == null)
				return false;
			return SterileAtmosphereFoods.Contains(go.GetInstanceID());
		}

		public override void OnSpawn()
		{
			gameObject.GetInstanceID();
			FoodStorage.OnStorageChange += FoodStorageItemChanged;
			base.OnSpawn();

			_operationalHandle = Subscribe((int)GameHashes.ActiveChanged, FoodStorageItemChanged);

			_gasConsumptionRate = Config.Instance.DupesRefrigeration_SterileAtmosphereConsumption / 1000f;
			_isPreserving = CanPreserve();
			UpdatePreservationState();
		}
		public override void OnCleanUp()
		{
			Unsubscribe(_operationalHandle);
			if (!FoodStorage.IsNullOrDestroyed())
				FoodStorage.OnStorageChange -= FoodStorageItemChanged;
			base.OnCleanUp();
		}
		void FoodStorageItemChanged(object _)
		{
			_isPreserving = CanPreserve();
			UpdatePreservationState();
		}

		void SetPreserveItem(GameObject item, bool preserveItem)
		{
			if (preserveItem)
				SterileAtmosphereFoods.Add(item.GetInstanceID());
			else
				SterileAtmosphereFoods.Remove(item.GetInstanceID());
		}

		private bool CanPreserve()
		{
			var primaryElement = GasStorage.FindFirstWithMass(ModAssets.Tags.AIO_SterileAtmosphere, _gasConsumptionRate);
			if (primaryElement == null)
				return false;
			_lastSterileElement = primaryElement.Element.name;
			return true;
		}

		public void Sim1000ms(float dt)
		{
			bool canPreserve = CanPreserve();
			if (canPreserve != _isPreserving)
			{
				_isPreserving = canPreserve;
				UpdatePreservationState();
			}
			ConsumePreservingGas(dt);
		}
		private void ConsumePreservingGas(float dt)
		{
			if (FoodStorage.items.Count == 0 || !_operational.IsActive)
				return;

			float consumptionMass = dt * _gasConsumptionRate;
			var gas = GasStorage.FindFirstWithMass(ModAssets.Tags.AIO_SterileAtmosphere, consumptionMass);
			if (gas == null)
			{
				SgtLogger.warning("could not find consumable gas with mass " + consumptionMass);
				return;
			}

			SimHashes gasElement = gas.ElementID;


			foreach (var item in FoodStorage.items)
			{
				if (item.TryGetComponent<PrimaryElement>(out var primaryElement)
					&& primaryElement.DiseaseIdx != byte.MaxValue
					&& primaryElement.DiseaseCount > 0
					&& ModElements.DoesDiseaseKillGerm(primaryElement.DiseaseIdx, gasElement)
					)
				{
					float diseaseCount = primaryElement.DiseaseCount;

					int removeDisease = Mathf.CeilToInt(diseaseCount * (1f - (TenSecHalflifePerSec / dt)));
					if (diseaseCount == 1)
						removeDisease = 1;

					SgtLogger.l("Sterile Freezer Atmosphere: removing " + removeDisease + " (" + diseaseCount + ") germs from " + primaryElement);
					primaryElement.ModifyDiseaseCount(-removeDisease, "SterilizingFridgeAtmosphere");
				}

			}
			GasStorage.ConsumeIgnoringDisease(ModAssets.Tags.AIO_SterileAtmosphere, consumptionMass);
		}
		private void UpdatePreservationState()
		{
			foreach (var item in FoodStorage.items)
				SetPreserveItem(item, _isPreserving && _operational.IsActive);

			if (!CanPreserve() || !_operational.IsActive)
				_selectable.RemoveStatusItem(StatusItemsDatabase.Freezer_SterileAtmosphere);
			else if (!_selectable.HasStatusItem(StatusItemsDatabase.Freezer_SterileAtmosphere))
			{
				_selectable.AddStatusItem(StatusItemsDatabase.Freezer_SterileAtmosphere, this);
			}
		}
		internal static Func<string, object, string> HandleStatusItem()
		{
			return delegate (string str, object obj)
			{
				if (obj is not FridgeAtmosphereController ps)
					return str;

				return string.Format(str, ps._lastSterileElement);
			};
		}
	}
}

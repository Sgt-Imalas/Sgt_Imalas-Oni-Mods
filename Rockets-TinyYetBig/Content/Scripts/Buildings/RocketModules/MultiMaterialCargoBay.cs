using KSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class MultiMaterialCargoBay : KMonoBehaviour
	{
		[SerializeField] private float solidCapacity, liquidCapacity, gasCapacity;

		[MyCmpReq] public TreeFilterable Filterable;
		[MyCmpReq] public Storage Storage;

		public void Configure(float solid = 0f, float liquid = 0f, float gas = 0f)
		{
			solidCapacity = solid;
			liquidCapacity = liquid;
			gasCapacity = gas;
		}

		internal float RemainingCapacityFor(GameObject gameObject)
		{
			if (!gameObject.TryGetComponent<PrimaryElement>(out var primaryElement))
			{
				SgtLogger.warning("Tried to get remaining capacity for a game object without a PrimaryElement component, returning 0");
				return 0f;
			}
			if (HasSolidStorage && primaryElement.Element.IsSolid)
			{
				return solidCapacity - Storage.GetMassAvailable(GameTags.Solid);
			}
			else if (HasLiquidStorage && primaryElement.Element.IsLiquid)
			{
				return liquidCapacity - Storage.GetMassAvailable(GameTags.Liquid);
			}
			else if (HasGasStorage && primaryElement.Element.IsGas)
			{
				return gasCapacity - Storage.GetMassAvailable(GameTags.Gas);
			}
			return 0f;
		}

		internal float RemainingCapacityFor(CargoBay.CargoType cargoType)
		{
			switch (cargoType)
			{
				case CargoBay.CargoType.Solids:
					return HasSolidStorage ? solidCapacity - Storage.GetMassAvailable(GameTags.Solid) : 0f;
				case CargoBay.CargoType.Liquids:
					return HasLiquidStorage ? liquidCapacity - Storage.GetMassAvailable(GameTags.Liquid) : 0f;
				case CargoBay.CargoType.Gasses:
					return HasGasStorage ? gasCapacity - Storage.GetMassAvailable(GameTags.Gas) : 0f;
				default:
					return 0f;
			}
		}
		internal float GetMassFor(CargoBay.CargoType cargoType)
		{
			switch (cargoType)
			{
				case CargoBay.CargoType.Solids:
					return HasSolidStorage ? Storage.GetMassAvailable(GameTags.Solid) : 0f;
				case CargoBay.CargoType.Liquids:
					return HasLiquidStorage ? Storage.GetMassAvailable(GameTags.Liquid) : 0f;
				case CargoBay.CargoType.Gasses:
					return HasGasStorage ? Storage.GetMassAvailable(GameTags.Gas) : 0f;
				default:
					return 0f;
			}
		}
		public bool HasSolidStorage => solidCapacity > 0f;
		public bool HasLiquidStorage => liquidCapacity > 0f;
		public bool HasGasStorage => gasCapacity > 0f;

	}
}

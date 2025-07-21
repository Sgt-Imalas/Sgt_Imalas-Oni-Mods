using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class ConduitCapacityDescriptor : KMonoBehaviour, IGameObjectEffectDescriptor
	{
		[MyCmpGet] SolidConduit solidConduit;
		[MyCmpGet] Conduit conduit;
		[MyCmpGet] ConduitBridge bridge;
		[MyCmpGet] SolidConduitBridge solidBridge;
		[MyCmpGet] HighPressureConduit highPressure;
		[MyCmpGet] LogisticConduit logisticConduit;

		[SerializeField]
		public ConduitType Conduit = ConduitType.None;

		[SerializeField]
		public float CachedConduitCapacity = -1;

		bool cached = false;

		public string FormatDescriptor(string text, float capacity)
		{
			string mass = GameUtil.GetFormattedMass(capacity, massFormat: GameUtil.MetricMassFormat.Kilogram);
			return string.Format(text, mass);
		}

		void CacheTypes()
		{
			if (cached) return;
			cached = true;
			if (Conduit != ConduitType.None) return; //already cached

			if (logisticConduit != null)
			{
				Conduit = ConduitType.Solid;
				CachedConduitCapacity = HighPressureConduitRegistration.SolidCap_Logistic;
				return;
			}
			else if (conduit != null)
			{
				Conduit = conduit.ConduitType;
			}
			else if (bridge != null)
			{
				Conduit = bridge.type;
			}
			else if (solidConduit != null || solidBridge != null)
			{
				Conduit = ConduitType.Solid;
			}
			CachedConduitCapacity = HighPressureConduitRegistration.GetMaxConduitCapacity(Conduit, highPressure != null);
		}

		public List<Descriptor> GetDescriptors(GameObject go)
		{
			CacheTypes();

			if (Conduit == ConduitType.Solid)
				return [new(FormatDescriptor(STRINGS.BUILDING.EFFECTS.SOLID_TRANSFER_CAPACITY_LIMIT.NAME, CachedConduitCapacity), FormatDescriptor(STRINGS.BUILDING.EFFECTS.SOLID_TRANSFER_CAPACITY_LIMIT.TOOLTIP, CachedConduitCapacity))];
			return [new(FormatDescriptor(STRINGS.BUILDING.EFFECTS.TRANSFER_CAPACITY_LIMIT.NAME, CachedConduitCapacity), FormatDescriptor(STRINGS.BUILDING.EFFECTS.TRANSFER_CAPACITY_LIMIT.TOOLTIP, CachedConduitCapacity))];
		}
	}
}

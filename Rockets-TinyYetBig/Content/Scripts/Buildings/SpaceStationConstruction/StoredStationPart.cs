using KSerialization;
using System;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UtilLibs;
using static LogicGateBase;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction
{
	[Serializable]
	public struct StoredStationPart
	{
		[Serialize] Tag _partId = null;
		[Serialize] Tag[] _resourceTags = null;
		[Serialize] float[] _resourceAmounts;
		[Serialize] float _deconstructionTime;
		[Serialize] bool _canDeconstruct;
		[Serialize] string _name, _desc;

		public StoredStationPart(Tag partId, string name, string desc, Tag[] resourceTags, float[] resourceAmountMass, float deconstructionTime, bool canDismantle = true)
		{
			_partId = partId;
			_name = name;
			_desc = desc;

			_resourceTags = resourceTags;
			_resourceAmounts = resourceAmountMass;

			_deconstructionTime = deconstructionTime;
			_canDeconstruct = canDismantle;
		}
		public float[] SerializedAmounts => _resourceAmounts;
		public Tag[] SerializedMaterials => _resourceTags;
		public bool CanDeconstructPart => _canDeconstruct;
		public float DeconstructionTime => _deconstructionTime;

		public string Name => _name;
		public string Desc => _desc;
		public Tag PartId => _partId;

		internal static StoredStationPart Create(GameObject gameObject)
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
			return new StoredStationPart(building.Def.PrefabID, building.GetProperName(), building.description, [.. deconstructable.constructionElements], [.. building.Def.Mass], building.Def.ConstructionTime * 0.25f);
		}
	}
}

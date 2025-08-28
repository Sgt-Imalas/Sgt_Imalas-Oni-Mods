using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Descriptor;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts.Descriptors
{
    class FridgeSaverDescriptor : KMonoBehaviour, IGameObjectEffectDescriptor
	{
		
		public void Cache()
		{
			CachedWattage = this.GetDef<RefrigeratorController.Def>().powerSaverEnergyUsage;
			CachedMaxWattage = this.GetComponent<Building>().Def.EnergyConsumptionWhenActive;
		}

		[SerializeField]
		public ConduitType Conduit = ConduitType.None;

		[SerializeField]
		public float CachedWattage = -1;
		[SerializeField]
		public float CachedMaxWattage = -1;

		public string FormatDescriptor(string text, float wattage, float maxWattage = -1)
		{
			text = text.Replace("{UsedPower}", GameUtil.GetFormattedWattage(wattage));
			if(maxWattage > 0)
			{
				text = text.Replace("{MaxPower}", GameUtil.GetFormattedWattage(maxWattage));
			}
			return text;
		}
		public List<Descriptor> GetDescriptors(GameObject go)
		{
			return [new(
				FormatDescriptor(global::STRINGS.BUILDING.STATUSITEMS.FRIDGESTEADY.NAME, CachedWattage), 
				FormatDescriptor(global::STRINGS.BUILDING.STATUSITEMS.FRIDGESTEADY.TOOLTIP, CachedWattage, CachedMaxWattage),DescriptorType.Requirement)];
		}
	}
}

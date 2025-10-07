using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA.ConduitEvents
{
	class DamageEvent : IScheduledEvent
	{
		public DamageEvent(GameObject target, string eventName)
		{
			Target = target;
			EventDisplayName = eventName;
		}
		public GameObject Target { get; set; }
		public string EventDisplayName;
		public void ExecuteEventAction()
		{
			if (Target == null)
			{
				SgtLogger.l("DamageEvent: Target was null" );
				return;
			}
			Target.Trigger((int)GameHashes.DoBuildingDamage, GetPressureDamageSource(EventDisplayName));
		}

		public static BuildingHP.DamageSourceInfo GetPressureDamageSource(string popString)
		{
			return new()
			{
				damage = 1,
				source = global::STRINGS.BUILDINGS.DAMAGESOURCES.LIQUID_PRESSURE,
				popString = popString,
			};
		}
	}
}

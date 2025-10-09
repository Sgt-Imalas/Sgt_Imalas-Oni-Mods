using RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA.ConduitEvents;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA
{
    static class HighPressureConduitEventHandler
	{
		static List<IScheduledEvent> ScheduledEvents = new();

		static SchedulerHandle? handle = null;
		public static void ScheduleDropNotification(GameObject notificationSource, int mass, int capacity)
		{
			ScheduledEvents.Add(new NotificationEvent(notificationSource, STRINGS.BUILDING.STATUSITEMS.HPA_SOLIDCONDUITITEMDROPPED.NAME, string.Format(STRINGS.BUILDING.STATUSITEMS.HPA_SOLIDCONDUITITEMDROPPED.TOOLTIP, mass, capacity)));
			TriggerEventScheduler();
		}
		public static void ScheduleForDamage(GameObject receiver, int mass, int capacity)
		{
			ScheduledEvents.Add(new NotificationEvent(receiver, STRINGS.BUILDING.STATUSITEMS.HPA_CONDUITOVERPRESSURIZED.NAME, string.Format(STRINGS.BUILDING.STATUSITEMS.HPA_CONDUITOVERPRESSURIZED.TOOLTIP, mass, capacity)));
			ScheduledEvents.Add(new DamageEvent(receiver, global::STRINGS.UI.GAMEOBJECTEFFECTS.DAMAGE_POPS.LIQUID_PRESSURE));
			TriggerEventScheduler();
		}
		static void TriggerEventScheduler()
		{
			if (handle != null || handle.HasValue)
				return;
			handle = GameScheduler.Instance.ScheduleNextFrame("ExecuteQueuedEvents", (_) => ExecuteQueuedEventActions());
		}


		static void ExecuteQueuedEventActions()
		{
			if(ScheduledEvents == null)
			{
				SgtLogger.l("how was ScheduledEvents null?");
				ScheduledEvents = [];
			}

			foreach (var targetedEvent in ScheduledEvents)
			{
				if(targetedEvent == null)
				{
					SgtLogger.l("how was targetedEvent null?");
					continue;
				}
				targetedEvent.ExecuteEventAction();
			}
			handle = null;
			ScheduledEvents.Clear();
		}

		internal static void CancelPendingEvents()
		{
			if(handle.HasValue)
				GameScheduler.Instance.scheduler.Clear(handle.Value);
			handle = null;
			ScheduledEvents.Clear();
		}

		internal static void PressureDamageHandling(int cell, ConduitType type, float sentMass, float receiverMax)
		{
			if (sentMass >= receiverMax * 2f && UnityEngine.Random.value < 0.33f)
			{
				//This damage CANNOT be dealt immediately, or it will cause the game to crash. This code execution occurs during an UpdateNetworkTask execution and does not seem to support executing Triggers
				//The damage will instead be queued and dealt by a scheduler on the next tick
				ScheduleForDamage(HighPressureConduitRegistration.GetConduitAt(cell, type), (int)sentMass, (int)receiverMax);
			}
		}
		internal static void PressureDamageHandling(KMonoBehaviour receiver, float sentMass, float receiverMax) => PressureDamageHandling(receiver?.gameObject, sentMass, receiverMax);
		internal static void PressureDamageHandling(GameObject receiver, float sentMass, float receiverMax)
		{
			//33% chance to damage the receiver when sender has double its capacity if the receiver is not a bridge
			//if receiver is a bridge, 33% to damage the bridge if the sender's contents are above the bridge's capacity at all
			if (receiver != null && sentMass >= receiverMax * 2f && UnityEngine.Random.value < 0.33f)
			{
				//This damage CANNOT be dealt immediately, or it will cause the game to crash. This code execution occurs during an UpdateNetworkTask execution and does not seem to support executing Triggers
				//The damage will instead be queued and dealt by a scheduler on the next tick
				ScheduleForDamage(receiver, (int)sentMass, (int)receiverMax);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.HPA.ConduitEvents
{
	class NotificationEvent : IScheduledEvent
	{
		static Dictionary<GameObject, float> PreviousEventTriggerTimes = [];
		public Notification Notification { get; set; }
		public GameObject Target { get; set; }
		public NotificationEvent(GameObject target, string notificationName, string notificationDescription, NotificationType type = NotificationType.Bad)
		{
			Target = target;
			Notification = new Notification(notificationName, type, (_, _) => notificationDescription, click_focus: target.transform);
		}
		public void ExecuteEventAction()
		{
			if(Target == null || Notification == null)
			{
				SgtLogger.l("NotificationEvent: Target or Notification was null");
				return;
			}

			float currentTime = KTime.Instance.UnscaledGameTime;

			if (PreviousEventTriggerTimes.TryGetValue(Target, out float prevTime) && prevTime + 15 > currentTime)
				return;

			PreviousEventTriggerTimes[Target] = KTime.Instance.UnscaledGameTime;
			Target.AddOrGet<Notifier>()?.Add(Notification);
		}
	}
}

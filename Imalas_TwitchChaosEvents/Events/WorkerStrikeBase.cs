using UnityEngine;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	class WorkerStrikeBase
	{
		public static void StartStrike(int durationSeconds)
		{
			foreach (var dupe in Components.LiveMinionIdentities.Items)
			{
				if (dupe != null && dupe.gameObject != null)
				{
					ToggleStrikingOn(dupe.gameObject, true);
				}
			}
			ToastHelper.InstantiateToast(
				STRINGS.CHAOSEVENTS.WORKERSTRIKE.NAME,
				STRINGS.CHAOSEVENTS.WORKERSTRIKE.TOASTTEXT);

			GameScheduler.Instance.Schedule("end strike", durationSeconds, (_) =>
			{
				foreach (var dupe in Components.LiveMinionIdentities.Items)
				{
					if (dupe != null && dupe.gameObject != null)
					{
						ToggleStrikingOn(dupe.gameObject, false);
					}
				}
				ToastHelper.InstantiateToast(
					STRINGS.CHAOSEVENTS.WORKERSTRIKE.NAME,
					STRINGS.CHAOSEVENTS.WORKERSTRIKE.TOASTTEXT_END);
			});
		}
		public static void ToggleStrikingOn(GameObject minion, bool enable)
		{
			if (minion.TryGetComponent<KSelectable>(out var selectable))
			{
				if (enable)
				{
					selectable.AddStatusItem(ModAssets.StatusItems.WorkerOnStrike, minion.GetProperName());
					minion.GetComponent<ChoreConsumer>().choreRulesChanged?.Signal();
				}
				else
				{
					selectable.RemoveStatusItem(ModAssets.StatusItems.WorkerOnStrike);
					minion.GetComponent<ChoreConsumer>().choreRulesChanged?.Signal();
				}
			}
		}

	}
}

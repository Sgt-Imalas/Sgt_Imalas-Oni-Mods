using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// destroys 50% of food up to 1mil kcal
	/// </summary>
	internal class RoachInfestationEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_HungryRoachesEvent";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.HUNGRYROACHES.NAME;

		public EventWeight EventWeight => EventWeight.WEIGHT_RARE;

		public Action<object> EventAction => (object data) =>
		{
			var TargetWorld = GetValidWorld();

			if (TargetWorld == null)
			{
				ToastManager.InstantiateToast(
		  STRINGS.CHAOSEVENTS.HUNGRYROACHES.TOAST,
		   STRINGS.CHAOSEVENTS.HUNGRYROACHES.EVENTFAIL);
				return;
			}

			ToastManager.InstantiateToast(
				STRINGS.CHAOSEVENTS.HUNGRYROACHES.TOAST,
				 string.Format(STRINGS.CHAOSEVENTS.HUNGRYROACHES.TOASTTEXT, TargetWorld.GetProperName())
				 );

			float Limit = 1000000f; //no more than 1.000.000kcal;

			List<Pickupable> pickupables = TargetWorld.worldInventory.GetPickupables(GameTags.Edible).ToList();
			if (pickupables != null)
			{
				for (int i = pickupables.Count - 1; i >= 0; --i)
				{

					var item = pickupables[i];
					if (Limit <= 0)
						break;

					//SgtLogger.l("item: "+item.ToString());

					var removedFood = item.Take(item.TotalAmount / 2f);
					removedFood.TryGetComponent<Edible>(out var edible);
					//SgtLogger.l("remov " + edible.Calories);

					var fx = FXHelpers.CreateEffect("fly_swarm_kanim", item.transform.GetPosition() + new UnityEngine.Vector3(0, -0.7f, 0), item.transform).gameObject.GetComponent<KBatchedAnimController>();
					fx.Play("swarm_pre");
					fx.Queue("swarm_loop", KAnim.PlayMode.Loop);

					GameScheduler.Instance.Schedule("destroyFlyFX", 15, (_) => UnityEngine.Object.Destroy(fx));

					Limit -= edible.Calories / 1000f;
					UnityEngine.Object.Destroy(removedFood.gameObject);

				}
			}

		};

		public Func<object, bool> Condition =>
			(data) =>
			{
				return GetValidWorld() != null;
			};

		WorldContainer GetValidWorld()
		{
			var activeWorld = ClusterManager.Instance.activeWorld;

			if (WorldValidForRoaches(activeWorld))
			{
				return activeWorld;
			}

			foreach (var world in ClusterManager.Instance.WorldContainers)
			{
				if (WorldValidForRoaches(world))
				{
					return world;
				}
			}
			return null;
		}
		bool WorldValidForRoaches(WorldContainer world)
		{
			float availableCalories = RationTracker.Get().CountAmount(null, world.worldInventory);
			if (availableCalories > Components.LiveMinionIdentities.GetWorldItems(world.id).Count * 2000f)
			{
				return true;
			}
			return false;
		}


		public Danger EventDanger => Danger.Extreme;

	}
}

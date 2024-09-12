using AmogusMorb.TwitchEvents.TwitchEventAddons;
using ONITwitchLib;
using System;
using System.Linq;
using UnityEngine;
using Util_TwitchIntegrationLib;

namespace AmogusMorb.TwitchEvents.Events
{
	internal class ImposterAmongUsVentEvent : ITwitchEventBase
	{
		public string EventGroupID => null;
		public string ID => "RTB_TwitchEvent_AmongUsVent";
		public string EventName => "Vent Hopper";

		public Danger EventDanger => Danger.Small;
		public string EventDescription => "Something crawled out of the vent!";
		public EventWeight EventWeight => (EventWeight.WEIGHT_NORMAL);
		public Func<object, bool> Condition =>
				(data) =>
				{

					if (GameClock.Instance.GetCycle() < 25 || Components.MinionIdentities.Count < 5 || VentComponents.Vents.Count < 1)
						return false;
					return true;
				};
		public Action<object> EventAction =>
			(data) =>
			{
				if (VentComponents.Vents.Count == 0)
				{
					Debug.LogWarning("No Vents found");
					return;

				}
				var dupeCoords = VentComponents.Vents.Items
					.Where(vent => !vent.IsNullOrDestroyed())
					.Where(vent => Grid.Element[vent.NaturalBuildingCell()].id == SimHashes.Oxygen)
					.ToList();


				Vector3 targetCoords;
				if (dupeCoords.Count == 0)
					targetCoords = Grid.CellToPos(VentComponents.Vents.Items.GetRandom().NaturalBuildingCell());
				else
					targetCoords = Grid.CellToPos(dupeCoords.GetRandom().NaturalBuildingCell());


				GameObject pet = GameUtil.KInstantiate(Assets.GetPrefab(ImposterConfig.ID), targetCoords, Grid.SceneLayer.Creatures);
				pet.SetActive(true);
				ToastManager.InstantiateToastWithPosTarget(EventName, EventDescription, targetCoords);
			};
	}
}

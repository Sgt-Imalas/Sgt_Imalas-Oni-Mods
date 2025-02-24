using Imalas_TwitchChaosEvents.Buildings;
using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using ONITwitchLib.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	internal class CreeperExplosionEvent : ITwitchEventBase
	{
		private static readonly CellElementEvent spawnEvent = new(
			"ChaosTwitch_SpawnDeadlyElementCreeper",
			"Spawned by Twitch",
			true
		);
		public string ID => "ChaosTwitch_SpawnDeadlyElementCreeper";

		public string EventGroupID => null;

		public string EventName => Strings.Get("STRINGS.ONITWITCH.EVENTS.ELEMENT_GROUP_DEADLY");

		public EventWeight EventWeight => EventWeight.WEIGHT_RARE;


		public Action<object> EventAction => (_) =>
		{
			var cellNearMouse = PosUtil.RandomCellNearMouse();
			var cell = GridUtil.FindCellWithFoundationClearance(cellNearMouse);

			var insulationElement = ElementLoader.FindElementByHash(SimHashes.SuperInsulator);
			var creeper = ElementLoader.FindElementByHash(ModElements.Creeper);

			SimMessages.ReplaceAndDisplaceElement(
			cell,
				ModElements.Creeper,
				spawnEvent,
				100_000,
				creeper.defaultValues.temperature);


			foreach (var neighborCell in GridUtil.GetNeighborsInBounds(cell))
			{
				SimMessages.ReplaceAndDisplaceElement(
					neighborCell,
					insulationElement.id,
					spawnEvent,
					float.Epsilon,
					creeper.defaultValues.temperature
				);
			}
			var antiCheesePlate = Assets.GetBuildingDef(AntiCheeseBackwallConfig.ID);
			Vector3 positionCbc = Grid.CellToPosCBC(cell, antiCheesePlate.SceneLayer);
			GameObject building = antiCheesePlate.Create(positionCbc, null, [SimHashes.SandStone.CreateTag()], antiCheesePlate.CraftRecipe, 293.15f, antiCheesePlate.BuildingComplete);

		};
		
		public Func<object, bool> Condition => (_) => Config.Instance.SkipMinCycle || (GameClock.Instance.GetCycle() > 50);

		public Danger EventDanger => Danger.Deadly;
	}
}

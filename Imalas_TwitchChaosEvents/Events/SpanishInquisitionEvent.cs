using ONITwitchLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Util_TwitchIntegrationLib.Scripts;
using Util_TwitchIntegrationLib;
using UtilLibs;
using Klei.AI;
using TUNING;
using TemplateClasses;
using ONITwitchLib.Utils;
using static Operational;
using ONITwitchLib.Logger;
using FMOD.Studio;

namespace Imalas_TwitchChaosEvents.Events
{
	class SpanishInquisitionEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_SpanishInquisition";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.SPANISHINQUISITION.NAME;

		public EventWeight EventWeight => EventWeight.WEIGHT_RARE;


		static bool CanSpawnInquisition(int cell)
		{
			int left = Grid.CellLeft(cell);
			int right = Grid.CellRight(cell);
			bool leftValid = GridUtil.IsCellEmpty(Grid.CellAbove(left)) && (Grid.Foundation[Grid.CellBelow(left)] || Grid.IsSolidCell(Grid.CellBelow(left)));
			bool rightValid = GridUtil.IsCellEmpty(Grid.CellAbove(right)) && (Grid.Foundation[Grid.CellBelow(right)] || Grid.IsSolidCell(Grid.CellBelow(right)));
			bool topValid = GridUtil.IsCellEmpty(Grid.CellAbove(cell)) && (Grid.Foundation[Grid.CellBelow(cell)] || Grid.IsSolidCell(Grid.CellBelow(cell)));

			return leftValid && rightValid && topValid;
		}


		public Action<object> EventAction => (object data) =>
		{
			int centerCell = PosUtil.ClampedMouseCell();

			var validPos = GameUtil.FloodFillFind<object>(
							static (cell, _) => CanSpawnInquisition(cell),
							null,
							centerCell,
							40,
							false,
							false
							);
			if (Grid.IsValidCell(validPos))
				centerCell = validPos;
			else
			{
				if (Components.Telepads.Any())
					centerCell = Grid.PosToCell(Components.Telepads.Items[0]);


			}

			var bossGO= SpawnInquisitionMember(STRINGS.CHAOSEVENTS.SPANISHINQUISITION.INQUISITION_MEMBER_1, centerCell);
			SpawnInquisitionMember(STRINGS.CHAOSEVENTS.SPANISHINQUISITION.INQUISITION_MEMBER_2, Grid.CellLeft(centerCell));
			SpawnInquisitionMember(STRINGS.CHAOSEVENTS.SPANISHINQUISITION.INQUISITION_MEMBER_3, Grid.CellRight(centerCell));

			if(Config.Instance.SpanishInquisitionFX)
				SoundUtils.PlaySound(ModAssets.SOUNDS.SPANISH_INQUISITION, SoundUtils.GetSFXVolume() * 1.0f, true);

			ToastManager.InstantiateToastWithPosTarget(STRINGS.CHAOSEVENTS.SPANISHINQUISITION.TOAST, STRINGS.CHAOSEVENTS.SPANISHINQUISITION.TOASTTEXT, bossGO.transform.position);
		};

		GameObject SpawnInquisitionMember(string dupeName, int cell)
		{

			Personality personality = Db.Get().Personalities.GetRandom(GameTags.Minions.Models.Standard, true, false);
			while(personality.genderStringKey != "MALE") //the 3 guys in the meme are dudes
				personality = Db.Get().Personalities.GetRandom(GameTags.Minions.Models.Standard, true, false);
			var minionStartingStats = new MinionStartingStats(personality);
			minionStartingStats.Traits.Add(Db.Get().traits.TryGet(ModAssets.ModTraits.InquisitionMember));
			minionStartingStats.voiceIdx = -1;
			minionStartingStats.Name = dupeName;
			foreach (string key in DUPLICANTSTATS.ALL_ATTRIBUTES) //give some starting stats so they arent completely useless
				minionStartingStats.StartingLevels[key] += 4;

			GameObject prefab = Assets.GetPrefab((Tag)BaseMinionConfig.GetMinionIDForModel(personality.model));

			MinionIdentity minionIdentity = Util.KInstantiate<MinionIdentity>(prefab);
			minionIdentity.name = prefab.name;
			Immigration.Instance.ApplyDefaultPersonalPriorities(minionIdentity.gameObject);
			minionIdentity.gameObject.SetActive(true);
			minionStartingStats.Apply(minionIdentity.gameObject);

			MinionResume component = minionIdentity.GetComponent<MinionResume>();
			for (int index = 0; index < 2; ++index)//give some starting stats so they arent completely useless
				component.ForceAddSkillPoint();

			var position = Grid.CellToPos(cell);
			position.x += 0.5f;
			minionIdentity.transform.SetPosition(position);


			GameObject prefabClothes = Assets.GetPrefab(CustomClothingConfig.ID);
			var clothes = Util.KInstantiate(prefabClothes, position);
			clothes.SetActive(true);
			EquippableFacade.AddFacadeToEquippable(clothes.GetComponent<Equippable>(), "decor_02"); //red snazzier suit to mirror the red uniforms

			clothes.TryGetComponent<Equippable>(out var equippable);
			clothes.GetComponent<EquippableWorkable>()?.CancelChore("Manual equip");
			minionIdentity.ValidateProxy();
			Equipment equipment = minionIdentity.GetEquipment();
			equippable.Assign(minionIdentity);
			minionIdentity.GetEquipment()?.refreshHandle.ClearScheduler();
			equipment.Equip(equippable);

			Game.Instance.SpawnFX(SpawnFXHashes.MeteorImpactDust, cell, 0.0f);
			return minionIdentity.gameObject;

		}

		bool InquisitionMembersAlive()
		{
			foreach (var dupe in Components.LiveMinionIdentities.Items)
			{
				if (dupe.TryGetComponent<Traits>(out var traits) && traits.HasTrait(ModAssets.ModTraits.InquisitionMember))
				{
					return true;
				}
			}
			return false;
		}

		public Func<object, bool> Condition =>
			(data) =>
			{
				//3 extra dupes are a bit much very early
				return (Config.Instance.SkipMinCycle || GameClock.Instance.GetCycle() > 32) && !InquisitionMembersAlive();
			};

		public Danger EventDanger => Danger.None;

	}
}

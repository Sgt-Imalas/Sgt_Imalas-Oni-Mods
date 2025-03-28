﻿using HarmonyLib;
using Imalas_TwitchChaosEvents.Elements;
using ONITwitchLib;
using System;
using UnityEngine;
using Util_TwitchIntegrationLib;

namespace Imalas_TwitchChaosEvents.Events
{
	/// <summary>
	/// Fart V2
	/// </summary>
	internal class ShartEvent : ITwitchEventBase
	{
		public string ID => "ChaosTwitch_Shart";

		public string EventGroupID => null;

		public string EventName => STRINGS.CHAOSEVENTS.SHART.NAME;

		public EventWeight EventWeight => EventWeight.WEIGHT_UNCOMMON;

		public Action<object> EventAction => (obj) =>
		{
			foreach (var minion in Components.LiveMinionIdentities.Items)
			{
				if (minion.model == GameTags.Minions.Models.Standard)	
					DoShart(minion.gameObject, 25f, ModElements.LiquidPoop.SimHash);
				else if(minion.model == GameTags.Minions.Models.Bionic)
					DoShart(minion.gameObject, 25f, SimHashes.LiquidGunk);
			}

			DoCringeEffect();

			ToastManager.InstantiateToast(STRINGS.CHAOSEVENTS.SHART.TOAST, STRINGS.CHAOSEVENTS.SHART.TOASTTEXT);
		};


		private static readonly AccessTools.FieldRef<HashedString[]> WorkAnimsGetter =
			AccessTools.StaticFieldRefAccess<HashedString[]>(AccessTools.Field(typeof(Flatulence), "WorkLoopAnims"));

		// most of this logic copied from Asquared `Fart`-Event
		private static void DoShart(GameObject dupe, float shartMass, SimHashes shartElement)
		{
			var dupePos = dupe.transform.position;
			var temperature = Db.Get().Amounts.Temperature.Lookup(dupe).value;
			var diseaseIDX = Db.Get().Diseases.GetIndex("FoodPoisoning");
			var diseaseCount = 200000;

			var equippable = dupe.GetComponent<SuitEquipper>().IsWearingAirtightSuit();
			if (equippable != null)
			{
				equippable.GetComponent<Storage>()
					.AddLiquid(shartElement, shartMass, temperature, diseaseIDX, diseaseCount, false);
			}
			else
			{
				SimMessages.AddRemoveSubstance(
					Grid.PosToCell(dupePos),
					shartElement,
					CellEventLogger.Instance.ElementConsumerSimUpdate,
					shartMass,
					temperature,
					diseaseIDX,
					diseaseCount
				);
				var effect = FXHelpers.CreateEffect(
					"odor_fx_kanim",
					dupePos,
					dupe.transform,
					true
				);
				effect.Play(WorkAnimsGetter());
				effect.destroyOnAnimComplete = true;
			}

			var objectIsSelectedAndVisible = SoundEvent.ObjectIsSelectedAndVisible(dupe);
			var audioPos = dupePos with { z = 0.0f };
			var volume = 1f;
			if (objectIsSelectedAndVisible)
			{
				audioPos = SoundEvent.AudioHighlightListenerPosition(audioPos);
				volume = SoundEvent.GetVolume(true);
			}
			//TODO: replace with shart sound!
			KFMOD.PlayOneShot(GlobalAssets.GetSound("Dupe_Flatulence"), audioPos, volume);
		}


		private static void DoCringeEffect()
		{
			foreach (var minionIdentity in Components.LiveMinionIdentities.Items)
			{
				minionIdentity.Trigger(
					(int)GameHashes.Cringe,
					Strings.Get("STRINGS.DUPLICANTS.DISEASES.PUTRIDODOUR.CRINGE_EFFECT").String
				);
				minionIdentity.gameObject.GetSMI<ThoughtGraph.Instance>().AddThought(Db.Get().Thoughts.PutridOdour);
			}
		}
		public Func<object, bool> Condition => (data) =>
		{
			return Components.LiveMinionIdentities.Count > 0;
		};

		public Danger EventDanger => Danger.Medium;
	}
}

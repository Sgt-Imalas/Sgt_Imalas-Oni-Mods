using AkisDecorPackB.Content.ModDb;
using Database;
using Klei.AI;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace AkisDecorPackB.Content.Scripts
{
	public class Inspiring : KMonoBehaviour
	{
		private EmoteReactable reactable;

		[MyCmpReq]
		private Exhibition exhibition;

		public override void OnSpawn()
		{
			base.OnSpawn();
			Subscribe((int)ModAssets.Hashes.FossilStageSet, OnStageChanged);
		}

		private void OnStageChanged(object obj)
		{
			if (AutoUnbox<ArtableStatuses.ArtableStatusType>.Unbox(obj, out var stage))
			{
				if (stage != ArtableStatuses.ArtableStatusType.AwaitingArting)
				{
					CreateReactable(stage);
				}
				else
				{
					RemoveReactable();
				}
			}
		}

		private void RemoveReactable()
		{
			if (reactable != null)
			{
				reactable.Cleanup();
			}

			reactable = null;
		}

		private Reactable CreateReactable(ArtableStatuses.ArtableStatusType statusItem)
		{
			Debug.Log("created reacatable");

			reactable = new EmoteReactable(
				gameObject,
				"DecorPackB_Reactable_Inspired",
				global::Db.Get().ChoreTypes.Emote,
				5,
				2);

			reactable.SetEmote(global::Db.Get().Emotes.Minion.ThumbsUp);
			reactable.RegisterEmoteStepCallbacks("react", go => OnEmote(go, statusItem), null);
			reactable.AddPrecondition(ReactorIsOnFloor);
			reactable.preventChoreInterruption = true;

			return reactable;
		}

		private bool ReactorIsOnFloor(GameObject reactor, Navigator.ActiveTransition transition)
		{
			return transition.end == NavType.Floor;
		}

		private void OnEmote(GameObject obj, ArtableStatuses.ArtableStatusType status)
		{
			Debug.Log("EMOTED");

			switch (status)
			{
				case ArtableStatuses.ArtableStatusType.LookingUgly:
					AddReactionEffect(obj, ModEffects.INSPIRED_LOW);
					break;
				case ArtableStatuses.ArtableStatusType.LookingOkay:
					AddReactionEffect(obj, ModEffects.INSPIRED_OKAY);
					break;
				case ArtableStatuses.ArtableStatusType.LookingGreat:
					AddReactionEffect(obj, ModEffects.INSPIRED_GREAT);
					break;
			}
		}

		private void AddReactionEffect(GameObject reactor, string effect)
		{
			if(!reactor.TryGetComponent<Effects>(out var effects))
			{
				return;
			}
			var hasSmall = effects.HasEffect(ModEffects.INSPIRED_LOW);
			var hasMedium = effects.HasEffect(ModEffects.INSPIRED_OKAY);
			var hasSuper = effects.HasEffect(ModEffects.INSPIRED_GREAT);

			switch (effect)
			{
				case ModEffects.INSPIRED_LOW:
					if (!hasMedium && !hasSuper)
					{
						effects.Add(ModEffects.INSPIRED_LOW, true);
					}

					break;
				case ModEffects.INSPIRED_OKAY:
					effects.Remove(ModEffects.INSPIRED_LOW);

					if (!hasSuper)
					{
						effects.Add(ModEffects.INSPIRED_OKAY, true);
					}

					break;
				case ModEffects.INSPIRED_GREAT:
					effects.Remove(ModEffects.INSPIRED_LOW);
					effects.Remove(ModEffects.INSPIRED_OKAY);
					effects.Add(ModEffects.INSPIRED_GREAT, true);
					break;
				default:
					SgtLogger.warning($"Something went wrong trying to add an Inspired Reaction effect. Effect ({effect}) is invalid.");
					break;
			}
			;
		}
	}
}

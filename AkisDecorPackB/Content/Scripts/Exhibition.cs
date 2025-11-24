using AkisDecorPackB.Content.ModDb;
using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisDecorPackB.Content.Scripts
{
	public class Exhibition : Artable
	{
		[MyCmpGet] KSelectable selectable;

		[Serialize]
		public string CreatorName;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();

			workerStatusItem = global::Db.Get().DuplicantStatusItems.Arting;
			attributeConverter = global::Db.Get().AttributeConverters.ResearchSpeed;
			skillExperienceSkillGroup = global::Db.Get().SkillGroups.Research.Id;
			requiredSkillPerk = ModSkillPerks.Can_ReconstructFossil.Id;

			SetWorkTime(Mod.IsDev ? 8f : 80f);

			overrideAnims = [Assets.GetAnim("anim_interacts_sculpture_kanim")];

			synchronizeAnims = false;
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			shouldShowSkillPerkStatusItem = true;
			SetReconstructionStatusItem();
		}

		private ArtableStatusItem GetScientistSkill(WorkerBase worker)
		{
			var stati = Db.Get().ArtableStatuses;
			if (worker.TryGetComponent(out MinionResume resume))
			{
				if (resume.HasPerk(ModSkillPerks.Can_ReconstructFossil_Great))
				{
					return stati.LookingGreat;
				}
				else if (resume.HasPerk(ModSkillPerks.Can_ReconstructFossil_Okay))
				{
					return stati.LookingOkay;
				}
			}
			return stati.LookingUgly;
		}

		public override void OnCompleteWork(WorkerBase worker)
		{
			if (worker.TryGetComponent(out MinionIdentity identity))
			{
				CreatorName = identity.name;
			}

			if (userChosenTargetStage == null || userChosenTargetStage.IsNullOrWhiteSpace())
			{
				SetRandomStage(worker);
			}
			else
			{
				SetUserChosenStage();
			}

			shouldShowSkillPerkStatusItem = false;
			UpdateStatusItem(null);
			Prioritizable.RemoveRef(gameObject);
		}

		private void SetUserChosenStage()
		{
			SetStage(userChosenTargetStage, false);
			userChosenTargetStage = null;
		}
		private void SetReconstructionStatusItem()
		{
			if (CurrentStage != "Default" && !CreatorName.IsNullOrWhiteSpace())
				selectable.AddStatusItem(ModStatusItems.fossilReconstruction, this);
			else
				selectable.RemoveStatusItem(ModStatusItems.fossilReconstruction);
		}

		private void SetRandomStage(WorkerBase worker)
		{
			var scientistSkill = GetScientistSkill(worker);
			var potentialStages = global::Db.GetArtableStages().GetPrefabStages(this.PrefabID());

			potentialStages.RemoveAll(stage => stage.statusItem.StatusType != scientistSkill.StatusType);
			var selectedStage = potentialStages.GetRandom();

			SetStage(selectedStage.id, false);
			EmoteOnCompletion(worker, selectedStage);
		}

		public override void SetStage(string stage_id, bool skip_effect)
		{
			base.SetStage(stage_id, skip_effect);

			var stage = global::Db.GetArtableStages().Get(CurrentStage);
			if (stage != null)
			{
				BoxingTrigger(ModAssets.Hashes.FossilStageSet, stage.statusItem.StatusType);
			}
			SetReconstructionStatusItem();
		}

		private static void EmoteOnCompletion(WorkerBase worker, ArtableStage stage)
		{
			if (stage.cheerOnComplete)
			{
				new EmoteChore(worker.GetComponent<ChoreProvider>(), global::Db.Get().ChoreTypes.EmoteHighPriority, "anim_cheer_kanim",
				[
					"cheer_pre",
					"cheer_loop",
					"cheer_pst"
				], null);
			}
			else
			{
				new EmoteChore(worker.GetComponent<ChoreProvider>(), global::Db.Get().ChoreTypes.EmoteHighPriority, "anim_disappointed_kanim",
				[
					"disappointed_pre",
					"disappointed_loop",
					"disappointed_pst"
				], null);
			}
		}
	}
}

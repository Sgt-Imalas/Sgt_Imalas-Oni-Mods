using Database;
using KSerialization;
using System.Collections.Generic;
using System.Linq;

namespace SkillingQueue
{
	internal class SavedSkillQueue : KMonoBehaviour, ISaveLoadable
	{
		[Serialize]
		private List<string> SkillIds;

		[MyCmpGet] MinionResume refResume;

		public int QueuedSkillCount => SkillIds.Count;

		internal SavedSkillQueue()
		{
			SkillIds = new List<string>(32);
		}
		public override void OnSpawn()
		{
			Patches.ResumeQueues.Add(refResume, this);
			Game.Instance.Subscribe((int)GameHashes.SkillPointAquired, OnSkillPointAquired);
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			Patches.ResumeQueues.Remove(refResume);
			Game.Instance.Unsubscribe((int)GameHashes.SkillPointAquired, OnSkillPointAquired);
			base.OnCleanUp();
		}

		public int GetFinalMorale()
		{
			int value = 0;
			var db = Db.Get().Skills;

			foreach (string id in SkillIds)
			{
				if (!refResume.HasMasteredSkill(id))
				{
					Skill skill = db.Get(id);
					if (refResume.HasSkillAptitude(skill))
					{
						value++;
					}
				}
			}
			return value;
		}
		public int GetFinalMoraleExpectancy()
		{
			int value = 0;
			var db = Db.Get().Skills;

			foreach (string id in SkillIds)
			{
				if (!refResume.HasMasteredSkill(id))
				{
					Skill skill = db.Get(id);
					value += skill.tier + 1;
				}
			}
			return value;
		}


		public void TryLearnSkills()
		{
			if (refResume.AvailableSkillpoints > 0)
			{
				OnSkillPointAquired(refResume);
			}
		}

		private void OnSkillPointAquired(object data)
		{
			if (data is not MinionResume resume)
				return;
			if (refResume != resume)
				return;
			if (SkillIds.Count == 0)
				return;

			int maxCount = SkillIds.Count;
			var dbskills = Db.Get().Skills;

			for (int i = 0; i < maxCount; i++)
			{
				if (refResume.AvailableSkillpoints <= 0)
					break;

				var SkillId = SkillIds.First();

				MinionResume.SkillMasteryConditions[] masteryConditions = refResume.GetSkillMasteryConditions(SkillId);

				if (refResume.HasMasteredSkill(SkillId))
					SkillIds.Remove(SkillId);

				if (refResume.CanMasterSkill(masteryConditions) && !refResume.HasMasteredSkill(SkillId))
				{
					SkillIds.Remove(SkillId);
					refResume.MasterSkill(SkillId);
					if (Config.Instance.NotificationOnSkill)
					{
						SkillLearnedMessage finishedMessage = new SkillLearnedMessage(resume, dbskills.Get(SkillId));
						Messenger.Instance.QueueMessage(finishedMessage);
						//GameScheduler.Instance.Schedule("remove message again", 300, (_) => Messenger.Instance.RemoveMessage(finishedMessage));
					}

				}
			}
			if (SkillIds.Count == 0 && Config.Instance.NotificationOnQueueComplete)
			{
				SkillQueueCompleteMessage allfinishedMessage = new SkillQueueCompleteMessage(resume);
				MusicManager.instance.PlaySong("Stinger_ResearchComplete");
				Messenger.Instance.QueueMessage(allfinishedMessage);
			}
		}

		public bool HasSkillQueued(string skillId, out int index)
		{
			index = SkillIds.FindIndex(item => item == skillId);

			return SkillIds.Contains(skillId);
		}

		private void RemoveSkill(string skillId)
		{
			SkillIds.RemoveAll(id => id == skillId);
		}
		private void AddSkillToQueue(string skillId, int InsertOverride = -1)
		{
			if (SkillIds.Contains(skillId))
				return;

			if (InsertOverride == -1)
				SkillIds.Add(skillId);
			else
				SkillIds.Insert(InsertOverride, skillId);
		}
		public void AddSkillWithPrerequisites(string skillId)
		{
			var dbSkills = Db.Get().Skills;
			AddSkillRecursively(skillId, dbSkills);
		}

		private void AddSkillRecursively(string skillId, Skills dbSkills, int InsertStart = -1)
		{
			if (SkillIds.Contains(skillId))
				return;


			Skill skill = dbSkills.Get(skillId);
			if (InsertStart == -1)
				InsertStart = SkillIds.Count;
			if (!refResume.HasMasteredSkill(skillId))
				AddSkillToQueue(skillId, InsertStart);


			foreach (var prevSkill in skill.priorSkills)
			{
				AddSkillRecursively(prevSkill, dbSkills, InsertStart);
			}

		}
		public void LearnedSkill(string skillId)
		{
			RemoveSkill(skillId);
		}
		public void RemoveSkillAndPostrequisites(string skillId)
		{
			RemoveSkillRecursively(skillId, Db.Get().Skills);
		}
		private void RemoveSkillRecursively(string skillId, Skills dbSkills)
		{
			if (!SkillIds.Contains(skillId))
				return;

			RemoveSkill(skillId);

			for (int i = SkillIds.Count - 1; i >= 0; --i)
			{
				var nextSkill = SkillIds[i];

				Skill skill = dbSkills.Get(nextSkill);
				if (skill.priorSkills.Contains(skillId))
				{
					RemoveSkillRecursively(nextSkill, dbSkills);
				}
			}

		}

		public bool CanMasterSkillAndPrerequisites(string skillId)
		{
			return CanMasterSkillRecursively(skillId, Db.Get().Skills);
		}

		private bool CanMasterSkillRecursively(string skillId, Skills dbSkills)
		{
			Skill skill = dbSkills.Get(skillId);
			MinionResume.SkillMasteryConditions[] masteryConditions = refResume.GetSkillMasteryConditions(skillId);
			if (masteryConditions.Contains(MinionResume.SkillMasteryConditions.UnableToLearn))
				return false;

			foreach (var prevSkill in skill.priorSkills)
			{
				if (!CanMasterSkillRecursively(prevSkill, dbSkills))
					return false;
			}
			return true;
		}
	}
}

using TUNING;

namespace LogicSatellites.Behaviours
{
	public class BuildingInternalConstructorRocketWorkable : Workable
	{
		private BuildingInternalConstructorRocket.Instance constructorInstance;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
			this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			this.minimumAttributeMultiplier = 0.75f;
			this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
			this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			this.resetProgressOnStop = false;
			this.multitoolContext = (HashedString)"build";
			this.multitoolHitEffectTag = (Tag)EffectConfigs.BuildSplashId;
			this.workingPstComplete = (HashedString[])null;
			this.workingPstFailed = (HashedString[])null;
			this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
			this.SetWorkTime(30f);
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			this.constructorInstance = this.GetSMI<BuildingInternalConstructorRocket.Instance>();
		}

		public override void OnCompleteWork(WorkerBase worker) => this.constructorInstance.ConstructionComplete();
	}
}

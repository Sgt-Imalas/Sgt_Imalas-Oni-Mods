using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

namespace OniRetroEdition.Buildings
{
	internal class GammaRayOvenRetro : ComplexFabricator, IGameObjectEffectDescriptor
	{
		[SerializeField]
		public int diseaseCountKillRate = 100;

		[MyCmpGet]
		RadiationEmitter emitter;
		public bool UseRads = true;
		[MyCmpGet]
		HighEnergyParticleStorage hepStorage;

		private MeterController storageMeter;

		[MyCmpGet]
		KBatchedAnimController kbac;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			choreType = Db.Get().ChoreTypes.Cook;
			fetchChoreTypeIdHash = Db.Get().ChoreTypes.CookFetch.IdHash;
		}

		void UpdateMeter(object data)
		{
			float maxValue = hepStorage.Capacity(), currentValue = hepStorage.Particles;

			float newMeterValue = Mathf.Clamp(currentValue / maxValue, 0, 1f);
			storageMeter?.SetPositionPercent(newMeterValue);

		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			workable.WorkerStatusItem = Db.Get().DuplicantStatusItems.Cooking;
			workable.overrideAnims = new KAnimFile[1] { Assets.GetAnim("anim_interacts_metalrefinery_retro_kanim") };
			workable.AttributeConverter = Db.Get().AttributeConverters.CookingSpeed;
			workable.AttributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			workable.SkillExperienceSkillGroup = Db.Get().SkillGroups.Cooking.Id;
			workable.SkillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			ComplexFabricatorWorkable complexFabricatorWorkable = workable;
			workable.synchronizeAnims = false;
			workable.SetOffsets(new[] { new CellOffset(1, 0) });
			workable.workLayer = Grid.SceneLayer.BuildingUse;

			if (UseRads)
			{
				workable.OnWorkableEventCB = OnWorkableAction;


				storageMeter = new MeterController(kbac, "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[]
				{
					"meter_target"
				});

				hepStorage.Subscribe((int)GameHashes.OnParticleStorageChanged, UpdateMeter);
			}

			complexFabricatorWorkable.OnWorkTickActions = (Action<Worker, float>)Delegate.Combine(complexFabricatorWorkable.OnWorkTickActions, (Action<Worker, float>)delegate (Worker worker, float dt)
			{
				Debug.Assert(worker != null, "How did we get a null worker?");
				if (diseaseCountKillRate > 0)
				{
					PrimaryElement component = GetComponent<PrimaryElement>();
					int num = Math.Max(1, (int)((float)diseaseCountKillRate * dt));
					component.ModifyDiseaseCount(-num, "GammaRayOven");
				}
			});
			if (UseRads)
			{
				emitter?.SetEmitting(false);
				UpdateMeter(null);
			}
		}
		void OnWorkableAction(Workable workable, Workable.WorkableEvent action)
		{
			switch (action)
			{
				case global::Workable.WorkableEvent.WorkStarted:
					emitter.SetEmitting(true);
					break;
				case global::Workable.WorkableEvent.WorkCompleted:
				case global::Workable.WorkableEvent.WorkStopped:
					emitter.SetEmitting(false); break;

			}
		}
		public override void OnCleanUp()
		{
			if (UseRads)
			{
				hepStorage.Unsubscribe((int)GameHashes.OnParticleStorageChanged, UpdateMeter);
			}
			base.OnCleanUp();
		}

		public override List<GameObject> SpawnOrderProduct(ComplexRecipe recipe)
		{
			List<GameObject> list = base.SpawnOrderProduct(recipe);
			foreach (GameObject item in list)
			{
				PrimaryElement component = item.GetComponent<PrimaryElement>();
				component.ModifyDiseaseCount(-component.DiseaseCount, "GammaRayOven.CompleteOrder");
			}

			operational.SetActive(value: false);
			return list;
		}

		public override List<Descriptor> GetDescriptors(GameObject go)
		{
			List<Descriptor> descriptors = base.GetDescriptors(go);
			descriptors.Add(new Descriptor(global::STRINGS.UI.BUILDINGEFFECTS.REMOVES_DISEASE, global::STRINGS.UI.BUILDINGEFFECTS.TOOLTIPS.REMOVES_DISEASE));
			return descriptors;
		}
	}
}

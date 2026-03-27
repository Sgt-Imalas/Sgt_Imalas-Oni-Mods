using KSerialization;
using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Text;
using TemplateClasses;
using TUNING;
using UnityEngine;
using UtilLibs;
using static ResearchTypes;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.SpaceStationConstruction
{
	internal class SpaceStationAttachablePartStorage : Workable, ISidescreenButtonControl
	{
		[Serialize] StoredStationPart? StoredPart = null;
		 CellOffset dropOffset = new(0, 1);
		[MyCmpReq] BuildingAttachPoint buildingAttachPoint;
		[MyCmpGet] KSelectable selectable;
		[MyCmpGet] KBatchedAnimController kbac;
		[Serialize] private bool isMarkedForPartDeconstruction;


		public Chore chore;

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			this.workerStatusItem = Db.Get().DuplicantStatusItems.Deconstructing;
			this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
			this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.MOST_DAY_EXPERIENCE;
			this.minimumAttributeMultiplier = 0.75f;
			this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
			this.skillExperienceMultiplier = SKILLS.MOST_DAY_EXPERIENCE;
			this.multitoolContext = (HashedString)"demolish";
			this.multitoolHitEffectTag = (Tag)EffectConfigs.DemolishSplashId;
			this.workingPstComplete = (HashedString[])null;
			this.workingPstFailed = (HashedString[])null;
			this.faceTargetWhenWorking = true;
		}
		public override Vector3 GetTargetPoint()
		{
			return  base.GetTargetPoint() + new Vector3(dropOffset.x,dropOffset.y);
		}
		public override void OnSpawn()
		{
			this.SetOffsetTable(OffsetGroups.InvertedWideTable);

			base.OnSpawn();
			dropOffset = buildingAttachPoint.points[1].position;
			RefreshAttachmentSlot();

			if (isMarkedForPartDeconstruction)
				QueuePartDeconstruct();
		}
		public override void OnCleanUp()
		{
			DismantleStoredPart();
			base.OnCleanUp();
		}

		public StoredStationPart? TakeStoredPart()
		{
			StoredStationPart? part = StoredPart;
			if (part.HasValue)
			{
				StoredPart = null;
			}
			RefreshAttachmentSlot();
			return part;
		}
		void RefreshAttachmentSlot()
		{
			bool hasPart = StoredPart != null;
			///prevent other parts from being constructed while the part storage is not empty;
			buildingAttachPoint.points[1].attachableType = hasPart ? GameTags.Empty : ModAssets.Tags.AttachmentSlotStationParts;

			if (hasPart)
			{
				SetWorkTime(StoredPart.Value.DeconstructionTime);
			}
			else
			{
			}
			kbac.SetSymbolVisiblity("storedPart", hasPart);

		}

		public void StoreNewPart(StoredStationPart part)
		{
			if(StoredPart != null)
			{
				SgtLogger.error("Tried storing a part in " + name + ", but it already held " + StoredPart.Value + "!");
				return;
			}
			StoredPart = part;
			RefreshAttachmentSlot();
		}

		public void DismantleStoredPart()
		{
			if (!StoredPart.HasValue)
				return;

			var part = TakeStoredPart().Value; 
			var pos = Grid.CellToPosCCC(Grid.OffsetCell(this.NaturalBuildingCell(), dropOffset), Grid.SceneLayer.Ore);

			for (int i = 0; i < part.SerializedMaterials.Length; i++)
			{
				Tag mat = part.SerializedMaterials[i];
				float amount = part.SerializedAmounts[i];
				GameObject item;
				Element element = ElementLoader.GetElement(mat);
				if (element != null)
				{
					if (element.IsGas)
						item = GasSourceManager.Instance.CreateChunk(element, amount, element.defaultValues.temperature, byte.MaxValue, 0, pos).gameObject;
					else if (element.IsLiquid)
						item = LiquidSourceManager.Instance.CreateChunk(element, amount, element.defaultValues.temperature, byte.MaxValue, 0, pos).gameObject;
					else if (element.IsSolid)
					{
						item = element.substance.SpawnResource(pos, amount, element.defaultValues.temperature, byte.MaxValue, 0, true, manual_activation: true);
						element.substance.ActivateSubstanceGameObject(item, byte.MaxValue, 0);
					}
				}
				else if (Assets.TryGetPrefab(mat))
				{
					item = Util.KInstantiate(Assets.TryGetPrefab(mat), pos);
					item.GetComponent<PrimaryElement>().Units = amount;
					item.SetActive(true);
				}
			}
		}

		public override void OnStartWork(WorkerBase worker)
		{
			this.progressBar.barColor = ProgressBarsConfig.Instance.GetBarColor("DeconstructBar");
			selectable.RemoveStatusItem(ModStatusItems.RTB_PendingStationPartDeconstruction);
		}
		public override void OnCompleteWork(WorkerBase worker)
		{
			base.OnCompleteWork(worker);
			DismantleStoredPart();
		}

		public void QueuePartDeconstruct()
		{
			if (DebugHandler.InstantBuildMode)
			{
				this.OnCompleteWork(null);
			}
			else
			{
				if (this.chore == null)
				{
					Prioritizable.AddRef(this.gameObject);
					this.requiredSkillPerk = Db.Get().SkillPerks.CanDemolish.Id;
					this.chore = (Chore)new WorkChore<SpaceStationAttachablePartStorage>(Db.Get().ChoreTypes.Demolish, this, only_when_operational: false, override_anims: Assets.GetAnim((HashedString)"anim_interacts_clothingfactory_kanim"), is_preemptable: true, ignore_building_assignment: true);
					selectable.AddStatusItem(ModStatusItems.RTB_PendingStationPartDeconstruction, this);
					this.isMarkedForPartDeconstruction = true;
					this.Trigger((int)GameHashes.WorkChoreDisabled, "Demolish");
				}
				this.UpdateStatusItem((object)null);
			}
		}

		public void CancelPartDeconstruct()
		{
			if (this.chore != null)
			{
				this.chore.Cancel("Cancelled demolition");
				this.chore = (Chore)null;
				selectable.RemoveStatusItem(ModStatusItems.RTB_PendingStationPartDeconstruction);
				this.ShowProgressBar(false);
				this.isMarkedForPartDeconstruction = false;
				Prioritizable.RemoveRef(this.gameObject);
			}
			this.UpdateStatusItem((object)null);
		}

		public string SidescreenButtonText => STRINGS.UI.RTB_DISMANTLESTATIONPART.LABEL;

		public string SidescreenButtonTooltip => STRINGS.UI.RTB_DISMANTLESTATIONPART.TOOLTIP;
		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride) { }

		public bool SidescreenEnabled() => true;

		public bool SidescreenButtonInteractable() => StoredPart != null && StoredPart.Value.CanDeconstructPart;

		public void OnSidescreenButtonPressed()
		{
			if (this.chore == null)
				this.QueuePartDeconstruct();
			else
				this.CancelPartDeconstruct();
		}

		public int HorizontalGroupID() => 0;

		public int ButtonSideScreenSortOrder() => 25;
	}
}

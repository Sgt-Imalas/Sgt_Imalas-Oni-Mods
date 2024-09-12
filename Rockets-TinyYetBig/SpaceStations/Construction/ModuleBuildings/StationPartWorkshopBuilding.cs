using KSerialization;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.SpaceStations.Construction.ModuleBuildings
{
	public class StationPartWorkshopBuilding : KMonoBehaviour, ISidescreenButtonControl
	{


		[Serialize]
		public StationPart StationPartRef;

		private CellOffset PartConstructionOffset = new CellOffset(0, 1);

		public string SidescreenButtonText => "Split into component parts"; ///TODO STRING

		public string SidescreenButtonTooltip => "splits down the finished module into transportable parts.";///TODO STRING

		public override void OnSpawn()
		{
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();

		}
		public int PartBottomPosition => Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour)this), this.PartConstructionOffset);
		public bool PartComplete()
		{
			int PartCount = 0;
			int TargetPartCount = 0;
			var attach = GetAttached();
			if (attach != null)
			{
				foreach (var part in AttachableBuilding.GetAttachedNetwork(attach))
				{
					if (part.TryGetComponent<StationPartBuilding>(out var partBuilding))
					{
						SgtLogger.l(partBuilding.ModuleParts.ToString(), "moduleparts");
						TargetPartCount = partBuilding.ModuleParts;
						PartCount++;
					}
				}
			}
			SgtLogger.l(PartCount + " " + TargetPartCount);
			return PartCount == TargetPartCount && TargetPartCount > 0;
		}

		public AttachableBuilding GetAttached()
		{
			Grid.ObjectLayers[(int)ObjectLayer.Building].TryGetValue(this.PartBottomPosition, out var bottomOfModule);
			if (bottomOfModule == null)
				return null;

			if (bottomOfModule.TryGetComponent<AttachableBuilding>(out var ReferenceBuilding))
			{
				return ReferenceBuilding;
			}
			return null;
		}


		public void DestroyConnected()
		{
			var attach = GetAttached();
			if (attach != null)
			{
				foreach (var part in AttachableBuilding.GetAttachedNetwork(attach))
				{
					if (part != this.gameObject && part.TryGetComponent<StationPartBuilding>(out var partBuilding))
					{
						partBuilding.DestroyWithoutDrops();
					}
				}
			}
		}
		public void DropConvertedResources()
		{
			Tag dropID = string.Empty;
			float dropAmount = 0f;
			var attach = GetAttached();
			if (attach != null)
			{
				foreach (var part in AttachableBuilding.GetAttachedNetwork(attach))
				{
					if (part != this.gameObject && part.TryGetComponent<StationPartBuilding>(out var partBuilding))
					{
						if (partBuilding.DropAmount > 0)
						{
							dropID = partBuilding.DropID;
							dropAmount = partBuilding.DropAmount;
							break;
						}
					}
				}
				if (dropAmount > 0)
				{
					this.TryGetComponent<PrimaryElement>(out var originPrimaryElement);

					GameObject loot = Util.KInstantiate(Assets.GetPrefab(dropID));
					loot.transform.SetPosition(Grid.CellToPosCCC(Grid.PosToCell(this), Grid.SceneLayer.Ore));
					PrimaryElement component2 = loot.GetComponent<PrimaryElement>();
					component2.Temperature = originPrimaryElement.Temperature;
					component2.Units = dropAmount;
					loot.SetActive(true);
				}
			}
		}

		public void DropTheResources()
		{
			DropConvertedResources();
			DestroyConnected();
		}


		public void SetButtonTextOverride(ButtonMenuTextOverride textOverride)
		{
		}

		public bool SidescreenEnabled() => true;

		public bool SidescreenButtonInteractable() => PartComplete();

		public void OnSidescreenButtonPressed() => DropTheResources();

		public int HorizontalGroupID() => -1;

		public int ButtonSideScreenSortOrder() => 21;
	}
}

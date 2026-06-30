using KSerialization;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules
{
	internal class RocketAttachableSocket : KMonoBehaviour
	{
		public static void ClearAll()
		{
			Components.Clear();
		}
		public static HashSet<RocketAttachableSocket> Components = [];

		[MyCmpGet] Rotatable rotatable;
		[MyCmpGet] RocketModuleUpgradeStorage upgradeStorage;

		[SerializeField]
		public CellOffset AttachmentOffset;
		[SerializeField]
		public Tag AttachmentTag = GameTags.Any;

		[SerializeField]
		[Serialize]
		bool _allowsAttachment = true;

		public bool AllowsAttachment => _allowsAttachment;

		public bool SetAllowsAttachment(bool value) => _allowsAttachment = value;

		public bool UpgradeProhibited(string upgradeId, out string upgradeFailureReason)
		{
			upgradeFailureReason = string.Empty;
			if (upgradeStorage == null || AttachmentTag != ModAssets.Tags.AttachmentSlotRocketModuleUpgrades)
				return false;
			return !upgradeStorage.UpgradeAllowed(upgradeId, out upgradeFailureReason);
		}

		public bool AcceptsAttachment(Tag connecting, int cell)
		{
			if (!_allowsAttachment)
				return false;

			var ownCell = Grid.PosToCell(this);
			if (!Grid.IsValidCell(ownCell))
				return false;

			var connectionCell = Grid.OffsetCell(ownCell, rotatable != null ? rotatable.GetRotatedCellOffset(AttachmentOffset) : AttachmentOffset);

			bool tagCorrect = AttachmentTag == GameTags.Any || AttachmentTag == connecting;
			bool cellCorrect = cell == connectionCell;

			return tagCorrect && cellCorrect;
		}
		public override void OnSpawn()
		{
			Components.Add(this);
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			Components.Remove(this);
			base.OnCleanUp();
		}

		internal void ConfigureAsUpgradeSlot()
		{
			int middle = Mathf.FloorToInt(Mathf.Max(GetComponent<Building>().Def.HeightInCells - 1, 1) / 2f);
			AttachmentTag = ModAssets.Tags.AttachmentSlotRocketModuleUpgrades;
			AttachmentOffset = new CellOffset(0, middle);

			//if (!attachPoint.points.Any(point => point.attachableType == ModAssets.Tags.AttachmentSlotRocketModuleUpgrades))
			//{
			//	attachPoint.points = attachPoint.points.Append(new BuildingAttachPoint.HardPoint(new CellOffset(0, middle), ModAssets.Tags.AttachmentSlotRocketModuleUpgrades, null));
			//}
			//attachPoint.TryAttachEmptyHardpoints();
		}

		internal static RocketAttachableSocket Get(AttachableBuilding attachableBuilding)
		{
			var attachmentTag = attachableBuilding.attachableToTag;
			var pos = Grid.OffsetCell(Grid.PosToCell(attachableBuilding), attachableBuilding.GetComponent<Building>()?.Def.attachablePosition ?? new CellOffset(0, 0));


			foreach (var attachable in Components)
			{
				if(attachable.AcceptsAttachment(attachmentTag,pos))
					return attachable;
			}
			return null;
		}
	}
}

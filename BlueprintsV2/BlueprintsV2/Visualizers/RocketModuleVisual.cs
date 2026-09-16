using BlueprintsV2.BlueprintData;
using BlueprintsV2.Visualizers;
using System.Collections.Generic;

namespace BlueprintsV2.BlueprintsV2.Visualizers
{
	internal class RocketModuleVisual : BuildingVisual, ICleanableVisual
	{
		static readonly Dictionary<ulong,HashSet<int>> AttachmentPoints = [];

		BuildingAttachPoint.HardPoint HardPoint;
		readonly bool _hasHardpoint = false;
		public RocketModuleVisual(BuildingConfig buildingConfig, int cell, ulong playerId) : base(buildingConfig, cell, playerId)
		{

			if(_def.BuildingComplete.TryGetComponent<BuildingAttachPoint>(out var BuildingAttachment))
			{
				for(int i = 0; i < BuildingAttachment.points.Length; i++)
				{
					var point = BuildingAttachment.points[i];
					if(point.attachableType == GameTags.Rocket)
					{
						_hasHardpoint = true;
						HardPoint = point;
						RegisterHardPoint(cell);
						break;
					}
				}
			}
		}
		protected override void CreateVisualizer()
		{
			if (!AttachmentPoints.ContainsKey(_playerId))
				AttachmentPoints[_playerId] = new HashSet<int>();
			base.CreateVisualizer();
		}

		public int DirtyCell { get; private set; } = -1;

		public override bool AllowedForRotation(Orientation rotation, bool flippedX, bool flippedY) => false;
		public override bool AllowedInWorld() => !ClusterManager.Instance?.activeWorld?.IsModuleInterior ?? false;

		public void Clean()
		{
			if(_hasHardpoint && DirtyCell >= 0)
			{
				AttachmentPoints[_playerId].Remove(cell);
				DirtyCell = -1;
			}
		}
		void RegisterHardPoint(int cellParam)
		{
			if (!_hasHardpoint)
				return;
			DirtyCell = Grid.OffsetCell(cellParam, HardPoint.position);
			AttachmentPoints[_playerId].Add(DirtyCell);
		}

		public override void MoveVisualizer(int cellParam, bool forceRedraw = false)
		{
			base.MoveVisualizer(cellParam, forceRedraw);
			Clean();
			RegisterHardPoint(cellParam);
		}

		public override PermittedRotations GetAllowedRotations() => PermittedRotations.FlipH;

		static HashSet<string> _attachmentFailsRocket = null;
		bool stringInit = false;
		public override bool IgnorableFailReason(int cellParam, string failReason)
		{
			if (base.IgnorableFailReason(cellParam, failReason))
				return true;
			if (!stringInit)
			{
				stringInit = true;
				_attachmentFailsRocket = [
					string.Format(global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_ATTACHPOINT, GameTags.Rocket)
					//, global::STRINGS.UI.TOOLTIPS.HELP_BUILDLOCATION_OCCUPIED
					];
			}
			if(_attachmentFailsRocket.Contains(failReason) && AttachmentPoints[_playerId].Contains(cellParam))
			{
				return true;
			}
			return false;
		}
	}
}

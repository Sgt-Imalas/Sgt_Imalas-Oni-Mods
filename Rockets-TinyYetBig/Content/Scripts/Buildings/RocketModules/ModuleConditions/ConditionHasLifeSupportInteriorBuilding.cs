using Rockets_TinyYetBig.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Text;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.RocketModules.ModuleConditions
{
	internal class ConditionHasLifeSupportInteriorBuilding : ProcessCondition
	{
		public ConditionHasLifeSupportInteriorBuilding(RocketModuleCluster clusterModule)
		{
			this.module = clusterModule;
		}
		RocketModuleCluster module;

		public override Status EvaluateCondition()
		{
			var moduleInterface = module.CraftInterface;
			var interior = moduleInterface.GetInteriorWorld();

			if (interior == null)
				return Status.Warning;

			var worldId = interior.id;

			if(worldId < 0)
				return Status.Warning;

			var targetBuildings = ModComps.InteriorLifeSupports.GetWorldItems(worldId);
			if(targetBuildings.Count == 0)
				return Status.Warning;

			return Status.Ready;
		}
		public override string GetStatusTooltip(Status status)
		{
			if (status == Status.Ready) return string.Format(STRINGS.UI.STARMAP.LAUNCHCHECKLIST.RTB_HASINTERIORBUILDING.STATUS.READY, STRINGS.BUILDINGS.PREFABS.RTB_INTERIOR_LIFESUPPORTACCESS.NAME);
			return string.Format(STRINGS.UI.STARMAP.LAUNCHCHECKLIST.RTB_HASINTERIORBUILDING.STATUS.FAILURE, STRINGS.BUILDINGS.PREFABS.RTB_INTERIOR_LIFESUPPORTACCESS.NAME);
		}
		public override string GetStatusMessage(Status status)
		{
			if (status == Status.Ready) return string.Format(STRINGS.UI.STARMAP.LAUNCHCHECKLIST.RTB_HASINTERIORBUILDING.TOOLTIP.READY, STRINGS.BUILDINGS.PREFABS.RTB_INTERIOR_LIFESUPPORTACCESS.NAME);
			return string.Format(STRINGS.UI.STARMAP.LAUNCHCHECKLIST.RTB_HASINTERIORBUILDING.TOOLTIP.FAILURE, STRINGS.BUILDINGS.PREFABS.RTB_INTERIOR_LIFESUPPORTACCESS.NAME);
		}

		public override bool ShowInUI() => this.EvaluateCondition() != ProcessCondition.Status.Ready;
	}
}

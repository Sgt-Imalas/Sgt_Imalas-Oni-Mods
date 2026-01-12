using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RoboRockets.Scripts
{
	internal class LimitOneAiCommandModule : SelectModuleCondition
	{
		public override bool EvaluateCondition(GameObject existingModule, BuildingDef selectedPart, SelectionContext selectionContext)
		{
			if (existingModule == null)
				return true;

			if (existingModule.TryGetComponent<BuildingUnderConstruction>(out var moduleUnderConstruction))
			{
				existingModule = moduleUnderConstruction.Def.BuildingComplete;
			}

			foreach (GameObject otherModuleGO in AttachableBuilding.GetAttachedNetwork(existingModule.GetComponent<AttachableBuilding>()))
			{
				GameObject moduleGOToCheck = otherModuleGO;
				if (moduleGOToCheck.TryGetComponent<BuildingUnderConstruction>(out var constructing))
					moduleGOToCheck = constructing.Def.BuildingComplete;

				if ((selectionContext != SelectionContext.ReplaceModule || otherModuleGO != existingModule.gameObject))
				{
					if (moduleGOToCheck.TryGetComponent<AIPassengerModule>(out _))
						return false;
				}
			}

			return true;
		}

		public override string GetStatusTooltip(bool ready, GameObject moduleBase, BuildingDef selectedPart)
		{
			return ready
				? global::STRINGS.UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ONE_ROBOPILOT_PER_ROCKET.COMPLETE
				: global::STRINGS.UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ONE_ROBOPILOT_PER_ROCKET.FAILED;

		}
	}
}

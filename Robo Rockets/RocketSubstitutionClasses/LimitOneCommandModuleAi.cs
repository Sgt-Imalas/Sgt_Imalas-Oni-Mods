
using RoboRockets;
using STRINGS;
using UnityEngine;

namespace KnastoronOniMods
{
	class LimitOneCommandModuleAi : LimitOneCommandModule
	{
		public override bool EvaluateCondition(
	   GameObject existingModule,
	   BuildingDef selectedPart,
	   SelectModuleCondition.SelectionContext selectionContext)
		{
			if (existingModule == null)
				return true;
			foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(existingModule.GetComponent<AttachableBuilding>()))
			{
				if ((selectionContext != SelectModuleCondition.SelectionContext.ReplaceModule || !(gameObject == existingModule.gameObject)) && (gameObject.GetComponent<RocketAiConditions>() != null || gameObject.GetComponent<BuildingUnderConstruction>() != null && gameObject.GetComponent<BuildingUnderConstruction>().Def.BuildingComplete.GetComponent<RocketCommandConditions>() != null))
					return false;
			}
			return true;
		}
		public override string GetStatusTooltip(
  bool ready,
  GameObject moduleBase,
  BuildingDef selectedPart)
		{
			return ready ? (string)UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ONE_COMMAND_PER_ROCKET.COMPLETE : (string)UI.UISIDESCREENS.SELECTMODULESIDESCREEN.CONSTRAINTS.ONE_COMMAND_PER_ROCKET.FAILED;
		}
	}
}

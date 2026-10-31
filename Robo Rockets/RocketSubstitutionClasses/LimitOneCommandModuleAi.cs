
using RoboRockets;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace KnastoronOniMods
{
	class LimitOneCommandModuleAi : LimitOneCommandModule
	{
		static readonly List<GameObject> _modulesPool = [];
		public override bool EvaluateCondition(
	   GameObject existingModule,
	   BuildingDef selectedPart,
	   SelectModuleCondition.SelectionContext selectionContext)
		{
			if (existingModule == null)
				return true;

			_modulesPool.Clear();
			AttachableBuilding.CollectAttachedBuildings(existingModule.GetComponent<AttachableBuilding>(), _modulesPool);
			foreach (GameObject gameObject in _modulesPool)
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

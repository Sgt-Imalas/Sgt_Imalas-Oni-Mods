using System.Collections.Generic;
using UnityEngine;

namespace Rockets_TinyYetBig.Behaviours
{
	public class OneModulePerRocket : SelectModuleCondition
	{
		string moduleID;
		public OneModulePerRocket(string id)
		{
			moduleID = id;
		}
		static readonly List<GameObject> _modulesPool = [];
		public override bool EvaluateCondition(
			GameObject existingModule,
			BuildingDef selectedPart,
			SelectModuleCondition.SelectionContext selectionContext)
		{
			if (existingModule == null|| !existingModule.TryGetComponent<AttachableBuilding>(out var attachable))
				return true;
			_modulesPool.Clear();
			AttachableBuilding.CollectAttachedBuildings(attachable, _modulesPool);
			foreach (GameObject gameObject in _modulesPool)
			{
				if ((selectionContext != SelectModuleCondition.SelectionContext.ReplaceModule || !(gameObject == existingModule.gameObject)) && gameObject.HasTag(RTGModuleConfig.RTB_RTG) || gameObject.GetComponent<BuildingUnderConstruction>() != null && gameObject.GetComponent<BuildingUnderConstruction>().Def.BuildingComplete.PrefabID() == moduleID)
					return false;
			}
			return true;
		}

		public override string GetStatusTooltip(
		  bool ready,
		  GameObject moduleBase,
		  BuildingDef selectedPart)
		{
			return ready ? STRINGS.UI_MOD.CONSTRAINTS.ONE_MODULE_PER_ROCKET.COMPLETE : STRINGS.UI_MOD.CONSTRAINTS.ONE_MODULE_PER_ROCKET.FAILED;
		}
	}
}

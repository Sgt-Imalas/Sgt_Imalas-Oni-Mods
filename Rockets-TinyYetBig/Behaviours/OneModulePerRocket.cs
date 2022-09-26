
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public override bool EvaluateCondition(
            GameObject existingModule,
            BuildingDef selectedPart,
            SelectModuleCondition.SelectionContext selectionContext)
        {
            if ((Object)existingModule == (Object)null)
                return true;
            foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(existingModule.GetComponent<AttachableBuilding>()))
            {
                if ((selectionContext != SelectModuleCondition.SelectionContext.ReplaceModule || !((Object)gameObject == (Object)existingModule.gameObject)) && gameObject.HasTag(RTGModuleConfig.RTB_RTG) || (Object)gameObject.GetComponent<BuildingUnderConstruction>() != (Object)null && gameObject.GetComponent<BuildingUnderConstruction>().Def.BuildingComplete.PrefabID() == moduleID)
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

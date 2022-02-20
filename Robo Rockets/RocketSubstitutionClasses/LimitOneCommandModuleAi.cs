
using Robo_Rockets;
using STRINGS;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            if ((Object)existingModule == (Object)null)
                return true;
            foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(existingModule.GetComponent<AttachableBuilding>()))
            {
                if ((selectionContext != SelectModuleCondition.SelectionContext.ReplaceModule || !((Object)gameObject == (Object)existingModule.gameObject)) && ((Object)gameObject.GetComponent<RocketAiConditions>() != (Object)null || (Object)gameObject.GetComponent<BuildingUnderConstruction>() != (Object)null && (Object)gameObject.GetComponent<BuildingUnderConstruction>().Def.BuildingComplete.GetComponent<RocketCommandConditions>() != (Object)null))
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

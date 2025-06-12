using HarmonyLib;
using Planticants.Content.ModDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Planticants.Patches
{
    class DropDown_Patches
    {

        [HarmonyPatch(typeof(DropDown), nameof(DropDown.Initialize))]
        public class DropDown_Initialize_Patch
        {
            public static void Prefix(DropDown __instance,ref IEnumerable<IListableOption> contentKeys)
            {
                if (!contentKeys.Any(key => key is CharacterContainer.MinionModelOption))
                    return;

                contentKeys = contentKeys.AddItem(new CharacterContainer.MinionModelOption(STRINGS.DUPLICANTS.MODEL.PLANT.NAME, [ModTags.PlantMinion], Assets.GetSprite((HashedString)"ui_duplicant_minion_selection")));
			}
        }
    }
}
    
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches.ResearchPatches
{
    class ResearchEntry_Patches
    {
        [HarmonyPatch(typeof(ResearchEntry), nameof(ResearchEntry.SetTech))]
        public class ResearchEntry_SetTech_Patch
        {
            /// <summary>
            /// sets the dlc for all RE buildings to a custom RE banner
            /// </summary>
            /// <param name="__instance"></param>
            public static void Postfix(ResearchEntry __instance)
            {
                int index = 1; //child 0 is the icon prefab

                var container = __instance.iconPanel.transform;
				foreach (TechItem unlockedItem in __instance.targetTech.unlockedItems)
                {
                    if (!Game.IsCorrectDlcActiveForCurrentSave(unlockedItem))
                        continue;
					var child = container.GetChild(index++);
                    if(child == null)
						continue;
					if (!unlockedItem.Id.StartsWith("RTB_") && !InjectionMethods.IsFromThisMod(unlockedItem.Id))
						continue;

					if (!child.TryGetComponent<HierarchyReferences>(out var hr))
                        continue;

					var dlcOverlay = hr.GetReference<KImage>("DLCOverlay");
					dlcOverlay.gameObject.SetActive(true);
					dlcOverlay.sprite = Assets.GetSprite("rtb_dlc_banner");
					dlcOverlay.color = Color.white;

                    if (child.TryGetComponent<ToolTip>(out var tt))
                    { 
                        tt.toolTip = $"{unlockedItem.Name}\n{unlockedItem.description}\n\n{STRINGS.UI.TOOLTIP_ADDON_RTB}";
					}
				}
            }
        }
    }
}

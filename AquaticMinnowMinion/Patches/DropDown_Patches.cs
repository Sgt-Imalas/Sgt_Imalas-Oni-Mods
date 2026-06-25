using AquaticMinnowMinion.Content.ModDb;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;
using static AquaticMinnowMinion.ModAssets;

namespace AquaticMinnowMinion.Patches
{
	class DropDown_Patches
	{

		[HarmonyPatch(typeof(DropDown), nameof(DropDown.Initialize))]
		public class DropDown_Initialize_Patch
		{
			public static void Prefix(DropDown __instance, ref IEnumerable<IListableOption> contentKeys)
			{
				if (!contentKeys.Any(key => key is CharacterContainer.MinionModelOption))
					return;

				bool bionicDlcActive = Game.IsDlcActiveForCurrentSave(DlcManager.DLC3_ID);

				var items = contentKeys.ToList();

				if (!bionicDlcActive)
				{
					for (int i = items.Count() - 1; i >= 0; --i)
					{
						var item = items[i];

						if (item is CharacterContainer.MinionModelOption option && option.permittedModels.Count == 1 && option.permittedModels[0] == GameTags.Minions.Models.Bionic)
						{
							items.RemoveAt(i);
							break;
						}
					}
				}
				if (Game.IsDlcActiveForCurrentSave(DlcManager.DLC5_ID))
					items.Add((new CharacterContainer.MinionModelOption(STRINGS.DUPLICANTS.MODEL.AQUATIC.NAME, [Tags.AquaticMinion], Assets.GetSprite((HashedString)"dreamIcon_Minnow"))));
				contentKeys = items;
			}
		}
	}
}

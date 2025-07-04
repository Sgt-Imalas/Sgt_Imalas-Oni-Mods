using BlueprintsV2.BlueprintData;
using BlueprintsV2.Tools;
using HarmonyLib;
using System.Collections.Generic;
using TMPro;
using static BlueprintsV2.ModAssets;

namespace BlueprintsV2.Patches
{
	internal class ToolPatches
	{

		[HarmonyPatch(typeof(FileNameDialog), "OnSpawn")]
		public static class FileNameDialogOnSpawn
		{
			//TODO!!!!!
			public static void Postfix(FileNameDialog __instance, TMP_InputField ___inputField)
			{
				if (__instance.name.StartsWith("BlueprintsV2_"))
				{
					___inputField.onValueChanged.RemoveAllListeners();
					___inputField.onEndEdit.RemoveAllListeners();

					if (__instance.name.StartsWith("BlueprintsV2_FolderDialog_"))
					{
						___inputField.onValueChanged.AddListener(delegate (string text)
						{
							for (int i = text.Length - 1; i >= 0; --i)
							{
								if (i < text.Length && ModAssets.BLUEPRINTS_PATH_DISALLOWEDCHARACTERS.Contains(text[i]))
								{
									text = text.Remove(i, 1);
								}
							}

							___inputField.text = text;
						});
					}
				}
			}
		}
	}
}

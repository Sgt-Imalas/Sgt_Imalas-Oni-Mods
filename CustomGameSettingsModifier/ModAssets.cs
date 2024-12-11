using UnityEngine;
using UtilLibs;

namespace CustomGameSettingsModifier
{
	internal class ModAssets
	{

		public static GameObject CustomGameSettings;
		public static void LoadAssets()
		{
			AssetBundle bundle = AssetUtils.LoadAssetBundle("customgamesettings_assets", platformSpecific: true);
			CustomGameSettings = bundle.LoadAsset<GameObject>("Assets/UIs/CustomGameSettingsChanger.prefab");

			//UIUtils.ListAllChildren(CustomGameSettings.transform);

			var TMPConverter = new TMPConverter();
			TMPConverter.ReplaceAllText(CustomGameSettings);

		}
	}
}

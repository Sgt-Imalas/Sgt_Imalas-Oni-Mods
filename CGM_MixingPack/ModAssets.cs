using Klei.AI;
using Klei.CustomSettings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace CGM_MixingPack
{
	internal class ModAssets
	{
		public class CGM_Addon_SubworldMixingSettingConfig : SubworldMixingSettingConfig
		{
			public CGM_Addon_SubworldMixingSettingConfig(string id, string worldgenPath, string[] required_content = null, string dlcIdFrom = null, bool triggers_custom_game = true, long coordinate_range = 5) : base(id, worldgenPath, required_content, dlcIdFrom, triggers_custom_game, coordinate_range)
			{
			}

			public override bool isModded => true;
		}
		public static void RegisterMixings(CustomGameSettings instance)
		{
			RegisterCustomMixingSetting("FrozenMixing", "subworldMixing/FrozenMixingSettings");
			RegisterCustomMixingSetting("ForestMixing", "subworldMixing/ForestMixingSettings");
			RegisterCustomMixingSetting("JungleMixing", "subworldMixing/JungleMixingSettings");
			RegisterCustomMixingSetting("MarshMixing", "subworldMixing/MarshMixingSettings");
			RegisterCustomMixingSetting("OceanMixing", "subworldMixing/OceanMixingSettings");
			RegisterCustomMixingSetting("OilMixing", "subworldMixing/OilMixingSettings");

			if (DlcManager.IsExpansion1Active())
			{
				RegisterCustomMixingSetting("RadioactiveMixing", "expansion1::subworldMixing/RadioactiveMixingSettings");
				RegisterCustomMixingSetting("MooMixing", "expansion1::subworldMixing/MooMixingSettings");
				RegisterCustomMixingSetting("SwampMixing", "expansion1::subworldMixing/SwampMixingSettings");
				RegisterCustomMixingSetting("WastelandMixing", "expansion1::subworldMixing/WastelandMixingSettings");
			}

			foreach (var mixing in CustomMixings)
				instance.AddMixingSettingsConfig(mixing);

		}
		public static List<CGM_Addon_SubworldMixingSettingConfig> CustomMixings = [];
		static void RegisterCustomMixingSetting(string id, string mixingPath)
		{
			SgtLogger.l("Adding custome biome mixing: "+id);
			var mixing = new CGM_Addon_SubworldMixingSettingConfig(id, mixingPath);
			mixing.required_content = [];
			CustomMixings.Add(mixing);
		}
	}
}

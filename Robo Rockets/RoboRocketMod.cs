using HarmonyLib;
using KMod;
using KnastoronOniMods;
using PeterHan.PLib;
using PeterHan.PLib.Core;
using PeterHan.PLib.Options;
using System;
using UtilLibs;

namespace RoboRockets
{
    class RoboRocketMod : UserMod2
    {		
		public override void OnLoad(Harmony harmony)
		{
			GameTags.MaterialBuildingElements.Add(GeneShufflerRechargeConfig.tag);
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.SpaceBrain);
            PUtil.InitLibrary(false);
			new POptions().RegisterOptions(this,typeof(Config));
			base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
        }
	}
}

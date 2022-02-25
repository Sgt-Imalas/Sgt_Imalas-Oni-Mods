using HarmonyLib;
using KMod;

namespace Robo_Rockets
{
    class RoboRocketMod : UserMod2
    {		
		public override void OnLoad(Harmony harmony)
		{
			base.OnLoad(harmony);
			GameTags.MaterialBuildingElements.Add(GeneShufflerRechargeConfig.ID);
		}
	}
}

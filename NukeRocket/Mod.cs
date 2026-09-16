using HarmonyLib;
using KMod;
using UtilLibs;

namespace ExplosiveMaterials
{
	public class Mod : UserMod2
	{
		public override void OnLoad(Harmony harmony)
		{
			GameTags.MaterialBuildingElements.Add(ModAssets.Tags.BuildableExplosive);
			//GameTags.Other.Add("x");
			base.OnLoad(harmony);
            SgtLogger.LogVersion(this, harmony);
        }
	}
}

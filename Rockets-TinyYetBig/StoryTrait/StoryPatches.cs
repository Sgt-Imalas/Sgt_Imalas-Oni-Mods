﻿using Database;

namespace Rockets_TinyYetBig.StoryTrait
{
	public class StoryPatches
	{
		public static Story CrashedUfoStory;


		//[HarmonyPatch(typeof(Db))]
		//[HarmonyPatch(nameof(Db.Initialize))]
		//public static class AddCustomStory
		//{
		//    static void Postfix()
		//    {
		//        CrashedUfoStory = new Story(CrashedUFOStoryTrait.CrashedUFOStoryTraitKey, "storytraits/RTB_CrashedUFOStoryTrait", 5, 6, 44).SetKeepsake("keepsake_test");
		//        Db.Get().Stories.AddStoryMod(CrashedUfoStory);
		//    }
		//}
	}
}

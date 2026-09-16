using KSerialization;
using PeterHan.PLib.Core;

namespace RoboRockets.LearningBrain
{
	internal class FlyingBrain : KMonoBehaviour
	{
		[Serialize]
		float learnedSpeed = 0.75f;
		[Serialize]
		bool awakened = false;
		[MyCmpGet]
		KPrefabID prefabID;
		[MyCmpGet]
		KSelectable selectable;
		[MyCmpGet]
		public UserNameable nameable;

		public float GetCurrentSpeed() => learnedSpeed;


		//private void OnRefreshUserMenu(object data)
		//{
		//	Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("ADD LEVEL", "Export Image", 
		//		() => learnedSpeed += 0.25f, tooltipText: "level up dat brain"));
		//	Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("", "RESET brain", 
		//		() => learnedSpeed = Config.Instance.AiLearnStart, tooltipText: "reset brain xp"));
		//}
		public override void OnSpawn()
		{
			base.OnSpawn();
			//Subscribe((int)GameHashes.RefreshUserMenu, OnRefreshUserMenu);

			if (!prefabID.HasTag(GameTags.PedestalDisplayable))
			{
				prefabID.AddTag(GameTags.PedestalDisplayable);
			}

			if (!awakened)
			{
				learnedSpeed = Config.Instance.AiLearnStart;
				awakened = true;
			}
			selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.ExperienceLevel, this);
		}
		public void TraveledDistance(int hexes = 1)
		{
			if (learnedSpeed < 1.0f)
			{
				learnedSpeed += hexes / 250f;
			}
			else if (learnedSpeed < 1.25f)
			{
				learnedSpeed += hexes / 350f;
			}
			else if (learnedSpeed < 1.50f)
			{
				learnedSpeed += hexes / 500f;
			}
			else if (learnedSpeed < 1.75f)
			{
				learnedSpeed += hexes / 700f;
			}
			else if (learnedSpeed < 2f)
			{
				learnedSpeed += hexes / 950f;
			}
			else if (learnedSpeed < 3f)
			{
				learnedSpeed += hexes / 2000f;
			}
		}
	}
}

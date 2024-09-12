using KSerialization;

namespace RoboRockets.LearningBrain
{
	internal class FlyingBrain : KMonoBehaviour
	{
		[Serialize]
		float learnedSpeed = 0.75f;
		[Serialize]
		bool awakened = false;
		[MyCmpGet]
		public UserNameable nameable;

		public float GetCurrentSpeed() => learnedSpeed;



		public override void OnSpawn()
		{
			base.OnSpawn();
			if (gameObject.TryGetComponent<KPrefabID>(out var prefab))
			{
				if (!prefab.HasTag(GameTags.PedestalDisplayable))
				{
					prefab.AddTag(GameTags.PedestalDisplayable);
				}
			}
			if (!awakened)
			{
				learnedSpeed = Config.Instance.AiLearnStart;
				awakened = true;
			}
			this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.ExperienceLevel, (object)this);
		}
		public void TraveledDistance(int hexes = 1)
		{
			if (learnedSpeed < 1.0f)
			{
				learnedSpeed += hexes / 250f;
			}
			else if (learnedSpeed < 1.25f)
			{
				learnedSpeed += hexes / 300f;
			}
			else if (learnedSpeed < 1.50f)
			{
				learnedSpeed += hexes / 400f;
			}
			else if (learnedSpeed < 1.75f)
			{
				learnedSpeed += hexes / 500f;
			}
			else if (learnedSpeed < 2f)
			{
				learnedSpeed += hexes / 750f;
			}
			else if (learnedSpeed < 3f)
			{
				learnedSpeed += hexes / 2000f;
			}
		}
	}
}

using KSerialization;

namespace RoboRockets
{
	public class AIPassengerModule : PassengerRocketModule
	{
		[Serialize]
		public bool variableSpeed = false;
		public override void OnSpawn()
		{
			base.OnSpawn();

			if (!variableSpeed)
			{
				this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, ModAssets.ExperienceLevel, Config.Instance.NoBrainRockets);
			}
		}

	}
}

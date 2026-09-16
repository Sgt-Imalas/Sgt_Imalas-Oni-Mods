using KSerialization;

namespace AkisSnowThings.Content.Scripts.Entities
{
	internal class EvergreenHarvestDesignatable : HarvestDesignatable
	{
		[Serialize]
		bool freshlyPlanted = true;

		public override void OnSpawn()
		{
			base.OnSpawn();
			if(freshlyPlanted)
			{
				SetHarvestWhenReady(false);
				freshlyPlanted = false;
			}
		}
	}
}

namespace SkinEffects
{
	internal class SkinLamp : KMonoBehaviour
	{
		[MyCmpGet]
		Light2D light;

		[MyCmpGet]
		BuildingFacade skin;

		public override void OnSpawn()
		{
			base.OnSpawn();
			ToggleLamp();
		}
		internal void ToggleLamp()
		{
			light.enabled = (!skin.currentFacade.IsNullOrWhiteSpace() && skin.currentFacade.Contains("shineornaments"));
			light.FullRefresh();
		}
	}
}

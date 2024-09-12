namespace KnastoronOniMods
{
	class SelfDestructInWrongEnvironmentComponent : KMonoBehaviour, ISim4000ms
	{
		int homeWorld;

		public override void OnSpawn()
		{
			homeWorld = this.GetMyWorldId();
		}

		public void SelfDestruct()
		{
			homeWorld = -1;
		}

		public void Sim4000ms(float dt)
		{

			// Debug.Log(homeWorld + "<- ->" + this.GetMyWorldId());
			if (this.gameObject == null || this.GetMyWorld() == null || this.GetMyWorldId() != homeWorld)
			{
				Destroy(this.gameObject);
			}

		}
	}
}

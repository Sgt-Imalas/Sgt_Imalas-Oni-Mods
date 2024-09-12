namespace CritterTraitsReborn.Components
{
	class Noisy : KMonoBehaviour, ISaveLoadable, ISim4000ms
	{
		public void Sim4000ms(float dt)
		{
			if (gameObject.GetComponent<Navigator>()?.IsMoving() ?? false)
			{
				AcousticDisturbance.Emit(gameObject, 1);
			}
		}
	}
}

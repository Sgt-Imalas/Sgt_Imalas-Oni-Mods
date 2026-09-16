using System;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings
{
	internal class RandomizedUserNameable : UserNameable
	{
		public override void OnSpawn()
		{
			if (string.IsNullOrEmpty(savedName))
			{
				SetName(Guid.NewGuid().ToString().Substring(0,10));
			}
			else
			{
				SetName(savedName);
			}
		}
	}
}

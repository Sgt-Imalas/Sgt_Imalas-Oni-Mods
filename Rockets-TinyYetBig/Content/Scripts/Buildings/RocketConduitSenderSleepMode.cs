using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings
{
	/// <summary>
	/// puts rocketconduitsenders into "sleep mode"
	/// aka turns off their active state after a few seconds of not sending so they only consume power when actively doing sth 
	/// (and playing their anims correctly)
	/// this has to happen with delay bc otherwise the state will just flicker if it happens directly in isim200
	/// </summary>
	internal class RocketConduitSenderSleepMode : KMonoBehaviour, ISim1000ms
	{
		[MyCmpReq]Operational operational;
		float idleTime = 0;

		public override void OnSpawn()
		{
			this.Sub(GameHashes.OnStorageChange, OnStorageChanged);
			base.OnSpawn();
		}
		public override void OnCleanUp()
		{
			this.UnsubAll();
			base.OnCleanUp();
		}
		void OnStorageChanged (object _) => idleTime = 0f;

		public void Sim1000ms(float dt)
		{
			idleTime += dt;
			if (idleTime >= 2f)
			{
				idleTime = float.NegativeInfinity;
				operational.SetActive(false);
			}
		}
	}
}

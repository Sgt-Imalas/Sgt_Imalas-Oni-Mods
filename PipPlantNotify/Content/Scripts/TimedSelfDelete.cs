using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace PipPlantNotify.Content.Scripts
{
	internal class TimedSelfDelete : KMonoBehaviour, ISim1000ms
	{
		[SerializeField]
		public int TimeUntilTillDelete = 10;
		[Serialize]
		int Timer = 0;

		public void Sim1000ms(float dt)
		{
			Timer++;
			if (Timer >= TimeUntilTillDelete)
				UnityEngine.Object.Destroy(this.gameObject);
		}
	}
}

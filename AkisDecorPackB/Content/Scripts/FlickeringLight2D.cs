using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AkisDecorPackB.Content.Scripts
{
	[SerializationConfig(MemberSerialization.OptIn)]
	public class FlickeringLight2D : Light2D, ISimEveryTick
	{
		[SerializeField] public float transitionSpeed;
		[SerializeField] public float minTime;
		[SerializeField] public float maxTime;
		[SerializeField] public Color darkColor;
		[SerializeField] public Color brightColor;

		private float nextEnergyTarget = -1.0f;
		private float nextFlickerWindow = -1.0f;
		private double elapsedSincelastFlicker;
		private float energy;

		public override void OnSpawn()
		{
			base.OnSpawn();
			energy = 1f;

			if (!Config.Instance.OilLantern_Flickers)
				SimAndRenderScheduler.instance.simEveryTick.Remove(this);

			Color = brightColor;
		}

		private void SetNextTarget()
		{
			nextEnergyTarget = UnityEngine.Random.value;
			nextFlickerWindow = UnityEngine.Random.Range(minTime, maxTime);
			elapsedSincelastFlicker = 0;
		}

		public void SimEveryTick(float dt)
		{
			elapsedSincelastFlicker += dt;

			if (elapsedSincelastFlicker > nextFlickerWindow && Mathf.Approximately(energy, nextEnergyTarget))
				SetNextTarget();

			energy = Mathf.MoveTowards(energy, nextEnergyTarget, dt * transitionSpeed);
			Color = Color.Lerp(darkColor, brightColor, energy);
		}
	}
}

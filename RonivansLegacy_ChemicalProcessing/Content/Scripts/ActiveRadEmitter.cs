using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	internal class ActiveRadEmitter : KMonoBehaviour, ISim200ms
	{
		[MyCmpReq] Operational operational;
		[MyCmpReq] RadiationEmitter radEmitter;

		[SerializeField] public float MaxRads = 8000;
		[SerializeField] public float RadChangePerSec = 600;
		[SerializeField] public float MaxRadRange = 30;

		[Serialize] public float CurrentRads = 0;
		public bool IsActive;
		

		public override void OnSpawn()
		{
			base.OnSpawn();
			UpdateRadEmission();
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
		}

		void UpdateRadEmission()
		{
			float radPercentage = CurrentRads / MaxRads;
			short range = (short)Mathf.RoundToInt(MaxRadRange * radPercentage);

			radEmitter.emitRadiusX = range;
			radEmitter.emitRadiusY = range;
			radEmitter.emitRads = CurrentRads;
			radEmitter.Refresh();
		}


		public void Sim200ms(float dt)
		{
			IsActive = operational.IsActive;

			if (IsActive)
			{				
				CurrentRads += RadChangePerSec * dt;
				if(CurrentRads > MaxRads)
					CurrentRads = MaxRads;
			}
			else
			{
				CurrentRads -= RadChangePerSec * dt;
				if (CurrentRads < 0)
					CurrentRads = 0;
			}
			UpdateRadEmission();
		}
	}
}

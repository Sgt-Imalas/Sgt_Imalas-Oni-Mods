using STRINGS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;
using UtilLibs;

namespace Rockets_TinyYetBig.Content.Scripts.Buildings.Research
{
	class DeepSpaceResearchCenter : ResearchCenter, ISim1000ms
	{
		[MyCmpReq] PrimaryElement primaryElement;
		public void Sim1000ms(float dt)
		{
			RecalculateEfficiency();
		}
		public void RecalculateEfficiency()
		{
			float researchPerInsight = 1f;

			float minEfficiencyTemp = UtilMethods.GetKelvinFromC(-100); //base efficiency of 1 at -100C or higher
			float maxEfficiencyTemp = 3; //max efficiency at 3K

			float calcTemperature = Mathf.Clamp(primaryElement.Temperature,maxEfficiencyTemp,minEfficiencyTemp);
			float efficiencyMultiplier = Mathf.InverseLerp(minEfficiencyTemp,maxEfficiencyTemp,calcTemperature);
			SgtLogger.l("Efficiency: " + efficiencyMultiplier);
			researchPerInsight /= efficiencyMultiplier;

			mass_per_point = researchPerInsight;
		}
	}
}

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class ConfigurableSolidConduitDispenser : SolidConduitDispenser
	{
		[SerializeField]
		public float massDispensed;
	}
}

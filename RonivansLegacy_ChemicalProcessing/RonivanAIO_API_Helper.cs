using HarmonyLib;
using RonivansLegacy_ChemicalProcessing.Content.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing
{
	public static class RonivanAIO_API_Helper
	{
		/// <summary>
		/// Copy this to your project
		/// </summary>
		///-----------------------------------------
		//public static class RonivanAIO_Integration
		//{
		//	//delegate definitions
		//	public delegate void PipedConduitDispenser_AddElementFilterDelegate(GameObject go, SimHashes element, int dispenserIndex = 0);
		//	public delegate void PipedConduitDispenser_AddTagFilterDelegate(GameObject go, Tag tag, int dispenserIndex = 0);

		//	//delegates to call
		//	public static PipedConduitDispenser_AddElementFilterDelegate PipedConduitDispenser_AddElementFilter;

		//	public static void Initialize(Harmony harmony)
		//	{
		//		//todo
		//	}
		//}




		///-----------------------------------------

		public static void PipedConduitDispenser_AddElementFilter(GameObject go, SimHashes element, int dispenserIndex = 0)
		{
			var dispensers = go.GetComponents<PipedConduitDispenser>();
			if(!dispensers.Any())
			{
				SgtLogger.warning("No PipedConduitDispenser components on " + go);
				return;
			}
			if(dispenserIndex >= dispensers.Count())
			{
				SgtLogger.warning("PipedConduitDispenser index was outside of bounds on" + go);
				return;
			}
			var dispenser = dispensers[dispenserIndex];
			dispenser.elementFilter = dispenser.elementFilter.Append(element);
		}
		public static void PipedConduitDispenser_AddTagFilter(GameObject go, Tag tag, int dispenserIndex = 0)
		{
			var dispensers = go.GetComponents<PipedConduitDispenser>();
			if (!dispensers.Any())
			{
				SgtLogger.warning("No PipedConduitDispenser components on " + go);
				return;
			}
			if (dispenserIndex >= dispensers.Count())
			{
				SgtLogger.warning("PipedConduitDispenser index was outside of bounds on" + go);
				return;
			}
			var dispenser = dispensers[dispenserIndex];
			dispenser.tagFilter = dispenser.tagFilter.Append(tag);
		}
		public static object AddComponent_PipedOptionalExhaust(GameObject go,
			Tag outputTag,
			float emissionRatePerSecond = 25f,
			float internalMaxCapacity = float.MaxValue,
			float atmosphericOverpressureThreshold = float.MaxValue, 
			int dispenserIndex = 0)
		{
			var dispensers = go.GetComponents<PipedConduitDispenser>();
			if (!dispensers.Any())
			{
				SgtLogger.warning("No PipedConduitDispenser components on " + go+ " for PipedOptionalExhaust addition");
				return null;
			}
			if (dispenserIndex >= dispensers.Count())
			{
				SgtLogger.warning("PipedConduitDispenser index was outside of bounds on" + go + " for PipedOptionalExhaust addition");
				return null;
			}
			var dispenser = dispensers[dispenserIndex];

			PipedOptionalExhaust optionalExhaust = go.AddComponent<PipedOptionalExhaust>();
			optionalExhaust.dispenser = dispenser;
			optionalExhaust.elementTag = outputTag;
			optionalExhaust.capacity = internalMaxCapacity;
			optionalExhaust.emissionRate = emissionRatePerSecond;
			optionalExhaust.OverpressureThreshold = atmosphericOverpressureThreshold;
			return optionalExhaust;
		}
	}
}

﻿using HarmonyLib;
using System;
using System.Collections.Generic;
using UtilLibs;

namespace SetStartDupes.API_IO
{
	internal class RainbowFarts_API
	{
		static List<string> FartIDs = new();
		internal static List<string> GetFartIDs()
		{
			return FartIDs;
		}
		private static bool TryInitialize(bool logWarnings = true)
		{
			var type = Type.GetType("RainbowFarts.RainbowFartsTuning, RainbowFarts");

			if (type == null)
			{
				if (logWarnings)
					Debug.LogWarning("RainbowFarts does not exist.");

				return false;
			}

			var m_Flatulence_Types = AccessTools.Field(type, "FLATULENCE_TYPES");

			if (m_Flatulence_Types == null)
			{
				if (logWarnings) Debug.LogWarning("FLATULENCE_TYPES is not a an existing field.");
				return false;
			}
			var flatulenceTypesTObj = m_Flatulence_Types.GetValue(null);

			var flatulenceTypes = m_Flatulence_Types.GetValue(null) as Array;
			if (flatulenceTypes == null)
			{
				if (logWarnings) Debug.LogWarning("could not get value for FLATULENCE_TYPES");
				return false;
			}
			foreach (var flatulence in flatulenceTypes)
			{
				var id = Traverse.Create(flatulence).Field("id").GetValue() as string;
				if (id == null)
				{
					if (logWarnings) Debug.LogWarning("could not get id");
					continue;
				}
				//SgtLogger.l(id, "FartTypeID");
				if (!FartIDs.Contains(id))
					FartIDs.Add(id);
			}

			return true;
		}
		public static void InitRainbowFartsAPI()
		{
			if (RainbowFarts_API.TryInitialize())
			{
				ModAssets.InitRainbowFarts();
			}
			else
				SgtLogger.l("Rainbow Farts mod not found, API is resting now, gn...zzz");
		}
	}
}

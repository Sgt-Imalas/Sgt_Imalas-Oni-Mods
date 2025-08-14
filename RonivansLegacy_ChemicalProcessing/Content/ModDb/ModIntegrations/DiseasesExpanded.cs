using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.ModDb.ModIntegrations
{
	internal class DiseasesExpanded
	{
		/// <summary>
		/// Integration with Diseases Expanded Alien Goo on comets
		/// </summary>
		public static EnhanceCometWithGermsDelegate EnhanceCometWithGerms_Delegate = null;
		public delegate void EnhanceCometWithGermsDelegate(GameObject go, byte idx, int impactCount);

		public static void EnhanceCometWithGerms(GameObject go, byte idx = byte.MaxValue, int impactCount = 1000000)
		{
			InitTypes();
			if (EnhanceCometWithGerms_Delegate != null)
				EnhanceCometWithGerms_Delegate(go, idx, impactCount);
		}
		static bool typesInitialized = false;
		static void InitTypes()
		{
			if (typesInitialized) return;
			typesInitialized = true;
			var DiseasesExpanded_Patches_SpaceGoo = Type.GetType("DiseasesExpanded.DiseasesExpanded_Patches_SpaceGoo, DiseasesExpandedMerged");
			if (DiseasesExpanded_Patches_SpaceGoo == null)
			{
				SgtLogger.l("DiseasesExpanded types not found.");
				//UtilMethods.ListAllTypesWithAssemblies();
				return;
			}
			var m_EnhanceCometWithGerms = AccessTools.Method(DiseasesExpanded_Patches_SpaceGoo, "EnhanceCometWithGerms", [typeof(GameObject),typeof(byte),typeof(int)]);
			if (m_EnhanceCometWithGerms == null)
			{
				SgtLogger.error("HysteresisStoragEnhanceCometWithGermsePatches.EnhanceCometWithGerms method not found.");
				return;
			}
			try
			{
				EnhanceCometWithGerms_Delegate = (EnhanceCometWithGermsDelegate)Delegate.CreateDelegate(typeof(EnhanceCometWithGermsDelegate), m_EnhanceCometWithGerms);
			}
			catch (Exception e)
			{
				SgtLogger.error("Failure to create EnhanceCometWithGermsDelegate for DiseasesExpanded component:\n" + e.Message);
			}
			SgtLogger.l("DiseasesExpanded integration: " + (EnhanceCometWithGerms_Delegate != null ? "Success" : "Failed"));
		}

	}
}

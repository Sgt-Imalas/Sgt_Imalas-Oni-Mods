using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace ClusterTraitGenerationManager.ModIntegrations
{
	internal class MoonletAPI
	{
		public static bool MoonletInitialized;

		public delegate Dictionary<string, Dictionary<string, object>> Moonlet_GetTemplates(string contentType);
		public static Moonlet_GetTemplates GetTemplates;

		public static bool InitializeIntegration()
		{
			SgtLogger.l("Initializing Moonlet.ModAPI integration");
			var type = Type.GetType("Moonlet.ModAPI, Moonlet", false, false);
			if (type == null)
			{
				SgtLogger.l("Moonlet.ModAPI not found, Moonlet Integration going to sleep. zzzzz");
				return false;
			}

			var m_getTemplates = AccessTools.Method(type, "GetTemplates",
				new[]
				{
					typeof(string)
				});

			if (m_getTemplates == null)
			{
				Debug.LogWarning("m_getTemplates is not a method on Moonlet.ModAPI.");
				return false;
			}

			GetTemplates = (Moonlet_GetTemplates)Delegate.CreateDelegate(typeof(Moonlet_GetTemplates), m_getTemplates);

			SgtLogger.l("Moonlet.ModAPI initialized");
			return true;
		}

		private static Dictionary<string, KMod.Mod> cachedMoonletWorldMods = null;
		public static Dictionary<string, KMod.Mod> GetMoonletWorldsMods()
		{
			if (!MoonletInitialized)
				return new();

			if (cachedMoonletWorldMods == null)
			{
				KMod.Manager modManager = Global.Instance.modManager;

				var moonletData = GetTemplates("worldgen/worlds");
				if (moonletData == null)
				{
					SgtLogger.error($"MoonletAPI dictionary for moonletData was null!!");
					return new();
				}

				cachedMoonletWorldMods = new();
				foreach (var worldKV in moonletData)
				{
					if (worldKV.Value == null)
					{
						SgtLogger.error($"MoonletAPI dictionary for {worldKV.Key} was null!!");
						continue;
					}


					string sourceModID = worldKV.Value["sourceMod"].ToString();
					var mod = modManager.mods.FirstOrDefault(mod => mod.staticID == sourceModID);
					if (mod == null)
					{
						SgtLogger.warning("could not find moonlet mod with the staticID " + sourceModID + ", defaulting to displaying CGM");
						mod = Mod.Instance;
					}
					cachedMoonletWorldMods[worldKV.Key] = mod;
					SgtLogger.l($"Moonlet World {worldKV.Key} was loaded from mod {sourceModID}");
				}
			}
			return cachedMoonletWorldMods;
		}
		public static bool IsMoonletMod(string worldPath, out KMod.Mod mod)
		{
			mod = null;
			if (!MoonletInitialized)
				return false;
			return GetMoonletWorldsMods().TryGetValue(worldPath, out mod);
		}
	}
}


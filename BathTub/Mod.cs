using BathTub.MP;
using HarmonyLib;
using KMod;
using ONI_MP_API;
using ONI_MP_API.Networking;
using System.Collections.Generic;
using System.Reflection;
using UtilLibs;

namespace BathTub
{
	public class Mod : UserMod2
	{
		public static Harmony haromy;
		public override void OnLoad(Harmony harmony)
		{
			haromy = harmony;
			base.OnLoad(harmony);
			SgtLogger.LogVersion(this, harmony);
		}
		public override void OnAllModsLoaded(Harmony harmony, IReadOnlyList<KMod.Mod> mods)
		{
			base.OnAllModsLoaded(harmony, mods);
			ModAssets.RoomsExpandedActive = ModIntegration.Rooms_Expanded.InitializeIntegration();

			SgtLogger.l("Registering Quack!");
			PacketRegistryAPI.AutoRegisterAll();
			PacketRegistryAPI.TryRegister(typeof(MemePacket));

			SgtLogger.l("session test:");
			SgtLogger.l( SessionInfoAPI.LocalSteamID.ToString() );
		}
	}
}

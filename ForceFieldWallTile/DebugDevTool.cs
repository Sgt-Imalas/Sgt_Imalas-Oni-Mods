//using FMOD.Studio;
//using FMODUnity;
//using ForceFieldWallTile.Content.Scripts;
////using ImGuiNET;
//using Klei;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UtilLibs;
//using static STRINGS.MISC;

//namespace ForceFieldWallTile
//{

//	public class DebugDevTool : DevTool
//	{
//		public static void Initialize()
//		{
//			DevToolManager.Instance.RegisterDevTool<DebugDevTool>("Mods/ForceField");
//		}
//		private static Vector4 zoneTypeColor;
//		private static Vector4 previousZoneTypeColor;
//		private static int selectedZoneType;
//		private static string audioFile = "";
//		private static Material mat;
//		public static bool renderLiquidTexture;
//		private static float uvScale = 10f;

//		private string[] zoneTypes;
//		private EventInstance instance;

//		public DebugDevTool()
//		{
//			RequiresGameRunning = true;
//			clip = Lighting.Instance.Settings.BackgroundClip;
//			bgscale = Lighting.Instance.Settings.BackgroundUVScale;
//		}

//		private static string rewardTestResult;
//		private static bool accelerateLifeCycles;


//		// TODO this is copy paste of FindAvailablePOISpawnLocations
//		private static List<AxialI> FindAvailablePOISpawnLocations(AxialI location)
//		{
//			var available = new List<AxialI>();
//			var flag = IsSuitablePOISpawnLocation(location);
//			if (flag)
//			{
//				available.Add(location);
//			}
//			for (var dist = 1; dist <= 2; dist++)
//			{
//				foreach (var direction in AxialI.DIRECTIONS)
//				{
//					var destination = location + direction * dist;
//					var flag2 = IsSuitablePOISpawnLocation(destination);
//					if (flag2)
//					{
//						available.Add(destination);
//					}
//				}
//			}
//			return available;
//		}
//		private static bool IsSuitablePOISpawnLocation(AxialI location)
//		{
//			var flag = !ClusterGrid.Instance.IsValidCell(location);
//			bool result;
//			if (flag)
//			{
//				result = false;
//			}
//			else
//			{
//				var entities = ClusterGrid.Instance.GetEntitiesOnCell(location);
//				foreach (var entity in entities)
//				{
//					var flag2 = entity.Layer == EntityLayer.Asteroid || entity.Layer == EntityLayer.POI;
//					if (flag2)
//					{
//						return false;
//					}
//				}
//				result = true;
//			}
//			return result;
//		}

//		public override void RenderTo(DevPanel panel)
//		{
//			//if (ImGui.Button("Spawn Oxygen"))
//			//	ElementLoader.FindElementByHash(SimHashes.Oxygen).substance.SpawnResource(GameUtil.GetActiveTelepad().transform.position, 1f, 315, 255, 0);



//			//if (ImGui.CollapsingHeader("FMOD Tests"))
//			//{
//			//	if (ImGui.Button("Control test"))
//			//		KFMOD.PlayUISound("event:/UI/Mouse/HUD_Click");

//			//	if (ImGui.Button("Jelly Bounce"))
//			//		KFMOD.PlayUISound("event:/jellybounce1");

//			//	if (ImGui.Button("Play Stinger"))
//			//		MusicManager.instance.PlaySong(Assets.GetSimpleSoundEventName("event:/beached/Music/ocean_palace"));
//			//}

//			//HandleSelectedObject(); 
//		}

//		private static void HandleSelectedObject()
//		{
//			var selectedObject = SelectTool.Instance?.selected;

//			if (selectedObject == null)
//				return;

//			if (selectedObject.TryGetComponent(out Navigator navigator))
//				ImGui.Text($"Navigator current type: {navigator.CurrentNavType}");
//			var debugs = selectedObject.GetComponents<IImguiDebug>();
//			if (debugs != null)
//			{
//				ImGui.Separator();
//				ImGui.Text($"Selected object: {selectedObject.GetProperName()}");
//				ImGui.Separator();

//				if (selectedObject.TryGetComponent(out BuildingHP buildingHP))
//				{
//					if (ImGui.Button("Break Building"))
//						buildingHP.DoDamage(100);
//				}

//				foreach (var imguiDebug in debugs)
//				{
//					ImGui.Separator();
//					ImGui.Text($"{imguiDebug.GetType()}");
//					imguiDebug.OnImguiDraw();
//				}
//			}

//			var joyBehavior = selectedObject.GetSMI<JoyBehaviourMonitor.Instance>();
//			if (joyBehavior != null && ImGui.Button("Overjoy"))
//				joyBehavior.GoToOverjoyed();

//			var stress = Db.Get().Amounts.Stress.Lookup(selectedObject);
//			if (stress != null)
//			{
//				if (joyBehavior != null)
//					ImGui.SameLine();

//				if (ImGui.Button("Stress"))
//					stress.SetValue(100f);
//			}
//		}

//		private float siltStoneUV = 1f;
//		private static float clip;
//		private float bgscale = 1f;


//	}
//}

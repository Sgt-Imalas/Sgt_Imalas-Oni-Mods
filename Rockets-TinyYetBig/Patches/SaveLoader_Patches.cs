using HarmonyLib;
using Klei;
using rail;
using Rockets_TinyYetBig.SpaceStations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.Patches
{
	class SaveLoader_Patches
	{
		public static readonly string RE_GridExpansionKey = "RTB_IncreaseGridHeight";
		public static bool IncreaseGridHeight = false;

		[HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.Load), [typeof(IReader)])]
		public class SaveLoader_Load_Patch
		{
			public static readonly int GridHeightIncrease = 105; //allow max height of a space station;

			static SaveFileRoot _saveFileRoot;
			static bool dataRead = false;

			public static void Prefix()
			{
				SgtLogger.l("SaveLoader_Load_Patch.Prefix called");
				dataRead = false;
			}

			public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
			{
				var codes = orig.ToList();
				int injectionCount = 0;

				SgtLogger.l("SaveLoader_Load_Patch.Transpiler called, injecting code to conditionally increase grid size");
				// find injection points
				for (var i = codes.Count - 1; i >= 0; i--)
				{
					var instruction = codes[i];
					if (instruction.LoadsField(m_SaveFileRoot_HeightInCells)) //called in two locations
					{
						injectionCount++;
						SgtLogger.l("Increasing grid height by " + GridHeightIncrease.ToString());
						codes.Insert(i + 1, new(OpCodes.Call, m_IncreaseGridSize));
					}
					else if (instruction.StoresField(m_Grid_Visible)) //called once
					{
						injectionCount++;
						SgtLogger.l("Increasing grid size on Visible array");
						codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, m_ResizeVisible));
					}
					else if (instruction.StoresField(m_Grid_Spawnable))//called once
					{
						injectionCount++;
						SgtLogger.l("Increasing grid size on Spawnable array");
						codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, m_ResizeSpawnable));
					}
					else if (instruction.StoresField(m_Grid_Damage))//called once
					{
						injectionCount++;
						SgtLogger.l("Increasing grid size on Damage array");
						codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, m_ResizeDamage));
					}
					else if (instruction.CallsConstructor(m_SaveFileRoot_Constructor))//called once
					{
						injectionCount++;
						SgtLogger.l("Fetching SaveFileRoot");
						codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, m_FetchSaveFileRootReference));
					}

				}

				if(injectionCount != 6)
				{
					SgtLogger.error("SAVELOADER TRANSPILER FAILED: Expected 6 code injections, but found " + injectionCount);
				}
				else
					SgtLogger.l("SaveLoader Load Patch Transpiler Successfull");
				return codes;
			}


			static FieldInfo m_SaveFileRoot_HeightInCells = AccessTools.Field(typeof(SaveFileRoot), nameof(SaveFileRoot.HeightInCells));
			static FieldInfo m_Grid_Visible = AccessTools.Field(typeof(Grid), nameof(Grid.Visible));
			static FieldInfo m_Grid_Spawnable = AccessTools.Field(typeof(Grid), nameof(Grid.Spawnable));
			static FieldInfo m_Grid_Damage = AccessTools.Field(typeof(Grid), nameof(Grid.Damage));

			static MethodInfo m_IncreaseGridSize = AccessTools.Method(typeof(SaveLoader_Load_Patch), nameof(IncreaseGridSize));
			static MethodInfo m_FetchSaveFileRootReference = AccessTools.Method(typeof(SaveLoader_Load_Patch), nameof(FetchSaveFileRootReference));
			static MethodInfo m_ResizeVisible = AccessTools.Method(typeof(SaveLoader_Load_Patch), nameof(ResizeVisible));
			static MethodInfo m_ResizeSpawnable = AccessTools.Method(typeof(SaveLoader_Load_Patch), nameof(ResizeSpawnable));
			static MethodInfo m_ResizeDamage = AccessTools.Method(typeof(SaveLoader_Load_Patch), nameof(ResizeDamage));
			static ConstructorInfo m_SaveFileRoot_Constructor = AccessTools.Constructor(typeof(SaveFileRoot));



			private static int IncreaseGridSize(int originalGridSize)
			{
				///moved here because the reference grabbing method doesnt have the values deserialized yet
				CheckResizeState();

				if (IncreaseGridHeight && _saveFileRoot != null)
				{
					return originalGridSize + GridHeightIncrease;
				}
				return originalGridSize;
			}

			private static void ResizeVisible()
			{
				if (IncreaseGridHeight && _saveFileRoot != null)
					Array.Resize(ref Grid.Visible, Grid.Visible.Length + (_saveFileRoot.WidthInCells * GridHeightIncrease));
			}
			private static void ResizeSpawnable()
			{
				if (IncreaseGridHeight && _saveFileRoot != null)
					Array.Resize(ref Grid.Spawnable, Grid.Spawnable.Length + (_saveFileRoot.WidthInCells * GridHeightIncrease));
			}
			private static void ResizeDamage()
			{
				if (IncreaseGridHeight && _saveFileRoot != null)
					Array.Resize(ref Grid.Damage, Grid.Damage.Length + (_saveFileRoot.WidthInCells * GridHeightIncrease));
			}
			private static SaveFileRoot FetchSaveFileRootReference(SaveFileRoot original)
			{
				_saveFileRoot = original;
				return original;
			}
			private static void CheckResizeState()
			{
				if(dataRead) //only read once per method execution
					return;
				dataRead = true;

				if (_saveFileRoot.streamed.TryGetValue(RE_GridExpansionKey, out var value))
				{
					SgtLogger.l("reading grid expansion data");
					// Convert byte array to a single boolean value
					IncreaseGridHeight = value != null && value.Length > 0 && BitConverter.ToBoolean(value, 0);
				}
				else
				{
					SgtLogger.l("no grid expansion data found yet, defaulting to no");
					IncreaseGridHeight = false;
				}
				SgtLogger.l(IncreaseGridHeight ? "Existing grid space was insufficient for further space stations, increasing Grid Height" : "Grid size was sufficient, leaving grid as is");
			}
		}


		[HarmonyPatch(typeof(SaveLoader), nameof(SaveLoader.PrepSaveFile))]
		public class SaveLoader_PrepSaveFile_Patch
		{
			public static void Postfix(SaveLoader __instance, ref SaveFileRoot __result)
			{
				SgtLogger.l("Checking if the next loading should increase grid height...");

				bool shouldExpand = !HasEnoughGridSpace(SpaceStation.SpaceStationDefaultSize);
				SgtLogger.l(shouldExpand ? "Grid requires size increase for next space station, expanding next saveload" : "Grid size is sufficient for next space station, not expanding");


				byte[] bytes = BitConverter.GetBytes(shouldExpand);
				__result.streamed[RE_GridExpansionKey] = bytes;
			}

			/// <summary>
			/// Mirroring Grid.GetFreeGridSpace without initializing anything in the sim
			/// </summary>
			/// <param name="size"></param>
			/// <param name="offset"></param>
			/// <returns></returns>
			public static bool HasEnoughGridSpace(Vector2I size)
			{
				Vector2I gridOffset = BestFit.GetGridOffset(ClusterManager.Instance.WorldContainers, size, out _);
				if (gridOffset.X <= Grid.WidthInCells && gridOffset.Y <= Grid.HeightInCells)
				{
					return true;
				}
				return false;
			}
		}
	}
}

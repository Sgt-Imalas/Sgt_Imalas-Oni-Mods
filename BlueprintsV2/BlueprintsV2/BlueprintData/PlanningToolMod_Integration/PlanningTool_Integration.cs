using BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration.EnumMirrors;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace BlueprintsV2.BlueprintsV2.BlueprintData.PlanningToolMod_Integration
{
	internal class PlanningTool_Integration
	{
		public static bool ModActive = false;

		public static Type t_PlanColorExtension, t_PlanningToolInterface, t_SaveLoadPlans, t_SaveLoadPlans_PlanData;
		public static Type t_PlanColor, t_PlanShape;

		static MethodInfo m_PlacePlan;
		static PropertyInfo p_SaveLoadPlans_Instance;
		static IDictionary PlanState = null;


		public static void PlacePlan(int cell, PlanShape shape, PlanColor color)
		{
			if (!ModActive)
				return;
			var planData = Activator.CreateInstance(t_SaveLoadPlans_PlanData);

			var traverse = Traverse.Create(planData);
			traverse.Field("Cell").SetValue(cell);
			traverse.Field("Shape").SetValue((int)shape);
		//	traverse.Field("Shape").SetValue(Enum.ToObject(t_PlanShape, (int)shape));
			traverse.Field("Color").SetValue((int)color);
			//traverse.Field("Color").SetValue(Enum.ToObject(t_PlanColor, (int)color));
			SgtLogger.l("Shape: " + shape + ", Color: " + color);
			UtilMethods.ListAllFieldValues(planData);
			m_PlacePlan.Invoke(null, [cell, planData]);
		}
		public static bool HasPlan(int cell, out PlanShape shape, out PlanColor color)
		{
			shape = default;
			color = default;
			if (!ModActive)
				return false;
			if (PlanState == null)
			{
				PlanState = (IDictionary)Traverse.Create(p_SaveLoadPlans_Instance.GetValue(null)).Field("PlanState").GetValue();
			}
			if (PlanState == null)
			{
				SgtLogger.warning("PlanState was not found on p_SaveLoadPlans_Instance!");
				return false;
			}

			if (!PlanState.Contains(cell))
				return false;
			var planDataObj = PlanState[cell];

			var traverse = Traverse.Create(planDataObj);
			shape = (PlanShape)traverse.Field("Shape").GetValue<int>();
			color = (PlanColor)traverse.Field("Color").GetValue<int>();
			return true;
		}

		internal static void Initialize()
		{
			ModActive = InitializeTypes();
			if (ModActive)
				VerifyTypes();
			ModActive = InitializeMethods();
			SgtLogger.l("PlanningTool ModIntegration: " + (ModActive ? "Active" : "Sleeping"));
		}

		private static void VerifyTypes()
		{
			if (t_PlanColorExtension == null)
			{
				SgtLogger.warning("PlanningTool.PlanColorExtension type not found!", "BlueprintsV2/PlanningTool_Integration");
				return;
			}
			if (t_PlanningToolInterface == null)
			{
				SgtLogger.warning("PlanningTool.PlanColorExtension type not found!", "BlueprintsV2/PlanningTool_Integration");
				return;
			}
			if (t_SaveLoadPlans == null)
			{
				SgtLogger.warning("PlanningTool.SaveLoadPlans type not found!", "BlueprintsV2/PlanningTool_Integration");
				return;
			}
			if (t_SaveLoadPlans_PlanData == null)
			{
				SgtLogger.warning("PlanningTool.SaveLoadPlans.PlanData type not found!", "BlueprintsV2/PlanningTool_Integration");
				return;
			}
			if (t_PlanColor == null)
			{
				SgtLogger.warning("PlanningTool.PlanColor enum type not found!", "BlueprintsV2/PlanningTool_Integration");
				return;
			}
			if (t_PlanShape == null)
			{
				SgtLogger.warning("PlanningTool.PlanShape enum type not found!", "BlueprintsV2/PlanningTool_Integration");
				return;
			}
		}

		private static bool InitializeTypes()
		{
			t_PlanColorExtension = Type.GetType("PlanningTool.PlanColorExtension, PlanningTool", false, false);
			if (t_PlanColorExtension == null)
				return false;
			t_PlanningToolInterface = Type.GetType("PlanningTool.PlanningToolInterface, PlanningTool", false, false);
			if (t_PlanningToolInterface == null)
				return false;
			t_SaveLoadPlans = Type.GetType("PlanningTool.SaveLoadPlans, PlanningTool", false, false);
			if (t_SaveLoadPlans == null)
				return false;
			t_SaveLoadPlans_PlanData = Type.GetType("PlanningTool.SaveLoadPlans+PlanData, PlanningTool", false, false);
			if (t_SaveLoadPlans_PlanData == null)
				return false;

			//enums
			t_PlanColor = Type.GetType("PlanningTool.PlanColor, PlanningTool", false, false);
			if (t_PlanColor == null)
				return false;
			t_PlanShape = Type.GetType("PlanningTool.PlanShape, PlanningTool", false, false);
			if (t_PlanShape == null)
				return false;

			return InitializeMethods();
		}

		private static bool InitializeMethods()
		{
			m_PlacePlan = AccessTools.Method(t_PlanningToolInterface, "PlacePlan");
			if (m_PlacePlan == null)
			{
				SgtLogger.warning("PlacePlan is not a method on PlanningTool.PlanningToolInterface.");
				return false;
			}

			p_SaveLoadPlans_Instance = AccessTools.Property(t_SaveLoadPlans, "Instance");
			if (p_SaveLoadPlans_Instance == null)
			{
				SgtLogger.warning("Instance is not a field on PlanningTool.SaveLoadPlans");
				return false;
			}
			return true;

		}
	}
}

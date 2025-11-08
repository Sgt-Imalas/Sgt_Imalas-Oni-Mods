using Database;
using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI.Extensions;

namespace UtilLibs.SharedTweaks
{
	/// <summary>
	/// space the connection lines horizontally to avoid them overlapping
	/// </summary>
	public sealed class SkillsWidgetBetterConnectionLines : PForwardedComponent
	{
		public static void Register()
		{
			new SkillsWidgetBetterConnectionLines().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);
		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethodOnSpawn = AccessTools.Method(typeof(SkillWidget), nameof(SkillWidget.RefreshLines));

				var onSpawnPostfix = AccessTools.Method(typeof(SkillsWidgetBetterConnectionLines), nameof(RefreshLinesPostfix));

				///change the line renderers to be more spaced out
				plibInstance.Patch(targetMethodOnSpawn, postfix: new(onSpawnPostfix, Priority.HigherThanNormal));
				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}

		/// <summary>
		/// vertical distance between 2 directly vertically adjacent tech nodes
		/// </summary>
		const float Y_Step = 92.3f;


		public static void RefreshLinesPostfix(SkillWidget __instance)
		{
			foreach (var lineRenderer in __instance.lines)
			{
				UnityEngine.Object.Destroy(lineRenderer.gameObject);
			}
			//foreach (var linepoint in __instance.linePoints)
			//	SgtLogger.l(__instance.skillID + " linepoint: " + linepoint.ToString());
			__instance.linePoints.Clear();

			List<Tuple<string, Vector2>> sourceBelow = [], sourceAbove = [], sourceEven = [];

			var currentSkill = Db.Get().Skills.Get(__instance.skillID);

			foreach (var priorSkill in currentSkill.priorSkills)
			{
				var verticalPosPrior = __instance.skillsScreen.GetSkillWidgetLineTargetPosition(priorSkill);
				var verticalPosOwn = __instance.lines_left.GetPosition();

				float verticalOffset = (verticalPosPrior.y - verticalPosOwn.y);
				var priorTarget = new Tuple<string, Vector2>(priorSkill, verticalPosPrior);

				//SgtLogger.l(" ->> verticalOffset: " + verticalOffset + " for " + __instance.skillID + " to " + priorSkill);
				if (verticalOffset < -1)
					sourceBelow.Add(priorTarget);
				else if (verticalOffset > 1)
					sourceAbove.Add(priorTarget);
				else
					sourceEven.Add(priorTarget);
			}
			sourceAbove.Sort((a, b) => a.second.y.CompareTo(b.second.y));
			sourceBelow.Sort((a, b) => -a.second.y.CompareTo(b.second.y));
			__instance.lines = [];
			foreach (var evenSkill in sourceEven)
			{
				CreateSkillConnection(__instance, currentSkill, evenSkill);
			}
			float connectionOffset = sourceEven.Any() ? 1 : sourceBelow.Any() && sourceAbove.Any() ? 0.625f : 0;
			for (int i = 0; i < sourceBelow.Count; i++)
			{
				CreateSkillConnection(__instance, currentSkill, sourceBelow[i], -connectionOffset - i);
			}
			for (int i = 0; i < sourceAbove.Count; i++)
			{
				CreateSkillConnection(__instance, currentSkill, sourceAbove[i], connectionOffset + i);
			}

		}

		static void CreateSkillConnection(SkillWidget __instance, Skill currentSkill, Tuple<string, Vector2> requisite, float connectionPointNr = 0)
		{
			var requisiteSkill = __instance.skillsScreen.skillWidgets[requisite.first];
			var requisiteConnectionPoint = requisite.second;
			var originPos = __instance.lines_left.GetPosition();

			//SgtLogger.l("requisiteRightBorderX: " + originPos.ToString() + " - " + requisiteConnectionPoint.ToString());
			float requisiteRightBorderX = originPos.x - requisiteConnectionPoint.x;

			Vector2 relativeStartPoint = Vector2.zero;
			Vector2 relativeEndPoint = Vector2.zero;

			float verticalOffset = (requisiteConnectionPoint.y - originPos.y);
			float totalConnections = Mathf.Max(0, currentSkill.priorSkills.Count);

			float YClamp = Mathf.Min(totalConnections, 3);

			float verticalStepDiff = Mathf.Clamp(verticalOffset / Y_Step, -YClamp, YClamp);
			verticalStepDiff = (float)Math.Round(verticalStepDiff * 4, MidpointRounding.ToEven) / 4f; //round to 0.25 steps, so we can use the same offset for all techs

			float stepOffset = 11;

			float maxHeightOffsetTarget = Y_Step / 3f;
			float relativeYDiffTarget = Mathf.Clamp(verticalStepDiff * stepOffset, -maxHeightOffsetTarget, maxHeightOffsetTarget);
			float relativeYDiffTargetConnectionPoint = Mathf.Clamp(connectionPointNr * stepOffset, -maxHeightOffsetTarget, maxHeightOffsetTarget);

			float maxHeightOffsetSource = Y_Step / 3f;
			float relativeYDiffSource = Mathf.Clamp(-verticalStepDiff * stepOffset, -maxHeightOffsetSource, maxHeightOffsetSource);

			relativeEndPoint = new(0, relativeYDiffTargetConnectionPoint);
			relativeStartPoint = new(0, relativeYDiffSource);

			float midPointConnectionCalc = Mathf.CeilToInt(Mathf.Max(0, totalConnections - 3) / 2f);

			float midpoint = stepOffset - (midPointConnectionCalc * stepOffset);
			if (midpoint < 0)
				midpoint = 0;

			//SgtLogger.l(" - verticalStepDiff: " + verticalStepDiff + " for " + currentTech.Id + " to " + requisite.Id + "; Midpoint: "+midpoint+"Total Cons: "+totalConnections+", relativeYDiffTarget " + relativeYDiffTarget + " , halfTechHeightTarget: " + maxHeightOffsetTarget + ", relativeYDiffSource: " + relativeYDiffSource);

			float horizontalOffsetTarget = midpoint + Mathf.Abs(relativeYDiffTarget);
			float horizontalOffsetSource = midpoint + Mathf.Abs(relativeYDiffSource);

			UILineRenderer component = CreateLineRenderer(__instance.lines_left.transform);
			component.Points = [
				new Vector2(0, 0) + relativeEndPoint,
						new Vector2(-horizontalOffsetTarget, 0) + relativeEndPoint,
						new Vector2(-horizontalOffsetSource, verticalOffset) + relativeStartPoint,
						new Vector2(-requisiteRightBorderX, verticalOffset) + relativeStartPoint,
						];

			if (totalConnections > 3)
				component.color = Color.Lerp(component.color, Color.red, 0.5f);

			//foreach (var point in component.Points)
			//{
			//	SgtLogger.l(currentSkill.Id + "->" + requisite + " - Point: " + point);
			//}
			__instance.lines = __instance.lines.Append(component);

		}

		static UILineRenderer CreateLineRenderer(Transform parent)
		{
			GameObject go = new GameObject("Line");
			go.AddComponent<RectTransform>();
			go.transform.SetParent(parent);
			go.transform.SetLocalPosition(Vector3.zero);
			go.rectTransform().sizeDelta = Vector2.zero;
			var lr = go.AddComponent<UILineRenderer>();
			lr.color = new Color(0.6509804f, 0.6509804f, 0.6509804f, 1f);
			return lr;
		}
	}
}

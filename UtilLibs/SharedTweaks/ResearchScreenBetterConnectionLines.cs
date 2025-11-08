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
	public sealed class ResearchScreenBetterConnectionLines : PForwardedComponent
	{
		public static void Register()
		{
			new ResearchScreenBetterConnectionLines().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethodOnSpawn = AccessTools.Method(typeof(ResearchEntry), nameof(ResearchEntry.OnSpawn));
				var onSpawnPostfix = AccessTools.Method(typeof(ResearchScreenBetterConnectionLines), nameof(OnSpawnPostfix));
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
		const float Y_Step = 250;

		static Dictionary<Tech, int> techConnectionPoints = null;

		public static void OnSpawnPostfix(ResearchEntry __instance)
		{
			if (techConnectionPoints == null)
			{
				techConnectionPoints = new Dictionary<Tech, int>();
				foreach (var tech in Db.Get().Techs.resources)
				{
					foreach (var requiredTech in tech.requiredTech)
					{
						if (!techConnectionPoints.ContainsKey(requiredTech))
						{
							techConnectionPoints.Add(requiredTech, 0);
						}
						techConnectionPoints[requiredTech]++;
					}
				}
			}


			foreach (var lineRenderer in __instance.techLineMap)
			{
				UnityEngine.Object.Destroy(lineRenderer.Value.gameObject);
			}
			__instance.techLineMap.Clear();

			List<Tech> sourceBelow = [], sourceAbove = [], sourceEven = [];

			var currentTech = __instance.targetTech;

			foreach (var sourceTech in currentTech.requiredTech)
			{
				float verticalOffset = (sourceTech.center.y - currentTech.center.y);
				//SgtLogger.l(" ->> verticalOffset: " + verticalOffset + " for " + currentTech.Id + " to " + sourceTech.Id);
				if (verticalOffset < -1)
					sourceBelow.Add(sourceTech);
				else if (verticalOffset > 1)
					sourceAbove.Add(sourceTech);
				else
					sourceEven.Add(sourceTech);
			}
			sourceAbove.Sort((a, b) => a.center.y.CompareTo(b.center.y));
			sourceBelow.Sort((a, b) => -a.center.y.CompareTo(b.center.y));
			foreach (var evenTech in sourceEven)
			{
				CreateTechConnection(__instance, currentTech, evenTech);
			}
			float connectionOffset = sourceEven.Any() ? 1 : sourceBelow.Any() && sourceAbove.Any() ? 0.625f : 0;
			for (int i = 0; i < sourceBelow.Count; i++)
			{
				CreateTechConnection(__instance, currentTech, sourceBelow[i], -connectionOffset - i);
			}
			for (int i = 0; i < sourceAbove.Count; i++)
			{
				CreateTechConnection(__instance, currentTech, sourceAbove[i], connectionOffset + i);
			}

			__instance.QueueStateChanged(isSelected: false);
			if (__instance.targetTech == null)
			{
				return;
			}

			foreach (TechInstance item2 in Research.Instance.GetResearchQueue())
			{
				if (item2.tech == __instance.targetTech)
				{
					__instance.QueueStateChanged(isSelected: true);
				}
			}
		}

		static void CreateTechConnection(ResearchEntry __instance, Tech currentTech, Tech requisite, float connectionPointNr = 0)
		{
			float techBorderX = currentTech.width / 2f + 25f;
			float requisiteTechRightBorderX = (currentTech.center.x - techBorderX - (requisite.center.x + techBorderX)) + 2;


			Vector2 relativeStartPoint = Vector2.zero;
			Vector2 relativeEndPoint = Vector2.zero;

			float verticalOffset = (requisite.center.y - currentTech.center.y);
			float totalConnections = Mathf.Max(0, techConnectionPoints[requisite]);

			float YClamp = Mathf.Min(totalConnections, 3);

			float verticalStepDiff = Mathf.Clamp(verticalOffset / Y_Step, -YClamp, YClamp);
			verticalStepDiff = (float)Math.Round(verticalStepDiff * 4, MidpointRounding.ToEven) / 4f; //round to 0.25 steps, so we can use the same offset for all techs

			//if (Mathf.Abs(verticalStepDiff) == 0.5f)
			//	verticalStepDiff *= 1.25f;

			if (Mathf.Abs(verticalStepDiff) == 1.25f)
				verticalStepDiff /= 1.25f;


			float stepOffset = 12f;

			float maxHeightOffsetTarget = currentTech.height / 2f;
			//default in game is 20px offset for at least one above/below)
			float relativeYDiffTarget = Mathf.Clamp(verticalStepDiff * stepOffset, -maxHeightOffsetTarget, maxHeightOffsetTarget);
			float relativeYDiffTargetConnectionPoint = Mathf.Clamp(connectionPointNr * stepOffset, -maxHeightOffsetTarget, maxHeightOffsetTarget);

			float maxHeightOffsetSource = (requisite.height / 2f);
			//default in game is 20px offset for at least one above/below)
			float relativeYDiffSource = Mathf.Clamp(-verticalStepDiff * stepOffset, -maxHeightOffsetSource, maxHeightOffsetSource);

			relativeEndPoint = new(0, relativeYDiffTargetConnectionPoint);
			relativeStartPoint = new(0, relativeYDiffSource);

			float midPointConnectionCalc = Mathf.CeilToInt(Mathf.Max(0, totalConnections - 3) / 2f);

			float midpoint = 32 - stepOffset - (midPointConnectionCalc * stepOffset);
			if (midpoint < 0)
				midpoint = 0;

			//SgtLogger.l(" - verticalStepDiff: " + verticalStepDiff + " for " + currentTech.Id + " to " + requisite.Id + "; Midpoint: "+midpoint+"Total Cons: "+totalConnections+", relativeYDiffTarget " + relativeYDiffTarget + " , halfTechHeightTarget: " + maxHeightOffsetTarget + ", relativeYDiffSource: " + relativeYDiffSource);

			float horizontalOffsetTarget = midpoint + Mathf.Abs(relativeYDiffTarget);
			float horizontalOffsetSource = midpoint + Mathf.Abs(relativeYDiffSource);

			UILineRenderer component = Util.KInstantiateUI(__instance.linePrefab, __instance.lineContainer.gameObject, true).GetComponent<UILineRenderer>();
			component.Points = [
				new Vector2(0, 0) + relativeEndPoint,
						new Vector2(-horizontalOffsetTarget, 0) + relativeEndPoint,
						new Vector2(-horizontalOffsetSource, verticalOffset) + relativeStartPoint,
						new Vector2(-requisiteTechRightBorderX, verticalOffset) + relativeStartPoint,
						];

			//foreach (var point in component.Points)
			//{
			//	SgtLogger.l(currentTech.Id + "->" + requisite.Id + " - Point: " + point);
			//}

			component.LineThickness = __instance.lineThickness_inactive;
			component.color = __instance.inactiveLineColor;
			__instance.techLineMap.Add(requisite, component);
		}
	}
}

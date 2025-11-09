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
		const float Y_Step = 102.3f;

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

		static void PrintMatrix()
		{
			if (reverseLookupTable.Count == 0)
			{
				Console.WriteLine("Matrix is empty.");
			}
			else
			{
				// Find bounds (min/max for X and Y)
				int minX = reverseLookupTable.Keys.Min(v => v.X);
				int maxX = reverseLookupTable.Keys.Max(v => v.X);
				int minY = reverseLookupTable.Keys.Min(v => v.Y);
				int maxY = reverseLookupTable.Keys.Max(v => v.Y);

				Console.WriteLine($"Matrix from X={minX}..{maxX}, Y={minY}..{maxY}\n");
				Console.Write("|||");

				for (int i = minX; i <= maxX; i++)
					Console.Write($"     {i}     |");
				Console.WriteLine();
				// Print matrix row by row
				for (int y = maxY; y >= minY; y--)
				{
					Console.Write(y.ToString() + (y < 10 ? " |" : "|"));
					for (int x = minX; x <= maxX; x++)
					{
						Vector2I pos = new Vector2I(x, y);

						if (reverseLookupTable.TryGetValue(pos, out string value))
						{
							value = value.Length > 10 ? value.Substring(0, 10) : value.PadRight(10, ' ');
							Console.Write("[" + value + "]");
						}
						else
							Console.Write("[          ]");        // placeholder for missing entry
					}
					Console.WriteLine(); // new line for next row
				}
			}

		}

		static Tag LastModel = null;
		static Dictionary<string, Vector2I> lookupTable = [];
		static Dictionary<Vector2I, string> reverseLookupTable = [];
		static int gradient = 0;
		static void RefreshSkillScreenMatrix(SkillWidget __instance)
		{
			if (LastModel == __instance.skillsScreen.SelectedMinionModel())
				return;
			lookupTable.Clear();
			reverseLookupTable.Clear();

			var skillWidgets = __instance.skillsScreen.skillWidgets
				.OrderBy(widget => widget.Value.transform.position.y)
				.ThenBy(widget => widget.Value.transform.position.x)
				.ToList();

			var model = __instance.skillsScreen.SelectedMinionModel();
			var skills = Db.Get().Skills;
			skillWidgets.RemoveAll(id => skills.Get(id.Key).requiredDuplicantModel != null && skills.Get(id.Key).requiredDuplicantModel != model);


			float yLevel = float.MinValue;
			int colYCache = -1;
			foreach (var skillWidget in skillWidgets)
			{
				var go = skillWidget.Value;
				var skill = skills.Get(skillWidget.Key);
				float x = go.transform.position.x;
				float y = go.transform.position.y;

				if (y > yLevel)
				{
					yLevel = y;
					colYCache++;
				}
				int columnY = colYCache;

				int rowX = skill.tier;

				//SgtLogger.l(skill.Id + ", " + rowX + "," + columnY + "(" + x + "," + y + ")");
				var data = new Vector2I(rowX, columnY);
				lookupTable.Add(skill.Id, data);
				reverseLookupTable.Add(data, skill.Id);
			}
		}

		static bool IsAdvancedConnection(Skill src, string dst_id)
		{
			var dst = Db.Get().Skills.Get(dst_id);

			if (!lookupTable.TryGetValue(src.Id, out var src_coord) || !lookupTable.TryGetValue(dst_id, out var dst_coord))
				return false;

			if (Math.Abs(src_coord.y - dst_coord.y) <= 1)
				return false;

			return true;
		}

		static void CreateSkillConnection(SkillWidget __instance, Skill currentSkill, Tuple<string, Vector2> requisite, float connectionPointNr = 0)
		{
			gradient = 0;
			RefreshSkillScreenMatrix(__instance);
			string requisiteSkillId = requisite.first;
			var requisiteSkillWidget = __instance.skillsScreen.skillWidgets[requisiteSkillId];
			var requisiteConnectionPoint = requisite.second;
			var originPos = __instance.lines_left.GetPosition();
			//SgtLogger.l("TotalDistance; " + ((Vector2)originPos).ToString() + " -> " + requisite.second.ToString());

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

			var start = new Vector2(0, 0) + relativeEndPoint;
			var p1 = new Vector2(-horizontalOffsetTarget, 0) + relativeEndPoint;
			var p2 = new Vector2(-horizontalOffsetSource, verticalOffset) + relativeStartPoint;
			var dst = new Vector2(-requisiteRightBorderX, verticalOffset) + relativeStartPoint;

			if (IsAdvancedConnection(currentSkill, requisite.first))
				CreateNotSoSimpleConnection(__instance, currentSkill, requisiteSkillId, start, p1, p2, dst);
			else
				CreateSimpleConnection(__instance, start, p1, p2, dst);


			//foreach (var point in component.Points)
			//{
			//	SgtLogger.l(currentSkill.Id + "->" + requisite + " - Point: " + point);
			//}

		}

		static void CreateNotSoSimpleConnection(SkillWidget instance, Skill srcSkill, string dst_id, Vector2 src, Vector2 p1, Vector2 p2, Vector2 dst)
		{
			var skillsDb = Db.Get().Skills;
			//PrintMatrix();

			if (!lookupTable.TryGetValue(srcSkill.Id, out var src_coord) || !lookupTable.TryGetValue(dst_id, out var dst_coord))
			{
				CreateSimpleConnection(instance, src, p1, p2, dst);
				return;
			}

			var layoutRowheight = Y_Step;
			var margin = 6.0f;

			UILineRenderer startRenderer = CreateLineRenderer(instance.lines_left.transform);
			startRenderer.Points = [src, p1];
			//startRenderer.color = UIUtils.GetRainbowColorForIndex(gradient++);
			List<UILineRenderer> lines = [startRenderer];
			Vector2 segmentStart = p1;
			Vector2 segmentEnd = p1;

			bool descending = src_coord.y > dst_coord.y;
			segmentEnd.y = 0;

			//SgtLogger.l("StartPoint: " + src_coord.ToString() + ", DestinationPoint: " + dst_coord.ToString());

			int direction = descending ? -1 : 1;
			//SgtLogger.l("segmentEnd: " + segmentEnd);
			for (int i = src_coord.y + direction; descending ? i > dst_coord.y : i < dst_coord.y ; i += direction)
			{
				Vector2I crossCheckSrc = new Vector2I(src_coord.x,i);
				Vector2I crossCheckDst = new Vector2I(dst_coord.x,i );
				//SgtLogger.l("Checking connection between " + crossCheckSrc + " and " + crossCheckDst);
				segmentEnd.y += layoutRowheight * direction;
				//SgtLogger.l("segmentEnd: " + segmentEnd);
				if (!reverseLookupTable.TryGetValue(crossCheckSrc, out string checkSkillSrc) || !reverseLookupTable.TryGetValue(crossCheckDst, out string checkSkillDst))
				{
					//SgtLogger.l("no skil found on one of the sides");
					continue;
				}

				Skill crossSrc = skillsDb.Get(checkSkillSrc);
				bool hasConnection = crossSrc.priorSkills.Contains(checkSkillDst);
				//SgtLogger.l(checkSkillSrc + " has connection to " + checkSkillDst + ": " + hasConnection);

				if (hasConnection)
				{
					segmentEnd.y -= margin * direction;
					var segmentRenderer = CreateLineRenderer(instance.lines_left.transform);
					//segmentRenderer.color = UIUtils.GetRainbowColorForIndex(gradient++);
					segmentRenderer.Points = [segmentStart, segmentEnd];
					//SgtLogger.l("adding segment between " + segmentStart + " and " + segmentEnd);
					lines.Add(segmentRenderer);

					segmentEnd.y += margin * direction;
					segmentStart = segmentEnd;
					segmentStart.y += margin * direction;
				}
			}

			UILineRenderer endRenderer = CreateLineRenderer(instance.lines_left.transform);
			endRenderer.Points = [segmentStart, p2, dst];
			//endRenderer.color = UIUtils.GetRainbowColorForIndex(gradient++);
			lines.Add(endRenderer);
			instance.lines = instance.lines.AddRangeToArray(lines.ToArray());
		}

		static void CreateSimpleConnection(SkillWidget instance, Vector2 src, Vector2 p1, Vector2 p2, Vector2 dst)
		{
			UILineRenderer component = CreateLineRenderer(instance.lines_left.transform);
			component.Points = [src, p1, p2, dst];
			instance.lines = instance.lines.Append(component);
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

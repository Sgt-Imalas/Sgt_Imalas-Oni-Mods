using HarmonyLib;
using PeterHan.PLib.Core;
using ProcGen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace UtilLibs.SharedTweaks
{
	/// <summary>
	/// space the connection lines horizontally to avoid them overlapping
	/// </summary>
	public sealed class ResearchScreenBetterConnectionLines : PForwardedComponent
	{
		///base64 encoded sprite
		static readonly string crossIcon_base64 = "iVBORw0KGgoAAAANSUhEUgAAAAYAAAA2CAYAAADktujlAAABb2lDQ1BpY2MAACiRdZG/S8NAFMe/bZWKrXTQQcQhQxWHFkRFHKWCXapDW8GqS3JNWiFJwyVFiqvg4lBwEF38Nfgf6Cq4KgiCIoi4uvprkRLfNYUUae94eR++ue/j3TsgmNGZYffMA4bp8Gw6Ja0W1qTwOyIYAChmZGZbS7nFPLqun0cERH5Iilrdz3VckaJqMyDQRzzLLO4QUzfIbDmW4D3iIVaWi8QnxAlODRLfCl3x+E1wyeMvwTyfXQCCoqZUamOljVmZG8QTxHFDr7JWP+ImUdVcyVEeoRiFjSzSSEGCgio2ocNBkrJJM+vsm2z6llEhD6OvhRo4OUookzdBapWqqpQ10lXaOmpi7v/naWvTU171aArofXXdzzEgvA806q77e+q6jTMg9AJcm76/QnOa+ya97mvxYyC2A1ze+JpyAFztAsPPlszlphSiCGoa8HFBj18ABu+B/nVvVq3/OH8C8tv0RHfA4REwTudjG38UdWgTWCvYsgAAAAlwSFlzAAAuIwAALiMBeKU/dgAAAY1JREFUOE/NlN1KAzEQhXcm2S4WoaCgL+CVD+Cl1z6ET+4b+AMKtbaW3YnnzCZLurTglRgIbebLnMxMJiv3D4/aHBlHjdz3B0DiLCApa4Jpkf9zPXkUyCgnUMsROKSxluPuCSyqcw48uhxJMQZ6UWaZ3QlopM3BKkvR2OYZCC6yRwGUdnBVAQbCGQmuZ4AeB6Doj1IpJUglbVKiN6AsUBSNyYbLlEyTmYcqEgJAiPv9bgWj2tB7HqIhCEbcfq2XZibW9wrZoBiwU8rOISXjTJgsIjx2200LA85OfieUqYqay/hffuTm9u7pWLixbReDMTEbmFyDvJkg6t4utoasrRdmXkoise3ONl5ERa0agjDWquuW67HsA8uOynrZNULrPTWmjcWxGUQxAUT1VbDiebl1Or8oLJ4RRQHskKl9ALyRKVWAd8nLqYZ7mwG2kZ/xkQHlyntx8JmvsjyDqdt3+UbLa2IQ/tS+q8s/eFF91QO/AzbrGtzKeAb/lMFN3njzbwk3ucepD1mtMjvp1PIHpY18YaNGfVIAAAAASUVORK5CYII=";
		static Sprite crossIcon;
		static void LoadIcon()
		{
			byte[] imageBytes = System.Convert.FromBase64String(crossIcon_base64);

			Texture2D tex = new Texture2D(6, 54);
			tex.LoadImage(imageBytes);
			tex.filterMode = FilterMode.Bilinear;

			crossIcon = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100.0f);
		}


		public static void Register()
		{
			new ResearchScreenBetterConnectionLines().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 1, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				LoadIcon();
				var targetMethod = AccessTools.Method(typeof(ResearchEntry), nameof(ResearchEntry.SetupLines));
				var onSpawnPostfix = AccessTools.Method(typeof(ResearchScreenBetterConnectionLines), nameof(CreateLinesPostfix));
				///change the line renderers to be more spaced out
				plibInstance.Patch(targetMethod, postfix: new(onSpawnPostfix, Priority.HigherThanNormal));
				Debug.Log(Assembly.GetExecutingAssembly().GetName().Name + ": " + this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(Assembly.GetExecutingAssembly().GetName().Name + ": " + this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}

		/// <summary>
		/// vertical distance between 2 directly vertically adjacent tech nodes
		/// </summary>
		const float Y_Step = 250;

		static Dictionary<Tech, int> techConnectionPoints = null;

		public static void CreateLinesPostfix(ResearchEntry __instance)
		{
			RefreshResearchMatrix(__instance);

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

		static bool init = false;
		static Dictionary<string, Vector2I> lookupTable = [];
		static Dictionary<Vector2I, string> reverseLookupTable = [];
		static int gradient = 0;
		static void RefreshResearchMatrix(ResearchEntry __instance)
		{
			if (init)
				return;

			init = true;
			lookupTable.Clear();
			reverseLookupTable.Clear();

			var entries = __instance.researchScreen.entryMap
				.OrderBy(widget => widget.Value.transform.position.y)
				.ThenBy(widget => widget.Value.transform.position.x)
				.ToList();


			float yLevel = float.MinValue;
			int colYCache = -1;
			foreach (var researchEntry in entries)
			{
				var go = researchEntry.Value;
				var tech = researchEntry.Key;
				float x = go.transform.position.x;
				float y = go.transform.position.y;

				if (y > yLevel)
				{
					yLevel = y;
					colYCache++;
				}
				int columnY = colYCache;

				int rowX = tech.tier;

				//SgtLogger.l(skill.Id + ", " + rowX + "," + columnY + "(" + x + "," + y + ")");
				var data = new Vector2I(rowX, columnY);
				if (!lookupTable.ContainsKey(tech.Id))
					lookupTable.Add(tech.Id, data);
				else
					SgtLogger.warning("ResearchMatrix duplicate key detected for tech " + tech.Id + " and value " + data.ToString());
				if (!reverseLookupTable.ContainsKey(data))
					reverseLookupTable.Add(data, tech.Id);
				else
					SgtLogger.warning("ResearchMatrix duplicate reverse key detected for value " + tech.Id + " and key " + data.ToString());
			}
			//PrintMatrix();
		}



		static void CreateTechConnection(ResearchEntry __instance, Tech currentTech, Tech requisite, float connectionPointNr = 0)
		{
			float techBorderX = currentTech.width / 2f + 25f;
			float requisiteTechRightBorderX = (currentTech.center.x - techBorderX - (requisite.center.x + techBorderX)) + 2;

			var prevEntry = __instance.researchScreen.entryMap[requisite];

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

			Vector2 start = new Vector2(0, 0) + relativeEndPoint,
					p1 = new Vector2(-horizontalOffsetTarget, 0) + relativeEndPoint,
					p2 = new Vector2(-horizontalOffsetSource, verticalOffset) + relativeStartPoint,
					end = new Vector2(-requisiteTechRightBorderX, verticalOffset) + relativeStartPoint;

			UILineRenderer component = Util.KInstantiateUI(__instance.linePrefab, __instance.lineContainer.gameObject, true).GetComponent<UILineRenderer>();
			component.Points = [start,p1,p2,end];


			//foreach (var point in component.Points)
			//{
			//	SgtLogger.l(currentTech.Id + "->" + requisite.Id + " - Point: " + point);
			//}

			component.LineThickness = __instance.lineThickness_inactive;
			component.color = __instance.inactiveLineColor;
			__instance.techLineMap.Add(requisite, component);



			if (!lookupTable.TryGetValue(currentTech.Id, out var startPos) || !lookupTable.TryGetValue(requisite.Id, out var endPos))
				return;
			int yDiff = startPos.y - endPos.y;

			if (Math.Abs(yDiff) <= 1)
				return;
			//it might overlap here!

			bool descending = yDiff > 0;
			var techs = Db.Get().Techs;
			int step = descending ? -1 : 1;

			for (int i = startPos.y + step; descending ? i > endPos.y : i < endPos.y; i += step)
			{
				Vector2I checkSrc = new(startPos.x, i), checkDest = new Vector2I(endPos.x, i);
				Vector2I checkSrc2 = new(startPos.x + 1, i), checkDest2 = new Vector2I(endPos.x - 1, i);
				if ((!reverseLookupTable.TryGetValue(checkSrc, out var srcTechId) && !reverseLookupTable.TryGetValue(checkSrc2, out srcTechId))
					|| (!reverseLookupTable.TryGetValue(checkDest, out var destTech) && !reverseLookupTable.TryGetValue(checkDest2, out destTech)))
					continue;
				var srcTech = techs.Get(srcTechId);
				if (srcTech.requiredTech.Any(t => t.Id == destTech))
				{
					SgtLogger.l("crossing detected for " + currentTech.Id + " at y level: " + i + ", with connection between " + srcTechId + " and " + destTech);
					var alt = __instance.researchScreen.entryMap[srcTech].rectTransform().anchoredPosition - __instance.rectTransform().anchoredPosition;
					var altPos = new Vector2(-horizontalOffsetTarget, alt.y+1.25f);// + relativeEndPoint;
					//altPos.y -= 6f;
					//SgtLogger.l("alt cross anchored pos: " + alt.y);
					CreateCrossRendererRelative(__instance, altPos);
				}
			}


		}

		static GameObject IconPrefab = null;

		static void CreateCrossRenderer(ResearchEntry __instance, Vector2 pos, Color? c = null)
		{
			if (IconPrefab == null)
			{
				IconPrefab = Util.KInstantiateUI(__instance.iconPrefab.transform.Find("Icon_FG").gameObject);
				IconPrefab.GetComponent<Image>().sprite = Assets.GetSprite("unknown");//crossIcon;
			}
			var icon = Util.KInstantiateUI(IconPrefab, __instance.lineContainer.gameObject, true);

			icon.name = __instance.targetTech.Id + "_LinePreventionMeasure";
			icon.transform.SetPosition(pos);
			var scale = icon.transform.localScale;
			//scale.x *= 1.15f;
			//scale.y *= 0.85f;
			icon.transform.localScale = scale;
			if (c.HasValue)
				icon.GetComponent<Image>().color = c.Value;

			SgtLogger.l("setting icon pos to " + pos.x + "," + pos.y + " with color: " + c.ToString());
		}
		static void CreateCrossRendererRelative(ResearchEntry __instance, Vector2 pos, Color? c = null)
		{
			if (IconPrefab == null)
			{
				IconPrefab = Util.KInstantiateUI(__instance.iconPrefab.transform.Find("Icon_FG").gameObject);
				IconPrefab.GetComponent<Image>().sprite = crossIcon;
			}
			var icon = Util.KInstantiateUI(IconPrefab, __instance.lineContainer.gameObject, true);

			icon.name = __instance.targetTech.Id + "_LinePreventionMeasure";
			icon.transform.localPosition = (pos);
			var scale = icon.transform.localScale;
			//scale.x *= 1.15f;
			//scale.y *= 0.85f;
			icon.transform.localScale = scale;
			if (c.HasValue)
				icon.GetComponent<Image>().color = c.Value;

			SgtLogger.l("setting icon pos to " + pos.x + "," + pos.y + " with color: " + c.ToString());
		}
	}
}

using HarmonyLib;
using PeterHan.PLib.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using static STRINGS.BUILDINGS.PREFABS;

namespace UtilLibs.SharedTweaks
{
	/// <summary>
	/// splits the codex entry converter panel for elementconverter buildings with multiple elementconverters
	/// </summary>
	public sealed class ElementConverterRecipePanelSplit : PForwardedComponent
	{
		public static void Register()
		{
			new ElementConverterRecipePanelSplit().RegisterForForwarding();
		}
		public override Version Version => new Version(1, 0, 0, 0);

		public override void Initialize(Harmony plibInstance)
		{
			try
			{
				var targetMethod = FindInnerCheckPrefabMethod();
				if (targetMethod == null)
					return;

				MethodInfo m_prefix = AccessTools.Method(typeof(ElementConverterRecipePanelSplit), nameof(CheckValiditiy_Prefix));
				MethodInfo m_transpiler = AccessTools.Method(typeof(ElementConverterRecipePanelSplit), nameof(Transpiler));

				plibInstance.Patch(targetMethod, prefix: new(m_prefix), transpiler: new (m_transpiler));
				Debug.Log(this.GetType().ToString() + " successfully patched");
			}
			catch (Exception e)
			{
				Debug.LogWarning(this.GetType().ToString() + " patch failed!");
				Debug.LogWarning(e.Message);
			}
		}
		public static MethodBase FindInnerCheckPrefabMethod()
		{
			var targetType = typeof(CodexEntryGenerator_Elements);
			{
				foreach (var method in targetType.GetMethods(AccessTools.all))
				{
					if (!method.Name.Contains("CheckPrefab"))
						continue;

					return method;
				}

			}
			Debug.LogWarning("CodexEntryGenerator_Elements_GetElementEntryContext_Patch FAILED!\n: failed to find target method for CheckPrefab");
			return null;
		}

		/// <summary>
		/// defines if the transpiler should override the elementconverter codex gen in the transpiler
		/// </summary>
		/// <param name="prefab"></param>
		public static void CheckValiditiy_Prefix(GameObject prefab, CodexEntryGenerator_Elements.CodexElementMap usedMap, CodexEntryGenerator_Elements.CodexElementMap made)
		{
			MultiConverter = false;
			if (!prefab.TryGetComponent<BuildingComplete>(out _))
				return;

			var converters = prefab.GetComponents<ElementConverter>();
			if (converters == null || converters.Length <= 1)
				return;

			if (converters.Any(c => c.inputIsCategory))
				return;

			_prefab = prefab;
			_usedMap = usedMap;
			_made = made;
			_converters = _prefab.GetComponents<ElementConverter>().Where(ec => ec.consumedElements != null && ec.outputElements != null).ToArray();
			MultiConverter = _converters.Length > 1;

		}
		static bool MultiConverter = false;
		static GameObject _prefab;
		static ElementConverter[] _converters = [];
		static CodexEntryGenerator_Elements.CodexElementMap _usedMap, _made;

		public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig, MethodBase original)
		{

			TranspilerHelper.GetLocIndexOfFirst<CodexEntryGenerator_Elements.ConversionEntry>(original, out int ce1_index); //should be 9

			MethodInfo redistribute = AccessTools.Method(typeof(ElementConverterRecipePanelSplit), nameof(ElementConverterRecipePanelSplit.RedistributeExtraConverters));
			FieldInfo ce_outSet = AccessTools.Field(typeof(CodexEntryGenerator_Elements.ConversionEntry), nameof(CodexEntryGenerator_Elements.ConversionEntry.outSet));

			//Debug.Log("CE1 INDEX: " + ce1_index);
			foreach (CodeInstruction ci in orig)
			{
				if (ci.StoresField(ce_outSet))
				{
					yield return ci;
					yield return new CodeInstruction(OpCodes.Ldloc_S, ce1_index);
					yield return new CodeInstruction(OpCodes.Call, redistribute);
					//yield return original;
				}
				else
					yield return ci;
			}
		}

		static void RedistributeExtraConverters(CodexEntryGenerator_Elements.ConversionEntry ce1)
		{
			if (!MultiConverter || _prefab == null || ce1.title != _prefab.GetProperName() || _converters == null || ce1.inSet == null || ce1.outSet == null)
				return;

			Debug.Log("Splitting merged elementconverter panel for " + global::STRINGS.UI.StripLinkFormatting(_prefab.GetProperName()) + " into " + _converters.Length + " panels.");

			for (int i = _converters.Length - 1; i >= 1; i--)
			{
				var converterToExclude = _converters[i];
				if (converterToExclude.consumedElements == null || converterToExclude.outputElements == null)
					continue;

				var ce2 = new CodexEntryGenerator_Elements.ConversionEntry();
				ce2.title = _prefab.GetProperName();
				ce2.prefab = _prefab;
				ce2.inSet = new();
				ce2.outSet = new();

				foreach (var inputElement in converterToExclude.consumedElements)
				{
					var lastElementInHashset = ce1.inSet.LastOrDefault(item => item.tag == inputElement.Tag && item.amount == inputElement.MassConsumptionRate && item.continuous);
					if (lastElementInHashset != null)
					{
						ce1.inSet.Remove(lastElementInHashset);
						//if it wasnt in entry 1 for whatever reason, maybe avoid adding it to the new entry
						ce2.inSet.Add(new ElementUsage(inputElement.Tag, inputElement.MassConsumptionRate, true));
					}
				}
				foreach (var outputElement in converterToExclude.outputElements)
				{

					var lastElementInHashset = ce1.outSet.LastOrDefault(item => item.tag == ElementLoader.FindElementByHash(outputElement.elementHash).tag && item.amount == outputElement.massGenerationRate && item.continuous);
					if (lastElementInHashset != null)
					{
						ce1.outSet.Remove(lastElementInHashset);
						//if it wasnt in entry 1 for whatever reason, maybe avoid adding it to the new entry
						ce2.outSet.Add(new ElementUsage(ElementLoader.FindElementByHash(outputElement.elementHash).tag, outputElement.massGenerationRate, true));
					}
				}
				///mirrored from game method:
				if (ce2.inSet.Count > 0 && ce2.outSet.Count > 0)
					_usedMap.Add(_prefab.PrefabID(), ce2);
				foreach (ElementUsage elementUsage in ce2.inSet)
					_usedMap.Add(elementUsage.tag, ce2);
				foreach (ElementUsage elementUsage in ce2.outSet)
					_made.Add(elementUsage.tag, ce2);
			}

		}
	}
}

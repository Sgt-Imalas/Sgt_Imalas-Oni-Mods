//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection.Emit;
//using System.Text;
//using UtilLibs;

//////Unused code; can be used to sort the wattage ordering to fix the maxsafewattage handling 4k wires as max
//namespace _3GuBsVisualFixesNTweaks.Patches
//{
//	internal class ElectricalUtilityNetwork_Patches
//	{
//		private static readonly List<Wire.WattageRating> wattagesSorted = [];
//		private static readonly Dictionary<Wire.WattageRating, int> wattagesOrdered = new Dictionary<Wire.WattageRating, int>();
//		private static bool wiresInitialized = false;

//		[HarmonyPatch(typeof(ElectricalUtilityNetwork), nameof(ElectricalUtilityNetwork.GetMaxSafeWattage))]
//		public class ElectricalUtilityNetwork_GetMaxSafeWattage_Patch
//		{
//			public static bool Prefix(ElectricalUtilityNetwork __instance, ref float __result, List<WireUtilityNetworkLink>[] bridgeGroups)
//			{
//				ElectricalUtilityNetwork_AddItem_Patch.InitIndices(__instance);

//				for (int rating = 0; rating < wattagesSorted.Count; ++rating)
//				{
//					var sortedRating = (int)wattagesSorted[rating];

//					List<Wire> wireGroup = __instance.wireGroups[sortedRating];
//					bool hasWireOfGroup = wireGroup != null && wireGroup.Count > 0;

//					List<WireUtilityNetworkLink> bridgeGroup = bridgeGroups[sortedRating];
//					bool hasBridgeOfGroup = bridgeGroup != null && bridgeGroup.Count > 0;

//					if (hasWireOfGroup || hasBridgeOfGroup)
//					{
//						__result = Wire.GetMaxWattageAsFloat((Wire.WattageRating)sortedRating);
//						return false;
//					}
//				}
//				__result = 0;
//				return false;
//			}
//		}

//		// [HarmonyPatch(typeof(ElectricalUtilityNetwork), nameof(ElectricalUtilityNetwork.AddItem))]
//		public class ElectricalUtilityNetwork_AddItem_Patch
//		{

//			//public static IEnumerable<CodeInstruction> Transpiler(ILGenerator _, IEnumerable<CodeInstruction> orig)
//			//         {
//			//	var wireMaxWattageRating = AccessTools.Field(typeof(Wire),nameof(Wire.MaxWattageRating));
//			//	var injected = AccessTools.Method(typeof(ElectricalUtilityNetwork_AddItem_Patch), nameof(GetWireRatingIndex));

//			//	foreach(var ci in orig)
//			//	{
//			//		yield return ci;
//			//		if(ci.LoadsField(wireMaxWattageRating))
//			//		{
//			//			yield return new CodeInstruction(OpCodes.Ldarg_0);
//			//			yield return new CodeInstruction(OpCodes.Call, injected);
//			//		}
//			//	}
//			//}
//			internal static void InitIndices(ElectricalUtilityNetwork network)
//			{
//				if (wiresInitialized)
//					return;
//				wiresInitialized = true;

//				int wireGroupCount = network.wireGroups.Length;
//				List<Wire.WattageRating> wattages = new List<Wire.WattageRating>(wireGroupCount);
//				for (int ratingVal = 0; ratingVal < wireGroupCount; ++ratingVal)
//				{
//					Wire.WattageRating rating = (Wire.WattageRating)ratingVal;
//					float wattage = Wire.GetMaxWattageAsFloat(rating);
//					if (wattage <= 0)
//						continue;
//					wattages.Add(rating);
//				}
//				wattages.Sort(WireWattageComparer);
//				wattagesSorted.Clear();
//				for (int index = 0; index < wattages.Count; ++index)
//				{
//					Wire.WattageRating rating = wattages[index];
//					wattagesOrdered[rating] = index;
//					wattagesSorted.Add(rating);
//				}
//				Debug.Log("Sorted wattage wire ratings:");
//				foreach (var wire in wattages)
//				{
//					Debug.Log($"wire rating wattage {wire} has capacity {Wire.GetMaxWattageAsFloat(wire)}");
//				}
//			}
//			private static Wire.WattageRating GetWireRatingIndex(Wire.WattageRating rating, ElectricalUtilityNetwork network)
//			{
//				InitIndices(network);
//				if (wattagesOrdered.TryGetValue(rating, out var index))
//					return (Wire.WattageRating)index;
//				return rating;
//			}

//			static int WireWattageComparer(Wire.WattageRating a, Wire.WattageRating b)
//			{
//				float wattageA = Wire.GetMaxWattageAsFloat(a);
//				float wattageB = Wire.GetMaxWattageAsFloat(b);
//				return wattageA.CompareTo(wattageB);
//			}
//		}
//	}
//}

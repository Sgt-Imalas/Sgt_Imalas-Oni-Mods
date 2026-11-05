using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;

namespace UtilLibs
{
	public static class Extensions
	{
		public static void AppendLine(this in KleiStringBuilder sb, ReadOnlySpan<char> chars = default)
		{
			sb.Append(chars);
			sb.Append('\n');
		}
		public static bool TryGetInterfaceSMIOrComponent<InterfaceType>(this GameObject go, out InterfaceType interfaceImplementation) where InterfaceType : class
		{
			if(go.TryGetComponent<InterfaceType>(out interfaceImplementation))
				return true;
			if(go.TryGetSMI<InterfaceType>(out interfaceImplementation))
				return true;
			return false;
		}
		public static bool TryGetSMI<StateMachineInstanceType>(this Component cmp, out StateMachineInstanceType smi) where StateMachineInstanceType : class
		{
			return cmp.gameObject.TryGetSMI<StateMachineInstanceType>(out smi);
		}
		public static bool TryGetSMI<StateMachineInstanceType>(this GameObject go, out StateMachineInstanceType smi) where StateMachineInstanceType : class
		{
			smi = null;
			if (!go.TryGetComponent<StateMachineController>(out var smc))
				return false;

			smi = smc.GetSMI<StateMachineInstanceType>();
			return smi != null;
		}

		public static bool TryGetTokenValue<T>(this JObject jObject, string key, out T value, JsonSerializer serializer = null)
		{
			value = default;
			if (jObject == null || !jObject.TryGetValue(key, out JToken token) || token.Type == JTokenType.Null)
				return false;

			if (token is JValue jValue && jValue.Value is T direct)
			{
				value = direct;
				return true;
			}

			try
			{
				value = serializer != null ? token.ToObject<T>(serializer) : token.ToObject<T>();
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
		{
			var known = new HashSet<TKey>();
			return source.Where(element => known.Add(keySelector(element)));
		}


		public static void AddRange(this IDictionary dict, IDictionary other)
		{
			foreach (DictionaryEntry item in other)
			{
				dict.Add(item.Key, item.Value);
			}
		}
		public static T CreateDelegate<T>(this MethodInfo method) where T : MulticastDelegate => (T)Delegate.CreateDelegate(typeof(T), method);
		

		/// <summary>
		/// Returns the overload that no other overload of the same name forwards away from,
		/// i.e. the end of the wrapper chain. Null if none or ambiguous.
		/// </summary>
		public static MethodInfo GetLatestOverloadPatchTarget(this Type type, string name, bool isStatic)
		{
			var overloads = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | (isStatic ? BindingFlags.Static : BindingFlags.Instance)).Where(m => m.Name == name).ToList();
			if (overloads.Count == 0)
				return null;

			// overload -> sibling overloads it calls
			var forwardsTo = new Dictionary<MethodBase, List<MethodBase>>();
			foreach (var m in overloads)
			{
				var callees = new List<MethodBase>();
				try
				{
					foreach (var instr in PatchProcessor.ReadMethodBody(m))
					{
						if (instr.Key != OpCodes.Call && instr.Key != OpCodes.Callvirt)
							continue;
						if (instr.Value is not MethodBase callee || Same(callee, m))
							continue;

						var sibling = overloads.FirstOrDefault(o => Same(o, callee));
						if (sibling != null && !callees.Contains(sibling))
							callees.Add(sibling);
					}
				}
				catch (Exception e)
				{
					Debug.LogWarning($"[MyMod] Could not read body of {m}: {e.Message}");
				}
				forwardsTo[m] = callees;
			}

			// when theres one "newest" method thats not called by any other overload, return it.
			var leaves = overloads
				.Where(m => forwardsTo[m].Count == 0 && !m.IsDefined(typeof(ObsoleteAttribute), false))
				.ToList();

			if (leaves.Count == 1)
				return leaves[0];

			if (leaves.Count == 0)
				return null;

			// multiple "newest" methods: pick the one most wrappers eventually reach. Unique winner only.
			var reach = leaves
				.Select(leaf => (leaf, count: overloads.Count(o => !Same(o, leaf) && Reaches(o, leaf, forwardsTo, new HashSet<MethodBase>()))))
				.OrderByDescending(x => x.count)
				.ToList();

			if (reach.Count > 1 && reach[0].count == reach[1].count)
				return null;

			return reach[0].leaf;
		}
		private static bool Same(MethodBase a, MethodBase b) => a.MetadataToken == b.MetadataToken && a.Module == b.Module;

		private static bool Reaches(MethodBase from, MethodBase to, Dictionary<MethodBase, List<MethodBase>> graph, HashSet<MethodBase> seen)
		{
			if (!seen.Add(from))
				return false;
			foreach (var next in graph[from])
				if (Same(next, to) || Reaches(next, to, graph, seen))
					return true;
			return false;
		}


		/// <summary>
		/// Compresses a string.
		/// </summary>
		/// <param name="text">The text.</param>
		/// <returns></returns>
		public static string CompressString(this string text)
		{
			//return "```" + text + "```";
			byte[] buffer = Encoding.UTF8.GetBytes(text);
			byte[] compressedData;
			using (var memoryStream = new MemoryStream())
			{
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
					gZipStream.Write(buffer, 0, buffer.Length);
				compressedData = memoryStream.ToArray();
			}
			var gZipBuffer = new byte[compressedData.Length + 4];
			Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
			Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);
			return Convert.ToBase64String(gZipBuffer);
		}

		/// <summary>
		/// Decompresses a string.
		/// </summary>
		/// <param name="compressedText">The compressed text.</param>
		/// <returns></returns>
		public static string DecompressString(this string compressedText)
		{
			try
			{
				byte[] gZipBuffer = Convert.FromBase64String(compressedText);
				using var compressedStream = new MemoryStream(gZipBuffer, 4, gZipBuffer.Length - 4);
				using var gZipStream = new GZipStream(compressedStream, CompressionMode.Decompress);
				using var uncompressedStream = new MemoryStream();
				gZipStream.CopyTo(uncompressedStream);
				return Encoding.UTF8.GetString(uncompressedStream.ToArray());
			}
			catch (Exception ex)
			{
				SgtLogger.error("Decompression Failure: " + ex.Message);
				return string.Empty;
			}
		}


		//return typeof(EntityTemplates).GetOverloadWithMostArguments(nameof(EntityTemplates.ExtendEntityToBasicPlant), true);

	}
}

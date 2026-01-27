using Database;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static Database.ArtableStatuses;

namespace BigSmallSculptures
{
	internal class ModAssets
	{
		public static string DecorPackS_SnowSculpture_ID = "SnowSculptures_SnowSculpture";
		public static string DecorPackA_GlassSculpture_ID = "DecorPackA_GlassSculpture";


		/// <summary>
		/// generated id->scale mapping of skins that need scaling on other sculpture types
		/// </summary>
		static Dictionary<string, float> GeneratedSculptureSkinScaleModifiers;
		public static void RegisterSkinScaleModifier(string skinId, float scaleMultiplier = 1f)
		{
			if (GeneratedSculptureSkinScaleModifiers == null)
				GeneratedSculptureSkinScaleModifiers = new();
			GeneratedSculptureSkinScaleModifiers[skinId] = scaleMultiplier;
		}
		public static bool TryGetCachedSkinScaleModifier(string skinId, out float scaleMultiplier)
		{
			scaleMultiplier = 1f;
			if (GeneratedSculptureSkinScaleModifiers != null && GeneratedSculptureSkinScaleModifiers.ContainsKey(skinId))
			{
				scaleMultiplier = GeneratedSculptureSkinScaleModifiers[skinId];
				return true;
			}
			return false;
		}

		static Dictionary<string, Tuple<ArtableStatusType, ArtableStatusType>> RemappedArtableRarities = new();
		static Dictionary<string, List<Tuple<string, float>>> SculpturesAllowedOnOtherSculptures;
		public static void InitializeDefaults()
		{
			AddSculptureSkinsAllowedOnOther(SmallSculptureConfig.ID, MarbleSculptureConfig.ID, 1.5f);
			AddSculptureSkinsAllowedOnOther(SmallSculptureConfig.ID, SculptureConfig.ID);
			AddSculptureSkinsAllowedOnOther(MarbleSculptureConfig.ID, SmallSculptureConfig.ID, 0.5f);

			AddSculptureSkinsAllowedOnOther(IceSculptureConfig.ID, DecorPackA_GlassSculpture_ID);
			AddSculptureSkinsAllowedOnOther(IceSculptureConfig.ID, DecorPackS_SnowSculpture_ID);
			AddSculptureSkinsAllowedOnOther(DecorPackA_GlassSculpture_ID, IceSculptureConfig.ID);
			AddSculptureSkinsAllowedOnOther(DecorPackS_SnowSculpture_ID, IceSculptureConfig.ID);

			///highest tier of ice sculpture is tier 2, so all skins that get generated from it need to be tier 3, and tier 3 skins that become it need to be downgraded to tier 2
			AddRarityRemap(IceSculptureConfig.ID, ArtableStatusType.LookingOkay, ArtableStatusType.LookingGreat);
		}

		static ArtableStatusItem GetMappedStatusItem(ArtableStatusType type)
		{
			switch (type)
			{
				default:
				case ArtableStatusType.LookingUgly:
					return Db.Get().ArtableStatuses.LookingUgly;
				case ArtableStatusType.LookingOkay:
					return Db.Get().ArtableStatuses.LookingOkay;
				case ArtableStatusType.LookingGreat:
					return Db.Get().ArtableStatuses.LookingGreat;
			}
		}
		static int GetDefaultDecor(ArtableStatusType type)
		{
			switch (type)
			{
				default:
				case ArtableStatusType.LookingUgly:
					return 5;
				case ArtableStatusType.LookingOkay:
					return 10;
				case ArtableStatusType.LookingGreat:
					return 15;
			}
		}
		public static bool TryGetRarityRemap(string sourceSculptureBuildingId, string targetSculptureBuildingId, ArtableStatusItem currentStatus, out ArtableStatusItem targetArtableStatus, out int decorBonus)
		{
			targetArtableStatus = null;
			Tuple<ArtableStatusType, ArtableStatusType> tuple;
			decorBonus = 0;
			if (RemappedArtableRarities.TryGetValue(sourceSculptureBuildingId, out tuple))
			{
				if (currentStatus.StatusType == tuple.first)
				{
					targetArtableStatus = GetMappedStatusItem(tuple.second);
					decorBonus = GetDefaultDecor(tuple.second);
					return true;
				}
			}
			else if (RemappedArtableRarities.TryGetValue(targetSculptureBuildingId, out tuple))
			{
				if (currentStatus.StatusType == tuple.second)
				{
					targetArtableStatus = GetMappedStatusItem(tuple.first);
					decorBonus = GetDefaultDecor(tuple.first);
					return true;
				}
			}
			return false;
		}
		public static void AddRarityRemap(string sculptureBuildingId, ArtableStatusType fromAddedSkinRarity, ArtableStatusType toAddedSkinRarity)
		{
			RemappedArtableRarities[sculptureBuildingId] = new Tuple<ArtableStatusType, ArtableStatusType>(fromAddedSkinRarity, toAddedSkinRarity);
		}
		public static bool BuildingSkinsAllowedOnOtherSculptures(string sourceSculptureId, out List<Tuple<string, float>> allowedOtherSculptures)
		{
			allowedOtherSculptures = null;
			if (SculpturesAllowedOnOtherSculptures != null && SculpturesAllowedOnOtherSculptures.ContainsKey(sourceSculptureId))
			{
				allowedOtherSculptures = SculpturesAllowedOnOtherSculptures[sourceSculptureId];
				return true;
			}
			return false;
		}

		/// <summary>
		/// if someone wants to reflect.
		/// </summary>
		/// <param name="sourceSculptureId">sculpture building id the skin is originally from</param>
		/// <param name="targetSculptureId">sculpture building id it should also be allowed on/param>
		/// <param name="scalingMultiplier">optional kanim scale multiplier</param>
		public static void AddSculptureSkinsAllowedOnOther(string sourceSculptureId, string targetSculptureId, float scalingMultiplier = 1f)
		{
			if (SculpturesAllowedOnOtherSculptures == null)
				SculpturesAllowedOnOtherSculptures = [];
			if (!SculpturesAllowedOnOtherSculptures.ContainsKey(sourceSculptureId))
				SculpturesAllowedOnOtherSculptures[sourceSculptureId] = new List<Tuple<string, float>>();
			var data = new Tuple<string, float>(targetSculptureId, scalingMultiplier);
			if (!SculpturesAllowedOnOtherSculptures[sourceSculptureId].Contains(data))
				SculpturesAllowedOnOtherSculptures[sourceSculptureId].Add(data);
		}

		internal static void BogdanovIt()
		{
			SgtLogger.l("Dump it ->");
			SgtLogger.l("");
			foreach (var kvp in SculpturesAllowedOnOtherSculptures)
			{
				SgtLogger.l($"-{kvp.Key}:");
				foreach (var tuple in kvp.Value)
				{
					SgtLogger.l($"--{tuple.first}");
				}
			}
			SgtLogger.l("");
			SgtLogger.l("Registered:");
			foreach (var kvp in GeneratedSculptureSkinScaleModifiers)
			{
				SgtLogger.l($"--{kvp.Key} -> {kvp.Value}");
			}

			foreach (var skin in Db.GetArtableStages().resources)
			{
				SgtLogger.l(skin.prefabId + " -> " + skin.id + " -> " + skin.Name);
			}

		}

		internal static void Load()
		{
			ModAssets.InitializeDefaults();
		}
	}
}

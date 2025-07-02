using HarmonyLib;
using Klei;
using Klei.CustomSettings;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PeterHan.PLib.Core;
using ProcGen;
using ProcGenGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using UnityEngine;
using UtilLibs;
using static AnimExportTool.Patches.MainMenu_OnPrefabInit.GameSettingExport;

namespace AnimExportTool
{
	internal class Patches
	{
		//static Texture2D BuildImageFromFrame(KAnimFile animFile, string animName = "ui", int frameIdx = 0)
		//{
		//    var go = UnityEngine.Object.Instantiate<GameObject>(EntityTemplates.unselectableEntityTemplate);
		//    var kbac = go.AddOrGet<KBatchedAnimController>();
		//    kbac.animFiles = new[]{ animFile };
		//    kbac.initialAnim = animName;
		//    kbac.Play(animName,speed:0);


		//}


		[HarmonyPatch(typeof(SubworldZoneRenderData), nameof(SubworldZoneRenderData.OnSpawn))]
		public class SubworldZoneRenderData_TargetMethod_Patch
		{
			public class ZoneColorInfo
			{
				public int Id;
				public string IdName;
				public string Name;
				public string Desc;
				public string ColorHex;
				public ZoneColorInfo(int id, string idName, Color color)
				{
					Id = id;
					IdName = idName;
					ColorHex = Util.ToHexString(color).Substring(0, 6);

					Name = Strings.Get(string.Format("STRINGS.SUBWORLDS.{0}.NAME", idName.ToUpperInvariant()));
					Desc = Strings.Get(string.Format("STRINGS.SUBWORLDS.{0}.DESC", idName.ToUpperInvariant()));
				}
			}

			public static void Postfix(SubworldZoneRenderData __instance)
			{

				Console.WriteLine("CCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
				Console.WriteLine("Biome Color Mapping:");
				Console.WriteLine();
				List<ZoneColorInfo> data = new();

				foreach (ProcGen.SubWorld.ZoneType zoneType in Enum.GetValues(typeof(ProcGen.SubWorld.ZoneType)))
				{
					data.Add(new((int)zoneType, zoneType.ToString(), __instance.zoneColours[(int)zoneType]));
				}
				Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(data));

				Console.WriteLine();
				Console.WriteLine();

			}
		}




		static Dictionary<Texture2D, Texture2D> Copies = new Dictionary<Texture2D, Texture2D>();
		public static Texture2D GetReadableCopy(Texture2D source)
		{
			if (Copies.ContainsKey(source))
				return Copies[source];

			if (source == null || source.width == 0 || source.height == 0) return null;

			RenderTexture renderTex = RenderTexture.GetTemporary(
						source.width,
						source.height,
						0,
						RenderTextureFormat.Default,
						RenderTextureReadWrite.Linear);

			Graphics.Blit(source, renderTex);
			RenderTexture previous = RenderTexture.active;
			RenderTexture.active = renderTex;
			Texture2D readableText = new Texture2D(source.width, source.height);


			readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
			readableText.Apply();
			RenderTexture.active = previous;
			RenderTexture.ReleaseTemporary(renderTex);
			Copies[source] = readableText;
			return readableText;
		}

		static Dictionary<Sprite, Texture2D> Copies2 = new Dictionary<Sprite, Texture2D>();
		static Texture2D GetSingleSpriteFromTexture(Sprite sprite, Color tint = default)
		{
			if (sprite == null || sprite.rect == null || sprite.rect.width <= 0 || sprite.rect.height <= 0)
				return null;

			bool useTint = tint != default;

			if (useTint || !Copies2.ContainsKey(sprite))
			{
				var output = new Texture2D(Mathf.RoundToInt(sprite.textureRect.width), Mathf.RoundToInt(sprite.textureRect.height));
				var r = sprite.textureRect;
				if (r.width == 0 || r.height == 0)
					return null;

				var readableTexture = GetReadableCopy(sprite.texture);

				if (readableTexture == null)
					return null;

				var pixels = readableTexture.GetPixels(Mathf.RoundToInt(r.x), Mathf.RoundToInt(r.y), Mathf.RoundToInt(r.width), Mathf.RoundToInt(r.height));
				if (useTint)
				{
					var tintedPixels = new Color[pixels.Length];
					for (int i = 0; i < pixels.Length; i++)
					{
						tintedPixels[i] = pixels[i] * tint;
					}
					//SgtLogger.l(Mathf.RoundToInt(output.width)* Mathf.RoundToInt(output.height)+" > "+tintedPixels.Length+" ?");
					output.SetPixels(tintedPixels);
				}
				else
				{
					output.SetPixels(pixels);
				}
				output.Apply();
				output.name = sprite.texture.name + " " + sprite.name;

				if (useTint)
					return output;

				Copies2.Add(sprite, output);
			}
			return Copies2[sprite];
		}
		static void WriteUISpriteToFile(Sprite sprite, string folder, string id, Color tint = default)
		{
			id = SanitationUtils.SanitizeName(id);

			Directory.CreateDirectory(folder);
			string fileName = Path.Combine(folder, id + ".png");
			var tex = GetSingleSpriteFromTexture(sprite, tint);

			if (tex == null)
				return;

			var imageBytes = tex.EncodeToPNG();
			File.WriteAllBytes(fileName, imageBytes);
		}
		[HarmonyPatch(typeof(MainMenu))]
		[HarmonyPatch(nameof(MainMenu.OnPrefabInit))]
		public static class AnimsFromWorldTraits
		{
			class SerializableWorldTrait
			{
				public string name;
				public string desc;
				public List<string> forbiddenDLCIds;

				public List<string> exclusiveWith;
				public List<string> exclusiveWithTags;
				public List<string> traitTags;

			}
			public static void GetWorldTraits()
			{
				List<SerializableWorldTrait> traitdata = new();
				var unknown = Assets.GetSprite("unknown_far");
				foreach (var traitKvp in ProcGen.SettingsCache.worldTraits)
				{
					ProcGen.WorldTrait trait = traitKvp.Value;
					string traitID = trait.filePath.Substring(trait.filePath.LastIndexOf("/") + 1);
					var UISprite = Assets.GetSprite(traitID);


					if (UISprite != null && UISprite != Assets.GetSprite("unknown") && UISprite != unknown)
					{
						WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, "WorldTraits"), traitID, Util.ColorFromHex(trait.colorHex));
						//WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, "ElementUISpritesByName"), STRINGS.UI.StripLinkFormatting(element.name), UISpriteDef.second);
					}
					traitdata.Add(new SerializableWorldTrait()
					{
						name = Strings.Get(trait.name),
						desc = Strings.Get(trait.description),
						forbiddenDLCIds = trait.forbiddenDLCIds,
						exclusiveWith = trait.exclusiveWith,
						exclusiveWithTags = trait.exclusiveWithTags,
						traitTags = trait.traitTags
					});
				}
				IO_Utils.WriteToFile(traitdata, Path.Combine(UtilMethods.ModPath, "WorldTraits", "worldtraitdata.json"));
			}
			public static void GetEggs()
			{
				foreach (var egg in Assets.GetPrefabsWithTag(GameTags.Egg))
				{
					GetAnimsFromEntity(egg);
				}
			}
			static void GetAnimsFromGeyser(GameObject geyserPrefab) =>
					GetAnimsFromEntity(geyserPrefab, "GeyserUISpritesById", "GeyserUISpritesByName");
			public static void GetGeysers()
			{
				foreach (var geyser in Assets.GetPrefabsWithTag(GameTags.GeyserFeature))
				{
					GetAnimsFromGeyser(geyser);
				}
				GetAnimsFromGeyser(Assets.GetPrefab(OilWellConfig.ID));
			}
			public static void GetAsteroids()
			{
				List<YamlIO.Error> errors = new List<YamlIO.Error>();
				ProcGen.SettingsCache.LoadFiles(errors);
				foreach (var WorldFromCache in ProcGen.SettingsCache.worlds.worldCache)
				{
					ProcGen.World world = WorldFromCache.Value;
					if ((int)world.skip >= 99)
						continue;
					GetAnimsFromAsteroid(world);
				}
			}

			public static void Postfix()
			{
				GetEggs();
				GetGeysers();
				GetWorldTraits();
				GetAsteroids();
			}
		}


		[HarmonyPatch(typeof(ElementLoader))]
		[HarmonyPatch(nameof(ElementLoader.Load))]
		public static class AnimsFromElements
		{

			public static void Postfix()
			{
				var unknown = Assets.GetSprite("unknown_far");
				foreach (var element in ElementLoader.elements)
				{
					var UISpriteDef = Def.GetUISprite(element);

					if (UISpriteDef == null)
					{
						SgtLogger.warning("element sprite for " + element.name + " not found");

						continue;
					}

					var UISprite = UISpriteDef.first;

					if (UISprite != null && UISprite != Assets.GetSprite("unknown") && UISprite != unknown)
					{
						WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, "ElementUISpritesById"), element.tag.ToString(), UISpriteDef.second);
						//WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, "ElementUISpritesByName"), STRINGS.UI.StripLinkFormatting(element.name), UISpriteDef.second);
					}

				}

			}
		}

		[HarmonyPatch(typeof(BuildingTemplates))]
		[HarmonyPatch(nameof(BuildingTemplates.CreateBuildingDef))]
		public static class AnimsFromBuildings
		{

			public static void Postfix(string id, string anim, BuildingDef __result)
			{
				var kanim = Assets.GetAnim(anim);
				if (kanim == null) return;

				var UISprite = Def.GetUISpriteFromMultiObjectAnim(kanim);

				if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
				{
					WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, "BuildingUISpritesById"), id);
					WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, "BuildingUISpritesByName"), STRINGS.UI.StripLinkFormatting(Strings.Get($"STRINGS.BUILDINGS.PREFABS.{id.ToUpperInvariant()}.NAME")));
				}
				//var defaultAnim = __result.DefaultAnimState;
				//var symbol = UIUtils.GetSymbolFromMultiObjectAnim(kanim, defaultAnim);
			}
		}
		[HarmonyPatch(typeof(EntityConfigManager))]
		[HarmonyPatch(nameof(EntityConfigManager.RegisterEntity))]
		public static class AnimsFromSingleEntity
		{
			private static readonly MethodInfo InjectBehind = AccessTools.Method(
				typeof(IEntityConfig),
				nameof(IEntityConfig.CreatePrefab)
				);

			private static readonly MethodInfo RegisterSpriteMethod = AccessTools.Method(
					typeof(AnimsFromSingleEntity),
					nameof(AnimsFromSingleEntity.RegisterSprite)
			   );
			public static GameObject RegisterSprite(GameObject instance)
			{
				GetAnimsFromEntity(instance);
				return instance;
			}

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();

				var insertionIndex = code.FindIndex(ci => ci.Calls(InjectBehind));


				if (insertionIndex != -1)
				{
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, RegisterSpriteMethod));
				}
				// Debug.Log("DEBUGMETHOD: " + new CodeInstruction(OpCodes.Call, PacketSizeHelper));

				return code;
			}
		}
		[HarmonyPatch(typeof(EntityConfigManager))]
		[HarmonyPatch(nameof(EntityConfigManager.RegisterEntities))]
		public static class AnimsFromMultiEntities
		{
			private static readonly MethodInfo InjectBehind = AccessTools.Method(
					typeof(IMultiEntityConfig),
					nameof(IMultiEntityConfig.CreatePrefabs)
			   );

			private static readonly MethodInfo RegisterSpriteMethod = AccessTools.Method(
					typeof(AnimsFromMultiEntities),
					nameof(AnimsFromMultiEntities.RegisterSprites)
			   );
			public static List<GameObject> RegisterSprites(List<GameObject> instance)
			{
				foreach (GameObject go in instance)
				{
					GetAnimsFromEntity(go);
				}
				return instance;
			}

			static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
			{
				var code = instructions.ToList();

				var insertionIndex = code.FindIndex(ci => ci.Calls(InjectBehind));


				if (insertionIndex != -1)
				{
					code.Insert(++insertionIndex, new CodeInstruction(OpCodes.Call, RegisterSpriteMethod));
				}
				// Debug.Log("DEBUGMETHOD: " + new CodeInstruction(OpCodes.Call, PacketSizeHelper));

				return code;
			}
		}
		static void GetAnimsFromEntity(GameObject instance, string idPath = "EntityUISpritesById", string namePath = "EntityUISpritesByName")
		{
			if (instance == null)
				return;

			if (!instance.TryGetComponent<KAnimControllerBase>(out var kbac) || kbac.animFiles.Length == 0)
				return;

			if (!instance.TryGetComponent<KPrefabID>(out var kPrefab))
				return;

			var UISpriteDef = Def.GetUISprite(instance);
			if (UISpriteDef == null)
				return;
			var UISprite = UISpriteDef.first;

			var prefabId = kPrefab.PrefabID();
			if (prefabId == null)
				return;
			var id = prefabId.ToString();

			if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
			{
				WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, idPath), id);
				WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, namePath), TagManager.GetProperName(id, true));
			}
		}
		static void GetAnimsFromAsteroid(ProcGen.World world, string idPath = "AsteroidUISpritesById", string namePath = "AsteroiUISpritesByName")
		{
			Sprite UISprite = ColonyDestinationAsteroidBeltData.GetUISprite(world.asteroidIcon);

			var id = Path.GetFileName(world.filePath).ToString();

			if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
			{
				WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, idPath), id);
				WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, namePath), Strings.Get(world.name));
			}
		}

		/////gather basegame care packages:


		//[HarmonyPatch(typeof(CarePackageInfo),MethodType.Constructor, new Type[] { typeof( string), typeof(float ), typeof(Func<bool> )})]
		//public class CarePackageConfig_TargetMethod_Patch
		//{
		//    public static void Postfix(string ID, float amount, Func<bool> requirement)
		//    {
		//        var list =   requirement.GetInvocationList();
		//        SgtLogger.l(ID + " x" + amount, "CAREPACKAGEGRAB");              
		//        foreach (var _delegate in list)
		//        {
		//            SgtLogger.l(_delegate.Method.Name);
		//        }
		//    }
		//}
		//[HarmonyPatch(typeof(CarePackageInfo), MethodType.Constructor, new Type[] { typeof(string), typeof(float), typeof(Func<bool>), typeof(string) })]
		//public class CarePackageConfig_TargetMethod_Patch2
		//{            
		//    public static void Postfix(string ID, float amount, Func<bool> requirement)
		//    {
		//        var list = requirement.GetInvocationList();
		//        SgtLogger.l(ID + " x" + amount, "CAREPACKAGEGRAB");
		//        foreach (var _delegate in list)
		//        {
		//            if(_delegate.Method!=null)
		//                SgtLogger.l(_delegate.Method.Name);
		//        }
		//    }
		//}






		public static StringBuilder EntityIdBuilder
		{
			get
			{
				if (EntityIdBuilder == null)
				{
					_entityIdBuilder = new StringBuilder();
					_entityIdBuilder.Append("| Name | Id | DLC |");
					_entityIdBuilder.Append("| :--: | :--:| :--:|");
				}
				return EntityIdBuilder;
			}

		}
		private static StringBuilder _entityIdBuilder = null;
		public static string StripFormatting(string toStrip)
		{
			toStrip = STRINGS.UI.StripLinkFormatting(toStrip);
			toStrip = toStrip.Replace(STRINGS.UI.PRE_KEYWORD, string.Empty);
			toStrip = toStrip.Replace(STRINGS.UI.PST_KEYWORD, string.Empty);
			toStrip = toStrip.Replace(STRINGS.UI.PRE_POS_MODIFIER, string.Empty);
			toStrip = toStrip.Replace(STRINGS.UI.PST_POS_MODIFIER, string.Empty);
			toStrip = toStrip.Replace("<i>", string.Empty);
			toStrip = toStrip.Replace("</i>", string.Empty);
			toStrip = toStrip.Replace("<sup>", string.Empty);
			toStrip = toStrip.Replace("</sup>", string.Empty);
			toStrip = toStrip.Replace("<smallcaps>", string.Empty);
			toStrip = toStrip.Replace("</smallcaps>", string.Empty);
			return toStrip;
		}

		static List<Tuple<string, string, string[]>> EntitiesByName = new();

		static void RegisterNames(GameObject go)
		{
			var prefab = go.GetComponent<KPrefabID>();
			string[] requiredDlcIds = prefab.requiredDlcIds;
			if (requiredDlcIds == null)
				requiredDlcIds = [];

			EntitiesByName.Add(new(StripFormatting(prefab.GetProperName()), prefab.PrefabTag.ToString(), requiredDlcIds));
		}

		//[HarmonyPatch(typeof(IEntityConfig), nameof(IEntityConfig.CreatePrefab))]
		//public class IEntityConfig_CreatePrefab_Patch
		//{
		//	public static void Postfix(GameObject __result)
		//	{
		//		RegisterNames(__result);
		//	}
		//}

		//[HarmonyPatch(typeof(IMultiEntityConfig), nameof(IMultiEntityConfig.CreatePrefabs))]
		//public class IMultiEntityConfig_CreatePrefabs_Patch
		//{
		//	public static void Postfix(List<GameObject> __result)
		//	{
		//		foreach(var en in __result)
		//			RegisterNames(en);
		//	}
		//}

		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnPrefabInit))]
		public class MainMenu_OnPrefabInit
		{
			public class StarmapGeneratorData
			{
				public Dictionary<string, VanillaStarmapLocation> Locations = new();
				public Dictionary<string, ElementData> Elements = new();
			}
			public class ElementData
			{
				public ElementData(string id, string name)
				{
					Id = id;
					Name = name;
				}
				public string Id;
				public string Name;
			}
			public class VanillaStarmapLocation
			{
				public string Id;
				public string Name;
				public string Description;
				public string Image;
				public Dictionary<string, float> Ressources_Elements;
				public Dictionary<string, int> Ressources_Entities;
			}
			public class SpacedOutStarmapLocation
			{
				public string Id;
				public string Name;
				public string Description;
				public string Image;
				public Dictionary<string, float> Ressources_Elements;
			}

			public class GameSettingExport
			{
				public class SettingLevel
				{
					public string Id;
					public string Name;
					public string Description;
					public long coordinate_value;
				}

				public string Id;
				public string Name;
				public string Description;
				public long coordinate_range;
				public string DlcIdFrom;
				public string Icon;
				public List<SettingLevel> Levels;
				public string[] MixingTags;
				public string[] ForbiddenClusterTags;
				public string WorldMixing;
				public string SubworldMixing;


				public MixingType SettingType = MixingType.None;
				//public MixingData

				public enum MixingType
				{
					None = -1,
					DLC = 0,
					Subworld = 1,
					World = 2,
				}
			}


			public class Asteroid
			{
				public string Id;
				public string Name;
				public string Image;
				public bool DisableWorldTraits = false;
				public List<ProcGen.World.TraitRule> TraitRules;
				public float worldTraitScale;
				public List<string> worldTags;
				public List<string> SpecialPOIs;
			}
			public class ClusterLayout
			{
				public string Id;
				public string Name;
				public string Prefix;
				public int menuOrder;
				public int startWorldIndex;
				public int numRings;
				public List<string> RequiredDlcsIDs;
				public List<string> ForbiddenDlcIDs;
				public List<ProcGen.WorldPlacement> worldPlacements;
				public List<ProcGen.SpaceMapPOIPlacement> poiPlacements;
				public int clusterCategory;
				public bool disableStoryTraits;
				public int fixedCoordinate;
				public string[] clusterTags;
			}
			public class DataExport
			{
				public List<ClusterLayout> clusters = new();
				public List<Asteroid> asteroids = new();
				public List<WorldTrait> worldTraits = new();
			}
			public class WorldTrait
			{
				public string Id;
				public string Name, Description, ColorHex;
				public List<string> forbiddenDLCIds, exclusiveWith, exclusiveWithTags, traitTags;
				public Dictionary<string, int> globalFeatureMods { get; set; }

				public WorldTrait()
				{
					exclusiveWith = new List<string>();
					exclusiveWithTags = new List<string>();
					forbiddenDLCIds = new List<string>();
					traitTags = new List<string>();
					Name = string.Empty;
					Id = string.Empty;
				}
			}

			public static string GenerateLocalizedEntry(string key, string value)
			{
				return $"<data name=\"{key}\" xml:space=\"preserve\"><value>{value}</value></data>";
			}

			static void GetAnimsFromRecoverable(GameObject geyserPrefab) =>
					GetAnimsFromEntity(geyserPrefab, "StarmapDestinationRecoverablesById", "StarmapDestinationRecoverablesByName");
			static void GetAnimsFromStarmapLocation(string spriteName, string fileName, string idPath = "StarmapDestinationsById")
			{
				Sprite UISprite = Assets.GetSprite(spriteName);

				var id = Path.GetFileName(fileName).ToString();

				if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
				{
					WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, idPath), id);
				}
			}


			//static SpacedOutStarmapLocation GetPoiData(GameObject item)
			//{
			//	if (item.TryGetComponent<ClusterGridEntity>(out var component1))
			//	{
			//		SpacedOutStarmapLocation data = new SpacedOutStarmapLocation();

			//		data.Id = component1.PrefabID().ToString();

			//		var animName = component1.AnimConfigs.First().initialAnim;
			//		var animFile = component1.AnimConfigs.First().animFile;

			//		if (data.Id.Contains(HarvestablePOIConfig.CarbonAsteroidField)) ///carbon field fix
			//			animName = "carbon_asteroid_field";

			//		if (animName == "closed_loop")///Temporal tear
			//			animName = "ui";

			//		data.Sprite = Def.GetUISpriteFromMultiObjectAnim(animFile, animName, true);

			//		if (item.TryGetComponent<HarvestablePOIConfigurator>(out var harvest))
			//		{
			//			var HarvestableConfig = HarvestablePOIConfigurator.FindType(harvest.presetType);
			//			data.Mineables = new(HarvestableConfig.harvestableElements);
			//			data.CapacityMin = HarvestableConfig.poiCapacityMin;
			//			data.CapacityMax = HarvestableConfig.poiCapacityMax;
			//			data.RechargeMin = HarvestableConfig.poiRechargeMin;
			//			data.RechargeMax = HarvestableConfig.poiRechargeMax;
			//		}
			//		if (item.TryGetComponent<ArtifactPOIConfigurator>(out _))
			//		{
			//			data.HasArtifacts = true;
			//		}

			//		if (item.TryGetComponent<InfoDescription>(out var descHolder))
			//		{
			//			data.Description = descHolder.description;
			//			data.Name = component1.Name;
			//		}
			//		if (component1 is ArtifactPOIClusterGridEntity && data.Name == null)
			//		{
			//			string artifact_ID = component1.PrefabID().ToString().Replace("ArtifactSpacePOI_", string.Empty);
			//			data.Name = global::Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + artifact_ID.ToUpper() + ".NAME"));
			//			data.Description = global::Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.ARTIFACT_POI." + artifact_ID.ToUpper() + ".DESC"));
			//		}
			//		if (component1 is TemporalTear && data.Name == null)
			//		{
			//			data.Name = global::Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.NAME"));
			//			data.Description = global::Strings.Get(new StringKey("STRINGS.UI.SPACEDESTINATIONS.WORMHOLE.DESCRIPTION"));
			//		}

			//		return data;
			//	}
			//	return null;
			//}
			//static List<SpacedOutStarmapLocation> GrabSpacedOutStarmapLocations()
			//{
			//	foreach (var item in Assets.GetPrefabsWithComponent<HarvestablePOIClusterGridEntity>())
			//	{
			//		var data = GetPoiData(item);
			//		if (data != null && !_so_POIs.ContainsKey(data.Id))
			//		{
			//			_so_POIs.Add(data.Id, data);
			//			_nonUniquePOI_Ids.Add(data.Id);
			//		}
			//		if (data != null && !_so_POI_IDs.Contains(data.Id))
			//			_so_POI_IDs.Add(data.Id);

			//	}
			//	foreach (var item in Assets.GetPrefabsWithComponent<ArtifactPOIClusterGridEntity>())
			//	{
			//		var data = GetPoiData(item);
			//		if (data != null && !_so_POIs.ContainsKey(data.Id))
			//		{
			//			_so_POIs.Add(data.Id, data);
			//			if (data.Id != TeapotId)
			//				_nonUniquePOI_Ids.Add(data.Id);
			//		}
			//		if (data != null && !_so_POI_IDs.Contains(data.Id))
			//			_so_POI_IDs.Add(data.Id);
			//	}
			//	foreach (var item in Assets.GetPrefabsWithComponent<TemporalTear>())
			//	{
			//		var data = GetPoiData(item);
			//		if (data != null && !_so_POIs.ContainsKey(data.Id))
			//		{
			//			_so_POIs.Add(data.Id, data);
			//		}
			//		if (data != null && !_so_POI_IDs.Contains(data.Id))
			//			_so_POI_IDs.Add(data.Id);
			//	}
			//}

			static List<string> AccumulatePOIdata(ProcGen.World asteroid)
			{
				var list = new List<string>();
				if (asteroid.worldTemplateRules.Any(rules => rules.names.Any(rule => rule.Contains("warp/receiver") || rule.Contains("warp/sender"))))
					list.Add("Teleporter");
				if (asteroid.seasons.Contains("LargeImpactor"))
					list.Add("LargeImpactor");
				if (asteroid.worldTemplateRules.Any(rules => rules.names.Any(rule => rule.Contains("geothermal_controller"))))
					list.Add("GeothermalController");
				return list;
			}
			public class DynamicContractResolver : DefaultContractResolver
			{

				private HashSet<Type> _typesToIgnore;
				public DynamicContractResolver(Type[] typesToIgnore)
				{
					_typesToIgnore = typesToIgnore.ToHashSet();
				}

				protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
				{
					IList<JsonProperty> properties = base.CreateProperties(type, memberSerialization);

					properties = properties.Where(p => !_typesToIgnore.Contains( p.PropertyType)) .ToList();

					return properties;
				}
			}


			public static void Postfix()
			{
				StringBuilder loc = new StringBuilder();
				var export = new DataExport();
				SgtLogger.l("Fetching Clusters");
				foreach (var cluster in ProcGen.SettingsCache.clusterLayouts.clusterCache.Values)
				{

					bool strippedMoonlet = false;
					string clusterName = Strings.Get(cluster.name);
					{
						if (clusterName.Contains("Moonlet Cluster - ") || clusterName.Contains("Mini Cluster - "))
						{
							strippedMoonlet = true;
							clusterName = clusterName.ToString().Replace("Moonlet Cluster - ", string.Empty).Replace("Mini Cluster - ", string.Empty);
							clusterName += " Cluster";
						}
					}

					var data = new ClusterLayout();
					data.Id = cluster.filePath;
					data.Name = StripFormatting(clusterName);
					data.Prefix = cluster.coordinatePrefix;
					data.menuOrder = cluster.menuOrder;
					data.RequiredDlcsIDs = cluster.requiredDlcIds?.ToList();
					data.ForbiddenDlcIDs = cluster.forbiddenDlcIds?.ToList();
					data.startWorldIndex = cluster.startWorldIndex;
					data.worldPlacements = [.. cluster.worldPlacements];
					data.clusterCategory = (int)cluster.clusterCategory;
					data.fixedCoordinate = cluster.fixedCoordinate;
					data.clusterTags = cluster.clusterTags?.ToArray();
					data.startWorldIndex = cluster.startWorldIndex;
					data.menuOrder = cluster.menuOrder;
					data.numRings = cluster.numRings;
					if (cluster.poiPlacements != null)
						data.poiPlacements = [.. cluster.poiPlacements];
					else
						data.poiPlacements = new List<ProcGen.SpaceMapPOIPlacement>();
					data.disableStoryTraits = cluster.disableStoryTraits;


					export.clusters.Add(data);
					loc.Append(GenerateLocalizedEntry(data.Name, data.Name));
				}
				SgtLogger.l("Fetching Asteroids");
				foreach (var world in ProcGen.SettingsCache.worlds.worldCache.Values)
				{
					var data = new Asteroid();
					data.Id = world.filePath;
					data.Name = StripFormatting(Strings.Get(world.name));
					data.DisableWorldTraits = world.disableWorldTraits;
					data.TraitRules = world.worldTraitRules;
					data.worldTraitScale = world.worldTraitScale;
					if (world.worldTags == null)
						data.worldTags = new List<string>();
					else
						data.worldTags = [.. world.worldTags];
					data.SpecialPOIs = AccumulatePOIdata(world);
					export.asteroids.Add(data);
					loc.Append(GenerateLocalizedEntry(data.Name, data.Name));
				}
				SgtLogger.l("Fetching World Traits");
				foreach (var trait in ProcGen.SettingsCache.worldTraits.Values)
				{
					var data = new WorldTrait();
					data.Id = trait.filePath;
					data.Name = StripFormatting(Strings.Get(trait.name));
					data.Description = StripFormatting(Strings.Get(trait.description));
					data.ColorHex = trait.colorHex;
					data.forbiddenDLCIds = trait.forbiddenDLCIds;
					data.exclusiveWith = trait.exclusiveWith;
					data.exclusiveWithTags = trait.exclusiveWithTags;
					data.traitTags = trait.traitTags;
					data.globalFeatureMods = trait.globalFeatureMods;

					export.worldTraits.Add(data);
					loc.Append(GenerateLocalizedEntry(data.Name, data.Name));
				}

				var entityConfigInterface = typeof(IEntityConfig);
				var multintityConfigInterface = typeof(IMultiEntityConfig);
				var hasDlcRestrictionsInterface = typeof(IHasDlcRestrictions);
				var types = AppDomain.CurrentDomain.GetAssemblies()
					.SelectMany(s => s.GetTypes())
					.Where(p => (entityConfigInterface.IsAssignableFrom(p) || multintityConfigInterface.IsAssignableFrom(p)) && p.IsClass);


				Console.WriteLine("ENTITIES:");
				//foreach(var en in EntitiesByName)
				//{
				//	EntityIdBuilder.Append($"| {en.first} | {en.second}| {string.Join(", ", en.third)}|");
				//}

				//Console.WriteLine(EntityIdBuilder.ToString());
				WorldgenMixing

				Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
				Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(export,new JsonSerializerSettings { ContractResolver = new DynamicContractResolver([typeof(Vector2I)]) }));
				Console.WriteLine("LOC:");
				Console.WriteLine(loc.ToString());
				var starmapExport = new StarmapGeneratorData();

				foreach (var location in Db.Get().SpaceDestinationTypes.resources)
				{
					var locationData = new VanillaStarmapLocation()
					{
						Id = location.Id,
						Name = StripFormatting(location.Name),
						Description = StripFormatting(location.description),
						Image = location.spriteName,
					};
					if (location.elementTable != null)
					{
						locationData.Ressources_Elements = location.elementTable.ToDictionary(kvp => kvp.Key.ToString(), kvp => kvp.Value.min);
						foreach (var elementK in location.elementTable)
						{
							var element = ElementLoader.GetElement(elementK.Key.CreateTag());
							starmapExport.Elements[element.id.ToString()] = new(element.id.ToString(), StripFormatting(element.name));
						}
					}

					if (location.recoverableEntities != null)
					{
						locationData.Ressources_Entities = new(location.recoverableEntities);

						foreach (var entity in location.recoverableEntities)
						{
							var prefab = Assets.GetPrefab(entity.Key);
							if (prefab != null)
							{
								GetAnimsFromRecoverable(prefab);
								starmapExport.Elements[entity.Key] = new(entity.Key.ToString(), StripFormatting(prefab.GetProperName()));
							}
							else
								SgtLogger.warning(entity.Key + " not found!");


						}
					}
					starmapExport.Locations.Add(location.Id, locationData);
					GetAnimsFromStarmapLocation(location.spriteName, location.Id);
				}

				Console.WriteLine("BBBBBBBBBBBBBBBBBB");
				Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(starmapExport));

				var mixingList = new List<GameSettingExport>();

				foreach (var coordinatedMixingSetting in CustomGameSettings.Instance.CoordinatedMixingSettings)
				{
					SettingConfig mixingSetting = CustomGameSettings.Instance.MixingSettings[coordinatedMixingSetting];
					MixingType mixingType = MixingType.DLC;

					var setting = new GameSettingExport()
					{
						Id = mixingSetting.id,
						Name = StripFormatting(mixingSetting.label),
						Description = StripFormatting(mixingSetting.tooltip),
						coordinate_range = mixingSetting.coordinate_range,
						DlcIdFrom = mixingSetting.GetRequiredDlcIds().FirstOrDefault(),
						Levels = mixingSetting.GetLevels().Select(l => new GameSettingExport.SettingLevel() { Id = l.id, Name = StripFormatting(l.label), Description = StripFormatting(l.tooltip), coordinate_value = l.coordinate_value }).ToList(),

					};
					if (mixingSetting is SubworldMixingSettingConfig subworldMixing)
					{
						setting.ForbiddenClusterTags = subworldMixing.forbiddenClusterTags?.ToArray();
						setting.Icon = subworldMixing.icon.name;

						mixingType = MixingType.Subworld;
						ProcGen.SubworldMixingSettings worldgenData = ProcGen.SettingsCache.GetCachedSubworldMixingSetting(subworldMixing.worldgenPath);
						setting.SubworldMixing = worldgenData?.subworld?.name;
					}
					else if (mixingSetting is WorldMixingSettingConfig worldMixing)
					{
						setting.ForbiddenClusterTags = worldMixing.forbiddenClusterTags?.ToArray();
						setting.Icon = worldMixing.icon.name;
						ProcGen.WorldMixingSettings worldgenData = ProcGen.SettingsCache.GetCachedWorldMixingSetting(worldMixing.worldgenPath);
						mixingType = MixingType.World;
						setting.WorldMixing = worldgenData?.world;
					}
					setting.SettingType = mixingType;

					mixingList.Add(setting);

				}
				Console.WriteLine("CCCCCCCCCCCCCCCCCCC");
				Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(mixingList));

			}
		}

	}
}

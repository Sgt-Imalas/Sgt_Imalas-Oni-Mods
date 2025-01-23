using HarmonyLib;
using Klei;
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
				//TranspilerHelper.PrintInstructions(code);
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
				//TranspilerHelper.PrintInstructions(code);
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


		[HarmonyPatch(typeof(MainMenu), nameof(MainMenu.OnPrefabInit))]
		public class MainMenu_OnPrefabInit
		{
			public class StarmapGeneratorData
			{
				public Dictionary<string, VanillaStarmapLocation> Locations = new();
				public Dictionary<string, ElementData> Elements= new();
			}
			public class ElementData
			{
				public ElementData(string id,string name)
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


			public class Asteroid
			{
				public string Id;
				public string Name;
				public string Image;
				public bool DisableWorldTraits = false;
				public List<ProcGen.World.TraitRule> TraitRules;
				public float worldTraitScale;
			}
			public class ClusterLayout
			{
				public string Id;
				public string Name;
				public string Prefix;
				public int menuOrder;
				public int startWorldIndex;
				public string[] RequiredDlcsIDs;
				public string[] ForbiddenDlcIDs;
				public List<string> WorldPlacementIDs;
				public int clusterCategory;
				public int fixedCoordinate;
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
			public static string StripFormatting(string toStrip)
			{
				toStrip = STRINGS.UI.StripLinkFormatting(toStrip);
				toStrip = toStrip.Replace(STRINGS.UI.PRE_KEYWORD, string.Empty);
				toStrip = toStrip.Replace(STRINGS.UI.PST_KEYWORD, string.Empty);
				return toStrip;
			}
            public static void Postfix()
			{
				StringBuilder loc = new StringBuilder();
				var export = new DataExport();
				foreach (var cluster in ProcGen.SettingsCache.clusterLayouts.clusterCache.Values)
				{

					bool strippedMoonlet = false;
					string clusterName = Strings.Get(cluster.name);
					{
						if(clusterName.Contains("Moonlet Cluster - ") || clusterName.Contains("Mini Cluster - "))
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
					data.RequiredDlcsIDs = cluster.requiredDlcIds;
					data.ForbiddenDlcIDs = cluster.forbiddenDlcIds;
					//data.WorldPlacements = cluster.worldPlacements;
					data.startWorldIndex = cluster.startWorldIndex;

					data.WorldPlacementIDs = cluster.worldPlacements.Select(pl => pl.world).ToList();
					data.clusterCategory = (int)cluster.clusterCategory;
					data.fixedCoordinate = cluster.fixedCoordinate;
					export.clusters.Add(data);
					loc.Append(GenerateLocalizedEntry(data.Name, data.Name));
                }
				foreach (var world in ProcGen.SettingsCache.worlds.worldCache.Values)
				{
					var data = new Asteroid();
					data.Id = world.filePath;
					data.Name = StripFormatting(Strings.Get(world.name));
					data.DisableWorldTraits = world.disableWorldTraits;
					data.TraitRules = world.worldTraitRules;
					data.worldTraitScale = world.worldTraitScale;

					export.asteroids.Add(data);
                    loc.Append(GenerateLocalizedEntry(data.Name, data.Name));
                }
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
				Console.WriteLine("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
				Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(export));
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
						foreach(var  elementK in location.elementTable)
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
					starmapExport.Locations.Add(location.Id,locationData);
					GetAnimsFromStarmapLocation(location.spriteName,location.Id);
				}
				
				Console.WriteLine("BBBBBBBBBBBBBBBBBB");
				Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(starmapExport));
			}
		}

	}
}

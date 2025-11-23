using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UtilLibs
{
	/// <summary>
	/// Credit: Aki
	/// </summary>
	public class AssetUtils
	{
		//
		//works both pre and postfix
		public static Sprite AddSpriteToAssets(FileInfo file, Assets instance = null, bool overrideExisting = false)
		{
			if (instance == null)
			{
				instance = Assets.instance;
			}
			string spriteId = Path.GetFileNameWithoutExtension(file.Name);
			TryLoadTexture(file.FullName, out Texture2D texture);
			var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
			sprite.name = spriteId;
			if (!overrideExisting && instance.SpriteAssets.Any(spritef => spritef != null && spritef.name == spriteId))
			{
				SgtLogger.l("Sprite " + spriteId + " was already existent in the sprite assets");
				return null;
			}
			if (overrideExisting)
				instance.SpriteAssets.RemoveAll(foundsprite2 => foundsprite2 != null && foundsprite2.name == spriteId);
			instance.SpriteAssets.Add(sprite);

			HashedString key = new HashedString(sprite.name);

			if(Assets.Sprites != null)
				Assets.Sprites[key] = sprite;

			return sprite;
		}

		//use in prefix
		public static Sprite AddSpriteToAssets(Assets instance, string spriteid, bool overrideExisting = false, TextureWrapMode mode = TextureWrapMode.Repeat)
		{
			var path = Path.Combine(UtilMethods.ModPath, "assets");
			var texture = LoadTexture(spriteid, path);
			texture.wrapMode = mode;
			var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
			sprite.name = spriteid;
			if (!overrideExisting && instance.SpriteAssets.Any(spritef => spritef != null && spritef.name == spriteid))
			{
				SgtLogger.l("Sprite " + spriteid + " was already existent in the sprite assets");
				return null;
			}
			if (overrideExisting)
				instance.SpriteAssets.RemoveAll(foundsprite2 => foundsprite2 != null && foundsprite2.name == spriteid);

			instance.SpriteAssets.Add(sprite);
			return sprite;
		}
		public static void OverrideSpriteTextures(Assets instance, FileInfo file)
		{
			string spriteId = Path.GetFileNameWithoutExtension(file.Name);
			var texture = AssetUtils.LoadTexture(file.FullName);

			if (instance.TextureAssets?.Any(foundsprite => foundsprite != null && foundsprite.name == spriteId)??false)
			{
				SgtLogger.l("removed existing TextureAsset: " + spriteId);
				instance.TextureAssets.RemoveAll(foundsprite2 => foundsprite2 != null && foundsprite2.name == spriteId);
			}
			instance.TextureAssets?.Add(texture);
			if (Assets.Textures?.Any(foundsprite => foundsprite != null && foundsprite.name == spriteId) ?? false)
			{
				SgtLogger.l("removed existing Texture: " + spriteId);
				Assets.Textures?.RemoveAll(foundsprite2 => foundsprite2 != null && foundsprite2.name == spriteId);
			}
			Assets.Textures?.Add(texture);

			if (instance.TextureAtlasAssets?.Any(TextureAtlas => TextureAtlas != null && TextureAtlas.texture != null && TextureAtlas.texture.name == spriteId) ?? false)
			{
				SgtLogger.l("replaced Texture Atlas Asset texture: " + spriteId);
				var atlasInQuestion = instance.TextureAtlasAssets.First(TextureAtlas => TextureAtlas != null && TextureAtlas.texture != null && TextureAtlas.texture.name == spriteId);
				if (atlasInQuestion != null)
				{
					atlasInQuestion.texture = texture;
				}
			}


			if (Assets.TextureAtlases?.Any(TextureAtlas => TextureAtlas != null && TextureAtlas.texture != null && TextureAtlas.texture.name == spriteId) ?? false)
			{
				var atlasInQuestion = Assets.TextureAtlases.First(TextureAtlas => TextureAtlas != null && TextureAtlas.texture != null && TextureAtlas.texture.name == spriteId);
				if (atlasInQuestion != null)
				{
					atlasInQuestion.texture = texture;
				}
			}

			var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
			sprite.name = spriteId;

			if (instance.SpriteAssets?.Any(foundsprite => foundsprite != null && foundsprite.name == spriteId) ?? false)
			{
				SgtLogger.l("removed existing SpriteAsset" + spriteId);
				instance.SpriteAssets.RemoveAll(foundsprite2 => foundsprite2 != null && foundsprite2.name == spriteId);
			}
			instance.SpriteAssets?.Add(sprite);

			if (Assets.Sprites?.ContainsKey(spriteId)??false)
			{
				SgtLogger.l("removed existing Sprite" + spriteId);
				Assets.Sprites.Remove(spriteId);
			}
			if (Assets.TintedSprites?.Any(foundsprite => foundsprite != null && foundsprite.name == spriteId) ?? false)
			{
				Assets.TintedSprites.First(foundsprite => foundsprite != null && foundsprite.name == spriteId).sprite = sprite;
			}
			Assets.Sprites?.Add(spriteId, sprite);

		}
		public static bool TryLoadTexture(string path, out Texture2D texture)
		{
			texture = LoadTexture(path, true);
			return texture != null;
		}
		public static Texture2D LoadTexture(string name, string directory)
		{
			if (directory == null)
			{
				directory = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets");
			}

			string path = Path.Combine(directory, name + ".png");

			return LoadTexture(path);
		}
		public static Texture2D LoadTexture(string path, bool warnIfFailed = true, int customTextureWidth = 1, int customTextureHeight = 1)
		{
			Texture2D texture = null;

			if (File.Exists(path))
			{
				byte[] data = TryReadFile(path);
				texture = new Texture2D(customTextureWidth, customTextureHeight);
				texture.LoadImage(data);
			}
			else if (warnIfFailed)
			{
				SgtLogger.logwarning($"Could not load texture at path {path}.", "SgtImalasUtils");
			}

			return texture;
		}
		public static byte[] TryReadFile(string texFile)
		{
			try
			{
				return File.ReadAllBytes(texFile);
			}
			catch (Exception e)
			{
				SgtLogger.logwarning("Could not read file: " + e, "SgtImalasUtils");
				return null;
			}
		}

		public static string baseAtlasFolder = Path.Combine("assets", "customatlastiles");
		public static void AddCustomTileTops(BuildingDef def, string name, bool shiny = false, string decorInfo = "tiles_glass_tops_decor_info", string existingPlaceID = null, string existingSpecID = null)
		{
			var info = UnityEngine.Object.Instantiate(global::Assets.GetBlockTileDecorInfo(decorInfo));

			string topsName = name.Contains("_tops") ? name : $"{name}_tiles_tops";
			// base
			if (info is object)
			{
				info.atlas = GetCustomAtlas(topsName, baseAtlasFolder, info.atlas);
				def.DecorBlockTileInfo = info;
			}

			// placement
			if (existingPlaceID.IsNullOrWhiteSpace())
			{
				var placeInfo = UnityEngine.Object.Instantiate(global::Assets.GetBlockTileDecorInfo(decorInfo));
				placeInfo.atlas = GetCustomAtlas(topsName+"_place", baseAtlasFolder, placeInfo.atlas);
				def.DecorPlaceBlockTileInfo = placeInfo;
			}
			else
			{
				def.DecorPlaceBlockTileInfo = global::Assets.GetBlockTileDecorInfo(existingPlaceID);
			}

			// specular
			if (shiny)
			{
				string id = existingSpecID.IsNullOrWhiteSpace() ? $"{topsName}_spec" : existingSpecID;
				info.atlasSpec = GetCustomAtlas(id, baseAtlasFolder, info.atlasSpec);
			}
		}
		public static void AddCustomTileAtlas(BuildingDef def, string name, bool shiny = false, string referenceAtlas = "tiles_metal")
		{
			TextureAtlas reference = global::Assets.GetTextureAtlas(referenceAtlas);

			string atlasName = name.Contains("tiles") ? name : $"{name}_tiles";

			// base
			def.BlockTileAtlas = GetCustomAtlas(atlasName, baseAtlasFolder, reference);

			// place
			def.BlockTilePlaceAtlas = GetCustomAtlas($"{atlasName}_place", baseAtlasFolder, reference);

			// specular
			if (shiny)
			{
				def.BlockTileShineAtlas = GetCustomAtlas($"{atlasName}_spec", baseAtlasFolder, reference);
			}
		}
		public static TextureAtlas GetCustomAtlas(string fileName, string folder, TextureAtlas tileAtlas)
		{
			string path = UtilMethods.ModPath;

			if (folder != null)
			{
				path = Path.Combine(path, folder);
			}

			var tex = LoadTexture(fileName, path);

			if (tex == null)
			{
				SgtLogger.error("could not load custom tile atlas texture: " + fileName + " in folder: " + path);	
				return null;
			}

			TextureAtlas atlas;
			atlas = ScriptableObject.CreateInstance<TextureAtlas>();
			atlas.texture = tex;
			atlas.scaleFactor = tileAtlas.scaleFactor;
			atlas.items = tileAtlas.items;

			return atlas;
		}
		public static AssetBundle LoadAssetBundle(string assetBundleName, string path = null, bool platformSpecific = false)
		{
			foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
			{
				if (bundle.name == assetBundleName)
				{
					return bundle;
				}
			}

			if (path.IsNullOrWhiteSpace())
			{
				path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "assets");
			}

			if (platformSpecific)
			{
				switch (Application.platform)
				{
					case RuntimePlatform.WindowsPlayer:
						path = Path.Combine(path, "windows");
						break;
					case RuntimePlatform.LinuxPlayer:
						path = Path.Combine(path, "linux");
						break;
					case RuntimePlatform.OSXPlayer:
						path = Path.Combine(path, "mac");
						break;
				}
			}

			path = Path.Combine(path, assetBundleName);

			var assetBundle = AssetBundle.LoadFromFile(path);

			if (assetBundle == null)
			{
				SgtLogger.warning($"Failed to load AssetBundle from path {path}");
				return null;
			}

			return assetBundle;
		}
	}
}

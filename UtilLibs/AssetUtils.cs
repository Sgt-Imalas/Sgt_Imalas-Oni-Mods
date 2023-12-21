using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Core.Tokens;

namespace UtilLibs
{
    /// <summary>
    /// Credit: Aki
    /// </summary>
    public class AssetUtils
    {
        public static void AddSpriteToAssets(Assets instance, string spriteid, bool overrideExisting = false)
        {
            var path = Path.Combine(UtilMethods.ModPath, "assets");
            var texture = LoadTexture(spriteid, path);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
            sprite.name = spriteid;
            if (!overrideExisting && instance.SpriteAssets.Any(spritef => spritef != null && spritef.name == spriteid))
            {
                SgtLogger.l("Sprite " + spriteid + " was already existent in the sprite assets");
                return;
            }
            if (overrideExisting)
                instance.SpriteAssets.RemoveAll(foundsprite2 => foundsprite2 != null && foundsprite2.name == spriteid);            

            instance.SpriteAssets.Add(sprite);
        }
        public static void OverrideSpriteTextures(Assets instance, FileInfo file)
        {
            string spriteId = Path.GetFileNameWithoutExtension(file.Name);
            var texture = AssetUtils.LoadTexture(file.FullName);

            if (instance.TextureAssets.Any(foundsprite => foundsprite != null && foundsprite.name == spriteId))
            {
                SgtLogger.l("removed existing TextureAsset: " + spriteId);
                instance.TextureAssets.RemoveAll(foundsprite2 => foundsprite2 != null && foundsprite2.name == spriteId);
            }
            instance.TextureAssets.Add(texture);
            if (Assets.Textures.Any(foundsprite => foundsprite.name == spriteId))
            {
                SgtLogger.l("removed existing Texture: " + spriteId);
                Assets.Textures.RemoveAll(foundsprite2 => foundsprite2 != null && foundsprite2.name == spriteId);
            }
            Assets.Textures.Add(texture);

            if (instance.TextureAtlasAssets.Any(TextureAtlas => TextureAtlas != null && TextureAtlas.texture != null && TextureAtlas.texture.name == spriteId))
            {
                SgtLogger.l("replaced Texture Atlas Asset texture: " + spriteId);
                var atlasInQuestion = instance.TextureAtlasAssets.First(TextureAtlas => TextureAtlas != null && TextureAtlas.texture != null && TextureAtlas.texture.name == spriteId);
                if (atlasInQuestion != null)
                {
                    atlasInQuestion.texture = texture;
                }
            }


            if (Assets.TextureAtlases.Any(TextureAtlas => TextureAtlas != null && TextureAtlas.texture != null && TextureAtlas.texture.name == spriteId))
            {
                var atlasInQuestion = Assets.TextureAtlases.First(TextureAtlas => TextureAtlas != null && TextureAtlas.texture != null && TextureAtlas.texture.name == spriteId);
                if (atlasInQuestion != null)
                {
                    atlasInQuestion.texture = texture;
                }

            }

            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
            sprite.name = spriteId;



            if (instance.SpriteAssets.Any(foundsprite => foundsprite.name == spriteId))
            {
                SgtLogger.l("removed existing SpriteAsset" + spriteId);
                instance.SpriteAssets.RemoveAll(foundsprite2 => foundsprite2.name == spriteId);
            }
            instance.SpriteAssets.Add(sprite);

            if (Assets.Sprites.ContainsKey(spriteId))
            {
                SgtLogger.l("removed existing Sprite" + spriteId);
                Assets.Sprites.Remove(spriteId);
            }
            if (Assets.TintedSprites.Any(foundsprite => foundsprite.name == spriteId))
            {
                Assets.TintedSprites.First(foundsprite => foundsprite.name == spriteId).sprite = sprite;
            }


            Assets.Sprites.Add(spriteId, sprite);

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
        public static Texture2D LoadTexture(string path, bool warnIfFailed = true)
        {
            Texture2D texture = null;

            if (File.Exists(path))
            {
                byte[] data = TryReadFile(path);
                texture = new Texture2D(1, 1);
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

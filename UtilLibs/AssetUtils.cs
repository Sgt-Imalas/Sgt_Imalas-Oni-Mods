using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace UtilLibs
{
    /// <summary>
    /// Credit: Aki
    /// </summary>
    public class AssetUtils
    {
        public static void AddSpriteToAssets(Assets instance, string spriteid)
        {
            var path = Path.Combine(UtilMethods.ModPath, "assets");
            var texture = LoadTexture(spriteid, path);
            var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector3.zero);
            sprite.name = spriteid;
            instance.SpriteAssets.Add(sprite);
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

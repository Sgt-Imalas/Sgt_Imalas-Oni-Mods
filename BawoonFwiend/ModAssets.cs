using Database;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI.Extensions;
using UtilLibs;
using static ModInfo;

namespace BawoonFwiend
{
    public class ModAssets
    {
        public static Effect JustAMachine;
        public static string JustAMachineId = "NotATrueFriend";
        public class Tags
        {
            public static Tag BalloonGas = TagManager.Create(nameof(BalloonGas));
        }


        public static Dictionary<KeyValuePair<KAnimFile, KAnim.Build.Symbol>, Sprite> BalloonSprites
            = new Dictionary<KeyValuePair<KAnimFile, KAnim.Build.Symbol>, Sprite>();

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


        private static List<string> _balloonFacadeIDs = null;
        public static List<string> GetAllBalloonArtistFacadeIDs()
        {
            if(_balloonFacadeIDs == null)
            {
                _balloonFacadeIDs = new List<string>();
                var db = Db.Get().Permits.BalloonArtistFacades;
                foreach(var facade in db.resources)
                {
                    _balloonFacadeIDs.Add(facade.Id);
                }
            }

            return _balloonFacadeIDs;

        }

        static Dictionary<Sprite, Texture2D> Copies2 = new Dictionary<Sprite, Texture2D>();
        static Texture2D GetSingleSpriteFromTexture(Sprite sprite)
        {
            if (sprite == null)
                return null;

            if (!Copies2.ContainsKey(sprite))
            {

                var output = new Texture2D((int)sprite.textureRect.width, (int)sprite.textureRect.height);
                var r = sprite.textureRect;
                if (r.width == 0 || r.height == 0)
                    return null;

                var readableTexture = GetReadableCopy(sprite.texture);

                if (readableTexture == null)
                    return null;

                var pixels = readableTexture.GetPixels((int)r.x, (int)r.y, (int)r.width, (int)r.height);
                output.SetPixels(pixels);
                output.Apply();
                output.name = sprite.texture.name + " " + sprite.name;
                Copies2.Add(sprite, output);
            }
            return Copies2[sprite];
        }

        static Dictionary<BalloonOverrideSymbol, Color> OverrideColors = new Dictionary<BalloonOverrideSymbol, Color>();
        public static Color GetColourFrom(BalloonOverrideSymbol SkinOverride)
        {
            if (OverrideColors.ContainsKey(SkinOverride))
                return OverrideColors[SkinOverride];

            Sprite BallonSprite = ModAssets.GetSpriteFrom(SkinOverride.animFile.Unwrap(), SkinOverride.symbol.Unwrap());
            Texture2D texture = GetSingleSpriteFromTexture(BallonSprite);
            Dictionary<Color, int> colors = new Dictionary<Color, int>();

            for (int x = 0; x < texture.width; ++x)
            {
                for (int y = 0; y < texture.height; ++y)
                {
                    var coloratpx = texture.GetPixel(x, y);
                    if (coloratpx == null || coloratpx.r + coloratpx.b + coloratpx.g < 0.3)
                        continue;

                    if (colors.ContainsKey(coloratpx))
                    {
                        colors[coloratpx]++;
                    }
                    else
                    {
                        colors[coloratpx] = 1;
                    }
                }
            }

            var all = colors.OrderBy(col => col.Value).Take(10).ToList();

            //foreach (var coloratpx in all)
            //{
            //    Debug.Log(coloratpx);
            //}

            //float r = 0,g=0,b=0,a = 0;

            //foreach (var coloratpx in all)
            //{
            //    g += coloratpx.Key.g/ all.Count;
            //    r+= coloratpx.Key.r / all.Count;
            //    b += coloratpx.Key.b / all.Count;
            //    a += coloratpx.Key.a / all.Count;
            //} = new Color(r , g , b , a); 


            var finalColor = colors.FirstOrDefault(x => x.Value == colors.Values.Max()).Key;
            OverrideColors[SkinOverride] = finalColor;
            return finalColor;
        }
        public static Sprite GetSpriteFrom(KAnimFile animFile, KAnim.Build.Symbol symbol)
        {
            var SpriteKey = new KeyValuePair<KAnimFile, KAnim.Build.Symbol>(animFile, symbol);
            if (BalloonSprites.TryGetValue(SpriteKey, out var spriteFound))
                return spriteFound;


            KAnimFileData data = animFile.GetData();
            int frame2 = default(KAnim.Anim.FrameElement).frame;
            KAnim.Build.SymbolFrameInstance symbolFrame = symbol.GetFrame(frame2);
            if (symbolFrame.Equals(default))
            {
                SgtLogger.l("SymbolFrame [" + frame2 + "] is missing");
                return Assets.GetSprite("unknown");
            }

            Texture2D texture = data.build.GetTexture(0);
            Debug.Assert(texture != null, "Invalid texture on " + animFile.name);
            float x = symbolFrame.uvMin.x;
            float x2 = symbolFrame.uvMax.x;
            float y = symbolFrame.uvMax.y;
            float y2 = symbolFrame.uvMin.y;
            int num = (int)((float)texture.width * Mathf.Abs(x2 - x));
            int num2 = (int)((float)texture.height * Mathf.Abs(y2 - y));
            float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
            Rect rect = default(Rect);
            rect.width = num;
            rect.height = num2;
            rect.x = (int)((float)texture.width * x);
            rect.y = (int)((float)texture.height * y);
            float pixelsPerUnit = 100f;
            if (num != 0)
            {
                pixelsPerUnit = 100f / (num3 / (float)num);
            }

            Sprite sprite = Sprite.Create(texture, rect, false ? new Vector2(0.5f, 0.5f) : Vector2.zero, pixelsPerUnit, 0u, SpriteMeshType.FullRect);
            BalloonSprites.Add(SpriteKey, sprite);
            return sprite;
        }

    }
}

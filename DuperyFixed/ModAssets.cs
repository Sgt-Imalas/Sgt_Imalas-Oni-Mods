using ClipperLib;
using DuperyFixed.MinionImages;
using Klei.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateClasses;
using UnityEngine;
using UnityEngine.Rendering;
using UtilLibs;
using static KAnim.Build;
using static ModInfo;
using static STRINGS.UI.DETAILTABS;
using static SymbolOverrideController;

namespace DuperyFixed
{
    internal class ModAssets
    {
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

        public static Sprite GetSpriteFrom(KAnim.Build.Symbol symbol, int frameOverride = -1)
        {
            if (symbol == null)
                return null;

            int frame2 = default(KAnim.Anim.FrameElement).frame;
            if(frameOverride>=0)
            {
                frame2 = frameOverride;
            }
            KAnim.Build.SymbolFrameInstance symbolFrame = symbol.GetFrame(frame2);
            if (symbolFrame.Equals(default))
            {
                SgtLogger.l("SymbolFrame [" + frame2 + "] is missing");
                return Assets.GetSprite("unknown");
            }

            Texture2D texture = symbol.build.GetTexture(0);
            //Debug.Assert(texture != null, "Invalid texture on " + animFile.name);
            float x = symbolFrame.uvMin.x;
            float x2 = symbolFrame.uvMax.x;
            float y = symbolFrame.uvMax.y;
            float y2 = symbolFrame.uvMin.y;
            int num = (int)(texture.width * Mathf.Abs(x2 - x));
            int num2 = (int)(texture.height * Mathf.Abs(y2 - y));
            float num3 = Mathf.Abs(symbolFrame.bboxMax.x - symbolFrame.bboxMin.x);
            Rect rect = default;
            rect.width = num;
            rect.height = num2;
            rect.x = (int)(texture.width * x);
            rect.y = (int)(texture.height * y);
            float pixelsPerUnit = 100f;
            if (num != 0)
            {
                pixelsPerUnit = 100f / (num3 / num);
            }

            Sprite sprite = Sprite.Create(texture, rect, false ? new Vector2(0.5f, 0.5f) : Vector2.zero, pixelsPerUnit, 0u, SpriteMeshType.FullRect);
            //SymbolSprites.Add(SpriteKey, sprite);
            return sprite;
        }

        public static List<KeyValuePair<string, string>> GetAccessoryIDs(Personality personality)
        {
            Dictionary<string, string> accessories = new Dictionary<string, string>();
            var slots = Db.Get().AccessorySlots;
            var bodyData = MinionStartingStats.CreateBodyData(personality);

            accessories.Add(slots.Eyes.Id, slots.Eyes.Lookup(bodyData.eyes).Id);
            accessories.Add(slots.HatHair.Id, slots.HatHair.Lookup("hat_" + HashCache.Get().Get(bodyData.hair)).Id);
            accessories.Add(slots.Hair.Id, slots.Hair.Lookup(bodyData.hair).Id);
            accessories.Add(slots.HeadShape.Id, slots.HeadShape.Lookup(bodyData.headShape).Id);
            accessories.Add(slots.Mouth.Id, slots.Mouth.Lookup(bodyData.mouth).Id);
            return accessories.ToList();
        }

        static Dictionary<Personality, Sprite> DreamImages = new();
        internal static Sprite GetDynamicDreamImage(Personality personality)
        {
            if (DreamImages.TryGetValue(personality, out var s))
                return s;

            var slots = Db.Get().AccessorySlots;
            var bodyData = MinionStartingStats.CreateBodyData(personality);

            Vector2I GetPivotPoint(KAnim.Build.Symbol symbol, Texture2D texture)
            {
                SymbolFrameInstance frame = symbol.GetFrame(0);
                var PivotX = frame.bboxMin.x + texture.width;
                var PivotY = frame.bboxMin.y + texture.height;
                return new (Mathf.RoundToInt(PivotX),Mathf.RoundToInt(PivotY));
            }

            Symbol
                symbolEyes = slots.Eyes.Lookup(bodyData.eyes).symbol,
                symbolHair = slots.Hair.Lookup(bodyData.hair).symbol,
                symbolHead = slots.HeadShape.Lookup(bodyData.headShape).symbol,
                symbolMouth = slots.Mouth.Lookup(bodyData.mouth).symbol;

            var output = new Texture2D(125, 125);
            void WriteToOutput(Symbol symbolToWrite, int xOffsetWrite = 0, int yOffsetWrite = 0, bool pivot = false, bool flipX=false, int symbolOverride = -1)
            {

                Texture2D toWrite = GetSingleSpriteFromTexture(GetSpriteFrom(symbolToWrite, symbolOverride));
                var pivotPoint = GetPivotPoint(symbolToWrite, toWrite);

                int xStart = 0;
                int yStart = 0;
                int xEnd = toWrite.width;
                int yEnd = toWrite.height;

                int xStartWrite = (output.width / 2) - ((toWrite.width / 2))+ xOffsetWrite; 
                int yStartWrite = (output.height / 2) - ((toWrite.height / 2))+ yOffsetWrite;
                if (pivot)
                {
                    xStartWrite += pivotPoint.x/2; //pivot points have their values x2
                    yStartWrite -= pivotPoint.y/2;
                }

                for (int x = xStart; x < xEnd; x++)
                {
                    for (int y = yStart; y < yEnd; y++)
                    {
                        var px = toWrite.GetPixel(x, y);

                        var outputX = flipX ? xEnd - x +xStartWrite : x + xStartWrite;
                        var outputY = y + yStartWrite;

                        if (px.a > 0.1f
                            && outputX >=0 && outputX < output.width 
                            && outputY >=0 && outputY < output.height
                            )
                        {
                            output.SetPixel(outputX, outputY, px);
                        }                        
                    }
                }
            }

            {

                for (int i = 0; i < output.width; i++)
                {
                    for (int j = 0; j < output.height; j++)
                    {
                        output.SetPixel(i, j, Color.clear);
                    }
                }
                
                WriteToOutput(symbolHead);
                WriteToOutput(symbolEyes,8,3, flipX:true);
                WriteToOutput(symbolHair,8,30, true);
                WriteToOutput(symbolMouth,10, -12, symbolOverride:22);


                //var imageBytes = output.EncodeToPNG();
                //File.WriteAllBytes(Path.Combine(UtilMethods.ModPath, personality.nameStringKey + ".png"), imageBytes);
            }




            string nameStringKey = personality.nameStringKey;
            output.Apply();
            output.name = "dreamIcon_" + (nameStringKey[0] + nameStringKey.Substring(1).ToLower());
            var sprite = Sprite.Create(output, new Rect(0, 0, 125, 125), new(0.5f, 0.5f));
            sprite.name = "dreamIcon_" + (nameStringKey[0] + nameStringKey.Substring(1).ToLower());
            DreamImages[personality] = sprite;

            return sprite;
        }
    }
}

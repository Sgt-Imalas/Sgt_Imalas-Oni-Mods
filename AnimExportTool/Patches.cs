using Database;
using HarmonyLib;
using Klei.AI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static ResearchTypes;
using TMPro;

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
            if (sprite == null || sprite.rect == null || sprite.rect.width <= 0|| sprite.rect.height <=0)
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
                    SgtLogger.l(Mathf.RoundToInt(output.width)* Mathf.RoundToInt(output.height)+" > "+tintedPixels.Length+" ?");
                    output.SetPixels(tintedPixels);
                }
                else
                {
                    output.SetPixels(pixels);
                }                
                output.Apply();
                output.name = sprite.texture.name + " " + sprite.name;

                if(useTint)
                    return output;

                Copies2.Add(sprite, output);
            }
            return Copies2[sprite];
        }
        static void WriteUISpriteToFile(Sprite sprite, string folder, string id, Color tint = default)
        {
            Directory.CreateDirectory(folder);
            string fileName = Path.Combine(folder, id + ".png");
            var tex = GetSingleSpriteFromTexture(sprite,tint);

            if (tex == null)
                return;

            var imageBytes = tex.EncodeToPNG();
            File.WriteAllBytes(fileName, imageBytes);
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
                if(kanim == null) return;

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
        static void GetAnimsFromEntity(GameObject instance)
        {
            if (!instance.TryGetComponent<KAnimControllerBase>(out var kbac) || kbac.animFiles.Length==0)
                return;

            if (!instance.TryGetComponent<KPrefabID>(out var kPrefab))
                return;

            var UISpriteDef = Def.GetUISprite(instance);
            if (UISpriteDef == null)
                return;
            var UISprite = UISpriteDef.first;
            var id = kPrefab.PrefabID().ToString();

            if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
            {
                WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, "EntityUISpritesById"), id);
                WriteUISpriteToFile(UISprite, Path.Combine(UtilMethods.ModPath, "EntityUISpritesByName"), TagManager.GetProperName(id,true));
            }
        }
    }
}

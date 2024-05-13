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
using static AnimExportTool.ModAssets;

namespace AnimExportTool
{
    internal class Patches
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

                var output = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
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

        static void WriteUISpriteToFile(Sprite sprite, string folder, string id)
        {
            Directory.CreateDirectory(folder);
            string fileName = Path.Combine(folder, id + ".png");
            var tex = GetSingleSpriteFromTexture(sprite);
            if (tex == null)
                return;

            var imageBytes = tex.EncodeToPNG();
            File.WriteAllBytes(fileName, imageBytes);
        }

        [HarmonyPatch(typeof(BuildingTemplates))]
        [HarmonyPatch(nameof(BuildingTemplates.CreateBuildingDef))]
        public static class AnimsFromBuildings
        {

            public static void Postfix(string id, string anim, BuildingDef __result)
            {
                var kanim = Assets.GetAnim(anim);
                if(kanim == null) return;

                var exportLocation = Path.Combine(UtilMethods.ModPath, "BuildingUISprites");

                var UISprite = Def.GetUISpriteFromMultiObjectAnim(kanim);

                if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
                {
                    WriteUISpriteToFile(UISprite, exportLocation, id);
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
            var exportLocation = Path.Combine(UtilMethods.ModPath, "EntityUISprites");
            if (!instance.TryGetComponent<KAnimControllerBase>(out var kbac) || kbac.animFiles.Length==0)
                return;

            if (!instance.TryGetComponent<KPrefabID>(out var kPrefab))
                return;


            SgtLogger.l(instance.name);
            var UISpriteDef = Def.GetUISprite(instance);
            if (UISpriteDef == null)
                return;
            var UISprite = UISpriteDef.first;


            if (UISprite != null && UISprite != Assets.GetSprite("unknown"))
            {
                WriteUISpriteToFile(UISprite, exportLocation, kPrefab.PrefabID().ToString());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;
using static KCompBuilder;

namespace SetStartDupes.DuplicityEditing.Helpers
{
    internal static class AccessorySlotHelper
    {
        public static List<Accessory> GetAccessories(AccessorySlot slot)
        {
            return slot.accessories.ToList();
        }
        public static List<AccessorySlot> GetAllChangeableSlot()
        {
            List<AccessorySlot> result = new List<AccessorySlot>();
            var slots = Db.Get().AccessorySlots;
            result.Add(slots.Eyes);
            result.Add(slots.Hair);
            //result.Add(slots.HatHair); //depends on hair
            result.Add(slots.HeadShape);
            //result.Add(slots.Mouth); //depending on HeadShape
            result.Add(slots.Body);
            result.Add(slots.Arm);
            result.Add(slots.ArmLower);
            result.Add(slots.Neck);
            result.Add(slots.Pelvis);
            result.Add(slots.Leg);
            result.Add(slots.Foot);
            result.Add(slots.Hand);
            result.Add(slots.Cuff);
            result.Add(slots.Belt);
            result.Add(slots.ArmLowerSkin);
            result.Add(slots.ArmUpperSkin);
            result.Add(slots.LegSkin);

            return result;
        }
        public static Dictionary<KeyValuePair<KAnimFile, KAnim.Build.Symbol>, Sprite> SymbolSprites
            = new Dictionary<KeyValuePair<KAnimFile, KAnim.Build.Symbol>, Sprite>();
        public static Sprite GetSpriteFrom(KAnim.Build.Symbol symbol)
        {
            ////var SpriteKey = new KeyValuePair<KAnimFile, KAnim.Build.Symbol>(animFile, symbol);
            //if (SymbolSprites.TryGetValue(SpriteKey, out var spriteFound))
            //    return spriteFound;

            if (symbol == null)
                return null;

            //KAnimFileData data = animFile.GetData();


            //if(data ==null) return null;    

            int frame2 = default(KAnim.Anim.FrameElement).frame;
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

        public static HashSet<string> CritterEffects = new()
        {
            "Ranched",
            "HadMilk",
            "EggSong",
            "EggHug",
            "HuggingFrenzy",
            "DivergentCropTended",
            "DivergentCropTendedWorm",
            "MooWellFed",
            "InteractedWithAirborneCondo",
            "InteractedWithCritterCondo",
            "InteractedWithUnderwaterCondo",
            "Charging",
            "BotSweeping",
            "BotMopping",
            "ScoutBotCharging",
            "MachineTinker",
            "FarmTinker"
        };

        internal static bool IsCritterTrait(string id)
        {
            return CritterEffects.Contains(id);
        }
    }
}

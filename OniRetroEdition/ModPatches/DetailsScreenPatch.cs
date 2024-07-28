using HarmonyLib;
using OniRetroEdition.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace OniRetroEdition.ModPatches
{
    internal class DetailsScreenPatch
    {
        static Image UIImage;
        [HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.OnSpawn))]
        public static class AddSelectableIcon
        {
            public static void Prefix(DetailsScreen __instance)
            {
                var bg = __instance.transform.Find("TitleBar/Border/BG");
                KImage icon = Util.KInstantiateUI<KImage>(bg.gameObject, bg.gameObject, true);
                var rect = icon.rectTransform();
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 35, 50);
                rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -15, 50);
                icon.overrideSprite = null;
                icon.sprite = Assets.GetSprite("Unknown");
                icon.color = Color.white;
                UIImage = icon;
            }
        }
        [HarmonyPatch(typeof(DetailsScreen), nameof(DetailsScreen.Refresh))]
        public static class SetSelectableIcon
        {
            static void SetPortrait(Sprite sprite, Color color = default)
            {
                if (UIImage == null)
                    return;

                UIImage.gameObject.SetActive(true);
                if (color == default)
                    color = Color.white;


                //var rectImg = UIImage.rectTransform();

                //Rect rect = sprite.rect;
                //if (rect.width > rect.height)
                //{
                //    var size = rect.height / rect.width * 50;
                //    rectImg.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -15, size);
                //}
                //else
                //{
                //    var size = rect.width / rect.height * 50;
                //    rectImg.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, 35 , size);
                //}

                UIImage.sprite = (sprite);
                UIImage.color = color;

            }
            public static void Postfix(DetailsScreen __instance, GameObject go)
            {

                var target = __instance.target;

                if (UIImage != null)
                {
                    UIImage.gameObject.SetActive(false);
                    if (target == null)
                        return;

                    if (target.TryGetComponent<CellSelectionObject>(out var comp))
                    {
                        var anim = Def.GetUISprite(comp.element);
                        SetPortrait(anim.first,anim.second);
                        return;
                    };

                    KSelectable component = target.GetComponent<KSelectable>();
                    if (!target.TryGetComponent<KSelectable>(out _) || target.TryGetComponent<MinionIdentity>(out _))
                    {
                        UIImage.gameObject.SetActive(false);
                        return;
                    }

                    var tuple = Def.GetUISprite(target);
                    if (tuple != null)
                    {
                        SetPortrait(tuple.first, tuple.second);
                        return;
                    }
                }
            }
        }
    }
}

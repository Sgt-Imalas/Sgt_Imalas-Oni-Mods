using HarmonyLib;
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
    internal class WattsonMessagePatches
    {
        [HarmonyPatch(typeof(WattsonMessage),nameof(WattsonMessage.OnPrefabInit))]
        private static class ReaddWattsonDupe
        {
            public static void Postfix(WattsonMessage __instance)
            {
                var bg = __instance.transform.Find("Dialog/BG").gameObject;

                var newImage = Util.KInstantiateUI(bg, bg.transform.parent.parent.gameObject, true);

                newImage.TryGetComponent<Image>(out var image);
                var rect = newImage.rectTransform();
                rect.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Horizontal, 350);
                rect.SetSizeWithCurrentAnchors(UnityEngine.RectTransform.Axis.Vertical, 300);
                rect.anchoredPosition = new(-460, 50);

                image.color = Color.white;
                image.sprite = Assets.GetSprite("welcomeDialog_guy");
            }
        }
    }
}

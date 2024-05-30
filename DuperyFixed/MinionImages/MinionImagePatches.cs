//using HarmonyLib;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UnityEngine.UI;
//using UtilLibs;

//namespace DuperyFixed.MinionImages
//{
//    internal class MinionImagePatches
//    {
//        [HarmonyPatch(typeof(MinionBrowserScreen))]
//        [HarmonyPatch(nameof(MinionBrowserScreen.PopulateGallery))]
//        public class ReplaceMissingDreamIcons
//        {
//            const string faceContainer = "minion_face_anim";
//            public static void Postfix(MinionBrowserScreen __instance)
//            {
//                for (int i = 0; i < __instance.Config.items.Length; i++)
//                {
//                    var item = __instance.Config.items[i];
//                    var button = __instance. galleryGridContent.transform.GetChild(i);
//                    if (item == null)
//                    {
//                        SgtLogger.warning("item was null, skipping");
//                        continue;
//                    }
//                    if (item == null)
//                    {
//                        SgtLogger.warning("button object for minion item was null, skipping");
//                        continue;
//                    }
//                    ReplaceMinionFaceIfMissing(item, button, __instance);

//                }                
//            }
//            static void ReplaceMinionFaceIfMissing(MinionBrowserScreen.GridItem item, Transform button,MinionBrowserScreen __instance)
//            {
//                Personality personality = null;
//                if(item is MinionBrowserScreen.GridItem.MinionInstanceTarget it)
//                    personality = it.GetPersonality();
//                else if(item is MinionBrowserScreen.GridItem.PersonalityTarget pt)
//                    personality = pt.GetPersonality();


//                button.TryGetComponent<HierarchyReferences>(out var refs);
//                var img = refs.GetReference<Image>("Icon");

//                if (personality == null && button.TryGetComponent<MinionPortraitHelper>(out var portraitHelper))
//                {
//                    SgtLogger.l("no personality for " + item.GetName());
//                    portraitHelper.gameObject.SetActive(false);
//                    img.gameObject.SetActive(true);
//                    return;
//                }
//                string namestringkey = personality.nameStringKey;
//                if(namestringkey == "MIMA")
//                    namestringkey = "Mi-Ma";

//                if(Assets.Sprites.TryGetValue("dreamIcon_" + namestringkey, out _))
//                {
//                    if (button.TryGetComponent<MinionPortraitHelper>(out portraitHelper))
//                    {
//                        SgtLogger.l("dream icon found for " + item.GetName());
//                        portraitHelper.gameObject.SetActive(false);
//                        img.gameObject.SetActive(true);
//                    }
//                    return;
//                }
                
//                img.gameObject.SetActive(false);


//                if (personality == null)
//                {
//                    SgtLogger.l("personality was null for " + item.GetName());
//                    return;
//                }

//                SgtLogger.l("trying to add face for " + item.GetName());
//                var face = button.Find(faceContainer);

//                if (face == null)
//                {
//                    face = Util.KInstantiateUI(MinionPortraitHelper.GetCrewPortraitPrefab(), button.gameObject, true).transform;
//                    face.name = faceContainer;
//                }
//                face.gameObject.SetActive(true);
//                face.TryGetComponent<MinionPortraitHelper>(out var helper);
//                helper.ApplyMinionAccessories(helper.GetAccessoryIDs(personality));


//            }
//        }
//    }
//}

//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using UnityEngine;
//using UtilLibs;
//using static DetailsScreen;
//using static KAnimControllerBase;
//using static STRINGS.NAMEGEN;

//namespace DuperyFixed.MinionImages
//{
//    public class MinionPortraitHelper : MonoBehaviour
//    {
//        private static GameObject crewPortraitPrefab;
//        public static GameObject GetCrewPortraitPrefab()
//        {
//            if (true)
//            {
//                / grabbin crew face from retiredcolony info screen
//                if (crewPortraitPrefab == null)
//                {
//                    var source = ScreenPrefabs.Instance.RetiredColonyInfoScreen;

//                    var clone = Util.KInstantiateUI(source.duplicantPrefab);
//                    UIUtils.ListAllChildrenPath(clone.transform);
//                    if (clone != null)
//                    {
//                        crewPortraitPrefab = Util.KInstantiateUI(clone);//.transform.Find("PortraitImage").gameObject);
//                        var tr = crewPortraitPrefab.transform;
//                        tr.Find("Labels").gameObject.SetActive(false);
//                        tr.Find("BG").gameObject.SetActive(false);
//                        UIUtils.FindAndRemove<KButton>(crewPortraitPrefab.transform, "");

//                        crewPortraitPrefab.AddOrGet<MinionPortraitHelper>();

//                        UnityEngine.Object.Destroy(clone);
//                    }
//                    ERROR!
//                    else
//                        Debug.Log("[DSS] Error creating search prefab!  The searchbar will not function!");

//                }
//                return crewPortraitPrefab;
//            }
//            else
//            {
//                / other option for a portrait, but kinda breaks that sourcescreen so its not used
//                if (crewPortraitPrefab == null)
//                    {
//                        foreach (SideScreenRef screen in DetailsScreen.Instance.sideScreens)
//                        {
//                            if (screen.screenPrefab.TryGetComponent<AccessControlSideScreen>(out var accessControl))
//                            {
//                                var rowPrefab = Util.KInstantiateUI<AccessControlSideScreenRow>(accessControl.rowPrefab.gameObject);
//                                crewPortraitPrefab = Util.KInstantiateUI(rowPrefab.crewPortraitPrefab.gameObject);
//                                crewPortraitPrefab.SetActive(false);

//                                return crewPortraitPrefab;
//                            }
//                        }
//                    }
//                return crewPortraitPrefab;
//            }
//        }

//        SymbolOverrideController soc;
//        KBatchedAnimController kbac;


//        public List<KeyValuePair<string, string>> GetAccessoryIDs(Personality personality)
//        {
//            Dictionary<string, string> accessories = new Dictionary<string, string>();
//            var slots = Db.Get().AccessorySlots;
//            var bodyData = MinionStartingStats.CreateBodyData(personality);

//            accessories.Add(slots.Eyes.Id, slots.Eyes.Lookup(bodyData.eyes).Id);
//            accessories.Add(slots.HatHair.Id, slots.HatHair.Lookup("hat_" + HashCache.Get().Get(bodyData.hair)).Id);
//            accessories.Add(slots.Hair.Id, slots.Hair.Lookup(bodyData.hair).Id);
//            accessories.Add(slots.HeadShape.Id, slots.HeadShape.Lookup(bodyData.headShape).Id);
//            accessories.Add(slots.Mouth.Id, slots.Mouth.Lookup(bodyData.mouth).Id);
//            return accessories.ToList();
//        }
//        public List<KeyValuePair<string, string>> GetAccessoryIDs(MinionIdentity identity = null, StoredMinionIdentity identityStored = null)
//        {

//            List<KeyValuePair<string, string>> accessories = new List<KeyValuePair<string, string>>();
//            List<ResourceRef<Accessory>> accessoriesOrigin = new();

//            if (identityStored != null)
//            {
//                accessoriesOrigin = identityStored.accessories;
//            }
//            else if (identity != null && identity.TryGetComponent<Accessorizer>(out var accessorizer))
//            {
//                accessoriesOrigin = accessorizer.accessories;
//            }

//            foreach (var accessory in accessoriesOrigin)
//            {
//                if (accessory.Get() != null)
//                {
//                    accessories.Add(new KeyValuePair<string, string>(accessory.Get().slot.Id, accessory.Get().Id));
//                }
//            }
//            return accessories;
//        }

//        public void ApplyMinionAccessories(List<KeyValuePair<string, string>> accessories)
//        {
//            if (soc == null)
//                soc = gameObject.GetComponentInChildren<SymbolOverrideController>();

//            if (kbac == null)
//                kbac = gameObject.GetComponentInChildren<KBatchedAnimController>();

//            kbac.visibilityType = VisibilityType.Default;
//            soc.RemoveAllSymbolOverrides();
//            kbac.SetSymbolVisiblity((KAnimHashedString)"snapTo_neck", false);
//            kbac.SetSymbolVisiblity((KAnimHashedString)"snapTo_goggles", false);
//            kbac.SetSymbolVisiblity((KAnimHashedString)"snapTo_hat", false);
//            kbac.SetSymbolVisiblity((KAnimHashedString)"snapTo_headfx", false);
//            kbac.SetSymbolVisiblity((KAnimHashedString)"snapTo_hat_hair", false);
//            foreach (KeyValuePair<string, string> accessory in accessories)
//            {
//                if (Db.Get().Accessories.Exists(accessory.Value))
//                {
//                    KAnim.Build.Symbol symbol = Db.Get().Accessories.Get(accessory.Value).symbol;
//                    AccessorySlot accessorySlot = Db.Get().AccessorySlots.Get(accessory.Key);
//                    soc.AddSymbolOverride((HashedString)accessorySlot.targetSymbolId, symbol);
//                    kbac.SetSymbolVisiblity((KAnimHashedString)accessory.Key, true);
//                }
//            }
//            soc.ApplyOverrides();

//            if (gameObject.activeInHierarchy)
//                this.StartCoroutine(this.ActivatePortraitsWhenReady());
//        }
//        private IEnumerator ActivatePortraitsWhenReady()
//        {
//            yield return 0;
//            kbac.transform.localScale = Vector3.one;
//        }
//    }
//}

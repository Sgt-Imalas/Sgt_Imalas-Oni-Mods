using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static InventoryOrganization;

namespace UtilLibs
{
    public static class SupplyClosetUtils
    {

        public static void AddItemsToSubcategory(string subcategoryID, string[] permitIDs)
        {
            if (!subcategoryIdToPermitIdsMap.ContainsKey(subcategoryID))
            {
                SgtLogger.error("Supply Closet Item subcategory not found! Use AddSubcategory instead");
            }
            else
            {
                for (int i = 0; i < permitIDs.Length; i++)
                {
                    subcategoryIdToPermitIdsMap[subcategoryID].Add(permitIDs[i]);
                }
            }
        }

        public static void AddSubcategory(string mainCategory, string subcategoryID, Sprite icon, int sortkey, string[] permitIDs)
        {

            if (categoryIdToSubcategoryIdsMap.ContainsKey(mainCategory))
            {
                if (!subcategoryIdToPermitIdsMap.ContainsKey(subcategoryID))
                {
                    subcategoryIdToPresentationDataMap.Add(subcategoryID, new SubcategoryPresentationData(subcategoryID, icon, sortkey));
                    subcategoryIdToPermitIdsMap.Add(subcategoryID, new HashSet<string>());

                    if(!categoryIdToSubcategoryIdsMap[mainCategory].Contains(subcategoryID))
                        categoryIdToSubcategoryIdsMap[mainCategory].Add(subcategoryID);
                    else
                        SgtLogger.warning("How did this happen? subcategory is registered to this main category but didnt exist?!");

                }
                else
                {
                    SgtLogger.warning("Supply Closet Item subcategory already existing! Use AddItemsToSubcategory instead");
                }
                for (int i = 0; i < permitIDs.Length; i++)
                {
                    subcategoryIdToPermitIdsMap[subcategoryID].Add(permitIDs[i]);
                }
            }
            else
                SgtLogger.error("Supply Closet Main Category not found!");
        }
    }
}

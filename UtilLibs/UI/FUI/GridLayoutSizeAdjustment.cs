using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using static STRINGS.DUPLICANTS.STATUSITEMS;

namespace UtilLibs.UI.FUI
{
    public class GridLayoutSizeAdjustment : KMonoBehaviour
    {
        [MyCmpReq]
        GridLayoutGroup referencedLayoutGroup;
        [MyCmpGet]
        RectTransform rectTransform;

        int paddingTop, paddingBottom, paddingLeft, paddingRight;
        float WidthToHeightRatio;

        public int minSize = 80, maxSize = 120;

        public bool allignWithWidth = true; //otherwise allign with height
        float spacingX, spacingY;

        public override void OnSpawn()
        {
            base.OnSpawn();
            paddingBottom = referencedLayoutGroup.padding.bottom;
            paddingLeft = referencedLayoutGroup.padding.left;
            paddingTop = referencedLayoutGroup.padding.top;
            paddingRight = referencedLayoutGroup.padding.right;
            spacingX = referencedLayoutGroup.spacing.x;
            spacingY = referencedLayoutGroup.spacing.y;

            StartCoroutine(GetRatio());
        }
        bool sizeRatioSet = false;
        IEnumerator GetRatio()
        {
            yield return new WaitForEndOfFrame();
            WidthToHeightRatio = referencedLayoutGroup.cellSize.x / referencedLayoutGroup.cellSize.y;
            sizeRatioSet=true;
            RequestGridResize();
        }

        public void SetValues(int minSize, int maxSize, bool allignWithWidth = true)
        {
            this.minSize = Math.Min(minSize, maxSize);
            this.maxSize = Math.Max(maxSize, minSize);
            this.allignWithWidth = allignWithWidth;
            RequestGridResize();
        }
        
        public void RequestGridResize()
        {
            if(!sizeRatioSet)
            {
                return;                    
            }

            //targetGridLayout.padding.left - (float)targetGridLayout.padding.right
            float SizeToFit = allignWithWidth 
                // Total Size                 minus paddings          plus 1x spacing as there is always 1 spacing less than there are items in a row/column
                ? rectTransform.rect.width - paddingLeft - paddingRight + spacingX-1 
                : rectTransform.rect.height - paddingTop - paddingBottom + spacingY - 1;
            
                SgtLogger.l(referencedLayoutGroup.padding.left + "," + referencedLayoutGroup.padding.right + "," + rectTransform.rect.width );
            SgtLogger.l(SizeToFit.ToString(), "SIZE");
            List<Tuple<int, float>> outputValues = new List<Tuple<int, float>>();

            for(int newSize = minSize; newSize <= maxSize; newSize++) 
            {
                float relevantCellSizeParam = allignWithWidth ? newSize + spacingX : newSize + spacingY;
                float amountItFitsIn = SizeToFit / relevantCellSizeParam;

                float trunctuated = (float)Math.Truncate(amountItFitsIn);

                float percentageLeft =  amountItFitsIn - trunctuated;
                outputValues.Add(new Tuple<int, float> (newSize, percentageLeft));
                SgtLogger.l(relevantCellSizeParam + " : " + amountItFitsIn + " : " + trunctuated + " : " + percentageLeft);
            }
            outputValues = outputValues.OrderBy(tu => tu.second).ToList();
            foreach (var va in outputValues)
            {
                SgtLogger.l(va.second.ToString(), va.first.ToString());
            }
            var newSizeValue = outputValues.First().first;
            SgtLogger.l(newSizeValue.ToString()+", sizeRatio "+ WidthToHeightRatio, "NEW");


            var NewCellSize = new Vector2(
                (allignWithWidth ? newSizeValue : newSizeValue * WidthToHeightRatio),
                (!allignWithWidth ? newSizeValue : newSizeValue / WidthToHeightRatio)
                ) ;

            SgtLogger.l(NewCellSize.ToString(), "NEWSIZE");

            referencedLayoutGroup.cellSize = NewCellSize;

            return;
            if(allignWithWidth)
                referencedLayoutGroup.SetLayoutHorizontal();
            else
                referencedLayoutGroup.SetLayoutVertical();


        }
    }
}

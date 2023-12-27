using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class VerticalPortAttachment : KMonoBehaviour
    {
        public ChainedBuilding.StatesInstance chainedBuilding;

        VerticalPortAttachment top, bottom;

        int topCell, bottomCell;
        public CellOffset TopOffset = new CellOffset(0, 1), BottomOffset = new CellOffset(0, -1);

        public void AttachTop(VerticalPortAttachment _top, bool propagate)
        {
            top = _top;

            if (propagate)
                top.AttachBottom(this, false);

            //if (chainedBuilding != null && propagate)
            //    chainedBuilding.DEBUG_Relink();
        }
        public void AttachBottom(VerticalPortAttachment _bottom, bool propagate)
        {
            bottom = _bottom;
            if(propagate)
                bottom.AttachTop(this, false);

        }
        public void DisconnectTop(bool propagate)
        {
            if (top != null && propagate)
                top.DisconnectBottom(false);
            top = null;

            if (chainedBuilding != null)
                chainedBuilding.DEBUG_Relink();
        }
        public void DisconnectBottom(bool propagate)
        {
            if (bottom != null && propagate)
                bottom.DisconnectTop(false);
            bottom = null;
            if (chainedBuilding != null)
                chainedBuilding.DEBUG_Relink();
        }

        public enum PropOrigin
        {
            undefined = 0,
            bottom = 1,
            top = 2,
        }
        public void PropagateCollectionEvents(
                ref HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain,
                ref bool foundHead,
                ChainedBuilding.StatesInstance ignoredLink,
                int origin = 0)
        {
            if (chainedBuilding != null && origin != 0)
                chainedBuilding.CollectToChain(ref chain, ref foundHead, ignoredLink);
            if (top != null && origin != (int)PropOrigin.top)
                top.PropagateCollectionEvents(ref chain, ref foundHead, ignoredLink, (int)PropOrigin.bottom);
            if (bottom != null && origin != (int)PropOrigin.bottom)
                bottom.PropagateCollectionEvents(ref chain, ref foundHead, ignoredLink, (int)PropOrigin.top);
        }

        public static List<VerticalPortAttachment> GetNetwork(VerticalPortAttachment start)
        {
            var items = new List<VerticalPortAttachment>();
            items.Add(start);
            if(start.top != null)
                start.top.AddToNetworkDownwards(ref items);
            if (start.bottom != null)
                start.bottom.AddToNetworkDownwards(ref items);

            return items;
        }
        public void AddToNetworkUpwards(ref List<VerticalPortAttachment> items)
        {
            items.Add(this);
            if (top != null)
                top.AddToNetworkUpwards(ref items);
        }
        public void AddToNetworkDownwards(ref List<VerticalPortAttachment> items)
        {
            items.Add(this);
            if (bottom != null)
                bottom.AddToNetworkDownwards(ref items);
        }


        public override void OnSpawn()
        {
            base.OnSpawn();
            chainedBuilding = gameObject.GetSMI<ChainedBuilding.StatesInstance>();


            int myCell = Grid.PosToCell(this);
            topCell = Grid.OffsetCell(myCell, TopOffset);
            bottomCell = Grid.OffsetCell(myCell, BottomOffset);

            if (Grid.ObjectLayers[(int)ObjectLayer.Building] != null
                && Grid.ObjectLayers[(int)ObjectLayer.Building].TryGetValue(topCell, out var topBuilding)
                && topBuilding.TryGetComponent<VerticalPortAttachment>(out var attachmentTop))
            {
                AttachTop(attachmentTop, true);
            }
            if (Grid.ObjectLayers[(int)ObjectLayer.Building] != null
                && Grid.ObjectLayers[(int)ObjectLayer.Building].TryGetValue(bottomCell, out var bottomBuilding)
                && bottomBuilding.TryGetComponent<VerticalPortAttachment>(out var attachmentBottom))
            {
                AttachBottom(attachmentBottom, true);
            }

            if (chainedBuilding != null)
                chainedBuilding.DEBUG_Relink();

        }
        public override void OnCleanUp()
        {
            if (bottom != null)
                bottom.DisconnectTop(false);
            this.DisconnectBottom(true);

            if (top != null)
                top.DisconnectBottom(false);
            this.DisconnectTop(true);

            if (chainedBuilding != null)
                chainedBuilding.DEBUG_Relink();

            base.OnCleanUp();
        }
    }
}

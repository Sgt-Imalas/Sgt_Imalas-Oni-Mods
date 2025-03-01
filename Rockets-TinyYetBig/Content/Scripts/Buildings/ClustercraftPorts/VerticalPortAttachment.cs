using System.Collections;
using System.Collections.Generic;

namespace Rockets_TinyYetBig.RocketFueling
{
    internal class VerticalPortAttachment : KMonoBehaviour
    {
        [MyCmpGet] KBatchedAnimController controller;

        public bool CrossPiece = false;

        VerticalPortAttachment top, bottom;

		int topCell, bottomCell;

        public int TopCell => Grid.OffsetCell(Grid.PosToCell(this), TopOffset);
		public int BottomCell => Grid.OffsetCell(Grid.PosToCell(this), BottomOffset);



		public CellOffset TopOffset = new CellOffset(0, 1), BottomOffset = new CellOffset(0, -1);

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			int myCell = Grid.PosToCell(this);
			topCell = Grid.OffsetCell(myCell, TopOffset);
			bottomCell = Grid.OffsetCell(myCell, BottomOffset);
		}
		void HandleConnectionSymbol()
        {
            if (CrossPiece)
            {
                bool beamConnectorCross = top != null && !top.CrossPiece;
                bool crossConnectorCross = top != null && top.CrossPiece;

                controller.SetSymbolVisiblity("sup_top", beamConnectorCross);
                controller.SetSymbolVisiblity("sup_top_fg", beamConnectorCross);
            }
            else
            {

                bool beamConnector = top != null && top.CrossPiece;
                controller.SetSymbolVisiblity("sup_bottom", beamConnector);
            }
        }

        public void AttachTop(VerticalPortAttachment _top, bool propagate)
        {
            top = _top;

            if (propagate)
                top.AttachBottom(this, false);

            HandleConnectionSymbol();
        }
        public void AttachBottom(VerticalPortAttachment _bottom, bool propagate)
        {
            bottom = _bottom;
            if (propagate)
                bottom.AttachTop(this, false);

        }
        public void DisconnectTop(bool propagate, bool delayRelink = false)
        {
            if (top != null && propagate)
                top.DisconnectBottom(false, delayRelink);
            top = null;

            HandleConnectionSymbol();
        }
        public void DisconnectBottom(bool propagate, bool delayRelink = false)
        {
            if (bottom != null && propagate)
                bottom.DisconnectTop(false, delayRelink);
            bottom = null;
        }

        public enum PropOrigin
        {
            undefined = 0,
            bottom = 1,
            top = 2,
        }
        //public void PropagateCollectionEvents(
        //        ref HashSetPool<ChainedBuilding.StatesInstance, ChainedBuilding.StatesInstance>.PooledHashSet chain,
        //        ref bool foundHead,
        //        ChainedBuilding.StatesInstance ignoredLink,
        //        int origin = 0)
        //{
        //    if (top != null && origin != (int)PropOrigin.top)
        //        top.PropagateCollectionEvents(ref chain, ref foundHead, ignoredLink, (int)PropOrigin.bottom);
        //    if (bottom != null && origin != (int)PropOrigin.bottom)
        //        bottom.PropagateCollectionEvents(ref chain, ref foundHead, ignoredLink, (int)PropOrigin.top);
        //}

        public static List<VerticalPortAttachment> GetNetwork(VerticalPortAttachment start)
        {
            var items = new List<VerticalPortAttachment>();
            items.Add(start);
            if (start.top != null)
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

			if (Grid.ObjectLayers[(int)ObjectLayer.Building] != null
                && Grid.ObjectLayers[(int)ObjectLayer.Building].TryGetValue(topCell, out var topBuilding)
                && topBuilding.TryGetComponent<VerticalPortAttachment>(out var attachmentTop)
                && Grid.PosToXY(topBuilding.transform.position).X == Grid.PosToXY(this.transform.position).X)
            {
                AttachTop(attachmentTop, true);
            }
            if (Grid.ObjectLayers[(int)ObjectLayer.Building] != null
                && Grid.ObjectLayers[(int)ObjectLayer.Building].TryGetValue(bottomCell, out var bottomBuilding)
                && bottomBuilding.TryGetComponent<VerticalPortAttachment>(out var attachmentBottom)
                && Grid.PosToXY(bottomBuilding.transform.position).X == Grid.PosToXY(this.transform.position).X)
            {
                AttachBottom(attachmentBottom, true);
            }

			HandleConnectionSymbol();
        }

        public override void OnCleanUp()
        {
            if (bottom != null)
                bottom.DisconnectTop(false, true);
            this.DisconnectBottom(true);

            if (top != null)
                top.DisconnectBottom(false,true);
            this.DisconnectTop(true);

            base.OnCleanUp();
        }
    }
}

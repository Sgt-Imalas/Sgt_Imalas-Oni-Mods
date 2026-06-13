using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rockets_TinyYetBig.NonRocketBuildings
{
	internal class RocketPortLadderHider : KMonoBehaviour
	{
		[MyCmpReq]
		KPrefabID kpid;
		[MyCmpReq]
		KBatchedAnimController kbac;
		[MyCmpReq]
		BuildingComplete buildingComplete;
		public override void OnSpawn()
		{
			base.OnSpawn();
			UpdateLeftConnectorSymbol();
		}
		public void UpdateLeftConnectorSymbol(bool cleanUp = false)
		{

			bool setLeftConnectorVisible = true;

			var tileLeft = Grid.CellLeft(this.buildingComplete.GetCell());
			var tileRight = Grid.CellRight(this.buildingComplete.GetCell());
			var ladderLeft = Grid.Objects[tileLeft, (int)buildingComplete.Def.ObjectLayer];
			var ladderRight = Grid.Objects[tileRight, (int)buildingComplete.Def.ObjectLayer];
			if (ladderLeft != null && ladderLeft.TryGetComponent<KPrefabID>(out var kpref) && kpref.PrefabID() == kpid.PrefabID())
			{
				setLeftConnectorVisible = false;
			}

			if (ladderRight != null && ladderRight.TryGetComponent<RocketPortLadderHider>(out var ladder))
			{
				ladder.UpdateLeftConnectorSymbol();
			}
			if (cleanUp)
				return;
			if (kbac?.layering?.layerControllers?.TryGetValue(KAnim.SymbolFlags.FG, out var fgControllerBase) ?? false)
				(fgControllerBase as KBatchedAnimController)?.SetSymbolVisiblity("connector_left", setLeftConnectorVisible);

		}
		public override void OnCleanUp()
		{
			UpdateLeftConnectorSymbol(true);
			base.OnCleanUp();
		}
	}
}

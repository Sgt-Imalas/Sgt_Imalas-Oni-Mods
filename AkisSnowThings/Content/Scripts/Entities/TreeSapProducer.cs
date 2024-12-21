using AkisSnowThings.Content.Scripts.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UtilLibs;

namespace AkisSnowThings.Content.Scripts.Entities
{

	public class TreeSapProducer : KMonoBehaviour, ISim200ms
	{
		public float sapProductionRatePerSecond;

		public Storage SapStorage;
		public Growing growth;
		[MyCmpReq]
		public WiltCondition wiltCondition;
		[MyCmpReq]
		public PrimaryElement primaryElement;
		[MyCmpReq]
		public ReceptacleMonitor rm;
		[MyCmpReq]
		public KSelectable selectable;
		public override void OnSpawn()
		{
			base.OnSpawn();
			growth = this.GetComponent<Growing>();
		}

		public void Sim200ms(float dt)
		{
			selectable.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest);
			if (!growth?.IsGrown() ?? false || wiltCondition.IsWilting() || wiltCondition.goingToWilt)
				return;

			foreach(var potentialWiltingSource in wiltCondition.WiltConditions)
			{
				//wilting condition +  bool value if it is fulfilled (true == not wilting by that condition)
				if (!potentialWiltingSource.Value) //tree would be wilting if growing, but it is already fully grown, so I have to check manually if any of the conditions are met.
				{
					//SgtLogger.l(((WiltCondition.Condition)potentialWiltingSource.Key).ToString() + " causes wilting");
					return;
				}
			}
			float rate = rm.Replanted ? sapProductionRatePerSecond : sapProductionRatePerSecond / 4f;
			rate *= dt;

			SapStorage.AddLiquid(SnowModElements.EvergreenTreeSap, rate, primaryElement.Temperature, 255, 0);
		}
	}
}

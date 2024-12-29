using AkisSnowThings.Content.Defs.Plants;
using AkisSnowThings.Content.Scripts.Elements;
using Klei.AI;
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

		[MyCmpReq] 
		public Modifiers plantModifiers;

		public override void OnSpawn()
		{
			base.OnSpawn();
			growth = this.GetComponent<Growing>();
			GameScheduler.Instance.ScheduleNextFrame("logging tree", (_) =>
			{
				foreach (var modifier in plantModifiers.GetAmounts())
					SgtLogger.l("Modifier: " + modifier.amount.Name + ": " + modifier.value);
				foreach (var attribute in plantModifiers.GetAttributes())
				{

					SgtLogger.l("Attribute: " + attribute.Attribute.Name + " base: " + attribute.GetBaseValue() + ", totalDisplay: " + attribute.GetTotalDisplayValue() + ", total: " + attribute.GetTotalValue());
					SgtLogger.l("Pre-Modified: " + plantModifiers.GetPreModifiedAttributeValue(attribute.Attribute));
				}
			});
		}

		public void Sim200ms(float dt)
		{
			selectable.RemoveStatusItem(Db.Get().MiscStatusItems.NotMarkedForHarvest);

			if (SapStorage.IsFull())
				return;

			if (!growth?.IsGrown() ?? false || wiltCondition.IsWilting() || wiltCondition.goingToWilt)
				return;

			foreach (var potentialWiltingSource in wiltCondition.WiltConditions)
			{
				//wilting condition +  bool value if it is fulfilled (true == not wilting by that condition)
				if (!potentialWiltingSource.Value) //tree would be wilting if growing, but it is already fully grown, so I have to check manually if any of the conditions are met.
				{
					SgtLogger.l(((WiltCondition.Condition)potentialWiltingSource.Key).ToString() + " causes wilting");
					return;
				}
			}
			float rate = rm.Replanted ? sapProductionRatePerSecond : sapProductionRatePerSecond / 4f;
			rate *= dt;			
			rate *= GetYieldModifiers();

			SapStorage.AddLiquid(SnowModElements.EvergreenTreeSap, rate, primaryElement.Temperature, 255, 0);
		}
		public float GetYieldModifiers()
		{
			float modifier = 1f;

			if (plantModifiers != null)
			{
				var db = Db.Get();
				float totalWoodYield = plantModifiers.GetPreModifiedAttributeValue(db.PlantAttributes.YieldAmount);

				float yieldModifier =  totalWoodYield / EvergreenTreeConfig.HARVEST_MASS;
				SgtLogger.l("yield modifier: "+yieldModifier);
				modifier *= yieldModifier;

				float growthTime = plantModifiers.GetPreModifiedAttributeValue(db.Amounts.Maturity.maxAttribute);
				float timeModifier = growthTime / (EvergreenTreeConfig.GROWTH_TIME/600f);
				modifier /= timeModifier;
			}

			return modifier;
		}
	}
}

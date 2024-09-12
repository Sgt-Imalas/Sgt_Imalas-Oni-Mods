using KSerialization;
using LogicSatellites.Entities;

namespace LogicSatellites.Behaviours
{
	class SatelliteTypeHolder : KMonoBehaviour, ISim1000ms
	{
		[MyCmpGet]
		Pickupable pickupable;
		[MyCmpGet]
		PrimaryElement primaryElement;

		[Serialize]
		private int _satelliteType = 0;
		public int SatelliteType
		{
			get
			{
				return _satelliteType;
			}
			set
			{
				_satelliteType = value;
				OverrideSatDescAndName();
			}
		}
		void OverrideSatDescAndName()
		{
			var nameHolder = gameObject.GetComponent<KSelectable>();
			var descHolder = gameObject.AddOrGet<InfoDescription>();
			if (nameHolder != null)
			{
				nameHolder.SetName(ModAssets.SatelliteConfigurations[_satelliteType].NAME);
			}
			if (descHolder != null)
			{
				descHolder.description = (ModAssets.SatelliteConfigurations[_satelliteType].DESC);
			}
		}

		public override void OnSpawn()
		{
			base.OnSpawn();
			OverrideSatDescAndName();
		}

		public void Sim1000ms(float dt)
		{
			if (pickupable.storage == null)
			{
				var constructionPart = GameUtil.KInstantiate(Assets.GetPrefab(SatelliteComponentConfig.ID), gameObject.transform.position, Grid.SceneLayer.Ore);
				constructionPart.SetActive(true);
				var constructionPartElement = constructionPart.GetComponent<PrimaryElement>();
				constructionPartElement.Units = primaryElement.Mass / constructionPartElement.MassPerUnit;

				Destroy(this.gameObject);
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisSnowThings.Content.Scripts.Entities
{
	internal class ResourceDropper : KMonoBehaviour
	{
		[MyCmpReq] PrimaryElement PrimaryElement;

		public bool SelfDestruct = true;
		public float MassOverride = -1;
		public SimHashes ElementOverride = SimHashes.Vacuum;

		public override void OnSpawn()
		{
			base.OnSpawn();

			if (this.SelfDestruct)
			{
				SpawnAsMass();
				CommitSelfDestruct();
			}
		}

		private void CommitSelfDestruct()
		{

			UnityEngine.Object.Destroy(this.gameObject);
		}

		private void SpawnAsMass()
		{
			if (MassOverride < 0)
			{
				MassOverride = PrimaryElement.Mass;
			}
			if (ElementOverride == SimHashes.Vacuum)
			{
				ElementOverride = PrimaryElement.ElementID;
			}
			var element = ElementLoader.GetElement(ElementOverride.CreateTag());
			element.substance.SpawnResource(this.transform.GetPosition(),MassOverride,PrimaryElement.Temperature,PrimaryElement.DiseaseIdx, PrimaryElement.diseaseCount);
		}
	}
}

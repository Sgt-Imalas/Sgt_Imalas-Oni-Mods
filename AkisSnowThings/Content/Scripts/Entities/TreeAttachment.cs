using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkisSnowThings.Content.Scripts.Entities
{
	public class TreeAttachment : KMonoBehaviour
	{
		[MyCmpReq]
		BuildingAttachPoint buildingAttachPoint;

		Growing growth;
		public override void OnSpawn()
		{
			base.OnSpawn();
			growth = this.GetSMI<Growing>();
		}

		public bool CanAttachToTree()
		{
			return growth.IsGrown();
		}
	}
}

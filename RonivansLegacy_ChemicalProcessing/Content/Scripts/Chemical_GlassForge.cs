using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class Chemical_GlassForge : KMonoBehaviour
	{
		public Guid statusHandle;
		[SerializeField]
		public CellOffset HeatedOutputOffset = new(0, 0);
		[SerializeField]
		public CellOffset? HeatedSecondaryOutputOffset = null;
		[SerializeField]
		public float MeltingTemperature = UtilMethods.GetKelvinFromC(1000);

		public static readonly EventSystem.IntraObjectHandler<Chemical_GlassForge> CheckPipesDelegate = new EventSystem.IntraObjectHandler<Chemical_GlassForge>(delegate (Chemical_GlassForge component, object data)
		{
			component.CheckPipes(data);
		});

		public override void OnPrefabInit()
		{
			base.OnPrefabInit();
			Subscribe(-2094018600, CheckPipesDelegate);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			Unsubscribe(-2094018600, CheckPipesDelegate);
		}

		public void CheckPipes(object data)
		{
			KSelectable component = GetComponent<KSelectable>();
			int cell = Grid.OffsetCell(Grid.PosToCell(this), HeatedOutputOffset);
			GameObject primaryPipe = Grid.Objects[cell, 16];
			bool setStatusItem = false;
			GameObject secondaryPipe = null;

			if (HeatedSecondaryOutputOffset.HasValue)
			{
				int cell2 = Grid.OffsetCell(Grid.PosToCell(this), HeatedSecondaryOutputOffset.Value);
				secondaryPipe = Grid.Objects[cell2, 16];
			}

			if (primaryPipe != null && primaryPipe.GetComponent<PrimaryElement>().Element.highTemp <= MeltingTemperature)
			{
				setStatusItem = true;
			}
			if (secondaryPipe != null && secondaryPipe.GetComponent<PrimaryElement>().Element.highTemp <= MeltingTemperature)
			{
				setStatusItem = true;
			}

			if (setStatusItem)
			{
				statusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.PipeMayMelt);
			}
			else
			{
				component.RemoveStatusItem(statusHandle);
			}
		}
	}
}

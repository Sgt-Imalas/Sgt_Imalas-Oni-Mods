using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs;

namespace RonivansLegacy_ChemicalProcessing.Content.Scripts
{
	class Chemical_GlassForge : ComplexFabricator
	{
		public Guid statusHandle;
		[SerializeField]
		public CellOffset HeatedOutputOffset = new(0,0);
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
			GameObject gameObject = Grid.Objects[cell, 16];
			if (gameObject != null)
			{
				if (gameObject.GetComponent<PrimaryElement>().Element.highTemp > MeltingTemperature)
				{
					component.RemoveStatusItem(statusHandle);
				}
				else
				{
					statusHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.PipeMayMelt);
				}
			}
			else
			{
				component.RemoveStatusItem(statusHandle);
			}
		}
	}
}

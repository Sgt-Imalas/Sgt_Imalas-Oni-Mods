using System;
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
			this.Sub((int)GameHashes.ConduitConnectionChanged, CheckPipes);
		}
		public override void OnSpawn()
		{
			base.OnSpawn();
			CheckPipes(null);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();
			this.UnsubAll();
		}

		public void CheckPipes(object _)
		{
			KSelectable component = GetComponent<KSelectable>();
			int cell = Grid.OffsetCell(Grid.PosToCell(this), HeatedOutputOffset);
			int pipeLayer = (int)ObjectLayer.LiquidConduit;
			GameObject primaryPipe = Grid.Objects[cell, pipeLayer];
			bool setStatusItem = false;
			GameObject secondaryPipe = null;

			if (HeatedSecondaryOutputOffset.HasValue)
			{
				int cell2 = Grid.OffsetCell(Grid.PosToCell(this), HeatedSecondaryOutputOffset.Value);
				secondaryPipe = Grid.Objects[cell2, pipeLayer];
			}

			if (primaryPipe != null && primaryPipe.GetComponent<PrimaryElement>().Element.highTemp <= MeltingTemperature)
			{
				setStatusItem = true;
			}
			if (secondaryPipe != null && secondaryPipe.GetComponent<PrimaryElement>().Element.highTemp <= MeltingTemperature)
			{
				setStatusItem = true;
			}

			statusHandle = component.ToggleStatusItem(Db.Get().BuildingStatusItems.PipeMayMelt, setStatusItem);
		}
	}
}

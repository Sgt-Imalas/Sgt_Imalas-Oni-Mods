using Rendering.World;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UtilLibs;

namespace NaturalConstruction.Content.Scripts
{
	/// <summary>
	/// for sandbox insta build
	/// </summary>
	internal class CompleteBuildingNaturalSpawner : KMonoBehaviour
	{
		[MyCmpGet] KSelectable selectable;
		[MyCmpReq] PrimaryElement element;
		[MyCmpReq] Building building;
		public override void OnSpawn()
		{
			base.OnSpawn();
			//GameScheduler.Instance.ScheduleNextFrame("spawn nat tile" ,(_)=>			SpawnNaturalTile());
			//StartCoroutine(DelayedReplacement());
			SpawnNaturalTile();
		}

		IEnumerator DelayedReplacement()
		{
			yield return null;
			SpawnNaturalTile();
		}

		void SpawnNaturalTile()
		{
			if (selectable.IsSelected)
				selectable.Unselect();

			PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Building, selectable.GetName(), base.transform);
			int cell = Grid.PosToCell(this);
			ushort elementIdx;
			float temperature;
			byte diseaseIdx = byte.MaxValue;
			int diseaseAmount = 0;
			float naturalMass = element.Mass;
			SimHashes elementId;

			elementId = element.Element.id;
			elementIdx = element.Element.idx;
			temperature = element.Temperature;
			diseaseIdx = element.DiseaseIdx;
			diseaseAmount = element.DiseaseCount;


			bool backwallBuilding = building.Def.ObjectLayer == ObjectLayer.Backwall;

			naturalMass *= Mathf.Clamp(Config.Instance.SpawningMassMultiplier, 1f, 2f);

			if (backwallBuilding)
			{
				SimMessages.SetBackwallData(cell, elementIdx, naturalMass, temperature);
			}
			else
			{
				ModAssets.MovePickupables(cell);
				SimMessages.ReplaceAndDisplaceElement(cell, elementId, CellEventLogger.Instance.TemplateLoader, naturalMass, temperature, diseaseIdx, diseaseAmount);
				GameScenePartitioner.Instance.TriggerEvent(cell, GameScenePartitioner.Instance.solidChangedLayer, null);
			}
			//gameObject.Trigger((int)GameHashes.NewConstruction, (object)this);
			this.DeleteObject();

		}
	}
}

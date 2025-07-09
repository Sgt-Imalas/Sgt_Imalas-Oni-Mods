using KSerialization;
using Newtonsoft.Json.Linq;
using System;
using UnityEngine;

namespace RotatableRadboltStorage
{
	class BatteryDirectionAddon : KMonoBehaviour,
		IHighEnergyParticleDirection,
		ISaveLoadable
	{

		[MyCmpReq]
		Building building;

		[MyCmpReq]
		private KSelectable selectable;
		[Serialize]
		private EightDirection _direction;
		private MeterController directionController;
		[SerializeField]
		public bool OuputOffByOne;

		public int GetCircularHEPOutputCell()
		{
			int x = 0, y = 0;
			if (OuputOffByOne)
			{
				if (Direction.ToString().Contains("Down"))
					y -= 1;
				else if (Direction.ToString().Contains("Up"))
					y += 1;
				if (Direction.ToString().Contains("Right"))
					x += 1;
				else if (Direction.ToString().Contains("Left"))
					x -= 1;
			}

			var offset = building.GetHighEnergyParticleOutputOffset();
			offset.x += x;
			offset.y += y;

			int cell = Grid.OffsetCell(building.GetCell(), offset);
			return cell;
		}

		//public void Fire()
		//{
		//    int particleOutputCell = GetCircularHEPOutputCell();
		//    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab((Tag)"HighEnergyParticle"), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
		//    gameObject.SetActive(true);
		//    if (!(gameObject != null))
		//        return;
		//    HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
		//    component.payload = hepStorage.ConsumeAndGet(particleThreshold);
		//    component.SetDirection(Direction);
		//}


		//public LocString CapacityUnits => UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;

		public EightDirection Direction
		{
			get => _direction;
			set
			{
				_direction = value;
				if (directionController == null)
					return;
				directionController.SetPositionPercent(EightDirectionUtil.GetDirectionIndex(_direction) / 7f);
			}
		}
		public void OnCopySettings(object data)
		{
			GameObject sauceGameObject = data as GameObject;
			if (sauceGameObject != null && sauceGameObject.TryGetComponent<BatteryDirectionAddon>(out var addon))
			{
				this.Direction = addon.Direction;
			}
		}
		
		public override void OnSpawn()
		{
			base.OnSpawn();
			directionController = new MeterController(GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
			directionController.gameObject.GetComponent<KBatchedAnimTracker>().matchParentOffset = true;
			Direction = Direction;
			Subscribe(-905833192, OnCopySettings);
		}
		public override void OnCleanUp()
		{
			base.OnCleanUp();

			Unsubscribe(-905833192, OnCopySettings);
		}
		public static void Blueprints_SetData(GameObject source, JObject data)
		{
			if (source.TryGetComponent<BatteryDirectionAddon>(out var behavior))
			{
				var t1 = data.GetValue("Direction");
				if (t1 == null)
					return;
				var Direction = t1.Value<int>();
				behavior.Direction = (EightDirection)Direction;

			}
		}
		public static JObject Blueprints_GetData(GameObject source)
		{
			if (source.TryGetComponent<BatteryDirectionAddon>(out var behavior))
			{
				return new JObject()
				{
					{ "Direction", (int)behavior.Direction},
				};
			}
			return null;
		}
	}
}

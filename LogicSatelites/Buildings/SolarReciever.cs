using LogicSatellites.Behaviours;
using UnityEngine;

namespace LogicSatellites.Buildings
{
	public class SolarReciever : KMonoBehaviour, IListableOption, ISim1000ms
	{
		public override void OnSpawn()
		{
			base.OnSpawn();
			ModAssets.SolarRecievers.Add(this);

			WorldContainer world = this.GetMyWorld();
			float Y = world.WorldOffset.y + world.WorldSize.y;
			float X = GetColumn();
			//Debug.Log(X + "; " + Y + "; coorsd");

		}

		public int SimulatedLuxFromConnectedSatellites()
		{
			return 0;
		}

		public override void OnCleanUp()
		{
			ModAssets.SolarRecievers.Remove(this);
			base.OnCleanUp();
		}

		public string GetProperName()
		{
			return gameObject.GetProperName() + " SatReciever";
		}
		public int GetColumn()
		{
			Grid.PosToXY(this.transform.position, out var x, out var y);
			return x;
		}

		public void Sim1000ms(float dt)
		{
			//CreateBeamFX();


		}
		public void CreateBeamFX(float x, float y)
		{
			WorldContainer world = this.GetMyWorld();
			int Y = world.WorldOffset.y + world.WorldSize.y - 5;
			int X = GetColumn();

			//var position = this.transform.position;position.x++;
			Quaternion rotation = Quaternion.Euler(-90f, 90f, 0.0f);
			Util.KInstantiate(EffectPrefabs.Instance.OpenTemporalTearBeam, new Vector3(x, y), rotation, this.gameObject);
		}
	}
}

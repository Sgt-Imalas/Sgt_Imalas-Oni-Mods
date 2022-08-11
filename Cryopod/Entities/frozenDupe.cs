using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Cryopod.ModAssets;

namespace Cryopod.Entities
{
    public class frozenDupe : KMonoBehaviour, ISim1000ms
	{
		[Serialize] public MinionStorage DupeStorage;
		[Serialize] private float ForceThawed; //amount of damage done on thawing based on forced process (no power f.e.)

		[Serialize] public float storedDupeDamage = -1; //Damage the dupe has recieved prior to storing
		[Serialize] public List<string> StoredSicknessIDs = new List<string>(); //Sicknessses the dupe had prior to storing
		private int timer = 5;

		public void SetMinionInStorage(List<MinionStorage.Info> dupe)
        {
			var list = new List<MinionStorage.Info>();
            if (dupe.Count > 0)
            {
				
				var minion = new MinionStorage.Info();
				minion.name = dupe.First().name;
				minion.id = dupe.First().id;
				minion.serializedMinion = dupe.First().serializedMinion;
				list.Add(minion);
			}
			DupeStorage.SetStoredMinionInfo(list);
		}
		public void SetDamageValue(float value)
		{
			storedDupeDamage = value;
		}
		public void SetCryoDamageValue(float value)
		{
			ForceThawed = value;
		}
		public void SetStoredSicknesses(List<string> sics)
        {
			StoredSicknessIDs.Clear();
			StoredSicknessIDs.AddRange(sics);
        }

        public void Sim1000ms(float dt)
		{
			Debug.Log(DupeStorage.GetStoredMinionInfo().Count + "<- On THROWOUT IT has these, " + timer);
			if (timer == 0)
			{
				if (DupeStorage.GetStoredMinionInfo().Count <= 0)
				{
					timer--;
					Debug.Log(DupeStorage.GetStoredMinionInfo().Count + "<- On THROWOUT IT has these");
					//Debug.LogError("NO dupe Info found");
					return;
				}

				var newDupe = DupeStorage.GetStoredMinionInfo()[0];
				Debug.Log("STATE 1 "+newDupe.name );
				var NewDupeDeserialized = DupeStorage.DeserializeMinion(newDupe.id, this.transform.position);

				Debug.Log("STATE 2 " + NewDupeDeserialized.name);
				Thawing.HandleDupeThawing(ref NewDupeDeserialized, ref StoredSicknessIDs, ref storedDupeDamage, ref ForceThawed);

				Debug.Log("STATE 5");
				//Util.KDestroyGameObject(this.gameObject);
			}
			if(timer>-1)
			timer--;
		}
    }
}

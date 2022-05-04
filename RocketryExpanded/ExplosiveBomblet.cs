using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace RocketryExpanded
{
    public class ExplosiveBomblet : KMonoBehaviour
    {
        private bool triggered = false;
        private int destructionCountdown;
        public float angle = 360f;
        public float speed = 20f;
        protected override void OnSpawn()
        {
            base.OnSpawn();
            //Explode();
        }
        public void Explode(int _amount = 36, int density = 1)
        {
            int amount = _amount;
            var incrementAmount = angle / amount / density;
            for (float PlacementAngle = 0; PlacementAngle <= angle; PlacementAngle += incrementAmount)
            {
                GameObject gameObject = Util.KInstantiate(Assets.GetPrefab((Tag)NuclearWasteCometConfig.ID), this.transform.GetPosition(), Quaternion.identity);
                gameObject.SetActive(true);
                Comet component = gameObject.GetComponent<Comet>();
                component.entityDamage = 200;
                component.massRange = new Vector2(0.1f, 0.1f);
                component.totalTileDamage = 2f;
                component.splashRadius = 2;
                float radian = (float)Mathf.Deg2Rad * PlacementAngle;

                        component.Velocity = new Vector2(Mathf.Cos(radian) * speed, Mathf.Sin(radian) * speed);
                        component.GetComponent<KBatchedAnimController>().Rotation = (float)PlacementAngle+90f;
            }
            SimMessages.EmitMass(Grid.PosToCell(this.transform.GetPosition()), 
                (byte)ElementLoader.GetElementIndex(SimHashes.Fallout), 
                 1000f,
                 5778f,
                 Db.Get().Diseases.GetIndex((HashedString)Db.Get().Diseases.RadiationPoisoning.Id),
                 100000000);
            Util.KDestroyGameObject(this.gameObject);
        }
    }
}

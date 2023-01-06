using LogicSatellites.Behaviours;
using LogicSatellites.Entities;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TUNING;
using UnityEngine;

namespace LogicSatellites.Buildings
{
    public class SolarReciever : KMonoBehaviour, IListableOption, ISim1000ms
    {
        public Light2D laserSource;
        protected override void OnSpawn()
        {
            base.OnSpawn();
            ModAssets.SolarRecievers.Add(this);

            WorldContainer world = this.GetMyWorld();
            float Y = world.WorldOffset.y + world.WorldSize.y ;
            float X = GetColumn();
            Debug.Log(X + "; " + Y + "; coorsd");


            laserSource = gameObject.AddComponent<Light2D>();
            laserSource.enabled = true;
            laserSource.Lux = 2000;
            laserSource.overlayColour = LIGHT2D.SUNLAMP_OVERLAYCOLOR;
            laserSource.Color = LIGHT2D.SUNLAMP_COLOR;
            laserSource.Range = Y- this.transform.position.y;
            laserSource.Angle = 1f;
            laserSource.Direction = new Vector2(0f, -1f);
            laserSource.shape = Mod.LaserBeam.KleiLightShape;
            laserSource.drawOverlay = false;
            laserSource.Offset = new Vector3(X, Y) - this.transform.position;
            laserSource.FullRefresh();
        }


        protected override void OnCleanUp()
        {
            ModAssets.SolarRecievers.Remove(this);
            Destroy(laserSource);
            base.OnCleanUp();
        }

        public string GetProperName()
        {
            return gameObject.GetProperName()+" SatReciever";
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
        public void CreateBeamFX(float x , float y)
        {
            WorldContainer world = this.GetMyWorld();
            int Y = world.WorldOffset.y + world.WorldSize.y - 5;
            int X = GetColumn();

            //var position = this.transform.position;position.x++;
            Quaternion rotation = Quaternion.Euler(-90f, 90f, 0.0f);
            Util.KInstantiate(EffectPrefabs.Instance.OpenTemporalTearBeam, new Vector3(x,y), rotation, this.gameObject);
        }
    }
}

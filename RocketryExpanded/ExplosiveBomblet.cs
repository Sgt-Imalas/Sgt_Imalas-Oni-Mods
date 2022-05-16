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
        public float dmg = 25f;
        protected override void OnSpawn()
        {
            base.OnSpawn();
            //Explode();
        }


        public void Explode(float _radius = 10f)
        {
            var area = ProcGen.Util.GetFilledCircle(transform.position, _radius);

            var center = new Vector2I((int)this.gameObject.transform.position.x, (int)this.gameObject.transform.position.y);

            var areaNotProtected = new List<Vector2I>();
            foreach(var cell in area)
            {
                if (HasNoBunkersInLine(center, cell))
                {
                    areaNotProtected.Add(cell);
                }
            }

            foreach(var cell in areaNotProtected)
            {
                WorldDamage.Instance.ApplyDamage(Grid.PosToCell(cell), GetDmgValAtPos2(cell,center,dmg), Grid.PosToCell(center));
                
            }
            Util.KDestroyGameObject(this.gameObject);
        }

        public bool HasNoBunkersInLine(Vector2I pos1,Vector2I pos2)
        {
            if (pos1 == pos2) { return true; }
            int x = pos1.x;
            int y = pos1.y;
            int x2 = pos2.x;
            int y2 = pos2.y;

            int w = x2 - x;
            int h = y2 - y;
            int dx1 = 0, dy1 = 0, dx2 = 0, dy2 = 0;
            if (w < 0) dx1 = -1; else if (w > 0) dx1 = 1;
            if (h < 0) dy1 = -1; else if (h > 0) dy1 = 1;
            if (w < 0) dx2 = -1; else if (w > 0) dx2 = 1;
            int longest = Math.Abs(w);
            int shortest = Math.Abs(h);
            if (!(longest > shortest))
            {
                longest = Math.Abs(h);
                shortest = Math.Abs(w);
                if (h < 0) dy2 = -1; else if (h > 0) dy2 = 1;
                dx2 = 0;
            }
            int numerator = longest >> 1;
            for (int i = 0; i <= longest; i++)
            {
                if (!CanDamageThisTile(x,y))
                {
                    return false;
                }
                numerator += shortest;
                if (!(numerator < longest))
                {
                    numerator -= longest;
                    x += dx1;
                    y += dy1;
                }
                else
                {
                    x += dx2;
                    y += dy2;
                }
            }

            return true;
        }
        public bool CanDamageThisTile(int x, int y)
        {
            int cell = Grid.XYToCell(x,y);
            if (!Grid.IsValidCell(cell) || Grid.Element[cell].id == SimHashes.Unobtanium)
            {
                return false;
            }
            GameObject targetTile = Grid.Objects[cell, (int)ObjectLayer.FoundationTile];
            if (targetTile != null && targetTile.HasTag(GameTags.Bunker))
            {
                return false;
            }    
            return true;
        }

        public float GetDmgValAtPos2(Vector2I pos1, Vector2I pos2, float dmgAtPos1)
        {
            if (pos1 == pos2)
            {
                return dmgAtPos1;
            }
            else
            {
                int dx = Math.Abs(pos1.x - pos2.x);
                int dy = Math.Abs(pos1.y - pos2.y);

                return (1f / ((dx * dx) + (dy * dy))) * dmgAtPos1;
            }
        }
    }
}

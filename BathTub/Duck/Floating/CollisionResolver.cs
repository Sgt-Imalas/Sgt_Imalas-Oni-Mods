using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BathTub.Duck.Floating
{
    public class CollisionResolver
    {
        public GravityComponent grav;
        Vector2 desiredPosition;
        Vector2 currentPosition;

        public Vector2 bestPosition;
        float bestDistance;

        Bounds bounds;

        public CollisionResolver(GravityComponent grav, Vector2 desiredPosition)
        {
            this.grav = grav;
            this.currentPosition = grav.transform.position;
            this.desiredPosition = desiredPosition;

            bestDistance = float.PositiveInfinity;
            bestPosition = currentPosition;

            KCollider2D collider = grav.transform.GetComponent<KCollider2D>();
            if (collider != null)
            {
                this.bounds = collider.bounds;
                bounds.Expand(0.05f); // Apply breathing room
            }
        }

        /**
         * Checks if bounds intersects a solid cell.
         */
        bool IntersectsSolidCell(Vector2 target)
        {
            bounds.center = target;
            Vector3 min = bounds.min;
            Vector3 max = bounds.max;

            if (Helpers.IsSolidCell(min) ||
              Helpers.IsSolidCell(max) ||
              Helpers.IsSolidCell(new Vector2(min.x, max.y)) ||
              Helpers.IsSolidCell(new Vector2(max.x, min.y)))
            {
                return true;
            }

            // If the length is greater than one tile, it's possible to have a body get stuck
            // because none of the 4 corners hit the tile.
            // They look like beefy loops but won't even fire 99% of the time.
            for (float x = min.x + 1f; x < max.x; x += 1f)
            {
                if (Helpers.IsSolidCell(new Vector2(x, min.y)) || Helpers.IsSolidCell(new Vector2(x, max.y)))
                {
                    return true;
                }
            }
            for (float y = min.y + 1f; y < max.y; y += 1f)
            {
                if (Helpers.IsSolidCell(new Vector2(min.x, y)) || Helpers.IsSolidCell(new Vector2(max.x, y)))
                {
                    return true;
                }
            }
            return false;
        }

        bool CheckResolves(Vector2 target)
        {
            float newDist = (target - currentPosition).sqrMagnitude;
            if (newDist < bestDistance && !IntersectsSolidCell(target))
            {
                bestPosition = target;
                bestDistance = newDist;
                return true;
            }
            return false;
        }

        bool SpiralSearchCollision()
        {
            Vector2 search = Vector2.zero;
            Vector2 dir = new Vector2(1f, 0f);
            Vector2 total = Vector2.zero;
            float limit = 0.1f;
            while (limit <= 2f)
            {
                search += dir * 0.1f;
                total += dir * 0.1f;
                if (CheckResolves(currentPosition + search)) return true;

                if (total.magnitude >= limit)
                {
                    dir = Vector2.Perpendicular(dir);
                    if (Mathf.Abs(total.y) >= limit)
                    {
                        limit += 0.1f;
                    }
                    total = Vector2.zero;
                }
            }
            return false;
        }

        public void ResolveCollisions()
        {
            if (CheckResolves(desiredPosition)) return;

            bool canMoveX = CheckResolves(new Vector2(desiredPosition.x, currentPosition.y));
            bool canMoveY = CheckResolves(new Vector2(currentPosition.x, desiredPosition.y));

            if (!canMoveY) grav.velocity.y = 0;
            if (!canMoveX) grav.velocity.x = 0;
            if (canMoveX || canMoveY) return;

            if (CheckResolves(currentPosition)) return;

            if (SpiralSearchCollision()) return;

            Debug.LogWarning("[Floatation] FAILED to resolve object collision - object is stuck.");
        }
    }
}

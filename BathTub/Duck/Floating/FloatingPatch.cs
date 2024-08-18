using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UtilLibs.YeetUtils;

namespace BathTub.Duck.Floating
{
    internal class FloatingPatch
    {
        [HarmonyPatch(typeof(GravityComponents), "FixedUpdate")]
        class GravityComponents_FixedUpdate
        {
            private const float MIN_X_VELOCITY = 2.5f;
            private const float MAX_X_VELOCITY = 5f;

            private static readonly GravityComponent NULL_COMPONENT = default(GravityComponent);

            private static float RandomXVelocity()
            {
                float sign = Mathf.Round(UnityEngine.Random.Range(0f, 1f)) * 2 - 1;
                return UnityEngine.Random.Range(MIN_X_VELOCITY, MAX_X_VELOCITY) * sign;
            }

            /**
             * X roaming
             */
            // TODO: Bias based on liquid mass in right/left cells
            private static bool ApplyXVelocityChanges(ref GravityComponent grav, float dt)
            {
                Vector2 newChange = new Vector2(grav.velocity.x, grav.velocity.y);

                if (Mathf.Abs(grav.velocity.x) < MIN_X_VELOCITY / 2 * dt)
                {
                    newChange.x += RandomXVelocity() * dt;
                }
                grav.velocity = newChange;
                return newChange.x < 0f;
            }

            /**
             * Reverse gravity application (floatation)
             */
            private static void ApplyYVelocityChanges(ref GravityComponent grav, float dt)
            {
                GravityComponents.Tuning tuning = TuningData<GravityComponents.Tuning>.Get();
                grav.transform.TryGetComponent<Floater>(out var floater);

                float yExtent =  Helpers.GetYExtent(grav);
                Vector2 position = (Vector2)floater.GetFloatingPosition(true);

                float distanceToSurface = Helpers.GetLiquidSurfaceDistanceAbove(position);
                if (distanceToSurface != float.PositiveInfinity && distanceToSurface > 0)
                {
                    Vector2 target = position + Vector2.up * distanceToSurface;
                    Mathf.SmoothDamp(position.y, target.y, ref grav.velocity.y, 1f, tuning.maxVelocityInLiquid, dt);
                }
                else if (grav.velocity.y > 0)
                {
                    Mathf.SmoothDamp(position.y, position.y, ref grav.velocity.y, 5f, tuning.maxVelocityInLiquid, dt);
                }
            }

            private static Dictionary<int, GravityComponent> gravComponentState = new Dictionary<int, GravityComponent>(1024);

            [HarmonyPriority(Priority.LowerThanNormal)]
            // TODO: Check for potential bug with 1-tile width water
            public static void Prefix(List<GravityComponent> ___data, float dt)
            {
                //processAsNormalGravity.Clear();
                gravComponentState.Clear();

                for (int i = 0; i < ___data.Count; i++)
                {
                    GravityComponent grav = ___data[i];
                    if (!Helpers.ShouldFloat(grav.transform, out var floater)) continue;

                    floater.UpdateDirection(ApplyXVelocityChanges(ref grav, dt));
                    ApplyYVelocityChanges(ref grav, dt);

                    Vector3 pos = grav.transform.GetPosition();
                    Vector3 newPosition = (Vector2)pos + grav.velocity * dt;

                    // Resolve collisions
                    CollisionResolver resolver = new CollisionResolver(grav, newPosition);
                    resolver.ResolveCollisions();

                    // Apply the new gravity/position
                    newPosition = resolver.bestPosition;
                    newPosition.z = pos.z;
                    grav = resolver.grav;

                    grav.transform.SetPosition(newPosition);
                    grav.elapsedTime += dt;

                    gravComponentState.Add(i, grav);
                    ___data[i] = NULL_COMPONENT;
                }
            }

            [HarmonyPriority(Priority.LowerThanNormal)]
            public static void Postfix(List<GravityComponent> ___data)
            {
                foreach (var newGrav in gravComponentState)
                {
                    ___data[newGrav.Key] = newGrav.Value;
                }
            }
        }
    }
}

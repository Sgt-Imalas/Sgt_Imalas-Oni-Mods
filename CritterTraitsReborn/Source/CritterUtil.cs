using Klei.AI;
using UnityEngine;
using System.Linq;
using System;

namespace Heinermann.CritterTraits
{
  static class CritterUtil
  {
    /**
     * Applies a scale safely to the following components and attributes:
     * - Anim controller
     * - Box collider
     * - Max HP
     * - Max Calories
     * 
     * If a component or attribute is missing, it is skipped.
     */
    public static void SetObjectScale(GameObject go, float scale, string description = null)
    {
      // Graphic
      var animController = go.GetComponent<KBatchedAnimController>();
      if (animController != null)
      {
        animController.animScale *= scale;
      }

      // Collision
      var boxCollider = go.GetComponent<KBoxCollider2D>();
      if (boxCollider != null)
      {
        boxCollider.size *= scale;
      }

      // HP and Calories
      var modifiers = go.GetComponent<Modifiers>();
      if (modifiers != null)
      {
        // We need to update the health here or max health will be altered without changing the current health
        var health = go.GetComponent<Health>();
        if (health != null && health.hitPoints == health.maxHitPoints)
        {
          health.hitPoints *= scale;
        }

        modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.HitPoints.maxAttribute.Id, scale - 1.0f, description, is_multiplier: true));
        modifiers.attributes.Add(new AttributeModifier(Db.Get().Amounts.Calories.maxAttribute.Id, scale - 1.0f, description, is_multiplier: true));
      }

      // Mass
      var primaryElement = go.GetComponent<PrimaryElement>();
      if (primaryElement != null)
      {
        primaryElement.MassPerUnit *= scale;
        primaryElement.Units = 1;
      }

      // Drops
      var butcherable = go.GetComponent<Butcherable>();
      if (butcherable != null)
      {
        // If a mod uses non-meat drops then we should take that into account
        var drops = butcherable.drops;
        var dropId = drops.FirstOrDefault() ?? MeatConfig.ID;

        if (scale < 1.0f)
        {
          int numDropsToRemove = (int)((1.0f - scale) / 0.25f);
          drops = drops.Take(drops.Length - numDropsToRemove).ToArray();
        }
        else if (scale > 1.0f)
        {
          int numDropsToAdd = (int)((scale - 1.0f) / 0.25f);
          for (int i = 0; i < numDropsToAdd; ++i)
          {
            drops = drops.Append(dropId);
          }
        }
        butcherable.SetDrops(drops);
      }
    }

    /**
     * Adds a lightBug light to the provided object with the given colour, range, and lux.
     */
    public static void AddObjectLight(GameObject go, Color color, float range, int lux)
    {
      if (go == null) return;

      Light2D light = go.GetComponent<Light2D>();
      if (light == null)
      {
        light = go.AddComponent<Light2D>();
        light.Color = color;
        light.overlayColour = TUNING.LIGHT2D.LIGHTBUG_OVERLAYCOLOR;
        light.Range = range;
        light.Angle = 0f;
        light.Direction = TUNING.LIGHT2D.LIGHTBUG_DIRECTION;
        light.Offset = TUNING.LIGHT2D.LIGHTBUG_OFFSET;
        light.shape = LightShape.Circle;
        light.drawOverlay = true;
        light.Lux = lux;
      }
    }

  }
}

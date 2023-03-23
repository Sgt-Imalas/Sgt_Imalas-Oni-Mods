using HarmonyLib;
using UnityEngine;

namespace CritterTraitsReborn.Patches
{
  [HarmonyPatch(typeof(EntityTemplates), "ExtendEntityToBasicCreature")]
  class EntityTemplates_ExtendEntityToBasicCreature {
    static void Postfix(ref GameObject __result,
      GameObject template,
      FactionManager.FactionID faction,
      string initialTraitID,
      string NavGridName,
      NavType navType,
      int max_probing_radius,
      float moveSpeed,
      string onDeathDropID,
      int onDeathDropCount,
      bool drownVulnerable,
      bool entombVulnerable,
      float warningLowTemperature,
      float warningHighTemperature,
      float lethalLowTemperature,
      float lethalHighTemperature)
    {
      __result.AddOrGet<Components.CritterTraits>();
    }
  }
}

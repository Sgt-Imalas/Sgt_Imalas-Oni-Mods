using HarmonyLib;
using System;
using UnityEngine;

namespace CritterTraitsReborn.Patches
{
  [HarmonyPatch(typeof(EntityTemplates), nameof(EntityTemplates.ExtendEntityToBasicCreature), new Type[]{
      typeof(GameObject),
      typeof(FactionManager.FactionID),
      typeof(string) ,
      typeof(string) ,
      typeof(NavType) ,
      typeof(int) ,
      typeof(float ),
      typeof(string) ,
      typeof(int),
      typeof(bool ) ,
      typeof(bool) ,
      typeof(float ),
      typeof(float ),
      typeof(float ),
      typeof(float )
})]
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

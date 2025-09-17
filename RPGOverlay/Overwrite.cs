using System;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.GameContent;

namespace RPGOverlay;

class Overwrite
{
    public Harmony instance;
    public void OverwriteNativeFunctions()
    {
        if (!Harmony.HasAnyPatches("rpgoverlay"))
        {
            instance = new Harmony("rpgoverlay");
            instance.PatchCategory("rpgoverlay");
            Debug.Log("Entity Overlay has been overwrited");
        }
        else
        {
            Debug.Log("RPGOverlay overwriter has already patched, probably by the singleplayer server");
        }
    }
}

#pragma warning disable IDE0060
[HarmonyPatchCategory("rpgoverlay")]
class EntityOverlay
{
    public static bool ShouldEnablePlayerLevel = false;

    // Overwrite Get Name
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Entity), "GetName")]
    public static string GetName(string __result, Entity __instance)
    {
        Debug.LogDebug($"GetName called with result: {__instance.WatchedAttributes.GetInt("RPGOverlayEntityLevel")} for {__instance.Code}");
        if (__instance.WatchedAttributes.HasAttribute("RPGOverlayEntityLevel") && ShouldEnablePlayerLevel)
            return Lang.Get("rpgoverlay:entity-level", __result, __instance.WatchedAttributes.GetInt("RPGOverlayEntityLevel"));
        else
            return __result;
    }

    // Overwrite Damage Configuration
    [HarmonyPatch(typeof(AiTaskMeleeAttack), MethodType.Constructor, [typeof(EntityAgent), typeof(JsonObject), typeof(JsonObject)])]
    [HarmonyPostfix]
    [HarmonyPriority(Priority.VeryLow)]
    public static void LoadConfig(AiTaskMeleeAttack __instance, EntityAgent entity, JsonObject taskConfig, JsonObject aiConfig)
    {
        // The damage property is already set, this harmony should be the last one to execute
        // so other mods can modify damage if needed
        FieldInfo protectedDamage = AccessTools.Field(typeof(AiTaskMeleeAttack), "damage");
        float damage = (float)protectedDamage.GetValue(__instance);

        Debug.Log($"Calculating entity datas for {entity.Code}, damage: {damage}, max health: {entity.GetBehavior<EntityBehaviorHealth>()?.BaseMaxHealth}");

        entity.WatchedAttributes.SetFloat("RPGOverlayEntityDamage", damage);
        entity.WatchedAttributes.SetFloat("RPGOverlayEntityHealth", entity.GetBehavior<EntityBehaviorHealth>()?.BaseMaxHealth ?? 0.0f);
        entity.WatchedAttributes.SetInt("RPGOverlayEntityLevel", Initialization.CalculateEntityLevel(entity));

        #region damage-tier
        int damageTier = (int)Math.Round(damage, Configuration.damageTierPerDamage);

        __instance.damageTier = damageTier;

        entity.WatchedAttributes.SetInt("RPGOverlayEntityDamageTier", damageTier);
        #endregion

        #region health-tier
        entity.WatchedAttributes.SetInt("RPGOverlayEntityHealthTier", (int)Math.Round(entity.WatchedAttributes.GetFloat("RPGOverlayEntityHealth") / Configuration.healthTierPerHealth));
        #endregion

        Initialization.SetInfoTexts(entity);

        Debug.Log($"Damage: {entity.WatchedAttributes.GetFloat("RPGOverlayEntityDamage")}, DamageTier: {__instance.damageTier} Health: {entity.WatchedAttributes.GetFloat("RPGOverlayEntityHealth")}, Level: {entity.WatchedAttributes.GetInt("RPGOverlayEntityLevel")}");
    }
}
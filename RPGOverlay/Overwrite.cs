using System;
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
    public bool levelUPCompatibility = false;
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
    // Overwrite Get Name
    [HarmonyPostfix]
    [HarmonyPatch(typeof(Entity), "GetName")]
    public static string GetName(string __result, Entity __instance)
    {
        // if (Configuration.enableExtendedLogs)
        //     Debug.Log($"GetName called with result: {__instance.WatchedAttributes.GetInt("RPGOverlayEntityLevel")} for {__instance.Code}");
        return Lang.Get("rpgoverlay:entity-level", __result, __instance.WatchedAttributes.GetInt("RPGOverlayEntityLevel"));
    }

    // Overwrite Damage Configuration
    [HarmonyPostfix]
    [HarmonyPatch(typeof(AiTaskMeleeAttack), "LoadConfig")]
    public static void LoadConfig(AiTaskMeleeAttack __instance, JsonObject taskConfig, JsonObject aiConfig)
    {
        if (__instance.entity == null) return;
        if (__instance.entity.Api.Side == EnumAppSide.Server)
        {
            if (Configuration.enableExtendedLogs)
                Debug.Log($"Calculating entity datas for {__instance.entity.Code}, damage: {taskConfig["damage"].AsFloat(2f)}, max health: {__instance.entity.GetBehavior<EntityBehaviorHealth>()?.BaseMaxHealth}");

            __instance.entity.WatchedAttributes.SetFloat("RPGOverlayEntityDamage", taskConfig["damage"].AsFloat(2f));
            __instance.entity.WatchedAttributes.SetFloat("RPGOverlayEntityHealth", __instance.entity.GetBehavior<EntityBehaviorHealth>()?.BaseMaxHealth ?? 0.0f);
            __instance.entity.WatchedAttributes.SetInt("RPGOverlayEntityLevel", Initialization.CalculateEntityLevel(__instance.entity));

            if (Configuration.enableExtendedLogs)
                Debug.Log($"Damage: {__instance.entity.WatchedAttributes.GetFloat("RPGOverlayEntityDamage")}, Health: {__instance.entity.WatchedAttributes.GetFloat("RPGOverlayEntityHealth")}, Level: {__instance.entity.WatchedAttributes.GetInt("RPGOverlayEntityLevel")}");

            Initialization.SetInfoTexts(__instance.entity);

            #region damage-tier
            int damageTier = (int)Math.Round(taskConfig["damage"].AsDouble(), Configuration.damageTierPerDamage);

            string data = taskConfig.Token?.ToString();

            // Parsing the readonly object into editable object
            JObject jsonObject;
            try
            {
                jsonObject = JObject.Parse(data);
            }
            catch (Exception ex)
            {
                if (Configuration.enableExtendedLogs)
                    Debug.Log($"Invalid json for entity: {__instance.entity.Code}, exception: {ex.Message}");
                return;
            }

            // Checking if damage exist
            if (jsonObject.TryGetValue("damageTier", out JToken _))
            {
                // Redefining the damage
                jsonObject["damageTier"] = damageTier;
            }

            // Updating the json
            taskConfig = new(JToken.Parse(jsonObject.ToString()));
            #endregion
        }
    }
}
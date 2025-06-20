﻿using System;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
using Vintagestory.API.Server;
namespace RPGOverlay;

public class Initialization : ModSystem
{
    private readonly Overwrite overwriter = new();

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        overwriter.OverwriteNativeFunctions();
    }

    public override void StartServerSide(ICoreServerAPI api)
    {
        base.StartServerSide(api);
        if (Configuration.enableLevelUPGlobalLevel && api.ModLoader.IsModEnabled("levelup"))
        {
            EntityOverlay.ShouldEnablePlayerLevel = true;
            foreach (string playerClass in LevelUP.Configuration.ClassExperience.Keys)
                LevelUP.Configuration.RegisterNewClassLevel(playerClass, "Global", 1.0f);
            LevelUP.Configuration.RegisterNewLevelTypeEXP("Global", Configuration.GlobalGetLevelByEXP);
            LevelUP.Server.ExperienceEvents.OnExperienceIncrease += LevelUPOnPlayerExperienceIncrease;
        }
    }

    private void LevelUPOnPlayerExperienceIncrease(IPlayer player, string type, ref ulong amount)
    {
        if (type != "Global")
        {
            // Increase global levels
            LevelUP.Server.Experience.IncreaseExperience(player, "Global", amount);

            // Check if player level up
            int previousLevel = player.Entity.WatchedAttributes.GetInt("LevelUP_Level_Global");
            ulong exp = LevelUP.Server.Experience.GetExperience(player, "Global");
            int nextLevel = Configuration.GlobalGetLevelByEXP(exp);

            // Update global for level up
            player.Entity.WatchedAttributes.SetLong("LevelUP_Global", (long)exp);
            player.Entity.WatchedAttributes.SetInt("LevelUP_Level_Global", nextLevel);
            // Update global for rpg overlay
            player.Entity.WatchedAttributes.SetInt("RPGOverlayEntityLevel", nextLevel);

            // Send message if player level up
            if (previousLevel < nextLevel && player is IServerPlayer serverPlayer)
                LevelUP.Server.Instance.CommunicationChannel.SendPacket(new LevelUP.ServerMessage() { message = $"playerlevelup&{nextLevel}&{"Global"}" }, serverPlayer);
        }
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);
        Configuration.UpdateBaseConfigurations(api);
    }

    public static void SetInfoTexts(Entity entity)
    {
        Debug.LogDebug($"Setting Info text for {entity.Code}");
        // Adding the health tier
        if (entity.WatchedAttributes.HasAttribute("extraInfoText"))
            entity.WatchedAttributes.GetTreeAttribute("extraInfoText")
                .SetString("hpTier", Lang.Get("rpgoverlay:health-tier", CalculateHealthTier(entity)));
    }

    public static int CalculateHealthTier(Entity entity)
    {
        // Health tier calculation
        return (int)Math.Round(entity.WatchedAttributes.GetFloat("RPGOverlayEntityHealth") / Configuration.healthTierPerHealth);
    }

    public static int CalculateEntityLevel(Entity entity)
    {
        Debug.LogDebug($"Calculating entity {entity.Code} level");

        // Getting level by damage
        int level = (int)Math.Round(entity.WatchedAttributes.GetFloat("RPGOverlayEntityDamage") / Configuration.levelPerDamage);

        Debug.LogDebug($"Level by damage {level}");

        // Getting level by health
        level += (int)Math.Round(entity.WatchedAttributes.GetFloat("RPGOverlayEntityHealth") / Configuration.levelPerHealth);

        Debug.LogDebug($"Level by health and damage {level}");

        return level;
    }

    public override void Dispose()
    {
        base.Dispose();
        overwriter.instance?.UnpatchAll();
    }

    public override double ExecuteOrder()
    {
        return 1.1;
    }
}

public class Debug
{
    static private ILogger logger;

    static public void LoadLogger(ILogger _logger) => logger = _logger;
    static public void Log(string message)
    {
        logger?.Log(EnumLogType.Notification, $"[RPGOverlay] {message}");
    }
    static public void LogDebug(string message)
    {
        if (Configuration.enableExtendedLogs)
            logger?.Log(EnumLogType.Debug, $"[RPGOverlay] {message}");
    }
    static public void LogWarn(string message)
    {
        logger?.Log(EnumLogType.Warning, $"[RPGOverlay] {message}");
    }
    static public void LogError(string message)
    {
        logger?.Log(EnumLogType.Error, $"[RPGOverlay] {message}");
    }
}

using System;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Common.Entities;
using Vintagestory.API.Config;
namespace RPGOverlay;

public class Initialization : ModSystem
{
    private readonly Overwrite overwriter = new();

    public override void Start(ICoreAPI api)
    {
        base.Start(api);
        overwriter.OverwriteNativeFunctions();
    }

    public override void StartClientSide(ICoreClientAPI api)
    {
        base.StartClientSide(api);
    }

    public override void AssetsLoaded(ICoreAPI api)
    {
        base.AssetsLoaded(api);
        Configuration.UpdateBaseConfigurations(api);
    }

    public static void SetInfoTexts(Entity entity)
    {
        if (Configuration.enableExtendedLogs)
            Debug.Log($"Setting Info text for {entity.Code}");
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
        if (Configuration.enableExtendedLogs)
            Debug.Log($"Calculating entity {entity.Code} level");

        // Getting level by damage
        int level = (int)Math.Round(entity.WatchedAttributes.GetFloat("RPGOverlayEntityDamage") / Configuration.levelPerDamage);

        if (Configuration.enableExtendedLogs)
            Debug.Log($"Level by damage {level}");

        // Getting level by health
        level += (int)Math.Round(entity.WatchedAttributes.GetFloat("RPGOverlayEntityHealth") / Configuration.levelPerHealth);

        if (Configuration.enableExtendedLogs)
            Debug.Log($"Level by health and damage {level}");

        return level;
    }

    public override void Dispose()
    {
        base.Dispose();
        overwriter.instance?.UnpatchAll();
    }
}

public class Debug
{
    private static readonly OperatingSystem system = Environment.OSVersion;
    static private ILogger loggerForNonTerminalUsers;

    static public void LoadLogger(ILogger logger) => loggerForNonTerminalUsers = logger;
    static public void Log(string message)
    {
        // Check if is linux or other based system and if the terminal is active for the logs to be show
        if (system.Platform == PlatformID.Unix || system.Platform == PlatformID.Other || Environment.UserInteractive)
            // Based terminal users
            Console.WriteLine($"{DateTime.Now:d.M.yyyy HH:mm:ss} [RPGOverlay] {message}");
        else
            // Unbased non terminal users
            loggerForNonTerminalUsers?.Log(EnumLogType.Notification, $"[RPGOverlay] {message}");
    }
}

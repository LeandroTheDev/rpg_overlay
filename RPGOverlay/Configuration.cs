using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Vintagestory.API.Common;

namespace RPGOverlay;

#pragma warning disable CA2211
public static class Configuration
{
    private static Dictionary<string, object> LoadConfigurationByDirectoryAndName(ICoreAPI api, string directory, string name, string defaultDirectory)
    {
        string directoryPath = Path.Combine(api.DataBasePath, directory);
        string configPath = Path.Combine(api.DataBasePath, directory, $"{name}.json");
        Dictionary<string, object> loadedConfig;
        try
        {
            // Load Configurations
            string jsonConfig = File.ReadAllText(configPath);
            loadedConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonConfig);
        }
        catch (DirectoryNotFoundException)
        {
            Debug.LogWarn($"WARNING: Configurations directory does not exist creating {name}.json and directory...");
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ERROR: Cannot create directory: {ex.Message}");
            }
            Debug.Log("Loading default configurations...");
            // Load default configurations
            loadedConfig = api.Assets.Get(new AssetLocation(defaultDirectory)).ToObject<Dictionary<string, object>>();

            Debug.LogError($"Configurations loaded, saving configs in: {configPath}");
            try
            {
                // Saving default configurations
                string defaultJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                File.WriteAllText(configPath, defaultJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ERROR: Cannot save default files to {configPath}, reason: {ex.Message}");
            }
        }
        catch (FileNotFoundException)
        {
            Debug.LogWarn($"WARNING: Configurations {name}.json cannot be found, recreating file from default");
            Debug.Log("Loading default configurations...");
            // Load default configurations
            loadedConfig = api.Assets.Get(new AssetLocation(defaultDirectory)).ToObject<Dictionary<string, object>>();

            Debug.LogError($"Configurations loaded, saving configs in: {configPath}");
            try
            {
                // Saving default configurations
                string defaultJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                File.WriteAllText(configPath, defaultJson);
            }
            catch (Exception ex)
            {
                Debug.LogError($"ERROR: Cannot save default files to {configPath}, reason: {ex.Message}");
            }

        }
        catch (Exception ex)
        {
            Debug.LogError($"ERROR: Cannot read the Configurations: {ex.Message}");
            Debug.Log("Loading default values from mod assets...");
            // Load default configurations
            loadedConfig = api.Assets.Get(new AssetLocation(defaultDirectory)).ToObject<Dictionary<string, object>>();
        }
        return loadedConfig;
    }


    #region baseconfigs
    public static int healthTierPerHealth = 5;
    public static int damageTierPerDamage = 3;
    public static int levelPerDamage = 3;
    public static int levelPerHealth = 2;
    public static int levelUpGlobalEXPPerLevelBase = 200;
    public static float levelUpGlobalEXPMultiplyPerLevel = 1.05f;
    public static bool enableLevelUPGlobalLevel = false;
    public static bool enableExtendedLogs = false;

    public static void UpdateBaseConfigurations(ICoreAPI api)
    {
        Dictionary<string, object> baseConfigs = LoadConfigurationByDirectoryAndName(
            api,
            "ModConfig/RPGOverlay/config",
            "base",
            "rpgoverlay:config/base.json"
        );

        { //healthTierPerHealth
            if (baseConfigs.TryGetValue("healthTierPerHealth", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: healthTierPerHealth is null");
                else if (value is not long) Debug.LogError($"CONFIGURATION ERROR: healthTierPerHealth is not int is {value.GetType()}");
                else healthTierPerHealth = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: healthTierPerHealth not set");
        }
        { //damageTierPerDamage
            if (baseConfigs.TryGetValue("damageTierPerDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: damageTierPerDamage is null");
                else if (value is not long) Debug.LogError($"CONFIGURATION ERROR: damageTierPerDamage is not int is {value.GetType()}");
                else damageTierPerDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: damageTierPerDamage not set");
        }
        { //levelPerDamage
            if (baseConfigs.TryGetValue("levelPerDamage", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: levelPerDamage is null");
                else if (value is not long) Debug.LogError($"CONFIGURATION ERROR: levelPerDamage is not int is {value.GetType()}");
                else levelPerDamage = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: levelPerDamage not set");
        }
        { //levelPerHealth
            if (baseConfigs.TryGetValue("levelPerHealth", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: levelPerHealth is null");
                else if (value is not long) Debug.LogError($"CONFIGURATION ERROR: levelPerHealth is not int is {value.GetType()}");
                else levelPerHealth = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: levelPerHealth not set");
        }
        { //enableLevelUPGlobalLevel
            if (baseConfigs.TryGetValue("enableLevelUPGlobalLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableLevelUPGlobalLevel is null");
                else if (value is not bool) Debug.LogError($"CONFIGURATION ERROR: enableLevelUPGlobalLevel is not boolean is {value.GetType()}");
                else enableLevelUPGlobalLevel = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableLevelUPGlobalLevel not set");
        }
        { //levelUpGlobalEXPPerLevelBase
            if (baseConfigs.TryGetValue("levelUpGlobalEXPPerLevelBase", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: levelUpGlobalEXPPerLevelBase is null");
                else if (value is not long) Debug.LogError($"CONFIGURATION ERROR: levelUpGlobalEXPPerLevelBase is not int is {value.GetType()}");
                else levelUpGlobalEXPPerLevelBase = (int)(long)value;
            else Debug.LogError("CONFIGURATION ERROR: levelUpGlobalEXPPerLevelBase not set");
        }
        { //levelUpGlobalEXPMultiplyPerLevel
            if (baseConfigs.TryGetValue("levelUpGlobalEXPMultiplyPerLevel", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: levelUpGlobalEXPMultiplyPerLevel is null");
                else if (value is not double) Debug.LogError($"CONFIGURATION ERROR: levelUpGlobalEXPMultiplyPerLevel is not double is {value.GetType()}");
                else levelUpGlobalEXPMultiplyPerLevel = (float)(double)value;
            else Debug.LogError("CONFIGURATION ERROR: levelUpGlobalEXPMultiplyPerLevel not set");
        }
        { //enableExtendedLogs
            if (baseConfigs.TryGetValue("enableExtendedLogs", out object value))
                if (value is null) Debug.LogError("CONFIGURATION ERROR: enableExtendedLogs is null");
                else if (value is not bool) Debug.LogError($"CONFIGURATION ERROR: enableExtendedLogs is not boolean is {value.GetType()}");
                else enableExtendedLogs = (bool)value;
            else Debug.LogError("CONFIGURATION ERROR: enableExtendedLogs not set");
        }
    }

    public static int GlobalGetLevelByEXP(ulong exp)
    {
        double baseExp = levelUpGlobalEXPPerLevelBase;
        double multiplier = levelUpGlobalEXPMultiplyPerLevel;

        if (multiplier <= 1.0)
        {
            return (int)(exp / baseExp);
        }

        double expDouble = exp;

        double level = Math.Log((expDouble * (multiplier - 1) / baseExp) + 1) / Math.Log(multiplier);

        return Math.Max(0, (int)Math.Floor(level));
    }
    #endregion
}
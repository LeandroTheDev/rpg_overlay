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
            Debug.Log($"WARNING: Configurations directory does not exist creating {name}.json and directory...");
            try
            {
                Directory.CreateDirectory(directoryPath);
            }
            catch (Exception ex)
            {
                Debug.Log($"ERROR: Cannot create directory: {ex.Message}");
            }
            Debug.Log("Loading default configurations...");
            // Load default configurations
            loadedConfig = api.Assets.Get(new AssetLocation(defaultDirectory)).ToObject<Dictionary<string, object>>();

            Debug.Log($"Configurations loaded, saving configs in: {configPath}");
            try
            {
                // Saving default configurations
                string defaultJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                File.WriteAllText(configPath, defaultJson);
            }
            catch (Exception ex)
            {
                Debug.Log($"ERROR: Cannot save default files to {configPath}, reason: {ex.Message}");
            }
        }
        catch (FileNotFoundException)
        {
            Debug.Log($"WARNING: Configurations {name}.json cannot be found, recreating file from default");
            Debug.Log("Loading default configurations...");
            // Load default configurations
            loadedConfig = api.Assets.Get(new AssetLocation(defaultDirectory)).ToObject<Dictionary<string, object>>();

            Debug.Log($"Configurations loaded, saving configs in: {configPath}");
            try
            {
                // Saving default configurations
                string defaultJson = JsonConvert.SerializeObject(loadedConfig, Formatting.Indented);
                File.WriteAllText(configPath, defaultJson);
            }
            catch (Exception ex)
            {
                Debug.Log($"ERROR: Cannot save default files to {configPath}, reason: {ex.Message}");
            }

        }
        catch (Exception ex)
        {
            Debug.Log($"ERROR: Cannot read the Configurations: {ex.Message}");
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
                if (value is null) Debug.Log("CONFIGURATION ERROR: healthTierPerHealth is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: healthTierPerHealth is not int is {value.GetType()}");
                else healthTierPerHealth = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: healthTierPerHealth not set");
        }
        { //damageTierPerDamage
            if (baseConfigs.TryGetValue("damageTierPerDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: damageTierPerDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: damageTierPerDamage is not int is {value.GetType()}");
                else damageTierPerDamage = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: damageTierPerDamage not set");
        }
        { //levelPerDamage
            if (baseConfigs.TryGetValue("levelPerDamage", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: levelPerDamage is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: levelPerDamage is not int is {value.GetType()}");
                else levelPerDamage = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: levelPerDamage not set");
        }
        { //levelPerHealth
            if (baseConfigs.TryGetValue("levelPerHealth", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: levelPerHealth is null");
                else if (value is not long) Debug.Log($"CONFIGURATION ERROR: levelPerHealth is not int is {value.GetType()}");
                else levelPerHealth = (int)(long)value;
            else Debug.Log("CONFIGURATION ERROR: levelPerHealth not set");
        }
        { //enableExtendedLogs
            if (baseConfigs.TryGetValue("enableExtendedLogs", out object value))
                if (value is null) Debug.Log("CONFIGURATION ERROR: enableExtendedLogs is null");
                else if (value is not bool) Debug.Log($"CONFIGURATION ERROR: enableExtendedLogs is not boolean is {value.GetType()}");
                else enableExtendedLogs = (bool)value;
            else Debug.Log("CONFIGURATION ERROR: enableExtendedLogs not set");
        }
    }
    #endregion
}
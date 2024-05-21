using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using ConfigSync;
using HarmonyLib;
using System;

namespace ConfigSync_Demo
{
    [ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, true)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency(ConfigSync.MyPluginInfo.PLUGIN_GUID)]
    public class ConfigSync_Demo : BaseUnityPlugin
    {
        internal static ConfigEntry<int>? BepInExConfig;
        public static ConfigSync_Demo Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }
        internal static int Value = new Random().Next(1, 20);
        internal static Configuration? SyncedConfig;
        internal int ConfigValue;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            BepInExConfig = Config.Bind("General", "ConfigDemo", Value,
            "Random number.");

            Configuration SyncedConfig = new Configuration("ConfigDemo", "ConfigSync_ConfigDemo", BepInExConfig.Value);
            // You can also use a getter (int SyncedConfigValue => (int)SyncedConfig.CurrentValue;) so the variable automatically updates, or you can use SyncedConfig.CurrentValue directly
            int SyncedConfigValue = (int)SyncedConfig.CurrentValue;

            BepInExConfig.SettingChanged += delegate
            {
                SyncedConfig.SetValue(BepInExConfig.Value);
                Logger.LogWarning("BepInExConfig value changed");
            };

            SyncedConfig.ConfigChanged += delegate
            {
                Logger.LogWarning($"SyncedConfig value changed {(int)SyncedConfig.CurrentValue}");
                SyncedConfigValue = (int)SyncedConfig.CurrentValue;
            };
            
            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        internal static void Patch()
        {
            Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

            Logger.LogDebug("Patching...");

            Harmony.PatchAll();

            Logger.LogDebug("Finished patching!");
        }

        internal static void Unpatch()
        {
            Logger.LogDebug("Unpatching...");

            Harmony?.UnpatchSelf();

            Logger.LogDebug("Finished unpatching!");
        }
    }
}

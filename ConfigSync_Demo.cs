using BepInEx;
using BepInEx.Logging;
using ConfigSync;
using ContentSettings.API;
using ContentSettings.API.Attributes;
using ContentSettings.API.Settings;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zorro.Settings;

namespace ConfigSync_Demo
{
    [ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, true)]
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    [BepInDependency(ConfigSync.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(ContentSettings.MyPluginInfo.PLUGIN_GUID, BepInDependency.DependencyFlags.HardDependency)]
    public class ConfigSync_Demo : BaseUnityPlugin
    {
        public static ConfigSync_Demo Instance { get; private set; } = null!;
        internal new static ManualLogSource Logger { get; private set; } = null!;
        internal static Harmony? Harmony { get; set; }

        internal static int DefaultValue = new System.Random().Next(1, 20);
        internal static Configuration SyncedConfig = new Configuration("ConfigDemo", "ConfigSync_ConfigDemo", DefaultValue);
        internal int SyncedConfigValue = (int)SyncedConfig.CurrentValue;

        private void Awake()
        {
            Logger = base.Logger;
            Instance = this;

            SyncedConfig.ConfigChanged += delegate
            {
                Logger.LogWarning($"SyncedConfig value changed: {(int)SyncedConfig.CurrentValue}");
                SyncedConfigValue = (int)SyncedConfig.CurrentValue;
            };

            SettingsLoader.RegisterSetting("Modded", "ConfigSyncDemo", new ModFeatureSetting());

            Patch();

            Logger.LogInfo($"{MyPluginInfo.PLUGIN_GUID} v{MyPluginInfo.PLUGIN_VERSION} has loaded!");
        }

        public class ModFeatureSetting : FloatSetting, ICustomSetting
        {
            public override void ApplyValue()
            {
                SyncedConfig!.SetValue(Mathf.RoundToInt(Value));
                Logger.LogWarning("ContentSetting value changed");
            }

            public override float GetDefaultValue()
            {
                return DefaultValue;
            }

            public string GetDisplayName()
            {
                return "ConfigDemo";
            }

            public override Unity.Mathematics.float2 GetMinMaxValue()
            {
                return new(1f, 20f);
            }

            public override float Clamp(float value)
            {
                return Mathf.RoundToInt(base.Clamp(value));
            }

            public override string Expose(float result)
            {
                return Mathf.RoundToInt(result).ToString();
            }
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

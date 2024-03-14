using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace RedGuyMod.Modules
{
    internal static class Config
    {
        public static ConfigFile myConfig;

        public static ConfigEntry<KeyboardShortcut> restKey;
        public static ConfigEntry<KeyboardShortcut> tauntKey;
        public static ConfigEntry<KeyboardShortcut> danceKey;

        public static ConfigEntry<bool> centeredBloodWell;
        public static ConfigEntry<bool> oldBloodWell;
        public static ConfigEntry<bool> badass;
        public static ConfigEntry<bool> permanentCling;
        public static ConfigEntry<bool> allySucc;
        public static ConfigEntry<bool> bossGrab;
        public static ConfigEntry<bool> permanentBossCling;
        public static ConfigEntry<bool> clingToAllies;
        public static ConfigEntry<bool> cursed;
        public static ConfigEntry<bool> useless;

        internal static void ReadConfig()
        {
            centeredBloodWell
    = Config.BindAndOptions("01 - General",
             "Centered Blood Well",
             false,
             "Puts the Blood Well gauge on top of the crosshair. Does nothing if Old Blood Well is enabled. Requires run restart.");

            oldBloodWell
                = Config.BindAndOptions("01 - General",
             "Old Blood Well",
             false,
             "Uses the old Blood Well gauge below the health bar. Requires run restart.");

            badass
= Config.BindAndOptions("01 - General",
"Badass Mode",
false,
"If set to true, Ravager becomes a total BadAss when selected.");

            permanentCling
= Config.BindAndOptions("01 - General",
"Permanent Cling",
false,
"If set to true, you can cling onto walls indefinitely.");

            allySucc
= Config.BindAndOptions("01 - General",
"Allow Ally Projectile Suck",
true,
"If set to false, Nullify stops absorbing ally projectiles.");

            bossGrab
= Config.BindAndOptions("01 - General",
"Allow Boss Grab",
true,
"If set to false, empowered Brutalize will no longer grab bosses.");

            permanentBossCling
= Config.BindAndOptions("01 - General",
"Permanent Boss Cling",
false,
"If set to true, Brutalize will cling onto bosses indefinitely.");

/*            clingToAllies
= Config.BindAndOptions("01 - General",
"Cling To Allies",
false,
"If set to true, Brutalize will cling to any allies it hits.");*/

            cursed
= Config.BindAndOptions("01 - General",
"Cursed",
false,
"Enables unfinished, stupid and old content.", true);

            useless
= Config.BindAndOptions("01 - General",
 "Useless",
 true,
 "Don't change this.");

            #region Emotes
            restKey
                = Config.BindAndOptions("02 - Keybinds",
                         "Rest Emote",
                         new KeyboardShortcut(KeyCode.Alpha1),
                         "Key used to Rest");
            tauntKey
                = Config.BindAndOptions("02 - Keybinds",
                                     "Taunt Emote",
                                     new KeyboardShortcut(KeyCode.Alpha2),
                                     "Key used to Taunt");

            danceKey
                = Config.BindAndOptions("02 - Keybinds",
                                     "Dance Emote",
                                     new KeyboardShortcut(KeyCode.Alpha3),
                                     "Key used to Dance");
            #endregion
        }

        public static void InitROO(Sprite modSprite, string modDescription)
        {
            if (MainPlugin.rooInstalled) _InitROO(modSprite, modDescription);
        }

        public static void _InitROO(Sprite modSprite, string modDescription)
        {
            ModSettingsManager.SetModIcon(modSprite);
            ModSettingsManager.SetModDescription(modDescription);
        }

        public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, string description = "", bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<T> configEntry = myConfig.Bind(section, name, defaultValue, description);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                TryRegisterOption(configEntry, restartRequired);
            }

            return configEntry;
        }

        public static ConfigEntry<float> BindAndOptionsSlider(string section, string name, float defaultValue, string description = "", float min = 0, float max = 20, bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<float> configEntry = myConfig.Bind(section, name, defaultValue, description);

            if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            {
                TryRegisterOptionSlider(configEntry, min, max, restartRequired);
            }

            return configEntry;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOption<T>(ConfigEntry<T> entry, bool restartRequired)
        {
            if (entry is ConfigEntry<float>)
            {
                ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig() { min = 0, max = 20, formatString = "{0:0.00}", restartRequired = restartRequired }));
            }
            if (entry is ConfigEntry<int>)
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, restartRequired));
            }
            if (entry is ConfigEntry<bool>)
            {
                ModSettingsManager.AddOption(new CheckBoxOption(entry as ConfigEntry<bool>, restartRequired));
            }
            if (entry is ConfigEntry<KeyboardShortcut>)
            {
                ModSettingsManager.AddOption(new KeyBindOption(entry as ConfigEntry<KeyboardShortcut>, restartRequired));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOptionSlider(ConfigEntry<float> entry, float min, float max, bool restartRequired)
        {
            ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig() { min = min, max = max, formatString = "{0:0.00}", restartRequired = restartRequired }));
        }

        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return Config.BindAndOptions("01 - General",
                         "Enabled",
                         true,
                         "Set to false to disable this character", true);
        }

        internal static ConfigEntry<bool> ForceUnlockConfig(string characterName)
        {
            return Config.BindAndOptions("01 - General",
                         "Force Unlock",
                         false,
                         "Makes this character unlocked by default", true);
        }

        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }


    public class StageSpawnInfo 
    {
        private string stageName;
        private int minStages;

        public StageSpawnInfo(string stageName, int minStages) {
            this.stageName = stageName;
            this.minStages = minStages;
        }

        public string GetStageName() { return stageName; }
        public int GetMinStages() { return minStages; }
    }
}
﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BetterUI.GameClasses;

using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace BetterUI
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations]

        public const string
          MODNAME = "BetterUI",
          AUTHOR = "MK",
          GUID = AUTHOR + "_" + MODNAME,
          VERSION = "2.3.0";


        internal static ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;

        
        // Player HUD
        public static ConfigEntry<bool> enablePlayerHudEditing;
        public static ConfigEntry<KeyCode> togglePlayerHudEditModeKey;
        public static ConfigEntry<KeyCode> modKeyPrimary;
        public static ConfigEntry<KeyCode> modKeySecondary;
        public static ConfigEntry<bool> useCustomHealthBar;
        public static ConfigEntry<bool> useCustomStaminaBar;
        public static ConfigEntry<bool> useCustomEitrBar;
        public static ConfigEntry<bool> useCustomFoodBar;
        public static ConfigEntry<int> customHealthBarRotation;
        public static ConfigEntry<int> customStaminaBarRotation;
        public static ConfigEntry<int> customSpoilerBarRotation;
        public static ConfigEntry<int> customFoodBarRotation;

        // Player Inventory
        public static ConfigEntry<bool> showDurabilityColor;
        public static ConfigEntry<int> durabilityColorPalette;
        public static ConfigEntry<bool> showItemStars;
        public static ConfigEntry<bool> showCustomCharInfo;
        public static ConfigEntry<bool> showCustomTooltips;
        public static ConfigEntry<bool> showCombinedItemStats;
        public static ConfigEntry<float> iconScaleSize;

        // Skill UI
        public static ConfigEntry<bool> customSkillUI;
        public static ConfigEntry<int> skillUITextSize;

        // HoverTexts
        public static ConfigEntry<int> timeLeftStyleFermenter;
        public static ConfigEntry<int> timeLeftStylePlant;
        public static ConfigEntry<int> timeLeftStyleCookingStation;
        public static ConfigEntry<int> chestHasRoomStyle;
        
        // Character XP
        public static ConfigEntry<bool> showCharacterXP;
        public static ConfigEntry<bool> showCharacterXpBar;
        public static ConfigEntry<bool> showXPNotifications;
        public static ConfigEntry<bool> extendedXPNotification;
        public static ConfigEntry<int> notificationTextSizeXP;

        // Enemy HUD
        public static ConfigEntry<bool> customEnemyHud;
        public static ConfigEntry<bool> showEnemyHPText;
        public static ConfigEntry<int> enemyLvlStyle;
        public static ConfigEntry<int> enemyNameTextSize;
        public static ConfigEntry<int> enemyHPTextSize;
        public static ConfigEntry<int> playerHPTextSize;
        public static ConfigEntry<bool> showPlayerHPText;
        public static ConfigEntry<bool> showLocalPlayerEnemyHud;
        public static ConfigEntry<int> bossHPTextSize;
        public static ConfigEntry<bool> makeTamedHPGreen;
        public static ConfigEntry<float> maxShowDistance;
        public static ConfigEntry<bool> useCustomAlertedStatus;

        // Map        
        public static ConfigEntry<float> mapPinScaleSize;
        
        // xUIData
        public static ConfigEntry<string> uiData;

        // Debug
        public static ConfigEntry<bool> isDebug;

        #endregion


        public Main()
        {
            log = Logger;
            harmony = new Harmony(GUID);
            assembly = Assembly.GetExecutingAssembly();
        }
        public void Awake()
        {
            string sectionName;

            // Don't be tempted to rename the sections. That resets all its config values for every user
            // The same goes for moving items between sections
            // That's also why there is no section number counter


            // Player HUD
            sectionName = "1 - Player HUD";

            togglePlayerHudEditModeKey = Config.Bind(sectionName, nameof(togglePlayerHudEditModeKey), KeyCode.F7,
                "Key used to toggle Player HUD editing mode. Accepted values: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            modKeyPrimary = Config.Bind(sectionName, nameof(modKeyPrimary), KeyCode.Mouse0,
                "Key needed to hold down to change HUD position. Accepted values: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            modKeySecondary = Config.Bind(sectionName, nameof(modKeySecondary), KeyCode.LeftControl,
                "Key needed to hold down to change element dimensions. Accepted Values: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            // Player HUD RESTART
            sectionName = "1 - Player HUD (Requires Logout)";

            enablePlayerHudEditing = Config.Bind(sectionName, nameof(enablePlayerHudEditing), true, "Enable the ability to edit the Player HUD by pressing a hotkey.");

            useCustomHealthBar = Config.Bind(sectionName, nameof(useCustomHealthBar), false, $"Resizable, rotatable HP bar. This bar will always be the same size and will not scale when you eat. Will also disable the default food bar, so use {nameof(useCustomFoodBar)}.");
            
            useCustomStaminaBar = Config.Bind(sectionName, nameof(useCustomStaminaBar), false, "Resizable, rotatable Stamina bar. This bar will always be visible and will not scale when you eat.");

            useCustomEitrBar = Config.Bind(sectionName, nameof(useCustomEitrBar), false, "Resizable, rotatable bar for the new Eitr resource. This bar will always be visible and will not scale when you eat.");

            useCustomFoodBar = Config.Bind(sectionName, nameof(useCustomFoodBar), false, $"Resizable, rotatable Food Bar. Requires {nameof(useCustomHealthBar)}.");

            customHealthBarRotation = Config.Bind(sectionName, nameof(customHealthBarRotation), 90, "Rotate healthbar in degrees.");

            customStaminaBarRotation = Config.Bind(sectionName, nameof(customStaminaBarRotation), 90, "Rotate staminabar in degrees.");

            customSpoilerBarRotation = Config.Bind(sectionName, nameof(customSpoilerBarRotation), 90, "Rotate bar for the new spoiler resource in degrees.");

            customFoodBarRotation = Config.Bind(sectionName, nameof(customFoodBarRotation), 180, "Rotate foodbar in degrees.");


            // Character Inventory
            sectionName = "2 - Character Inventory";

            showDurabilityColor = Config.Bind(sectionName, nameof(showDurabilityColor), true, "Show colored durability bars for items.");

            durabilityColorPalette = Config.Bind(sectionName, nameof(durabilityColorPalette), 0, "Change Durabilty bar colors. Options: 0 = Green, Yellow, Orange, Red, 1 = White, Light Yellow, Light Cyan, Blue.");

            showItemStars = Config.Bind(sectionName, nameof(showItemStars), true, "Show item quality as stars.");

            showCustomCharInfo = Config.Bind(sectionName, nameof(showCustomCharInfo), true, "Show Deaths, Builds, and Crafts stats on character selection screen. Also shows the Kills stat if something increases it.");

            showCustomTooltips = Config.Bind(sectionName, nameof(showCustomTooltips), true, "Show more info on inventory item tooltips. Disable this is using Epic Loot.");

            showCombinedItemStats = Config.Bind(sectionName, nameof(showCombinedItemStats), true, "Show all item stats when mouse is hovered over armor amount.");

            iconScaleSize = Config.Bind(sectionName, nameof(iconScaleSize), 0.75f, "Scale item icon by this factor. Ex. 0.75 makes them 75% of original size.");


            // Skills UI
            sectionName = "3 - Character Skills";

            customSkillUI = Config.Bind(sectionName, nameof(customSkillUI), false, "Toggle the use of custom skills UI.");

            skillUITextSize = Config.Bind(sectionName, nameof(skillUITextSize), 14, "Select text size on skills UI.");


            // Hover Text
            sectionName = "4 - Hover Text";

            timeLeftStyleFermenter = Config.Bind(sectionName, nameof(timeLeftStyleFermenter), 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left.");

            timeLeftStylePlant = Config.Bind(sectionName, nameof(timeLeftStylePlant), 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left.");

            timeLeftStyleCookingStation = Config.Bind(sectionName, nameof(timeLeftStyleCookingStation), 2, "Select duration display. 0 = Default, 1= % Done, 2 = min:sec left.");

            chestHasRoomStyle = Config.Bind(sectionName, nameof(chestHasRoomStyle), 2, "Select how chest emptyness is displayed. 0 = Default | 1 = % | 2 = items / max_room. | 3 = free slots.");


            // Character XP
            sectionName = "5 - Character XP";

            showCharacterXP = Config.Bind(sectionName, nameof(showCharacterXP), true, "Enable Character XP. This combines all skill levels to show overall character progress.");

            showXPNotifications = Config.Bind(sectionName, nameof(showXPNotifications), true, "Show whenever you gain xp from actions.");

            notificationTextSizeXP = Config.Bind(sectionName, nameof(notificationTextSizeXP), 14, "XP notification font size.");

            extendedXPNotification = Config.Bind(sectionName, nameof(extendedXPNotification), true, "Extend notification with: (xp gained) [current/overall xp].");

            // Character XP RESTART
            sectionName = "5 - Character XP (Requires Logout)";

            showCharacterXpBar = Config.Bind(sectionName, nameof(showCharacterXpBar), true, "Show Character XP Bar on the bottom of the screen. Character XP must be enabled.");


            // Enemy HUD
            sectionName = "6 - Enemy HUD";

            customEnemyHud = Config.Bind(sectionName, nameof(customEnemyHud), true, "Enable custom enemy HUD changes. If this is set to false, all options in this section will be disabled.");

            useCustomAlertedStatus = Config.Bind(sectionName, nameof(useCustomAlertedStatus), true, "Hide the vanilla alerted icons above the enemy health bar and instead change the color of the name based on the alerted status.");

            showEnemyHPText = Config.Bind(sectionName, nameof(showEnemyHPText), true, "Show the text with HP amount on enemies health bar.");

            enemyLvlStyle = Config.Bind(sectionName, nameof(enemyLvlStyle), 0, "Choose how enemy lvl is shown. 0 = Default (stars) | 1 = Prefix before name (Lv. 1) | 2 = Both.");

            enemyNameTextSize = Config.Bind(sectionName, nameof(enemyNameTextSize), 14, "Font size of the name on the enemy.");

            enemyHPTextSize = Config.Bind(sectionName, nameof(enemyHPTextSize), 10, "Font size of the HP text on the enemy health bar.");

            showPlayerHPText = Config.Bind(sectionName, nameof(showPlayerHPText), true, "Show the health numbers on other player's health bar in Multiplayer.");

            showLocalPlayerEnemyHud = Config.Bind(sectionName, nameof(showLocalPlayerEnemyHud), false, "Show the EnemyHud/HealthBar for your player.");
            showLocalPlayerEnemyHud.SettingChanged += (_, _) => BetterEnemyHud.ShowLocalPlayerEnemyHudConfigChanged();

            playerHPTextSize = Config.Bind(sectionName, nameof(playerHPTextSize), 10, "The size of the font to display on other player's health bar in Multiplayer.");

            bossHPTextSize = Config.Bind(sectionName, nameof(bossHPTextSize), 14, "The size of the font to display on the Boss's health bar.");

            makeTamedHPGreen = Config.Bind(sectionName, nameof(makeTamedHPGreen), true, "Make the health bar for tamed creatures green instead of red.");

            maxShowDistance = Config.Bind(sectionName, nameof(maxShowDistance), 1f, "How far you will see enemy HP Bar. This is an multiplier, 1 = game default, 2 = 2x default.");


            // Map
            sectionName = "7 - Map";

            mapPinScaleSize = Config.Bind(sectionName, nameof(mapPinScaleSize), 1f, "Scale map pins by this factor. Ex. 1.5 makes the 150% of original size.");


            // Debug
            sectionName = "8 - Debug";

            isDebug = Config.Bind(sectionName, nameof(isDebug), false, "Enable debug logging.");


            // xDataUI
            sectionName = "9 - xDataUI";
            uiData = Config.Bind(sectionName, nameof(uiData), "none", "This is your customized UI info. Edit to none, if having issues or wanting to reset positions.");
        }
        public void Start()
        {
            harmony.PatchAll(assembly);
        }

        /*
        public void OnDestroy()
        {
          harmony?.UpatchSelf();
        }
        */
    }
}

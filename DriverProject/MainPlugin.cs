using System;
using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using UnityEngine;
using R2API.Networking;

[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]

namespace RedGuyMod
{
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ContactLight.LostInTransit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    [R2APISubmoduleDependency(new string[]
    {
        "PrefabAPI",
        "LanguageAPI",
        "SoundAPI",
        "DirectorAPI",
        "LoadoutAPI",
        "UnlockableAPI",
        "NetworkingAPI",
        "RecalculateStatsAPI",
    })]

    public class MainPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.rob.RedGuy";
        public const string MODNAME = "RedGuy";
        public const string MODVERSION = "1.0.0";

        public const string developerPrefix = "ROB";

        public static MainPlugin instance;

        public static bool starstormInstalled;
        public static bool scepterInstalled;
        public static bool rooInstalled;
        public static bool litInstalled;

        private void Awake()
        {
            instance = this;

            Modules.Config.myConfig = Config;

            starstormInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2");
            litInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ContactLight.LostInTransit");
            scepterInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
            rooInstalled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");

            Log.Init(Logger);
            Modules.Config.ReadConfig();
            Modules.Assets.PopulateAssets();
            Modules.CameraParams.InitializeParams();
            //Modules.States.RegisterStates();
            //Modules.Buffs.RegisterBuffs();
            Modules.Projectiles.RegisterProjectiles();
            //Modules.Tokens.AddTokens();
            Modules.ItemDisplays.PopulateDisplays();
            //Modules.NetMessages.RegisterNetworkMessages();

            new Content.Survivors.RedGuy().CreateCharacter();

            Hook();

            new Modules.ContentPacks().Initialize();

            RoR2.ContentManagement.ContentManager.onContentPacksAssigned += LateSetup;
        }

        private void LateSetup(global::HG.ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {
            Modules.Survivors.Driver.SetItemDisplays();
        }

        private void Hook()
        {
            //R2API.RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            //On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, R2API.RecalculateStatsAPI.StatHookEventArgs args) {

            /*if (sender.HasBuff(Modules.Buffs.armorBuff)) {

                args.armorAdd += 500f;
            }

            if (sender.HasBuff(Modules.Buffs.slowStartBuff)) {

                args.armorAdd += 20f;
                args.moveSpeedReductionMultAdd += 1f; //movespeed *= 0.5f // 1 + 1 = divide by 2?
                args.attackSpeedMultAdd -= 0.5f; //attackSpeed *= 0.5f;
                args.damageMultAdd -= 0.5f; //damage *= 0.5f;
            }*/
        }

        public static float GetICBMDamageMult(CharacterBody body)
        {
            float mult = 1f;
            if (body && body.inventory)
            {
                int itemcount = body.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                int stack = itemcount - 1;
                if (stack > 0) mult += stack * 0.5f;
            }
            return mult;
        }

        public static bool CheckIfBodyIsTerminal(CharacterBody body)
        {
            if (MainPlugin.starstormInstalled) return _CheckIfBodyIsTerminal(body);
            return false;
        }

        public static bool _CheckIfBodyIsTerminal(CharacterBody body)
        {
            return body.HasBuff(Moonstorm.Starstorm2.SS2Content.Buffs.BuffTerminationReady);
        }
    }
}
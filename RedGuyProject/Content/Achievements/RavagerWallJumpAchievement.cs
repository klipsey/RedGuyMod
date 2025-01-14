﻿using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;

namespace RedGuyMod.Modules.Achievements
{
    internal class RavagerWallJumpAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_ACHIEVEMENT_ID";
        public override string UnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_REWARD_ID";
        public override string UnlockableNameToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBlinkIcon");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_WALLJUMP_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RobRavagerBody");
        }

        public override void OnInstall()
        {
            base.OnInstall();
            RedGuyMod.Content.Components.RedGuyController.onWallJumpIncremented += Check;
        }

        private void Check(int count)
        {
            if (count >= 20)
            {
                if (base.meetsBodyRequirement)
                {
                    base.Grant();
                }
            }
        }

        public override void OnUninstall()
        {
            base.OnUninstall();
            RedGuyMod.Content.Components.RedGuyController.onWallJumpIncremented -= Check;
        }
    }
}
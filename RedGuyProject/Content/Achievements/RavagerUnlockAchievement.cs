using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;

namespace RedGuyMod.Modules.Achievements
{
    internal class RavagerUnlockAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_ACHIEVEMENT_ID";
        public override string UnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = "";
        public override string UnlockableNameToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRavagerAchievement");

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));

        private void Check(CharacterBody characterBody)
        {
            if (Run.instance is null) return;

            if (Run.instance.stageClearCount >= 2 && Run.instance.time <= 900f)
            {
                base.Grant();
            }
        }

        public override void OnInstall()
        {
            base.OnInstall();

            CharacterBody.onBodyStartGlobal += Check;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            CharacterBody.onBodyStartGlobal -= Check;
        }
    }
}
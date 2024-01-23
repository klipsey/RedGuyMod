using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RedGuyMod.Modules.Achievements
{
    internal class RavagerVoidAchievement : ModdedUnlockable
    {
        public override string AchievementIdentifier { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_ACHIEVEMENT_ID";
        public override string UnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_REWARD_ID";
        public override string AchievementNameToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_ACHIEVEMENT_NAME";
        public override string PrerequisiteUnlockableIdentifier { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_UNLOCKABLE_REWARD_ID";
        public override string UnlockableNameToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_UNLOCKABLE_NAME";
        public override string AchievementDescToken { get; } = MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_ACHIEVEMENT_DESC";
        public override Sprite Sprite { get; } = Addressables.LoadAssetAsync<SkinDef>("RoR2/DLC1/VoidSurvivor/skinVoidSurvivorDefault.asset").WaitForCompletion().icon;

        public override Func<string> GetHowToUnlock { get; } = (() => Language.GetStringFormatted("UNLOCK_VIA_ACHIEVEMENT_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));
        public override Func<string> GetUnlocked { get; } = (() => Language.GetStringFormatted("UNLOCKED_FORMAT", new object[]
                            {
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_ACHIEVEMENT_NAME"),
                                Language.GetString(MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_UNLOCKABLE_ACHIEVEMENT_DESC")
                            }));

        public override BodyIndex LookUpRequiredBodyIndex()
        {
            return BodyCatalog.FindBodyIndex("RobRavagerBody");
        }

        public override void OnInstall()
        {
            base.OnInstall();
            Run.onClientGameOverGlobal += Check;
        }

        public void Check(Run run, RunReport runReport)
        {
            if (run is null) return;
            if (runReport is null) return;

            if (!runReport.gameEnding) return;

            if (runReport.gameEnding.isWin)
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "voidraid")
                {
                    if (base.meetsBodyRequirement)
                    {
                        base.Grant();
                    }
                }
            }
        }

        public override void OnUninstall()
        {
            base.OnUninstall();
            Run.onClientGameOverGlobal -= Check;
        }
    }
}
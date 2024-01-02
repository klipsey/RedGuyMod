using R2API;
using System;

namespace RedGuyMod.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string prefix = MainPlugin.developerPrefix + "_RAVAGER_BODY_";

            string desc = "The Ravager is an agile melee bruiser who heals off slain foes to stay in the thick of it.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Charging wall jumps is the best way to deal with flying enemies." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Cleave is handy for cancelling high damage attacks as well as keeping up the damage output." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Coagulate can be helpful in a pinch if you're not sure you can get the Blood Well filled, but it can also be used passively to stay topped off." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Brutalize heals a ton on demand so be sure to grab and consume something if you're getting low." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, leaving behind an incomprehensible amount of bloodshed.";
            string outroFailure = "..and so he vanished, his ridiculous violence coming to an end.";

            string lore = "The Ravager was not born with a special power...\n\n";
            lore += "...instead, he was born with a Cursed Bloodspilled Katana but one day he went to the Poop Dimension and was met with an evil power.....\n\n";
            lore += "Finally escaped from the Poop Dimension and wielding this awesome evil power himself he has come in the Petrichor V for destroid the universe\n";

            LanguageAPI.Add(prefix + "NAME", "Ravager");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Malformed Brute");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MONSOON_SKIN_NAME", "Divine");
            LanguageAPI.Add(prefix + "TYPHOON_SKIN_NAME", "");
            LanguageAPI.Add(prefix + "MINECRAFT_SKIN_NAME", "Minecraft");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Physical Prowess");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"The Ravager can <style=cIsUtility>jump off walls</style> and <style=cIsHealth>enemies</style>. Wall jumps can be charged for a mighty leap.");

            LanguageAPI.Add(prefix + "CONFIRM_NAME", "Confirm");
            LanguageAPI.Add(prefix + "CONFIRM_DESCRIPTION", "Proceed with the current skill.");

            LanguageAPI.Add(prefix + "CANCEL_NAME", "Cancel");
            LanguageAPI.Add(prefix + "CANCEL_DESCRIPTION", "Cancel the current skill.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Hack");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", $"Swing for <style=cIsDamage>{100f * SkillStates.Ravager.Slash._damageCoefficient}% damage</style>. Fills up the <style=cIsHealth>Blood Well</style> on hit, draining when full to heal you for <style=cIsHealing>75% max health</style> and <style=cIsDamage>augment your skills</style> temporarily.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SPINSLASH_NAME", "Cleave");
            LanguageAPI.Add(prefix + "SECONDARY_SPINSLASH_DESCRIPTION", $"Perform a wide, <style=cIsUtility>stunning</style> slash for <style=cIsDamage>{100f * SkillStates.Ravager.SpinSlash._damageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_BEAM_NAME", "Eldritch Blast");
            LanguageAPI.Add(prefix + "UTILITY_BEAM_DESCRIPTION", $"<style=cIsHealth>Absorb</style> incoming <style=cIsUtility>projectiles</style> to charge up a devastating blast that deals up to <style=cIsDamage>{100f * SkillStates.Ravager.ChargeBeam.maxDamageCoefficient}% damage</style> when fully charged.");

            LanguageAPI.Add(prefix + "UTILITY_HEAL_NAME", "Coagulate");
            LanguageAPI.Add(prefix + "UTILITY_HEAL_DESCRIPTION", "Instantly drain the <style=cIsHealth>Blood Well</style>, <style=cIsHealing>healing yourself</style> with the blood consumed.");

            LanguageAPI.Add(prefix + "UTILITY_SNATCH_NAME", "Snatch");
            LanguageAPI.Add(prefix + "UTILITY_SNATCH_DESCRIPTION", "Send forth your <style=cIsDamage>imp arm</style> and grasp a distant enemy within its clutches, <style=cIsUtility>pulling yourself toward whatever it hits</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_GRAB_NAME", "Brutalize");
            LanguageAPI.Add(prefix + "SPECIAL_GRAB_DESCRIPTION", $"Lunge and <style=cIsUtility>grab</style> nearby enemies, then slam down for <style=cIsDamage>{100f * SkillStates.Ravager.DashGrab.groundSlamDamageCoefficient}% damage</style>. If this kills, <style=cIsHealth>consume</style> them to restore <style=cIsHealing>30% max health</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_GRAB_SCEPTER_NAME", "Vicious Assault");
            //LanguageAPI.Add(prefix + "SPECIAL_GRAB_SCEPTER_DESCRIPTION", $"Throw a grenade that <style=cIsUtility>stuns</style> enemies for <style=cIsDamage>{100f * SkillStates.Driver.ThrowGrenade.damageCoefficient}% damage</style>. <style=cIsUtility>You can hold up to two.</style>" + Helpers.ScepterDescription("Throw a molotov that bursts into flames instead."));

            LanguageAPI.Add(prefix + "SPECIAL_TRANSFIGURE_NAME", "Brutalize");
            LanguageAPI.Add(prefix + "SPECIAL_GRAB_DESCRIPTION", $"Lunge and <style=cIsUtility>grab</style> nearby enemies, then slam down for <style=cIsDamage>{100f * SkillStates.Ravager.DashGrab.groundSlamDamageCoefficient}% damage</style>. If this kills, <style=cIsHealth>consume</style> them to restore <style=cIsHealing>30% max health</style>.");

            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_REDGUY_M12", "<style=cKeywordName>Hold Stance</style><style=cSub>While charging a wall jump, enter a <style=cIsUtility>hold stance</style>. Release to swing for up to <style=cIsDamage>4x damage</style> based on velocity.");
            LanguageAPI.Add("KEYWORD_REDGUY_M1", "<style=cKeywordName>Empowered Effect</style><style=cSub>Swings faster.");
            LanguageAPI.Add("KEYWORD_REDGUY_M2", "<style=cKeywordName>Empowered Effect</style><style=cSub>Deals more damage to <style=cIsUtility>low health enemies</style>.");
            LanguageAPI.Add("KEYWORD_REDGUY_HEAL", $"<style=cKeywordName>Empowered Effect</style><style=cSub>Explodes for up to <style=cIsDamage>{100f * SkillStates.Ravager.Heal.maxDamageCoefficient}% damage</style>.");
            LanguageAPI.Add("KEYWORD_REDGUY_GRAB", "<style=cKeywordName>Empowered Effect</style><style=cSub>Drags enemy along the ground, dealing damage over time.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "Paint It Red");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "Paint It Red");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Apply 50 stacks of Bleed on one enemy.");

            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_UNLOCKABLE_NAME", "Ravager: Mastery");
            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: Mastery");
            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, beat the game or obliterate on Monsoon.");

            LanguageAPI.Add(prefix + "TYPHOON_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: Grand Mastery");
            LanguageAPI.Add(prefix + "TYPHOON_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: Grand Mastery");
            LanguageAPI.Add(prefix + "TYPHOON_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, beat the game or obliterate on Typhoon or Eclipse.\n<color=#8888>(Counts any difficulty Typhoon or higher)</color>");
            //Ravager: Hungry
            //As Ravager, consume 8 enemies with one grab.
            LanguageAPI.Add(prefix + "BEAM_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: In a Trail of Fire");
            LanguageAPI.Add(prefix + "BEAM_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: In a Trail of Fire");
            LanguageAPI.Add(prefix + "BEAM_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, slam an enemy into the ground 5+ times in one grab.");

            LanguageAPI.Add(prefix + "SUIT_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: All My Fellas");
            LanguageAPI.Add(prefix + "SUIT_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: All My Fellas");
            LanguageAPI.Add(prefix + "SUIT_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, have 15 allies alive at once.");
            #endregion
        }
    }
}
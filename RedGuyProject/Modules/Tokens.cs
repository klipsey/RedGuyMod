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
            desc = desc + "< ! > Brutalize heals on demand so be sure to grab and consume something if you're getting low." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, in search of new lands to desecrate.";
            string outroFailure = "..and so he vanished, his ridiculous violence finally coming to an end.";

            string lore = "The Ravager was not born with a special power...\n\n";
            lore += "...instead, he was born with a Cursed Bloodspilled Katana but one day he went to the Poop Dimension and was met with an evil power.....\n\n";
            lore += "Finally escaped from the Poop Dimension and wielding this awesome evil power himself he has come in the Petrichor V for destroid the universe\n";

            LanguageAPI.Add(prefix + "NAME", "Ravager");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Callous Usurper");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MONSOON_SKIN_NAME", "Voidtouched");
            LanguageAPI.Add(prefix + "MONSOON_ALTERNATE_SKIN_NAME", "Voidtouched EX");
            LanguageAPI.Add(prefix + "TYPHOON_SKIN_NAME", "");
            LanguageAPI.Add(prefix + "VOID_SKIN_NAME", "Voidborn");
            LanguageAPI.Add(prefix + "MAHORAGA_SKIN_NAME", "Divine");
            LanguageAPI.Add(prefix + "MINECRAFT_SKIN_NAME", "Minecraft");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "BLOODWELL_NAME", "Blood Well");
            LanguageAPI.Add(prefix + "BLOODWELL_DESCRIPTION", $"The Ravager stores up <style=cIsHealth>blood</style> with his strikes, draining when full to heal you for <style=cIsHealing>75% missing health</style> and <style=cIsDamage>empower your skills</style> temporarily.");

            LanguageAPI.Add(prefix + "BLOODWELL2_NAME", "Ichor Canister");
            LanguageAPI.Add(prefix + "BLOODWELL2_DESCRIPTION", $"The Ravager stores up <style=cIsHealth>blood</style> with his strikes, draining when full to heal you for <style=cIsHealing>100% max health</style> and <style=cIsDamage>empower your skills</style> temporarily. <style=cIsHealth>Drains faster while attacking.</style>");

            LanguageAPI.Add(prefix + "PARASITE_NAME", "The Parasite");
            LanguageAPI.Add(prefix + "PARASITE_DESCRIPTION", $"<style=cIsHealth>It hungers....</style>");

            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Physical Prowess");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"The Ravager can <style=cIsUtility>jump off walls</style> and <style=cIsHealth>enemies</style>. Wall jumps can be charged for a <style=cIsDamage>mighty leap</style>.");
            
            LanguageAPI.Add(prefix + "PASSIVE2_NAME", "Twisted Mutation");
            LanguageAPI.Add(prefix + "PASSIVE2_DESCRIPTION", $"Jump in midair to charge up a <style=cIsUtility>forward blink</style>, once per jump. Landing <style=cIsDamage>melee hits</style> refreshes this ability.");

            LanguageAPI.Add(prefix + "PASSIVE3_NAME", "Twisted Mutation (Legacy)");
            LanguageAPI.Add(prefix + "PASSIVE3_DESCRIPTION", $"<style=cIsHealth>Costs 5% current health.</style> Jump in midair to <style=cIsUtility>blink forward</style>, once per jump. Landing <style=cIsDamage>melee hits</style> refreshes this ability.");

            LanguageAPI.Add(prefix + "CONFIRM_NAME", "Confirm");
            LanguageAPI.Add(prefix + "CONFIRM_DESCRIPTION", "Proceed with the current skill.");

            LanguageAPI.Add(prefix + "CANCEL_NAME", "Cancel");
            LanguageAPI.Add(prefix + "CANCEL_DESCRIPTION", "Cancel the current skill.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_NAME", "Hack");
            LanguageAPI.Add(prefix + "PRIMARY_SLASH_DESCRIPTION", $"Swing forward for <style=cIsDamage>{100f * SkillStates.Ravager.Slash._damageCoefficient}% damage</style>. Use while <style=cIsUtility>charging a wall jump</style> to instead enter a <style=cIsDamage>hold stance</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_SLASHCOMBO_NAME", "Dismantle");
            LanguageAPI.Add(prefix + "PRIMARY_SLASHCOMBO_DESCRIPTION", $"Swing forward for <style=cIsDamage>{100f * SkillStates.Ravager.SlashCombo._damageCoefficient}% damage</style>. Every 3rd hit <style=cIsUtility>stuns</style> and deals <style=cIsDamage>{100f * SkillStates.Ravager.SlashCombo.finisherDamageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_SPINSLASH_NAME", "Cleave");
            LanguageAPI.Add(prefix + "SECONDARY_SPINSLASH_DESCRIPTION", $"Lunge forward and perform a wide, <style=cIsUtility>stunning</style> slash for <style=cIsDamage>{100f * SkillStates.Ravager.SpinSlash._damageCoefficient}% damage</style>.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_BEAM_NAME", "Nullify");
            LanguageAPI.Add(prefix + "UTILITY_BEAM_DESCRIPTION", $"<style=cIsHealth>Absorb</style> incoming <style=cIsUtility>projectiles</style> to charge up a devastating blast that deals up to <style=cIsDamage>5000% damage</style> when fully charged.");

            LanguageAPI.Add(prefix + "UTILITY_HEAL_NAME", "Coagulate");
            LanguageAPI.Add(prefix + "UTILITY_HEAL_DESCRIPTION", "Instantly drain the <style=cIsHealth>Blood Well</style>, <style=cIsHealing>healing yourself</style> with the blood consumed.");

            LanguageAPI.Add(prefix + "UTILITY_SWAP_NAME", "Boogie Woogie");
            LanguageAPI.Add(prefix + "UTILITY_SWAP_DESCRIPTION", "Switch places with <style=cIsUtility>any entity</style>.");

            LanguageAPI.Add(prefix + "UTILITY_SNATCH_NAME", "Snatch");
            LanguageAPI.Add(prefix + "UTILITY_SNATCH_DESCRIPTION", "Send forth your <style=cIsDamage>imp arm</style> and grasp a distant enemy within its clutches, <style=cIsUtility>pulling yourself toward whatever it hits</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_GRAB_NAME", "Brutalize");
            LanguageAPI.Add(prefix + "SPECIAL_GRAB_DESCRIPTION", $"Lunge and <style=cIsUtility>grab</style> nearby enemies, then slam down for <style=cIsDamage>{100f * SkillStates.Ravager.DashGrab.groundSlamDamageCoefficient}% damage</style>. If this kills, <style=cIsHealth>consume</style> them to restore <style=cIsHealing>10% max health</style>.");

            //LanguageAPI.Add(prefix + "SPECIAL_GRAB_SCEPTER_NAME", "Vicious Assault");
            //LanguageAPI.Add(prefix + "SPECIAL_GRAB_SCEPTER_DESCRIPTION", $"Throw a grenade that <style=cIsUtility>stuns</style> enemies for <style=cIsDamage>{100f * SkillStates.Driver.ThrowGrenade.damageCoefficient}% damage</style>. <style=cIsUtility>You can hold up to two.</style>" + Helpers.ScepterDescription("Throw a molotov that bursts into flames instead."));

            LanguageAPI.Add(prefix + "SPECIAL_PUNCH_NAME", "Pummel");
            LanguageAPI.Add(prefix + "SPECIAL_PUNCH_DESCRIPTION", $"Lunge and <style=cIsUtility>punch</style>, dealing <style=cIsDamage>{100f * SkillStates.Ravager.DashGrab.punchDamageCoefficient}% damage</style> with a <style=cIsUtility>shockwave</style> through them for the same damage. If this kills, <style=cIsHealth>consume</style> them to restore <style=cIsHealing>10% max health</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_THROW_NAME", "Hurl");
            LanguageAPI.Add(prefix + "SPECIAL_THROW_DESCRIPTION", $"Lunge and <style=cIsUtility>grab</style> nearby enemies, then throw them for <style=cIsDamage>{100f * SkillStates.Ravager.DashGrab.groundSlamDamageCoefficient}% damage</style> on impact. If this kills, <style=cIsHealth>consume</style> them to restore <style=cIsHealing>10% max health</style>.");
            #endregion

            #region Keywords
            LanguageAPI.Add("KEYWORD_REDGUY_M12", "<style=cKeywordName>Hold Stance</style><style=cSub>While charging a wall jump, enter a <style=cIsUtility>hold stance</style>. Release to swing for up to <style=cIsDamage>5x damage</style> based on velocity.");
            LanguageAPI.Add("KEYWORD_REDGUY_M1", "<style=cKeywordName>Empowered Effect</style><style=cSub>Swings faster.");
            LanguageAPI.Add("KEYWORD_REDGUY_M2", "<style=cKeywordName>Empowered Effect (Ground)</style><style=cSub>Successful hits <style=cIsUtility>lunge farther</style> and <style=cIsDamage>boost damage by 50%</style>, stacking.");
            LanguageAPI.Add("KEYWORD_REDGUY_M2B", "<style=cKeywordName>Empowered Effect (Air)</style><style=cSub>Deals more damage to <style=cIsUtility>low health enemies</style> and lunges farther.");
            LanguageAPI.Add("KEYWORD_REDGUY_HEAL", $"<style=cKeywordName>Empowered Effect</style><style=cSub>Explodes for <style=cIsDamage>{100f * SkillStates.Ravager.Heal.maxDamageCoefficient}% damage</style>.");
            LanguageAPI.Add("KEYWORD_REDGUY_BEAM", $"<style=cKeywordName>Empowered Effect</style><style=cSub>Charges much faster.");
            LanguageAPI.Add("KEYWORD_REDGUY_GRAB", "<style=cKeywordName>Empowered Effect</style><style=cSub>Can grab bosses. Drags enemies along the ground, dealing damage over time.");
            LanguageAPI.Add("KEYWORD_REDGUY_GRAB2", "<style=cKeywordName>Bosses</style><style=cSub>Against bosses, <style=cIsDamage>cling</style> onto them instead and go goblin mode.");
            LanguageAPI.Add("KEYWORD_REDGUY_PUNCH", "<style=cKeywordName>Empowered Effect</style><style=cSub>Unleashes a barrage of punches, dealing heavy damage.");
            #endregion

            #region Achievements
            LanguageAPI.Add(prefix + "UNLOCKABLE_UNLOCKABLE_NAME", "A Dream of Armageddon");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_NAME", "A Dream of Armageddon");
            LanguageAPI.Add(prefix + "UNLOCKABLE_ACHIEVEMENT_DESC", "Apply 50 stacks of Bleed on one enemy.");

            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_UNLOCKABLE_NAME", "Ravager: Mastery");
            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: Mastery");
            LanguageAPI.Add(prefix + "MONSOONUNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, beat the game or obliterate on Monsoon.");

            LanguageAPI.Add(prefix + "TYPHOON_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: Grand Mastery");
            LanguageAPI.Add(prefix + "TYPHOON_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: Grand Mastery");
            LanguageAPI.Add(prefix + "TYPHOON_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, beat the game or obliterate on Typhoon or Eclipse.\n<color=#8888>(Counts any difficulty Typhoon or higher)</color>");

            LanguageAPI.Add(prefix + "PUNCH_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: In a Trail of Fire");
            LanguageAPI.Add(prefix + "PUNCH_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: In a Trail of Fire");
            LanguageAPI.Add(prefix + "PUNCH_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, slam an enemy into the ground 3+ times in one grab.");

            LanguageAPI.Add(prefix + "THROW_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: Hungry Fella");
            LanguageAPI.Add(prefix + "THROW_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: Hungry Fella");
            LanguageAPI.Add(prefix + "THROW_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, consume 8 or more enemies at once.");

            LanguageAPI.Add(prefix + "BEAM_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: Calm as an Ocean");
            LanguageAPI.Add(prefix + "BEAM_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: Calm as an Ocean");
            LanguageAPI.Add(prefix + "BEAM_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, complete a stage without filling the Blood Well.");

            LanguageAPI.Add(prefix + "WALLJUMP_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: Maximum Monkey");
            LanguageAPI.Add(prefix + "WALLJUMP_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: Maximum Monkey");
            LanguageAPI.Add(prefix + "WALLJUMP_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, wall jump 20 times without touching the ground.");

            LanguageAPI.Add(prefix + "SUIT_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: All My Fellas");
            LanguageAPI.Add(prefix + "SUIT_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: All My Fellas");
            LanguageAPI.Add(prefix + "SUIT_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, have 15 allies alive at once.");

            LanguageAPI.Add(prefix + "VOID_UNLOCKABLE_UNLOCKABLE_NAME", "Ravager: From The Nothing");
            LanguageAPI.Add(prefix + "VOID_UNLOCKABLE_ACHIEVEMENT_NAME", "Ravager: From The Nothing");
            LanguageAPI.Add(prefix + "VOID_UNLOCKABLE_ACHIEVEMENT_DESC", "As Ravager, escape the Planetarium.");
            #endregion
        }
    }
}
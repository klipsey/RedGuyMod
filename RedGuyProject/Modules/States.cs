using System.Collections.Generic;
using System;

namespace RedGuyMod.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = new List<Type>();

        public static void RegisterStates()
        {
            entityStates.Add(typeof(RedGuyMod.SkillStates.ConsumedDeath));
            entityStates.Add(typeof(RedGuyMod.SkillStates.FuckMyAss));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.SpawnState));

            entityStates.Add(typeof(RedGuyMod.SkillStates.BaseStates.BaseMeleeAttack));

            entityStates.Add(typeof(RedGuyMod.SkillStates.Emote.BaseEmote));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Emote.Rest));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Emote.Taunt));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Emote.Dance));

            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.MainState));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.BaseRavagerSkillState));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.BaseRavagerState));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.ChargeJump));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.WallJump));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.Blink));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.WallJumpSmall));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.WallJumpBig));

            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.Slash));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.SlashCombo));

            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.SpinSlash));

            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.Heal));

            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.ChargeBeam));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.FireBeam));

            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.DashGrab));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.GrabLaunch));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.PunchRecoil));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.ChargeSlash));
            entityStates.Add(typeof(RedGuyMod.SkillStates.Ravager.ThrowSlash));
        }

        internal static void AddSkill(Type t)
        {
            entityStates.Add(t);
        }
    }
}
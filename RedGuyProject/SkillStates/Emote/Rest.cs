﻿namespace RedGuyMod.SkillStates.Emote
{
    public class Rest : BaseEmote
    {
        public override void OnEnter()
        {
            base.OnEnter();

            this.PlayEmote("RestEmote", "", 1.2f);
        }
    }
}
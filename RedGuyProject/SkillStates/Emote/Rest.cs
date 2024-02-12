namespace RedGuyMod.SkillStates.Emote
{
    public class Rest : BaseEmote
    {
        public override void OnEnter()
        {
            base.OnEnter();

            this.PlayEmote("Rest", "", 1.5f);
        }
    }
}
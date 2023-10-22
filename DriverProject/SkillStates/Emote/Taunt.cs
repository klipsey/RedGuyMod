namespace RobDriver.SkillStates.Emote
{
    public class Taunt : BaseEmote
    {
        public override void OnEnter()
        {
            base.OnEnter();

            this.PlayEmote("TauntEmote", "", 1.5f);
        }
    }
}
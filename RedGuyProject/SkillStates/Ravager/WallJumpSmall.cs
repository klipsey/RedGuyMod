namespace RedGuyMod.SkillStates.Ravager
{
    public class WallJumpSmall : BaseRavagerState
    {
        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("FullBody, Override Soft", "Jump");
            this.outer.SetNextStateToMain();
        }
    }
}
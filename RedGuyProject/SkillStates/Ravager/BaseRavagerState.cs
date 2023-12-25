using EntityStates;
using RedGuyMod.Content.Components;

namespace RedGuyMod.SkillStates.Ravager
{
    public class BaseRavagerState : BaseState
    {
        protected RedGuyController penis;

        public override void OnEnter()
        {
            this.penis = this.gameObject.GetComponent<RedGuyController>();
            base.OnEnter();
        }
    }
}
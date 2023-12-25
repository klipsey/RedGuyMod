using EntityStates;
using RedGuyMod.Content.Components;

namespace RedGuyMod.SkillStates.Ravager
{
    public class BaseRavagerSkillState : BaseSkillState
    {
        protected RedGuyController penis;
        protected bool empowered;

        public override void OnEnter()
        {
            this.RefreshEmpoweredState();
            base.OnEnter();
        }

        protected void RefreshEmpoweredState()
        {
            if (!this.penis) this.penis = this.gameObject.GetComponent<RedGuyController>();
            if (this.penis) this.empowered = this.penis.draining;
        }
    }
}
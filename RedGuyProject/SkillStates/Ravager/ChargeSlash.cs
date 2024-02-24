using UnityEngine;
using RoR2;
using EntityStates;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ChargeSlash : BaseRavagerSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayCrossfade("Gesture, Override", "ChargeSlash", "Slash.playbackRate", 0.3f, 0.1f);
            Util.PlaySound("sfx_ravager_foley_01", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if ((!this.inputBank.skill1.down && base.fixedAge >= 0.1f) || (this.isGrounded && base.fixedAge >= 1.25f))
                {
                    if (!this.penis.isWallClinging)
                    {
                        this.outer.SetNextState(new ThrowSlash());
                        return;
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
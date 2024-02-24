using EntityStates;
using RedGuyMod.Content;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RedGuyMod.SkillStates.Ravager
{
    public class PunchRecoil : BaseRavagerSkillState
    {
        public float baseDuration = 1.2f;

        private float duration;
        private bool hopped;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            base.PlayAnimation("FullBody, Override", "PunchHit", "Grab.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hopped)
            {
                if (base.fixedAge >= (this.duration * 0.15f))
                {
                    this.hopped = true;
                    this.characterMotor.Motor.ForceUnground();
                    this.characterMotor.velocity = this.GetAimRay().direction * -12f;
                    this.characterMotor.velocity += new Vector3(0f, 10f, 0f);
                }
                else
                {
                    this.characterMotor.velocity = Vector3.zero;
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= (this.duration * 0.15f)) return InterruptPriority.Any;
            else return InterruptPriority.Skill;
        }
    }
}
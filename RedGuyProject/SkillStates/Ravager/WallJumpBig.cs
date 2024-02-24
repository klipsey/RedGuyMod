using UnityEngine;
using RoR2;
using EntityStates;

namespace RedGuyMod.SkillStates.Ravager
{
    public class WallJumpBig : BaseRavagerState
    {
        public float duration = 0.25f;

        public float jumpForce;
        public Vector3 jumpDir;

        private bool isSliding;

        public override void OnEnter()
        {
            base.OnEnter();
            this.GetModelAnimator().SetFloat("leapDir", this.inputBank.aimDirection.y);
            base.PlayAnimation("FullBody, Override Soft", "Leap");
            Util.PlaySound("sfx_ravager_leap", this.gameObject);
            Util.PlaySound("sfx_ravager_sonido", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                this.characterMotor.Motor.ForceUnground();
                this.characterMotor.velocity = this.jumpDir * this.jumpForce;

                if (this.isGrounded && !this.isSliding && base.fixedAge >= 0.1f)
                {
                    base.PlayAnimation("Body", "Sprint");
                    base.PlayAnimation("FullBody, Override Soft", "Slide");
                    this.isSliding = true;
                }

                this.characterDirection.moveVector = this.jumpDir;

                if (base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }
    }
}
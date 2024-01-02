using UnityEngine;
using RoR2;
using EntityStates;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ChargeJump : BaseRavagerState
    {
        public float duration = 0.65f;

        private bool success;
        private Vector3 origin;
        private ParticleSystem x1;
        private ParticleSystem x2;
        private float jumpTime;
        private bool hasJumped;
        private Vector3 jumpDir;
        private float jumpForce;
        private bool isSliding;

        public override void OnEnter()
        {
            base.OnEnter();
            this.origin = this.transform.position;
            base.PlayAnimation("Body", "JumpCharge", "Jump.playbackRate", this.duration);
            base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");

            this.x1 = this.FindModelChild("FootChargeL").gameObject.GetComponent<ParticleSystem>();
            this.x2 = this.FindModelChild("FootChargeR").gameObject.GetComponent<ParticleSystem>();

            this.x1.Play();
            this.x2.Play();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!this.success) base.PlayAnimation("Body", "AscendDescend");
            this.x1.Stop();
            this.x2.Stop();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.hasJumped)
            {
                this.jumpTime -= Time.fixedDeltaTime;
                
                if (this.jumpTime <= 0f)
                {
                    this.outer.SetNextStateToMain();
                }
                else this.characterMotor.velocity = this.jumpDir * this.jumpForce;

                if (this.isGrounded && !this.isSliding)
                {
                    base.PlayAnimation("Body", "Sprint");
                    base.PlayAnimation("FullBody, Override Soft", "Slide");
                    this.isSliding = true;
                }

                this.characterDirection.moveVector = this.jumpDir;

                return;
            }

            this.characterMotor.Motor.SetPosition(this.origin);
            this.characterMotor.velocity = Vector3.zero;

            if (base.isAuthority)
            {
                if (this.inputBank.skill1.down)
                {
                    EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetInterruptState(new ChargeSlash(), InterruptPriority.Skill);
                }
                
                if (base.fixedAge >= this.duration || !this.inputBank.jump.down || this.isGrounded)
                {
                    this.x1.Stop();
                    this.x2.Stop();

                    if (base.fixedAge <= 0.2f)
                    {
                        this.success = true;
                        base.PlayAnimation("FullBody, Override Soft", "Jump");
                        GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1.6f, 1.5f, false);
                        this.outer.SetNextStateToMain();
                    }
                    else
                    {
                        this.success = true;
                        this.GetModelAnimator().SetFloat("leapDir", this.inputBank.aimDirection.y);
                        base.PlayAnimation("FullBody, Override Soft", "Leap");
                        Util.PlaySound("sfx_ravager_leap", this.gameObject);
                        Util.PlaySound("sfx_ravager_sonido", this.gameObject);

                        float recoil = 15f;
                        base.AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);

                        float charge = Util.Remap(base.fixedAge, 0f, this.duration, 0f, 1f);

                        this.jumpDir = this.GetAimRay().direction;
                        this.jumpForce = (Util.Remap(charge, 0f, 1f, 1.8f, 4.5f) * this.characterBody.jumpPower);
                        this.characterMotor.velocity = this.jumpDir * this.jumpForce;
                        this.hasJumped = true;
                        this.jumpTime = 0.25f;

                        EffectData effectData = new EffectData
                        {
                            origin = this.transform.position + (Vector3.up * 0.75f),
                            rotation = Util.QuaternionSafeLookRotation(this.GetAimRay().direction),
                            scale = 1f
                        };

                        EffectManager.SpawnEffect(this.penis.skinDef.leapEffectPrefab, effectData, true);
                    }
                }
            }
        }
    }
}
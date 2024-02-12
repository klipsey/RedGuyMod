using UnityEngine;
using RoR2;
using EntityStates;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ChargeJump : BaseRavagerState
    {
        public float duration = 0.65f;
        public bool hopoo = false;

        private bool success;
        private Vector3 origin;
        private ParticleSystem x1;
        private ParticleSystem x2;
        private float jumpTime;
        private bool hasJumped;
        private Vector3 jumpDir;
        private float jumpForce;
        private bool isSliding;
        private uint playID;
        private bool permaCling;

        public override void OnEnter()
        {
            base.OnEnter();
            this.origin = this.transform.position;
            base.PlayAnimation("Body", "JumpCharge", "Jump.playbackRate", this.duration);
            base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
            this.permaCling = Modules.Config.permanentCling.Value;
            this.penis.isWallClinging = true;

            this.x1 = this.FindModelChild("FootChargeL").gameObject.GetComponent<ParticleSystem>();
            this.x2 = this.FindModelChild("FootChargeR").gameObject.GetComponent<ParticleSystem>();

            this.x1.Play();
            this.x2.Play();

            this.playID = Util.PlaySound("sfx_ravager_charge_jump", this.gameObject);

            this.penis.IncrementWallJump();
        }

        public override void OnExit()
        {
            base.OnExit();
            if (!this.success) base.PlayAnimation("Body", "AscendDescend");
            this.penis.isWallClinging = false;
            this.x1.Stop();
            this.x2.Stop();
            AkSoundEngine.StopPlayingID(this.playID);
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

            if (base.isAuthority)
            {
                this.characterMotor.Motor.SetPosition(this.origin);
                this.characterMotor.velocity = Vector3.zero;

                if (this.inputBank.skill1.down)
                {
                    EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetInterruptState(new ChargeSlash(), InterruptPriority.Skill);
                }
                
                if ((base.fixedAge >= this.duration && !this.permaCling) || !this.inputBank.jump.down || (!this.hopoo && this.isGrounded))
                {
                    this.x1.Stop();
                    this.x2.Stop();

                    if (base.fixedAge <= 0.2f)
                    {
                        this.success = true;
                        GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1.6f, 1.5f, false);
                        this.outer.SetNextState(new WallJumpSmall());
                    }
                    else
                    {
                        this.success = true;

                        float recoil = 15f;
                        base.AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);

                        float charge = Mathf.Clamp01(Util.Remap(base.fixedAge, 0f, this.duration, 0f, 1f));

                        this.jumpDir = this.GetAimRay().direction;

                        float movespeed = Mathf.Clamp(this.characterBody.moveSpeed, 1f, 18f);

                        this.jumpForce = (Util.Remap(charge, 0f, 1f, 0.17733990147f, 0.37334975369f) * this.characterBody.jumpPower * movespeed);

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

                        this.outer.SetNextState(new WallJumpBig
                        {
                            jumpDir = this.jumpDir,
                            jumpForce = this.jumpForce
                        });
                    }
                }
            }
        }
    }
}
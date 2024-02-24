using UnityEngine;
using EntityStates;
using RedGuyMod.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;

namespace RedGuyMod.SkillStates.Ravager
{
    public class SpinSlash : BaseMeleeAttack
    {
        public static float _damageCoefficient = 5f;

        private bool hasHopped;
        private SlashType slashType;
        private GameObject swingEffectInstance;

        private enum SlashType
        {
            Ground,
            Air,
            AirDown,
            AirUp
        }

        public override void OnEnter()
        {
            this.RefreshEmpoweredState();
            this.penis.skibidi = true;

            this.slashType = SlashType.Ground;
            if (!this.isGrounded)
            {
                this.slashType = SlashType.Air;
                if (this.inputBank.aimDirection.y >= -0.1f) this.slashType = SlashType.AirUp;
                else if (this.inputBank.aimDirection.y <= -0.3f) this.slashType = SlashType.AirDown;
            }

            this.hitboxName = "SwordBig";

            this.damageCoefficient = SpinSlash._damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.4f;
            this.baseEarlyExitTime = 0.6f;
            this.attackRecoil = 2f / this.attackSpeedStat;
            this.hitStopDuration = 0.1f;

            this.muzzleString = "SwingMuzzleLeap";
            this.characterMotor.velocity *= 0.2f;

            if (this.slashType == SlashType.AirUp)
            {
                this.pushForce = 0f;
                this.bonusForce = Vector3.up * 1000f;
                this.muzzleString = "SwingMuzzleUp";
                this.hitStopDuration = 0f;
            }
            else if (this.slashType == SlashType.Air)
            {
                this.hitStopDuration = 0f;
            }
            else if (this.slashType == SlashType.AirDown)
            {
                this.muzzleString = "SwingMuzzleDown";
            }

            this.attackStartTime = 0.18f;
            this.attackEndTime = 0.4f;

            this.smoothHitstop = true;

            this.swingSoundString = "sfx_ravager_bigswing";
            this.swingEffectPrefab = this.penis.skinDef.bigSwingEffectPrefab;
            this.hitSoundString = "";
            this.hitEffectPrefab = this.penis.skinDef.slashEffectPrefab;
            this.impactSound = Modules.Assets.bigSlashSoundEvent.index;
            this.hitHopVelocity = 0f;

            this.damageType = DamageType.Stun1s;
            if (this.empowered) this.damageType |= DamageType.BonusToLowHealth;

            Util.PlaySound("sfx_ravager_foley_01", this.gameObject);

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if ((this.slashType == SlashType.Air || this.slashType == SlashType.AirUp) && this.characterMotor.velocity.y <= 0f && !this.hasHopped) this.characterMotor.velocity.y = 0f;
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);
            if (this.penis)
            {
                this.penis.FillGauge(1f + (amount * 0.5f));
                this.penis.RefreshBlink();
            }
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("SwordPivot").rotation = Util.QuaternionSafeLookRotation(direction);

                if ((this.slashType == SlashType.Air || this.slashType == SlashType.AirUp) && !this.hasHopped)
                {
                    this.hasHopped = true;
                    this.characterMotor.velocity = Vector3.up * 19f;
                }
                else if (this.slashType == SlashType.AirDown && !this.hasHopped)
                {
                    this.characterMotor.velocity = Vector3.up * 19f;
                    this.hasHopped = true;
                }
            }

            base.FireAttack();
        }

        protected override void PlaySwingEffect()
        {
            this.characterBody.isSprinting = true;
            Util.PlaySound(this.swingSoundString, this.gameObject);
            if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    this.swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }

            if (this.slashType == SlashType.Air)
            {
                float force = 18f;
                if (this.empowered) force = 22f;
                this.characterMotor.velocity += this.GetAimRay().direction * force;
            }
            else if (this.slashType == SlashType.Ground)
            {
                float force = 40f;
                if (this.empowered) force = 50f;
                if (this.inputBank.moveVector != Vector3.zero) this.characterMotor.velocity += this.characterDirection.forward * force;
            }
            else if (this.slashType == SlashType.AirUp)
            {
                float force = 18f;
                if (this.empowered) force = 22f;
                this.characterMotor.velocity += this.GetAimRay().direction * force;
            }
            else if (this.slashType == SlashType.AirDown)
            {
                float force = 4;
                if (this.empowered) force = 30f;
                this.characterMotor.velocity += this.GetAimRay().direction * force;
            }
        }

        protected override void TriggerHitStop()
        {
            base.TriggerHitStop();

            if (this.swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = 20f;
            }
        }

        protected override void ClearHitStop()
        {
            base.ClearHitStop();

            if (this.swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = fuck.initialDuration;
            }
        }

        protected override void PlayAttackAnimation()
        {
            if (this.slashType == SlashType.AirUp)
            {
                base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
                base.PlayCrossfade("FullBody, Override", "SpinSlashAir", "Slash.playbackRate", this.duration, 0.1f);
            }
            else if (this.slashType == SlashType.Ground)
            {
                base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
                base.PlayCrossfade("Gesture, Override", "SpinSlash2", "Slash.playbackRate", this.duration, 0.1f);
                base.PlayCrossfade("FullBody, Override", "SpinSlash", "Slash.playbackRate", this.duration, 0.1f);
            }
            else if (this.slashType == SlashType.Air)
            {
                base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
                base.PlayCrossfade("FullBody, Override", "SpinSlashAir2", "Slash.playbackRate", this.duration, 0.1f);
            }
            else if (this.slashType == SlashType.AirDown)
            {
                base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
                base.PlayCrossfade("FullBody, Override", "SpinSlashAir3", "Slash.playbackRate", this.duration, 0.1f);
            }
        }

        protected override void SetNextState()
        {
            if (this.skillLocator.primary.skillDef.skillNameToken == Content.Survivors.RedGuy.primaryNameToken)
            {
                this.outer.SetNextState(new Slash
                {
                    swingIndex = 0
                });
            }
            else
            {
                this.outer.SetNextState(new SlashCombo
                {
                    swingIndex = 0
                });
            }

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch >= (this.duration * this.baseEarlyExitTime)) return InterruptPriority.Skill;
            return InterruptPriority.PrioritySkill;
        }
    }
}
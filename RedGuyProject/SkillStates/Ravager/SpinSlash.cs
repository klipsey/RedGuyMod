using UnityEngine;
using EntityStates;
using RedGuyMod.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;

namespace RedGuyMod.SkillStates.Ravager
{
    public class SpinSlash : BaseMeleeAttack
    {
        public static float _damageCoefficient = 7.5f;

        private bool hasHopped;
        private bool airSlash;

        public override void OnEnter()
        {
            this.RefreshEmpoweredState();

            this.airSlash = !this.isGrounded;

            this.hitboxName = "SwordBig";

            this.damageCoefficient = SpinSlash._damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.3f;
            this.baseEarlyExitTime = 0.7f;
            this.attackRecoil = 2f / this.attackSpeedStat;
            this.hitStopDuration = 0.1f;

            this.muzzleString = "SwingMuzzleLeap";

            if (this.airSlash)
            {
                this.pushForce = 0f;
                this.bonusForce = Vector3.up * 1000f;
                this.characterMotor.velocity *= 0.2f;
                this.muzzleString = "SwingMuzzleUp";
                this.hitStopDuration = 0f;
            }

            this.attackStartTime = 0.13f;
            this.attackEndTime = 0.4f;

            this.smoothHitstop = true;

            this.swingSoundString = "sfx_ravager_swing";
            this.swingEffectPrefab = Modules.Assets.swingEffect;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.slashImpactEffect;
            this.impactSound = Modules.Assets.slashSoundEvent.index;
            this.hitHopVelocity = 0f;

            this.damageType = DamageType.Stun1s;
            if (this.empowered) this.damageType |= DamageType.BonusToLowHealth;

            Util.PlaySound("sfx_ravager_foley_01", this.gameObject);

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.airSlash && this.characterMotor.velocity.y <= 0f && !this.hasHopped) this.characterMotor.velocity.y = 0f;
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);
            if (this.penis) this.penis.FillGauge(1f + (amount * 0.5f));
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("SwordPivot").rotation = Util.QuaternionSafeLookRotation(direction);

                if (this.airSlash && !this.hasHopped)
                {
                    this.hasHopped = true;
                    this.characterMotor.velocity = Vector3.up * 19f;
                }
            }

            base.FireAttack();
        }

        protected override void PlaySwingEffect()
        {
            Util.PlaySound(this.swingSoundString, this.gameObject);
            if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    GameObject swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    ScaleParticleSystemDuration fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }

            if (this.airSlash)
            {
                float force = 8f;
                if (this.empowered) force = 16f;
                this.characterMotor.velocity += this.GetAimRay().direction * force;
            }
            else
            {
                float force = 12f;
                if (this.empowered) force = 24f;
                if (this.inputBank.moveVector != Vector3.zero) this.characterMotor.velocity += this.characterDirection.forward * force;
            }
        }

        protected override void PlayAttackAnimation()
        {
            if (this.airSlash)
            {
                base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
                base.PlayCrossfade("FullBody, Override", "SpinSlashAir", "Slash.playbackRate", this.duration, 0.1f);
            }
            else
            {
                base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
                base.PlayCrossfade("Gesture, Override", "SpinSlash", "Slash.playbackRate", this.duration, 0.1f);
            }
        }

        protected override void SetNextState()
        {
            this.outer.SetNextState(new Slash());
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.stopwatch >= (this.duration * this.baseEarlyExitTime)) return InterruptPriority.Skill;
            return InterruptPriority.PrioritySkill;
        }
    }
}
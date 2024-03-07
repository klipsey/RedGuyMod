using UnityEngine;
using EntityStates;
using RedGuyMod.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;
using RedGuyMod.Content.Components;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ClingFlourish : BaseMeleeAttack
    {
        public static float _damageCoefficient = 10f;
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.RefreshEmpoweredState();
            this.hitboxName = "Sword";

            this.damageCoefficient = ClingFlourish._damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.4f;
            if (this.empowered) this.baseDuration *= 0.65f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 2f / this.attackSpeedStat;

            this.attackStartTime = 0.21f;
            this.attackEndTime = 0.5f;

            this.hitStopDuration = 0.32f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_ravager_bigswing";
            this.swingEffectPrefab = this.penis.skinDef.bigSwingEffectPrefab;
            this.hitSoundString = "";
            this.hitEffectPrefab = this.penis.skinDef.slashEffectPrefab;
            this.impactSound = Modules.Assets.bigSlashSoundEvent.index;

            this.damageType = DamageType.Stun1s;

            this.muzzleString = "SwingMuzzleLeap";
            Util.PlaySound("sfx_ravager_foley_01", this.gameObject);

            base.OnEnter();
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);
            if (this.penis)
            {
                this.penis.FillGauge(1.5f + (amount * 0.5f));
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

                EntityStateMachine.FindByCustomName(this.gameObject, "Body").SetNextStateToMain();

                this.characterMotor.velocity = -this.GetAimRay().direction * 25f;
                this.transform.parent = null;
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
                    this.swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    this.swingEffectInstance.transform.localScale *= 1.5f;
                    ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
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
            base.PlayCrossfade("FullBody, Override", "ClingFlourish", "Slash.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
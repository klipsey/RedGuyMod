using UnityEngine;
using EntityStates;
using RedGuyMod.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ThrowSlash : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;
        private float charge;

        public override void OnEnter()
        {
            this.RefreshEmpoweredState();
            this.hitboxName = "SwordBig";

            this.charge = Mathf.Clamp01(Util.Remap(this.characterMotor.velocity.magnitude, 0f, 60f, 0f, 1f));

            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, Slash._damageCoefficient, Slash._damageCoefficient * 5f);
            this.pushForce = 200f;
            this.baseDuration = 0.8f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 2f / this.attackSpeedStat;

            this.attackStartTime = 0f;
            this.attackEndTime = 0.3f;

            this.hitStopDuration = 0.08f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_ravager_swing";
            this.swingEffectPrefab = this.penis.skinDef.basicSwingEffectPrefab;
            this.hitSoundString = "";
            this.hitEffectPrefab = this.penis.skinDef.slashEffectPrefab;
            this.impactSound = Modules.Assets.slashSoundEvent.index;

            this.damageType = DamageType.Generic;

            this.muzzleString = "SwingMuzzleLeap";

            if (this.charge >= 0.9f)
            {
                this.hitStopDuration *= 2.5f;
                this.swingSoundString = "sfx_ravager_bigswing";
                this.impactSound = Modules.Assets.bigSlashSoundEvent.index;
                this.swingEffectPrefab = this.penis.skinDef.bigSwingEffectPrefab;
                this.damageType = DamageType.Stun1s;
            }

            base.OnEnter();
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);
            if (this.penis) this.penis.FillGauge(0.5f + (amount * 0.5f));
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("SwordPivot").rotation = Util.QuaternionSafeLookRotation(direction);
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
            if (this.charge >= 0.9f)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
                base.PlayAnimation("FullBody, Override", "ThrowSlashMax", "Slash.playbackRate", this.duration * 2f);
                base.PlayAnimation("Gesture, Override", "ThrowSlashMax", "Slash.playbackRate", this.duration * 2f);
            }
            else base.PlayAnimation("Gesture, Override", "ThrowSlash", "Slash.playbackRate", this.duration);
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
        }
    }
}
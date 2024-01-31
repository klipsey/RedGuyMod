using UnityEngine;
using EntityStates;
using RedGuyMod.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;
using RedGuyMod.Content.Components;

namespace RedGuyMod.SkillStates.Ravager
{
    public class SlashCombo : BaseMeleeAttack
    {
        public static float _damageCoefficient = 1.8f;
        public static float finisherDamageCoefficient = 3.9f;
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.RefreshEmpoweredState();
            this.hitboxName = "Sword";

            this.damageCoefficient = SlashCombo._damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.1f;
            if (this.empowered) this.baseDuration *= 0.65f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 2f / this.attackSpeedStat;

            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.3f;

            this.hitStopDuration = 0.08f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_ravager_swing";
            this.swingEffectPrefab = this.penis.skinDef.basicSwingEffectPrefab;
            this.hitSoundString = "";
            this.hitEffectPrefab = this.penis.skinDef.slashEffectPrefab;
            this.impactSound = Modules.Assets.slashSoundEvent.index;

            this.damageType = DamageType.Generic;

            if (this.swingIndex == 0) this.muzzleString = "SwingMuzzle1";
            else this.muzzleString = "SwingMuzzle2";

            if (this.swingIndex == 2)
            {
                this.muzzleString = "SwingMuzzleLeap";

                this.duration *= 1.25f;
                this.baseEarlyExitTime = 0.75f;
                this.hitStopDuration *= 2.5f;
                this.attackStartTime = 0.22f;
                this.damageType = DamageType.Stun1s;
                this.swingSoundString = "sfx_ravager_bigswing";
                this.impactSound = Modules.Assets.bigSlashSoundEvent.index;
                this.damageCoefficient = SlashCombo.finisherDamageCoefficient;
            }

            base.OnEnter();

            /*if (this.empowered)
            {
                this.FireAttack();
                this.InitializeAttack();
            }*/
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);
            if (this.penis)
            {
                this.penis.FillGauge(0.5f + (amount * 0.5f));
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
            if (this.swingIndex == 1) base.PlayCrossfade("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration, 0.1f);
            else if (this.swingIndex == 0) base.PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration, 0.1f);
            else base.PlayCrossfade("Gesture, Override", "SpinSlash", "Slash.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
            this.FireShuriken();

            int index = this.swingIndex;
            if (index == 0) index = 1;
            else if (index == 1) index = 2;
            else index = 0;

            this.outer.SetNextState(new SlashCombo
            {
                swingIndex = index
            });
        }
    }
}
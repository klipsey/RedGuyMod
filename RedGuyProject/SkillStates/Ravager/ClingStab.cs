using UnityEngine;
using EntityStates;
using RedGuyMod.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;
using RedGuyMod.Content.Components;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ClingStab : BaseMeleeAttack
    {
        public static float _damageCoefficient = 6f;
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.RefreshEmpoweredState();
            this.hitboxName = "Sword";

            this.damageCoefficient = ClingStab._damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.4f;
            if (this.empowered) this.baseDuration *= 0.65f;
            this.baseEarlyExitTime = 0.75f;
            this.attackRecoil = 5f / this.attackSpeedStat;

            this.attackStartTime = 0.4f;
            this.attackEndTime = 0.5f;

            this.hitStopDuration = 0.24f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_ravager_swing";
            this.swingEffectPrefab = this.penis.skinDef.basicSwingEffectPrefab;
            this.hitSoundString = "";
            this.hitEffectPrefab = this.penis.skinDef.slashEffectPrefab;
            this.impactSound = Modules.Assets.bigSlashSoundEvent.index;

            this.damageType = DamageType.Generic;

            this.muzzleString = "SwingMuzzle1";

            base.OnEnter();

            /*if (this.empowered)
            {
                this.FireAttack();
                this.InitializeAttack();
            }*/
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= (this.duration * this.earlyExitTime) && base.isAuthority)
            {
                if (base.inputBank.skill2.down)
                {
                    if (!this.hasFired) this.FireAttack();
                    this.SetNextState2();
                    return;
                }
            }
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
            }

            base.FireAttack();
        }

        protected override void PlaySwingEffect()
        {
            Util.PlaySound(this.swingSoundString, this.gameObject);
            /*if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    this.swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }*/
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
            base.PlayCrossfade("FullBody, Override", "ClingStab", "Slash.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
            this.FireShuriken();

            int index = this.swingIndex;
            if (index == 0) index = 1;
            else index = 0;

            this.outer.SetNextState(new ClingSlash
            {
                swingIndex = index
            });
        }

        protected virtual void SetNextState2()
        {
            int index = this.swingIndex;
            if (index == 0) index = 1;
            else index = 0;

            this.outer.SetNextState(new ClingStab
            {
                swingIndex = index
            });
        }
    }
}
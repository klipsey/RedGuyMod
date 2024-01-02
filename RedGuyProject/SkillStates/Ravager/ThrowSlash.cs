using UnityEngine;
using EntityStates;
using RedGuyMod.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ThrowSlash : BaseMeleeAttack
    {
        public override void OnEnter()
        {
            this.RefreshEmpoweredState();
            this.hitboxName = "Sword";

            this.damageCoefficient = Util.Remap(this.characterMotor.velocity.magnitude, 0f, 90f, Slash._damageCoefficient, Slash._damageCoefficient * 4f);
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
                    GameObject swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    ScaleParticleSystemDuration fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }
        }

        protected override void PlayAttackAnimation()
        {
            base.PlayAnimation("Gesture, Override", "ThrowSlash", "Slash.playbackRate", this.duration);
        }

        protected override void SetNextState()
        {
            this.outer.SetNextState(new Slash
            {
                swingIndex = 0
            });
        }
    }
}
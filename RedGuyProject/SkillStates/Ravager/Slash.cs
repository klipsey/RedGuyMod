using UnityEngine;
using EntityStates;
using RedGuyMod.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;
using RedGuyMod.Content.Components;

namespace RedGuyMod.SkillStates.Ravager
{
    public class Slash : BaseMeleeAttack
    {
        public static float _damageCoefficient = 3.2f;

        public override void OnEnter()
        {
            this.RefreshEmpoweredState();
            this.hitboxName = "Sword";

            this.damageCoefficient = Slash._damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.1f;
            if (this.empowered) this.baseDuration *= 0.6f;
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
            if (this.swingIndex == 1) base.PlayCrossfade("Gesture, Override", "Slash2", "Slash.playbackRate", this.duration, 0.1f);
            else base.PlayCrossfade("Gesture, Override", "Slash1", "Slash.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
            this.FireShuriken();

            int index = this.swingIndex;
            if (index == 0) index = 1;
            else index = 0;

            this.outer.SetNextState(new Slash
            {
                swingIndex = index
            });
        }
    }
}
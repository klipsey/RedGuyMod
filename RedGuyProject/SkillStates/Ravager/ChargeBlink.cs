using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ChargeBlink : ChargeJump
    {
        private Transform modelTransform;

        public override void OnEnter()
        {
            this.duration = 0.6f;
            base.OnEnter();
            this.modelTransform = this.GetModelTransform();

            if (this.modelTransform)
            {
                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = this.duration;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDissolve.mat").WaitForCompletion();
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
            }
        }

        protected override void PlayAnim()
        {
            base.PlayCrossfade("Body", "JumpChargeHopoo", "Jump.playbackRate", this.duration, 0.1f);
        }

        protected override void SetJumpTime()
        {
            this.jumpTime = 0.5f;

            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(this.jumpDir);
            effectData.origin = Util.GetCorePosition(this.gameObject);
            EffectManager.SpawnEffect(EntityStates.ImpMonster.BlinkState.blinkPrefab, effectData, false);
        }

        protected override void NextState()
        {
            this.outer.SetNextState(new BlinkBig
            {
                jumpDir = this.jumpDir,
                jumpForce = this.jumpForce * 1.5f
            });
        }
    }
}
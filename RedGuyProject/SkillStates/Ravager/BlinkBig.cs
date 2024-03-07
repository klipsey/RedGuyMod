using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RedGuyMod.SkillStates.Ravager
{
    public class BlinkBig : WallJumpBig
    {
        private Transform modelTransform;
        private CharacterModel characterModel;

        public override void OnEnter()
        {
            this.duration = 0.35f;
            base.OnEnter();
            this.modelTransform = this.GetModelTransform();

            if (this.modelTransform) this.characterModel = this.modelTransform.GetComponent<CharacterModel>();

            if (this.characterModel) this.characterModel.invisibilityCount++;

            base.gameObject.layer = LayerIndex.fakeActor.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

        public override void OnExit()
        {
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();

            base.OnExit();

            EffectData effectData = new EffectData();
            effectData.rotation = Util.QuaternionSafeLookRotation(-this.jumpDir);
            effectData.origin = Util.GetCorePosition(this.gameObject);
            EffectManager.SpawnEffect(EntityStates.ImpMonster.BlinkState.blinkPrefab, effectData, false);

            this.characterMotor.velocity = Vector3.up * 10f;

            if (this.characterModel) this.characterModel.invisibilityCount--;

            if (this.modelTransform)
            {
                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 0.5f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDissolve.mat").WaitForCompletion();
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
            }
        }
    }
}
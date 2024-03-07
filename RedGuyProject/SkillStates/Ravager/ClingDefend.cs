using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ClingDefend : BaseRavagerSkillState
    {
        private Transform modelTransform;
        private TemporaryOverlay temporaryOverlay;

        public override void OnEnter()
        {
            base.OnEnter();
            this.modelTransform = this.GetModelTransform();
            base.PlayCrossfade("FullBody, Override", "ClingDefend", 0.05f);

            if (NetworkServer.active) this.characterBody.AddBuff(Content.Survivors.RedGuy.clingDefendBuff);

            if (this.modelTransform)
            {
                this.temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1000f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDissolve.mat").WaitForCompletion();
                temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("FullBody, Override", "ClingDefendOff");
            if (NetworkServer.active) this.characterBody.RemoveBuff(Content.Survivors.RedGuy.clingDefendBuff);

            if (this.temporaryOverlay) Destroy(this.temporaryOverlay);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (!this.inputBank.skill3.down && base.fixedAge >= 0.1f)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
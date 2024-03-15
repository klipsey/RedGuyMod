using UnityEngine;
using RoR2;
using EntityStates;
using static RoR2.CameraTargetParams;
using RoR2.UI;

namespace RedGuyMod.SkillStates.Ravager
{
    public class Cling : GenericCharacterMain
    {
        public HurtBox targetHurtbox;
        public Vector3 offset;
        public GameObject anchor;

        private Transform modelTransform;
        private Content.Components.RedGuyController penis;
        private CameraParamsOverrideHandle camParamsOverrideHandle;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private bool cancelling;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterDirection.enabled = false;
            this.modelLocator.enabled = false;
            this.modelTransform = this.GetModelTransform();
            this.penis = this.GetComponent<Content.Components.RedGuyController>();

            this.penis.clingTimer = 8f;

            this.skillLocator.primary.SetSkillOverride(this, Content.Survivors.RedGuy.clingSlashSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.secondary.SetSkillOverride(this, Content.Survivors.RedGuy.clingStabSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.special.SetSkillOverride(this, Content.Survivors.RedGuy.clingFlourishSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (this.skillLocator.utility.skillDef.skillNameToken == Content.Survivors.RedGuy.healNameToken)
            {
                this.skillLocator.utility.SetSkillOverride(this, Content.Survivors.RedGuy.clingHealSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            else
            {
                this.skillLocator.utility.SetSkillOverride(this, Content.Survivors.RedGuy.clingBeamSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }

            this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, RavagerCameraParams.CLING, 1f);
            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(this.characterBody, Modules.Assets.clingCrosshair, CrosshairUtils.OverridePriority.Skill);

            base.gameObject.layer = LayerIndex.ignoreRaycast.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
        }

        public override void OnExit()
        {
            base.gameObject.layer = LayerIndex.defaultLayer.intVal;
            base.characterMotor.Motor.RebuildCollidableLayers();
            base.OnExit();
            this.characterDirection.enabled = true;
            this.modelLocator.enabled = true;

            this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
            if (this.crosshairOverrideRequest != null) this.crosshairOverrideRequest.Dispose();

            this.skillLocator.primary.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingSlashSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.secondary.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingStabSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.special.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingFlourishSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (this.skillLocator.utility.skillDef.skillNameToken == Content.Survivors.RedGuy.healNameToken)
            {
                this.skillLocator.utility.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingHealSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
            else
            {
                this.skillLocator.utility.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingBeamSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            }
        }

        public override void HandleMovements()
        {
        }

        public override void ProcessJump()
        {
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.characterBody.isSprinting = false;

            if (base.isAuthority)
            {
                if (this.targetHurtbox && this.anchor && this.targetHurtbox.healthComponent.alive)
                {
                    this.characterMotor.Motor.SetPosition(this.anchor.transform.position);

                    if (this.modelTransform)
                    {
                        this.modelTransform.rotation = Util.QuaternionSafeLookRotation((this.targetHurtbox.transform.position - this.transform.position).normalized);
                        this.modelTransform.position = this.transform.position;
                    }
                }
                else
                {
                    this.cancelling = true;
                }

                if (this.inputBank.jump.justPressed) this.cancelling = true;
                if (this.penis.clingTimer <= 0f) this.cancelling = true;

                this.characterMotor.velocity = Vector3.zero;
            }

            if (this.cancelling)
            {
                base.PlayAnimation("FullBody, Override", "BufferEmpty");
                this.characterMotor.velocity = Vector3.up * 15f;
                this.outer.SetNextStateToMain();
            }
        }
    }
}
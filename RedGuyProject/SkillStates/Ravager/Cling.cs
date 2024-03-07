using UnityEngine;
using RoR2;
using EntityStates;

namespace RedGuyMod.SkillStates.Ravager
{
    public class Cling : GenericCharacterMain
    {
        public HurtBox targetHurtbox;
        public Vector3 offset;
        public GameObject anchor;

        private Transform modelTransform;
        private Content.Components.RedGuyController penis;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterDirection.enabled = false;
            this.modelLocator.enabled = false;
            this.modelTransform = this.GetModelTransform();
            this.penis = this.GetComponent<Content.Components.RedGuyController>();

            this.skillLocator.primary.SetSkillOverride(this, Content.Survivors.RedGuy.clingSlashSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.secondary.SetSkillOverride(this, Content.Survivors.RedGuy.clingStabSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.utility.SetSkillOverride(this, Content.Survivors.RedGuy.clingDefendSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.special.SetSkillOverride(this, Content.Survivors.RedGuy.clingFlourishSkillDef, GenericSkill.SkillOverridePriority.Contextual);

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

            this.skillLocator.primary.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingSlashSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.secondary.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingStabSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.utility.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingDefendSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.special.UnsetSkillOverride(this, Content.Survivors.RedGuy.clingFlourishSkillDef, GenericSkill.SkillOverridePriority.Contextual);
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
                    base.PlayAnimation("FullBody, Override", "BufferEmpty");
                    this.characterMotor.velocity = Vector3.up * 15f;
                    this.outer.SetNextStateToMain();
                }

                this.characterMotor.velocity = Vector3.zero;
            }
        }
    }
}
using UnityEngine;
using RoR2;
using RoR2.Orbs;
using EntityStates.ImpMonster;

namespace RedGuyMod.Content
{
    public class ConsumeOrb : Orb
    {
        public float healOverride = -1f;

        public override void Begin()
        {
            base.duration = base.distanceToTarget / Random.Range(2f, 5f);

            EffectData effectData = new EffectData
            {
                origin = this.origin,
                genericFloat = base.duration
            };

            effectData.SetHurtBoxReference(this.target);

            EffectManager.SpawnEffect(Modules.Assets.consumeOrb, effectData, true);
        }

        public override void OnArrival()
        {
            if (this.target)
            {
                Util.PlaySound("sfx_ravager_consume", this.target.gameObject);

                if (this.target.healthComponent)
                {
                    if (this.healOverride != -1f)
                    {
                        this.target.healthComponent.Heal(this.healOverride, default(ProcChainMask));
                    }
                    else this.target.healthComponent.HealFraction(0.3f, default(ProcChainMask));

                    Transform modelTransform = this.target.healthComponent.body.modelLocator.modelTransform;
                    if (modelTransform && BlinkState.destealthMaterial)
                    {
                        TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay.duration = 1f;
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = BlinkState.destealthMaterial;
                        temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        temporaryOverlay.animateShaderAlpha = true;
                    }
                }
            }
        }
    }
}
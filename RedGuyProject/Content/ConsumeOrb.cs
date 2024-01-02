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

            GameObject effectPrefab = Modules.Assets.consumeOrb;
            if (this.target)
            {
                Components.RedGuyController penis = this.target.healthComponent.gameObject.GetComponent<Components.RedGuyController>();
                if (penis)
                {
                    effectPrefab = penis.skinDef.bloodOrbEffectPrefab;
                }
            }

            EffectManager.SpawnEffect(effectPrefab, effectData, true);
        }

        public override void OnArrival()
        {
            if (this.target)
            {
                if (this.target.healthComponent)
                {
                    if (this.healOverride != -1f)
                    {
                        this.target.healthComponent.Heal(this.healOverride, default(ProcChainMask));
                    }
                    else this.target.healthComponent.HealFraction(0.3f, default(ProcChainMask));

                    Components.RedGuyController penis = this.target.healthComponent.gameObject.GetComponent<Components.RedGuyController>();
                    if (penis)
                    {
                        Transform modelTransform = this.target.healthComponent.body.modelLocator.modelTransform;
                        if (modelTransform && penis.skinDef)
                        {
                            TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                            temporaryOverlay.duration = 1f;
                            temporaryOverlay.destroyComponentOnEnd = true;
                            temporaryOverlay.originalMaterial = penis.skinDef.bloodOrbOverlayMaterial;
                            temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                            temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                            temporaryOverlay.animateShaderAlpha = true;
                        }

                        Util.PlaySound(penis.skinDef.consumeSoundString, this.target.gameObject);
                    }
                }
            }
        }
    }
}
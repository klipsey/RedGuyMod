using UnityEngine;
using RoR2;
using RoR2.Orbs;
using EntityStates.ImpMonster;
using UnityEngine.Networking;
using R2API.Networking;
using R2API.Networking.Interfaces;

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
                    else this.target.healthComponent.HealFraction(0.1f, default(ProcChainMask));

                    NetworkIdentity identity = this.target.healthComponent.gameObject.GetComponent<NetworkIdentity>();
                    if (!identity) return;
                    new SyncOrbOverlay(identity.netId, this.target.healthComponent.gameObject).Send(NetworkDestination.Clients);
                }
            }
        }
    }
}
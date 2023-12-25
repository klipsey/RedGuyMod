using UnityEngine;
using RoR2;

namespace RedGuyMod.Content.Components
{
    public class RedGuyController : MonoBehaviour
    {
        public float drainRate = 30f;
        public float maxDecayRate = 360f;
        public float decayGrowth = 10f;
        public float meter = 0f;

        public bool draining;
        private float decay;
        public float storedHealth;
        private float iDontEvenKnowAnymore;
        private HealthComponent healthComponent;
        private Animator animator;

        private ParticleSystem steamEffect;
        private ParticleSystem chargeEffect;
        private uint playID;

        public bool inGrab;

        private void Awake()
        {
            this.healthComponent = this.GetComponent<HealthComponent>();
            Transform modelTransform = this.GetComponent<ModelLocator>().modelTransform;
            this.animator = modelTransform.GetComponent<Animator>();
            ChildLocator childLocator = modelTransform.GetComponent<ChildLocator>();

            this.steamEffect = childLocator.FindChild("Steam").gameObject.GetComponent<ParticleSystem>();
            this.chargeEffect = childLocator.FindChild("ArmCharge").gameObject.GetComponent<ParticleSystem>();
        }

        private void FixedUpdate()
        {
            this.decay = Mathf.Clamp(this.decay + (this.decayGrowth * Time.fixedDeltaTime), 0f, this.maxDecayRate);

            if (this.draining) this.decay = this.drainRate;
            this.meter = Mathf.Clamp(this.meter - (this.decay * Time.fixedDeltaTime), 0f, 100f);

            if (this.meter <= 0f)
            {
                if (!this.inGrab)
                {
                    if (this.draining)
                    {
                        this.steamEffect.Stop();
                        this.chargeEffect.Stop();

                        AkSoundEngine.StopPlayingID(this.playID);
                        Util.PlaySound("sfx_ravager_steam", this.gameObject);
                    }

                    this.draining = false;
                }
            }

            if (this.draining)
            {
                float amount = this.iDontEvenKnowAnymore * Time.fixedDeltaTime;
                this.storedHealth -= amount;
                if (this.storedHealth >= 0f) this.healthComponent.Heal(iDontEvenKnowAnymore * Time.fixedDeltaTime, default(ProcChainMask), false);
            }

            if (this.animator) this.animator.SetBool("isEmpowered", this.draining);
        }

        public void FillGauge(float multiplier = 1f)
        {
            if (this.draining) return;

            this.decay = 0f;

            this.meter += 10f * multiplier;
            if (this.healthComponent.combinedHealthFraction <= 0.5f) this.meter += 10f * multiplier;

            if (this.meter >= 100f)
            {
                this.ActivateDrain();
            }
        }

        public void ActivateDrain()
        {
            this.meter = 100f;
            this.draining = true;

            this.steamEffect.Play();
            this.chargeEffect.Play();

            this.storedHealth = this.healthComponent.fullHealth * 0.75f;
            this.iDontEvenKnowAnymore = this.storedHealth / (100f / this.drainRate);

            Util.PlaySound("sfx_ravager_bloodrush", this.gameObject);
            this.playID = Util.PlaySound("sfx_ravager_steam_loop", this.gameObject);

            EffectManager.SpawnEffect(Modules.Assets.drainTextEffect, new EffectData
            {
                origin = this.transform.position + (Vector3.up * 1.8f),
                rotation = Quaternion.identity,
                scale = 1f
            }, false);
        }

        public void CalcStoredHealth(bool isEmpowered)
        {
            if (isEmpowered) this.storedHealth = this.healthComponent.fullHealth * Util.Remap(this.meter, 0f, 100f, 0f, 1f);
            else this.storedHealth = this.healthComponent.fullHealth * Util.Remap(this.meter, 0f, 100f, 0f, 0.5f);
        }
    }
}
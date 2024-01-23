using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using R2API.Networking;
using System;

namespace RedGuyMod.Content.Components
{
    public class RedGuyController : MonoBehaviour
    {
        public bool blinkReady;

        public float drainRate = 24f;
        public float maxDecayRate = 360f;
        public float decayGrowth = 10f;
        public float meter = 0f;

        public bool draining;
        private float decay;
        public float storedHealth;
        private float iDontEvenKnowAnymore;
        private HealthComponent healthComponent;
        private Animator animator;

        public int wallJumpCounter;

        private ParticleSystem steamEffect;
        private ParticleSystem chargeEffect;
        private ParticleSystem blackElectricityEffect;
        private ParticleSystem handElectricityEffect;
        private uint playID;
        private CharacterBody characterBody;
        private ModelSkinController skinController;
        private ChildLocator childLocator;

        public float chargeValue;

        public bool inGrab;

        private RavagerSkinDef cachedSkinDef;
        public RedGuyPassive passive;

        public static event Action<int> onWallJumpIncremented;
        public static event Action<bool> onStageCompleted;

        private bool wasBloodWellFilled;

        public RavagerSkinDef skinDef
        {
            get
            {
                if (!this.cachedSkinDef) this.ApplySkin();
                return this.cachedSkinDef;
            }
        }

        private void Awake()
        {
            this.characterBody = this.GetComponent<CharacterBody>();
            this.healthComponent = this.GetComponent<HealthComponent>();
            Transform modelTransform = this.GetComponent<ModelLocator>().modelTransform;
            this.animator = modelTransform.GetComponent<Animator>();
            this.childLocator = modelTransform.GetComponent<ChildLocator>();
            this.skinController = this.GetComponentInChildren<ModelSkinController>();
            this.passive = this.GetComponent<RedGuyPassive>();
            this.wallJumpCounter = 0;

            this.steamEffect = this.childLocator.FindChild("Steam").gameObject.GetComponent<ParticleSystem>();
            this.chargeEffect = this.childLocator.FindChild("ArmCharge").gameObject.GetComponent<ParticleSystem>();
            this.blackElectricityEffect = this.childLocator.FindChild("BlackElectricity").gameObject.GetComponent<ParticleSystem>();
            this.handElectricityEffect = this.childLocator.FindChild("HandElectricity").gameObject.GetComponent<ParticleSystem>();

            this.Invoke("ApplySkin", 0.3f);

            RoR2.TeleporterInteraction.onTeleporterFinishGlobal += TeleporterInteraction_onTeleporterFinishGlobal;
        }

        private void TeleporterInteraction_onTeleporterFinishGlobal(TeleporterInteraction obj)
        {
            Action<bool> action = onStageCompleted;
            if (action == null) return;
            action(this.wasBloodWellFilled);
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
                        this.blackElectricityEffect.Stop();

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
                if (NetworkServer.active) if (this.storedHealth >= 0f) this.healthComponent.Heal(iDontEvenKnowAnymore * Time.fixedDeltaTime, default(ProcChainMask), false);
            }

            if (this.characterBody.characterMotor.isGrounded) this.blinkReady = true;

            if (this.blinkReady)
            {
                if (!this.handElectricityEffect.isPlaying) this.handElectricityEffect.Play();
            }
            else
            {
                if (this.handElectricityEffect.isPlaying) this.handElectricityEffect.Stop();
            }

            if (this.animator) this.animator.SetBool("isEmpowered", this.draining);
        }

        public void RefreshBlink()
        {
            this.blinkReady = true;
        }

        public void IncrementWallJump()
        {
            this.wallJumpCounter++;

            Action<int> action = onWallJumpIncremented;
            if (action == null) return;
            action(this.wallJumpCounter);
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

            NetworkIdentity identity = this.GetComponent<NetworkIdentity>();
            if (!identity) return;

            new SyncBloodWell(identity.netId, (ulong)(this.meter * 100f)).Send(NetworkDestination.Server);
        }

        public void ActivateDrain()
        {
            this.wasBloodWellFilled = true;
            this.meter = 100f;
            this.draining = true;

            this.steamEffect.Play();
            this.chargeEffect.Play();
            this.blackElectricityEffect.Play();

            this.storedHealth = this.healthComponent.fullHealth;// * 0.75f;
            this.iDontEvenKnowAnymore = this.storedHealth / (100f / this.drainRate);

            Util.PlaySound("sfx_ravager_bloodrush", this.gameObject);
            this.playID = Util.PlaySound("sfx_ravager_steam_loop", this.gameObject);

            //Modules.Assets.drainTextEffect
            EffectManager.SpawnEffect(this.skinDef.bloodRushActivationEffectPrefab, new EffectData
            {
                origin = this.transform.position + (Vector3.up * 1.8f),
                rotation = Quaternion.identity,
                scale = 1f
            }, false);

            Transform modelTransform = this.characterBody.modelLocator.modelTransform;
            if (modelTransform)
            {
                TemporaryOverlay temporaryOverlay = modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1f;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/matOnFire.mat").WaitForCompletion();
                temporaryOverlay.inspectorCharacterModel = modelTransform.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
            }
        }

        public void CalcStoredHealth(bool isEmpowered)
        {
            if (isEmpowered) this.storedHealth = this.healthComponent.fullHealth * Util.Remap(this.meter, 0f, 100f, 0f, 1f);
            else this.storedHealth = this.healthComponent.fullHealth * Util.Remap(this.meter, 0f, 100f, 0f, 0.5f);
        }

        public void ApplySkin()
        {
            if (this.skinController)
            {
                this.cachedSkinDef = RavagerSkinCatalog.GetSkin(this.skinController.skins[this.skinController.currentSkinIndex].nameToken);

                this.childLocator.FindChild("BlackElectricity").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = this.cachedSkinDef.swordElectricityMat;
                this.childLocator.FindChild("SwordElectricity").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = this.cachedSkinDef.electricityMat;
                this.childLocator.FindChild("FootChargeL").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = this.cachedSkinDef.electricityMat;
                this.childLocator.FindChild("FootChargeR").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = this.cachedSkinDef.electricityMat;
                this.childLocator.FindChild("ArmCharge").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = this.cachedSkinDef.electricityMat;
                this.childLocator.FindChild("HandElectricity").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = this.cachedSkinDef.electricityMat;

                foreach (Light i in this.childLocator.gameObject.GetComponentsInChildren<Light>())
                {
                    if (i)
                    {
                        i.color = this.cachedSkinDef.glowColor;
                    }
                }
            }
            else this.cachedSkinDef = RavagerSkinCatalog.GetSkin(0);
        }
    }
}
using EntityStates;
using EntityStates.CaptainDefenseMatrixItem;
using RedGuyMod.Content;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ChargeBeam : BaseRavagerSkillState
    {
        public float baseDuration = 15f;
        public static float maxDamageCoefficient = 24f;
        public static float minDamageCoefficient = 2f;

        private float duration;
        private float stopwatch;
        private GameObject chargeEffectInstance;
        private uint playId;
        private bool isCharged;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;

            base.PlayCrossfade("Gesture, Override", "ChargeBeam", "Beam.playbackRate", this.duration, 0.05f);

            this.chargeEffectInstance = GameObject.Instantiate(Modules.Assets.beamChargeEffect);
            this.chargeEffectInstance.transform.parent = this.FindModelChild("HandL");
            this.chargeEffectInstance.transform.localPosition = new Vector3(-0.5f, 0f, -0.2f);
            this.chargeEffectInstance.transform.localRotation = Quaternion.identity;

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            Util.PlaySound("sfx_ravager_charge_beam", this.gameObject);
            this.playId = Util.PlaySound("sfx_ravager_beam_loop", this.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("Gesture, Override", "FireBeam", "Beam.playbackRate", 1f);

            if (this.chargeEffectInstance) Destroy(this.chargeEffectInstance);
            if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            AkSoundEngine.StopPlayingID(this.playId);
        }

        private float charge
        {
            get
            {
                float value = Mathf.Clamp(this.stopwatch, 0f, this.duration);
                return Util.Remap(value, 0f, this.duration, 0f, 1f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if (this.empowered) this.stopwatch += 4f * Time.fixedDeltaTime;
            this.characterBody.outOfCombatStopwatch = 0f;
            this.StartAimMode(0.5f);
            this.characterBody.isSprinting = false;

            if (!this.isCharged)
            {
                if (this.charge >= 1f)
                {
                    this.isCharged = true;
                    Util.PlaySound("sfx_ravager_beam_maxcharge", this.gameObject);
                }
            }

            this.chargeEffectInstance.transform.localScale = Vector3.one * Util.Remap(this.charge, 0f, 1f, 0f, 2f);

            if (NetworkServer.active)
            {
                this.CheckForProjectiles();
            }

            if (base.isAuthority)
            {
                if (!this.inputBank.skill3.down)
                {
                    this.FireBeam();
                    this.outer.SetNextStateToMain();
                    return;
                }
            }
        }

        protected virtual void CheckForProjectiles()
        {
            Collider[] array = Physics.OverlapSphere(this.chargeEffectInstance.transform.position, 12f, LayerIndex.projectile.mask);

            int projectileCount = 0;

            for (int i = 0; i < array.Length; i++)
            {
                ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                if (pc)
                {
                    if (pc.owner != this.gameObject)
                    {
                        Util.PlaySound("sfx_ravager_beam_consume", pc.gameObject);
                        projectileCount++;
                        this.stopwatch += 2f;
                        Destroy(pc.gameObject);

                        Vector3 position = this.chargeEffectInstance.transform.position;
                        Vector3 start = pc.transform.position;
                        if (DefenseMatrixOn.tracerEffectPrefab)
                        {
                            EffectData effectData = new EffectData
                            {
                                origin = position,
                                start = start
                            };
                            EffectManager.SpawnEffect(DefenseMatrixOn.tracerEffectPrefab, effectData, true);
                        }
                    }
                }
            }
        }

        protected virtual void FireBeam()
        {
            float storedCharge = this.charge;

            float recoilAmplitude = Util.Remap(storedCharge, 0f, 1f, 1f, 16f);// / this.attackSpeedStat;

            base.AddRecoil(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
            this.characterBody.AddSpreadBloom(4f);
            EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/MuzzleflashGolem.prefab").WaitForCompletion(), gameObject, "HandL", false);

            GameObject tracer = null;
            GameObject impact = null;

            tracer = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunSuper.prefab").WaitForCompletion();
            impact = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolem.prefab").WaitForCompletion();

            /*if (storedCharge < 0.33f)
            {
                tracer = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgun.prefab").WaitForCompletion();
                impact = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/ImpactRailgun.prefab").WaitForCompletion();
            }
            else if (storedCharge >= 0.33f && storedCharge < 0.95f)
            {
                tracer = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunSuper.prefab").WaitForCompletion();
                impact = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolem.prefab").WaitForCompletion();
            }
            else
            {
                tracer = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/TracerGolem.prefab").WaitForCompletion();
                impact = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolemDeath.prefab").WaitForCompletion();
            }*/

            //fire_beam
            Util.PlaySound("sfx_ravager_explosion", this.gameObject);

            if (base.isAuthority)
            {
                float damage = Util.Remap(storedCharge, 0f, 1f, ChargeBeam.minDamageCoefficient, ChargeBeam.maxDamageCoefficient) * this.damageStat;

                Ray aimRay = GetAimRay();

                BulletAttack bulletAttack = new BulletAttack
                {
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = damage,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Stun1s,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = 2000f,
                    force = Util.Remap(storedCharge, 0f, 1f, 5f, 5000f),
                    hitMask = LayerIndex.CommonMasks.bullet,
                    isCrit = this.RollCrit(),
                    owner = this.gameObject,
                    muzzleName = "HandL",
                    smartCollision = true,
                    procChainMask = default,
                    procCoefficient = 1f,
                    radius = Util.Remap(storedCharge, 0f, 1f, 0f, 5f),
                    sniper = false,
                    stopperMask = LayerIndex.noCollision.mask,
                    weapon = null,
                    tracerEffectPrefab = tracer,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = impact,
                    HitEffectNormal = impact,
                    maxSpread = 0f,
                    minSpread = 0f,
                    bulletCount = 1
                };
                bulletAttack.Fire();

                //this.characterMotor.ApplyForce(aimRay.direction * -this.selfForce);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
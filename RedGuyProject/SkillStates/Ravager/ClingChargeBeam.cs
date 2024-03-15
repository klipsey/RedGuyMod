using EntityStates;
using EntityStates.CaptainDefenseMatrixItem;
using RedGuyMod.Content;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.CameraTargetParams;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ClingChargeBeam : BaseRavagerSkillState
    {
        public float baseDuration = 30f;
        public static float maxDamageCoefficient = 24f;
        public static float minDamageCoefficient = 2f;
        public float minRadius = 0.1f;
        public float maxRadius = 14f;

        private float duration;
        private float stopwatch;
        private GameObject chargeEffectInstance;
        private uint playId;
        private bool isCharged;
        private Animator animator;
        private float fuckMyAss;

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private bool allySucc;

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.penis) this.penis.projectilesDeleted = 0;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.allySucc = Modules.Config.allySucc.Value;
            this.animator = this.GetModelAnimator();

            base.PlayCrossfade("FullBody, Override", "ClingChargeBeam", "Beam.playbackRate", this.duration * 0.1f, 0.05f);

            this.chargeEffectInstance = GameObject.Instantiate(Modules.Assets.beamChargeEffect);
            this.chargeEffectInstance.transform.parent = this.FindModelChild("HandL");
            this.chargeEffectInstance.transform.localPosition = new Vector3(-0.5f, 0f, -0.2f);
            this.chargeEffectInstance.transform.localRotation = Quaternion.identity;

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(this.characterBody, Modules.Assets.clingBeamCrosshair, CrosshairUtils.OverridePriority.PrioritySkill);

            Util.PlaySound("sfx_ravager_charge_beam", this.gameObject);
            this.playId = Util.PlaySound("sfx_ravager_beam_loop", this.gameObject);
        }

        public override void OnExit()
        {
            base.OnExit();

            base.PlayAnimation("FullBody, Override", "ClingSlash1", "Slash.playbackRate", 1f);

            if (this.crosshairOverrideRequest != null) this.crosshairOverrideRequest.Dispose();

            if (this.chargeEffectInstance) Destroy(this.chargeEffectInstance);

            if (NetworkServer.active)
            {
                this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

                int j = this.characterBody.GetBuffCount(Content.Survivors.RedGuy.projectileEatedBuff);
                for (int i = 0; i < j; i++)
                {
                    this.characterBody.RemoveBuff(Content.Survivors.RedGuy.projectileEatedBuff);
                }
            }

            AkSoundEngine.StopPlayingID(this.playId);

            this.penis.projectilesDeleted = 0;
        }

        private float charge
        {
            get
            {
                float value = Mathf.Clamp(this.stopwatch + (this.characterBody.GetBuffCount(Content.Survivors.RedGuy.projectileEatedBuff) * 1.25f), 0f, this.duration);
                return Util.Remap(value, 0f, this.duration, 0f, 1f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;
            if (this.empowered) this.stopwatch += 10f * Time.fixedDeltaTime;
            this.characterBody.outOfCombatStopwatch = 0f;
            this.penis.clingTimer += Time.fixedDeltaTime;
            this.StartAimMode(0.5f);
            this.characterBody.isSprinting = false;

            if (this.animator)
            {
                float beamType = 0f;
                if (this.inputBank.skill2.down)
                {
                    beamType = -1f;
                }
                else if (this.inputBank.skill1.down)
                {
                    beamType = 1f;
                }

                this.fuckMyAss = Mathf.Lerp(this.fuckMyAss, beamType, Time.fixedDeltaTime * 8f);
                this.animator.SetFloat("beamType", this.fuckMyAss);
            }


            if (!this.isCharged)
            {
                if (this.charge >= 1f)
                {
                    this.isCharged = true;
                    Util.PlaySound("sfx_ravager_beam_maxcharge", this.gameObject);
                    base.PlayCrossfade("FullBody, Override", "ClingChargeBeamMax", "Beam.playbackRate", 0.5f, 0.05f);
                }
            }

            this.CheckForProjectiles();

            this.chargeEffectInstance.transform.localScale = Vector3.one * Util.Remap(this.charge, 0f, 1f, 0f, 2f);
            this.penis.chargeValue = this.charge;

            if (base.isAuthority)
            {
                if (!this.inputBank.skill3.down)
                {
                    this.FireBeam();
                    return;
                }
            }
        }

        protected virtual void EatProjectile(ProjectileController pc)
        {
            Util.PlaySound("sfx_ravager_beam_consume", pc.gameObject);

            if (NetworkServer.active)
            {
                this.characterBody.AddBuff(Content.Survivors.RedGuy.projectileEatedBuff);
                this.penis.projectilesDeleted++;

                if (base.fixedAge <= 0.5f)
                {
                    this.characterBody.AddBuff(Content.Survivors.RedGuy.projectileEatedBuff);
                    this.penis.projectilesDeleted++;

                    this.characterBody.AddBuff(Content.Survivors.RedGuy.projectileEatedBuff);
                    this.penis.projectilesDeleted++;

                    this.characterBody.AddBuff(Content.Survivors.RedGuy.projectileEatedBuff);
                    this.penis.projectilesDeleted++;
                }

                GameObject.Destroy(pc.gameObject);

                if (DefenseMatrixOn.tracerEffectPrefab)
                {
                    EffectData effectData = new EffectData
                    {
                        origin = this.chargeEffectInstance.transform.position,
                        start = pc.transform.position
                    };
                    EffectManager.SpawnEffect(DefenseMatrixOn.tracerEffectPrefab, effectData, true);
                }
            }
        }

        protected virtual void CheckForProjectiles()
        {
            Collider[] array = Physics.OverlapSphere(this.chargeEffectInstance.transform.position, 12f, LayerIndex.projectile.mask);

            for (int i = 0; i < array.Length; i++)
            {
                ProjectileController pc = array[i].GetComponentInParent<ProjectileController>();
                if (pc)
                {
                    if (pc.owner != this.gameObject)
                    {
                        if (!this.allySucc)
                        {
                            if (pc.teamFilter.teamIndex != this.GetTeam())
                            {
                                this.EatProjectile(pc);
                            }
                        }
                        else
                        {
                            this.EatProjectile(pc);
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
            if (charge >= 0.75f) EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion(), gameObject, "HandL", true);
            else if (charge >= 0.25f) EffectManager.SimpleMuzzleFlash(Modules.Assets.cssEffect, gameObject, "HandL", true);
            else EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/MuzzleflashGolem.prefab").WaitForCompletion(), gameObject, "HandL", true);
            Util.PlaySound("sfx_ravager_blast", this.gameObject);

            GameObject tracer = null;
            GameObject impact = null;

            tracer = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunSuper.prefab").WaitForCompletion();
            impact = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolem.prefab").WaitForCompletion();

            if (storedCharge >= 0.5f)
            {
                tracer.transform.GetChild(4).GetChild(3).localScale = Vector3.one * 6f;
            }

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

            this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}
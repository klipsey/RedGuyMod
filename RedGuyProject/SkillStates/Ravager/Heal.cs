using RedGuyMod.Content;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RedGuyMod.SkillStates.Ravager
{
    public class Heal : BaseRavagerSkillState
    {
        public static float maxDamageCoefficient = 24f;
        public static float minDamageCoefficient = 6f;

        public override void OnEnter()
        {
            base.OnEnter();

            float charge = 0f;

            if (this.penis)
            {
                charge = Util.Remap(this.penis.meter, 0f, 100f, 0f, 1f);

                Util.PlaySound(this.penis.skinDef.healSoundString, this.gameObject);
                this.penis.CalcStoredHealth(this.empowered);

                float amount = this.penis.storedHealth;
                this.penis.meter = 0f;
                this.penis.storedHealth = 0f;

                if (charge > 0f)
                {
                    if (this.empowered)
                    {
                        base.PlayCrossfade("Gesture, Override", "CoagulateBlast", "Heal.playbackRate", 1.5f, 0.05f);
                    }
                    else
                    {
                        if (charge > 0.5f) base.PlayCrossfade("Gesture, Override", "Coagulate", "Heal.playbackRate", 1.25f, 0.05f);
                        else base.PlayCrossfade("Gesture, Override", "CoagulateLow", "Heal.playbackRate", 0.8f, 0.05f);
                    }
                }

                if (amount > 0f)
                {
                    if (this.empowered)
                    {
                        if (NetworkServer.active)
                        {
                            this.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 1.5f);
                        }

                        if (base.isAuthority)
                        {
                            BlastAttack.Result result = new BlastAttack
                            {
                                attacker = base.gameObject,
                                procChainMask = default(ProcChainMask),
                                impactEffect = EffectIndex.Invalid,
                                losType = BlastAttack.LoSType.None,
                                damageColorIndex = DamageColorIndex.Default,
                                damageType = DamageType.Stun1s,
                                procCoefficient = 1f,
                                bonusForce = 400 * Vector3.up,
                                baseForce = 2000f,
                                baseDamage = Util.Remap(charge, 0f, 1f, Heal.minDamageCoefficient, Heal.maxDamageCoefficient) * this.damageStat,
                                falloffModel = BlastAttack.FalloffModel.None,
                                radius = 16f,
                                position = this.characterBody.corePosition,
                                attackerFiltering = AttackerFiltering.NeverHitSelf,
                                teamIndex = base.GetTeam(),
                                inflictor = base.gameObject,
                                crit = base.RollCrit()
                            }.Fire();
                        }

                        Util.PlaySound("sfx_ravager_explosion", this.gameObject);

                        EffectManager.SpawnEffect(this.penis.skinDef.bloodBombEffectPrefab, new EffectData
                        {
                            origin = this.transform.position + (Vector3.up * 1.8f),
                            rotation = Quaternion.identity,
                            scale = 1f
                        }, false);
                    }

                    ConsumeOrb orb = new ConsumeOrb();
                    orb.origin = this.FindModelChild("HandR").position;
                    orb.target = this.characterBody.mainHurtBox;
                    orb.healOverride = amount;
                    OrbManager.instance.AddOrb(orb);
                }
            }

            if (base.isAuthority)
            {
                if (!this.isGrounded)
                {
                    if (this.empowered) this.SmallHop(this.characterMotor, Util.Remap(charge, 0f, 1f, 2f, 8f));
                    else this.SmallHop(this.characterMotor, 15f);
                }
            }

            this.outer.SetNextStateToMain();
        }
    }
}
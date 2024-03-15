using RedGuyMod.Content;
using RoR2;
using RoR2.Orbs;
using UnityEngine;
using UnityEngine.Networking;

namespace RedGuyMod.SkillStates.Ravager
{
    public class ClingHeal : BaseRavagerSkillState
    {
        public static float maxDamageCoefficient = 24f;
        public static float minDamageCoefficient = 6f;

        private float amount;
        private float charge;

        public override void OnEnter()
        {
            base.OnEnter();

            if (this.penis)
            {
                Util.PlaySound(this.penis.skinDef.healSoundString, this.gameObject);

                if (base.isAuthority)
                {
                    this.charge = Util.Remap(this.penis.meter, 0f, 100f, 0f, 1f);
                    this.amount = Util.Remap(this.penis.meter, 0f, 100f, 0f, this.healthComponent.fullHealth * 0.5f);
                }
                this.penis.meter = 0f;
                this.penis.storedHealth = 0f;

                if (charge > 0f)
                {
                    if (this.empowered)
                    {
                        base.PlayCrossfade("FullBody, Override", "ClingHealBlast", "Heal.playbackRate", 1.5f, 0.05f);
                    }
                    else
                    {
                        if (charge > 0.5f) base.PlayCrossfade("FullBody, Override", "ClingHeal", "Heal.playbackRate", 1.25f, 0.05f);
                        else base.PlayCrossfade("FullBody, Override", "ClingHealLow", "Heal.playbackRate", 0.8f, 0.05f);
                    }
                }

                if (this.amount > 0f)
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
                                baseDamage = Util.Remap(charge, 0f, 1f, Heal.maxDamageCoefficient, Heal.maxDamageCoefficient) * this.damageStat,
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

                    if (NetworkServer.active)
                    {
                        ConsumeOrb orb = new ConsumeOrb();
                        orb.origin = this.FindModelChild("HandR").position;
                        orb.target = this.characterBody.mainHurtBox;
                        orb.healOverride = amount;
                        OrbManager.instance.AddOrb(orb);
                    }
                }
            }

            if (base.isAuthority)
            {
                if (!this.isGrounded)
                {
                    if (!this.empowered) this.SmallHop(this.characterMotor, Util.Remap(charge, 0f, 1f, 8f, 12f));
                    else this.SmallHop(this.characterMotor, 16f);
                }
            }

            this.outer.SetNextStateToMain();
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.amount);
            writer.Write(this.charge);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.amount = reader.ReadSingle();
            this.charge = reader.ReadSingle();
        }
    }
}
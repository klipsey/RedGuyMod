using RedGuyMod.Content;
using RoR2;
using RoR2.Orbs;
using UnityEngine.Networking;

namespace RedGuyMod.SkillStates.Ravager
{
    public class Heal : BaseRavagerSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            float charge = 0f;

            if (this.penis)
            {
                charge = Util.Remap(this.penis.meter, 0f, 100f, 0f, 1f);

                Util.PlaySound("sfx_ravager_steam", this.gameObject);
                this.penis.CalcStoredHealth(this.empowered);

                float amount = this.penis.storedHealth;
                this.penis.meter = 0f;
                this.penis.storedHealth = 0f;

                if (amount > 0f)
                {
                    if (this.empowered)
                    {
                        if (NetworkServer.active)
                        {
                            this.characterBody.AddTimedBuff(RoR2Content.Buffs.HiddenInvincibility, 1.5f);
                        }
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
                    this.SmallHop(this.characterMotor, Util.Remap(charge, 0f, 1f, 2f, 8f));
                }
            }

            this.outer.SetNextStateToMain();
        }
    }
}
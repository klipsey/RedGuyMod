using UnityEngine;
using RoR2;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace RedGuyMod.Content.Components
{
    public class RedGuyPunchBarrage : MonoBehaviour
    {
        public GameObject attacker;
		public Transform targetTransform;
        public float damage;
		public bool isCrit;

        public int punchCount = 10;

        public float timeBetweenPunches = 0.08f;
        private float punchStopwatch;
		private ProjectileController projectileController;
		private ProjectileDamage projectileDamage;

		private void Start()
        {
			this.projectileController = this.GetComponent<ProjectileController>();
			this.projectileDamage = this.GetComponent<ProjectileDamage>();

			this.attacker = this.projectileController.owner;
			this.damage = this.projectileDamage.damage;
			this.isCrit = this.projectileDamage.crit;
			this.targetTransform = this.attacker.GetComponent<RedGuyController>().punchTarget;
        }

        private void FixedUpdate()
        {
			this.punchStopwatch -= Time.fixedDeltaTime;

			if (!this.attacker || !this.targetTransform || this.punchCount <= 0)
            {
				Destroy(this.gameObject);
				return;
            }

            if (this.punchStopwatch <= 0f)
            {
				this.Punch();
            }
        }

        private void Punch()
        {
			this.punchCount--;
			this.punchStopwatch = this.timeBetweenPunches;

			if (NetworkServer.active)
            {
				BlastAttack.Result result = new BlastAttack
				{
					attacker = this.attacker,
					procChainMask = default(ProcChainMask),
					impactEffect = EffectIndex.Invalid,
					losType = BlastAttack.LoSType.None,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					procCoefficient = 1f,
					bonusForce = Vector3.zero,
					baseForce = 0f,
					baseDamage = this.damage,
					falloffModel = BlastAttack.FalloffModel.None,
					radius = 0.3f,
					position = this.targetTransform.position,
					attackerFiltering = AttackerFiltering.NeverHitSelf,
					teamIndex = this.projectileController.teamFilter.teamIndex,
					inflictor = this.attacker,
					crit = this.isCrit
				}.Fire();
			}

			EffectManager.SpawnEffect(Modules.Assets.punchImpactEffect, new EffectData
			{
				origin = this.targetTransform.position,
				scale = 3f
			}, false);

			Util.PlaySound("sfx_ravager_punch_barrage", this.gameObject);
		}
    }
}
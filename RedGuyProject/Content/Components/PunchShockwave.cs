using System;
using System.Collections.Generic;
using System.Linq;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RedGuyMod.Content.Components
{
	[RequireComponent(typeof(ProjectileDamage))]
	[RequireComponent(typeof(ProjectileController))]
	public class PunchShockwave : MonoBehaviour
	{
		public int attackFireCount = 1;
		public float attackInterval = 1f;
		public float listClearInterval = 3f;
		public float attackRange = 20f;
		[Range(0f, 180f)]
		public float minAngleFilter;
		[Range(0f, 180f)]
		public float maxAngleFilter = 180f;
		public float procCoefficient = 0.1f;
		public float damageCoefficient = 1f;
		public int bounces;
		public bool inheritDamageType;

		private ProjectileController projectileController;
		private ProjectileDamage projectileDamage;
		private List<HealthComponent> previousTargets;
		private TeamFilter teamFilter;
		private float attackTimer;
		private float listClearTimer;
		private BullseyeSearch search;
		private float stopwatch;

		private TeamIndex myTeamIndex
		{
			get
			{
				if (!this.teamFilter)
				{
					return TeamIndex.Neutral;
				}
				return this.teamFilter.teamIndex;
			}
		}

		private void Awake()
		{
			if (NetworkServer.active)
			{
				this.projectileController = base.GetComponent<ProjectileController>();
				this.teamFilter = this.projectileController.teamFilter;
				this.projectileDamage = base.GetComponent<ProjectileDamage>();
				this.attackTimer = 0f;
				this.previousTargets = new List<HealthComponent>();
				this.search = new BullseyeSearch();
				return;
			}
			base.enabled = false;
		}

		private void ClearList()
		{
			this.previousTargets.Clear();
		}

		private void FixedUpdate()
		{
			this.stopwatch += Time.fixedDeltaTime;
			if (NetworkServer.active)
			{
				this.UpdateServer();
				return;
			}
			base.enabled = false;
		}

		private void UpdateServer()
		{
			if (this.stopwatch <= 0.25f) return;

			this.listClearTimer -= Time.fixedDeltaTime;
			if (this.listClearTimer <= 0f)
			{
				this.ClearList();
				this.listClearTimer = this.listClearInterval;
			}
			this.attackTimer -= Time.fixedDeltaTime;
			if (this.attackTimer <= 0f)
			{
				this.attackTimer += this.attackInterval;
				Vector3 position = base.transform.position;
				Vector3 forward = base.transform.forward;
				for (int i = 0; i < this.attackFireCount; i++)
				{
					HurtBox hurtBox = this.FindNextTarget(position, forward);
					if (hurtBox)
					{
						this.previousTargets.Add(hurtBox.healthComponent);
						RavLightningOrb lightningOrb = new RavLightningOrb();
						lightningOrb.bouncedObjects = new List<HealthComponent>();
						lightningOrb.attacker = this.projectileController.owner;
						lightningOrb.inflictor = base.gameObject;
						lightningOrb.teamIndex = this.myTeamIndex;
						lightningOrb.damageValue = this.projectileDamage.damage * this.damageCoefficient;
						lightningOrb.isCrit = this.projectileDamage.crit;
						lightningOrb.origin = position;
						lightningOrb.bouncesRemaining = this.bounces;
						lightningOrb.procCoefficient = this.procCoefficient;
						lightningOrb.target = hurtBox;
						lightningOrb.damageColorIndex = this.projectileDamage.damageColorIndex;
						if (this.inheritDamageType)
						{
							lightningOrb.damageType = this.projectileDamage.damageType;
						}
						OrbManager.instance.AddOrb(lightningOrb);
					}
				}
			}
		}

		public HurtBox FindNextTarget(Vector3 position, Vector3 forward)
		{
			this.search.searchOrigin = position;
			this.search.searchDirection = forward;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.myTeamIndex);
			this.search.filterByLoS = false;
			this.search.minAngleFilter = this.minAngleFilter;
			this.search.maxAngleFilter = this.maxAngleFilter;
			this.search.maxDistanceFilter = this.attackRange;
			this.search.RefreshCandidates();
			return this.search.GetResults().FirstOrDefault((HurtBox hurtBox) => !this.previousTargets.Contains(hurtBox.healthComponent));
		}
	}
}
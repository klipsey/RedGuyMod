﻿using RoR2;
using RoR2.Orbs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RedGuyMod.Content
{
	public class RavLightningOrb : Orb
	{
		public float speed = 100f;
		public float damageValue;
		public GameObject attacker;
		public GameObject inflictor;
		public int bouncesRemaining;
		public List<HealthComponent> bouncedObjects;
		public TeamIndex teamIndex;
		public bool isCrit;
		public ProcChainMask procChainMask;
		public float procCoefficient = 1f;
		public DamageColorIndex damageColorIndex;
		public float range = 20f;
		public float damageCoefficientPerBounce = 1f;
		public int targetsToFindPerBounce = 1;
		public DamageType damageType;

		private bool canBounceOnSameTarget;
		private bool failedToKill;
		private BullseyeSearch search;

		public override void Begin()
		{
			base.duration = 0.1f;

			EffectData effectData = new EffectData
			{
				origin = this.origin,
				genericFloat = base.duration
			};
			effectData.SetHurtBoxReference(this.target);

			EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OrbEffects/LightningOrbEffect"), effectData, true);
			//Time.timeScale = 0f;
		}

		public override void OnArrival()
		{
			if (this.target)
			{
				HealthComponent healthComponent = this.target.healthComponent;
				if (healthComponent)
				{
					DamageInfo damageInfo = new DamageInfo();
					damageInfo.damage = this.damageValue;
					damageInfo.attacker = this.attacker;
					damageInfo.inflictor = this.inflictor;
					damageInfo.force = Vector3.zero;
					damageInfo.crit = this.isCrit;
					damageInfo.procChainMask = this.procChainMask;
					damageInfo.procCoefficient = this.procCoefficient;
					damageInfo.position = this.target.transform.position;
					damageInfo.damageColorIndex = this.damageColorIndex;
					damageInfo.damageType = this.damageType;
					healthComponent.TakeDamage(damageInfo);
					GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
					GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
				}
				this.failedToKill |= (!healthComponent || healthComponent.alive);
				if (this.bouncesRemaining > 0)
				{
					for (int i = 0; i < this.targetsToFindPerBounce; i++)
					{
						if (this.bouncedObjects != null)
						{
							if (this.canBounceOnSameTarget)
							{
								this.bouncedObjects.Clear();
							}
							this.bouncedObjects.Add(this.target.healthComponent);
						}
						HurtBox hurtBox = this.PickNextTarget(this.target.transform.position);
						if (hurtBox)
						{
							RavLightningOrb lightningOrb = new RavLightningOrb();
							lightningOrb.search = this.search;
							lightningOrb.origin = this.target.transform.position;
							lightningOrb.target = hurtBox;
							lightningOrb.attacker = this.attacker;
							lightningOrb.inflictor = this.inflictor;
							lightningOrb.teamIndex = this.teamIndex;
							lightningOrb.damageValue = this.damageValue * this.damageCoefficientPerBounce;
							lightningOrb.bouncesRemaining = this.bouncesRemaining - 1;
							lightningOrb.isCrit = this.isCrit;
							lightningOrb.bouncedObjects = this.bouncedObjects;
							lightningOrb.procChainMask = this.procChainMask;
							lightningOrb.procCoefficient = this.procCoefficient;
							lightningOrb.damageColorIndex = this.damageColorIndex;
							lightningOrb.damageCoefficientPerBounce = this.damageCoefficientPerBounce;
							lightningOrb.speed = this.speed;
							lightningOrb.range = this.range;
							lightningOrb.damageType = this.damageType;
							lightningOrb.failedToKill = this.failedToKill;
							OrbManager.instance.AddOrb(lightningOrb);
						}
					}
					return;
				}
			}
		}

		public HurtBox PickNextTarget(Vector3 position)
		{
			if (this.search == null)
			{
				this.search = new BullseyeSearch();
			}
			this.search.searchOrigin = position;
			this.search.searchDirection = Vector3.zero;
			this.search.teamMaskFilter = TeamMask.allButNeutral;
			this.search.teamMaskFilter.RemoveTeam(this.teamIndex);
			this.search.filterByLoS = false;
			this.search.sortMode = BullseyeSearch.SortMode.Distance;
			this.search.maxDistanceFilter = this.range;
			this.search.RefreshCandidates();
			HurtBox hurtBox = (from v in this.search.GetResults()
							   where !this.bouncedObjects.Contains(v.healthComponent)
							   select v).FirstOrDefault<HurtBox>();
			if (hurtBox)
			{
				this.bouncedObjects.Add(hurtBox.healthComponent);
			}
			return hurtBox;
		}
	}
}
using EntityStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RedGuyMod.SkillStates.Ravager
{
	public class Blink : BaseRavagerState
	{
		public float duration = 0.15f;
		public float speedCoefficient = 10f;
		public float healthCostFraction = 0.1f;

		private Transform modelTransform;
		private float stopwatch;
		private Vector3 blinkVector = Vector3.zero;
		private CharacterModel characterModel;
        private HurtBoxGroup hurtboxGroup;

        public override void OnEnter()
		{
			base.OnEnter();
			Util.PlaySound(EntityStates.ImpMonster.BlinkState.beginSoundString, this.gameObject);
			this.modelTransform = base.GetModelTransform();

			if (this.modelTransform)
			{
				this.characterModel = this.modelTransform.GetComponent<CharacterModel>();
				this.hurtboxGroup = this.modelTransform.GetComponent<HurtBoxGroup>();
			}

			if (this.characterModel)
			{
				this.characterModel.invisibilityCount++;
			}

			if (this.hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter + 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}

			this.blinkVector = this.GetBlinkVector();
			this.CreateBlinkEffect(Util.GetCorePosition(this.gameObject));

			if (NetworkServer.active && this.healthComponent && !this.penis.draining)
			{
				DamageInfo damageInfo = new DamageInfo();
				damageInfo.damage = this.healthComponent.combinedHealth * this.healthCostFraction;
				damageInfo.position = this.characterBody.corePosition;
				damageInfo.force = Vector3.zero;
				damageInfo.damageColorIndex = DamageColorIndex.Default;
				damageInfo.crit = false;
				damageInfo.attacker = null;
				damageInfo.inflictor = null;
				damageInfo.damageType = (DamageType.NonLethal | DamageType.BypassArmor);
				damageInfo.procCoefficient = 0f;
				damageInfo.procChainMask = default(ProcChainMask);
				base.healthComponent.TakeDamage(damageInfo);
			}
		}

		protected virtual Vector3 GetBlinkVector()
		{
			return base.inputBank.aimDirection;
		}

		private void CreateBlinkEffect(Vector3 origin)
		{
			EffectData effectData = new EffectData();
			effectData.rotation = Util.QuaternionSafeLookRotation(this.blinkVector);
			effectData.origin = origin;
			EffectManager.SpawnEffect(EntityStates.ImpMonster.BlinkState.blinkPrefab, effectData, false);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (base.characterMotor && base.characterDirection)
			{
				base.characterMotor.velocity = Vector3.zero;
				base.characterMotor.rootMotion += this.blinkVector * (this.moveSpeedStat * this.speedCoefficient * Time.fixedDeltaTime);
			}
			if (this.stopwatch >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
			}
		}

		public override void OnExit()
		{
			if (!this.outer.destroying)
			{
				//Util.PlaySound(this.endSoundString, base.gameObject);
				this.CreateBlinkEffect(Util.GetCorePosition(base.gameObject));

				if (this.modelTransform)
				{
					TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
					temporaryOverlay.duration = 0.6f;
					temporaryOverlay.animateShaderAlpha = true;
					temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
					temporaryOverlay.destroyComponentOnEnd = true;
					temporaryOverlay.originalMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDissolve.mat").WaitForCompletion();
					temporaryOverlay.AddToCharacerModel(this.modelTransform.GetComponent<CharacterModel>());
				}
			}

			if (this.characterModel)
			{
				this.characterModel.invisibilityCount--;
			}

			if (this.hurtboxGroup)
			{
				HurtBoxGroup hurtBoxGroup = this.hurtboxGroup;
				int hurtBoxesDeactivatorCounter = hurtBoxGroup.hurtBoxesDeactivatorCounter - 1;
				hurtBoxGroup.hurtBoxesDeactivatorCounter = hurtBoxesDeactivatorCounter;
			}

			if (base.characterMotor)
			{
				base.characterMotor.disableAirControlUntilCollision = false;
			}

			base.OnExit();
		}

        public override InterruptPriority GetMinimumInterruptPriority()
        {
			return InterruptPriority.Skill;
        }
    }
}
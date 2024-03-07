using UnityEngine;
using RoR2;
using EntityStates;
using RedGuyMod.Modules;
using RedGuyMod.SkillStates.Emote;
using BepInEx.Configuration;
using RedGuyMod.Content.Components;
using UnityEngine.AddressableAssets;

namespace RedGuyMod.SkillStates.Ravager
{
	public class MainState : GenericCharacterMain
	{
		private Animator animator;
		public LocalUser localUser;

		private RedGuyController penis;
		private float landingTimer;
		private float _lastYSpeed;
		private bool wasGrounded2;

		public override void OnEnter()
		{
			base.OnEnter();
            this.animator = this.modelAnimator;
			this.penis = this.GetComponent<RedGuyController>();
            this.FindLocalUser();
		}

		public override void FixedUpdate()
		{
			if (!this.isGrounded && this.characterMotor.velocity.y <= -5f) this._lastYSpeed = this.characterMotor.velocity.y;

			if (this.isGrounded && !this.wasGrounded2)
			{
				//Chat.AddMessage(this._lastYSpeed.ToString());
				this.Landing(this._lastYSpeed);
			}

			this.wasGrounded2 = this.isGrounded;

			base.FixedUpdate();
			this.landingTimer -= Time.fixedDeltaTime;

			if (this.penis.skibidi)
            {
				this.penis.skibidi = false;
				if (this.landingTimer > 0f)
                {
					this.landingTimer = 0f;
					base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
				}
            }

			if (this.animator)
			{
				bool cock = false;
				if (!this.characterBody.outOfDanger || !this.characterBody.outOfCombat) cock = true;

				this.animator.SetBool("inCombat", cock);

				if (this.isGrounded) this.animator.SetFloat("airBlend", 0f);
				else this.animator.SetFloat("airBlend", 1f);
			}

			//emotes
			if (base.isAuthority && base.characterMotor.isGrounded)
			{
				this.CheckEmote<Rest>(Config.restKey);
				this.CheckEmote<Taunt>(Config.tauntKey);
				this.CheckEmote<Dance>(Config.danceKey);
			}
		}

		private void CheckEmote(KeyCode keybind, EntityState state)
		{
			if (Input.GetKeyDown(keybind))
			{
				if (!localUser.isUIFocused)
				{
					outer.SetInterruptState(state, InterruptPriority.Any);
				}
			}
		}

		private void CheckEmote<T>(ConfigEntry<KeyboardShortcut> keybind) where T : EntityState, new()
		{
			if (Modules.Config.GetKeyPressed(keybind))
			{
				FindLocalUser();

				if (localUser != null && !localUser.isUIFocused)
				{
					outer.SetInterruptState(new T(), InterruptPriority.Any);
				}
			}
		}

		private void FindLocalUser()
		{
			if (localUser == null)
			{
				if (base.characterBody)
				{
					foreach (LocalUser lu in LocalUserManager.readOnlyLocalUsersList)
					{
						if (lu.cachedBody == base.characterBody)
						{
							this.localUser = lu;
							break;
						}
					}
				}
			}
		}

        public override void HandleMovements()
        {
            base.HandleMovements();

			if (this.landingTimer > 0f)
			{
				this.characterMotor.moveDirection = Vector3.zero;
				this.characterDirection.moveVector = this.characterDirection.forward;
			}
		}

		private void Landing(float speed)
        {
			if (speed <= -100f && !this.penis.inGrab)
            {
				this.HeaviestLanding();
			}

			if (speed <= -60f && !this.penis.inGrab) this.HeavyLanding();
        }

		private void HeavyLanding()
        {
			base.PlayAnimation("FullBody, Override Soft", "HeavyLanding", "Landing.playbackRate", 1.1f);
			this.landingTimer = 1f;
			this.characterMotor.velocity = Vector3.zero;
			Util.PlaySound("sfx_ravager_landing", this.gameObject);
			this._lastYSpeed = 0f;

			EffectManager.SpawnEffect(Modules.Assets.groundImpactEffect, new EffectData
			{
				origin = this.transform.position,
				rotation = Quaternion.identity,
				scale = 1f
			}, true);

			// damage
			BlastAttack.Result result = new BlastAttack
			{
				attacker = this.gameObject,
				procChainMask = default(ProcChainMask),
				impactEffect = EffectIndex.Invalid,
				losType = BlastAttack.LoSType.None,
				damageColorIndex = DamageColorIndex.Default,
				damageType = DamageType.Stun1s | DamageType.NonLethal,
				procCoefficient = 1f,
				bonusForce = Vector3.up * 800f,
				baseForce = 0f,
				baseDamage = 0f,
				falloffModel = BlastAttack.FalloffModel.None,
				radius = 4f,
				position = this.transform.position,
				attackerFiltering = AttackerFiltering.NeverHitSelf,
				teamIndex = this.GetTeam(),
				inflictor = this.gameObject,
				crit = false
			}.Fire();
		}

		private void HeaviestLanding()
		{
			base.PlayAnimation("FullBody, Override Soft", "HeavyLanding", "Landing.playbackRate", 2.2f);
			this.landingTimer = 2f;
			this.characterMotor.velocity = Vector3.zero;
			Util.PlaySound("sfx_ravager_landing", this.gameObject);
			Util.PlaySound("sfx_ravager_ground_impact", this.gameObject);
			this._lastYSpeed = 0f;

			EffectManager.SpawnEffect(Modules.Assets.heavyGroundImpactEffect, new EffectData
			{
				origin = this.transform.position,
				rotation = Quaternion.identity,
				scale = 1f
			}, true);

			// damage
			BlastAttack.Result result = new BlastAttack
			{
				attacker = this.gameObject,
				procChainMask = default(ProcChainMask),
				impactEffect = EffectIndex.Invalid,
				losType = BlastAttack.LoSType.None,
				damageColorIndex = DamageColorIndex.Default,
				damageType = DamageType.Stun1s | DamageType.NonLethal,
				procCoefficient = 1f,
				bonusForce = Vector3.up * 2000f,
				baseForce = 0f,
				baseDamage = 0f,
				falloffModel = BlastAttack.FalloffModel.None,
				radius = 8f,
				position = this.transform.position,
				attackerFiltering = AttackerFiltering.NeverHitSelf,
				teamIndex = this.GetTeam(),
				inflictor = this.gameObject,
				crit = false
			}.Fire();
		}

		public override void ProcessJump()
		{
			if (this.landingTimer > 0f && this.jumpInputReceived)
			{
				if (this.penis.passive.isWallJump)
                {
					EntityStateMachine.FindByCustomName(this.gameObject, "Passive").SetInterruptState(new ChargeJump(), InterruptPriority.Skill);
					this.jumpInputReceived = false;
					return;
                }
				this.jumpInputReceived = false;
				return;
			}

			if (this.hasCharacterMotor)
			{
				bool hopooFeather = false;
				bool waxQuail = false;

				if (this.jumpInputReceived && base.characterBody && base.characterMotor.jumpCount < base.characterBody.maxJumpCount)
				{
					int waxQuailCount = base.characterBody.inventory.GetItemCount(RoR2Content.Items.JumpBoost);
					float horizontalBonus = 1f;
					float verticalBonus = 1f;

					if (base.characterMotor.jumpCount >= base.characterBody.baseJumpCount)
					{
						this.penis.hopoFeatherTimer = 0.1f;
						hopooFeather = true;
						horizontalBonus = 1.5f;
						verticalBonus = 1.5f;
					}
					else if (waxQuailCount > 0 && base.characterBody.isSprinting)
					{
						float v = base.characterBody.acceleration * base.characterMotor.airControl;

						if (base.characterBody.moveSpeed > 0f && v > 0f)
						{
							waxQuail = true;
							float num2 = Mathf.Sqrt(10f * (float)waxQuailCount / v);
							float num3 = base.characterBody.moveSpeed / v;
							horizontalBonus = (num2 + num3) / num3;
						}
					}

					GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, horizontalBonus, verticalBonus, false);

					if (this.hasModelAnimator)
					{
						int layerIndex = base.modelAnimator.GetLayerIndex("Body");
						if (layerIndex >= 0)
						{
							if (this.characterBody.isSprinting)
							{
								this.modelAnimator.CrossFadeInFixedTime("SprintJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
							}
							else
							{
								if (hopooFeather)
								{
									this.modelAnimator.CrossFadeInFixedTime("BonusJump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
								}
								else
								{
									this.modelAnimator.CrossFadeInFixedTime("Jump", this.smoothingParameters.intoJumpTransitionTime, layerIndex);
								}
							}
						}
					}

					if (hopooFeather)
					{
						/*EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
						{
							origin = base.characterBody.footPosition
						}, true);*/
					}
					else if (base.characterMotor.jumpCount > 0)
					{
						EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CharacterLandImpact"), new EffectData
						{
							origin = base.characterBody.footPosition,
							scale = base.characterBody.radius
						}, true);
					}

					if (waxQuail)
					{
						EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BoostJumpEffect"), new EffectData
						{
							origin = base.characterBody.footPosition,
							rotation = Util.QuaternionSafeLookRotation(base.characterMotor.velocity)
						}, true);
					}

					base.characterMotor.jumpCount++;

					// set up double jump anim
					if (this.animator)
					{
						float x = this.animatorWalkParamCalculator.animatorWalkSpeed.y;
						float y = this.animatorWalkParamCalculator.animatorWalkSpeed.x;

						// neutral jump
						if (Mathf.Abs(x) <= 0.45f && Mathf.Abs(y) <= 0.45f || this.inputBank.moveVector == Vector3.zero)
						{
							x = 0f;
							y = 0f;
						}

						if (Mathf.Abs(x) > Mathf.Abs(y))
						{
							// side flip
							if (x > 0f) x = 1f;
							else x = -1f;
							y = 0f;
						}
						else if (Mathf.Abs(x) < Mathf.Abs(y))
						{
							// forward/backflips
							if (y > 0f) y = 1f;
							else y = -1f;
							x = 0f;
						}
						// eh this feels less dynamic. ignore the slight anim clipping issues ig and just blend them
						//  actualyl don't because the clipping issues are nightmarish

						// have to cache it at time of jump otherwise you can fuck up the jump anim in weird ways by turning during it
						this.animator.SetFloat("forwardSpeedCached", y);
						this.animator.SetFloat("rightSpeedCached", x);
						// turns out this wasn't even used in the end. the animation didn't break at all in practice, only in theory
						// Fuck You rob you fucking moron

						//  update: this was actually used. what the hell are you doing?
					}
				}
			}
		}
	}
}
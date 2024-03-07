using UnityEngine;
using RoR2;
using EntityStates;
using System.Collections.Generic;
using System.Linq;
using RedGuyMod.Content;

namespace RedGuyMod.SkillStates.Ravager
{
    public class WallJump : BaseRavagerState
    {
        private bool jumpAvailable;
		private float airTime;

		private bool isAltPassive;
		private bool isLegacyAltPassive;

        public override void OnEnter()
        {
            base.OnEnter();

			if (this.penis && this.penis.passive)
            {
				if (this.penis.passive.isBlink) this.isAltPassive = true;
				else this.isAltPassive = false;

				if (this.penis.passive.isLegacyBlink) this.isLegacyAltPassive = true;
				else this.isLegacyAltPassive = false;
			}
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

			if (this.isAltPassive)
            {
				if (this.isGrounded)
				{
					this.penis.blinkReady = true;
					this.airTime = 0f;
					this.penis.wallJumpCounter = 0;
				}
				else this.airTime += Time.fixedDeltaTime;

				if (this.inputBank.jump.justPressed && !this.isGrounded && base.isAuthority)
				{
					if (this.airTime >= 0.15f)
					{
						if (this.penis.blinkReady)
						{
							// hopoo feather interaction
							if (this.penis.hopoFeatherTimer > 0f)
							{
								EntityStateMachine.FindByCustomName(this.gameObject, "Body").SetInterruptState(new ChargeBlink
								{
									hopoo = true
								}, InterruptPriority.Any);

								return;
							}

							if (!this.penis.draining) this.penis.blinkReady = false;

							EntityStateMachine.FindByCustomName(this.gameObject, "Body").SetInterruptState(new ChargeBlink(), InterruptPriority.Any);

							return;
						}

						if (this.AttemptEnemyStep())
						{
							this.jumpAvailable = true;
							base.PlayAnimation("Body", "JumpEnemy");
							Util.PlaySound("sfx_ravager_enemystep", this.gameObject);
							GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1.5f, 1.5f, false);
							return;
						}
					}
				}

				return;
            }

			if (this.isLegacyAltPassive)
            {
				if (this.penis.draining) this.penis.blinkReady = true;

				if (this.isGrounded)
				{
					this.jumpAvailable = true;
					this.airTime = 0f;
					this.penis.wallJumpCounter = 0;
				}
				else this.airTime += Time.fixedDeltaTime;

				if (this.inputBank.jump.justPressed && !this.isGrounded && this.penis.hopoFeatherTimer <= 0f)
				{
					if (this.airTime >= 0.1f)
					{
						if (this.penis.blinkReady)
						{
							this.penis.blinkReady = false;
							this.BlinkForward();
							return;
						}
					}
				}
			}
			else
            {
				// heha
				this.jumpAvailable = true;

				if (this.isGrounded)
				{
					this.jumpAvailable = true;
					this.airTime = 0f;
					this.penis.wallJumpCounter = 0;
				}
				else this.airTime += Time.fixedDeltaTime;

				if (this.inputBank.jump.justPressed && !this.isGrounded && base.isAuthority)
				{
					if (this.airTime >= 0.15f)
					{
						if (this.jumpAvailable)
						{
							// hopoo feather interaction
							if (this.penis.hopoFeatherTimer > 0f)
							{
								EntityStateMachine.FindByCustomName(this.gameObject, "Body").SetInterruptState(new ChargeJump
								{
									hopoo = true
								}, InterruptPriority.Any);

								return;
							}

							if (this.AttemptWallJump())
							{
								if (this.penis && !this.penis.draining) this.jumpAvailable = false;

								EntityStateMachine.FindByCustomName(this.gameObject, "Body").SetInterruptState(new ChargeJump(), InterruptPriority.Any);

								return;
							}
						}

						if (this.AttemptEnemyStep())
						{
							this.jumpAvailable = true;
							base.PlayAnimation("Body", "JumpEnemy");
							Util.PlaySound("sfx_ravager_enemystep", this.gameObject);
							GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1.5f, 1.5f, false);
							return;
						}
					}
				}
			}
        }

        private bool AttemptEnemyStep()
        {
			BullseyeSearch2 bullseyeSearch = new BullseyeSearch2
			{
				teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam()),
				filterByLoS = false,
				searchOrigin = this.transform.position + (Vector3.up * 0.5f),
				searchDirection = UnityEngine.Random.onUnitSphere,
				sortMode = BullseyeSearch2.SortMode.Distance,
				onlyBullseyes = false,
				maxDistanceFilter = 5f,
				maxAngleFilter = 360f
			};

			bullseyeSearch.RefreshCandidates();
			bullseyeSearch.FilterOutGameObject(base.gameObject);
			List<HurtBox> list = bullseyeSearch.GetResults().ToList<HurtBox>();
			foreach (HurtBox hurtBox in list)
			{
				if (hurtBox)
				{
					if (hurtBox.healthComponent && hurtBox.healthComponent.body)
					{
						return true;
					}
				}
			}

			return false;
		}

		private bool AttemptWallJump()
        {
			if (Physics.CheckSphere(this.transform.position + (Vector3.up * 0.35f), 2.1f, LayerIndex.world.mask, QueryTriggerInteraction.Collide)) return true;

			return false;
        }

		private void BlinkForward()
		{
			this.jumpAvailable = false;
			EntityStateMachine.FindByCustomName(this.gameObject, "Slide").SetInterruptState(new Blink(), InterruptPriority.Any);
		}
	}
}
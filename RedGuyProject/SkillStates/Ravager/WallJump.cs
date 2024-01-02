using UnityEngine;
using RoR2;
using EntityStates;
using System.Collections.Generic;
using System.Linq;

namespace RedGuyMod.SkillStates.Ravager
{
    public class WallJump : BaseRavagerState
    {
        private bool jumpAvailable;
		private float airTime;

        public override void FixedUpdate()
        {
            base.FixedUpdate();

			// heha
			this.jumpAvailable = true;

			if (this.isGrounded)
			{
				this.jumpAvailable = true;
				this.airTime = 0f;
			}
			else this.airTime += Time.fixedDeltaTime;

            if (this.inputBank.jump.justPressed && !this.isGrounded)
            {
				if (this.airTime >= 0.15f)
				{
					if (this.jumpAvailable)
					{
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
						base.PlayAnimation("Body", "Jump");
						GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1.5f, 1.5f, false);
						return;
					}
				}
			}
        }

        private bool AttemptEnemyStep()
        {
			BullseyeSearch bullseyeSearch = new BullseyeSearch
			{
				teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam()),
				filterByLoS = false,
				searchOrigin = this.transform.position + (Vector3.up * 0.5f),
				searchDirection = UnityEngine.Random.onUnitSphere,
				sortMode = BullseyeSearch.SortMode.Distance,
				maxDistanceFilter = 7f,
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
			if (Physics.CheckSphere(this.transform.position + (Vector3.up * 0.85f), 1.9f, LayerIndex.world.mask, QueryTriggerInteraction.Collide)) return true;

			return false;
        }
    }
}
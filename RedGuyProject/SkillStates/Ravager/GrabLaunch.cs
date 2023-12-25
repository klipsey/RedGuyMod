using System.Collections.Generic;
using EntityStates;
using RoR2;
using UnityEngine;
using RedGuyMod.Content.Components;

namespace RedGuyMod.SkillStates.Ravager
{
	public class GrabLaunch : BaseState
	{
		public bool isThrowing;
		public float exitSpeed;
		public Vector3 lastSafeFootPosition;
		public List<GrabController> grabController = new List<GrabController>();
		private float duration = 0.5f;
		public Vector3 direction;

		public override void OnEnter()
		{
			base.OnEnter();
			RaycastHit raycastHit;

			if (!Physics.Raycast(new Ray(base.characterBody.footPosition, Vector3.down), out raycastHit, 100f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
				base.transform.position = this.lastSafeFootPosition + Vector3.up * 5;
			//Util.PlaySound("DragLaunch", base.gameObject);
			//Util.PlaySound("DragLaunchVoice", base.gameObject);
			
			if (this.isThrowing) base.PlayAnimation("FullBody, Override", "DashGrabEndThrow", "Grab.playbackRate", this.duration);
			else base.PlayAnimation("FullBody, Override", "DashGrabEnd", "Grab.playbackRate", this.duration);

			this.direction = base.characterMotor.moveDirection;
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();

			base.characterDirection.forward = this.direction;
			//base.characterMotor.moveDirection = this.direction * this.exitSpeed * Mathf.Lerp(1f, 0f, base.fixedAge / this.duration) / 4f;
			base.characterMotor.velocity = Vector3.zero; ////////delet
			base.characterMotor.moveDirection = Vector3.zero;
			if (base.fixedAge >= this.duration)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}
	}
}
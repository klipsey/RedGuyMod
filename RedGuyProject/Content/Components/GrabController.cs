using System;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RedGuyMod.Content.Components
{
	public class GrabController : MonoBehaviour
	{
		public CharacterBody attackerBody;

		public bool empowered;

		private GameObject extraCollider;
		private GameObject extraCollider2;
		private int extraLayer;
		private int extraLayer2;
		private bool modelLocatorStartedDisabled;
		public Transform pivotTransform;
		public CharacterBody body;
		public CharacterMotor motor;
		private CharacterDirection direction;
		private ModelLocator modelLocator;
		private Transform modelTransform;
		private Quaternion originalRotation;

		private GameObject anchor;
		private GrabTracker grabTracker;
		private Vector3 offset;

		private void Awake()
		{
			this.body = base.GetComponent<CharacterBody>();
			this.motor = base.GetComponent<CharacterMotor>();
			this.direction = base.GetComponent<CharacterDirection>();
			this.modelLocator = base.GetComponent<ModelLocator>();
			this.anchor = new GameObject();

			if (this.modelLocator)
			{
				// greater wisp
				/*Transform transform = base.transform.Find("Model Base/mdlGreaterWisp/GreaterWispArmature/HurtBox");
				if (transform)
				{
					this.extraLayer = transform.gameObject.layer;
					transform.gameObject.layer = LayerIndex.noCollision.intVal;
				}*/
				Transform transform = base.transform.Find("Model Base/mdlGreaterWisp/GreaterWispArmature/ROOT/Mask/StandableSurfacePosition/StandableSurface");
				if (transform)
				{
					this.extraLayer2 = transform.gameObject.layer;
					transform.gameObject.layer = LayerIndex.noCollision.intVal;
				}

				// archaic wisp
				transform = base.transform.Find("Model Base/mdlArchWisp/ArchWispArmature/ROOT/StandableSurfacePosition/StandableSurface");
				if (transform)
				{
					this.extraLayer2 = transform.gameObject.layer;
					transform.gameObject.layer = LayerIndex.noCollision.intVal;
				}

				// lunar wisp
				if (this.gameObject.name == "LunarWispBody(Clone)")
                {
					transform = this.GetComponent<ModelLocator>().modelTransform.Find("StandableSurface/StandableSurface");
					if (transform)
					{
						this.extraLayer2 = transform.gameObject.layer;
						transform.gameObject.layer = LayerIndex.noCollision.intVal;
					}
				}

				// vagrant
				transform = base.transform.Find("Model Base/mdlVagrant/VagrantArmature/DetatchedHull/Hull.003/StandableSurfacePosition/StandableSurface");
				if (transform)
				{
					this.extraLayer2 = transform.gameObject.layer;
					transform.gameObject.layer = LayerIndex.noCollision.intVal;
				}

				// scu
				transform = base.transform.Find("Model Base/mdlRoboBallBoss/RoboBallBossArmature/ROOT/StandableSurfacePosition/StandableSurface");
				if (transform)
				{
					this.extraLayer2 = transform.gameObject.layer;
					transform.gameObject.layer = LayerIndex.noCollision.intVal;
				}

				// xi
				if (this.gameObject.name == "MegaConstructBody(Clone)")
				{
					transform = this.GetComponent<ModelLocator>().modelTransform.Find("MegaConstructArmature/ROOT/base/body.1/StandableSurfacePosition/StandableSurface");
					if (transform)
					{
						this.extraLayer2 = transform.gameObject.layer;
						transform.gameObject.layer = LayerIndex.noCollision.intVal;
					}
				}
			}
			base.gameObject.layer = LayerIndex.noCollision.intVal;
			if (this.direction)
			{
				this.direction.enabled = false;
			}
			if (this.modelLocator)
			{
				if (!this.modelLocator.enabled)
				{
					this.modelLocatorStartedDisabled = true;
				}
				if (this.modelLocator.modelTransform)
				{
					this.modelTransform = this.modelLocator.modelTransform;
					this.originalRotation = this.modelTransform.rotation;
					this.modelLocator.enabled = false;
				}
			}

			if (Modules.Assets.bodyGrabOffsets.ContainsKey(this.gameObject.name))
            {
				this.offset = Modules.Assets.bodyGrabOffsets[this.gameObject.name];
            }
		}

		private void Start()
        {
			this.anchor.transform.parent = this.pivotTransform;
			this.anchor.transform.localPosition = this.offset;

			if (this.attackerBody)
            {
				this.grabTracker = this.gameObject.AddComponent<GrabTracker>();
				this.grabTracker.attackerBody = this.attackerBody;
            }

			if (NetworkServer.active && this.empowered) this.body.AddBuff(Content.Survivors.RedGuy.grabbedBuff);
		}

		private void Update()
        {
			if (this.motor)
			{
				this.motor.disableAirControlUntilCollision = true;
				this.motor.velocity = Vector3.zero;
				this.motor.rootMotion = Vector3.zero;
				this.motor.Motor.SetPosition(this.anchor.transform.position, true);
			}
			if (this.anchor)
			{
				base.transform.position = this.anchor.transform.position;
			}
			if (this.modelTransform)
			{
				this.modelTransform.position = this.anchor.transform.position;
				this.modelTransform.rotation = this.anchor.transform.rotation;
			}
		}

		private void FixedUpdate()
		{
			if (this.grabTracker)
            {
				this.grabTracker.Refresh();
            }	
			
			if (this.motor)
			{
				this.motor.disableAirControlUntilCollision = true;
				this.motor.velocity = Vector3.zero;
				this.motor.rootMotion = Vector3.zero;
				this.motor.Motor.SetPosition(this.anchor.transform.position, true);
			}
			if (this.anchor)
			{
				base.transform.position = this.anchor.transform.position;
			}
			if (this.modelTransform)
			{
				this.modelTransform.position = this.anchor.transform.position;
				this.modelTransform.rotation = this.anchor.transform.rotation;
			}
			RaycastHit raycastHit;
			if (Physics.Raycast(new Ray(base.transform.position + Vector3.up * 2f, Vector3.down), out raycastHit, 6f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
			{
				this.lastGroundPosition = raycastHit.point;
			}
		}

		private Vector3 lastGroundPosition;
		public void Launch(Vector3 launchVector)
		{
			if (this.modelLocator && !this.modelLocatorStartedDisabled)
			{
				this.modelLocator.enabled = true;
			}
			if (this.modelTransform)
			{
				this.modelTransform.rotation = this.originalRotation;
			}
			if (this.direction)
			{
				this.direction.enabled = true;
			}
			if (this.body.healthComponent && this.body.healthComponent.alive)
			{
				if (this.modelLocator)
				{
					/*Transform transform = base.transform.Find("Model Base/mdlGreaterWisp/GreaterWispArmature/HurtBox");
					if (transform)
					{
						transform.gameObject.layer = this.extraLayer;
					}*/
					Transform transform = base.transform.Find("Model Base/mdlGreaterWisp/GreaterWispArmature/ROOT/Mask/StandableSurfacePosition/StandableSurface");
					if (transform)
					{
						transform.gameObject.layer = this.extraLayer2;
					}

					// archaic wisp
					transform = base.transform.Find("Model Base/mdlArchWisp/ArchWispArmature/ROOT/StandableSurfacePosition/StandableSurface");
					if (transform)
					{
						transform.gameObject.layer = this.extraLayer2;
					}

					// lunar wisp
					if (this.gameObject.name == "LunarWispBody(Clone)")
					{
						transform = this.GetComponent<ModelLocator>().modelTransform.Find("StandableSurface/StandableSurface");
						if (transform)
						{
							transform.gameObject.layer = this.extraLayer2;
						}
					}

					// vagrant
					if (this.gameObject.name == "Vagrant(Clone)")
					{
						transform = base.transform.Find("Model Base/mdlVagrant/VagrantArmature/DetatchedHull/Hull.003/StandableSurfacePosition/StandableSurface");
						if (transform)
						{
							transform.gameObject.layer = this.extraLayer2;
						}
					}

					// scu / awu
					if (this.gameObject.name == "RoboBallBossBody(Clone)" || this.gameObject.name == "SuperRoboBallBossBody(Clone)")
					{
						transform = base.transform.Find("Model Base/mdlRoboBallBoss/RoboBallBossArmature/ROOT/StandableSurfacePosition/StandableSurface");
						if (transform)
						{
							transform.gameObject.layer = this.extraLayer2;
						}
					}

					// xi
					if (this.gameObject.name == "MegaConstructBody(Clone)")
					{
						transform = this.GetComponent<ModelLocator>().modelTransform.Find("MegaConstructArmature/ROOT/base/body.1/StandableSurfacePosition/StandableSurface");
						if (transform)
						{
							transform.gameObject.layer = this.extraLayer2;
						}
					}
				}
				base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			}
			RaycastHit raycastHit;
			if (!Physics.Raycast(new Ray(this.body.footPosition, Vector3.down), out raycastHit, 15f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
			{
				base.transform.position = this.lastGroundPosition;
			}
			if (this.motor)
			{
				float force = 0.25f;
				float f = Mathf.Max(140f, motor.mass);
				force = f / 140f;
				launchVector *= force;
				this.motor.ApplyForce(launchVector, false, false);
			}
			else
			{
				float force = 0.25f;
				if (body.rigidbody)
				{
					float f = Mathf.Max(200f, body.rigidbody.mass);
					force = f / 200f;
				}
				launchVector *= force;
				DamageInfo damageInfo = new DamageInfo
				{
					position = this.body.transform.position,
					attacker = null,
					inflictor = null,
					damage = 0f,
					damageColorIndex = DamageColorIndex.Default,
					damageType = DamageType.Generic,
					crit = false,
					force = launchVector,
					procChainMask = default(ProcChainMask),
					procCoefficient = 0f
				};
				this.body.healthComponent.TakeDamageForce(damageInfo, false, false);
			}
			GameObject.Destroy(this);
		}

		public void Release()
		{
			if (this.modelLocator && !this.modelLocatorStartedDisabled)
			{
				this.modelLocator.enabled = true;
			}
			if (this.modelTransform)
			{
				this.modelTransform.rotation = this.originalRotation;
			}
			if (this.direction)
			{
				this.direction.enabled = true;
			}
			if (this.extraCollider)
			{
				this.extraCollider.layer = this.extraLayer;
			}
			if (this.extraCollider2)
			{
				this.extraCollider2.layer = this.extraLayer2;
			}
			base.gameObject.layer = LayerIndex.defaultLayer.intVal;
			GameObject.Destroy(this);
		}

		private void OnDestroy()
        {
			Destroy(this.anchor);
			this.KillBuff();
		}

		public void KillBuff()
        {
			if (NetworkServer.active && this.body && this.empowered) this.body.RemoveBuff(Content.Survivors.RedGuy.grabbedBuff);
		}
	}
}
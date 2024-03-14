using System;
using System.Collections.Generic;
using System.Linq;
using EntityStates;
using EntityStates.Commando;
using RedGuyMod.Content;
using RedGuyMod.Content.Components;
using RoR2;
using RoR2.Audio;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static RoR2.CameraTargetParams;

namespace RedGuyMod.SkillStates.Ravager
{
	public class DashGrab : BaseRavagerSkillState
	{
		private float finalAirTime;
		private Vector3 lastSafeFootPosition;
		private float airTimeDamageCoefficient = 1.5f;
		private GameObject fireEffect;
		private GameObject dragEffect;
		private float minDropTime = 0.35f;
		private float attackRecoil = 7f;
		protected AnimationCurve dashSpeedCurve;
		protected AnimationCurve dragSpeedCurve;
		protected AnimationCurve dropSpeedCurve;
		private float grabDuration = 0.5f;
		private Vector3 targetMoveVector;
		private Vector3 targetMoveVectorVelocity;
		private bool wasGrounded;
		public static float upForce = 800f;
		public static float launchForce = 1200f;
		public static float throwForce = 12000f;
		public static float turnSmoothTime = 0.01f;
		public static float turnSpeed = 20f;
		public static float dragMaxSpeedCoefficient = 5f;
		private bool canGrabBoss;

		private float dragDamageCoefficient = 1f;
		private float dragDamageInterval = 0.1f;
		private float dragDamageStopwatch;
		private float dragStopwatch;
		private float baseDragDuration = 2.5f;
		private float dragDuration;
		private float dragMaxSpeedTime = 0.8f;
		private float maxAirTime = 0.67f;
		private float smallHopVelocity = 15f;
		private float windupDuration = 0.2f;
		private float exitDuration = 0.5f;

		protected GameObject swingEffectPrefab;
		protected GameObject hitEffectPrefab;
		protected NetworkSoundEventIndex impactSound;
		public static float groundSlamDamageCoefficient = 10f;
		public static float punchDamageCoefficient = 12.5f;
		private float chargeDamageCoefficient = 8f;
		private float chargeImpactForce = 2000f;
		private Vector3 bonusForce = Vector3.up * 2000f;
		private Vector3 aimDirection;
		private List<GrabController> grabController;
		private float stopwatch;
		private Animator animator;
		private bool hasGrabbed;
		private OverlapAttack attack;
		private float grabRadius = 8f;
		private float groundSlamRadius = 4f;
		private DashGrab.SubState subState;
		public static float dodgeFOV = DodgeState.dodgeFOV;
		private uint soundID;
		private bool c1;
		private bool c2;
		private bool releaseEnemies;
		private CameraParamsOverrideHandle camParamsOverrideHandle;
		//private CameraParamsOverrideHandle camParamsOverrideHandle2;
		private bool s1;
		private int slamCount;

		public static event Action<int> onSlamCountIncremented;

		protected virtual bool forcePunch
        {
			get
            {
				return false;
            }
        }

		protected virtual bool forceThrow
		{
			get
			{
				return false;
			}
		}

		protected virtual string startAnimString
        {
			get
            {
				return "DashGrabStart";
            }
        }

		protected virtual string dashAnimString
		{
			get
			{
				return "DashGrab";
			}
		}

		private enum SubState
		{
			Windup,
			DashGrab,
			MissedGrab,
			AirGrabbed,
			Dragging,
			Throwing
		}

		public override void OnEnter()
		{
			base.OnEnter();
			this.animator = base.GetModelAnimator();
			this.aimDirection = base.GetAimRay().direction;
			this.aimDirection.y = Mathf.Clamp(this.aimDirection.y, -0.75f, 0.75f);
			this.stopwatch = 0f;
			this.grabController = new List<GrabController>();
			this.dragDuration = this.baseDragDuration;
			this.canGrabBoss = Modules.Config.bossGrab.Value;
			this.penis.skibidi = true;

			//this.fireEffect = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.grabFireEffect, base.FindModelChild("HandL2"));
			base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
			base.PlayAnimation("FullBody, Override", this.startAnimString, "Grab.playbackRate", this.windupDuration);

			if (this.forcePunch && this.empowered) Util.PlaySound("sfx_ravager_ready_punch", this.gameObject);
			else Util.PlaySound("sfx_ravager_shine", this.gameObject);

			//if (this.characterBody && NetworkServer.active) base.characterBody.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;

			Transform modelTransform = base.GetModelTransform();
			HitBoxGroup hitBoxGroup = null;
			if (modelTransform)
			{
				hitBoxGroup = Array.Find<HitBoxGroup>(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == "Drag");
			}

			this.characterMotor.velocity *= 0.1f;

			this.attack = new OverlapAttack();
			this.attack.damageType = DamageType.Generic;
			this.attack.attacker = base.gameObject;
			this.attack.inflictor = base.gameObject;
			this.attack.teamIndex = base.GetTeam();
			this.attack.damage = this.chargeDamageCoefficient * this.damageStat;
			this.attack.procCoefficient = 1f;
			this.attack.hitEffectPrefab = this.hitEffectPrefab;
			this.attack.forceVector = this.bonusForce;
			this.attack.pushAwayForce = this.chargeImpactForce;
			this.attack.hitBoxGroup = hitBoxGroup;
			this.attack.isCrit = base.RollCrit();
			//this.attack.impactSound = Modules.Assets.jab2HitSoundEvent.index;
			this.dashSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 14f),
				new Keyframe(0.8f, 0f),
				new Keyframe(1f, 0f)
			});
			this.dragSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.35f, 1f),
				new Keyframe(0.9f, 5f),
				new Keyframe(1f, 5f)
			});
			this.dropSpeedCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 0f),
				new Keyframe(0.9f, 25f),
				new Keyframe(1f, 25f)
			});
			this.subState = DashGrab.SubState.Windup;

			this.penis.inGrab = true;

			this.GetModelAnimator().SetFloat("leapDir", this.inputBank.aimDirection.y);
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.stopwatch += Time.fixedDeltaTime;
			if (this.subState == DashGrab.SubState.Windup)
			{
				this.characterBody.isSprinting = false;

				if (this.stopwatch >= this.windupDuration)
				{
					this.stopwatch = 0f;
					this.subState = DashGrab.SubState.DashGrab;
					base.PlayAnimation("FullBody, Override", this.dashAnimString, "Grab.playbackRate", this.grabDuration * 1.25f);
					Util.PlaySound("sfx_ravager_lunge", this.gameObject);
				}

				this.characterMotor.velocity.y = 0f;
			}
			else
			{
				if (this.subState == DashGrab.SubState.DashGrab)
				{
					this.characterBody.isSprinting = true;

					float num = this.dashSpeedCurve.Evaluate(this.stopwatch / this.grabDuration);
					base.characterMotor.rootMotion += this.aimDirection * (num * this.moveSpeedStat * Time.fixedDeltaTime);
					base.characterMotor.velocity.y = 0f;

					if (!this.hasGrabbed)
					{
						this.AttemptGrab(this.grabRadius);
					}

					if (this.hasGrabbed)
					{
						this.stopwatch = 0f;

						if (this.forceThrow)
                        {
							this.subState = DashGrab.SubState.Throwing;

							this.c1 = true;
							this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, RavagerCameraParams.SLAM, 2f);
						}
						else
                        {
							this.subState = DashGrab.SubState.AirGrabbed;

							this.c1 = true;
							this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, RavagerCameraParams.SLAM, 2f);
						}
					}
					else
					{
						if (this.stopwatch >= this.grabDuration)
						{
							if (this.fireEffect) EntityState.Destroy(this.fireEffect);
							this.stopwatch = 0f;
							this.outer.SetNextStateToMain();
							this.subState = DashGrab.SubState.MissedGrab;
						}
					}
				}
				else
				{
					if (this.subState == DashGrab.SubState.AirGrabbed)
					{
						this.characterBody.isSprinting = true;
						/*if (base.inputBank.jump.justPressed && this.stopwatch >= this.minDropTime * 2f)
						{
							if (base.isAuthority)
							{
								this.outer.SetNextState(new GrabLaunch
								{
									grabController = this.grabController,
									exitSpeed = 0f,
									lastSafeFootPosition = this.transform.position
								});
							}
							return;
						}*/

						if (base.isGrounded && this.stopwatch >= this.minDropTime)
						{
							this.targetMoveVector = Vector3.zero;

							if (this.empowered)
                            {
								this.dragEffect = UnityEngine.Object.Instantiate<GameObject>(Modules.Assets.groundDragEffect, base.FindModelChild("HandL").position, Util.QuaternionSafeLookRotation(Vector3.up));
								this.dragEffect.transform.parent = base.FindModelChild("HandL");
							}

							this.finalAirTime = (this.stopwatch / this.maxAirTime);
							if (base.isAuthority)
							{
								//Util.PlaySound("sfx_ravager_explosion", base.gameObject);
								float c = (finalAirTime + 1) * this.airTimeDamageCoefficient;
								float attackRecoil = this.attackRecoil;
								base.AddRecoil(-1f * attackRecoil, -2f * attackRecoil, -0.5f * attackRecoil, 0.5f * attackRecoil);

								//Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossGroundSlam.prefab").WaitForCompletion()
								EffectManager.SpawnEffect(Modules.Assets.slamImpactEffect, new EffectData
								{
									origin = base.transform.position,
									scale = this.groundSlamRadius * c,
									networkSoundEventIndex = Modules.Assets.explosionSoundEvent.index
								}, true);

								this.IterateSlamCount();

								//Util.PlaySound("sfx_ravager_ground_impact", this.gameObject);

								Vector3 attackPosition = this.FindModelChild("HandL").position;

								if (this.grabController.Count > 0)
                                {
									foreach (GrabController i in this.grabController)
                                    {
										if (i && i.body && i.body.mainHurtBox) attackPosition = i.body.mainHurtBox.transform.position;
                                    }
                                }

								float dmg = c * DashGrab.groundSlamDamageCoefficient * this.damageStat;
								if (this.s1) c *= 0.5f;
								this.s1 = true;

								BlastAttack.Result result = new BlastAttack
								{
									attacker = base.gameObject,
									procChainMask = default(ProcChainMask),
									impactEffect = EffectIndex.Invalid,
									losType = BlastAttack.LoSType.None,
									damageColorIndex = DamageColorIndex.Default,
									damageType = DamageType.Stun1s,
									procCoefficient = 1f,
									bonusForce = DashGrab.upForce * Vector3.up,
									baseForce = DashGrab.launchForce,
									baseDamage = dmg,
									falloffModel = BlastAttack.FalloffModel.SweetSpot,
									radius = this.groundSlamRadius,
									position = attackPosition,
									attackerFiltering = AttackerFiltering.NeverHitSelf,
									teamIndex = base.GetTeam(),
									inflictor = base.gameObject,
									crit = base.RollCrit()
								}.Fire();
							}

							if (base.isAuthority && !this.empowered)
							{
								this.outer.SetNextState(new GrabLaunch
								{
									grabController = this.grabController,
									exitSpeed = 0f,
									lastSafeFootPosition = this.lastSafeFootPosition
								});
								return;
							}

							base.modelLocator.normalizeToFloor = true;
							this.subState = DashGrab.SubState.Dragging;
							this.stopwatch = 0f;

							this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
							//this.camParamsOverrideHandle2 = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, RavagerCameraParams.DRAG, 1f);
							//this.c2 = true;

							//base.PlayAnimation("FullBody, Override", "SSpecGrab", "Slash.playbackRate", this.grabDuration);

							this.soundID = Util.PlaySound("sfx_ravager_dragloop", base.gameObject);
							this.animator.SetBool("dragGround", true);
						}
						else
						{
							float d = this.dropSpeedCurve.Evaluate(this.stopwatch / this.maxAirTime);
							base.characterMotor.rootMotion += 1.5f * Vector3.down * d * Time.fixedDeltaTime + 0.5f * base.inputBank.moveVector * d * Time.fixedDeltaTime;
						}
					}
					else
					{
						if (this.subState == DashGrab.SubState.Dragging)
						{
							this.characterBody.isSprinting = true;

							if (base.characterMotor.lastGroundedTime.timeSince >= 0.15f)
							{
								if (this.dragEffect)
								{
									EntityState.Destroy(this.dragEffect);
								}
								this.subState = SubState.AirGrabbed;
								this.stopwatch = 0f;
								this.dragDamageStopwatch = 0f;
								this.animator.SetBool("dragGround", false);
								base.PlayAnimation("FullBody, Override", "DashGrabHit", "Grab.playbackRate", this.grabDuration);
								AkSoundEngine.StopPlayingID(this.soundID);
								return;
							}

							RaycastHit raycastHit = default(RaycastHit);
							Vector3 position = base.FindModelChild("HandL").position;
							position.y += 1f;

							if (this.dragEffect)
							{
								if (Physics.Raycast(new Ray(position, Vector3.down), out raycastHit, 4f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
									this.dragEffect.transform.position = raycastHit.point;
								else
									this.dragEffect.transform.position = raycastHit.point = base.FindModelChild("HandL").position;
							}

							this.dragStopwatch += Time.fixedDeltaTime;

							this.dragDamageStopwatch += Time.fixedDeltaTime;
							if (this.dragDamageStopwatch >= this.dragDamageInterval)
							{
								this.DamageTargets();
								this.dragDamageStopwatch = 0f;
							}
							float d2 = this.dragSpeedCurve.Evaluate(this.stopwatch / this.dragMaxSpeedTime);
							this.targetMoveVector = Vector3.ProjectOnPlane(Vector3.SmoothDamp(this.targetMoveVector, base.inputBank.aimDirection, ref this.targetMoveVectorVelocity, DashGrab.turnSmoothTime, DashGrab.turnSpeed), Vector3.up).normalized;
							base.characterDirection.moveVector = this.targetMoveVector;
							Vector3 forward = base.characterDirection.forward;
							base.characterMotor.moveDirection = forward * d2;
							List<HurtBox> list = new List<HurtBox>();

							this.animator.SetFloat("dragSpeed", d2);


							foreach (GrabController controller in this.grabController)
							{
								if (controller.body)
								{
									HealthComponent healthComponent = controller.body.healthComponent;
									if (healthComponent)
									{
										this.attack.ignoredHealthComponentList.Add(healthComponent);
									}
								}

							}

							if (base.isAuthority)
							{
								if (this.attack.Fire(list))
								{

								}
							}

							if (this.isGrounded)
                            {
								if (!this.wasGrounded)
                                {
									base.PlayAnimation("FullBody, Override", "DashGrabRun");
                                }
                            }

							this.wasGrounded = this.isGrounded;

							if (this.dragStopwatch >= this.dragDuration * 0.9f) this.KillBuffs();

							if (this.dragStopwatch >= this.dragDuration || (base.inputBank.jump.justPressed))
							{
								/*
								foreach (GrabController grabController in this.grabController)
								{
									if (grabController)
									{
										grabController.Launch(base.characterMotor.moveDirection.normalized * DashGrab.launchForce + Vector3.up * DashGrab.upForce);
										base.modelLocator.normalizeToFloor = true;
									}
								}
								*/
								if (base.isAuthority)
								{
									this.outer.SetNextState(new GrabLaunch
									{
										grabController = this.grabController,
										exitSpeed = d2,
										lastSafeFootPosition = this.lastSafeFootPosition,
										isThrowing = this.grabController.Count > 0
									});
								}
								return;

							}
						}
						else if (this.subState == SubState.Throwing)
                        {
							if (!this.c1)
                            {
								this.c1 = true;
								base.PlayAnimation("FullBody, Override", "GrabThrow", "Grab.playbackRate", 1.5f);
                            }

							if (this.stopwatch >= 1.2f * 0.5f)
                            {
								base.SmallHop(base.characterMotor, this.smallHopVelocity);

								this.TryReleaseGrab();
							}

							if (this.stopwatch >= 1.2f)
                            {
								this.outer.SetNextStateToMain();
								return;
                            }
                        }
						else
						{
							this.releaseEnemies = true;
							base.characterMotor.velocity = Vector3.zero; ////////delet
							base.characterMotor.moveDirection = Vector3.zero;

							if (this.stopwatch >= this.exitDuration)
							{
								this.outer.SetNextStateToMain();
							}

						}
					}
				}
			}
		}

		private void KillBuffs()
        {
			foreach (GrabController grabController in this.grabController)
			{
				if (grabController)
				{
					grabController.KillBuff();
				}
			}
		}

		private void DamageTargets()
		{
			foreach (GrabController grabController in this.grabController)
			{
				if (grabController)
				{
					DamageInfo damageInfo = new DamageInfo
					{
						position = grabController.body.gameObject.transform.position,
						attacker = base.gameObject,
						inflictor = base.gameObject,
						damage = this.dragDamageCoefficient * this.damageStat,
						damageColorIndex = DamageColorIndex.Default,
						damageType = DamageType.Stun1s,
						crit = base.RollCrit(),
						force = Vector3.zero,
						procChainMask = default(ProcChainMask),
						procCoefficient = 0.1f
					};

					if (grabController.body && grabController.body.healthComponent && NetworkServer.active)// Util.HasEffectiveAuthority(grabController.body.gameObject))
					{
						grabController.body.healthComponent.TakeDamage(damageInfo);
						this.ForceFlinch(grabController.body);
					}
				}
			}
		}

		public override void OnExit()
		{
			base.OnExit();
			this.penis.inGrab = false;

			if (this.c1) this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
			//if (this.c2) this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle2);
			if (this.dragEffect) EntityState.Destroy(this.dragEffect);
			if (this.fireEffect) EntityState.Destroy(this.fireEffect);

			AkSoundEngine.StopPlayingID(this.soundID);
			RaycastHit raycastHit;

			if (!Physics.Raycast(new Ray(base.characterBody.footPosition, Vector3.down), out raycastHit, 100f, LayerIndex.world.mask, QueryTriggerInteraction.Collide))
				base.transform.position = this.lastSafeFootPosition + Vector3.up * 5;
			AkSoundEngine.StopPlayingID(this.soundID);
			base.modelLocator.normalizeToFloor = false;
			this.animator.SetBool("dragGround", false);

			this.TryReleaseGrab();

			if (NetworkServer.active)
			{
				//base.characterBody.bodyFlags &= ~CharacterBody.BodyFlags.IgnoreFallDamage;
			}
		}

		private void TryReleaseGrab()
        {
			if (this.grabController.Count > 0)
			{
				if (this.forceThrow)
                {
					foreach (GrabController grabController in this.grabController)
					{
						if (grabController)
						{
							grabController.Launch(base.characterMotor.moveDirection.normalized * DashGrab.launchForce + Vector3.up * DashGrab.upForce);
						}
					}
				}
				else
                {
					if (this.releaseEnemies)
					{
						foreach (GrabController grabController in this.grabController)
						{
							if (grabController)
							{
								grabController.Release();
							}
						}
					}
					else
					{
						foreach (GrabController grabController in this.grabController)
						{
							if (grabController)
							{
								grabController.Launch(this.GetAimRay().direction * DashGrab.throwForce);
							}
						}

						this.DamageTargets();
					}
				}
			}
		}

		protected virtual void ForceFlinch(CharacterBody body)
		{
			if (Util.HasEffectiveAuthority(body.gameObject))
			{
				SetStateOnHurt component = body.healthComponent.GetComponent<SetStateOnHurt>();
				if (component)
				{
					if (component.canBeHitStunned)
					{
						component.SetPain();
					}
					else if (component.canBeStunned)
					{
						component.SetStun(1f);
					}
					foreach (EntityStateMachine e in body.gameObject.GetComponents<EntityStateMachine>())
					{
						if (e && e.customName.Equals("Weapon"))
						{
							e.SetNextStateToMain();
						}
					}
				}
			}
		}

		public void AttemptGrab(float grabRadius)
		{
			Ray aimRay = base.GetAimRay();

			BullseyeSearch2 bullseyeSearch = new BullseyeSearch2
			{
				teamMaskFilter = TeamMask.GetEnemyTeams(base.GetTeam()),
				filterByLoS = false,
				searchOrigin = base.transform.position,
				searchDirection = UnityEngine.Random.onUnitSphere,
				sortMode = BullseyeSearch2.SortMode.Distance,
				onlyBullseyes = false,
				maxDistanceFilter = grabRadius,
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
						bool canGrab = !this.forcePunch;

						if (this.canGrabBoss)
                        {
							if (hurtBox.healthComponent.body.isChampion && !this.empowered) canGrab = false;
							if (hurtBox.healthComponent.body.isChampion && this.empowered) this.dragDuration *= 0.5f;
						}
						else
                        {
							if (hurtBox.healthComponent.body.isChampion) canGrab = false;
						}

						if (canGrab)
						{
							Vector3 between = hurtBox.healthComponent.transform.position - base.transform.position;
							Vector3 v = between / 4f;
							v.y = Math.Max(v.y, between.y);
							base.characterMotor.AddDisplacement(v);
							
							GrabController grabController = hurtBox.healthComponent.body.gameObject.AddComponent<GrabController>();
							grabController.pivotTransform = base.FindModelChild("HandL");
							grabController.attackerBody = this.characterBody;
							grabController.empowered = this.empowered;

							if (this.fireEffect) EntityState.Destroy(this.fireEffect);
							this.grabController.Add(grabController);
							this.ForceFlinch(hurtBox.healthComponent.body);
							base.PlayAnimation("FullBody, Override", "DashGrabHit", "Grab.playbackRate", this.grabDuration);

							// swap team of vagrant balls
							if (hurtBox.healthComponent.gameObject.name.Contains("VagrantTrackingBomb"))
                            {
								TeamComponent team = hurtBox.healthComponent.body.teamComponent;
								TeamFilter team2 = hurtBox.healthComponent.gameObject.GetComponent<TeamFilter>();
								TeamIndex playerTeam = this.GetTeam();
								team.teamIndex = playerTeam;
								team2.teamIndex = playerTeam;

								hurtBox.healthComponent.gameObject.GetComponent<ProjectileController>().owner = this.gameObject;
								hurtBox.healthComponent.gameObject.GetComponent<ProjectileDamage>().damage *= 5f;
								hurtBox.healthComponent.gameObject.GetComponent<ProjectileImpactExplosion>().blastRadius *= 3f;
                            }

							if (!this.hasGrabbed)
                            {
								Util.PlaySound("sfx_ravager_grab", base.gameObject);
								base.SmallHop(base.characterMotor, this.smallHopVelocity);
								this.penis.RefreshBlink();
							}
							this.hasGrabbed = true;
						}
						else
                        {
							if (!this.forcePunch && hurtBox.healthComponent.body.isChampion)
                            {
								base.PlayCrossfade("FullBody, Override", "Cling", 0.1f);
								Util.PlaySound("sfx_ravager_grab", base.gameObject);

								HurtBox targetHurtbox = hurtBox;
								float dist = Mathf.Infinity;
								foreach (HurtBox i in hurtBox.healthComponent.gameObject.GetComponentsInChildren<HurtBox>())
                                {
									float d = Vector3.Distance(this.transform.position, i.transform.position);
									if (d < dist)
                                    {
										dist = d;
										targetHurtbox = i;
                                    }
                                }

								if (base.isAuthority)
                                {
									Vector3 offset = (this.transform.position - targetHurtbox.transform.position).normalized * this.penis.offsetDistance;

									GameObject anchor = new GameObject();

									anchor.transform.parent = targetHurtbox.transform;
									anchor.transform.position = targetHurtbox.transform.position + offset;

									EntityStateMachine.FindByCustomName(this.gameObject, "Body").SetNextState(new Cling
									{
										targetHurtbox = targetHurtbox,
										offset = offset,
										anchor = anchor
									});

									this.outer.SetNextStateToMain();
                                }

								return;
							}

							if (hurtBox.healthComponent.body.isChampion || this.forcePunch)
                            {
                                // PUNCH
                                /*if (NetworkServer.active)
								{
									DamageInfo info = new DamageInfo
									{
										attacker = this.gameObject,
										crit = this.RollCrit(),
										damage = DashGrab.groundSlamDamageCoefficient * this.damageStat,
										damageColorIndex = DamageColorIndex.Default,
										damageType = DamageType.Stun1s,
										force = aimRay.direction * 24000f,
										inflictor = this.gameObject,
										position = hurtBox.transform.position,
										procChainMask = default(ProcChainMask),
										procCoefficient = 1f,
									};

									hurtBox.healthComponent.TakeDamage(info);

									if (this.empowered)
                                    {

                                    }
								}*/
                                this.penis.RefreshBlink();
                                EffectManager.SpawnEffect(Modules.Assets.bloodBombEffect, new EffectData
								{
									origin = hurtBox.transform.position,
									scale = 2f
								}, false);

								if (this.empowered) Util.PlaySound("sfx_ravager_punch_empowered", this.gameObject);
								else Util.PlaySound("sfx_ravager_punch", this.gameObject);

								if (Modules.Assets.bodyPunchSounds.ContainsKey(hurtBox.healthComponent.gameObject.name))
								{
									Util.PlaySound(Modules.Assets.bodyPunchSounds[hurtBox.healthComponent.gameObject.name], hurtBox.gameObject);
								}
								else Util.PlaySound("sfx_ravager_punch_generic", hurtBox.gameObject);

								hurtBox.healthComponent.gameObject.AddComponent<ConsumeTracker>().attackerBody = this.characterBody;

								if (base.isAuthority)
								{
									float dmg = DashGrab.punchDamageCoefficient * this.damageStat;
									//if (this.empowered) dmg *= 2f;

									float force = 4000f;
									if (hurtBox.healthComponent.body.isChampion) force = 24000f;

									// damage
									BlastAttack.Result result = new BlastAttack
									{
										attacker = base.gameObject,
										procChainMask = default(ProcChainMask),
										impactEffect = EffectIndex.Invalid,
										losType = BlastAttack.LoSType.None,
										damageColorIndex = DamageColorIndex.Default,
										damageType = DamageType.Stun1s | DamageType.NonLethal,
										procCoefficient = 1f,
										bonusForce = this.GetAimRay().direction.normalized * force,
										baseForce = 0f,
										baseDamage = dmg,
										falloffModel = BlastAttack.FalloffModel.None,
										radius = 0.4f,
										position = hurtBox.transform.position,
										attackerFiltering = AttackerFiltering.NeverHitSelf,
										teamIndex = base.GetTeam(),
										inflictor = base.gameObject,
										crit = base.RollCrit()
									}.Fire();

									// shockwave
									FireProjectileInfo fireProjectileInfo = default(FireProjectileInfo);
									fireProjectileInfo.position = hurtBox.transform.position + (aimRay.direction * -4f);
									fireProjectileInfo.rotation = Quaternion.LookRotation(aimRay.direction);
									fireProjectileInfo.crit = this.RollCrit();
									fireProjectileInfo.damage = 10f * this.damageStat;
									fireProjectileInfo.owner = this.gameObject;
									fireProjectileInfo.projectilePrefab = Modules.Projectiles.punchShockwave;
									ProjectileManager.instance.FireProjectile(fireProjectileInfo);

									// barrage
									if (this.empowered)
                                    {
										this.penis.punchTarget = hurtBox.transform;

										fireProjectileInfo = default(FireProjectileInfo);
										fireProjectileInfo.position = hurtBox.transform.position;
										fireProjectileInfo.rotation = Quaternion.LookRotation(aimRay.direction);
										fireProjectileInfo.crit = this.RollCrit();
										fireProjectileInfo.damage = 1.2f * this.damageStat;
										fireProjectileInfo.owner = this.gameObject;
										fireProjectileInfo.projectilePrefab = Modules.Projectiles.punchBarrage;
										ProjectileManager.instance.FireProjectile(fireProjectileInfo);
									}

									this.outer.SetNextState(new PunchRecoil());
								}

								return;
							}
                        }
					}
				}
			}
		}

		private void IterateSlamCount()
        {
			this.slamCount++;

			Action<int> action = onSlamCountIncremented;
			if (action == null) return;
			action(this.slamCount);
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Frozen;
		}
	}
}
using EntityStates;
using RoR2;
using RoR2.Audio;
using RoR2.UI;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RedGuyMod.SkillStates.Ravager
{
    public class FireBeam : BaseRavagerSkillState
    {
		public GameObject muzzleflashEffectPrefab;
		public GameObject hitEffectPrefab;
		public GameObject beamVfxPrefab;
		public string enterSoundString = "";
		public string exitSoundString = "";
		public float tickRate = 30f;
		public static float damageCoefficientPerSecond = 25f;
		public float maxDuration = 2f;
		public float minDuration = 0.5f;
		public float procCoefficientPerSecond = 1f;
		public float forcePerSecond = 700f;
		public float maxDistance = 250f;
		public float minDistance = 240f;
		public float bulletRadius = 3f;
		public float recoilAmplitude = 1f;
		public float spreadBloomValue = 0.3f;
		public float maxSpread = 0f;
		public string muzzle = "HandL";
		public string animationLayerName = "";
		public string animationEnterStateName = "";
		public string animationExitStateName = "";
		public float charge;

		private GameObject blinkVfxInstance;
		private float duration;
		private float fireCountdown;
		private uint playID;

		private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

		public override void OnEnter()
		{
			//this.muzzleflashEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidBlinkVfx.prefab").WaitForCompletion();
			this.hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamImpactCorrupt.prefab").WaitForCompletion();
			this.beamVfxPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamCorrupt.prefab").WaitForCompletion();

			//this.beamVfxPrefab.transform.Find("Offset").Find("Mesh, Additive").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabTripleBeamSphere1.mat").WaitForCompletion();

			base.OnEnter();
			this.duration = Util.Remap(this.charge, 0f, 1f, this.minDuration, this.maxDuration) / this.attackSpeedStat;

			this.PlayAnimation(this.animationLayerName, "FireBeamLoop");

			this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(this.characterBody, Modules.Assets.beamCrosshair, CrosshairUtils.OverridePriority.Skill);

			this.blinkVfxInstance = UnityEngine.Object.Instantiate<GameObject>(this.beamVfxPrefab);
			this.blinkVfxInstance.transform.SetParent(this.characterBody.aimOriginTransform, false);
			this.blinkVfxInstance.transform.localPosition = Vector3.zero;
			this.blinkVfxInstance.transform.localScale *= 3.5f;

			Util.PlaySound("sfx_ravager_beam_start", this.gameObject);
			this.playID = Util.PlaySound("sfx_ravager_beamloop", this.gameObject);

			if (NetworkServer.active)
			{
				base.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
			}
		}

		public override void FixedUpdate()
		{
			base.FixedUpdate();
			this.characterBody.SetAimTimer(0.5f);
			this.characterBody.isSprinting = false;
			this.characterBody.outOfCombatStopwatch = 0f;
			this.fireCountdown -= Time.fixedDeltaTime;

			if (this.fireCountdown <= 0f)
			{
				this.fireCountdown = 1f / this.tickRate / this.attackSpeedStat;
				this.FireBullet();
			}

			this.penis.chargeValue = Util.Remap(base.fixedAge, 0f, this.duration / this.attackSpeedStat, this.charge, 0f);

			if (this.blinkVfxInstance)
			{
				Vector3 point = base.GetAimRay().GetPoint(this.maxDistance);
				RaycastHit raycastHit;
				if (Util.CharacterRaycast(base.gameObject, base.GetAimRay(), out raycastHit, this.maxDistance, LayerIndex.world.mask, QueryTriggerInteraction.UseGlobal))
				{
					point = raycastHit.point;
				}
				this.blinkVfxInstance.transform.forward = point - this.blinkVfxInstance.transform.position;
			}

			if (base.fixedAge >= this.duration && base.isAuthority)
			{
				this.outer.SetNextStateToMain();
				return;
			}
		}

		public override void OnExit()
		{
			if (this.blinkVfxInstance) VfxKillBehavior.KillVfxObject(this.blinkVfxInstance);
			if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

			base.PlayAnimation("Gesture, Override", "FireBeam", "Beam.playbackRate", 1f);

			this.penis.chargeValue = 0f;
			this.PlayAnimation(this.animationLayerName, this.animationExitStateName);
			Util.PlaySound("sfx_ravager_beam_end", this.gameObject);
			AkSoundEngine.StopPlayingID(this.playID);

			if (this.crosshairOverrideRequest != null) this.crosshairOverrideRequest.Dispose();

			base.OnExit();
		}

		private void FireBullet()
		{
			Ray aimRay = this.GetAimRay();
			this.AddRecoil(-1f * this.recoilAmplitude, -2f * this.recoilAmplitude, -0.5f * this.recoilAmplitude, 0.5f * this.recoilAmplitude);
			if (this.muzzleflashEffectPrefab) EffectManager.SimpleMuzzleFlash(this.muzzleflashEffectPrefab, this.gameObject, this.muzzle, false);

			if (base.isAuthority)
			{
				new BulletAttack
				{
					owner = this.gameObject,
					weapon = this.gameObject,
					origin = aimRay.origin,
					aimVector = aimRay.direction,
					muzzleName = this.muzzle,
					maxDistance = Mathf.Lerp(this.minDistance, this.maxDistance, UnityEngine.Random.value),
					minSpread = 0f,
					maxSpread = this.maxSpread,
					radius = this.bulletRadius,
					falloffModel = BulletAttack.FalloffModel.None,
					smartCollision = true,
					stopperMask = default(LayerMask),
					hitMask = LayerIndex.entityPrecise.mask,
					damage = (FireBeam.damageCoefficientPerSecond * this.damageStat) / this.tickRate,
					procCoefficient = this.procCoefficientPerSecond / this.tickRate,
					force = this.forcePerSecond / this.tickRate,
					isCrit = this.RollCrit(),
					hitEffectPrefab = this.hitEffectPrefab
				}.Fire();
			}

			this.characterBody.AddSpreadBloom(this.spreadBloomValue);
		}

		public override InterruptPriority GetMinimumInterruptPriority()
		{
			return InterruptPriority.Pain;
		}
	}
}
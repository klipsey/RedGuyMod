using EntityStates;
using RoR2;
using UnityEngine.Networking;

namespace RedGuyMod.SkillStates.Ravager
{
    public class SpawnState : BaseState
    {
        public static float duration = 4f;

        private CameraRigController cameraController;
        private bool initCamera;
        private bool cock;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayAnimation("Body", "Spawn");

            if (NetworkServer.active) base.characterBody.AddBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge > 0.56f && !this.cock)
            {
                this.cock = true;

                EffectManager.SimpleMuzzleFlash(EntityStates.ImpMonster.BlinkState.blinkPrefab, base.gameObject, "Chest", false);
                Util.PlaySound(EntityStates.ImpMonster.BlinkState.beginSoundString, base.gameObject);
            }

            // i don't know if all this null checking is necessary but i'd rather play it safe than spend time testing
            if (!this.cameraController)
            {
                if (base.characterBody && base.characterBody.master)
                {
                    if (base.characterBody.master.playerCharacterMasterController)
                    {
                        if (base.characterBody.master.playerCharacterMasterController.networkUser)
                        {
                            this.cameraController = base.characterBody.master.playerCharacterMasterController.networkUser.cameraRigController;
                        }
                    }
                }
            }
            else
            {
                if (!this.initCamera)
                {
                    this.initCamera = true;
                    ((RoR2.CameraModes.CameraModePlayerBasic.InstanceData)this.cameraController.cameraMode.camToRawInstanceData[this.cameraController]).SetPitchYawFromLookVector(-base.characterDirection.forward);
                }
            }

            if (base.fixedAge >= SpawnState.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (NetworkServer.active) base.characterBody.RemoveBuff(RoR2Content.Buffs.HiddenInvincibility);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}
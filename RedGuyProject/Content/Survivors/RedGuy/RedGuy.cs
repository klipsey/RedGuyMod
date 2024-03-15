using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using RoR2.CharacterAI;
using RoR2.Navigation;
using RoR2.Orbs;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using RoR2.UI;
using System.Linq;
using RedGuyMod.Content.Components;
using UnityEngine.UI;

namespace RedGuyMod.Content.Survivors
{
    internal class RedGuy
    {
        internal static RedGuy instance;

        internal static GameObject characterPrefab;
        internal static GameObject displayPrefab;

        internal static GameObject umbraMaster;

        internal static ConfigEntry<bool> forceUnlock;
        internal static ConfigEntry<bool> characterEnabled;

        public static Color characterColor = new Color(1f, 28f / 255f, 59f / 255f);

        public const string bodyName = "RobRavagerBody";

        public static int bodyRendererIndex; // use this to store the rendererinfo index containing our character's body
                                             // keep it last in the rendererinfos because teleporter particles for some reason require this. hopoo pls

        // item display stuffs
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        // buff
        internal static BuffDef grabbedBuff;
        internal static BuffDef projectileEatedBuff;
        internal static BuffDef clingDefendBuff;

        // achievements
        internal static UnlockableDef characterUnlockableDef;
        internal static UnlockableDef masteryUnlockableDef;
        internal static UnlockableDef grandMasteryUnlockableDef;
        internal static UnlockableDef voidUnlockableDef;

        internal static UnlockableDef blinkUnlockableDef;
        internal static UnlockableDef beamUnlockableDef;
        internal static UnlockableDef punchUnlockableDef;

        // skill overrides
        internal static SkillDef confirmSkillDef;
        internal static SkillDef cancelSkillDef;

        internal static SkillDef clingSlashSkillDef;
        internal static SkillDef clingStabSkillDef;
        internal static SkillDef clingDefendSkillDef;
        internal static SkillDef clingHealSkillDef;
        internal static SkillDef clingBeamSkillDef;
        internal static SkillDef clingFlourishSkillDef;

        internal static string bodyNameToken;
        internal static string primaryNameToken;
        internal static string healNameToken;

        internal void CreateCharacter()
        {
            instance = this;

            characterEnabled = Modules.Config.CharacterEnableConfig("Ravager");

            if (characterEnabled.Value)
            {
                forceUnlock = Modules.Config.ForceUnlockConfig("Ravager");

                masteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Modules.Achievements.MasteryAchievement>();
                //grandMasteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.GrandMasteryAchievement>();
                //suitUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.SuitAchievement>();
                voidUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Modules.Achievements.RavagerVoidAchievement>();

                blinkUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Modules.Achievements.RavagerWallJumpAchievement>();
                beamUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Modules.Achievements.RavagerBeamAchievement>();
                punchUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Modules.Achievements.RavagerPunchAchievement>();

                //if (!forceUnlock.Value) characterUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.DriverUnlockAchievement>();

                characterPrefab = CreateBodyPrefab(true);

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab("RavagerDisplay", characterPrefab);

                displayPrefab.GetComponentInChildren<ChildLocator>().FindChild("SwordElectricity").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunImpact.mat").WaitForCompletion();

                if (forceUnlock.Value) Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "RAVAGER");
                else Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "RAVAGER", characterUnlockableDef);

                umbraMaster = CreateMaster(characterPrefab, "RobRavagerMonsterMaster");

                grabbedBuff = Modules.Buffs.AddNewBuff("RavagerGrabbed", null, Color.white, false, false, true);
                projectileEatedBuff = Modules.Buffs.AddNewBuff("RavagerProjectileEated", null, Color.white, true, false, true);
                clingDefendBuff = Modules.Buffs.AddNewBuff("RavagerClingDefend", null, Color.white, false, false, true);
            }

            Hook();
        }

        private static GameObject CreateBodyPrefab(bool isPlayer)
        {
            bodyNameToken = MainPlugin.developerPrefix + "_RAVAGER_BODY_NAME";

            #region Body
            GameObject newPrefab = Modules.Prefabs.CreatePrefab(bodyName, "mdlRavager", new BodyInfo
            {
                armor = 0f,
                armorGrowth = 0f,
                bodyName = "RobRavagerBody",
                bodyNameToken = bodyNameToken,
                bodyColor = characterColor,
                characterPortrait = Modules.Assets.LoadCharacterIcon("Ravager"),
                crosshair = Modules.Assets.LoadCrosshair("SimpleDot"),
                damage = 12f,
                healthGrowth = 48f,
                healthRegen = 0.5f,
                jumpCount = 1,
                maxHealth = 160f,
                subtitleNameToken = MainPlugin.developerPrefix + "_RAVAGER_BODY_SUBTITLE",
                podPrefab = null,//RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
                moveSpeed = 7f,
                acceleration = 60f,
                jumpPower = 15f,
                attackSpeed = 1f,
                crit = 1f,
                modelBasePosition = Modules.Prefabs.defaultBodyInfo.modelBasePosition,
                cameraPivot = Modules.Prefabs.defaultBodyInfo.cameraPivot,
                aimOrigin = Modules.Prefabs.defaultBodyInfo.aimOrigin
            });

            ChildLocator childLocator = newPrefab.GetComponentInChildren<ChildLocator>();

            childLocator.gameObject.AddComponent<Content.Components.GenericAnimationEvents>();

            CharacterBody body = newPrefab.GetComponent<CharacterBody>();
            body.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(RedGuyMod.SkillStates.Ravager.SpawnState));
            body.bodyFlags |= CharacterBody.BodyFlags.IgnoreFallDamage;
            //body.bodyFlags |= CharacterBody.BodyFlags.SprintAnyDirection;
            //body.sprintingSpeedMultiplier = 1.75f;

            //newPrefab.AddComponent<NinjaMod.Modules.Components.NinjaController>();

            //SfxLocator sfx = newPrefab.GetComponent<SfxLocator>();
            //sfx.barkSound = "";
            //sfx.landingSound = "";
            //sfx.deathSound = "";
            //sfx.fallDamageSound = "";

            //FootstepHandler footstep = newPrefab.GetComponentInChildren<FootstepHandler>();
            //footstep.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericHugeFootstepDust");
            //footstep.baseFootstepString = "Play_moonBrother_step";
            //footstep.sprintFootstepOverrideString = "Play_moonBrother_sprint";

            //KinematicCharacterMotor characterController = newPrefab.GetComponent<KinematicCharacterMotor>();
            //characterController.CapsuleRadius = 4f;
            //characterController.CapsuleHeight = 9f;

            //CharacterDirection direction = newPrefab.GetComponent<CharacterDirection>();
            //direction.turnSpeed = 135f;

            //Interactor interactor = newPrefab.GetComponent<Interactor>();
            //interactor.maxInteractionDistance = 8f;

            //newPrefab.GetComponent<CameraTargetParams>().cameraParams = Modules.CameraParams.CreateCameraParamsWithData(DriverCameraParams.DEFAULT);

            //newPrefab.GetComponent<CharacterDirection>().turnSpeed = 720f;

            foreach (EntityStateMachine i in newPrefab.GetComponents<EntityStateMachine>())
            {
                if (i.customName == "Body") i.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.MainState));
                /*if (i.customName == "Slide")
                {
                    i.initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.WallJump));
                    i.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.WallJump));
                }*/
            }
            EntityStateMachine passiveController = newPrefab.AddComponent<EntityStateMachine>();
            passiveController.initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.WallJump));
            passiveController.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.WallJump));
            passiveController.customName = "Passive";

            //var state = isPlayer ? typeof(EntityStates.SpawnTeleporterState) : typeof(SpawnState);
            //newPrefab.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType(state);

            // schizophrenia
            newPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FuckMyAss));

            newPrefab.AddComponent<Content.Components.RedGuyController>();

            //Material elecMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunImpact.mat").WaitForCompletion();
            childLocator.FindChild("Steam").gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustSpeckledLarge.mat").WaitForCompletion();
            /*childLocator.FindChild("BlackElectricity").gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matTracerBright.mat").WaitForCompletion();
            childLocator.FindChild("SwordElectricity").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = elecMat;
            childLocator.FindChild("FootChargeL").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = elecMat;
            childLocator.FindChild("FootChargeR").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = elecMat;
            childLocator.FindChild("ArmCharge").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = elecMat;*/


            FlickerLight light1 = childLocator.FindChild("SwordLight").gameObject.AddComponent<FlickerLight>();
            FlickerLight light2 = Resources.Load<GameObject>("Prefabs/CharacterBodies/MercBody").GetComponentInChildren<CharacterModel>().baseLightInfos[1].light.gameObject.GetComponent<FlickerLight>();
            light1.sinWaves = light2.sinWaves;
            light1.light = light1.gameObject.GetComponent<Light>();

            FlickerLight light3 = childLocator.FindChild("HandLight").gameObject.AddComponent<FlickerLight>();
            light3.sinWaves = light2.sinWaves;
            light3.light = light3.gameObject.GetComponent<Light>();
            #endregion

            #region Model
            bodyRendererIndex = 0;

            Modules.Prefabs.SetupCharacterModel(newPrefab, new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Modules.Assets.CreateMaterial("matBody", 1f, Color.white)
                },
                new CustomRendererInfo
                {
                    childName = "SwordModel",
                    material = Modules.Assets.CreateMaterial("matSword", 1f, Color.white)
                },
                new CustomRendererInfo
                {
                    childName = "ImpWrapModel",
                    material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBoss.mat").WaitForCompletion()
                } }, bodyRendererIndex);
            #endregion

            CreateHitboxes(newPrefab);
            SetupHurtboxes(newPrefab);
            CreateSkills(newPrefab);
            CreateSkins(newPrefab);
            InitializeItemDisplays(newPrefab);

            return newPrefab;
        }

        private static void SetupHurtboxes(GameObject bodyPrefab)
        {
            HurtBoxGroup hurtboxGroup = bodyPrefab.GetComponentInChildren<HurtBoxGroup>();
            List<HurtBox> hurtboxes = new List<HurtBox>();

            hurtboxes.Add(bodyPrefab.GetComponentInChildren<ChildLocator>().FindChild("MainHurtbox").GetComponent<HurtBox>());

            HealthComponent healthComponent = bodyPrefab.GetComponent<HealthComponent>();

            foreach (Collider i in bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponentsInChildren<Collider>())
            {
                if (i.gameObject.name != "MainHurtbox")
                {
                    HurtBox hurtbox = i.gameObject.AddComponent<HurtBox>();
                    hurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
                    hurtbox.healthComponent = healthComponent;
                    hurtbox.isBullseye = false;
                    hurtbox.damageModifier = HurtBox.DamageModifier.Normal;
                    hurtbox.hurtBoxGroup = hurtboxGroup;

                    hurtboxes.Add(hurtbox);
                }
            }

            hurtboxGroup.hurtBoxes = hurtboxes.ToArray();
        }

        private static GameObject CreateMaster(GameObject bodyPrefab, string masterName)
        {
            GameObject newMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/LemurianMaster"), masterName, true);
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;

            #region AI
            foreach (AISkillDriver ai in newMaster.GetComponentsInChildren<AISkillDriver>())
            {
                MainPlugin.DestroyImmediate(ai);
            }

            newMaster.GetComponent<BaseAI>().fullVision = true;

            AISkillDriver revengeDriver = newMaster.AddComponent<AISkillDriver>();
            revengeDriver.customName = "Revenge";
            revengeDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            revengeDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            revengeDriver.activationRequiresAimConfirmation = true;
            revengeDriver.activationRequiresTargetLoS = false;
            revengeDriver.selectionRequiresTargetLoS = true;
            revengeDriver.maxDistance = 24f;
            revengeDriver.minDistance = 0f;
            revengeDriver.requireSkillReady = true;
            revengeDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            revengeDriver.ignoreNodeGraph = true;
            revengeDriver.moveInputScale = 1f;
            revengeDriver.driverUpdateTimerOverride = 2.5f;
            revengeDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            revengeDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            revengeDriver.maxTargetHealthFraction = Mathf.Infinity;
            revengeDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            revengeDriver.maxUserHealthFraction = 0.5f;
            revengeDriver.skillSlot = SkillSlot.Utility;

            AISkillDriver grabDriver = newMaster.AddComponent<AISkillDriver>();
            grabDriver.customName = "Grab";
            grabDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            grabDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            grabDriver.activationRequiresAimConfirmation = true;
            grabDriver.activationRequiresTargetLoS = false;
            grabDriver.selectionRequiresTargetLoS = true;
            grabDriver.maxDistance = 8f;
            grabDriver.minDistance = 0f;
            grabDriver.requireSkillReady = true;
            grabDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            grabDriver.ignoreNodeGraph = true;
            grabDriver.moveInputScale = 1f;
            grabDriver.driverUpdateTimerOverride = 0.5f;
            grabDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            grabDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            grabDriver.maxTargetHealthFraction = Mathf.Infinity;
            grabDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            grabDriver.maxUserHealthFraction = Mathf.Infinity;
            grabDriver.skillSlot = SkillSlot.Primary;

            AISkillDriver stompDriver = newMaster.AddComponent<AISkillDriver>();
            stompDriver.customName = "Stomp";
            stompDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            stompDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            stompDriver.activationRequiresAimConfirmation = true;
            stompDriver.activationRequiresTargetLoS = false;
            stompDriver.selectionRequiresTargetLoS = true;
            stompDriver.maxDistance = 32f;
            stompDriver.minDistance = 0f;
            stompDriver.requireSkillReady = true;
            stompDriver.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            stompDriver.ignoreNodeGraph = true;
            stompDriver.moveInputScale = 0.4f;
            stompDriver.driverUpdateTimerOverride = 0.5f;
            stompDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            stompDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            stompDriver.maxTargetHealthFraction = Mathf.Infinity;
            stompDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            stompDriver.maxUserHealthFraction = Mathf.Infinity;
            stompDriver.skillSlot = SkillSlot.Secondary;

            AISkillDriver followCloseDriver = newMaster.AddComponent<AISkillDriver>();
            followCloseDriver.customName = "ChaseClose";
            followCloseDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            followCloseDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            followCloseDriver.activationRequiresAimConfirmation = false;
            followCloseDriver.activationRequiresTargetLoS = false;
            followCloseDriver.maxDistance = 32f;
            followCloseDriver.minDistance = 0f;
            followCloseDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            followCloseDriver.ignoreNodeGraph = false;
            followCloseDriver.moveInputScale = 1f;
            followCloseDriver.driverUpdateTimerOverride = -1f;
            followCloseDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            followCloseDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            followCloseDriver.maxTargetHealthFraction = Mathf.Infinity;
            followCloseDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            followCloseDriver.maxUserHealthFraction = Mathf.Infinity;
            followCloseDriver.skillSlot = SkillSlot.None;

            AISkillDriver followDriver = newMaster.AddComponent<AISkillDriver>();
            followDriver.customName = "Chase";
            followDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            followDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            followDriver.activationRequiresAimConfirmation = false;
            followDriver.activationRequiresTargetLoS = false;
            followDriver.maxDistance = Mathf.Infinity;
            followDriver.minDistance = 0f;
            followDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            followDriver.ignoreNodeGraph = false;
            followDriver.moveInputScale = 1f;
            followDriver.driverUpdateTimerOverride = -1f;
            followDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            followDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            followDriver.maxTargetHealthFraction = Mathf.Infinity;
            followDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            followDriver.maxUserHealthFraction = Mathf.Infinity;
            followDriver.skillSlot = SkillSlot.None;
            followDriver.shouldSprint = true;
            #endregion

            Modules.Prefabs.masterPrefabs.Add(newMaster);

            return newMaster;
        }

        private static void CreateHitboxes(GameObject prefab)
        {
            ChildLocator childLocator = prefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Transform hitboxTransform = childLocator.FindChild("SwordHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Sword");

            hitboxTransform = childLocator.FindChild("SwordHitboxBig");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "SwordBig");

            hitboxTransform = childLocator.FindChild("DragHitbox");
            Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Drag");
        }

        private static void CreateSkills(GameObject prefab)
        {
            RedGuyPassive passive = prefab.AddComponent<RedGuyPassive>();
            Modules.Skills.CreateSkillFamilies(prefab);

            string prefix = MainPlugin.developerPrefix;
            SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = false;
            skillLocator.passiveSkill.skillNameToken = prefix + "_RAVAGER_BODY_BLOODWELL_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = prefix + "_RAVAGER_BODY_BLOODWELL_DESCRIPTION";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBloodWellIcon");

            #region Generic
            confirmSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_CONFIRM_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_CONFIRM_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_CONFIRM_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "fuck",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });

            cancelSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_CANCEL_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_CANCEL_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_CANCEL_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCancelIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCancelIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "fuck",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });
            #endregion

            #region Passive
            passive.bloodWellPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_BLOODWELL_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_BLOODWELL_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_BLOODWELL_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBloodWellIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBloodWellIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            passive.bloodWellAltPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_BLOODWELL2_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_BLOODWELL2_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_BLOODWELL2_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBloodWellIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBloodWellIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            passive.wallJumpPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_PASSIVE_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_PASSIVE_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_PASSIVE_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassiveIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassiveIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            passive.blinkPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_PASSIVE2_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_PASSIVE2_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_PASSIVE2_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBlinkIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBlinkIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            passive.legacyBlinkPassive = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_PASSIVE3_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_PASSIVE3_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_PASSIVE3_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBlinkIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBlinkIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            Modules.Skills.AddPassiveSkills(passive.bloodPassiveSkillSlot.skillFamily, new SkillDef[]{
                passive.bloodWellPassive//,
                //passive.bloodWellAltPassive
            });

            if (Modules.Config.cursed.Value)
            {
                Modules.Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, new SkillDef[]{
                    passive.wallJumpPassive,
                    passive.blinkPassive,
                    passive.legacyBlinkPassive
                });

                Modules.Skills.AddUnlockablesToFamily(passive.passiveSkillSlot.skillFamily,
                null, blinkUnlockableDef, blinkUnlockableDef);
            }
            else
            {
                Modules.Skills.AddPassiveSkills(passive.passiveSkillSlot.skillFamily, new SkillDef[]{
                    passive.wallJumpPassive,
                    passive.blinkPassive
                });

                Modules.Skills.AddUnlockablesToFamily(passive.passiveSkillSlot.skillFamily,
                null, blinkUnlockableDef);
            }
            #endregion

            #region Primary
            primaryNameToken = prefix + "_RAVAGER_BODY_PRIMARY_SLASH_NAME";

            RavagerSkillDef primary = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.Slash)),
                "Weapon",
                primaryNameToken,
                prefix + "_RAVAGER_BODY_PRIMARY_SLASH_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon2"), true);

            primary.keywordTokens = new string[]
            {
                "KEYWORD_AGILE", "KEYWORD_REDGUY_M1", "KEYWORD_REDGUY_M12"
            };

            RavagerSkillDef primary2 = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.SlashCombo)),
    "Weapon",
    prefix + "_RAVAGER_BODY_PRIMARY_SLASHCOMBO_NAME",
    prefix + "_RAVAGER_BODY_PRIMARY_SLASHCOMBO_DESCRIPTION",
    Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon2"), true);

            primary2.keywordTokens = new string[]
            {
                "KEYWORD_AGILE", "KEYWORD_REDGUY_M1", "KEYWORD_REDGUY_M12"
            };

            Modules.Skills.AddPrimarySkills(prefab,
                primary,
                primary2);

            clingSlashSkillDef = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.ClingSlash)),
                "Weapon",
                primaryNameToken,
                prefix + "_RAVAGER_BODY_PRIMARY_SLASH_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon2"), true);

            clingStabSkillDef = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.ClingStab)),
    "Weapon",
    primaryNameToken,
    prefix + "_RAVAGER_BODY_PRIMARY_SLASH_DESCRIPTION",
    Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon2"), true);

            clingDefendSkillDef = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.ClingDefend)),
"Weapon",
primaryNameToken,
prefix + "_RAVAGER_BODY_PRIMARY_SLASH_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon2"), true);
            clingDefendSkillDef.interruptPriority = EntityStates.InterruptPriority.Skill;

            clingFlourishSkillDef = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.ClingFlourish)),
"Weapon",
primaryNameToken,
prefix + "_RAVAGER_BODY_PRIMARY_SLASH_DESCRIPTION",
Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon2"), true);
            clingFlourishSkillDef.interruptPriority = EntityStates.InterruptPriority.PrioritySkill;
            #endregion

            #region Secondary
            RavagerSkillDef spinSlashSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_SECONDARY_SPINSLASH_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_SECONDARY_SPINSLASH_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_SECONDARY_SPINSLASH_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpinSlashIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSpinSlashIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.SpinSlash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
                {
                    "KEYWORD_REDGUY_M2"
                }
            });

            Modules.Skills.AddSecondarySkills(prefab, spinSlashSkillDef);
            #endregion

            #region Utility
            healNameToken = prefix + "_RAVAGER_BODY_UTILITY_HEAL_NAME";
            RavagerSkillDef healSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = healNameToken,
                skillNameToken = healNameToken,
                skillDescriptionToken = prefix + "_RAVAGER_BODY_UTILITY_HEAL_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texHealIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texHealIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.Heal)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 3f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
                {
                    "KEYWORD_REDGUY_HEAL"
                }
            });

            clingHealSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = healNameToken,
                skillNameToken = healNameToken,
                skillDescriptionToken = prefix + "_RAVAGER_BODY_UTILITY_HEAL_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texHealIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texHealIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.ClingHeal)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 3f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
                {
                    "KEYWORD_REDGUY_HEAL"
                }
            });

            RavagerSkillDef beamSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_UTILITY_BEAM_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_UTILITY_BEAM_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_UTILITY_BEAM_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBeamIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBeamIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.ChargeBeam)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
                {
                    "KEYWORD_REDGUY_BEAM"
                }
            });

            clingBeamSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_UTILITY_BEAM_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_UTILITY_BEAM_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_UTILITY_BEAM_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBeamIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBeamIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.ClingChargeBeam)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
                {
                    "KEYWORD_REDGUY_BEAM"
                }
            });

            Modules.Skills.AddUtilitySkills(prefab, /*beamSkillDef,*/ healSkillDef, beamSkillDef);

            Modules.Skills.AddUnlockablesToFamily(skillLocator.utility.skillFamily, null, beamUnlockableDef);
            #endregion

            #region Special
            RavagerSkillDef grabSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_SPECIAL_GRAB_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_SPECIAL_GRAB_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_SPECIAL_GRAB_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGrabIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGrabIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.DashGrab)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 10f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
                {
                    "KEYWORD_REDGUY_GRAB",
                    "KEYWORD_REDGUY_GRAB2"
                }
            });

            RavagerSkillDef punchSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_SPECIAL_PUNCH_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_SPECIAL_PUNCH_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_SPECIAL_PUNCH_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPunchIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPunchIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.DashPunch)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 10f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
    {
                    "KEYWORD_REDGUY_PUNCH"
    }
            });

            RavagerSkillDef throwSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_SPECIAL_THROW_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_SPECIAL_THROW_NAME",
                skillDescriptionToken = prefix + "_RAVAGER_BODY_SPECIAL_THROW_DESCRIPTION",
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGrabIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGrabIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.DashThrow)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 10f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
                keywordTokens = new string[]
{
                    "KEYWORD_REDGUY_THROW"
}
            });

            Modules.Skills.AddSpecialSkills(prefab, grabSkillDef, punchSkillDef/*, throwSkillDef*/);

            Modules.Skills.AddUnlockablesToFamily(skillLocator.special.skillFamily, null, punchUnlockableDef);
            #endregion
        }

        private static void CreateSkins(GameObject prefab)
        {
            GameObject model = prefab.GetComponentInChildren<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;

            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            List<SkinDef> skins = new List<SkinDef>();

            #region DefaultSkin
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_RAVAGER_BODY_DEFAULT_SKIN_NAME",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            defaultSkin.meshReplacements = new SkinDef.MeshReplacement[]
{
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBody"),
                    renderer = mainRenderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSword"),
                    renderer = childLocator.FindChild("SwordModel").GetComponent<SkinnedMeshRenderer>()
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshImpWrap"),
                    renderer = childLocator.FindChild("ImpWrapModel").GetComponent<SkinnedMeshRenderer>()
                }
};

            skins.Add(defaultSkin);
            #endregion

            #region MasterySkin
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_RAVAGER_BODY_MONSOON_SKIN_NAME",
    Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texVoidSkin"),
                SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matBodyVoid", 10f, Color.white),
                    Modules.Assets.CreateMaterial("matSwordVoid", 10f, Color.white),
                    Addressables.LoadAssetAsync<Material>("RoR2/DLC1/SlowOnHitVoid/BaubleVoid.mat").WaitForCompletion()
                }),
    mainRenderer,
    model,
    masteryUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBodyVoid2"),
                    renderer = mainRenderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSwordVoid"),
                    renderer = childLocator.FindChild("SwordModel").GetComponent<SkinnedMeshRenderer>()
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshVoidWrap"),
                    renderer = childLocator.FindChild("ImpWrapModel").GetComponent<SkinnedMeshRenderer>()
                }
            };

            skins.Add(masterySkin);
            #endregion

            #region MasterySkinAlternate
            SkinDef masterySkinAlternate = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_RAVAGER_BODY_MONSOON_ALTERNATE_SKIN_NAME",
    Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texVoidSkin"),
                SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matBody", 10f, Color.white),
                    Modules.Assets.CreateMaterial("matSword", 10f, Color.white),
                    Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBoss.mat").WaitForCompletion()
                }),
    mainRenderer,
    model,
    masteryUnlockableDef);

            masterySkinAlternate.meshReplacements = new SkinDef.MeshReplacement[]
            {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBodyVoid2"),
                    renderer = mainRenderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSwordVoid"),
                    renderer = childLocator.FindChild("SwordModel").GetComponent<SkinnedMeshRenderer>()
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshVoidWrap"),
                    renderer = childLocator.FindChild("ImpWrapModel").GetComponent<SkinnedMeshRenderer>()
                }
            };

            skins.Add(masterySkinAlternate);
            #endregion

            RavagerSkinDef defaultSkinDef = ScriptableObject.CreateInstance<RavagerSkinDef>();
            defaultSkinDef.name = "rsdDefault";
            defaultSkinDef.nameToken = defaultSkin.nameToken;
            defaultSkinDef.basicSwingEffectPrefab = Modules.Assets.swingEffect;
            defaultSkinDef.bigSwingEffectPrefab = Modules.Assets.bigSwingEffect;
            defaultSkinDef.leapEffectPrefab = Modules.Assets.leapEffect;
            defaultSkinDef.slashEffectPrefab = Modules.Assets.slashImpactEffect;
            defaultSkinDef.bloodOrbEffectPrefab = Modules.Assets.consumeOrb;
            defaultSkinDef.bloodBombEffectPrefab = Modules.Assets.bloodBombEffect;
            defaultSkinDef.bloodRushActivationEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion();
            defaultSkinDef.bloodOrbOverlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDissolve.mat").WaitForCompletion();
            defaultSkinDef.bloodRushOverlayMaterial = Modules.Assets.bloodOverlayMat;
            defaultSkinDef.consumeSoundString = "sfx_ravager_consume";
            defaultSkinDef.healSoundString = "sfx_ravager_steam";
            defaultSkinDef.electricityMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunImpact.mat").WaitForCompletion();
            defaultSkinDef.swordElectricityMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/ChainLightningVoid/matLightningVoid.mat").WaitForCompletion();
            defaultSkinDef.glowColor = Color.red;
            RavagerSkinCatalog.AddSkin(defaultSkinDef);

            RavagerSkinDef masterySkinDef = ScriptableObject.CreateInstance<RavagerSkinDef>();
            masterySkinDef.name = "rsdMastery";
            masterySkinDef.nameToken = masterySkin.nameToken;
            masterySkinDef.basicSwingEffectPrefab = Modules.Assets.swingEffectVoid;
            masterySkinDef.bigSwingEffectPrefab = Modules.Assets.bigSwingEffectVoid;
            masterySkinDef.leapEffectPrefab = Modules.Assets.leapEffectVoid;
            masterySkinDef.slashEffectPrefab = Modules.Assets.slashImpactEffect;
            masterySkinDef.bloodOrbEffectPrefab = Modules.Assets.consumeOrbVoid;
            masterySkinDef.bloodBombEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorCorruptDeathMuzzleflash.prefab").WaitForCompletion();
            masterySkinDef.bloodRushActivationEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorCorruptDeathMuzzleflash.prefab").WaitForCompletion();
            masterySkinDef.bloodOrbOverlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterSphereOverlay1.mat").WaitForCompletion();
            masterySkinDef.bloodRushOverlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorCorruptOverlay.mat").WaitForCompletion();
            masterySkinDef.consumeSoundString = "sfx_ravager_consume";
            masterySkinDef.healSoundString = "sfx_ravager_steam";
            masterySkinDef.electricityMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/ChainLightningVoid/matLightningVoid.mat").WaitForCompletion();
            masterySkinDef.swordElectricityMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/ChainLightningVoid/matLightningVoid.mat").WaitForCompletion();
            masterySkinDef.glowColor = new Color(157f / 255f, 42f / 255f, 179 / 255f);
            RavagerSkinCatalog.AddSkin(masterySkinDef);

            if (Modules.Config.cursed.Value)
            {
                #region VoidSkin
                SkinDef voidSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_RAVAGER_BODY_VOID_SKIN_NAME",
        Addressables.LoadAssetAsync<SkinDef>("RoR2/DLC1/VoidSurvivor/skinVoidSurvivorDefault.asset").WaitForCompletion().icon,
                    SkinRendererInfos(defaultRenderers, new Material[]
                    {
                    Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorFlesh.mat").WaitForCompletion(),
                    Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorHead.mat").WaitForCompletion(),
                    Addressables.LoadAssetAsync<Material>("RoR2/DLC1/SlowOnHitVoid/BaubleVoid.mat").WaitForCompletion()
                    }),
        mainRenderer,
        model,
        voidUnlockableDef);

                voidSkin.meshReplacements = new SkinDef.MeshReplacement[]
                {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBodyVoid"),
                    renderer = mainRenderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshHeadVoid"),
                    renderer = childLocator.FindChild("SwordModel").GetComponent<SkinnedMeshRenderer>()
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshVoidWrap2"),
                    renderer = childLocator.FindChild("ImpWrapModel").GetComponent<SkinnedMeshRenderer>()
                }
                };

                skins.Add(voidSkin);
                #endregion

                #region MahoragaSkin
                SkinDef mahoragaSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_RAVAGER_BODY_MAHORAGA_SKIN_NAME",
        Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMonsoonSkin"),
                    SkinRendererInfos(defaultRenderers, new Material[]
                    {
                    Modules.Assets.CreateMaterial("matBodyAlt"),
                    Modules.Assets.CreateMaterial("matSwordAlt"),
                    Modules.Assets.CreateMaterial("matBodyAlt")
                    }),
        mainRenderer,
        model);

                mahoragaSkin.meshReplacements = new SkinDef.MeshReplacement[]
                {
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshBodyAlt"),
                    renderer = mainRenderer
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshSwordAlt"),
                    renderer = childLocator.FindChild("SwordModel").GetComponent<SkinnedMeshRenderer>()
                },
                new SkinDef.MeshReplacement
                {
                    mesh = Modules.Assets.mainAssetBundle.LoadAsset<Mesh>("meshDivineWheel"),
                    renderer = childLocator.FindChild("ImpWrapModel").GetComponent<SkinnedMeshRenderer>()
                }
                };

                skins.Add(mahoragaSkin);
                #endregion

                RavagerSkinDef voidSkinDef = ScriptableObject.CreateInstance<RavagerSkinDef>();
                voidSkinDef.name = "rsdVoid";
                voidSkinDef.nameToken = voidSkin.nameToken;
                voidSkinDef.basicSwingEffectPrefab = Modules.Assets.swingEffectVoid;
                voidSkinDef.bigSwingEffectPrefab = Modules.Assets.bigSwingEffectVoid;
                voidSkinDef.leapEffectPrefab = Modules.Assets.leapEffectVoid;
                voidSkinDef.slashEffectPrefab = Modules.Assets.slashImpactEffect;
                voidSkinDef.bloodOrbEffectPrefab = Modules.Assets.consumeOrbVoid;
                voidSkinDef.bloodBombEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorCorruptDeathMuzzleflash.prefab").WaitForCompletion();
                voidSkinDef.bloodRushActivationEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorCorruptDeathMuzzleflash.prefab").WaitForCompletion();
                voidSkinDef.bloodOrbOverlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterSphereOverlay1.mat").WaitForCompletion();
                voidSkinDef.bloodRushOverlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorCorruptOverlay.mat").WaitForCompletion();
                voidSkinDef.consumeSoundString = "sfx_ravager_consume";
                voidSkinDef.healSoundString = "sfx_ravager_steam";
                voidSkinDef.electricityMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/ChainLightningVoid/matLightningVoid.mat").WaitForCompletion();
                voidSkinDef.swordElectricityMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/ChainLightningVoid/matLightningVoid.mat").WaitForCompletion();
                voidSkinDef.glowColor = new Color(157f / 255f, 42f / 255f, 179 / 255f);
                RavagerSkinCatalog.AddSkin(voidSkinDef);

                RavagerSkinDef mahoragaSkinDef = ScriptableObject.CreateInstance<RavagerSkinDef>();
                mahoragaSkinDef.name = "rsdMahoraga";
                mahoragaSkinDef.nameToken = mahoragaSkin.nameToken;
                mahoragaSkinDef.basicSwingEffectPrefab = Modules.Assets.swingEffectMastery;
                mahoragaSkinDef.bigSwingEffectPrefab = Modules.Assets.bigSwingEffectMastery;
                mahoragaSkinDef.leapEffectPrefab = Modules.Assets.leapEffectMastery;
                mahoragaSkinDef.slashEffectPrefab = Modules.Assets.slashImpactEffectMastery;
                mahoragaSkinDef.bloodOrbEffectPrefab = Modules.Assets.consumeOrbMastery;
                mahoragaSkinDef.bloodBombEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/OmniExplosionVFXMajorConstruct.prefab").WaitForCompletion();
                mahoragaSkinDef.bloodRushActivationEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/MuzzleflashLunarGolemTwinShot.prefab").WaitForCompletion();
                mahoragaSkinDef.bloodOrbOverlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashExpanded.mat").WaitForCompletion();
                mahoragaSkinDef.bloodRushOverlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandParentSunChannelStartBeam.mat").WaitForCompletion();
                mahoragaSkinDef.consumeSoundString = "sfx_ravager_consume_alt";
                mahoragaSkinDef.healSoundString = "sfx_ravager_wheel";
                mahoragaSkinDef.electricityMat = Addressables.LoadAssetAsync<Material>("RoR2/Junk/GrandParent/matGrandparentTeleportFlash.mat").WaitForCompletion();
                mahoragaSkinDef.swordElectricityMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Loader/matLightningLongYellow.mat").WaitForCompletion();
                mahoragaSkinDef.glowColor = Color.white;
                RavagerSkinCatalog.AddSkin(mahoragaSkinDef);
            }

            skinController.skins = skins.ToArray();
        }

        private static void InitializeItemDisplays(GameObject prefab)
        {
            CharacterModel characterModel = prefab.GetComponentInChildren<CharacterModel>();

            if (itemDisplayRuleSet == null)
            {
                itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
                itemDisplayRuleSet.name = "idrs" + bodyName;
            }

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
            characterModel.itemDisplayRuleSet.keyAssetRuleGroups = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.keyAssetRuleGroups;// itemDisplayRuleSet;
            itemDisplayRules = itemDisplayRuleSet.keyAssetRuleGroups.ToList();
        }

        internal static void SetItemDisplays()
        {
            // uhh
            Modules.ItemDisplays.PopulateDisplays();

            ReplaceItemDisplay(RoR2Content.Items.SecondarySkillMagazine, new ItemDisplayRule[]
            {
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                    limbMask = LimbFlags.None,
childName = "Sword",
localPos = new Vector3(0.00096F, -0.06695F, -0.11279F),
localAngles = new Vector3(328.2355F, 6.92052F, 165.5537F),
localScale = new Vector3(0.08365F, 0.08365F, 0.08365F)
                }
            });

            ReplaceItemDisplay(DLC1Content.Items.PermanentDebuffOnHit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayScorpion"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.00001F, 0.25541F, -0.04204F),
localAngles = new Vector3(10.25173F, 0F, 180F),
localScale = new Vector3(1.23384F, 1.23384F, 1.23384F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.RandomDamageZone, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayRandomDamageZone"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0.02159F, 0.53998F, -0.34732F),
localAngles = new Vector3(12.95415F, 0F, 0F),
localScale = new Vector3(0.13601F, 0.13601F, 0.13601F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.CritGlasses, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayGlasses"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.18539F, 0.16193F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.25238F, 0.18035F, 0.25014F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.AttackSpeedOnCrit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                    limbMask = LimbFlags.None,
childName = "Sword",
localPos = new Vector3(0.00001F, -0.25745F, 0.00959F),
localAngles = new Vector3(0F, 0F, 180F),
localScale = new Vector3(0.15442F, 0.15442F, 0.15442F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.CritGlassesVoid, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayGlassesVoid"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.20164F, 0.15033F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.2583F, 0.30333F, 0.30333F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.LunarSun, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplaySunHeadNeck"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0F, 0.43585F, 0.01679F),
localAngles = new Vector3(-0.00001F, 262.1551F, 0.00001F),
localScale = new Vector3(1.91291F, 1.99828F, 1.99828F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplaySunHead"),
                    limbMask = LimbFlags.Head,
childName = "Head",
localPos = new Vector3(0F, 0.15526F, 0.0304F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(1F, 1F, 1F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.GhostOnKill, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayMask"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0.003F, 0.18377F, 0.10972F),
localAngles = new Vector3(355.744F, 0.15F, 0F),
localScale = new Vector3(0.3983F, 0.48622F, 0.45863F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.GoldOnHit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayBoneCrown"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.16878F, 0.0021F),
localAngles = new Vector3(8.52676F, 0F, 0F),
localScale = new Vector3(1.00482F, 1.00373F, 1.00373F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.JumpBoost, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayWaxBird"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(-0.15614F, -0.05199F, -0.09115F),
localAngles = new Vector3(0F, 342.8027F, 0F),
localScale = new Vector3(1F, 1F, 1F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.KillEliteFrenzy, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.19467F, 0.035F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.25F, 0.25F, 0.25F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.LunarPrimaryReplacement, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayBirdEye"),
                    limbMask = LimbFlags.None,
childName = "Sword",
localPos = new Vector3(0.00001F, -0.30349F, 0.00004F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.Crowbar, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayCrowbar"),
                    limbMask = LimbFlags.None,
childName = "Sword",
localPos = new Vector3(0.05007F, -0.00044F, 0.00026F),
localAngles = new Vector3(2.08417F, 359.7186F, 359.4993F),
localScale = new Vector3(0.4F, 0.4F, 0.4F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.ArmorPlate, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0.11703F, 0.34167F, -0.14988F),
localAngles = new Vector3(329.9208F, 281.8911F, 237.3754F),
localScale = new Vector3(-0.2958F, 0.2958F, 0.29581F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.Behemoth, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayBehemoth"),
                    limbMask = LimbFlags.None,
childName = "LowerArmL",
localPos = new Vector3(0.17737F, 0.28677F, -0.00002F),
localAngles = new Vector3(350.773F, 90F, 0F),
localScale = new Vector3(0.09F, 0.09F, 0.09F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.FireballsOnHit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayFireballsOnHit"),
                    limbMask = LimbFlags.None,
childName = "Sword",
localPos = new Vector3(-0.1916F, 1.05205F, 0.00002F),
localAngles = new Vector3(280.8255F, 270F, 180F),
localScale = new Vector3(0.11599F, 0.11599F, 0.11599F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.FlatHealth, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplaySteakCurved"),
                    limbMask = LimbFlags.None,
childName = "Sword",
localPos = new Vector3(-0.00001F, 1.14147F, 0.11173F),
localAngles = new Vector3(294.7101F, -0.00001F, 180F),
localScale = new Vector3(0.15F, 0.15F, 0.15F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.Medkit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayMedkit"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0.25141F, -0.09947F, -0.13521F),
localAngles = new Vector3(270.422F, 349.1096F, 141.4891F),
localScale = new Vector3(0.75172F, 0.75172F, 0.75172F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.NearbyDamageBonus, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayDiamond"),
                    limbMask = LimbFlags.None,
childName = "HandR",
localPos = new Vector3(-0.05574F, 0.10007F, -0.00403F),
localAngles = new Vector3(2.93824F, 348.6614F, 13.66504F),
localScale = new Vector3(0.06821F, 0.06821F, 0.06821F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.Bandolier, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayBandolier"),
                    limbMask = LimbFlags.None,
childName = "ThighR",
localPos = new Vector3(-0.00913F, 0.33169F, 0.0145F),
localAngles = new Vector3(90F, 294.7478F, 0F),
localScale = new Vector3(0.37958F, 0.54297F, 0.29605F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.BarrierOnOverHeal, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayAegis"),
                    limbMask = LimbFlags.None,
childName = "Sword",
localPos = new Vector3(0.04708F, 0.44137F, 0.00002F),
localAngles = new Vector3(90F, 270F, 0F),
localScale = new Vector3(0.25808F, 0.25808F, 0.25808F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.BleedOnHit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayTriTip"),
                    limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0.10979F, 0.34789F, 0.35536F),
localAngles = new Vector3(23.47682F, 195.9212F, 8.74267F),
localScale = new Vector3(0.65499F, 0.65499F, 0.65499F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.FragileDamageBonus, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayDelicateWatch"),
                    limbMask = LimbFlags.None,
childName = "HandL",
localPos = new Vector3(-0.00156F, 0.04171F, 0.01022F),
localAngles = new Vector3(90F, 87.54613F, 0F),
localScale = new Vector3(0.67914F, 0.89103F, 0.89103F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.PrimarySkillShuriken, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayShuriken"),
                    limbMask = LimbFlags.None,
childName = "HandL",
localPos = new Vector3(0.08106F, 0.10368F, 0.06106F),
localAngles = new Vector3(1.42652F, 354.053F, 77.58773F),
localScale = new Vector3(0.43691F, 0.43691F, 0.43691F)
                }
});

            ReplaceItemDisplay(DLC1Content.Items.MoreMissile, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayICBM"),
                    limbMask = LimbFlags.None,
childName = "LowerArmL",
localPos = new Vector3(-0.0953F, 0.43344F, -0.00002F),
localAngles = new Vector3(0F, 0F, 199.9023F),
localScale = new Vector3(0.33272F, 0.24947F, 0.33272F)
                }
});

            if (MainPlugin.litInstalled) SetLITDisplays();

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            //itemDisplayRuleSet.GenerateRuntimeValues();
        }

        internal static void SetLITDisplays()
        {
            return;
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = LostInTransit.LITContent.Items.Lopper,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
        {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayLopper"),
                            limbMask = LimbFlags.None,
childName = "Chest",
localPos = new Vector3(0F, 0.20282F, -0.19089F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.19059F, 0.19059F, 0.19059F)
                        }
        }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = LostInTransit.LITContent.Items.Chestplate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
{
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayBackPlate"),
                            limbMask = LimbFlags.None,
             childName = "Chest",
localPos = new Vector3(0F, 0.23366F, 0.01011F),
localAngles = new Vector3(349.1311F, 0F, 0F),
localScale = new Vector3(0.13457F, 0.19557F, 0.19557F)
                        }
}
                }
            });
        }

        internal static void ReplaceItemDisplay(Object keyAsset, ItemDisplayRule[] newDisplayRules)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup[] cock = itemDisplayRules.ToArray();
            for (int i = 0; i < cock.Length; i++)
            {
                if (cock[i].keyAsset == keyAsset)
                {
                    // replace the item display rule
                    cock[i].displayRuleGroup.rules = newDisplayRules;
                }
            }
            itemDisplayRules = cock.ToList();
        }

        private static CharacterModel.RendererInfo[] SkinRendererInfos(CharacterModel.RendererInfo[] defaultRenderers, Material[] materials)
        {
            CharacterModel.RendererInfo[] newRendererInfos = new CharacterModel.RendererInfo[defaultRenderers.Length];
            defaultRenderers.CopyTo(newRendererInfos, 0);

            newRendererInfos[0].defaultMaterial = materials[0];
            newRendererInfos[1].defaultMaterial = materials[1];
            newRendererInfos[2].defaultMaterial = materials[2];

            return newRendererInfos;
        }

        private static void Hook()
        {
            RoR2.UI.HUD.onHudTargetChangedGlobal += HUDSetup;

            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;

            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;// the most useless hook ever.
        }

        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            // this is beyond stupid lmfao who let this monkey code
            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("RobRavagerBody"))
            {
                foreach (LanguageTextMeshController i in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>())
                {
                    if (i && i.token == "LOADOUT_SKILL_MISC") i.token = "Passive";
                }
            }
        }

        private static void HealthComponent_UpdateLastHitTime(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
        {
            orig(self, damageValue, damagePosition, damageIsSilent, attacker);

            if (self && self.body && self.body.HasBuff(RedGuy.grabbedBuff))
            {
                if (self.health <= 0f) self.health = 1f;
            }
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);

            if (self && self.body && self.body.HasBuff(RedGuy.grabbedBuff))
            {
                if (self.health <= 0f) self.health = 1f;
            }
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.victimBody && damageReport.victimBody.HasBuff(RedGuy.grabbedBuff))
            {
                damageReport.victimBody = null;
                damageReport.victim = null;
                damageReport.attacker = null;
                damageReport.attackerBody = null;
                return;
            }

            if (damageReport.attacker && damageReport.attackerBody)
            {
                if (damageReport.attacker.name.Contains(RedGuy.bodyName))
                {
                    if (damageReport.victim && damageReport.victimBody && damageReport.victimBody.healthComponent && damageReport.victimBody.healthComponent.health <= 0f)
                    {
                        GrabTracker tracker = damageReport.victim.gameObject.GetComponent<GrabTracker>();
                        ConsumeTracker tracker2 = damageReport.victim.gameObject.GetComponent<ConsumeTracker>();

                        if (tracker || tracker2)
                        {
                            CharacterBody targetBody = damageReport.attackerBody;

                            if (tracker) targetBody = tracker.attackerBody;
                            if (tracker2) targetBody = tracker2.attackerBody;

                            ConsumeOrb orb = new ConsumeOrb();
                            orb.origin = damageReport.victim.transform.position;
                            orb.target = Util.FindBodyMainHurtBox(targetBody);
                            OrbManager.instance.AddOrb(orb);

                            Vector3 effectPos = damageReport.victim.transform.position;
                            // snap to ground here
                            if (tracker)
                            {
                                RaycastHit raycastHit;
                                if (Physics.Raycast(effectPos, Vector3.down, out raycastHit, 10f, LayerIndex.world.mask))
                                {
                                    effectPos = raycastHit.point;
                                }
                            }

                            if (Modules.Assets.bloodExplosionOverrides.ContainsKey(damageReport.victim.name))
                            {
                                EffectManager.SpawnEffect(Modules.Assets.bloodExplosionOverrides[damageReport.victim.name], new EffectData
                                {
                                    origin = effectPos,
                                    rotation = Quaternion.identity,
                                    color = Color.white
                                }, true);
                            }
                            else
                            {
                                if (damageReport.victimBody.modelLocator && damageReport.victimBody.modelLocator.modelTransform)
                                {
                                    SurfaceDefProvider surfaceDefProvider = damageReport.victimBody.modelLocator.modelTransform.GetComponentInChildren<SurfaceDefProvider>();
                                    if (surfaceDefProvider && surfaceDefProvider.surfaceDef)
                                    {
                                        EffectManager.SpawnEffect(Modules.Assets.genericBloodExplosionEffect, new EffectData
                                        {
                                            origin = effectPos,
                                            rotation = Quaternion.identity,
                                            color = surfaceDefProvider.surfaceDef.approximateColor
                                        }, true);
                                    }
                                    else
                                    {
                                        EffectManager.SpawnEffect(Modules.Assets.largeBloodExplosionEffect, new EffectData
                                        {
                                            origin = effectPos,
                                            rotation = Quaternion.identity,
                                            color = Color.white
                                        }, true);
                                    }
                                }
                                else
                                {
                                    EffectManager.SpawnEffect(Modules.Assets.largeBloodExplosionEffect, new EffectData
                                    {
                                        origin = effectPos,
                                        rotation = Quaternion.identity,
                                        color = Color.white
                                    }, true);
                                }
                            }

                            CharacterDeathBehavior deathBehavior = damageReport.victimBody.GetComponent<CharacterDeathBehavior>();
                            if (deathBehavior) deathBehavior.deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.ConsumedDeath));
                        }
                    }
                }
            }
        }

        internal static void HUDSetup(RoR2.UI.HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster.bodyPrefab == RedGuy.characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;

                /*if (DriverPlugin.riskUIInstalled)
                {
                    RiskUIHudSetup(hud);
                    return;
                }*/

                Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                if (Modules.Config.oldBloodWell.Value)
                {
                    // you're back :-)
                    if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BloodGauge"))
                    {
                        GameObject bloodGauge = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                        bloodGauge.name = "BloodGauge";

                        GameObject.DestroyImmediate(bloodGauge.transform.GetChild(0).gameObject);
                        MonoBehaviour.Destroy(bloodGauge.GetComponentInChildren<LevelText>());
                        MonoBehaviour.Destroy(bloodGauge.GetComponentInChildren<ExpBar>());

                        BloodGauge bloodGaugeComponent = bloodGauge.AddComponent<BloodGauge>();
                        bloodGaugeComponent.targetHUD = hud;
                        bloodGaugeComponent.fillRectTransform = bloodGauge.transform.Find("ExpBarRoot").GetChild(0).GetChild(0).GetComponent<RectTransform>();

                        bloodGauge.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);
                        bloodGauge.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>().token = "Blood Well";

                        bloodGauge.transform.Find("ExpBarRoot").GetChild(0).GetComponent<Image>().enabled = true;

                        bloodGauge.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                        RectTransform rect = bloodGauge.GetComponent<RectTransform>();
                        rect.anchorMax = new Vector2(1f, 1f);
                        rect.anchoredPosition = new Vector2(0f, -20f);
                    }
                }
                else
                {
                    GameObject chargeRing = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("ChargeRing"));
                    chargeRing.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                    RectTransform rect = chargeRing.GetComponent<RectTransform>();

                    if (Modules.Config.centeredBloodWell.Value)
                    {
                        rect.localScale = new Vector3(0.5f, 0.5f, 1f);
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(0f, 0f);
                        rect.pivot = new Vector2(0.5f, 0.5f);
                        rect.anchoredPosition = new Vector2(0f, 0f);
                        rect.localPosition = new Vector3(0f, 0f, 0f);
                    }
                    else
                    {
                        rect.localScale = new Vector3(0.4f, 0.4f, 1f);
                        rect.anchorMin = new Vector2(0f, 0f);
                        rect.anchorMax = new Vector2(0f, 0f);
                        rect.pivot = new Vector2(0.5f, 0f);
                        rect.anchoredPosition = new Vector2(50f, 0f);
                        rect.localPosition = new Vector3(65f, -75f, 0f);
                    }

                    var p = chargeRing.transform.GetChild(0).gameObject.AddComponent<Content.Components.BloodGauge2>();
                    p.targetHUD = hud;
                    p.fillBar = p.GetComponent<Image>();
                }
            }
        }
    }
}
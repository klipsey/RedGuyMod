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

        public static Color characterColor = Color.red;

        public const string bodyName = "RobRedGuyBody";

        public static int bodyRendererIndex; // use this to store the rendererinfo index containing our character's body
                                             // keep it last in the rendererinfos because teleporter particles for some reason require this. hopoo pls

        // item display stuffs
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        internal static UnlockableDef characterUnlockableDef;
        internal static UnlockableDef masteryUnlockableDef;
        internal static UnlockableDef grandMasteryUnlockableDef;

        internal static UnlockableDef supplyDropUnlockableDef;

        // skill overrides
        internal static SkillDef confirmSkillDef;
        internal static SkillDef cancelSkillDef;

        internal static string bodyNameToken;

        internal void CreateCharacter()
        {
            instance = this;

            characterEnabled = Modules.Config.CharacterEnableConfig("RedGuy");

            if (characterEnabled.Value)
            {
                forceUnlock = Modules.Config.ForceUnlockConfig("RedGuy");

                //masteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.MasteryAchievement>();
                //grandMasteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.GrandMasteryAchievement>();
                //suitUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.SuitAchievement>();

                //supplyDropUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.SupplyDropAchievement>();

                //if (!forceUnlock.Value) characterUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.DriverUnlockAchievement>();

                characterPrefab = CreateBodyPrefab(true);

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab("RedGuyDisplay", characterPrefab);

                if (forceUnlock.Value) Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "REDGUY");
                else Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "REDGUY", characterUnlockableDef);

                umbraMaster = CreateMaster(characterPrefab, "RobRedGuyMonsterMaster");
            }

            Hook();
        }

        private static GameObject CreateBodyPrefab(bool isPlayer)
        {
            bodyNameToken = MainPlugin.developerPrefix + "_REDGUY_BODY_NAME";

            #region Body
            GameObject newPrefab = Modules.Prefabs.CreatePrefab(bodyName, "mdlRedGuy", new BodyInfo
            {
                armor = 20f,
                armorGrowth = 0f,
                bodyName = "RobDriverBody",
                bodyNameToken = bodyNameToken,
                bodyColor = characterColor,
                characterPortrait = Modules.Assets.LoadCharacterIcon("RedGuy"),
                crosshair = Modules.Assets.LoadCrosshair("Standard"),
                damage = 12f,
                healthGrowth = 0f,
                healthRegen = 0f,
                jumpCount = 2,
                maxHealth = 100f,
                subtitleNameToken = MainPlugin.developerPrefix + "_REDGUY_BODY_SUBTITLE",
                podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
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

            //CharacterBody body = newPrefab.GetComponent<CharacterBody>();
            //body.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(SpawnState));
            //body.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage;
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

            //newPrefab.GetComponent<EntityStateMachine>().mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MainState));

            //var state = isPlayer ? typeof(EntityStates.SpawnTeleporterState) : typeof(SpawnState);
            //newPrefab.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType(state);

            // schizophrenia
            //newPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FuckMyAss));

            //newPrefab.AddComponent<Modules.Components.DriverController>();
            #endregion

            #region Model
            Material mainMat = Modules.Assets.CreateMaterial("matRedGuy", 1f, Color.white);

            bodyRendererIndex = 0;

            Modules.Prefabs.SetupCharacterModel(newPrefab, new CustomRendererInfo[] {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = mainMat
                },
                new CustomRendererInfo
                {
                    childName = "GunModelL",
                    material = mainMat
                },
                new CustomRendererInfo
                {
                    childName = "GunModelR",
                    material = mainMat
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
            //ChildLocator childLocator = prefab.GetComponentInChildren<ChildLocator>();
            //GameObject model = childLocator.gameObject;

            //Transform hitboxTransform = childLocator.FindChild("PunchHitbox");
            //Modules.Prefabs.SetupHitbox(model, hitboxTransform, "Punch");
        }

        private static void CreateSkills(GameObject prefab)
        {
            Modules.Skills.CreateSkillFamilies(prefab);

            string prefix = MainPlugin.developerPrefix;
            SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = prefix + "_REDGUY_BODY_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = prefix + "_REDGUY_BODY_PASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassiveIcon");

            confirmSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_REDGUY_BODY_CONFIRM_NAME",
                skillNameToken = prefix + "_REDGUY_BODY_CONFIRM_NAME",
                skillDescriptionToken = prefix + "_REDGUY_BODY_CONFIRM_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
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
                skillName = prefix + "_REDGUY_BODY_CANCEL_NAME",
                skillNameToken = prefix + "_REDGUY_BODY_CANCEL_NAME",
                skillDescriptionToken = prefix + "_REDGUY_BODY_CANCEL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texCancelIcon"),
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

            //#region Primary
            /*Modules.Skills.AddPrimarySkills(prefab,
                Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shoot)), "Weapon", prefix + "_DRIVER_BODY_PRIMARY_PISTOL_NAME", prefix + "_DRIVER_BODY_PRIMARY_PISTOL_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false));//,
                                                                                                                                                                                                                                                                                                                           //Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Revolver.Shoot)), "Weapon", prefix + "_DRIVER_BODY_PRIMARY_PISTOL_NAME", prefix + "_DRIVER_BODY_PRIMARY_PISTOL_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false));
            #endregion

            #region Secondary
            SkillDef steadyAimSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            SkillDef pissSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_PISTOL_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Revolver.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Driver.shotgunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
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
            });

            Driver.riotShotgunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
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
            });

            Driver.slugShotgunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
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
            });

            Driver.machineGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texZapIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MachineGun.Zap)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
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
            });

            Driver.heavyMachineGunSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_MACHINEGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texZapIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MachineGun.Zap)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
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
            });

            Driver.rocketLauncherSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
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
            });

            Driver.bazookaSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SHOTGUN_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Bash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 6f,
                beginSkillCooldownOnSkillEnd = false,
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
            });

            Driver.sniperSecondarySkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SECONDARY_SNIPER_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SECONDARY_SNIPER_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SECONDARY_SNIPER_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SniperRifle.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Modules.Skills.AddSecondarySkills(prefab, steadyAimSkillDef, pissSkillDef);
            #endregion

            #region Utility
            SkillDef slideSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_UTILITY_SLIDE_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_UTILITY_SLIDE_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_UTILITY_SLIDE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlideIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Slide)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            SkillDef skateboardSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlideIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Skateboard.Start)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            skateCancelSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_UTILITY_SKATEBOARD_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlideIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Skateboard.Stop)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddUtilitySkills(prefab, slideSkillDef, skateboardSkillDef);
            #endregion

            #region Special
            SkillDef stunGrenadeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texStunGrenadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.ThrowGrenade)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
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
                stockToConsume = 1
            });

            SkillDef knifeSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_GRENADE_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texStunGrenadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SwingKnife)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 18f,
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
                stockToConsume = 1
            });

            SkillDef supplyDropSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_NAME",
                skillNameToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_NAME",
                skillDescriptionToken = prefix + "_DRIVER_BODY_SPECIAL_SUPPLY_DROP_DESCRIPTION",
                skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SupplyDrop.AimSupplyDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 1
            });

            Modules.Skills.AddSpecialSkills(prefab, stunGrenadeSkillDef, supplyDropSkillDef);

            Modules.Skills.AddUnlockablesToFamily(skillLocator.special.skillFamily, null, supplyDropUnlockableDef);
            #endregion
            */
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
            SkinDef defaultSkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_DRIVER_BODY_DEFAULT_SKIN_NAME",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                defaultRenderers,
                mainRenderer,
                model);

            skins.Add(defaultSkin);
            #endregion

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
childName = "GunR",
localPos = new Vector3(0.00888F, -0.03648F, -0.20898F),
localAngles = new Vector3(39.35415F, 348.9445F, 164.0792F),
localScale = new Vector3(0.06F, 0.06F, 0.06F)
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
localPos = new Vector3(0.0006F, 0.25054F, 0.04672F),
localAngles = new Vector3(314.7648F, 358.1459F, 0.48047F),
localScale = new Vector3(0.30902F, 0.09537F, 0.30934F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.AttackSpeedOnCrit, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                    limbMask = LimbFlags.None,
childName = "UpperArmR",
localPos = new Vector3(-0.01092F, 0.02048F, -0.00403F),
localAngles = new Vector3(309.4066F, 250.1116F, 175.7708F),
localScale = new Vector3(0.363F, 0.363F, 0.363F)
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
localPos = new Vector3(0F, 0.1555F, 0.11598F),
localAngles = new Vector3(340.0668F, 0F, 0F),
localScale = new Vector3(0.30387F, 0.39468F, 0.46147F)
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
localPos = new Vector3(-0.02605F, 0.38179F, -0.0112F),
localAngles = new Vector3(-0.00001F, 262.1551F, 0.00001F),
localScale = new Vector3(1.76594F, 1.84475F, 1.84475F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplaySunHead"),
                    limbMask = LimbFlags.Head,
childName = "Head",
localPos = new Vector3(0F, 0.10143F, -0.01147F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
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
localPos = new Vector3(0.0029F, 0.15924F, 0.07032F),
localAngles = new Vector3(355.7367F, 0.15F, 0F),
localScale = new Vector3(0.6F, 0.6F, 0.6F)
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
localPos = new Vector3(0F, 0.15159F, -0.0146F),
localAngles = new Vector3(8.52676F, 0F, 0F),
localScale = new Vector3(0.90509F, 0.90509F, 0.90509F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.JumpBoost, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayWaxBird"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, -0.228F, -0.108F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.79857F, 0.79857F, 0.79857F)
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
localPos = new Vector3(0F, 0.12823F, 0.035F),
localAngles = new Vector3(0F, 0F, 0F),
localScale = new Vector3(0.17982F, 0.17982F, 0.17982F)
                }
});

            ReplaceItemDisplay(RoR2Content.Items.LunarPrimaryReplacement, new ItemDisplayRule[]
{
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = Modules.ItemDisplays.LoadDisplay("DisplayBirdEye"),
                    limbMask = LimbFlags.None,
childName = "Head",
localPos = new Vector3(0F, 0.18736F, 0.08896F),
localAngles = new Vector3(306.9798F, 180F, 180F),
localScale = new Vector3(0.31302F, 0.31302F, 0.31302F)
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

            return newRendererInfos;
        }

        private static void Hook()
        {
            RoR2.UI.HUD.onHudTargetChangedGlobal += HUDSetup;
        }

        internal static void HUDSetup(RoR2.UI.HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster.bodyPrefab == RedGuy.characterPrefab)
            {
                /*var skillsContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomRightCluster").Find("Scaler");

                skillsContainer.Find("SprintCluster").gameObject.SetActive(false);
                skillsContainer.Find("InventoryCluster").gameObject.SetActive(false);

                GameObject weaponSlot = GameObject.Instantiate(skillsContainer.Find("EquipmentSlot").gameObject, skillsContainer);
                weaponSlot.name = "WeaponSlot";

                EquipmentIcon equipmentIconComponent = weaponSlot.GetComponent<EquipmentIcon>();
                Components.WeaponIcon weaponIconComponent = weaponSlot.AddComponent<Components.WeaponIcon>();

                weaponIconComponent.iconImage = equipmentIconComponent.iconImage;
                weaponIconComponent.displayRoot = equipmentIconComponent.displayRoot;
                weaponIconComponent.flashPanelObject = equipmentIconComponent.stockFlashPanelObject;
                weaponIconComponent.reminderFlashPanelObject = equipmentIconComponent.reminderFlashPanelObject;
                weaponIconComponent.isReadyPanelObject = equipmentIconComponent.isReadyPanelObject;
                weaponIconComponent.tooltipProvider = equipmentIconComponent.tooltipProvider;
                weaponIconComponent.targetHUD = hud;

                weaponSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480f, -17.1797f);

                HGTextMeshProUGUI keyText = weaponSlot.transform.Find("DisplayRoot").Find("EquipmentTextBackgroundPanel").Find("EquipmentKeyText").gameObject.GetComponent<HGTextMeshProUGUI>();
                keyText.gameObject.GetComponent<InputBindingDisplayController>().enabled = false;
                keyText.text = "Weapon";

                weaponSlot.transform.Find("DisplayRoot").Find("EquipmentStack").gameObject.SetActive(false);
                weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject.SetActive(false);

                // duration bar
                GameObject chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("ChargeBar"));
                chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

                RectTransform rect = chargeBar.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(-10f, 13f);
                rect.localPosition = new Vector3(-33f, -10f, 0f);
                rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                weaponIconComponent.durationDisplay = chargeBar;
                weaponIconComponent.durationBar = chargeBar.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

                MonoBehaviour.Destroy(equipmentIconComponent);*/
            }
        }
    }
}
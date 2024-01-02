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

        public static Color characterColor = Color.red;

        public const string bodyName = "RobRavagerBody";

        public static int bodyRendererIndex; // use this to store the rendererinfo index containing our character's body
                                             // keep it last in the rendererinfos because teleporter particles for some reason require this. hopoo pls

        // item display stuffs
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        // buff
        internal static BuffDef grabbedBuff;

        internal static UnlockableDef characterUnlockableDef;
        internal static UnlockableDef masteryUnlockableDef;
        internal static UnlockableDef grandMasteryUnlockableDef;

        // skill overrides
        internal static SkillDef confirmSkillDef;
        internal static SkillDef cancelSkillDef;

        internal static string bodyNameToken;

        internal void CreateCharacter()
        {
            instance = this;

            characterEnabled = Modules.Config.CharacterEnableConfig("Ravager");

            if (characterEnabled.Value)
            {
                forceUnlock = Modules.Config.ForceUnlockConfig("Ravager");

                masteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<RedGuyMod.Modules.Achievements.MasteryAchievement>();
                //grandMasteryUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.GrandMasteryAchievement>();
                //suitUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.SuitAchievement>();

                //supplyDropUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.SupplyDropAchievement>();

                //if (!forceUnlock.Value) characterUnlockableDef = R2API.UnlockableAPI.AddUnlockable<Achievements.DriverUnlockAchievement>();

                characterPrefab = CreateBodyPrefab(true);

                displayPrefab = Modules.Prefabs.CreateDisplayPrefab("RavagerDisplay", characterPrefab);

                displayPrefab.GetComponentInChildren<ChildLocator>().FindChild("SwordElectricity").gameObject.GetComponent<ParticleSystemRenderer>().trailMaterial = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunImpact.mat").WaitForCompletion();

                if (forceUnlock.Value) Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "RAVAGER");
                else Modules.Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "RAVAGER", characterUnlockableDef);

                umbraMaster = CreateMaster(characterPrefab, "RobRavagerMonsterMaster");

                grabbedBuff = Modules.Buffs.AddNewBuff("RavagerGrabbed", null, Color.white, false, false, true);
            }

            Hook();
        }

        private static GameObject CreateBodyPrefab(bool isPlayer)
        {
            bodyNameToken = MainPlugin.developerPrefix + "_RAVAGER_BODY_NAME";

            #region Body
            GameObject newPrefab = Modules.Prefabs.CreatePrefab(bodyName, "mdlRavager", new BodyInfo
            {
                armor = 20f,
                armorGrowth = 0f,
                bodyName = "RobRavagerBody",
                bodyNameToken = bodyNameToken,
                bodyColor = characterColor,
                characterPortrait = Modules.Assets.LoadCharacterIcon("Ravager"),
                crosshair = Modules.Assets.LoadCrosshair("SimpleDot"),
                damage = 12f,
                healthGrowth = 48f,
                healthRegen = 2.5f,
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

            foreach (EntityStateMachine i in newPrefab.GetComponents<EntityStateMachine>())
            {
                if (i.customName == "Body") i.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.MainState));
                if (i.customName == "Slide")
                {
                    i.initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.WallJump));
                    i.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.WallJump));
                }
            }

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
            Modules.Skills.CreateSkillFamilies(prefab);

            string prefix = MainPlugin.developerPrefix;
            SkillLocator skillLocator = prefab.GetComponent<SkillLocator>();

            skillLocator.passiveSkill.enabled = true;
            skillLocator.passiveSkill.skillNameToken = prefix + "_RAVAGER_BODY_PASSIVE_NAME";
            skillLocator.passiveSkill.skillDescriptionToken = prefix + "_RAVAGER_BODY_PASSIVE_DESCRIPTION";
            skillLocator.passiveSkill.icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPassiveIcon");

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

            #region Primary
            RavagerSkillDef primary = Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.Slash)),
                "Weapon",
                prefix + "_RAVAGER_BODY_PRIMARY_SLASH_NAME",
                prefix + "_RAVAGER_BODY_PRIMARY_SLASH_DESCRIPTION",
                Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"), Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon2"), true);

            primary.keywordTokens = new string[]
            {
                "KEYWORD_AGILE", "KEYWORD_REDGUY_M1", "KEYWORD_REDGUY_M12"
            };

            Modules.Skills.AddPrimarySkills(prefab,
                primary);                                                                                                                                                                                                                                                                      //Modules.Skills.CreatePrimarySkillDef(new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Revolver.Shoot)), "Weapon", prefix + "_DRIVER_BODY_PRIMARY_PISTOL_NAME", prefix + "_DRIVER_BODY_PRIMARY_PISTOL_DESCRIPTION", Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false));
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
                cancelSprintingOnActivation = true,
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
            RavagerSkillDef healSkillDef = Modules.Skills.CreateSkillDef(new SkillDefInfo
            {
                skillName = prefix + "_RAVAGER_BODY_UTILITY_HEAL_NAME",
                skillNameToken = prefix + "_RAVAGER_BODY_UTILITY_HEAL_NAME",
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
                mustKeyPress = false,
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
                baseIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGrappleIcon"),
                empoweredIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGrappleIcon2"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Ravager.ChargeBeam)),
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
                    "KEYWORD_REDGUY_GRAB"
                }
            });

            Modules.Skills.AddSpecialSkills(prefab, grabSkillDef);
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

            skins.Add(defaultSkin);
            #endregion

            #region MasterySkin
            SkinDef masterySkin = Modules.Skins.CreateSkinDef(MainPlugin.developerPrefix + "_RAVAGER_BODY_MONSOON_SKIN_NAME",
    Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMonsoonSkin"),
                SkinRendererInfos(defaultRenderers, new Material[]
                {
                    Modules.Assets.CreateMaterial("matBodyAlt", 1f, Color.white),
                    Modules.Assets.CreateMaterial("matSwordAlt"),
                    Modules.Assets.CreateMaterial("matBodyAlt", 1f, Color.white)
                }),
    mainRenderer,
    model,
    masteryUnlockableDef);

            masterySkin.meshReplacements = new SkinDef.MeshReplacement[]
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

            skins.Add(masterySkin);
            #endregion

            skinController.skins = skins.ToArray();



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
            defaultSkinDef.consumeSoundString = "sfx_ravager_consume";
            defaultSkinDef.healSoundString = "sfx_ravager_steam";
            defaultSkinDef.electricityMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Railgunner/matRailgunImpact.mat").WaitForCompletion();
            defaultSkinDef.swordElectricityMat = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/ChainLightningVoid/matLightningVoid.mat").WaitForCompletion();
            defaultSkinDef.glowColor = Color.red;
            RavagerSkinCatalog.AddSkin(defaultSkinDef);

            RavagerSkinDef masterySkinDef = ScriptableObject.CreateInstance<RavagerSkinDef>();
            masterySkinDef.name = "rsdMastery";
            masterySkinDef.nameToken = masterySkin.nameToken;
            masterySkinDef.basicSwingEffectPrefab = Modules.Assets.swingEffectMastery;
            masterySkinDef.bigSwingEffectPrefab = Modules.Assets.bigSwingEffectMastery;
            masterySkinDef.leapEffectPrefab = Modules.Assets.leapEffectMastery;
            masterySkinDef.slashEffectPrefab = Modules.Assets.slashImpactEffectMastery;
            masterySkinDef.bloodOrbEffectPrefab = Modules.Assets.consumeOrbMastery;
            masterySkinDef.bloodBombEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/OmniExplosionVFXMajorConstruct.prefab").WaitForCompletion();
            masterySkinDef.bloodRushActivationEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/MuzzleflashLunarGolemTwinShot.prefab").WaitForCompletion();
            masterySkinDef.bloodOrbOverlayMaterial = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressFlashExpanded.mat").WaitForCompletion();
            masterySkinDef.consumeSoundString = "sfx_ravager_consume_alt";
            masterySkinDef.healSoundString = "sfx_ravager_wheel";
            masterySkinDef.electricityMat = Addressables.LoadAssetAsync<Material>("RoR2/Junk/GrandParent/matGrandparentTeleportFlash.mat").WaitForCompletion();
            masterySkinDef.swordElectricityMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Loader/matLightningLongYellow.mat").WaitForCompletion();
            masterySkinDef.glowColor = Color.white;
            RavagerSkinCatalog.AddSkin(masterySkinDef);
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
                    if (damageReport.victim && damageReport.victimBody)
                    {
                        GrabTracker tracker = damageReport.victim.gameObject.GetComponent<GrabTracker>();
                        if (tracker)
                        {
                            ConsumeOrb orb = new ConsumeOrb();
                            orb.origin = damageReport.victim.transform.position;
                            orb.target = Util.FindBodyMainHurtBox(damageReport.attackerBody);
                            OrbManager.instance.AddOrb(orb);

                            if (Modules.Assets.bloodExplosionOverrides.ContainsKey(damageReport.victim.name))
                            {
                                EffectManager.SpawnEffect(Modules.Assets.bloodExplosionOverrides[damageReport.victim.name], new EffectData
                                {
                                    origin = damageReport.victim.transform.position,
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
                                            origin = damageReport.victim.transform.position,
                                            rotation = Quaternion.identity,
                                            color = surfaceDefProvider.surfaceDef.approximateColor
                                        }, true);
                                    }
                                    else
                                    {
                                        EffectManager.SpawnEffect(Modules.Assets.largeBloodExplosionEffect, new EffectData
                                        {
                                            origin = damageReport.victim.transform.position,
                                            rotation = Quaternion.identity,
                                            color = Color.white
                                        }, true);
                                    }
                                }
                                else
                                {
                                    EffectManager.SpawnEffect(Modules.Assets.largeBloodExplosionEffect, new EffectData
                                    {
                                        origin = damageReport.victim.transform.position,
                                        rotation = Quaternion.identity,
                                        color = Color.white
                                    }, true);
                                }
                            }

                            if (damageReport.victimBody.modelLocator && damageReport.victimBody.modelLocator.modelTransform)
                            {
                                GameObject.Destroy(damageReport.victimBody.modelLocator.modelTransform.gameObject);
                            }
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

                GameObject chargeRing = GameObject.Instantiate(Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("ChargeRing"));
                chargeRing.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                RectTransform rect = chargeRing.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.4f, 0.4f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(50f, 0f);
                rect.localPosition = new Vector3(65f, -75f, 0f);

                var p = chargeRing.transform.GetChild(0).gameObject.AddComponent<Content.Components.BloodGauge2>();
                p.targetHUD = hud;
                p.fillBar = p.GetComponent<Image>();

                // you'll be back someday
                /*if (!hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BloodGauge"))
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

                    rect = bloodGauge.GetComponent<RectTransform>();
                    rect.anchorMax = new Vector2(1f, 1f);
                    rect.anchoredPosition = new Vector2(0f, -20f);
                }*/
            }
        }
    }
}
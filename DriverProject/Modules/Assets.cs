using System.Reflection;
using R2API;
using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System.IO;
using RoR2.Audio;
using System.Collections.Generic;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using TMPro;

namespace RedGuyMod.Modules
{
    internal static class Assets
    {
        internal static AssetBundle mainAssetBundle;

        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;

        internal static List<EffectDef> effectDefs = new List<EffectDef>();
        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        public static GameObject jammedEffectPrefab;
        public static GameObject stunGrenadeModelPrefab;

        public static GameObject pistolAimCrosshairPrefab;

        public static Mesh pistolMesh;
        public static Mesh shotgunMesh;
        public static Mesh riotShotgunMesh;
        public static Mesh slugShotgunMesh;
        public static Mesh machineGunMesh;
        public static Mesh bazookaMesh;
        public static Mesh rocketLauncherMesh;
        public static Mesh sniperMesh;

        public static Material pistolMat;
        public static Material shotgunMat;
        public static Material riotShotgunMat;
        public static Material slugShotgunMat;
        public static Material machineGunMat;
        public static Material rocketLauncherMat;
        public static Material bazookaMat;
        public static Material sniperMat;

        public static Material knifeMat;

        public static GameObject shotgunShell;

        public static GameObject weaponPickup;
        public static GameObject weaponPickupLegendary;
        public static GameObject weaponPickupOld;

        public static GameObject weaponPickupEffect;

        internal static Texture pistolWeaponIcon;
        internal static Texture shotgunWeaponIcon;
        internal static Texture riotShotgunWeaponIcon;
        internal static Texture slugShotgunWeaponIcon;
        internal static Texture machineGunWeaponIcon;
        internal static Texture bazookaWeaponIcon;
        internal static Texture rocketLauncherWeaponIcon;
        internal static Texture sniperWeaponIcon;

        public static GameObject shotgunTracer;
        public static GameObject shotgunTracerCrit;

        internal static DriverWeaponDef pistolWeaponDef;
        internal static DriverWeaponDef shotgunWeaponDef;
        internal static DriverWeaponDef riotShotgunWeaponDef;
        internal static DriverWeaponDef slugShotgunWeaponDef;
        internal static DriverWeaponDef machineGunWeaponDef;
        internal static DriverWeaponDef heavyMachineGunWeaponDef;
        internal static DriverWeaponDef bazookaWeaponDef;
        internal static DriverWeaponDef rocketLauncherWeaponDef;
        internal static DriverWeaponDef sniperWeaponDef;

        internal static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DriverMod.robdriver"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("DriverMod.driver_bank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            //drainPunchChargeEffect = LoadEffect("DrainPunchChargeEffect", true);

            //punchSoundDef = CreateNetworkSoundEventDef("RegigigasPunchImpact");

            jammedEffectPrefab = CreateTextPopupEffect("DriverGunJammedEffect", "ROB_DRIVER_JAMMED_POPUP");

            pistolAimCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("DriverPistolAimCrosshair", false);

            GameObject chargeBar = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeBar"));
            chargeBar.transform.SetParent(pistolAimCrosshairPrefab.transform);

            RectTransform rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.075f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(0f, -60f, 0f);

            chargeBar.transform.GetChild(0).gameObject.AddComponent<Modules.Components.CrosshairChargeBar>().crosshairController = pistolAimCrosshairPrefab.GetComponent<RoR2.UI.CrosshairController>();


            pistolMesh = mainAssetBundle.LoadAsset<Mesh>("meshPistol");
            shotgunMesh = mainAssetBundle.LoadAsset<Mesh>("meshSuperShotgun");
            riotShotgunMesh = mainAssetBundle.LoadAsset<Mesh>("meshRiotShotgun");
            slugShotgunMesh = mainAssetBundle.LoadAsset<Mesh>("meshSlugShotgun");
            machineGunMesh = mainAssetBundle.LoadAsset<Mesh>("meshMachineGun");
            bazookaMesh = mainAssetBundle.LoadAsset<Mesh>("meshBazooka");
            rocketLauncherMesh = mainAssetBundle.LoadAsset<Mesh>("meshRocketLauncher");
            sniperMesh = mainAssetBundle.LoadAsset<Mesh>("meshSniperRifle");

            pistolMat = CreateMaterial("matPistol");
            shotgunMat = CreateMaterial("matShotgun");
            riotShotgunMat = CreateMaterial("matRiotShotgun");
            slugShotgunMat = CreateMaterial("matSlugShotgun");
            machineGunMat = CreateMaterial("matMachineGun");
            bazookaMat = CreateMaterial("matBazooka");
            rocketLauncherMat = CreateMaterial("matRocketLauncher");
            sniperMat = CreateMaterial("matSniperRifle");

            knifeMat = CreateMaterial("matKnife");

            shotgunShell = mainAssetBundle.LoadAsset<GameObject>("ShotgunShell");
            shotgunShell.GetComponentInChildren<MeshRenderer>().material = CreateMaterial("matShotgunShell");
            shotgunShell.AddComponent<Modules.Components.ShellController>();


            #region Normal weapon pickup
            weaponPickup = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/AmmoPack.prefab").WaitForCompletion().InstantiateClone("DriverWeaponPickup", true);

            weaponPickup.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 55f;
            weaponPickup.GetComponent<DestroyOnTimer>().duration = 60f;

            AmmoPickup ammoPickupComponent = weaponPickup.GetComponentInChildren<AmmoPickup>();
            Components.WeaponPickup weaponPickupComponent = ammoPickupComponent.gameObject.AddComponent<Components.WeaponPickup>();

            weaponPickupComponent.baseObject = ammoPickupComponent.baseObject;
            weaponPickupComponent.pickupEffect = ammoPickupComponent.pickupEffect;
            weaponPickupComponent.teamFilter = ammoPickupComponent.teamFilter;

            Material uncommonPickupMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Bandolier/matPickups.mat").WaitForCompletion());
            uncommonPickupMat.SetColor("_TintColor", new Color(0f, 80f / 255f, 0f, 1f));

            weaponPickup.GetComponentInChildren<MeshRenderer>().enabled = false;/*.materials = new Material[]
            {
                Assets.shotgunMat,
                uncommonPickupMat
            };*/

            GameObject pickupModel = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("WeaponPickup"));
            pickupModel.transform.parent = weaponPickup.transform.Find("Visuals");
            pickupModel.transform.localPosition = new Vector3(0f, -0.35f, 0f);
            pickupModel.transform.localRotation = Quaternion.identity;

            MeshRenderer pickupMesh = pickupModel.GetComponentInChildren<MeshRenderer>();
            /*pickupMesh.materials = new Material[]
            {
                CreateMaterial("matCrate1"),
                CreateMaterial("matCrate2")//,
                //uncommonPickupMat
            };*/
            pickupMesh.material = CreateMaterial("matBriefcase");

            GameObject textShit = GameObject.Instantiate(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BearProc"));
            MonoBehaviour.Destroy(textShit.GetComponent<EffectComponent>());
            textShit.transform.parent = pickupModel.transform;
            textShit.transform.localPosition = Vector3.zero;
            textShit.transform.localRotation = Quaternion.identity;

            textShit.GetComponent<DestroyOnTimer>().enabled = false;

            ObjectScaleCurve whatTheFuckIsThis = textShit.GetComponentInChildren<ObjectScaleCurve>();
            //whatTheFuckIsThis.enabled = false;
            //whatTheFuckIsThis.transform.localScale = Vector3.one * 2;
            //whatTheFuckIsThis.timeMax = 60f;
            Transform helpMe = whatTheFuckIsThis.transform;
            MonoBehaviour.DestroyImmediate(whatTheFuckIsThis);
            helpMe.transform.localScale = Vector3.one * 1.25f;

            MonoBehaviour.Destroy(ammoPickupComponent);
            MonoBehaviour.Destroy(weaponPickup.GetComponentInChildren<RoR2.GravitatePickup>());
            #endregion

            #region Legendary weapon pickup
            weaponPickupLegendary = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/AmmoPack.prefab").WaitForCompletion().InstantiateClone("DriverWeaponPickupLegendary", true);

            weaponPickupLegendary.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 110f;
            weaponPickupLegendary.GetComponent<DestroyOnTimer>().duration = 120f;

            AmmoPickup ammoPickupComponent2 = weaponPickupLegendary.GetComponentInChildren<AmmoPickup>();
            Components.WeaponPickup weaponPickupComponent2 = ammoPickupComponent2.gameObject.AddComponent<Components.WeaponPickup>();

            weaponPickupComponent2.baseObject = ammoPickupComponent2.baseObject;
            weaponPickupComponent2.pickupEffect = ammoPickupComponent2.pickupEffect;
            weaponPickupComponent2.teamFilter = ammoPickupComponent2.teamFilter;

            weaponPickupLegendary.GetComponentInChildren<MeshRenderer>().enabled = false;

            GameObject pickupModel2 = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("WeaponPickupLegendary"));
            pickupModel2.transform.parent = weaponPickupLegendary.transform.Find("Visuals");
            pickupModel2.transform.localPosition = new Vector3(0f, -0.35f, 0f);
            pickupModel2.transform.localRotation = Quaternion.identity;

            MeshRenderer pickupMesh2 = pickupModel2.GetComponentInChildren<MeshRenderer>();
            pickupMesh2.material = CreateMaterial("matBriefcaseGold");

            GameObject textShit2 = GameObject.Instantiate(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BearProc"));
            MonoBehaviour.Destroy(textShit2.GetComponent<EffectComponent>());
            textShit2.transform.parent = pickupModel2.transform;
            textShit2.transform.localPosition = Vector3.zero;
            textShit2.transform.localRotation = Quaternion.identity;

            textShit2.GetComponent<DestroyOnTimer>().enabled = false;

            ObjectScaleCurve whatTheFuckIsThis2 = textShit2.GetComponentInChildren<ObjectScaleCurve>();
            //whatTheFuckIsThis.enabled = false;
            //whatTheFuckIsThis.transform.localScale = Vector3.one * 2;
            //whatTheFuckIsThis.timeMax = 60f;
            Transform helpMe2 = whatTheFuckIsThis2.transform;
            MonoBehaviour.DestroyImmediate(whatTheFuckIsThis2);
            helpMe2.transform.localScale = Vector3.one * 1.25f;

            MonoBehaviour.Destroy(ammoPickupComponent2);
            MonoBehaviour.Destroy(weaponPickupLegendary.GetComponentInChildren<RoR2.GravitatePickup>());
            #endregion

            #region Old weapon pickup
            weaponPickupOld = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/AmmoPack.prefab").WaitForCompletion().InstantiateClone("DriverWeaponPickupOld", true);

            weaponPickupOld.GetComponent<BeginRapidlyActivatingAndDeactivating>().delayBeforeBeginningBlinking = 55f;
            weaponPickupOld.GetComponent<DestroyOnTimer>().duration = 60f;

            AmmoPickup ammoPickupComponent3 = weaponPickupOld.GetComponentInChildren<AmmoPickup>();
            Components.WeaponPickup weaponPickupComponent3 = ammoPickupComponent3.gameObject.AddComponent<Components.WeaponPickup>();

            weaponPickupComponent3.baseObject = ammoPickupComponent3.baseObject;
            weaponPickupComponent3.pickupEffect = ammoPickupComponent3.pickupEffect;
            weaponPickupComponent3.teamFilter = ammoPickupComponent3.teamFilter;

            weaponPickupOld.GetComponentInChildren<MeshRenderer>().enabled = false;

            GameObject pickupModel3 = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("WeaponPickupOld"));
            pickupModel3.transform.parent = weaponPickupOld.transform.Find("Visuals");
            pickupModel3.transform.localPosition = new Vector3(0f, -0.35f, 0f);
            pickupModel3.transform.localRotation = Quaternion.identity;

            MeshRenderer pickupMesh3 = pickupModel3.GetComponentInChildren<MeshRenderer>();
            pickupMesh3.materials = new Material[]
            {
                CreateMaterial("matCrate1"),
                CreateMaterial("matCrate2")
            };

             GameObject textShit3 = GameObject.Instantiate(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BearProc"));
            MonoBehaviour.Destroy(textShit3.GetComponent<EffectComponent>());
            textShit3.transform.parent = pickupModel3.transform;
            textShit3.transform.localPosition = Vector3.zero;
            textShit3.transform.localRotation = Quaternion.identity;

            textShit3.GetComponent<DestroyOnTimer>().enabled = false;

            ObjectScaleCurve whatTheFuckIsThis3 = textShit3.GetComponentInChildren<ObjectScaleCurve>();
            //whatTheFuckIsThis.enabled = false;
            //whatTheFuckIsThis.transform.localScale = Vector3.one * 2;
            //whatTheFuckIsThis.timeMax = 60f;
            Transform helpMe3 = whatTheFuckIsThis3.transform;
            MonoBehaviour.DestroyImmediate(whatTheFuckIsThis3);
            helpMe3.transform.localScale = Vector3.one * 1.25f;

            MonoBehaviour.Destroy(ammoPickupComponent3);
            MonoBehaviour.Destroy(weaponPickupOld.GetComponentInChildren<RoR2.GravitatePickup>());
            #endregion

            weaponPickupEffect = weaponPickupComponent.pickupEffect.InstantiateClone("RobDriverWeaponPickupEffect", true);
            weaponPickupEffect.AddComponent<NetworkIdentity>();
            AddNewEffectDef(weaponPickupEffect, "sfx_driver_pickup");


            weaponPickupComponent.pickupEffect = weaponPickupEffect;
            weaponPickupComponent2.pickupEffect = weaponPickupEffect;
            weaponPickupComponent3.pickupEffect = weaponPickupEffect;


            pistolWeaponIcon = mainAssetBundle.LoadAsset<Texture>("texPistolWeaponIcon");
            shotgunWeaponIcon = mainAssetBundle.LoadAsset<Texture>("texShotgunWeaponIcon");
            riotShotgunWeaponIcon = mainAssetBundle.LoadAsset<Texture>("texRiotShotgunWeaponIcon");
            slugShotgunWeaponIcon = mainAssetBundle.LoadAsset<Texture>("texSlugShotgunWeaponIcon");
            machineGunWeaponIcon = mainAssetBundle.LoadAsset<Texture>("texMachineGunWeaponIcon");
            bazookaWeaponIcon = mainAssetBundle.LoadAsset<Texture>("texBazookaWeaponIcon");
            rocketLauncherWeaponIcon = mainAssetBundle.LoadAsset<Texture>("texRocketLauncherWeaponIcon");
            sniperWeaponIcon = mainAssetBundle.LoadAsset<Texture>("texSniperRifleWeaponIcon");


            shotgunTracer = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("DriverShotgunTracer", true);

            if (!shotgunTracer.GetComponent<EffectComponent>()) shotgunTracer.AddComponent<EffectComponent>();
            if (!shotgunTracer.GetComponent<VFXAttributes>()) shotgunTracer.AddComponent<VFXAttributes>();
            if (!shotgunTracer.GetComponent<NetworkIdentity>()) shotgunTracer.AddComponent<NetworkIdentity>();

            Material bulletMat = null;

            foreach (LineRenderer i in shotgunTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    bulletMat = UnityEngine.Object.Instantiate<Material>(i.material);
                    bulletMat.SetColor("_TintColor", new Color(0.68f, 0.58f, 0.05f));
                    i.material = bulletMat;
                    i.startColor = new Color(0.68f, 0.58f, 0.05f);
                    i.endColor = new Color(0.68f, 0.58f, 0.05f);
                }
            }

            shotgunTracerCrit = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun").InstantiateClone("DriverShotgunTracerCritical", true);

            if (!shotgunTracerCrit.GetComponent<EffectComponent>()) shotgunTracerCrit.AddComponent<EffectComponent>();
            if (!shotgunTracerCrit.GetComponent<VFXAttributes>()) shotgunTracerCrit.AddComponent<VFXAttributes>();
            if (!shotgunTracerCrit.GetComponent<NetworkIdentity>()) shotgunTracerCrit.AddComponent<NetworkIdentity>();

            foreach (LineRenderer i in shotgunTracerCrit.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    Material material = UnityEngine.Object.Instantiate<Material>(i.material);
                    material.SetColor("_TintColor", Color.yellow);
                    i.material = material;
                    i.startColor = new Color(0.8f, 0.24f, 0f);
                    i.endColor = new Color(0.8f, 0.24f, 0f);
                }
            }

            AddNewEffectDef(shotgunTracer);
            AddNewEffectDef(shotgunTracerCrit);

            Modules.Config.InitROO(Assets.mainAssetBundle.LoadAsset<Sprite>("texDriverIcon"), "Literally me");

            // actually i have to run this in driver's script, so the skilldefs can be created first
            //InitWeaponDefs();
            // kinda jank kinda not impactful enough to care about changing
        }

        internal static void InitWeaponDefs()
        {
            // ignore this one, this is the default
            pistolWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
            {
                nameToken = "ROB_DRIVER_PISTOL_NAME",
                descriptionToken = "ROB_DRIVER_PISTOL_DESC",
                icon = Assets.pistolWeaponIcon,
                crosshairPrefab = Assets.LoadCrosshair("Standard"),
                tier = DriverWeaponTier.Common,
                baseDuration = 0f,
                primarySkillDef = null,
                secondarySkillDef = null,
                mesh = Assets.pistolMesh,
                material = Assets.pistolMat,
                animationSet = DriverWeaponDef.AnimationSet.Default,
            });
            DriverWeaponCatalog.AddWeapon(pistolWeaponDef);

            // example of creating a WeaponDef through code and adding it to the catalog for driver to obtain
            if (Modules.Config.shotgunEnabled.Value)
            {
                shotgunWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
                {
                    nameToken = "ROB_DRIVER_SHOTGUN_NAME",
                    descriptionToken = "ROB_DRIVER_SHOTGUN_DESC",
                    icon = Assets.shotgunWeaponIcon,
                    crosshairPrefab = Assets.LoadCrosshair("SMG"),
                    tier = DriverWeaponTier.Uncommon,
                    baseDuration = Config.shotgunDuration.Value,
                    primarySkillDef = Survivors.Driver.shotgunPrimarySkillDef,
                    secondarySkillDef = Survivors.Driver.shotgunSecondarySkillDef,
                    mesh = Assets.shotgunMesh,
                    material = Assets.shotgunMat,
                    animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                    calloutSoundString = "sfx_driver_callout_shotgun"
                });// now add it to the catalog here; catalog is necessary for networking
                DriverWeaponCatalog.AddWeapon(shotgunWeaponDef);
            }

            if (Modules.Config.riotShotgunEnabled.Value)
            {
                riotShotgunWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
                {
                    nameToken = "ROB_DRIVER_RIOT_SHOTGUN_NAME",
                    descriptionToken = "ROB_DRIVER_RIOT_SHOTGUN_DESC",
                    icon = Assets.riotShotgunWeaponIcon,
                    crosshairPrefab = Assets.LoadCrosshair("SMG"),
                    tier = DriverWeaponTier.Uncommon,
                    baseDuration = Config.riotShotgunDuration.Value,
                    primarySkillDef = Survivors.Driver.riotShotgunPrimarySkillDef,
                    secondarySkillDef = Survivors.Driver.riotShotgunSecondarySkillDef,
                    mesh = Assets.riotShotgunMesh,
                    material = Assets.riotShotgunMat,
                    animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                    calloutSoundString = "sfx_driver_callout_shotgun"
                });
                DriverWeaponCatalog.AddWeapon(riotShotgunWeaponDef);
            }

            if (Modules.Config.slugShotgunEnabled.Value)
            {
                slugShotgunWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
                {
                    nameToken = "ROB_DRIVER_SLUG_SHOTGUN_NAME",
                    descriptionToken = "ROB_DRIVER_SLUG_SHOTGUN_DESC",
                    icon = Assets.slugShotgunWeaponIcon,
                    crosshairPrefab = Assets.LoadCrosshair("SMG"),
                    tier = DriverWeaponTier.Uncommon,
                    baseDuration = Config.slugShotgunDuration.Value,
                    primarySkillDef = Survivors.Driver.slugShotgunPrimarySkillDef,
                    secondarySkillDef = Survivors.Driver.slugShotgunSecondarySkillDef,
                    mesh = Assets.slugShotgunMesh,
                    material = Assets.slugShotgunMat,
                    animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                    calloutSoundString = "sfx_driver_callout_shotgun"
                });
                DriverWeaponCatalog.AddWeapon(slugShotgunWeaponDef);
            }

            if (Modules.Config.machineGunEnabled.Value)
            {
                machineGunWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
                {
                    nameToken = "ROB_DRIVER_MACHINEGUN_NAME",
                    descriptionToken = "ROB_DRIVER_MACHINEGUN_DESC",
                    icon = Assets.machineGunWeaponIcon,
                    crosshairPrefab = Assets.LoadCrosshair("Standard"),
                    tier = DriverWeaponTier.Uncommon,
                    baseDuration = Config.machineGunDuration.Value,
                    primarySkillDef = Survivors.Driver.machineGunPrimarySkillDef,
                    secondarySkillDef = Survivors.Driver.machineGunSecondarySkillDef,
                    mesh = Assets.machineGunMesh,
                    material = Assets.machineGunMat,
                    animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                    calloutSoundString = "sfx_driver_callout_machine_gun"
                });
                DriverWeaponCatalog.AddWeapon(machineGunWeaponDef);
            }

            if (Modules.Config.heavyMachineGunEnabled.Value)
            {
                heavyMachineGunWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
                {
                    nameToken = "ROB_DRIVER_HEAVY_MACHINEGUN_NAME",
                    descriptionToken = "ROB_DRIVER_HEAVY_MACHINEGUN_DESC",
                    icon = Assets.machineGunWeaponIcon,
                    crosshairPrefab = Assets.LoadCrosshair("Standard"),
                    tier = DriverWeaponTier.Uncommon,
                    baseDuration = Config.heavyMachineGunDuration.Value,
                    primarySkillDef = Survivors.Driver.heavyMachineGunPrimarySkillDef,
                    secondarySkillDef = Survivors.Driver.heavyMachineGunSecondarySkillDef,
                    mesh = Assets.machineGunMesh,
                    material = Assets.machineGunMat,
                    animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                    calloutSoundString = "sfx_driver_callout_machine_gun"
                });
                DriverWeaponCatalog.AddWeapon(heavyMachineGunWeaponDef);
            }

            if (Modules.Config.heavyMachineGunEnabled.Value)
            {
                sniperWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
                {
                    nameToken = "ROB_DRIVER_SNIPER_NAME",
                    descriptionToken = "ROB_DRIVER_SNIPER_DESC",
                    icon = Assets.sniperWeaponIcon,
                    crosshairPrefab = Assets.LoadCrosshair("Standard"),
                    tier = DriverWeaponTier.Uncommon,
                    baseDuration = Config.heavyMachineGunDuration.Value,
                    primarySkillDef = Survivors.Driver.sniperPrimarySkillDef,
                    secondarySkillDef = Survivors.Driver.sniperSecondarySkillDef,
                    mesh = Assets.sniperMesh,
                    material = Assets.sniperMat,
                    animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                    calloutSoundString = ""
                });
                DriverWeaponCatalog.AddWeapon(sniperWeaponDef);
            }

            bazookaWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
            {
                nameToken = "ROB_DRIVER_BAZOOKA_NAME",
                descriptionToken = "ROB_DRIVER_BAZOOKA_DESC",
                icon = Assets.bazookaWeaponIcon,
                crosshairPrefab = Assets.LoadCrosshair("ToolbotGrenadeLauncher"),
                tier = DriverWeaponTier.Uncommon,
                baseDuration = Config.rocketLauncherDuration.Value,
                primarySkillDef = Survivors.Driver.bazookaPrimarySkillDef,
                secondarySkillDef = Survivors.Driver.bazookaSecondarySkillDef,
                mesh = Assets.bazookaMesh,
                material = Assets.bazookaMat,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                calloutSoundString = "sfx_driver_callout_rocket_launcher"
            });
            DriverWeaponCatalog.AddWeapon(bazookaWeaponDef);

            if (Modules.Config.rocketLauncherEnabled.Value)
            {
                rocketLauncherWeaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
                {
                    nameToken = "ROB_DRIVER_ROCKETLAUNCHER_NAME",
                    descriptionToken = "ROB_DRIVER_ROCKETLAUNCHER_DESC",
                    icon = Assets.rocketLauncherWeaponIcon,
                    crosshairPrefab = Assets.LoadCrosshair("ToolbotGrenadeLauncher"),
                    tier = DriverWeaponTier.Legendary,
                    baseDuration = Config.rocketLauncherDuration.Value,
                    primarySkillDef = Survivors.Driver.rocketLauncherPrimarySkillDef,
                    secondarySkillDef = Survivors.Driver.rocketLauncherSecondarySkillDef,
                    mesh = Assets.rocketLauncherMesh,
                    material = Assets.rocketLauncherMat,
                    animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                    calloutSoundString = "sfx_driver_callout_rocket_launcher"
                });
                DriverWeaponCatalog.AddWeapon(rocketLauncherWeaponDef);
            }
        }

        internal static GameObject CreateTextPopupEffect(string prefabName, string token, string soundName = "")
        {
            GameObject i = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BearProc").InstantiateClone(prefabName, true);

            i.GetComponent<EffectComponent>().soundName = soundName;
            if (!i.GetComponent<NetworkIdentity>()) i.AddComponent<NetworkIdentity>();

            i.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>().token = token;

            Assets.AddNewEffectDef(i);

            return i;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            foreach (Renderer i in objectToConvert.GetComponentsInChildren<Renderer>())
            {
                if (i)
                {
                    if (i.material)
                    {
                        i.material.shader = hotpoo;
                    }
                }
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        public static GameObject LoadSurvivorModel(string modelName) {
            GameObject model = mainAssetBundle.LoadAsset<GameObject>(modelName);
            if (model == null) {
                Log.Error("Trying to load a null model- check to see if the name in your code matches the name of the object in Unity");
                return null;
            }

            return PrefabAPI.InstantiateClone(model, model.name, false);
        }

        internal static Texture LoadCharacterIcon(string characterName)
        {
            return mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");
        }

        internal static GameObject LoadCrosshair(string crosshairName)
        {
            return Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");
        }

        private static GameObject LoadEffect(string resourceName)
        {
            return LoadEffect(resourceName, "", false);
        }

        private static GameObject LoadEffect(string resourceName, string soundName)
        {
            return LoadEffect(resourceName, soundName, false);
        }

        private static GameObject LoadEffect(string resourceName, bool parentToTransform)
        {
            return LoadEffect(resourceName, "", parentToTransform);
        }

        private static GameObject LoadEffect(string resourceName, string soundName, bool parentToTransform)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;
            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.effectIndex = EffectIndex.Invalid;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        internal static void AddNewEffectDef(GameObject effectPrefab)
        {
            AddNewEffectDef(effectPrefab, "");
        }

        internal static void AddNewEffectDef(GameObject effectPrefab, string soundName)
        {
            EffectDef newEffectDef = new EffectDef();
            newEffectDef.prefab = effectPrefab;
            newEffectDef.prefabEffectComponent = effectPrefab.GetComponent<EffectComponent>();
            newEffectDef.prefabName = effectPrefab.name;
            newEffectDef.prefabVfxAttributes = effectPrefab.GetComponent<VFXAttributes>();
            newEffectDef.spawnSoundEventName = soundName;

            effectDefs.Add(newEffectDef);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor, float normalStrength)
        {
            if (!commandoMat) commandoMat = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

            Material mat = UnityEngine.Object.Instantiate<Material>(commandoMat);
            Material tempMat = Assets.mainAssetBundle.LoadAsset<Material>(materialName);

            if (!tempMat) return commandoMat;

            mat.name = materialName;
            mat.SetColor("_Color", tempMat.GetColor("_Color"));
            mat.SetTexture("_MainTex", tempMat.GetTexture("_MainTex"));
            mat.SetColor("_EmColor", emissionColor);
            mat.SetFloat("_EmPower", emission);
            mat.SetTexture("_EmTex", tempMat.GetTexture("_EmissionMap"));
            mat.SetFloat("_NormalStrength", normalStrength);

            return mat;
        }

        public static Material CreateMaterial(string materialName)
        {
            return Assets.CreateMaterial(materialName, 0f);
        }

        public static Material CreateMaterial(string materialName, float emission)
        {
            return Assets.CreateMaterial(materialName, emission, Color.black);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}
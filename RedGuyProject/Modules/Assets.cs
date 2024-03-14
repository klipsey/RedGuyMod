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
using UnityEngine.Rendering.PostProcessing;
using ThreeEyedGames;
using RoR2.UI;

namespace RedGuyMod.Modules
{
    internal static class Assets
    {
        internal static AssetBundle mainAssetBundle;

        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;

        internal static List<EffectDef> effectDefs = new List<EffectDef>();
        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        internal static Material bloodOverlayMat;

        internal static GameObject beamCrosshair;
        internal static GameObject clingCrosshair;
        internal static GameObject clingBeamCrosshair;

        internal static GameObject swingEffect;
        internal static GameObject bigSwingEffect;
        internal static GameObject slashImpactEffect;

        internal static GameObject swingEffectMastery;
        internal static GameObject bigSwingEffectMastery;
        internal static GameObject slashImpactEffectMastery;

        internal static GameObject swingEffectVoid;
        internal static GameObject bigSwingEffectVoid;

        internal static GameObject leapEffect;
        internal static GameObject leapEffectMastery;
        internal static GameObject leapEffectVoid;

        internal static GameObject slamImpactEffect;
        internal static GameObject bloodBombEffect;
        internal static GameObject cssEffect;

        internal static GameObject punchImpactEffect;

        internal static GameObject genericBloodExplosionEffect;
        internal static GameObject largeBloodExplosionEffect;
        internal static GameObject gupBloodExplosionEffect;
        internal static GameObject golemBloodExplosionEffect;
        internal static GameObject lunarGolemBloodExplosionEffect;
        internal static GameObject insectBloodExplosionEffect;
        internal static GameObject clayBloodExplosionEffect;
        internal static GameObject fireBloodExplosionEffect;
        internal static GameObject greenFireBloodExplosionEffect;
        internal static GameObject purpleFireBloodExplosionEffect;
        internal static GameObject healingBloodExplosionEffect;

        internal static GameObject drainTextEffect;

        internal static GameObject consumeOrb;
        internal static GameObject consumeOrbMastery;
        internal static GameObject consumeOrbVoid;

        internal static GameObject beamChargeEffect;

        internal static GameObject groundDragEffect;
        internal static GameObject groundImpactEffect;
        internal static GameObject heavyGroundImpactEffect;

        internal static NetworkSoundEventDef slashSoundEvent;
        internal static NetworkSoundEventDef bigSlashSoundEvent;
        internal static NetworkSoundEventDef explosionSoundEvent;

        internal static Dictionary<string, GameObject> bloodExplosionOverrides = new Dictionary<string, GameObject>();
        internal static Dictionary<string, string> bodyPunchSounds = new Dictionary<string, string>();
        internal static Dictionary<string, Vector3> bodyGrabOffsets = new Dictionary<string, Vector3>();
        internal static List<string> grabBlacklist = new List<string>();

        internal static void PopulateAssets()
        {
            if (mainAssetBundle == null)
            {
                using (var assetStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("RedGuyMod.robravager"))
                {
                    mainAssetBundle = AssetBundle.LoadFromStream(assetStream);
                }
            }

            using (Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("RedGuyMod.ravager_bank.bnk"))
            {
                byte[] array = new byte[manifestResourceStream2.Length];
                manifestResourceStream2.Read(array, 0, array.Length);
                SoundAPI.SoundBanks.Add(array);
            }

            Modules.Config.InitROO(Assets.mainAssetBundle.LoadAsset<Sprite>("texRavagerIcon"), "Ravager mod");

            bloodOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidCrabMatterOverlay.mat").WaitForCompletion());
            bloodOverlayMat.SetColor("_TintColor", Color.red);

            #region Beam Crosshair
            beamCrosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2Crosshair.prefab").WaitForCompletion().InstantiateClone("RavagerBeamCrosshair", false);

            beamCrosshair.GetComponent<CrosshairController>().skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];

            beamCrosshair.transform.Find("Center, Available").gameObject.SetActive(false);
            beamCrosshair.transform.Find("GameObject").gameObject.SetActive(false);

            GameObject chargeBar = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeBar"));
            chargeBar.transform.SetParent(beamCrosshair.transform);

            RectTransform rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.075f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(0f, -60f, 0f);

            chargeBar.transform.GetChild(0).gameObject.AddComponent<Content.Components.CrosshairChargeBar>().crosshairController = beamCrosshair.GetComponent<RoR2.UI.CrosshairController>();
            #endregion

            #region Cling Crosshair
            clingCrosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/SimpleDotCrosshair.prefab").WaitForCompletion().InstantiateClone("RavagerClingCrosshair", false);

            chargeBar = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeBar"));
            chargeBar.transform.SetParent(clingCrosshair.transform);

            rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.075f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(0f, 60f, 0f);

            chargeBar.transform.GetChild(0).gameObject.AddComponent<Content.Components.CrosshairClingBar>().crosshairController = clingCrosshair.GetComponent<RoR2.UI.CrosshairController>();
            #endregion

            #region Cling Beam Crosshair
            clingBeamCrosshair = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2Crosshair.prefab").WaitForCompletion().InstantiateClone("RavagerClingBeamCrosshair", false);

            clingBeamCrosshair.GetComponent<CrosshairController>().skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];

            clingBeamCrosshair.transform.Find("Center, Available").gameObject.SetActive(false);
            clingBeamCrosshair.transform.Find("GameObject").gameObject.SetActive(false);

            chargeBar = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeBar"));
            chargeBar.transform.SetParent(clingBeamCrosshair.transform);

            rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.075f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(0f, -60f, 0f);

            chargeBar.transform.GetChild(0).gameObject.AddComponent<Content.Components.CrosshairChargeBar>().crosshairController = clingBeamCrosshair.GetComponent<RoR2.UI.CrosshairController>();

            chargeBar = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeBar"));
            chargeBar.transform.SetParent(clingBeamCrosshair.transform);

            rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.075f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(0f, 60f, 0f);

            chargeBar.transform.GetChild(0).gameObject.AddComponent<Content.Components.CrosshairClingBar>().crosshairController = clingBeamCrosshair.GetComponent<RoR2.UI.CrosshairController>();
            #endregion

            swingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("RavagerSwordSwing");
            swingEffect.transform.GetChild(0).gameObject.SetActive(false);
            swingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();

            swingEffectVoid = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("RavagerSwordSwingVoid");
            swingEffectVoid.transform.GetChild(0).gameObject.SetActive(false);
            swingEffectVoid.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorMeleeSlash.mat").WaitForCompletion();

            swingEffectMastery = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("RavagerSwordSwingMastery");
            swingEffectMastery.transform.GetChild(0).gameObject.SetActive(false);
            swingEffectMastery.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion();

            bigSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("RavagerBigSwordSwing");
            bigSwingEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            var sex = bigSwingEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            bigSwingEffect.transform.GetChild(0).localScale = Vector3.one * 2f;
            MainPlugin.Destroy(bigSwingEffect.GetComponent<EffectComponent>());

            bigSwingEffectVoid = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("RavagerBigSwordSwingVoid");
            bigSwingEffectVoid.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorMeleeSlash.mat").WaitForCompletion();
            sex = bigSwingEffectVoid.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            bigSwingEffectVoid.transform.GetChild(0).localScale = Vector3.one * 2f;
            MainPlugin.Destroy(bigSwingEffectVoid.GetComponent<EffectComponent>());

            bigSwingEffectMastery = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("RavagerBigSwordSwingMastery");
            bigSwingEffectMastery.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion();
            sex = bigSwingEffectMastery.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            bigSwingEffectMastery.transform.GetChild(0).localScale = Vector3.one * 2f;
            MainPlugin.Destroy(bigSwingEffectMastery.GetComponent<EffectComponent>());

            slamImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleGuardGroundSlam.prefab").WaitForCompletion().InstantiateClone("RavagerSlamImpact", false);
            AddNewEffectDef(slamImpactEffect);

            bloodBombEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("RavagerBloodBomb", false);

            Material bloodMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
            bloodBombEffect.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodBombEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodBombEffect.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodBombEffect.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodBombEffect.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();
            bloodBombEffect.GetComponentInChildren<Light>().gameObject.SetActive(false);

            bloodBombEffect.GetComponentInChildren<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalGold.asset").WaitForCompletion();

            AddNewEffectDef(bloodBombEffect);

            cssEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("RavagerMenuEffect", false);

            cssEffect.transform.localScale *= 0.05f;

            cssEffect.transform.Find("Particles/LongLifeNoiseTrails").localScale *= 0.25f;
            cssEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").localScale *= 0.25f;
            cssEffect.transform.Find("Particles/Flash, White").localScale *= 0.25f;
            cssEffect.transform.Find("Particles/Flash, Red").localScale *= 0.25f;
            cssEffect.transform.Find("Particles/Distortion").localScale *= 0.25f;
            cssEffect.transform.Find("Particles/Dash").localScale *= 0.25f;
            cssEffect.transform.Find("Particles/Dash, Bright").localScale *= 0.25f;
            cssEffect.transform.Find("Particles/DashRings").localScale *= 0.25f;
            cssEffect.transform.Find("Particles/Sphere").localScale *= 0.25f;

            foreach (ShakeEmitter i in cssEffect.GetComponentsInChildren<ShakeEmitter>())
            {
                if (i)
                {
                    i.wave.amplitude *= 0.15f;
                }
            }

            AddNewEffectDef(cssEffect);

            //AddNewEffectDef(swingEffect);

            //
            slashImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("RavagerSwordImpact", false);
            slashImpactEffect.GetComponent<OmniEffect>().enabled = false;

            Material hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", Color.red);

            slashImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = hitsparkMat;

            slashImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            slashImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();

            Material slashMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            slashMat.SetColor("_TintColor", Color.red);

            slashImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = slashMat;

            slashImpactEffect.transform.GetChild(4).localScale = Vector3.one * 3f;
            slashImpactEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();

            slashImpactEffect.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();
            slashImpactEffect.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark2Void.mat").WaitForCompletion();

            slashImpactEffect.transform.GetChild(1).localScale = Vector3.one * 1.5f;

            slashImpactEffect.transform.GetChild(1).gameObject.SetActive(true);
            slashImpactEffect.transform.GetChild(2).gameObject.SetActive(true);
            slashImpactEffect.transform.GetChild(3).gameObject.SetActive(true);
            slashImpactEffect.transform.GetChild(4).gameObject.SetActive(true);
            slashImpactEffect.transform.GetChild(5).gameObject.SetActive(true);
            slashImpactEffect.transform.GetChild(6).gameObject.SetActive(true);
            slashImpactEffect.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);

            slashImpactEffect.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);

            slashImpactEffect.transform.localScale = Vector3.one * 1.5f;

            AddNewEffectDef(slashImpactEffect);
            //

            //
            slashImpactEffectMastery = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("RavagerSwordImpactMastery", false);
            slashImpactEffectMastery.GetComponent<OmniEffect>().enabled = false;

            hitsparkMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            hitsparkMat.SetColor("_TintColor", Color.white);

            slashImpactEffectMastery.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = hitsparkMat;

            slashImpactEffectMastery.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            slashImpactEffectMastery.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matOmniRing2Huntress.mat").WaitForCompletion();

            slashMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRadialSlash1Generic.mat").WaitForCompletion());
            //slashMat.SetColor("_TintColor", Color.white);

            slashImpactEffectMastery.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = slashMat;

            slashImpactEffectMastery.transform.GetChild(4).localScale = Vector3.one * 3f;
            slashImpactEffectMastery.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();

            slashImpactEffectMastery.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarWisp/matOmniHitspark1LunarWisp.mat").WaitForCompletion();
            slashImpactEffectMastery.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniHitspark2Generic.mat").WaitForCompletion();

            slashImpactEffectMastery.transform.GetChild(1).localScale = Vector3.one * 1.5f;

            slashImpactEffectMastery.transform.GetChild(1).gameObject.SetActive(true);
            slashImpactEffectMastery.transform.GetChild(2).gameObject.SetActive(true);
            slashImpactEffectMastery.transform.GetChild(3).gameObject.SetActive(true);
            slashImpactEffectMastery.transform.GetChild(4).gameObject.SetActive(true);
            slashImpactEffectMastery.transform.GetChild(5).gameObject.SetActive(true);
            slashImpactEffectMastery.transform.GetChild(6).gameObject.SetActive(true);
            slashImpactEffectMastery.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);

            slashImpactEffectMastery.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);

            slashImpactEffectMastery.transform.localScale = Vector3.one * 1.5f;

            AddNewEffectDef(slashImpactEffectMastery);
            //

            leapEffect = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BrotherDashEffect"), "RavagerDashEffect", true);
            leapEffect.AddComponent<NetworkIdentity>();
            leapEffect.GetComponent<EffectComponent>().applyScale = true;
            leapEffect.transform.localScale *= 0.35f;

            leapEffect.transform.Find("Dash").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            leapEffect.transform.Find("Dash").transform.localPosition = new Vector3(0f, 0f, -8f);
            leapEffect.transform.Find("Donut").transform.localScale = Vector3.one * 0.25f;
            leapEffect.transform.Find("Donut").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpBossPortal.mat").WaitForCompletion();
            leapEffect.transform.Find("Donut, Distortion").transform.localScale = Vector3.one * 0.25f;
            leapEffect.GetComponentInChildren<Light>().color = Color.red;

            AddNewEffectDef(leapEffect);

            leapEffectMastery = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BrotherDashEffect"), "RavagerDashEffectMastery", true);
            leapEffectMastery.AddComponent<NetworkIdentity>();
            leapEffectMastery.GetComponent<EffectComponent>().applyScale = true;
            leapEffectMastery.transform.localScale *= 0.35f;

            leapEffectMastery.transform.Find("Dash").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwipe.mat").WaitForCompletion();
            leapEffectMastery.transform.Find("Dash").transform.localPosition = new Vector3(0f, 0f, -8f);
            leapEffectMastery.transform.Find("Donut").transform.localScale = Vector3.one * 0.25f;
            leapEffectMastery.transform.Find("Donut").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandparentTeleportOutBoom.mat").WaitForCompletion();
            leapEffectMastery.transform.Find("Donut, Distortion").transform.localScale = Vector3.one * 0.25f;
            leapEffectMastery.GetComponentInChildren<Light>().color = Color.white;

            AddNewEffectDef(leapEffectMastery);

            leapEffectVoid = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BrotherDashEffect"), "RavagerDashEffectVoid", true);
            leapEffectVoid.AddComponent<NetworkIdentity>();
            leapEffectVoid.GetComponent<EffectComponent>().applyScale = true;
            leapEffectVoid.transform.localScale *= 0.35f;

            leapEffectVoid.transform.Find("Dash").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorMeleeSlash.mat").WaitForCompletion();
            leapEffectVoid.transform.Find("Dash").transform.localPosition = new Vector3(0f, 0f, -8f);
            leapEffectVoid.transform.Find("Donut").transform.localScale = Vector3.one * 0.25f;
            leapEffectVoid.transform.Find("Donut").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/DeepVoidPortal/matDeepVoidPortalOpaque.mat").WaitForCompletion();
            leapEffectVoid.transform.Find("Donut, Distortion").transform.localScale = Vector3.one * 0.25f;
            leapEffectVoid.GetComponentInChildren<Light>().color = new Color(157f / 255f, 42f / 255f, 179 / 255f);

            AddNewEffectDef(leapEffectVoid);

            genericBloodExplosionEffect = CreateBloodExplosionEffect("RavagerGenericBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodGeneric.mat").WaitForCompletion());
            largeBloodExplosionEffect = CreateBloodExplosionEffect("RavagerLargeBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion(), 3f);
            gupBloodExplosionEffect = CreateBloodExplosionEffect("RavagerGupBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Gup/matGupBlood.mat").WaitForCompletion());
            golemBloodExplosionEffect = CreateBloodExplosionEffect("RavagerGolemBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/Titan/matTitanLaserGlob.mat").WaitForCompletion());
            lunarGolemBloodExplosionEffect = CreateBloodExplosionEffect("RavagerLunarGolemBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolemExplosion.mat").WaitForCompletion());
            insectBloodExplosionEffect = CreateBloodExplosionEffect("RavagerInsectBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/DLC1/AcidLarva/matAcidLarvaBlood.mat").WaitForCompletion());
            clayBloodExplosionEffect = CreateBloodExplosionEffect("RavagerClayBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodClayLarge.mat").WaitForCompletion());
            fireBloodExplosionEffect = CreateBloodExplosionEffect("RavagerFireBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matFirePillarParticle.mat").WaitForCompletion());
            greenFireBloodExplosionEffect = CreateBloodExplosionEffect("RavagerGreenFireBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/GreaterWisp/matOmniExplosion1GreaterWisp.mat").WaitForCompletion());
            purpleFireBloodExplosionEffect = CreateBloodExplosionEffect("RavagerPurpleFireBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Junk/AncientWisp/matAncientWillowispSpiral.mat").WaitForCompletion());
            healingBloodExplosionEffect = CreateBloodExplosionEffect("RavagerHealingBloodExplosion", Addressables.LoadAssetAsync<Material>("RoR2/Base/Beetle/matBeetleQueenAcidFizz.mat").WaitForCompletion());

            slashSoundEvent = CreateNetworkSoundEventDef("sfx_ravager_slash");
            bigSlashSoundEvent = CreateNetworkSoundEventDef("sfx_ravager_bigslash");
            explosionSoundEvent = CreateNetworkSoundEventDef("sfx_ravager_impact_ground");

            punchImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/OmniImpactVFXLoader.prefab").WaitForCompletion().InstantiateClone("RavagerPunchImpact", true);
            punchImpactEffect.GetComponent<EffectComponent>().applyScale = true;

            punchImpactEffect.transform.Find("Flash, Hard").gameObject.SetActive(false);
            punchImpactEffect.transform.Find("Scaled Hitspark 1 (Random Color)").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();
            punchImpactEffect.transform.Find("Scaled Hitspark 3 (Random Color)").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/KillEliteFrenzy/matOmniHitspark3Frenzy.mat").WaitForCompletion();
            punchImpactEffect.transform.Find("ScaledSmokeRing, Mesh").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/FireBallDash/matDustOpaque.mat").WaitForCompletion();

            punchImpactEffect.transform.Find("Impact Shockwave").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniRing1Void.mat").WaitForCompletion();

            GameObject fistEffect = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("PunchEffect"));
            fistEffect.transform.parent = punchImpactEffect.transform;
            fistEffect.transform.localPosition = Vector3.zero;
            fistEffect.transform.localRotation = Quaternion.identity;
            fistEffect.GetComponentInChildren<ParticleSystemRenderer>().material = CreateMaterial("matBody", 5f);

            AddNewEffectDef(punchImpactEffect);

            groundImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/PodGroundImpact.prefab").WaitForCompletion().InstantiateClone("RavagerGroundImpact", true);

            groundImpactEffect.transform.Find("Particles/Flash").transform.localScale = Vector3.one * 0.5f;
            groundImpactEffect.transform.Find("Particles/Dust").transform.localScale = Vector3.one * 0.25f;
            groundImpactEffect.transform.Find("Particles/Dust, Directional").transform.localScale = new Vector3(0.5f, 0.3f, 0.5f);
            groundImpactEffect.transform.Find("Particles/Point Light").gameObject.SetActive(false);

            AddNewEffectDef(groundImpactEffect);

            heavyGroundImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/PodGroundImpact.prefab").WaitForCompletion().InstantiateClone("RavagerHeavyGroundImpact", true);

            heavyGroundImpactEffect.transform.Find("Particles/Flash").transform.localScale = Vector3.one * 0.75f;
            heavyGroundImpactEffect.transform.Find("Particles/Dust").transform.localScale = Vector3.one * 0.5f;
            heavyGroundImpactEffect.transform.Find("Particles/Dust, Directional").transform.localScale = new Vector3(0.75f, 0.6f, 0.75f);
            heavyGroundImpactEffect.transform.Find("Particles/Point Light").gameObject.SetActive(false);

            AddNewEffectDef(heavyGroundImpactEffect);

            CreateOrb();

            bloodExplosionOverrides = new Dictionary<string, GameObject>()
            {
                {"GolemBody(Clone)", golemBloodExplosionEffect },
                {"TitanBody(Clone)", golemBloodExplosionEffect },
                {"GupBody(Clone)", gupBloodExplosionEffect },
                {"GeepBody(Clone)", gupBloodExplosionEffect },
                {"GipBody(Clone)", gupBloodExplosionEffect },
                {"ClayBruiserBody(Clone)", clayBloodExplosionEffect },
                {"ClayGrenadierBody(Clone)", clayBloodExplosionEffect },
                {"MoffeinClayManBody(Clone)", clayBloodExplosionEffect },
                {"AcidLarvaBody(Clone)", insectBloodExplosionEffect },
                {"LunarConstructBody(Clone)", lunarGolemBloodExplosionEffect },
                {"LunarWispBody(Clone)", lunarGolemBloodExplosionEffect },
                {"LunarGolemBody(Clone)", lunarGolemBloodExplosionEffect },
                {"LunarExploderBody(Clone)", lunarGolemBloodExplosionEffect },
                {"BellBody(Clone)", golemBloodExplosionEffect },
                {"BodyBrassMonolith(Clone)", golemBloodExplosionEffect },
                {"WispBody(Clone)", fireBloodExplosionEffect },
                {"GreaterWispBody(Clone)", greenFireBloodExplosionEffect },
                {"MoffeinAncientWispBody(Clone)", purpleFireBloodExplosionEffect },
                {"RoboBallMiniBody(Clone)", golemBloodExplosionEffect },
                {"RoboBallBossBody(Clone)", golemBloodExplosionEffect },
                {"SuperRoboBallBossBody(Clone)", golemBloodExplosionEffect },
                {"NewtBody(Clone)", lunarGolemBloodExplosionEffect },
                {"AffixEarthHealerBody(Clone)", healingBloodExplosionEffect }
            };

            bodyPunchSounds = new Dictionary<string, string>()
            {
                {"GolemBody(Clone)", "sfx_ravager_punch_rock" },
                {"TitanBody(Clone)", "sfx_ravager_punch_rock" },
                {"TitanGoldBody(Clone)", "sfx_ravager_punch_rock" },
                {"LunarConstructBody(Clone)", "sfx_ravager_punch_rock" },
                {"LunarWispBody(Clone)", "sfx_ravager_punch_rock" },
                {"LunarGolemBody(Clone)", "sfx_ravager_punch_rock" },
                {"LunarExploderBody(Clone)", "sfx_ravager_punch_rock" },
                {"RoboBallMiniBody(Clone)", "sfx_ravager_punch_metal" },
                {"RoboBallBossBody(Clone)", "sfx_ravager_punch_metal" },
                {"SuperRoboBallBossBody(Clone)", "sfx_ravager_punch_metal" },
                {"BellBody(Clone)", "sfx_ravager_punch_metal" },
                {"BodyBrassMonolith(Clone)", "sfx_ravager_punch_metal" },
                {"BeetleBody(Clone)", "sfx_ravager_punch_bug" },
                {"BeetleGuardBody(Clone)", "sfx_ravager_punch_bug" },
                {"BeetleGuardAllyBody(Clone)", "sfx_ravager_punch_bug" },
                {"BeetleQueen2Body(Clone)", "sfx_ravager_punch_bug" },
                {"AcidLarvaBody(Clone)", "sfx_ravager_punch_bug" },
                {"GupBody(Clone)", "sfx_ravager_punch_goo" },
                {"GeepBody(Clone)", "sfx_ravager_punch_goo_small" },
                {"GipBody(Clone)", "sfx_ravager_punch_goo_small" },
                {"JellyfishBody(Clone)", "sfx_ravager_punch_jelly" },
                {"VagrantBody(Clone)", "sfx_ravager_punch_jelly" },
            };

            bodyGrabOffsets = new Dictionary<string, Vector3>()
            {
                {"BeetleBody(Clone)", new Vector3(0f, 0f, 0f)},
                {"LemurianBody(Clone)", new Vector3(0f, -0.2f, -0.4f)},
                {"LemurianBruiserBody(Clone)", new Vector3(-0.5f, -1.5f, -1.8f)},
                {"BisonBody(Clone)", new Vector3(-0.4f, 0f, -0.9f)},
                {"ClayBruiserBody(Clone)", new Vector3(-1.5f, -0.3f, -0.9f)},
                {"HermitCrabBody(Clone)", new Vector3(-0.5f, 0f, -0.25f)},
                {"MiniMushroomBody(Clone)", new Vector3(-0.5f, 0f, 0f)},
                {"GolemBody(Clone)", new Vector3(-0.8f, -0.3f, -1f)},
                {"ImpBody(Clone)", new Vector3(-0.8f, 0f, 0.1f)},
                {"ParentBody(Clone)", new Vector3(0f, 0f, -15f)},
                {"TitanBody(Clone)", new Vector3(0f, 0f, 0f)},
                {"TitanGoldBody(Clone)", new Vector3(0f, 0f, 0f)},
                {"ImpBossBody(Clone)", new Vector3(0f, 0f, 0f)},
                {"AcidLarvaBody(Clone)", new Vector3(-0.45f, -0.2f, -0.3f)},
                {"ClayGrenadierBody(Clone)", new Vector3(0.6f, -0.3f, -1.2f)},
            };

            grabBlacklist = new List<string>
            {
                "BrotherBody(Clone)",
                "BrotherHurtBody(Clone)",
                "BodyBrassMonolith(Clone)"
            };

            drainTextEffect = CreateTextPopupEffect("RavagerDrainTextEffect", "BLOOD RUSH!");
            drainTextEffect.transform.localScale *= 2.5f;
            drainTextEffect.GetComponentInChildren<TMP_Text>().color = Color.red;

            groundDragEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectileghosts/sunderghost"), "RavagerGroundDrag");
            MainPlugin.Destroy(groundDragEffect.GetComponent<ProjectileGhostController>());

            beamChargeEffect = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorChargeMegaBlaster.prefab").WaitForCompletion(), "RavagerBeamCharge");
            MainPlugin.Destroy(beamChargeEffect.GetComponent<ObjectScaleCurve>());
            //
            beamChargeEffect.transform.Find("Base").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();
            beamChargeEffect.transform.Find("OrbCore").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabJointBrokenSphere.mat").WaitForCompletion();
            beamChargeEffect.transform.Find("Sparks, Misc").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterCoreCorrupted.mat").WaitForCompletion();
            beamChargeEffect.transform.Find("Sparks, In").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();

            beamChargeEffect.transform.Find("Base (1)").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorLightningCorrupted.mat").WaitForCompletion();
            beamChargeEffect.transform.Find("Base (1)").localScale = Vector3.one * 1.5f;

            GameObject shotgunTracer = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoShotgun");

            Material redTrailMat = null;

            foreach (LineRenderer i in shotgunTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i)
                {
                    redTrailMat = UnityEngine.Object.Instantiate<Material>(i.material);
                    redTrailMat.SetColor("_TintColor", Color.red);
                }
            }

            GameObject beamSphereEffect = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeSphere"));
            beamSphereEffect.transform.parent = beamChargeEffect.transform;
            beamSphereEffect.transform.localPosition = Vector3.zero;
            beamSphereEffect.transform.localRotation = Quaternion.identity;
            beamSphereEffect.transform.localScale = Vector3.one * 0.7f;
            beamSphereEffect.GetComponent<MeshRenderer>().material = CreateMaterial("matChargeSphere", 15f, Color.red);
            beamSphereEffect.GetComponentInChildren<ParticleSystemRenderer>().trailMaterial = redTrailMat;
        }

        private static GameObject CreateBloodExplosionEffect(string effectName, Material bloodMat, float scale = 1f)
        {
            GameObject newEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/BrotherSlamImpact.prefab").WaitForCompletion().InstantiateClone(effectName, true);

            newEffect.transform.Find("Spikes, Small").gameObject.SetActive(false);

            newEffect.transform.Find("PP").gameObject.SetActive(false);//.GetComponent<PostProcessVolume>().sharedProfile = Addressables.LoadAssetAsync<PostProcessProfile>("RoR2/Base/title/ppLocalMagmaWorm.asset").WaitForCompletion();
            newEffect.transform.Find("Point light").gameObject.SetActive(false);//.GetComponent<Light>().color = Color.yellow;
            newEffect.transform.Find("Flash Lines").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOpaqueDustLargeDirectional.mat").WaitForCompletion();

            newEffect.transform.GetChild(3).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.GetChild(6).GetComponent<ParticleSystemRenderer>().material = bloodMat;
            newEffect.transform.Find("Fire").GetComponent<ParticleSystemRenderer>().material = bloodMat;

            var sex = newEffect.transform.Find("Fire").GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.5f;
            sex = newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.3f;
            sex = newEffect.transform.GetChild(6).GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.4f;

            newEffect.transform.Find("Physics").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/MagmaWorm/matFracturedGround.mat").WaitForCompletion();

            newEffect.transform.Find("Decal").GetComponent<Decal>().Material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDecal.mat").WaitForCompletion();
            newEffect.transform.Find("Decal").GetComponent<AnimateShaderAlpha>().timeMax = 10f;

            newEffect.transform.Find("FoamSplash").gameObject.SetActive(false);
            newEffect.transform.Find("FoamBilllboard").gameObject.SetActive(false);
            newEffect.transform.Find("Dust").gameObject.SetActive(false);
            newEffect.transform.Find("Dust, Directional").gameObject.SetActive(false);

            newEffect.transform.localScale = Vector3.one * scale;

            AddNewEffectDef(newEffect, "sfx_ravager_gore");

            ParticleSystemColorFromEffectData fuck = newEffect.AddComponent<ParticleSystemColorFromEffectData>();
            fuck.particleSystems = new ParticleSystem[]
            {
                newEffect.transform.Find("Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.Find("Flash Lines, Fire").GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(6).GetComponent<ParticleSystem>(),
                newEffect.transform.GetChild(3).GetComponent<ParticleSystem>()
            };
            fuck.effectComponent = newEffect.GetComponent<EffectComponent>();

            return newEffect;
        }

        private static void CreateOrb()
        {
            consumeOrb = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "RavagerConsumeOrbEffect", true);
            if (!consumeOrb.GetComponent<NetworkIdentity>()) consumeOrb.AddComponent<NetworkIdentity>();

            TrailRenderer trail = consumeOrb.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>();
            trail.widthMultiplier = 0.35f;
            trail.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();

            consumeOrb.transform.Find("VFX").Find("Core").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
            consumeOrb.transform.Find("VFX").localScale = Vector3.one * 0.5f;

            consumeOrb.transform.Find("VFX").Find("Core").localScale = Vector3.one * 4.5f;

            consumeOrb.transform.Find("VFX").Find("PulseGlow").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing2Generic.mat").WaitForCompletion();

            //consumeOrb.GetComponent<OrbEffect>().endEffect = Modules.Assets.slowStartPickupEffect;

            Modules.Assets.AddNewEffectDef(consumeOrb);

            consumeOrbMastery = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "RavagerConsumeOrbMasteryEffect", true);
            if (!consumeOrbMastery.GetComponent<NetworkIdentity>()) consumeOrbMastery.AddComponent<NetworkIdentity>();

            trail = consumeOrbMastery.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>();
            trail.widthMultiplier = 0.35f;
            trail.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandparentTeleportOutBoom.mat").WaitForCompletion();

            consumeOrbMastery.transform.Find("VFX").Find("Core").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Loader/matOmniRing2Loader.mat").WaitForCompletion();
            consumeOrbMastery.transform.Find("VFX").localScale = Vector3.one * 0.5f;

            consumeOrbMastery.transform.Find("VFX").Find("Core").localScale = Vector3.one * 4.5f;

            consumeOrbMastery.transform.Find("VFX").Find("PulseGlow").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Grandparent/matGrandParentSunGlow.mat").WaitForCompletion();

            //consumeOrb.GetComponent<OrbEffect>().endEffect = Modules.Assets.slowStartPickupEffect;

            Modules.Assets.AddNewEffectDef(consumeOrbMastery);

            consumeOrbVoid = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "RavagerConsumeOrbVoidEffect", true);
            if (!consumeOrbVoid.GetComponent<NetworkIdentity>()) consumeOrbVoid.AddComponent<NetworkIdentity>();

            trail = consumeOrbVoid.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>();
            trail.widthMultiplier = 0.35f;
            trail.material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidRaidCrab/matVoidRaidCrabTrail2.mat").WaitForCompletion();

            consumeOrbVoid.transform.Find("VFX").Find("Core").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterCore.mat").WaitForCompletion();
            consumeOrbVoid.transform.Find("VFX").localScale = Vector3.one * 0.5f;

            consumeOrbVoid.transform.Find("VFX").Find("Core").localScale = Vector3.one * 7f;

            consumeOrbVoid.transform.Find("VFX").Find("PulseGlow").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFire.mat").WaitForCompletion();

            //consumeOrb.GetComponent<OrbEffect>().endEffect = Modules.Assets.slowStartPickupEffect;

            Modules.Assets.AddNewEffectDef(consumeOrbVoid);
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
            return Assets.CreateMaterial(materialName, emission, Color.white);
        }

        public static Material CreateMaterial(string materialName, float emission, Color emissionColor)
        {
            return Assets.CreateMaterial(materialName, emission, emissionColor, 0f);
        }
    }
}
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

namespace RedGuyMod.Modules
{
    internal static class Assets
    {
        internal static AssetBundle mainAssetBundle;

        internal static Shader hotpoo = Resources.Load<Shader>("Shaders/Deferred/HGStandard");
        internal static Material commandoMat;

        internal static List<EffectDef> effectDefs = new List<EffectDef>();
        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        internal static GameObject swingEffect;
        internal static GameObject bigSwingEffect;
        internal static GameObject slashImpactEffect;

        internal static GameObject swingEffectMastery;
        internal static GameObject bigSwingEffectMastery;
        internal static GameObject slashImpactEffectMastery;

        internal static GameObject leapEffect;
        internal static GameObject leapEffectMastery;

        internal static GameObject slamImpactEffect;
        internal static GameObject bloodBombEffect;
        internal static GameObject cssEffect;

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

        internal static GameObject beamChargeEffect;

        internal static GameObject groundDragEffect;

        internal static NetworkSoundEventDef slashSoundEvent;
        internal static NetworkSoundEventDef bigSlashSoundEvent;
        internal static NetworkSoundEventDef explosionSoundEvent;

        internal static Dictionary<string, GameObject> bloodExplosionOverrides = new Dictionary<string, GameObject>();
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

            swingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("RavagerSwordSwing");
            swingEffect.transform.GetChild(0).gameObject.SetActive(false);
            swingEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();

            swingEffectMastery = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("RavagerSwordSwingMastery");
            swingEffectMastery.transform.GetChild(0).gameObject.SetActive(false);
            swingEffectMastery.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion();

            bigSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("RavagerBigSwordSwing");
            bigSwingEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            var sex = bigSwingEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            bigSwingEffect.transform.GetChild(0).localScale = Vector3.one * 2f;
            MainPlugin.Destroy(bigSwingEffect.GetComponent<EffectComponent>());

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
                    i.wave.amplitude *= 0.25f;
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
            {"NewtBody(Clone)", lunarGolemBloodExplosionEffect },
            {"AffixEarthHealerBody(Clone)", healingBloodExplosionEffect }
        };

            drainTextEffect = CreateTextPopupEffect("RavagerDrainTextEffect", "BLOOD RUSH!");
            drainTextEffect.transform.localScale *= 2.5f;
            drainTextEffect.GetComponentInChildren<TMP_Text>().color = Color.red;

            groundDragEffect = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("prefabs/projectileghosts/sunderghost"), "RavagerGroundDrag");
            MainPlugin.Destroy(groundDragEffect.GetComponent<ProjectileGhostController>());

            beamChargeEffect = PrefabAPI.InstantiateClone(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorChargeMegaBlaster.prefab").WaitForCompletion(), "RavagerBeamCharge");
            MainPlugin.Destroy(beamChargeEffect.GetComponent<ObjectScaleCurve>());

            beamChargeEffect.transform.Find("Base").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorLightningCorrupted.mat").WaitForCompletion();
            beamChargeEffect.transform.Find("OrbCore").GetComponent<MeshRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/MagmaWorm/matMagmaWormExplosionSphere.mat").WaitForCompletion();
            beamChargeEffect.transform.Find("Sparks, Misc").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();
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
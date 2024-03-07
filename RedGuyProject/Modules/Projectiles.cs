using R2API;
using Rewired.ComponentControls.Effects;
using RoR2;
using RoR2.EntityLogic;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.Rendering.PostProcessing;

namespace RedGuyMod.Modules
{
    internal static class Projectiles
    {
        internal static GameObject punchShockwave;
        internal static GameObject punchBarrage;

        internal static void RegisterProjectiles()
        {
            punchShockwave = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderZapCone.prefab").WaitForCompletion().InstantiateClone("RavagerPunchShockwave", true);

            var p = punchShockwave.GetComponent<ProjectileProximityBeamController>();
            p.lightningType = RoR2.Orbs.LightningOrb.LightningType.MageLightning;
            p.damageCoefficient = 1f;

            punchShockwave.transform.Find("Effect/Flash").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matCritImpactShockwave.mat").WaitForCompletion();
            var c = punchShockwave.transform.Find("Effect/Flash").GetComponent<ParticleSystem>().main;
            c.startColor = Color.red;

            punchShockwave.transform.Find("Effect/Impact Shockwave").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniRing1Void.mat").WaitForCompletion();

            punchShockwave.transform.Find("Flash").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpPortalEffect.mat").WaitForCompletion();

            punchShockwave.transform.Find("Effect/Sparks, Single").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();

            punchShockwave.transform.Find("Effect/Lines").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();

            punchShockwave.transform.Find("Effect/Ring").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();

            punchShockwave.transform.Find("Effect/Point Light").GetComponent<Light>().color = Color.red;

            // new shockwave :-D
            /*punchShockwave.GetComponent<ProjectileProximityBeamController>().attackFireCount = 0;

            Content.Components.PunchShockwave shockwave = punchShockwave.AddComponent<Content.Components.PunchShockwave>();
            shockwave.attackFireCount = 12;
            shockwave.attackInterval = 3f;
            shockwave.listClearInterval = 3f;
            shockwave.attackRange = 50f;
            shockwave.minAngleFilter = 0f;
            shockwave.maxAngleFilter = 60f;
            shockwave.procCoefficient = 1f;
            shockwave.damageCoefficient = 1f;
            shockwave.bounces = 0;
            shockwave.inheritDamageType = false;*/
            // ended up biting off way more than i can chew.
            // i don't want to make a new lightning trail, new impact effect, all the materials associated with that. then find out what code
            // even creates the lightning to being with. fuck this i give up

            Modules.Prefabs.projectilePrefabs.Add(punchShockwave);


            punchBarrage = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderZapCone.prefab").WaitForCompletion().InstantiateClone("RavagerPunchBarrage", true);

            MainPlugin.Destroy(punchBarrage.GetComponent<ProjectileProximityBeamController>());
            MainPlugin.Destroy(punchBarrage.GetComponent<DelayedEvent>());
            MainPlugin.Destroy(punchBarrage.GetComponent<StartEvent>());
            punchBarrage.AddComponent<Content.Components.RedGuyPunchBarrage>();

            Modules.Prefabs.projectilePrefabs.Add(punchBarrage);
        }

        private static void InitializeImpactExplosion(ProjectileImpactExplosion projectileImpactExplosion)
        {
            projectileImpactExplosion.blastDamageCoefficient = 1f;
            projectileImpactExplosion.blastProcCoefficient = 1f;
            projectileImpactExplosion.blastRadius = 1f;
            projectileImpactExplosion.bonusBlastForce = Vector3.zero;
            projectileImpactExplosion.childrenCount = 0;
            projectileImpactExplosion.childrenDamageCoefficient = 0f;
            projectileImpactExplosion.childrenProjectilePrefab = null;
            projectileImpactExplosion.destroyOnEnemy = false;
            projectileImpactExplosion.destroyOnWorld = false;
            projectileImpactExplosion.explosionSoundString = "";
            projectileImpactExplosion.falloffModel = RoR2.BlastAttack.FalloffModel.None;
            projectileImpactExplosion.fireChildren = false;
            projectileImpactExplosion.impactEffect = null;
            projectileImpactExplosion.lifetime = 0f;
            projectileImpactExplosion.lifetimeAfterImpact = 0f;
            projectileImpactExplosion.lifetimeExpiredSoundString = "";
            projectileImpactExplosion.lifetimeRandomOffset = 0f;
            projectileImpactExplosion.offsetForLifetimeExpiredSound = 0f;
            projectileImpactExplosion.timerAfterImpact = false;

            projectileImpactExplosion.GetComponent<ProjectileDamage>().damageType = DamageType.Generic;
        }

        private static GameObject CreateGhostPrefab(string ghostName)
        {
            GameObject ghostPrefab = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>(ghostName).InstantiateClone(ghostName);
            ghostPrefab.AddComponent<NetworkIdentity>();
            ghostPrefab.AddComponent<ProjectileGhostController>();

            Modules.Assets.ConvertAllRenderersToHopooShader(ghostPrefab);

            return ghostPrefab;
        }

        private static GameObject CloneProjectilePrefab(string prefabName, string newPrefabName)
        {
            GameObject newPrefab = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Projectiles/" + prefabName), newPrefabName);
            return newPrefab;
        }
    }
}
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RedGuyMod.Content.Components
{
    public class RedGuyCSS : MonoBehaviour
    {
        private void Awake()
        {
            this.Invoke("Bomba", 0.68f);

            if (Modules.Config.badass.Value) Util.PlaySound("sfx_ravager_badass", this.gameObject);
        }

        private void Bomba()
        {
            Util.PlaySound("sfx_ravager_stomp", this.gameObject);

            EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/CharacterLandImpact.prefab").WaitForCompletion(), new EffectData
            {
                origin = this.GetComponentInChildren<ChildLocator>().FindChild("FootR").position + new Vector3(0f, -0.25f, 0f),
                rotation = Quaternion.identity,
                scale = 0.25f
            }, false);
        }
    }
}
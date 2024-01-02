using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RedGuyMod.Content.Components
{
    public class RedGuyCSS : MonoBehaviour
    {
        private void Awake()
        {
            this.Invoke("Bomba", 0.52f);
        }

        private void Bomba()
        {
            Util.PlaySound("sfx_ravager_explosion", this.gameObject);

            EffectManager.SpawnEffect(Modules.Assets.cssEffect, new EffectData
            {
                origin = this.transform.position + (Vector3.up * 1.5f),
                rotation = Quaternion.identity,
                scale = 0.25f
            }, false);
        }
    }
}
using RoR2;
using UnityEngine;

namespace RedGuyMod.Content.Components
{
    public class RavagerOverlayTracker : MonoBehaviour
    {
        public RoR2.TemporaryOverlay overlay;
        public RoR2.CharacterBody body;
        public Transform modelTransform;
        public RedGuyController penis;

        public void FixedUpdate()
        {
            if (this.body)
            {
                if (!this.penis) this.penis = this.body.GetComponent<RedGuyController>();

                if (this.penis)
                {
                    if (!this.penis.draining)
                    {
                        this.Kill();
                    }
                }
            }
        }

        private void Kill()
        {
            if (this.modelTransform)
            {
                TemporaryOverlay temporaryOverlay = this.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                temporaryOverlay.duration = 1f;
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = this.penis.skinDef.bloodRushOverlayMaterial;
                temporaryOverlay.inspectorCharacterModel = this.modelTransform.GetComponent<CharacterModel>();
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.animateShaderAlpha = true;
            }

            UnityEngine.Object.Destroy(this.overlay);
            UnityEngine.Object.Destroy(this);
        }
    }
}
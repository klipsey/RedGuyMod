using UnityEngine;

namespace RedGuyMod.Content.Components
{
    public class RavagerOverlayTracker : MonoBehaviour
    {
        public RoR2.TemporaryOverlay overlay;
        public RoR2.CharacterBody body;
        public RedGuyController penis;

        public void FixedUpdate()
        {
            if (this.body)
            {
                this.penis = this.body.GetComponent<RedGuyController>();

                if (this.penis)
                {
                    if (!this.penis.draining)
                    {
                        UnityEngine.Object.Destroy(this.overlay);
                        UnityEngine.Object.Destroy(this);
                    }
                }
            }
        }
    }
}
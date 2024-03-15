using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RedGuyMod.Content.Components
{
    public class CrosshairClingBar : MonoBehaviour
    {
        public CrosshairController crosshairController { get; set; }

        private RedGuyController penis;
        private Image fillBar;

        private void Awake()
        {
            this.fillBar = this.GetComponent<Image>();
            this.fillBar.fillAmount = 0f;
        }

        private void FixedUpdate()
        {
            if (this.penis)
            {
                if (this.fillBar) this.fillBar.fillAmount = Util.Remap(this.penis.clingTimer, 8f, 0f, 1f, 0f);

                return;
            }

            if (this.crosshairController)
            {
                if (this.crosshairController.hudElement.targetCharacterBody)
                {
                    this.penis = this.crosshairController.hudElement.targetCharacterBody.gameObject.GetComponent<RedGuyController>();
                }
            }
            else
            {
                this.crosshairController = this.GetComponentInParent<CrosshairController>();
            }
        }
    }
}
using RoR2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace RedGuyMod.Content.Components
{
    public class CrosshairChargeBar : MonoBehaviour
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
                if (this.fillBar) this.fillBar.fillAmount = this.penis.chargeValue;

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
using UnityEngine;
using UnityEngine.UI;
using RoR2;
using RoR2.UI;
using TMPro;

namespace RedGuyMod.Content.Components
{
	public class BloodGauge2 : MonoBehaviour
	{
		public HUD targetHUD;
		public RedGuyController target;
		public Image fillBar;

		private PlayerCharacterMasterController playerCharacterMasterController;

		private void Update()
		{
			if (!this.target)
			{
				if (!this.playerCharacterMasterController)
				{
					this.playerCharacterMasterController = (this.targetHUD.targetMaster ? this.targetHUD.targetMaster.GetComponent<PlayerCharacterMasterController>() : null);
				}

				if (this.playerCharacterMasterController && this.playerCharacterMasterController.master.hasBody)
				{
					RedGuyController fuckYou = this.playerCharacterMasterController.master.GetBody().GetComponent<RedGuyController>();
					if (fuckYou) this.SetTarget(fuckYou);
				}
			}
			else
			{
				this.UpdateDisplay();
			}
		}

		public void SetTarget(RedGuyController jhhhh)
		{
			this.target = jhhhh;
		}

		private void UpdateDisplay()
		{
			if (this.fillBar)
			{
				float fill = Util.Remap(this.target.meter, 0f, 100f, 0f, 1f);

				this.fillBar.fillAmount = fill;

				if (this.target.draining)
				{
					this.fillBar.color = new Color(1f, 0f, 46f / 255f);
				}
				else
				{
					this.fillBar.color = new Color(152f / 255f, 12f / 255f, 37f / 255f);
				}
			}
		}
	}
}
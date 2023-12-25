using System;
using RoR2;
using UnityEngine;

namespace RedGuyMod.Content.Components
{
	public class GrabTracker : MonoBehaviour
	{
		public CharacterBody attackerBody;

		private float lifetime;

		private void Awake()
        {
			this.Refresh();
        }

		private void FixedUpdate()
        {
			this.lifetime -= Time.fixedDeltaTime;

			if (this.lifetime <= 0f) Destroy(this);
        }

		public void Refresh()
        {
			this.lifetime = 0.1f;
        }
	}
}
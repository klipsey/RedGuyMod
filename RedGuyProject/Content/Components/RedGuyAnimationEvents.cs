using UnityEngine;
using RoR2;

namespace RedGuyMod.Content.Components
{
    public class RedGuyAnimationEvents : MonoBehaviour
    {
        public void PlaySound(string soundString)
        {
            Util.PlaySound(soundString, this.gameObject);
        }
    }
}
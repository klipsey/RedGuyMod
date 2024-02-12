using RoR2;
using EntityStates;
using UnityEngine;

namespace RedGuyMod.SkillStates
{
    public class ConsumedDeath : GenericCharacterDeath
    {
        public override void OnEnter()
        {
            if (this.modelLocator && this.modelLocator.modelTransform)
            {
                GameObject.Destroy(this.modelLocator.modelTransform.gameObject);
            }

            base.OnEnter();
        }
    }
}
using JetBrains.Annotations;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RedGuyMod.Content
{
    public class RavagerSkillDef : SkillDef
    {
        public Sprite baseIcon;
        public Sprite empoweredIcon;

        public override void OnFixedUpdate([NotNull] GenericSkill skillSlot)
        {
            base.OnFixedUpdate(skillSlot);

            if (skillSlot.characterBody)
            {
                RedGuyMod.Content.Components.RedGuyController penis = skillSlot.characterBody.GetComponent<RedGuyMod.Content.Components.RedGuyController>();
                if (penis)
                {
                    if (!penis.draining) this.icon = this.baseIcon;
                    else this.icon = this.empoweredIcon;
                }
                else this.icon = this.baseIcon;
            }
            else this.icon = this.baseIcon;
        }
    }
}
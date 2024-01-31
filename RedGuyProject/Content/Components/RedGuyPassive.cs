using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RedGuyMod.Content.Components
{
    public class RedGuyPassive : MonoBehaviour
    {
        public SkillDef wallJumpPassive;
        public SkillDef blinkPassive;
        public GenericSkill passiveSkillSlot;

        public SkillDef bloodWellPassive;
        public SkillDef bloodWellAltPassive;
        public GenericSkill bloodPassiveSkillSlot;

        public bool isWallJump
        {
            get
            {
                if (this.wallJumpPassive && this.passiveSkillSlot)
                {
                    return this.passiveSkillSlot.skillDef == this.wallJumpPassive;
                }

                return false;
            }
        }

        public bool isBlink
        {
            get
            {
                if (this.blinkPassive && this.passiveSkillSlot)
                {
                    return this.passiveSkillSlot.skillDef == this.blinkPassive;
                }

                return false;
            }
        }

        public bool isAltBloodWell
        {
            get
            {
                if (this.bloodWellAltPassive && this.bloodPassiveSkillSlot)
                {
                    return this.bloodPassiveSkillSlot.skillDef == this.bloodWellAltPassive;
                }

                return false;
            }
        }
    }
}
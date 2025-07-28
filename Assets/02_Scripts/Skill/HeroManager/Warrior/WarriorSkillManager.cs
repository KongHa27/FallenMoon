using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorSkillManager : SkillManager
{
    protected override SkillBase CreateSkill(SkillData data, Transform caster, SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Skill1:
                return new WarriorSkill1(data, caster);
            case SkillType.Skill2:
                return new WarriorSkill2(data, caster);
            case SkillType.MoveSkill:
                return new WarriorMoveSkill(data, caster);
            default:
                return new BasicSkill(data, caster);
        }
    }
}

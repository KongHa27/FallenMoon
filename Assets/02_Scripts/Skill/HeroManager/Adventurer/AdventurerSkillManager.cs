using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모험가 스킬 매니저
/// </summary>
public class AdventurerSkillManager : SkillManager
{
    protected override SkillBase CreateSkill(SkillData data, Transform caster, SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.Skill1:
                return new AdventurerSkill1(data, caster);

            case SkillType.Skill2:
                return new AdventurerSkill2(data, caster);

            case SkillType.MoveSkill:
                return new AdventurerMoveSkill(data, caster);
            
            default:
                return new BasicSkill(data, caster);
        }
    }
}

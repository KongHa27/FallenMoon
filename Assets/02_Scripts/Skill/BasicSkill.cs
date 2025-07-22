using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 기본 스킬 클래스
/// 특별한 기능 없이 로그만 출력
/// </summary>
public class BasicSkill : SkillBase
{
    public BasicSkill(SkillData data, Transform caster) : base(data, caster)
    {
    }

    protected override void ExecuteSkill()
    {
        Debug.Log("기본 스킬 사용!");
    }
}

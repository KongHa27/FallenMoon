using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//이후 각 캐릭터의 스킬 매니저가 이 스킬 매니저를 상속

/// <summary>
/// 캐릭터의 3개 스킬(스킬 1, 2, 이동 스킬)을 관리하는 매니저 클래스
/// </summary>
public class SkillManager : MonoBehaviour
{
    [Header("----- 스킬 설정 -----")]
    [SerializeField] SkillData _skill1Data;     // 스킬1 데이터 (우클릭)
    [SerializeField] SkillData _skill2Data;     // 스킬2 데이터 (R키)
    [SerializeField] SkillData _moveSkillData;  // 이동스킬 데이터 (좌쉬프트)

    // 스킬 타입을 키로 하는 스킬 딕셔너리
    Dictionary<SkillType, SkillBase> _skills = new Dictionary<SkillType, SkillBase>();

    /// <summary>
    /// 스킬 타입(종류) enum
    /// </summary>
    public enum SkillType
    {
        Skill1,     // 스킬1
        Skill2,     // 스킬2
        MoveSkill   // 이동스킬
    }

    /// <summary>
    /// 스킬 종류에 따른 스킬 쿨다운 변경 이벤트
    /// UI 업데이트 등에 사용
    /// </summary>
    public event System.Action<SkillType, float, float> OnSkillCooldownChanged;

    /// <summary>
    /// 스킬 시스템 초기화
    /// </summary>
    /// <param name="caster">스킬 사용자의 트랜스폼</param>
    public void Initialize(Transform caster)
    {
        // 스킬 인스턴스 생성 (캐릭터별로 다른 스킬 클래스 할당)
        _skills[SkillType.Skill1] = CreateSkill(_skill1Data, caster, SkillType.Skill1);
        _skills[SkillType.Skill2] = CreateSkill(_skill2Data, caster, SkillType.Skill2);
        _skills[SkillType.MoveSkill] = CreateSkill(_moveSkillData, caster, SkillType.MoveSkill);
    }

    /// <summary>
    /// 스킬 생성 함수
    /// 하위 클래스에서 오버라이드하여 캐릭터별 스킬 할당
    /// </summary>
    /// <param name="data">스킬 데이터</param>
    /// <param name="caster">스킬 사용자</param>
    /// <param name="skillType">스킬 타입</param>
    /// <returns>생성된 스킬 인스턴스</returns>
    protected virtual SkillBase CreateSkill(SkillData data, Transform caster, SkillType skillType)
    {
        // 기본 스킬 반환 (하위 클래스에서 오버라이드)
        return new BasicSkill(data, caster);
    }

    /// <summary>
    /// 스킬 사용
    /// </summary>
    /// <param name="skillType">사용할 스킬 타입</param>
    public void UseSkill(SkillType skillType)
    {
        if (_skills.TryGetValue(skillType, out SkillBase skill))
        {
            skill.UseSkill();
        }
    }

    /// <summary>
    /// 스킬 사용 가능 여부 확인
    /// </summary>
    /// <param name="skillType">확인할 스킬 타입</param>
    /// <returns>사용 가능하면 true</returns>
    public bool CanUseSkill(SkillType skillType)
    {
        if (_skills.TryGetValue(skillType, out SkillBase skill))
        {
            return skill.CanUse();
        }
        return false;
    }

    /// <summary>
    /// 스킬 인스턴스 가져오기
    /// </summary>
    /// <param name="skillType">가져올 스킬 타입</param>
    /// <returns>스킬 인스턴스</returns>
    public SkillBase GetSkill(SkillType skillType)
    {
        _skills.TryGetValue(skillType, out SkillBase skill);
        return skill;
    }

    /// <summary>
    /// 매 프레임 스킬 업데이트
    /// </summary>
    private void Update()
    {
        // 모든 스킬의 상태 업데이트
        foreach (var skill in _skills.Values)
        {
            skill.UpdateSkill(Time.deltaTime);
        }

        // 쿨다운 이벤트 발행 (UI 업데이트용)
        foreach (var kvp in _skills)
        {
            var skill = kvp.Value;
            OnSkillCooldownChanged?.Invoke(kvp.Key, skill.State.cooldownTimer, skill.Data.cooldown);
        }
    }
}

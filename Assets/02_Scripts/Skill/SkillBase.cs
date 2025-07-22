using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class SkillBase
{
    protected SkillData _data;
    protected SkillState _state;
    protected Transform _caster;    //스킬 사용자 트랜스폼

    public SkillData Data => _data;
    public SkillState State => _state;

    /// <summary>
    /// 스킬 생성자
    /// </summary>
    /// <param name="data"></param>
    /// <param name="caster"></param>
    public SkillBase(SkillData data, Transform caster)
    {
        _data = data;
        _caster = caster;
        _state = new SkillState();
    }

    /// <summary>
    /// 스킬 사용 가능 여부 확인
    /// (스킬 쿨타임 + 활성화 가능 여부 확인)
    /// </summary>
    /// <returns>사용 가능하면 true 반환</returns>
    public virtual bool CanUse()
    {
        return !_state.isOnCooldown && !_state.isActive;
    }

    /// <summary>
    /// 스킬 사용 가능 여부 확인 후
    /// 실행 및 쿨다운 시작하는 함수
    /// </summary>
    public virtual void UseSkill()
    {
        if (!CanUse())
        {
            Debug.Log("스킬 사용 불가!!");
            return;
        }

        //스킬 실행
        ExecuteSkill();

        //쿨다운 시작
        _state.StartCooldown(_data.cooldown);

        //지속성 스킬이면 활성화 시작
        if (_data.duration > 0)
            _state.StartActive(_data.duration);
    }

    /// <summary>
    /// 각 스킬 별로 구현해야 하는 실제 스킬 실행 함수
    /// 하위 클래스에서 오버라이드
    /// </summary>
    protected abstract void ExecuteSkill();

    /// <summary>
    /// 스킬 업데이트 함수
    /// (쿨다운 및 지속 시간 관리)
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void UpdateSkill(float deltaTime)
    {
        _state.UpdateCooldown(deltaTime);
        _state.UpdateActive(deltaTime);

        //지속성 스킬이 활성화 되면
        if (_state.isActive)
            OnActiveUpdate(deltaTime);
    }

    /// <summary>
    /// 지속성 스킬이 활성화 중일 때 매 프레임마다 호출되는 함수
    /// (스킬의 지속 효과)
    /// </summary>
    /// <param name="deltaTime"></param>
    protected virtual void OnActiveUpdate(float deltaTime) { }

    /// <summary>
    /// 지속성 스킬 지속 시간 종료 시 호출되는 함수
    /// </summary>
    public virtual void OnSkillEnd() { }

    /// <summary>
    /// 이 스킬이 활성화 중일 때 일반 이동 입력을 차단할지 여부
    /// 각 스킬에서 오버라이드하여 설정
    /// </summary>
    public virtual bool BlocksMovementInput => false;

    /// <summary>
    /// 현재 이 스킬이 이동을 차단하고 있는지 확인
    /// </summary>
    public bool IsBlockingMovement => _state.isActive && BlocksMovementInput;
}

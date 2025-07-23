using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 모험가 이동 스킬 "구르기" 클래스
/// </summary>
public class AdventurerMoveSkill : SkillBase
{
    Rigidbody2D _rigid;
    int _originalLayer;
    float _rollSpeed;
    bool _isRolling = false;
    public bool IsRolling => _isRolling;

    public AdventurerMoveSkill(SkillData data, Transform caster) : base(data, caster)
    {
        _rigid = caster.GetComponent<Rigidbody2D>();
        _originalLayer = caster.gameObject.layer;

        // range 값을 구르기 속도로 사용
        _rollSpeed = _data.range;
    }

    protected override void ExecuteSkill()
    {
        Debug.Log($"!!!이동 스킬 \"{_data.skillName}\" 사용!!!");

        // 무적 상태로 변경
        _caster.gameObject.layer = LayerMask.NameToLayer("Invincibility");

        // 구르기 방향 결정
        int direction = GetRollDirection();

        //구르기 상태 변경
        _isRolling = true;

        // 구르기 이동 시작
        StartRolling(direction);

        // 이펙트 생성
        if (_data.effectPrefab != null)
        {
            GameObject effect = Object.Instantiate(_data.effectPrefab, _caster.position, Quaternion.identity);
            Object.Destroy(effect);
        }

        // 사운드 재생
        if (_data.soundClip != null)
        {
            AudioSource.PlayClipAtPoint(_data.soundClip, _caster.position);
        }
    }

    /// <summary>
    /// 구르기 방향을 결정하는 함수
    /// 키보드 입력이 있으면 입력 방향을, 없으면 바라보는 방향을 반환
    /// </summary>
    /// <returns>구르기 방향 (-1: 왼쪽, 1: 오른쪽)</returns>
    private int GetRollDirection()
    {
        // 현재 키보드 입력 확인
        float horizontalInput = Input.GetAxisRaw("Horizontal");

        // 키보드 입력이 있으면 입력 방향 우선
        if (Mathf.Abs(horizontalInput) > 0.1f)
        {
            int inputDirection = horizontalInput > 0 ? 1 : -1;
            Debug.Log($"키보드 입력 방향으로 구르기: {inputDirection}");
            return inputDirection;
        }

        // 키보드 입력이 없으면 현재 바라보는 방향
        SpriteRenderer renderer = _caster.GetComponentInChildren<SpriteRenderer>();
        int faceDirection = renderer.flipX ? -1 : 1;
        Debug.Log($"바라보는 방향으로 구르기: {faceDirection}");
        return faceDirection;
    }

    /// <summary>
    /// 구르기 이동을 시작하는 함수
    /// </summary>
    /// <param name="direction">구르기 방향 (-1: 왼쪽, 1: 오른쪽)</param>
    private void StartRolling(int direction)
    {
        if (_rigid != null)
        {
            // RigidBody2D 설정 확인용 디버그
            Debug.Log($"구르기 시작 - 방향: {direction}, 속도: {_rollSpeed}");
            Debug.Log($"RigidBody 상태 - Drag: {_rigid.drag}, GravityScale: {_rigid.gravityScale}");

            // 현재 속도 확인
            Debug.Log($"구르기 전 속도: {_rigid.velocity}");

            // Y축 속도는 유지하고 X축만 변경 (더 강력한 힘으로)
            Vector2 newVelocity = new Vector2(direction * _rollSpeed, _rigid.velocity.y);
            _rigid.velocity = newVelocity;

            // 속도 적용 후 확인
            Debug.Log($"구르기 후 속도: {_rigid.velocity}");
        }
        else
        {
            Debug.LogError("RigidBody2D가 없음");
        }
    }

    /// <summary>
    /// 구르기 중 매 프레임 호출되는 함수
    /// </summary>
    /// <param name="deltaTime"></param>
    protected override void OnActiveUpdate(float deltaTime)
    {
        if (State.isActive && _state.activeTimer <= 0)
            OnSkillEnd();
    }

    public override void UpdateSkill(float deltaTime)
    {
        bool wasActive = _state.isActive;

        base.UpdateSkill(deltaTime);

        if (wasActive && !_state.isActive)
            OnSkillEnd();
    }

    /// <summary>
    /// 구르기가 끝났을 때 호출되는 함수
    /// </summary>
    public override void OnSkillEnd()
    {
        //이미 종료된 상태라면 중복 실행 방지
        if (!_isRolling) return;

        _isRolling = false;

        // 원래 레이어로 복구 (무적 해제)
        _caster.gameObject.layer = _originalLayer;

        // 이동 속도 완전히 정지
        if (_rigid != null)
        {
            Vector2 velocity = _rigid.velocity;
            velocity.x = 0f;
            _rigid.velocity = velocity;
        }

        Debug.Log("구르기 종료! 무적 상태 해제");
    }

    /// <summary>
    /// 구르기 스킬 사용 가능 여부 체크 (오버라이드)
    /// </summary>
    /// <returns></returns>
    public override bool CanUse()
    {
        bool baseCanUse = base.CanUse();

        return baseCanUse;
    }

    /// <summary>
    /// 구르기 중에는 일반 이동 차단
    /// </summary>
    public override bool BlocksMovementInput => true;
}

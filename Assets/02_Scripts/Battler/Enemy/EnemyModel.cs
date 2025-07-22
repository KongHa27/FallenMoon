using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EnemyModel : BattlerModel
{
    [Header("----- 설정 데이터 -----")]
    [SerializeField] EnemyData _data;

    [Header("----- 런타임 데이터 -----")]
    [SerializeField] bool _isElite;
    [SerializeField] float _expReward;
    [SerializeField] int _goldReward;
    [SerializeField] int _erosionLevel;

    public bool IsElite => _isElite;
    public float MoveSpeed => _moveSpeed;
    public float ExpReward => _expReward;
    public int GoldReward => _goldReward;
    public EnemyData Data => _data;

    // 엘리트 상태 변화 이벤트
    public event Action<bool> OnEliteStatusChanged;

    /// <summary>
    /// 적 초기화 (침식도 레벨에 따라 스탯 설정)
    /// </summary>
    /// <param name="level">현재 침식도 레벨</param>
    /// <param name="forceElite">강제로 엘리트로 만들지 여부</param>
    public void Initialize(int level, bool forceElite = false)
    {
        _erosionLevel = level;

        // 엘리트 여부 결정
        if (forceElite)
            _isElite = true;
        else if (_data.CanBeElite)
            _isElite = UnityEngine.Random.Range(0f, 1f) < GetEliteChance(level);
        else
            _isElite = false;

        // 스탯 설정
        _maxHp = _data.GetMaxHp(level, _isElite);
        _curHp = _maxHp;
        _damage = _data.GetDamage(level, _isElite) * DifficultyManager.Instance.GetEnemyDamageMultiplier();
        _amor = _data.GetAmor(_isElite);
        _moveSpeed = _data.MoveSpeed;
        _expReward = _data.GetExpReward(level, _isElite);
        _goldReward = _data.GetGoldReward(level, _isElite);

        // 이벤트 발행
        OnEliteStatusChanged?.Invoke(_isElite);
        HpChangeEvent();
    }

    /// <summary>
    /// 침식도 레벨에 따른 엘리트 확률 계산
    /// </summary>
    /// <param name="level">침식도 레벨</param>
    /// <returns>엘리트 확률 (0~1)</returns>
    private float GetEliteChance(int level)
    {
        //7레벨 부터 확률 존재
        if (level < 7) return 0f;

        //레벨이 높아질수록 엘리트 확률 증가 (기본 1% + 레벨당 1%, 최대 30%)
        float baseChance = 0.01f;
        float levelBonus = (level - 6) * 0.01f;

        if (level >= 30)
            return Mathf.Clamp01(0.3f);
        else
            return Mathf.Clamp01(baseChance + levelBonus);
    }

    /// <summary>
    /// 엘리트 여부 확인 (읽기 전용)
    /// </summary>
    /// <returns></returns>
    public bool CheckIsElite()
    {
        return _isElite;
    }

    /// <summary>
    /// 적이 죽었을 때 경험치 보상 반환
    /// </summary>
    /// <returns>경험치 보상량</returns>
    public float GetExpRewardOnDeath()
    {
        return _expReward;
    }

    /// <summary>
    /// 적이 죽었을 때 골드 보상 반환
    /// </summary>
    /// <returns>골드 보상량</returns>
    public int GetGoldRewardOnDeath()
    {
        return _goldReward;
    }
}

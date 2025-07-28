using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeroModel : BattlerModel
{
    [Header("----- 설정 데이터 -----")]
    [SerializeField] HeroData _data;

    [Header("----- 런타임 데이터 -----")]
    [SerializeField] float _hpRegen;
    [SerializeField] float _maxExp;
    [SerializeField] float _curExp;
    [SerializeField] int _level;
    [SerializeField] int _gold;


    //이동 속력 변경 이벤트
    public event Action<float> OnSpeedChanged;
    //점프 파워 변경 이벤트
    public event Action<float> OnPowerChanged;
    //경험치 변경 이벤트
    public event Action<float, float> OnExpChanged;
    //레벨 변경 이벤트
    public event Action<int, int> OnLevelChanged;
    //골드 변경 이벤트
    public event Action<int> OnGoldChanged;

    public int Gold => _gold;
    public float Amor => _amor;
    public float MoveSpeed => _moveSpeed;

    private void Start()
    {
        _hpBar = GameObject.Find("HpBar").GetComponent<Image>();
    }

    public void SetHeroData(HeroData heroData)
    {
        if (heroData != null)
        {
            _data = heroData;
            Debug.Log($"HeroData 설정 완료: {heroData.name}");
        }
        else
            Debug.LogWarning("전달받은 HeroData가 null입니다. 기본 데이터를 사용합니다.");
    }

    /// <summary>
    /// 런타임 데이터 초기화
    /// </summary>
    public void Initialize()
    {
        _maxHp = _data.MaxHp;
        _curHp = _maxHp;
        _hpRegen = _data.HpRegen;

        _moveSpeed = _data.MoveSpeed;
        _jumpPower = _data.JumpPower;

        _damage = _data.Damage;
        _amor = _data.Amor;

        _level = 1;
        _curExp = 0;
        _maxExp = _data.GetMaxExp(_level);
        _gold = 0;

        StartCoroutine(RegenHpCoroutine());

        HpChangeEvent();
        OnSpeedChanged?.Invoke(_moveSpeed);
        OnPowerChanged?.Invoke(_jumpPower);
        OnExpChanged?.Invoke(_curExp, _maxExp);
        OnLevelChanged?.Invoke(_level, _level);
        OnGoldChanged?.Invoke(_gold);
    }


    /// <summary>
    /// 기본 체력 재생 코루틴
    /// </summary>
    /// <returns></returns>
    private IEnumerator RegenHpCoroutine()
    {
        while (true)
        {
            if (_curHp < _maxHp)
            {
                _curHp = Mathf.Min(_curHp + _hpRegen * Time.deltaTime, _maxHp);
                HpChangeEvent();
            }
            yield return null;
        }
    }

    public void TakeHitByDarkness()
    {
        //체력에 대미지 적용
        _curHp -= (float)(_maxHp*0.08);

        //체력 변경 이벤트 발행
        HpChangeEvent();

        //사망 시 사망 이벤트 발행
        if (_curHp <= 0)
            DeadEvent();
    }


    #region 경험치, 레벨업, 골드
    /// <summary>
    /// 경험치 획득 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddExp(float amount)
    {
        int curLevel = _level;

        _curExp += amount;

        while (_curExp >= _maxExp)
        {
            _curExp -= _maxExp;
            LevelUp();
            _maxExp = _data.GetMaxExp(_level);
        }

        OnExpChanged?.Invoke(_curExp, _maxExp);

        if (curLevel != _level)
        {
            OnLevelChanged?.Invoke(curLevel, _level);
        }
    }

    /// <summary>
    /// 레벨 업 하면 호출되는 함수
    /// </summary>
    public void LevelUp()
    {
        _level++;

        _maxHp += _data.HpIncrementRate;
        _curHp += _data.HpIncrementRate;
        _hpRegen += _data.HpRegenIncrementRate;

        _damage += _data.DamageIncrementRate;
        _amor += _data.AmorIncrementRate;
    }

    /// <summary>
    /// 골드 획득 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddGold(int amount)
    {
        if (amount <= 0) return;

        _gold += amount;

        OnGoldChanged?.Invoke(_gold);
    }

    /// <summary>
    /// 골드 소비 함수
    /// </summary>
    /// <param name="amount">소비할 골드 양</param>
    /// <returns>소비 성공 여부</returns>
    public bool TryToSpendGold(int amount)
    {
        if (amount <= 0 || _gold < amount) return false;

        _gold -= amount;

        OnGoldChanged?.Invoke(_gold);
        return true;
    }
    #endregion


    #region 추가 스탯 적용 (최대 체력, 힐, 이동 속도
    /// <summary>
    /// 추가 체력을 적용하는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddMaxHp(float amount)
    {
        _maxHp += amount;
        _curHp += amount;

        HpChangeEvent();
    }

    /// <summary>
    /// 추가 체력 재생
    /// </summary>
    /// <param name="amount"></param>
    public void AddHpRegen(float amount)
    {
        _hpRegen += amount;
    }

    /// <summary>
    /// 힐 함수
    /// </summary>
    /// <param name="amount"></param>
    public void Heal(float amount)
    {
        if (_curHp >= _maxHp) return;

        _curHp = Mathf.Min(_curHp + amount, _maxHp);

        HpChangeEvent();
    }

    /// <summary>
    /// 추가 이동 속도를 적용하는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddMoveSpeed(float amount)
    {
        _moveSpeed += amount;

        OnSpeedChanged?.Invoke(_moveSpeed);
    }

    public void AddAmor(float amount)
    {
        _amor += amount;
    }

    /// <summary>
    /// 추가 점프력을 적용하는 함수
    /// </summary>
    /// <param name="amount"></param>
    public void AddJumpPower(float amount)
    {
        _jumpPower += amount;
        OnPowerChanged?.Invoke(_jumpPower);
    }
    #endregion
}

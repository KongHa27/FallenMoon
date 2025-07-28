using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "GameSettings/EnemyData", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("----- 기본 정보 -----")]
    [SerializeField] string _name;              //적 이름
    [SerializeField] bool _canBeElite = true;   //엘리트 적이 될 수 있는지 여부

    [Header("----- 기본 스탯 -----")]
    [SerializeField] float _baseMaxHp;      //최대 체력
    [SerializeField] float _baseDamage;     //공격력
    [SerializeField] float _amor;           //방어력
    [SerializeField] float _moveSpeed;      //이동속도
    [SerializeField] float _baseExpReward;  //보상 경험치
    [SerializeField] int _baseGoldReward;   //보상 골드

    [Header("----- 레벨별 증가율 (%) -----")]
    [SerializeField] float _hpIncreaseRate = 0.3f;           //체력 +30%
    [SerializeField] float _damageIncreaseRate = 0.2f;       //공격력 +20%
    [SerializeField] float _expRewardIncreaseRate = 0.2f;    //경험치 보상 +20%
    [SerializeField] float _goldRewardIncreaseRate = 0.2f;   //골드 보상 +20%

    [Header("----- 엘리트 배수 -----")]
    [SerializeField] float _eliteHpMultiplier = 2f;
    [SerializeField] float _eliteDamageMultiplier = 1.5f;
    [SerializeField] float _eliteAmorMultiplier = 1.5f;
    [SerializeField] float _eliteExpMultiplier = 3f;
    [SerializeField] float _eliteGoldMultiplier = 2f;

    public string Name => _name;
    public float Amor => _amor;
    public float MoveSpeed => _moveSpeed;
    public bool CanBeElite => _canBeElite;

    /// <summary>
    /// 침식도 레벨에 따른 적의 최대 체력 계산
    /// </summary>
    public float GetMaxHp(int level, bool isElite = false)
    {
        float hp = _baseMaxHp * (1f + _hpIncreaseRate * level);
        return isElite ? hp * _eliteHpMultiplier : hp;
    }

    /// <summary>
    /// 침식도 레벨에 따른 적의 공격력 계산
    /// </summary>
    public float GetDamage(int level, bool isElite = false)
    {
        float damage = _baseDamage * (1f + _damageIncreaseRate * level);

        return isElite ? damage * _eliteDamageMultiplier : damage;
    }

    /// <summary>
    /// 엘리트 여부에 따른 적의 방어력 계산
    /// </summary>
    public float GetAmor(bool isElite = false)
    {
        return isElite ? _amor * _eliteAmorMultiplier : _amor;
    }

    /// <summary>
    /// 침식도 레벨에 따른 적의 경험치 보상 계산
    /// </summary>
    public float GetExpReward(int level, bool isElite = false)
    {
        float exp = _baseExpReward * (1f + _expRewardIncreaseRate * level);
        return isElite ? exp * _eliteExpMultiplier : exp;
    }

    /// <summary>
    /// 침식도 레벨에 따른 적의 골드 보상 계산
    /// </summary>
    public int GetGoldReward(int level, bool isElite = false)
    {
        int gold = (int)(_baseGoldReward * (1f + _goldRewardIncreaseRate * level));
        return isElite ? (int)(gold * _eliteGoldMultiplier) : gold;
    }
}

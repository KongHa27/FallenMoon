using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GameSettings/HeroData", fileName = "HeroData")]
public class HeroData : ScriptableObject
{
    [Header("----- 정보 -----")]
    [SerializeField] string _name;

    [Header("----- 체력 -----")]
    [SerializeField] float _maxHp;
    [SerializeField] float _hpIncrementRate;        //체력 증가
    [SerializeField] float _hpRegen;                //체력 재생 (초당)
    [SerializeField] float _hpRegenIncrementRate;   //체력 재생 증가

    [Header("----- 이동 -----")]
    [SerializeField] float _moveSpeed;
    [SerializeField] float _jumpPower;

    [Header("----- 전투 -----")]
    [SerializeField] float _damage;
    [SerializeField] float _damageIncrementRate;    //공격력 증가
    [SerializeField] float _amor;
    [SerializeField] float _amorIncrementRate;      //방어력 증가

    [Header("----- 경험치 -----")]
    [SerializeField] float _baseExp;
    [SerializeField] float _expIncrementRate;       //경험치 배수

    public float MaxHp => _maxHp;
    public float HpIncrementRate => _hpIncrementRate;
    public float HpRegen => _hpRegen;
    public float HpRegenIncrementRate => _hpRegenIncrementRate;
    public float MoveSpeed => _moveSpeed;
    public float JumpPower => _jumpPower;
    public float Damage => _damage;
    public float DamageIncrementRate => _damageIncrementRate;
    public float Amor => _amor;
    public float AmorIncrementRate => _amorIncrementRate;

    public float GetMaxExp(int level)
    {
        if (level <= 0)
            return _baseExp;

        return _baseExp * Mathf.Pow(_expIncrementRate, level);
    }
}

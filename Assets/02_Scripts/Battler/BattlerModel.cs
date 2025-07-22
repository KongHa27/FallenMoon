using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlerModel : MonoBehaviour, IDamageable, IAttackable
{
    [Header("----- 이동 스탯 -----")]
    [SerializeField] protected float _moveSpeed;
    [SerializeField] protected float _jumpPower;

    [Header("----- 체력 스탯 -----")]
    [SerializeField] protected float _maxHp;
    [SerializeField] protected float _curHp;

    [Header("----- 공격 스탯 -----")]
    [SerializeField] protected float _damage;
    [SerializeField] protected float _amor;

    [Header("----- 캔버스 뷰 -----")]
    [SerializeField] Image _hpBar;

    public event Action OnAttacked;
    public event Action<float, float> OnHpChanged;
    public event Action OnDead;


    public void Attack(IDamageable damageable)
    {
        damageable.TakeHit(_damage);
    }

    public void TakeHit(float damage)
    {
        //방어력 적용
        damage = Mathf.Max(damage - _amor, 0);

        //체력에 대미지 적용
        _curHp = Mathf.Min(_curHp - damage, _maxHp);

        //피격 이벤트 발행
        OnAttacked?.Invoke();
        //체력 변경 이벤트 발행
        HpChangeEvent();

        //사망 시 사망 이벤트 발행
        if (_curHp <= 0)
        {
            _curHp = 0;
            DeadEvent();
        }
            
    }

    public void HpChangeEvent()
    {
        OnHpChanged?.Invoke(_curHp, _maxHp);
        _hpBar.fillAmount = _curHp / _maxHp;
    }

    public void DeadEvent()
    {
        OnDead?.Invoke();
    }

    /// <summary>
    /// 현재 공격력 반환
    /// </summary>
    public float GetDamage()
    {
        return _damage;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorSkill2 : SkillBase
{
    private const int STRIKE_COUNT = 6;
    private const float DAMAGE_MULTIPLIER = 1.1f;
    private const float STRIKE_INTERVAL = 0.2f;

    private IDamageable _target;
    private int _currentStrike;
    private float _strikeTimer;
    private int _originalLayer;
    private LayerMask _enemyLayerMask = -1;

    public WarriorSkill2(SkillData data, Transform caster) : base(data, caster)
    {
    }

    protected override void ExecuteSkill()
    {
        Debug.Log("Warrior Skill 2: 연속 공격!");

        // 가장 가까운 적 찾기
        _target = FindNearestEnemy();

        if (_target == null)
        {
            Debug.Log("공격할 적이 없습니다!");
            return;
        }

        // 무적 상태로 변경
        GameObject casterObject = _caster.gameObject;
        _originalLayer = casterObject.layer;
        casterObject.layer = LayerMask.NameToLayer("Invincibility");

        // 연속 공격 시작
        _currentStrike = 0;
        _strikeTimer = 0f;

        Debug.Log("무적 상태 활성화!");
    }

    protected override void OnActiveUpdate(float deltaTime)
    {
        if (_target == null) return;

        _strikeTimer += deltaTime;

        // 공격 간격마다 타격
        if (_strikeTimer >= STRIKE_INTERVAL && _currentStrike < STRIKE_COUNT)
        {
            PerformStrike();
            _strikeTimer = 0f;
            _currentStrike++;
        }
    }

    public override void OnSkillEnd()
    {
        // 무적 상태 해제
        if (_caster != null)
        {
            _caster.gameObject.layer = _originalLayer;
            Debug.Log("무적 상태 해제!");
        }

        _target = null;
        _currentStrike = 0;
    }

    private IDamageable FindNearestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_caster.position, _data.range, _enemyLayerMask);

        IDamageable nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            if (collider.transform == _caster) continue;

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float distance = Vector2.Distance(_caster.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = damageable;
                }
            }
        }

        return nearestEnemy;
    }

    private void PerformStrike()
    {
        BattlerModel casterModel = _caster.GetComponent<BattlerModel>();
        if (casterModel != null && _target != null)
        {
            float damage = casterModel.GetDamage() * DAMAGE_MULTIPLIER;
            _target.TakeHit(damage);

            // 타격 이펙트
            if (_data.effectPrefab != null)
            {
                MonoBehaviour targetMono = _target as MonoBehaviour;
                if (targetMono != null)
                {
                    GameObject effect = Object.Instantiate(_data.effectPrefab, targetMono.transform.position, Quaternion.identity);
                    Object.Destroy(effect, 0.5f);
                }
            }

            Debug.Log($"연속 공격 {_currentStrike + 1}/{STRIKE_COUNT}: {damage} 데미지!");
        }
    }
}

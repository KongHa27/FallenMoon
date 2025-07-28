using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorAttack : AttackSystem
{
    [Header("----- 근거리 공격 설정 -----")]
    [SerializeField] private float _attackRange = 2f;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private LayerMask _enemyLayerMask = -1;

    [Header("----- 공격 데미지 배율 -----")]
    [SerializeField] private float _normalAttackMultiplier = 1.2f;  // 120%
    [SerializeField] private float _criticalAttackMultiplier = 2.4f; // 240%
    [SerializeField] private int _criticalAttackInterval = 3;        // 매 3번째 공격

    [Header("----- 시각적 효과 -----")]
    [SerializeField] private GameObject _attackEffectPrefab;
    [SerializeField] private float _effectDuration = 0.2f;

    private float _lastAttackTime;
    private int _attackCount = 0;
    private bool _isCriticalAttack = false;

    public override void Initialize(BattlerModel model, Transform transform)
    {
        base.Initialize(model, transform);
        _lastAttackTime = -_attackCooldown; // 시작 시 즉시 공격 가능하도록
    }

    public override void PerformAttack(IDamageable target = null)
    {
        if (!CanAttack()) return;

        _lastAttackTime = Time.time;
        _attackCount++;

        // 매 3번째 공격인지 확인
        _isCriticalAttack = (_attackCount % _criticalAttackInterval == 0);

        // 공격 범위 내의 적들을 찾아서 공격
        IDamageable targetToDamage = target ?? FindNearestEnemy();

        if (targetToDamage != null)
        {
            AttackTarget(targetToDamage);
        }

        // 공격 이펙트 표시
        ShowAttackEffect();

        Debug.Log($"근거리 공격 실행! 공격 횟수: {_attackCount}, 크리티컬: {_isCriticalAttack}");
    }

    public override bool CanAttack()
    {
        return Time.time >= _lastAttackTime + _attackCooldown;
    }

    public override float GetAttackRange()
    {
        return _attackRange;
    }

    /// <summary>
    /// 가장 가까운 적을 찾는 함수
    /// </summary>
    /// <returns>가장 가까운 IDamageable 적</returns>
    private IDamageable FindNearestEnemy()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_transform.position, _attackRange, _enemyLayerMask);

        IDamageable nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            // 자기 자신은 제외
            if (collider.transform == _transform) continue;

            IDamageable damageable = collider.GetComponent<IDamageable>();
            if (damageable != null)
            {
                float distance = Vector2.Distance(_transform.position, collider.transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = damageable;
                }
            }
        }

        return nearestEnemy;
    }

    /// <summary>
    /// 특정 타겟을 공격하는 함수
    /// </summary>
    /// <param name="target">공격할 대상</param>
    private void AttackTarget(IDamageable target)
    {
        if (_model == null) return;

        float baseDamage = _model.GetDamage();
        float damageMultiplier = _isCriticalAttack ? _criticalAttackMultiplier : _normalAttackMultiplier;
        float finalDamage = baseDamage * damageMultiplier;

        target.TakeHit(finalDamage);

        Debug.Log($"적에게 {finalDamage} 데미지 ({damageMultiplier * 100}%)");
    }

    /// <summary>
    /// 공격 이펙트를 표시하는 함수
    /// </summary>
    private void ShowAttackEffect()
    {
        if (_attackEffectPrefab != null)
        {
            GameObject effect = Instantiate(_attackEffectPrefab, _transform.position, _transform.rotation);

            // 크리티컬 공격일 때 이펙트 크기 증가
            if (_isCriticalAttack)
            {
                effect.transform.localScale *= 1.5f;
            }

            Destroy(effect, _effectDuration);
        }
    }

    /// <summary>
    /// 공격 범위를 시각적으로 표시 (디버그용)
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_transform == null) return;

        Gizmos.color = _isCriticalAttack ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(_transform.position, _attackRange);
    }

    /// <summary>
    /// 현재 공격 횟수를 반환
    /// </summary>
    public int GetAttackCount()
    {
        return _attackCount;
    }

    /// <summary>
    /// 다음 공격이 크리티컬인지 확인
    /// </summary>
    public bool IsNextAttackCritical()
    {
        return (_attackCount + 1) % _criticalAttackInterval == 0;
    }

    /// <summary>
    /// 공격 횟수 초기화 (필요시 사용)
    /// </summary>
    public void ResetAttackCount()
    {
        _attackCount = 0;
    }
}

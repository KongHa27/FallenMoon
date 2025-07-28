using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/// <summary>
/// 보스 몬스터 클래스
/// </summary>
public class Boss : Enemy
{
    [Header("----- 보스 설정 -----")]
    [SerializeField] BossData _bossData;
    [SerializeField] Transform _patternPoint;    //패턴(지면 강타) 포인트

    //마법진 시스템 용 - 보스 사망 이벤트
    public event Action OnBossDead;

    //패턴 관리
    float[] _patternCooldowns;
    bool _isUsingPattern = false;
    Coroutine _patternRoutine;

    protected override void Start()
    {
        base.Start();

        //보스 데이터로 초기화
        if (Model.Data is BossData bossData)
        {
            _bossData = bossData;
            InitializeBossPatterns();
        }
        else
            Debug.LogError("Boss에 BossData가 존재하지 않습니다!");
    }

    /// <summary>
    /// 보스 패턴 초기화
    /// </summary>
    void InitializeBossPatterns()
    {
        if (_bossData?.Patterns == null) return;

        _patternCooldowns = new float[_bossData.Patterns.Length];

        // 패턴 시작 딜레이 후 패턴 루틴 시작
        StartCoroutine(StartPatternAfterDelay());
    }

    /// <summary>
    /// 딜레이 후 패턴 시작
    /// </summary>
    IEnumerator StartPatternAfterDelay()
    {
        yield return new WaitForSeconds(_bossData.PatternStartDelay);
        _patternRoutine = StartCoroutine(PatternUpdateRoutine());
    }

    /// <summary>
    /// 패턴 업데이트 루틴
    /// </summary>
    IEnumerator PatternUpdateRoutine()
    {
        while (!_isDead)
        {
            if (!_isUsingPattern && _target != null)
            {
                TryUsePattern();
            }

            // 쿨다운 업데이트
            UpdatePatternCooldowns();

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 패턴 쿨다운 업데이트
    /// </summary>
    void UpdatePatternCooldowns()
    {
        for (int i = 0; i < _patternCooldowns.Length; i++)
        {
            if (_patternCooldowns[i] > 0)
                _patternCooldowns[i] -= 0.1f;
        }
    }

    /// <summary>
    /// 패턴 사용 시도
    /// </summary>
    void TryUsePattern()
    {
        if (_bossData?.Patterns == null || _target == null) return;

        for (int i = 0; i < _bossData.Patterns.Length; i++)
        {
            BossPattern pattern = _bossData.Patterns[i];

            // 쿨다운 체크
            if (_patternCooldowns[i] <= 0)
            {
                float distanceToTarget = Vector2.Distance(transform.position, _target.position);

                // 거리에 따른 패턴 선택
                bool canUsePattern = false;

                switch (pattern.patternType)
                {
                    case BossPatternType.RockThrow:
                        canUsePattern = distanceToTarget >= 3f && distanceToTarget <= pattern.range;
                        break;
                    case BossPatternType.GroundSlam:
                        canUsePattern = distanceToTarget <= pattern.range;
                        break;
                }

                if (canUsePattern)
                {
                    StartCoroutine(UsePattern(pattern, i));
                    return;
                }
            }
        }
    }

    /// <summary>
    /// 패턴 사용
    /// </summary>
    IEnumerator UsePattern(BossPattern pattern, int patternIndex)
    {
        _isUsingPattern = true;
        _patternCooldowns[patternIndex] = pattern.cooldown;

        Debug.Log($"보스 패턴 사용: {pattern.patternName}");

        // 캐스팅 시간
        yield return new WaitForSeconds(pattern.castTime);

        // 패턴 실행
        switch (pattern.patternType)
        {
            case BossPatternType.RockThrow:
                yield return StartCoroutine(ExecuteRockThrow(pattern));
                break;
            case BossPatternType.GroundSlam:
                yield return StartCoroutine(ExecuteGroundSlam(pattern));
                break;
        }

        _isUsingPattern = false;
    }

    /// <summary>
    /// 암석 투척 패턴
    /// </summary>
    IEnumerator ExecuteRockThrow(BossPattern pattern)
    {
        if (_target == null || pattern.projectilePrefab == null) yield break;

        Vector3 targetPos = _target.position;
        Vector3 throwDirection = (targetPos - transform.position).normalized;

        // 투사체 생성
        GameObject projectile = Instantiate(pattern.projectilePrefab, transform.position + throwDirection, Quaternion.identity);

        // 투사체에 BossProjectile 컴포넌트 추가
        BossProjectile projectileScript = projectile.GetComponent<BossProjectile>();
        if (projectileScript == null)
            projectileScript = projectile.AddComponent<BossProjectile>();

        projectileScript.Initialize(targetPos, pattern.damage, 5f); // 속도 5

        yield return null;
    }

    /// <summary>
    /// 지면 강타 패턴
    /// </summary>
    IEnumerator ExecuteGroundSlam(BossPattern pattern)
    {
        // 경고 이펙트 생성 (선택사항)
        if (pattern.effectPrefab != null)
        {
            GameObject warningEffect = Instantiate(pattern.effectPrefab, _patternPoint != null ? _patternPoint.position : transform.position, Quaternion.identity);
            Destroy(warningEffect, 1f);
        }

        // 경고 시간
        yield return new WaitForSeconds(0.5f);

        // 범위 내의 모든 타겟에게 데미지
        Collider2D[] targets = Physics2D.OverlapCircleAll(
            _patternPoint != null ? _patternPoint.position : transform.position,
            pattern.range,
            _targetLayer
        );

        foreach (Collider2D target in targets)
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable != null)
            {
                damageable.TakeHit(pattern.damage);
                Debug.Log($"지면 강타로 {target.name}에게 {pattern.damage} 데미지!");
            }
        }

        yield return null;
    }

    /// <summary>
    /// 보스 사망 처리 (기존 OnDead를 오버라이드)
    /// </summary>
    protected override void OnDead()
    {
        // 패턴 루틴 정지
        if (_patternRoutine != null)
        {
            StopCoroutine(_patternRoutine);
            _patternRoutine = null;
        }

        // 보스 사망 이벤트 발행 (마법진 시스템용)
        OnBossDead?.Invoke();

        // 기존 사망 처리
        base.OnDead();
    }

    // 디버그용 기즈모
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (_bossData?.Patterns != null)
        {
            // 패턴 범위 표시
            foreach (BossPattern pattern in _bossData.Patterns)
            {
                switch (pattern.patternType)
                {
                    case BossPatternType.RockThrow:
                        Gizmos.color = Color.blue;
                        Gizmos.DrawWireSphere(transform.position, pattern.range);
                        break;
                    case BossPatternType.GroundSlam:
                        Gizmos.color = Color.red;
                        Vector3 slamPos = _patternPoint != null ? _patternPoint.position : transform.position;
                        Gizmos.DrawWireSphere(slamPos, pattern.range);
                        break;
                }
            }
        }
    }
}
